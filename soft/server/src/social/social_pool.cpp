#include "social_pool.h"
#include "social_message.h"
#include "social_dhc.h"

void SocialPool::init()
{
	social_lists_ = 0;
	update_social_lists_count_ = 0;
	dump_save_count_ = 0;
	Request *req = new Request();
	req->add(opc_query, MAKE_GUID(et_social_list, 0), new dhc::social_list_t);
	DB_SOCIAL->upcall(req, boost::bind(&SocialPool::init_callback, this, _1));
}

void SocialPool::init_callback(Request *req)
{
	if (req->result() < 0)
	{
		social_lists_ = new dhc::social_list_t();
		social_lists_->set_gold_refresh_time(service::timer()->now());
		social_lists_->set_guid(MAKE_GUID(et_social_list, 0));
		save_social_list(social_lists_, true);
	}
	else
	{
		social_lists_ = (dhc::social_list_t*)req->release_data();
	}
}

void SocialPool::fini()
{
	for (SocialMap::iterator it = socials_.begin();
		it != socials_.end();
		++it)
	{
		SocialList& sl = it->second;
		for (SocialList::iterator jt = sl.begin();
			jt != sl.end();
			++jt)
		{
			save_social(jt->second);
		}
	}
	save_social_list(social_lists_);
}

void SocialPool::update()
{
	uint64_t now_time = service::timer()->now();
	int count = 0;
	dhc::social_t *social = 0;
	do
	{
		if (update_.empty())
		{
			break;
		}

		UpdateSocial us = update_.top();
		if (now_time < us.time)
		{
			break;
		}
		update_.pop();

		if (us.target_guid == 0)
		{
			if (save_player_social(us.player_guid))
			{
				add_update(us.player_guid, 0, service::timer()->now() + SC_UPDATE_TIME);
				break;
			}
		}
		else
		{
			social = get_social(us.player_guid, us.target_guid);
			if (social && social->stype() == ST_APPLY)
			{
				delete_social(social);
				SocialMessage::push_soical_delete(us.player_guid, us.target_guid);
			}
		}
		
		count++;
	} while (count < 50);


	if (dump_save_count_ >= 200)
	{
		service::log()->debug("save count is:<%d>\n", dump_save_count_);
	}

	count = 0;
	std::set<uint64_t> has_save;
	for (std::map<uint64_t, std::set<uint64_t> >::iterator it = save_lists_.begin();
		it != save_lists_.end();
		++it)
	{
		if (count >= 10)
		{
			break;
		}
		for (std::set<uint64_t>::iterator jt = it->second.begin();
			jt != it->second.end();
			++jt)
		{
			social = get_social(it->first, *jt);
			if (social)
			{
				save_social(social);
				dump_save_count_--;
			}
		}
		count++;
		has_save.insert(it->first);
	}

	for (std::set<uint64_t>::const_iterator it = has_save.begin();
		it != has_save.end();
		++it)
	{
		save_lists_.erase(*it);
	}

	if (save_lists_.empty())
	{
		dump_save_count_ = 0;
	}

	if (social_lists_)
	{
		if (service::timer()->trigger_time(social_lists_->gold_refresh_time(), 0, 0))
		{
			social_lists_->set_gold_refresh_time(now_time);
			social_lists_->clear_player_bgift_guids();
			social_lists_->clear_player_bgift_nums();
		}

		update_social_lists_count_++;
		if (update_social_lists_count_ >= 5)
		{
			update_social_lists_count_ = 0;
			save_social_list(social_lists_);
		}
	}
}


void SocialPool::load(uint64_t player_guid, LoaderCallback cb)
{
	Request *req = new Request();
	protocol::game::social_list_player_load* playerload = new protocol::game::social_list_player_load();
	playerload->set_player_guid(player_guid);
	req->add(opc_query, MAKE_GUID(et_player, 0), playerload);
	DB_SOCIAL->upcall(req, boost::bind(&SocialPool::load_callback, this, _1, player_guid, cb));
}

void SocialPool::load_callback(Request *req, uint64_t player_guid, LoaderCallback cb)
{
	if (req->result() >= 0)
	{
		protocol::game::social_list_player_load* social_lists = (protocol::game::social_list_player_load*)req->data();
		for (int i = 0; i < social_lists->socials_size(); ++i)
		{
			dhc::social_t *new_social = new dhc::social_t();
			new_social->CopyFrom(social_lists->socials(i));
			socials_[new_social->player_guid()][new_social->target_guid()] = new_social;

			if (is_online(new_social->target_guid()))
			{
				new_social->set_sflag(new_social->sflag() | SF_ONLINE);
			}
			else
			{
				new_social->set_sflag(new_social->sflag() & (~SF_ONLINE));
			}
			if (is_fight(new_social->target_guid()))
			{
				new_social->set_sflag(new_social->sflag() | SF_FIGHT);
			}
			else
			{
				new_social->set_sflag(new_social->sflag() & (~SF_FIGHT));
			}
			
			if (new_social->stype() == ST_APPLY)
			{
				add_update(new_social->player_guid(), new_social->target_guid(), new_social->ttime());
			}
		}
	}
	
	add_update(player_guid, 0, service::timer()->now() + SC_UPDATE_TIME);

	cb();
}

