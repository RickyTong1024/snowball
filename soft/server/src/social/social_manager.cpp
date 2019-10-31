#include "social_manager.h"
#include "social_pool.h"
#include "social_message.h"
#include "social_dhc.h"

SocialManager::SocialManager()
{

}

SocialManager::~SocialManager()
{

}

int SocialManager::init()
{
	sSocialDhc->init();
	timer_id_ = service::timer()->schedule(boost::bind(&SocialManager::update, this, _1), 2000, "social");
	if (timer_id_ == -1)
	{
		return -1;
	}
	sSocialPool->init();
	return 0;
}

int SocialManager::fini()
{
	sSocialPool->fini();
	if (timer_id_ != -1)
	{
		service::timer()->cancel(timer_id_);
		timer_id_ = -1;
	}
	sSocialDhc->fini();
	return 0;
}

int SocialManager::update(const ACE_Time_Value & cur_time)
{
	sSocialPool->update();
	return 0;
}

int SocialManager::req_hall_login(Packet *pck, const std::string &name, int id)
{
	protocol::game::req_social_login msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	sSocialPool->set_online(msg.player_guid(), true);
	
	dhc::social_t social;
	social.set_player_guid(msg.player_guid());
	social.set_name(msg.name());
	social.set_cup(msg.cup());
	social.set_avatar(msg.avatar());
	social.set_toukuang(msg.toukuang());
	social.set_region_id(msg.region_id());
	social.set_level(msg.level());
	social.set_name_color(msg.name_color());
	social.set_achieve_point(msg.achieve_point());
	social.set_max_score(msg.max_score());
	social.set_max_sha(msg.max_sha());
	social.set_max_lsha(msg.max_lsha());

	if (!sSocialPool->has_social(msg.player_guid()))
	{
		sSocialPool->load(msg.player_guid(), boost::bind(&SocialManager::req_hall_login_callback, this, social, name, id));
		return 0;
	}

	return req_hall_login_callback(social, name, id);
}


int SocialManager::req_hall_login_callback(const dhc::social_t& login_social, const std::string& name, int id)
{
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);

	std::map<int, std::vector<std::pair<uint64_t, int> > > channel_guids;
	dhc::social_t *social = 0;

	protocol::game::smsg_social_data smsg;
	smsg.set_reject(sSocialPool->is_reject(login_social.player_guid()));

	std::map<uint64_t, dhc::social_t*>& socials =
		sSocialPool->get_socials(login_social.player_guid());

	for (std::map<uint64_t, dhc::social_t*>::iterator it = socials.begin();
		it != socials.end();
		++it)
	{
		if (it->second->stype() == ST_APPLY)
		{
			smsg.set_apply_num(smsg.apply_num() + 1);
		}
		else if (it->second->stype() == ST_FRIEND)
		{
			smsg.add_friend_guids(it->second->target_guid());
			smsg.set_social_gold(smsg.social_gold() + it->second->gold());
			if (it->second->msgs_size() > 0)
			{
				smsg.set_msg_num(smsg.msg_num() + it->second->msgs_size());
			}
		}
		else if (it->second->stype() == ST_BLACK)
		{
			smsg.add_black_guids(it->second->target_guid());
		}
		
		social = sSocialPool->get_social(it->second->target_guid(), it->second->player_guid());
		if (social && social->stype() == ST_FRIEND)
		{
			social->set_sflag(social->sflag() | SF_ONLINE);
			social->set_sflag(social->sflag() & (~SF_FIGHT));
			social->set_ttime(service::timer()->now());
			social->set_name(login_social.name());
			social->set_cup(login_social.cup());
			social->set_avatar(login_social.avatar());
			social->set_toukuang(login_social.toukuang());
			social->set_region_id(login_social.region_id());
			social->set_level(login_social.level());
			social->set_name_color(login_social.name_color());
			social->set_achieve_point(login_social.achieve_point());
			social->set_max_score(login_social.max_score());
			social->set_max_sha(login_social.max_sha());
			social->set_max_lsha(login_social.max_lsha());
			sSocialPool->update_social(social);
			channel_guids[it->second->target_guid() % names.size()].push_back(std::make_pair(it->second->target_guid(), social->sflag()));
		}
	}

	SocialMessage::rep_social_login(smsg, name, id);
	SocialMessage::push_social_login(login_social.player_guid(), login_social.name(), channel_guids);

	return 0;
}

