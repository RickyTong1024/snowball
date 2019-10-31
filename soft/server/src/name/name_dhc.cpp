#include "name_dhc.h"
#include "sqlquery2.h"

int NameDhc::init()
{
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
	dhc_social_ = new Dhc();
	if (-1 == dhc_social_->init(this, "snowball_social"))
	{
		return -1;
	}
	return 0;
}

int NameDhc::fini()
{
	dhc_social_->fini();
	delete dhc_social_;
	for (int i = 0; i < dhc_players_.size(); ++i)
	{
		dhc_players_[i]->fini();
		delete dhc_players_[i];
	}
	return 0;
}

Dhc *NameDhc::db_player(uint64_t guid)
{
	int index = guid % dhc_players_.size();
	return dhc_players_[index];
}

Dhc *NameDhc::db_social()
{
	return dhc_social_;
}

int NameDhc::do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_player)
	{
		Sqlplayer_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_name)
	{
		Sqlname_t s(guid, data);
		return s.insert(conn);
	}
	return -1;
}

int NameDhc::do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_player)
	{
		Sqlplayer_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_name)
	{
		Sqlname_t s(guid, data);
		return s.query(conn);
	}
	return -1;
}

int NameDhc::do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_player)
	{
		Sqlplayer_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_name)
	{
		Sqlname_t s(guid, data);
		return s.update(conn);
	}
	return -1;
}

int NameDhc::do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_player)
	{
		Sqlplayer_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_name)
	{
		Sqlname_t s(guid, data);
		return s.remove(conn);
	}
	return -1;
}