void SocialPool::create_social(const dhc::social_t &social)
{
	if (get_social(social.player_guid(), social.target_guid()))
	{
		service::log()->error("create social has exist: p<%lld>, t<%lld>, t<%d>\n", social.player_guid(), social.target_guid(), social.stype());
		return;
	}
	dhc::social_t *new_social = new dhc::social_t();
	new_social->CopyFrom(social);
	new_social->set_guid(DB_GTOOL->assign(et_social));

	if (social.stype() == ST_FRIEND &&
		is_online(social.target_guid()))
	{
		new_social->set_sflag(SF_ONLINE);
		new_social->set_ttime(service::timer()->now());
	}
	socials_[new_social->player_guid()][new_social->target_guid()] = new_social;
	save_social(new_social, true);

	if (new_social->stype() == ST_APPLY)
	{
		add_update(new_social->player_guid(), new_social->target_guid(), new_social->ttime());
	}
}

void SocialPool::delete_social(dhc::social_t *social)
{
	SocialMap::iterator it = socials_.find(social->player_guid());
	if (it != socials_.end())
	{
		it->second.erase(social->target_guid());
		if (it->second.empty())
		{
			socials_.erase(it);
		}
	}

	Request *req = new Request();
	dhc::social_t *save_social = new dhc::social_t();
	save_social->CopyFrom(*social);
	req->add(opc_remove, save_social->guid(), save_social);
	DB_SOCIAL->upcall(req, 0);

	delete social;
}

void SocialPool::delete_social(uint64_t player_guid, uint64_t target_guid)
{
	Request *req = new Request();
	dhc::social_t *cmsg = new dhc::social_t;
	cmsg->set_player_guid(player_guid);
	cmsg->set_target_guid(target_guid);
	req->add(opc_remove, MAKE_GUID(et_post, 0), cmsg);
	DB_SOCIAL->upcall(req, 0);
}

void SocialPool::delete_social(uint64_t player_guid, int type)
{
	std::vector<dhc::social_t*> sos;
	SocialMap::iterator it = socials_.find(player_guid);
	if (it != socials_.end())
	{
		for (SocialList::iterator jt = it->second.begin();
			jt != it->second.end();
			++jt)
		{
			if (jt->second->stype() == type)
			{
				sos.push_back(jt->second);
			}
		}
	}

	for (int i = 0; i < sos.size(); ++i)
	{
		delete_social(sos[i]);
	}
}

void SocialPool::update_social(dhc::social_t *social)
{
	dump_save_count_++;
	save_lists_[social->player_guid()].insert(social->target_guid());
}

dhc::social_t* SocialPool::get_social(uint64_t player_guid, uint64_t target_guid)
{
	SocialMap::iterator it = socials_.find(player_guid);
	if (it == socials_.end())
	{
		return 0;
	}
	SocialList::iterator jt = it->second.find(target_guid);
	if (jt == it->second.end())
	{
		return 0;
	}
	return jt->second;
}


std::map<uint64_t, dhc::social_t*>& SocialPool::get_socials(uint64_t player_guid)
{
	SocialMap::iterator it = socials_.find(player_guid);
	if (it == socials_.end())
	{
		return dumy_social_list_;
	}

	return it->second;
}

bool SocialPool::has_social(uint64_t player_guid) const
{
	return social_loads_.find(player_guid) != social_loads_.end();
}

void SocialPool::save_social(dhc::social_t *social, bool is_new)
{
	if (social->changed())
	{
		social->clear_changed();
		opcmd_t opt = opc_update;
		if (is_new)
		{
			opt = opc_insert;
		}
		Request *req = new Request();
		dhc::social_t *cmsg = new dhc::social_t;
		cmsg->CopyFrom(*social);
		req->add(opt, cmsg->guid(), cmsg);
		DB_SOCIAL->upcall(req, 0);
	}
}

void SocialPool::save_social_list(dhc::social_list_t *social, bool is_new /* = false */)
{
	if (social->changed())
	{
		social->clear_changed();
		opcmd_t opt = opc_update;
		if (is_new)
		{
			opt = opc_insert;
		}
		Request *req = new Request();
		dhc::social_list_t *cmsg = new dhc::social_list_t;
		cmsg->CopyFrom(*social);
		req->add(opt, cmsg->guid(), cmsg);
		DB_SOCIAL->upcall(req, 0);
	}
}

