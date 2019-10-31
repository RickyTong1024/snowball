#include "hall_dhc.h"
#include "sqlquery.h"
#include "sqlquery2.h"

int HallDhc::init()
{
	gtool_ = new GTool();
	if (-1 == gtool_->init("snowball_gtool"))
	{
		return -1;
	}

	std::vector<std::string> names;
	service::server_env()->get_db_names("snowball_player", names);
	for (int i = 0; i < names.size(); ++i)
	{
		Dhc * dhc_player = new Dhc();
		if (-1 == dhc_player->init(this, names[i]))
		{
			return -1;
		}
		dhc_players_.push_back(dhc_player);
	}
	return 0;
}

int HallDhc::fini()
{
	for (int i = 0; i < dhc_players_.size(); ++i)
	{
		dhc_players_[i]->fini();
		delete dhc_players_[i];
	}
	gtool_->fini();
	delete gtool_;
	return 0;
}

GTool *HallDhc::db_gtool()
{
	return gtool_;
}

Dhc *HallDhc::db_player(uint64_t guid)
{
	int index = guid % dhc_players_.size();
	return dhc_players_[index];
}

int HallDhc::do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_player)
	{
		Sqlplayer_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_role)
	{
		Sqlrole_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_battle_his)
	{
		Sqlbattle_his_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_post)
	{
		Sqlpost_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_post_new)
	{
		Sqlpost_new_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_reharge)
	{
		Sqlrecharge_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_share)
	{
		Sqlshare_t s(guid, data);
		return s.insert(conn);
	}
	return -1;
}

int HallDhc::do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_player)
	{
		Sqlplayer_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_role)
	{
		Sqlrole_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_battle_his)
	{
		Sqlbattle_his_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_post)
	{
		Sqlpost_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_post_new)
	{
		Sqlpost_new_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_reharge)
	{
		Sqlrecharge_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_share)
	{
		Sqlshare_t s(guid, data);
		return s.query(conn);
	}
	return -1;
}

int HallDhc::do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_player)
	{
		Sqlplayer_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_role)
	{
		Sqlrole_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_battle_his)
	{
		Sqlbattle_his_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_post)
	{
		Sqlpost_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_post_new)
	{
		Sqlpost_new_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_reharge)
	{
		Sqlrecharge_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_share)
	{
		Sqlshare_t s(guid, data);
		return s.update(conn);
	}
	return -1;
}

int HallDhc::do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_player)
	{
		Sqlplayer_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_role)
	{
		Sqlrole_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_battle_his)
	{
		Sqlbattle_his_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_post)
	{
		Sqlpost_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_post_new)
	{
		Sqlpost_new_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_reharge)
	{
		Sqlrecharge_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_share)
	{
		Sqlshare_t s(guid, data);
		return s.remove(conn);
	}
	return -1;
}
