#include "sqlquery2.h"
#include <sstream>
#include "protocol_inc.h"

int Sqlname_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	mysqlpp::Transaction trans(*conn, mysqlpp::Transaction::serializable, mysqlpp::Transaction::session);
	dhc::name_t *obj = (dhc::name_t *)data_;
	query << "SELECT guid FROM name_t WHERE guid=" << boost::lexical_cast<std::string>(obj->guid());
	mysqlpp::StoreQueryResult res = query.store();

	if (res && res.num_rows() >= 1)
	{
		query << "UPDATE name_t SET "
			<< "guid=" << boost::lexical_cast<std::string>(obj->guid())
			<< ","
			<< "name=" << mysqlpp::quote << obj->name()
			<< " WHERE guid=" << boost::lexical_cast<std::string>(obj->guid());

		mysqlpp::SimpleResult res1 = query.execute();

		if (!res1)
		{
			service::log()->error(query.error());
			return -1;
		}
	}
	else
	{
		query << "INSERT INTO name_t SET "
			<< "guid=" << boost::lexical_cast<std::string>(obj->guid())
			<< ","
			<< "name=" << mysqlpp::quote << obj->name();

		mysqlpp::SimpleResult res1 = query.execute();

		if (!res1)
		{
			service::log()->error(query.error());
			return -1;
		}
	}
	trans.commit();

	return 0;
}

int Sqlname_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	protocol::game::msg_name_list *obj_list = (protocol::game::msg_name_list *)data_;
	query << "SELECT guid FROM name_t where name=" << mysqlpp::quote << boost::lexical_cast<std::string>(obj_list->name());
	query << " limit 10";

	mysqlpp::StoreQueryResult res = query.store();

	if (!res)
	{
		return -1;
	}

	for (int i = 0; i < res.num_rows(); ++i)
	{
		if (!res.at(i).at(0).is_null())
		{
			obj_list->add_guids(res.at(i).at(0));
		}
	}

	return 0;
}

int Sqlname_t::update(mysqlpp::Connection *conn)
{
	return -1;
}

int Sqlname_t::remove(mysqlpp::Connection *conn)
{
	return -1;
}

//////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////

int Sqlplayer_t::insert(mysqlpp::Connection *conn)
{
	return -1;
}

int Sqlplayer_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	protocol::game::msg_name_player *obj = (protocol::game::msg_name_player *)data_;
	query << "SELECT guid, name, sex, level, cup, avatar_on, toukuang_on, region, yue_time, nian_time FROM player_t WHERE guid="
		<< boost::lexical_cast<std::string>(guid_);

	mysqlpp::StoreQueryResult res = query.store();

	if (!res || res.num_rows() != 1)
	{
		return -1;
	}

	if (!res.at(0).at(0).is_null())
	{
		obj->set_guid(res.at(0).at(0));
	}
	if (!res.at(0).at(1).is_null())
	{
		obj->set_name((std::string)res.at(0).at(1));
	}
	if (!res.at(0).at(2).is_null())
	{
		obj->set_sex(res.at(0).at(2));
	}
	if (!res.at(0).at(3).is_null())
	{
		obj->set_level(res.at(0).at(3));
	}
	if (!res.at(0).at(4).is_null())
	{
		obj->set_cup(res.at(0).at(4));
	}
	if (!res.at(0).at(5).is_null())
	{
		obj->set_avatar_on(res.at(0).at(5));
	}
	if (!res.at(0).at(6).is_null())
	{
		obj->set_toukuang_on(res.at(0).at(6));
	}
	if (!res.at(0).at(7).is_null())
	{
		obj->set_region_id(res.at(0).at(7));
	}
	uint64_t yt = 0;
	uint64_t nt = 0;
	if (!res.at(0).at(8).is_null())
	{
		yt = res.at(0).at(8);
	}
	if (!res.at(0).at(9).is_null())
	{
		nt = res.at(0).at(9);
	}
	int nc = 0;
	if (nt > service::timer()->now())
	{
		nc = 2;
	}
	else if (yt > service::timer()->now())
	{
		nc = 1;
	}
	obj->set_name_color(nc);
	return 0;
}

int Sqlplayer_t::update(mysqlpp::Connection *conn)
{
	return -1;
}

int Sqlplayer_t::remove(mysqlpp::Connection *conn)
{
	return -1;
}
