#include "rank_manager.h"
#include "rank_message.h"
#include "rank_dhc.h"

#define RANK_TIME 60000
#define MAX_RANK_NUM 100

RankManager::RankManager()
	: timer_id_(-1)
{

}

RankManager::~RankManager()
{

}

int RankManager::init()
{
	sRankDhc->init();
	load();
	timer_id_ = service::timer()->schedule(boost::bind(&RankManager::update, this, _1), RANK_TIME, "rank");
	if (-1 == timer_id_)
	{
		return -1;
	}
	return 0;
}

int RankManager::fini()
{
	if (-1 != timer_id_)
	{
		service::timer()->cancel(timer_id_);
		timer_id_ = -1;
	}
	sRankDhc->fini();
	save_all(true);
	return 0;
}

void RankManager::load()
{
	for (int i = rt_cup; i < rt_end; ++i)
	{
		dhc::rank_t *rank = new dhc::rank_t;
		Request *req = new Request();
		req->add(opc_query, MAKE_GUID(et_rank, i), rank);
		DB_RANK->upcall(req, boost::bind(&RankManager::load_callback, this, _1, i));
	}
}

void RankManager::load_callback(Request *req, int id)
{
	dhc::rank_t *rank = 0;
	if (req->result() < 0)
	{
		rank = new dhc::rank_t();
		rank->set_guid(MAKE_GUID(et_rank, id));
		ranks_[id] = rank;
		save(id, true);
	}
	else
	{
		rank = (dhc::rank_t*)req->release_data();
		ranks_[id] = rank;
	}
	while (rank->name_size() < rank->player_guid_size())
	{
		rank->add_name("");
	}
	while (rank->sex_size() < rank->player_guid_size())
	{
		rank->add_sex(0);
	}
	while (rank->level_size() < rank->player_guid_size())
	{
		rank->add_level(1);
	}
	while (rank->avatar_size() < rank->player_guid_size())
	{
		rank->add_avatar(0);
	}
	while (rank->toukuang_size() < rank->player_guid_size())
	{
		rank->add_toukuang(0);
	}
	while (rank->region_id_size() < rank->player_guid_size())
	{
		rank->add_region_id(0);
	}
	while (rank->name_color_size() < rank->player_guid_size())
	{
		rank->add_name_color(0);
	}
	while (rank->value_size() < rank->player_guid_size())
	{
		rank->add_value(0);
	}
}

void RankManager::save(int id, bool is_new, bool release)
{
	if (ranks_.find(id) == ranks_.end())
	{
		return;
	}
	dhc::rank_t * rank = ranks_[id];
	if (rank->changed())
	{
		rank->clear_changed();
		opcmd_t opt = opc_update;
		if (is_new)
		{
			opt = opc_insert;
		}
		Request *req = new Request();
		dhc::rank_t *cmsg = new dhc::rank_t;
		cmsg->CopyFrom(*rank);
		req->add(opt, cmsg->guid(), cmsg);
		DB_RANK->upcall(req, 0);
	}
	if (release)
	{
		delete rank;
		ranks_.erase(id);
	}
}

void RankManager::save_all(bool release)
{
	for (int i = rt_cup; i < rt_end; ++i)
	{
		save(i, false, release);
	}
}

