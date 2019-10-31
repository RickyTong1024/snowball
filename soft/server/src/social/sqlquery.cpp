#include "sqlquery.h"
#include <sstream>
#include "protocol_inc.h"

int Sqlsocial_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::social_t *obj = (dhc::social_t *)data_;
	query << "INSERT INTO social_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "player_guid=" << boost::lexical_cast<std::string>(obj->player_guid());
	query << ",";
	query << "target_guid=" << boost::lexical_cast<std::string>(obj->target_guid());
	query << ",";
	query << "name=" << mysqlpp::quote << obj->name();
	query << ",";
	query << "cup=" << boost::lexical_cast<std::string>(obj->cup());
	query << ",";
	query << "avatar=" << boost::lexical_cast<std::string>(obj->avatar());
	query << ",";
	query << "toukuang=" << boost::lexical_cast<std::string>(obj->toukuang());
	query << ",";
	query << "region_id=" << boost::lexical_cast<std::string>(obj->region_id());
	query << ",";
	query << "level=" << boost::lexical_cast<std::string>(obj->level());
	query << ",";
	query << "sex=" << boost::lexical_cast<std::string>(obj->sex());
	query << ",";
	query << "stype=" << boost::lexical_cast<std::string>(obj->stype());
	query << ",";
	query << "sflag=" << boost::lexical_cast<std::string>(obj->sflag());
	query << ",";
	query << "verify=" << mysqlpp::quote << obj->verify();
	query << ",";
	query << "gold=" << boost::lexical_cast<std::string>(obj->gold());
	query << ",";
	{
		uint32_t size = obj->msgs_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			std::string v = obj->msgs(i);
			uint32_t len = v.size() + 1;
			ssm.write(reinterpret_cast<char*>(&len), sizeof(uint32_t));
			ssm.write(v.data(), len);
		}
		query << "msgs=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->msgtimes_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->msgtimes(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "msgtimes=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "ttime=" << boost::lexical_cast<std::string>(obj->ttime());
	query << ",";
	query << "name_color=" << boost::lexical_cast<std::string>(obj->name_color());
	query << ",";
	query << "achieve_point=" << boost::lexical_cast<std::string>(obj->achieve_point());
	query << ",";
	query << "max_score=" << boost::lexical_cast<std::string>(obj->max_score());
	query << ",";
	query << "max_sha=" << boost::lexical_cast<std::string>(obj->max_sha());
	query << ",";
	query << "max_lsha=" << boost::lexical_cast<std::string>(obj->max_lsha());

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlsocial_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::social_t *obj = (dhc::social_t *)data_;
	query << "SELECT * FROM social_t WHERE guid="
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
		obj->set_player_guid(res.at(0).at(1));
	}
	if (!res.at(0).at(2).is_null())
	{
		obj->set_target_guid(res.at(0).at(2));
	}
	if (!res.at(0).at(3).is_null())
	{
		obj->set_name((std::string)res.at(0).at(3));
	}
	if (!res.at(0).at(4).is_null())
	{
		obj->set_cup(res.at(0).at(4));
	}
	if (!res.at(0).at(5).is_null())
	{
		obj->set_avatar(res.at(0).at(5));
	}
	if (!res.at(0).at(6).is_null())
	{
		obj->set_toukuang(res.at(0).at(6));
	}
	if (!res.at(0).at(7).is_null())
	{
		obj->set_region_id(res.at(0).at(7));
	}
	if (!res.at(0).at(8).is_null())
	{
		obj->set_level(res.at(0).at(8));
	}
	if (!res.at(0).at(9).is_null())
	{
		obj->set_sex(res.at(0).at(9));
	}
	if (!res.at(0).at(10).is_null())
	{
		obj->set_stype(res.at(0).at(10));
	}
	if (!res.at(0).at(11).is_null())
	{
		obj->set_sflag(res.at(0).at(11));
	}
	if (!res.at(0).at(12).is_null())
	{
		obj->set_verify((std::string)res.at(0).at(12));
	}
	if (!res.at(0).at(13).is_null())
	{
		obj->set_gold(res.at(0).at(13));
	}
	if (!res.at(0).at(14).is_null())
	{
		std::string temp(res.at(0).at(14).data(), res.at(0).at(14).length());
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
	if (!res.at(0).at(15).is_null())
	{
		std::string temp(res.at(0).at(15).data(), res.at(0).at(15).length());
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
	if (!res.at(0).at(16).is_null())
	{
		obj->set_ttime(res.at(0).at(16));
	}
	if (!res.at(0).at(17).is_null())
	{
		obj->set_name_color(res.at(0).at(17));
	}
	if (!res.at(0).at(18).is_null())
	{
		obj->set_achieve_point(res.at(0).at(18));
	}
	if (!res.at(0).at(19).is_null())
	{
		obj->set_max_score(res.at(0).at(19));
	}
	if (!res.at(0).at(20).is_null())
	{
		obj->set_max_sha(res.at(0).at(20));
	}
	if (!res.at(0).at(21).is_null())
	{
		obj->set_max_lsha(res.at(0).at(21));
	}
	return 0;
}

int Sqlsocial_t::update(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::social_t *obj = (dhc::social_t *)data_;
	query << "UPDATE social_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "player_guid=" << boost::lexical_cast<std::string>(obj->player_guid());
	query << ",";
	query << "target_guid=" << boost::lexical_cast<std::string>(obj->target_guid());
	query << ",";
	query << "name=" << mysqlpp::quote << obj->name();
	query << ",";
	query << "cup=" << boost::lexical_cast<std::string>(obj->cup());
	query << ",";
	query << "avatar=" << boost::lexical_cast<std::string>(obj->avatar());
	query << ",";
	query << "toukuang=" << boost::lexical_cast<std::string>(obj->toukuang());
	query << ",";
	query << "region_id=" << boost::lexical_cast<std::string>(obj->region_id());
	query << ",";
	query << "level=" << boost::lexical_cast<std::string>(obj->level());
	query << ",";
	query << "sex=" << boost::lexical_cast<std::string>(obj->sex());
	query << ",";
	query << "stype=" << boost::lexical_cast<std::string>(obj->stype());
	query << ",";
	query << "sflag=" << boost::lexical_cast<std::string>(obj->sflag());
	query << ",";
	query << "verify=" << mysqlpp::quote << obj->verify();
	query << ",";
	query << "gold=" << boost::lexical_cast<std::string>(obj->gold());
	query << ",";
	{
		uint32_t size = obj->msgs_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			std::string v = obj->msgs(i);
			uint32_t len = v.size() + 1;
			ssm.write(reinterpret_cast<char*>(&len), sizeof(uint32_t));
			ssm.write(v.data(), len);
		}
		query << "msgs=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->msgtimes_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->msgtimes(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "msgtimes=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "ttime=" << boost::lexical_cast<std::string>(obj->ttime());
	query << ",";
	query << "name_color=" << boost::lexical_cast<std::string>(obj->name_color());
	query << ",";
	query << "achieve_point=" << boost::lexical_cast<std::string>(obj->achieve_point());
	query << ",";
	query << "max_score=" << boost::lexical_cast<std::string>(obj->max_score());
	query << ",";
	query << "max_sha=" << boost::lexical_cast<std::string>(obj->max_sha());
	query << ",";
	query << "max_lsha=" << boost::lexical_cast<std::string>(obj->max_lsha());
	query << " WHERE guid=" << boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlsocial_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::social_t *obj = (dhc::social_t *)data_;
	query << "DELETE FROM social_t WHERE guid="
		<< boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

//////////////////////////////////////////////////////////////////////////

int Sqlsocial_list_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::social_list_t *obj = (dhc::social_list_t *)data_;
	query << "INSERT INTO social_list_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	{
		uint32_t size = obj->player_guids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->player_guids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "player_guids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->player_times_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->player_times(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "player_times=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->player_bgift_guids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->player_bgift_guids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "player_bgift_guids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->player_bgift_nums_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->player_bgift_nums(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "player_bgift_nums=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "gold_refresh_time=" << boost::lexical_cast<std::string>(obj->gold_refresh_time());

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlsocial_list_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::social_list_t *obj = (dhc::social_list_t *)data_;
	query << "SELECT * FROM social_list_t WHERE guid="
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
		std::string temp(res.at(0).at(1).data(), res.at(0).at(1).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint64_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
			obj->add_player_guids(v);
		}
	}
	if (!res.at(0).at(2).is_null())
	{
		std::string temp(res.at(0).at(2).data(), res.at(0).at(2).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint64_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
			obj->add_player_times(v);
		}
	}
	if (!res.at(0).at(3).is_null())
	{
		std::string temp(res.at(0).at(3).data(), res.at(0).at(3).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint64_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
			obj->add_player_bgift_guids(v);
		}
	}
	if (!res.at(0).at(4).is_null())
	{
		std::string temp(res.at(0).at(4).data(), res.at(0).at(4).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_player_bgift_nums(v);
		}
	}
	if (!res.at(0).at(5).is_null())
	{
		obj->set_gold_refresh_time(res.at(0).at(5));
	}
	return 0;
}

int Sqlsocial_list_t::update(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::social_list_t *obj = (dhc::social_list_t *)data_;
	query << "UPDATE social_list_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	{
		uint32_t size = obj->player_guids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->player_guids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "player_guids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->player_times_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->player_times(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "player_times=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->player_bgift_guids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->player_bgift_guids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "player_bgift_guids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->player_bgift_nums_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->player_bgift_nums(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "player_bgift_nums=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "gold_refresh_time=" << boost::lexical_cast<std::string>(obj->gold_refresh_time());
	query << " WHERE guid=" << boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlsocial_list_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::social_list_t *obj = (dhc::social_list_t *)data_;
	query << "DELETE FROM social_list_t WHERE guid="
		<< boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}