int SocialManager::terminal_push_hall_logout(Packet *pck, const std::string &name)
{
	protocol::game::req_social_login msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	sSocialPool->set_online(msg.player_guid(), false);

	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);

	std::map<int, std::vector<std::pair<uint64_t, int> > > channel_guids;
	dhc::social_t *social = 0;

	std::map<uint64_t, dhc::social_t*>& socials = sSocialPool->get_socials(msg.player_guid());
	for (std::map<uint64_t, dhc::social_t*>::iterator it = socials.begin();
		it != socials.end();
		++it)
	{
		social = sSocialPool->get_social(it->second->target_guid(), it->second->player_guid());
		if (social && social->stype() == ST_FRIEND)
		{
			social->set_sflag(social->sflag() & (~SF_ONLINE));
			social->set_sflag(social->sflag() & (~SF_FIGHT));
			social->set_name(msg.name());
			social->set_cup(msg.cup());
			social->set_avatar(msg.avatar());
			social->set_toukuang(msg.toukuang());
			social->set_region_id(msg.region_id());
			social->set_level(msg.level());
			social->set_name_color(msg.name_color());
			social->set_achieve_point(msg.achieve_point());
			social->set_max_score(msg.max_score());
			social->set_max_sha(msg.max_sha());
			social->set_max_lsha(msg.max_lsha());
			sSocialPool->update_social(social);
			channel_guids[it->second->target_guid() % names.size()].push_back(std::make_pair(it->second->target_guid(), social->sflag()));
		}
	}

	SocialMessage::push_social_logout(msg.player_guid(), channel_guids);

	return 0;
}

int SocialManager::req_hall_look(Packet *pck, const std::string &name, int id)
{
	protocol::game::req_social_look msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.player_guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.player_guid());
		sSocialPool->load(msg.player_guid(), boost::bind(&SocialManager::look_hall_callback, this, msg, name, id));
		return 0;
	}

	look_hall_callback(msg, name, id);

	return 0;
}

void SocialManager::look_hall_callback(const protocol::game::req_social_look &msg, const std::string &name, int id)
{
	protocol::game::smsg_social_look smsg;
	std::map<uint64_t, dhc::social_t*>& socials =
		sSocialPool->get_socials(msg.player_guid());

	for (std::map<uint64_t, dhc::social_t*>::iterator it = socials.begin();
		it != socials.end();
		++it)
	{
		if (it->second->stype() == msg.type())
		{
			dhc::social_t *tsocial = it->second;
			if (sSocialPool->is_fight(tsocial->target_guid()))
			{
				tsocial->set_sflag(tsocial->sflag() | SF_FIGHT);
			}
			else
			{
				tsocial->set_sflag(tsocial->sflag() & (~SF_FIGHT));
			}
			smsg.add_socials()->CopyFrom(*tsocial);

			if (it->second->msgs_size() > 0)
			{
				it->second->clear_msgs();
				it->second->clear_msgtimes();
				sSocialPool->update_social(it->second);
			}
		}
	}

	SocialMessage::rep_soical_look(smsg, 0, name, id);
}

int SocialManager::req_hall_delete(Packet *pck, const std::string &name, int id)
{
	protocol::game::req_social_delete msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.player_guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.player_guid());
		SocialMessage::rep_social_delete(name, id, ERROR_SYSTEM);
		return -1;
	}

	if (msg.target_guid() == 0)
	{
		sSocialPool->delete_social(msg.player_guid(), ST_BLACK);
	}
	else
	{
		dhc::social_t *social = sSocialPool->get_social(msg.player_guid(), msg.target_guid());
		if (!social)
		{
			SocialMessage::rep_social_delete(name, id, ERROR_SYSTEM);
			return -1;
		}
		if (social->stype() == ST_FRIEND)
		{
			dhc::social_t *tsocial = sSocialPool->get_social(msg.target_guid(), msg.player_guid());
			if (tsocial)
			{
				sSocialPool->delete_social(tsocial);
				SocialMessage::push_soical_delete(msg.target_guid(), msg.player_guid(), sSocialPool->get_gold(msg.target_guid()), true, true);
			}
			else
			{
				sSocialPool->delete_social(msg.target_guid(), msg.player_guid());
			}
			SocialMessage::push_soical_delete(msg.player_guid(), msg.target_guid(), sSocialPool->get_gold(msg.player_guid()), true, false);
		}
		sSocialPool->delete_social(social);
	}

	SocialMessage::rep_social_delete(name, id, 0);

	return 0;
}


