#include "sqlquery2.h"
#include <sstream>
#include "protocol_inc.h"
#include "utils.h"

int SqlPlayerSocial_t::insert(mysqlpp::Connection *conn)
{
	return -1;
}

int SqlPlayerSocial_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	protocol::game::social_list_player_load *obj_list = (protocol::game::social_list_player_load *)data_;
	query << "SELECT guid,player_guid,target_guid,name,cup,avatar,toukuang,level,sex,stype,sflag,verify,gold,msgs,msgtimes,ttime,name_color FROM social_t where player_guid=";
	query << boost::lexical_cast<std::string>(obj_list->player_guid());

	mysqlpp::StoreQueryResult res = query.store();

	if (!res)
	{
		return -1;
	}

	for (int i = 0; i < res.num_rows(); ++i)
	{
		dhc::social_t* obj = obj_list->add_socials();
		if (!res.at(i).at(0).is_null())
		{
			obj->set_guid(res.at(i).at(0));
		}
		if (!res.at(i).at(1).is_null())
		{
			obj->set_player_guid(res.at(i).at(1));
		}
		if (!res.at(i).at(2).is_null())
		{
			obj->set_target_guid(res.at(i).at(2));
		}
		if (!res.at(i).at(3).is_null())
		{
			obj->set_name((std::string)res.at(i).at(3));
		}
		if (!res.at(i).at(4).is_null())
		{
			obj->set_cup(res.at(i).at(4));
		}
		if (!res.at(i).at(5).is_null())
		{
			obj->set_avatar(res.at(i).at(5));
		}
		if (!res.at(i).at(6).is_null())
		{
			obj->set_toukuang(res.at(i).at(6));
		}
		if (!res.at(i).at(7).is_null())
		{
			obj->set_level(res.at(i).at(7));
		}
		if (!res.at(i).at(8).is_null())
		{
			obj->set_sex(res.at(i).at(8));
		}
		if (!res.at(i).at(9).is_null())
		{
			obj->set_stype(res.at(i).at(9));
		}
		if (!res.at(i).at(10).is_null())
		{
			obj->set_sflag(res.at(i).at(10));
		}
		if (!res.at(i).at(11).is_null())
		{
			obj->set_verify(std::string(res.at(i).at(11)));
		}
		if (!res.at(i).at(12).is_null())
		{
			obj->set_gold(res.at(i).at(12));
		}
		if (!res.at(i).at(13).is_null())
		{
			std::string temp(res.at(0).at(13).data(), res.at(0).at(13).length());
			std::stringstream ssm(temp);
			uint32_t size = 0;
			ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
			uint32_t len = 0;
			for (uint32_t i = 0; i < size; i++)
			{
				ssm.read(reinterpret_cast<char*>(&len), sizeof(uint32_t));
				boost::scoped_array<char> buf(new char[len]);
				ssm.read(buf.get(), len);
				obj->add_msgs(buf.get(), len);
			}
		}
		if (!res.at(0).at(14).is_null())
		{
			std::string temp(res.at(0).at(14).data(), res.at(0).at(14).length());
			std::stringstream ssm(temp);
			uint32_t size = 0;
			ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
			uint64_t v;
			for (uint32_t i = 0; i < size; i++)
			{
				ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
				obj->add_msgtimes(v);
			}
		}
		if (!res.at(i).at(15).is_null())
		{
			obj->set_ttime(res.at(i).at(15));
		}
		if (!res.at(i).at(16).is_null())
		{
			obj->set_name_color(res.at(i).at(16));
		}
	}

	return 0;
}

int SqlPlayerSocial_t::update(mysqlpp::Connection *conn)
{
	return -1;
}

int SqlPlayerSocial_t::remove(mysqlpp::Connection *conn)
{
	return -1;
}


int SqlSocialRemove_t::insert(mysqlpp::Connection *conn)
{
	return -1;
}

int SqlSocialRemove_t::query(mysqlpp::Connection *conn)
{
	return -1;
}

int SqlSocialRemove_t::update(mysqlpp::Connection *conn)
{
	return -1;
}

int SqlSocialRemove_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::social_t *obj = (dhc::social_t *)data_;
	query << "DELETE FROM social_t WHERE player_guid="
		<< boost::lexical_cast<std::string>(obj->player_guid());
	query << " and target_guid="
		<< boost::lexical_cast<std::string>(obj->target_guid());

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