int RankManager::push_hall_rank_update(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_rank_update msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	int id = msg.id();
	int value = msg.value();
	if (ranks_.find(id) == ranks_.end())
	{
		return -1;
	}
	dhc::rank_t *rank = ranks_[id];
	int index = -1;
	for (int i = 0; i < rank->player_guid_size(); ++i)
	{
		if (rank->player_guid(i) == msg.player_guid())
		{
			if (rank->value(i) == value)
			{
				rank->set_name(i, msg.name());
				rank->set_sex(i, msg.sex());
				rank->set_level(i, msg.level());
				rank->set_avatar(i, msg.avatar());
				rank->set_toukuang(i, msg.toukuang());
				rank->set_region_id(i, msg.region_id());
				rank->set_name_color(i, msg.name_color());
				return -1;
			}
			index = i;
			break;
		}
	}
	if (index != -1)
	{
		for (int i = index; i < rank->player_guid_size() - 1; ++i)
		{
			rank->set_player_guid(i, rank->player_guid(i + 1));
			rank->set_name(i, rank->name(i + 1));
			rank->set_sex(i, rank->sex(i + 1));
			rank->set_level(i, rank->level(i + 1));
			rank->set_avatar(i, rank->avatar(i + 1));
			rank->set_toukuang(i, rank->toukuang(i + 1));
			rank->set_region_id(i, rank->region_id(i + 1));
			rank->set_name_color(i, rank->name_color(i + 1));
			rank->set_value(i, rank->value(i + 1));
		}
	}
	else
	{
		rank->add_player_guid(msg.player_guid());
		rank->add_name(msg.name());
		rank->add_sex(msg.sex());
		rank->add_level(msg.level());
		rank->add_avatar(msg.avatar());
		rank->add_toukuang(msg.toukuang());
		rank->add_region_id(msg.region_id());
		rank->add_name_color(msg.name_color());
		rank->add_value(value);
	}

	index = -1;
	for (int i = rank->player_guid_size() - 2; i >= 0; --i)
	{
		if (rank->value(i) < value)
		{
			rank->set_player_guid(i + 1, rank->player_guid(i));
			rank->set_name(i + 1, rank->name(i));
			rank->set_sex(i + 1, rank->sex(i));
			rank->set_level(i + 1, rank->level(i));
			rank->set_avatar(i + 1, rank->avatar(i));
			rank->set_toukuang(i + 1, rank->toukuang(i));
			rank->set_region_id(i + 1, rank->region_id(i));
			rank->set_name_color(i + 1, rank->name_color(i));
			rank->set_value(i + 1, rank->value(i));
		}
		else
		{
			index = i;
			break;
		}
	}
	rank->set_player_guid(index + 1, msg.player_guid());
	rank->set_name(index + 1, msg.name());
	rank->set_sex(index + 1, msg.sex());
	rank->set_level(index + 1, msg.level());
	rank->set_avatar(index + 1, msg.avatar());
	rank->set_toukuang(index + 1, msg.toukuang());
	rank->set_region_id(index + 1, msg.region_id());
	rank->set_name_color(index + 1, msg.name_color());
	rank->set_value(index + 1, value);

	if (rank->player_guid_size() > MAX_RANK_NUM)
	{
		rank->mutable_player_guid()->RemoveLast();
		rank->mutable_name()->RemoveLast();
		rank->mutable_sex()->RemoveLast();
		rank->mutable_level()->RemoveLast();
		rank->mutable_avatar()->RemoveLast();
		rank->mutable_toukuang()->RemoveLast();
		rank->mutable_region_id()->RemoveLast();
		rank->mutable_name_color()->RemoveLast();
		rank->mutable_value()->RemoveLast();
	}
	return 0;
}

int RankManager::push_hall_rank_forbidden(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_rank_forbidden msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	for (std::map<int, dhc::rank_t *>::iterator it = ranks_.begin(); it != ranks_.end(); ++it)
	{
		dhc::rank_t *rank = (*it).second;
		int index = -1;
		for (int i = 0; i < rank->player_guid_size(); ++i)
		{
			if (rank->player_guid(i) == msg.guid())
			{
				index = i;
				break;
			}
		}
		if (index != -1)
		{
			for (int i = index; i < rank->player_guid_size() - 1; ++i)
			{
				rank->set_player_guid(i, rank->player_guid(i + 1));
				rank->set_name(i, rank->name(i + 1));
				rank->set_sex(i, rank->sex(i + 1));
				rank->set_level(i, rank->level(i + 1));
				rank->set_avatar(i, rank->avatar(i + 1));
				rank->set_toukuang(i, rank->toukuang(i + 1));
				rank->set_region_id(i, rank->region_id(i + 1));
				rank->set_name_color(i, rank->name_color(i + 1));
				rank->set_value(i, rank->value(i + 1));
			}
			rank->mutable_player_guid()->RemoveLast();
			rank->mutable_name()->RemoveLast();
			rank->mutable_sex()->RemoveLast();
			rank->mutable_level()->RemoveLast();
			rank->mutable_avatar()->RemoveLast();
			rank->mutable_toukuang()->RemoveLast();
			rank->mutable_region_id()->RemoveLast();
			rank->mutable_name_color()->RemoveLast();
			rank->mutable_value()->RemoveLast();
		}
	}
	return 0;
}

int RankManager::update(ACE_Time_Value tv)
{
	save_all();
	RankMessage::push_rank_hall_cache(ranks_);
	return 0;
}