int SocialManager::req_hall_add(Packet *pck, const std::string &name, int id)
{
	protocol::game::rmsg_social_apply msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.social().target_guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.social().target_guid());
		SocialMessage::rep_social_add(dhc::social_t(), ERROR_SYSTEM, name, id);
		return -1;
	}

	if (!sSocialPool->has_social(msg.social().player_guid()))
	{
		sSocialPool->load(msg.social().player_guid(), boost::bind(&SocialManager::add_hall_callback, this, msg, name, id));
		return 0;
	}

	add_hall_callback(msg, name, id);

	return 0;
}

void SocialManager::add_hall_callback(const protocol::game::rmsg_social_apply &msg, const std::string &name, int id)
{
	/// 添加人
	dhc::social_t *social = sSocialPool->get_social(msg.social().target_guid(), msg.social().player_guid());
	if (!social)
	{
		SocialMessage::rep_social_add(dhc::social_t(), ERROR_SYSTEM, name, id);
		return;
	}

	if (social->stype() == ST_FRIEND)
	{
		SocialMessage::rep_social_add(dhc::social_t(), ERROR_SOCIAL_FRIEND, name, id);
		return;
	}

	if (msg.social().gold() == 1)
	{
		sSocialPool->delete_social(social);
		SocialMessage::rep_social_add(dhc::social_t(), 0, name, id);
		return;
	}
	int type = 0;
	/// 被添加人
	dhc::social_t *bsocial = sSocialPool->get_social(msg.social().player_guid(), msg.social().target_guid());
	if (bsocial)
	{
		type = bsocial->stype();
		if (bsocial->stype() == ST_FRIEND)
		{
			SocialMessage::rep_social_add(dhc::social_t(), ERROR_SOCIAL_FRIEND, name, id);
			return;
		}
		bsocial->set_stype(ST_FRIEND);
		bsocial->set_sflag(SF_ONLINE);
		bsocial->set_ttime(service::timer()->now());
		sSocialPool->update_social(bsocial);
	}
	else
	{
		sSocialPool->create_social(msg.social());
	}

	social->set_stype(ST_FRIEND);
	social->set_ttime(service::timer()->now());
	if (sSocialPool->is_online(social->target_guid()))
	{
		social->set_sflag(social->sflag() | SF_ONLINE);
	}
	else
	{
		social->set_sflag(social->sflag() & (~SF_ONLINE));
	}
	if (sSocialPool->is_fight(social->target_guid()))
	{
		social->set_sflag(social->sflag() | SF_FIGHT);
	}
	else
	{
		social->set_sflag(social->sflag() & (~SF_FIGHT));
	}
	sSocialPool->update_social(social);

	SocialMessage::rep_social_add(*social, 0, name, id);
	SocialMessage::push_social_add(msg.social(), type);
}

int SocialManager::req_hall_apply(Packet *pck, const std::string &name, int id)
{
	protocol::game::rmsg_social_apply msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.social().target_guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.social().target_guid());
		SocialMessage::rep_social_apply(ERROR_SYSTEM, name, id);
		return -1;
	}

	if (!sSocialPool->has_social(msg.social().player_guid()))
	{
		sSocialPool->load(msg.social().player_guid(), boost::bind(&SocialManager::load_apply_callback, this, msg, name, id));
		return 0;
	}

	load_apply_callback(msg, name, id);

	return 0;
}

void SocialManager::load_apply_callback(const protocol::game::rmsg_social_apply &msg, const std::string &name, int id)
{
	/// 被申请的人
	if (sSocialPool->is_reject(msg.social().player_guid()))
	{
		SocialMessage::rep_social_apply(ERROR_SOCIAL_BLACK, name, id);
		return;
	}
	dhc::social_t *bsocial = sSocialPool->get_social(msg.social().player_guid(), msg.social().target_guid());
	if (bsocial && bsocial->stype() == ST_APPLY)
	{
		SocialMessage::rep_social_apply(ERROR_SOCIAL_APPLY, name, id);
		return;
	}
	if (bsocial && bsocial->stype() == ST_BLACK)
	{
		SocialMessage::rep_social_apply(ERROR_SOCIAL_BLACK, name, id);
		return;
	}
	if (bsocial && bsocial->stype() == ST_FRIEND)
	{
		SocialMessage::rep_social_apply(ERROR_SOCIAL_FRIEND, name, id);
		return;
	}

	/// 申请人
	dhc::social_t *social = sSocialPool->get_social(msg.social().target_guid(), msg.social().player_guid());
	if (social && social->stype() == ST_FRIEND)
	{
		SocialMessage::rep_social_apply(ERROR_SOCIAL_FRIEND, name, id);
		return;
	}
	if (social && social->stype() == ST_BLACK)
	{
		sSocialPool->delete_social(social);
	}
	if (social && social->stype() == ST_APPLY)
	{

	}

	sSocialPool->create_social(msg.social());

	SocialMessage::push_social_apply(msg.social().player_guid(), sSocialPool->get_apply_num(msg.social().player_guid()));
	SocialMessage::rep_social_apply(0, name, id);
}