bool SocialPool::save_player_social(uint64_t player_guid)
{
	if (social_loads_flag_.find(player_guid) != social_loads_flag_.end())
	{
		social_loads_flag_.erase(player_guid);
		return true;
	}

	SocialMap::iterator it = socials_.find(player_guid);
	if (it != socials_.end())
	{
		for (SocialList::iterator jt = it->second.begin();
			jt != it->second.end();
			++jt)
		{
			save_social(jt->second);
			delete jt->second;
		}
	}
	socials_.erase(player_guid);
	social_loads_.erase(player_guid);
	social_loads_flag_.erase(player_guid);
	return false;
}

void SocialPool::add_update(uint64_t player_guid, uint64_t target_guid, uint64_t time)
{
	UpdateSocial us;
	us.player_guid = player_guid;
	us.target_guid = target_guid;
	us.time = time;
	update_.push(us);

	if (target_guid == 0)
	{
		social_loads_.insert(player_guid);
	}
}

int SocialPool::get_apply_num(uint64_t player_guid) const
{
	int count = 0;
	SocialMap::const_iterator it = socials_.find(player_guid);
	if (it != socials_.end())
	{
		for (SocialList::const_iterator jt = it->second.begin();
			jt != it->second.end();
			++jt)
		{
			if (jt->second->stype() == ST_APPLY)
			{
				count += 1;
			}
		}
	}
	return count;
}

int SocialPool::get_and_set_gold(uint64_t player_guid)
{
	int gold = 0;
	SocialMap::iterator it = socials_.find(player_guid);
	if (it != socials_.end())
	{
		for (SocialList::iterator jt = it->second.begin();
			jt != it->second.end();
			++jt)
		{
			if (jt->second->stype() == ST_FRIEND)
			{
				gold += jt->second->gold();
				jt->second->set_gold(0);
				update_social(jt->second);
			}
		}
	}
	return gold;
}

int SocialPool::get_gold(uint64_t player_guid) const
{
	int gold = 0;
	SocialMap::const_iterator it = socials_.find(player_guid);
	if (it != socials_.end())
	{
		for (SocialList::const_iterator jt = it->second.begin();
			jt != it->second.end();
			++jt)
		{
			if (jt->second->stype() == ST_FRIEND)
			{
				gold += jt->second->gold();
			}
		}
	}
	return gold;
}

void SocialPool::set_reject(uint64_t player_guid)
{
	int index = -1;
	for (int i = 0; i < social_lists_->player_guids_size(); ++i)
	{
		if (social_lists_->player_guids(i) == player_guid)
		{
			index = i;
			break;
		}
	}
	if (index == -1)
	{
		social_lists_->add_player_guids(player_guid);
	}
	else
	{
		social_lists_->mutable_player_guids()->SwapElements(index, social_lists_->player_guids_size() - 1);
		social_lists_->mutable_player_guids()->RemoveLast();
	}
	save_social_list(social_lists_);
}

bool SocialPool::is_reject(uint64_t player_guid) const
{
	for (int i = 0; i < social_lists_->player_guids_size(); ++i)
	{
		if (social_lists_->player_guids(i) == player_guid)
		{
			return true;
		}
	}
	return false;
}

bool SocialPool::is_online(uint64_t player_guid) const
{
	return onlines_.find(player_guid) != onlines_.end();
}

void SocialPool::set_online(uint64_t player_guid, bool online)
{
	if (online)
	{
		onlines_.insert(player_guid);
		social_loads_flag_.insert(player_guid);
	}
	else
	{
		onlines_.erase(player_guid);
	}
}

bool SocialPool::is_fight(uint64_t player_guid) const
{
	return fights_.find(player_guid) != fights_.end();
}

void SocialPool::set_fight(uint64_t player_guid, int fight)
{
	if (fight)
	{
		fights_.insert(player_guid);
	}
	else
	{
		fights_.erase(player_guid);
	}
}

int SocialPool::get_gold_receive_num(uint64_t player_guid) const
{
	for (int i = 0; i < social_lists_->player_bgift_guids_size(); ++i)
	{
		if (social_lists_->player_bgift_guids(i) == player_guid)
		{
			return social_lists_->player_bgift_nums(i);
		}
	}
	return 0;
	
}

void SocialPool::set_gold_receive_num(uint64_t player_guid)
{
	int index = -1;
	for (int i = 0; i < social_lists_->player_bgift_guids_size(); ++i)
	{
		if (social_lists_->player_bgift_guids(i) == player_guid)
		{
			index = i;
			break;
		}
	}
	if (index == -1)
	{
		social_lists_->add_player_bgift_guids(player_guid);
		social_lists_->add_player_bgift_nums(1);
	}
	else
	{
		social_lists_->set_player_bgift_nums(index, social_lists_->player_bgift_nums(index) + 1);
	}
}