int SocialManager::req_social_black(Packet *pck, const std::string &name, int id)
{
	protocol::game::rmsg_social_black msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.target_guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.target_guid());
		SocialMessage::rep_social_black(dhc::social_t(), ERROR_SYSTEM, 0, name, id);
		return -1;
	}

	/// 屏蔽的人
	dhc::social_t *social = sSocialPool->get_social(msg.target_guid(), msg.player_guid());
	if (social)
	{
		if (social->stype() == ST_BLACK)
		{
			SocialMessage::rep_social_black(*social, ERROR_SYSTEM, ST_BLACK, name, id);
			return -1;
		}
		if (social->stype() == ST_APPLY)
		{
			social->set_stype(ST_BLACK);
			sSocialPool->update_social(social);

			SocialMessage::rep_social_black(*social, 0, ST_APPLY, name, id);
			return 0;
		}
		if(social->stype() == ST_FRIEND)
		{
			social->set_stype(ST_BLACK);
			social->set_gold(0);
			social->clear_msgs();
			social->clear_msgtimes();
			sSocialPool->update_social(social);

			/// 删除被屏蔽的人
			dhc::social_t *tsocial = sSocialPool->get_social(msg.player_guid(), msg.target_guid());
			if (tsocial)
			{
				sSocialPool->delete_social(tsocial);
			}
			else
			{
				sSocialPool->delete_social(msg.player_guid(), msg.target_guid());
			}
			SocialMessage::rep_social_black(*social, 0, ST_FRIEND, name, id);
			SocialMessage::push_soical_delete(msg.player_guid(), msg.target_guid(), sSocialPool->get_gold(msg.player_guid()), true, true);
			SocialMessage::push_soical_delete(msg.target_guid(), msg.player_guid(), sSocialPool->get_gold(msg.target_guid()), true, false);
		}
		return 0;
	}

	/// 请求被屏蔽人信息
	SocialMessage::req_social_black(msg.player_guid(), msg.target_guid(), boost::bind(&SocialManager::req_social_black_callback, this, _1, _2, name, id));

	return 0;
}

int SocialManager::req_social_black_callback(Packet *pck, int error_code, const std::string &name, int id)
{
	protocol::game::rmsg_social_apply msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (error_code > 0)
	{
		SocialMessage::rep_social_black(msg.social(), error_code, 0, name, id);
		return 0;
	}

	sSocialPool->create_social(msg.social());

	SocialMessage::rep_social_black(msg.social(), 0, 0, name, id);

	return 0;
}

int SocialManager::terminal_push_hall_gift(Packet *pck, const std::string &name)
{
	protocol::game::pmsg_social_gift msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.target_guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.target_guid());
		return -1;
	}

	/// 赠送的人
	dhc::social_t *social = sSocialPool->get_social(msg.target_guid(), msg.player_guid());
	if (!social)
	{
		return -1;
	}

	if (social->stype() != ST_FRIEND)
	{
		return -1;
	}

	/// 被赠送的人
	dhc::social_t *bsocial = sSocialPool->get_social(msg.player_guid(), msg.target_guid());
	if (!bsocial)
	{
		return -1;
	}

	if (bsocial->stype() != ST_FRIEND)
	{
		return -1;
	}

	bsocial->set_gold(msg.gold());
	sSocialPool->update_social(bsocial);

	if (social->sflag() & SF_ONLINE)
	{
		SocialMessage::push_social_gift(msg.player_guid(), msg.target_guid(), msg.gold());
	}

	return 0;
}

int SocialManager::terminal_push_hall_chat(Packet *pck, const std::string &name)
{
	protocol::game::pmsg_social_chat msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.guid());
		return -1;
	}

	/// 发送的人
	dhc::social_t *social = sSocialPool->get_social(msg.guid(), msg.player_guid());
	if (!social)
	{
		return -1;
	}

	if (social->stype() != ST_FRIEND)
	{
		return -1;
	}

	/// 被发送的人
	dhc::social_t *bsocial = sSocialPool->get_social(msg.player_guid(), msg.guid());
	if (!bsocial)
	{
		return -1;
	}

	if (bsocial->stype() != ST_FRIEND)
	{
		return -1;
	}

	if (social->sflag() & SF_ONLINE)
	{
		SocialMessage::push_social_chat(bsocial, msg.text());
	}
	else
	{
		if (bsocial->msgs_size() < 50)
		{
			bsocial->add_msgs(msg.text());
			bsocial->add_msgtimes(service::timer()->now());
			sSocialPool->update_social(bsocial);
		}
	}
	return 0;
}

int SocialManager::req_hall_gold(Packet *pck, const std::string &name, int id)
{
	protocol::game::req_social_gold msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.player_guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.player_guid());
		SocialMessage::rep_soical_gold(msg.player_guid(), 0, name, id, ERROR_SYSTEM);
		return -1;
	}

	int gold = sSocialPool->get_and_set_gold(msg.player_guid());

	SocialMessage::rep_soical_gold(msg.player_guid(), gold, name, id, 0);

	return 0;
}

int SocialManager::req_hall_reject(Packet *pck, const std::string &name, int id)
{
	protocol::game::rmsg_social_reject msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.player_guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.player_guid());
		SocialMessage::rep_social_reject(name, id, ERROR_SYSTEM);
		return -1;
	}

	sSocialPool->set_reject(msg.player_guid());
	SocialMessage::rep_social_reject(name, id, 0);
	return 0;
}

int SocialManager::req_hall_gift(Packet *pck, const std::string &name, int id)
{
	protocol::game::pmsg_social_gift msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (!sSocialPool->has_social(msg.target_guid()))
	{
		service::log()->debug("social service wrong, p<%lld>\n", msg.target_guid());
		SocialMessage::rep_social_gift(ERROR_SYSTEM, name, id);
		return -1;
	}

	if (!sSocialPool->has_social(msg.player_guid()))
	{
		sSocialPool->load(msg.player_guid(), boost::bind(&SocialManager::load_gift_callback, this, msg, name, id));
		return 0;
	}

	load_gift_callback(msg, name, id);
	return 0;
}

void SocialManager::load_gift_callback(const protocol::game::pmsg_social_gift &msg, const std::string &name, int id)
{
	/// 赠送的人
	dhc::social_t *social = sSocialPool->get_social(msg.target_guid(), msg.player_guid());
	if (!social)
	{
		SocialMessage::rep_social_gift(ERROR_SYSTEM, name, id);
		return;
	}

	if (social->stype() != ST_FRIEND)
	{
		SocialMessage::rep_social_gift(ERROR_SYSTEM, name, id);
		return;
	}

	/// 被赠送的人
	dhc::social_t *bsocial = sSocialPool->get_social(msg.player_guid(), msg.target_guid());
	if (!bsocial)
	{
		SocialMessage::rep_social_gift(ERROR_SYSTEM, name, id);
		return;
	}

	if (bsocial->stype() != ST_FRIEND)
	{
		SocialMessage::rep_social_gift(ERROR_SYSTEM, name, id);
		return;
	}

	if (sSocialPool->get_gold_receive_num(msg.player_guid()) >= 10)
	{
		SocialMessage::rep_social_gift(ERROR_SOCIAL_GIFT_NUM, name, id);
		return;
	}

	bsocial->set_gold(msg.gold());
	sSocialPool->update_social(bsocial);
	sSocialPool->set_gold_receive_num(msg.player_guid());

	if (social->sflag() & SF_ONLINE)
	{
		SocialMessage::push_social_gift(msg.player_guid(), msg.target_guid(), msg.gold());
	}

	SocialMessage::rep_social_gift(0, name, id);
}

int SocialManager::terminal_push_hall_fight(Packet *pck, const std::string &name)
{
	protocol::game::push_social_fight msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	sSocialPool->set_fight(msg.player_guid(), msg.fight());
	return 0;
}
