#include "sqlquery.h"
#include <sstream>
#include "protocol_inc.h"

int Sqlrank_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::rank_t *obj = (dhc::rank_t *)data_;
	query << "INSERT INTO rank_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	{
		uint32_t size = obj->player_guid_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->player_guid(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "player_guid=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->name_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			std::string v = obj->name(i);
			uint32_t len = v.size() + 1;
			ssm.write(reinterpret_cast<char*>(&len), sizeof(uint32_t));
			ssm.write(v.data(), len);
		}
		query << "name=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->sex_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->sex(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "sex=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->level_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->level(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "level=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->avatar_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->avatar(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "avatar=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->toukuang_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->toukuang(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "toukuang=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->region_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->region_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "region_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->name_color_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->name_color(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "name_color=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->value_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->value(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "value=" << mysqlpp::quote << ssm.str();
	}

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlrank_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::rank_t *obj = (dhc::rank_t *)data_;
	query << "SELECT * FROM rank_t WHERE guid="
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
			obj->add_player_guid(v);
		}
	}
	if (!res.at(0).at(2).is_null())
	{
		std::string temp(res.at(0).at(2).data(), res.at(0).at(2).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint32_t len = 0;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&len), sizeof(uint32_t));
			boost::scoped_array<char> buf(new char[len]);
			ssm.read(buf.get(), len);
			obj->add_name(buf.get(), len);
		}
	}
	if (!res.at(0).at(3).is_null())
	{
		std::string temp(res.at(0).at(3).data(), res.at(0).at(3).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_sex(v);
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
			obj->add_level(v);
		}
	}
	if (!res.at(0).at(5).is_null())
	{
		std::string temp(res.at(0).at(5).data(), res.at(0).at(5).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_avatar(v);
		}
	}
	if (!res.at(0).at(6).is_null())
	{
		std::string temp(res.at(0).at(6).data(), res.at(0).at(6).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_toukuang(v);
		}
	}
	if (!res.at(0).at(7).is_null())
	{
		std::string temp(res.at(0).at(7).data(), res.at(0).at(7).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_region_id(v);
		}
	}
	if (!res.at(0).at(8).is_null())
	{
		std::string temp(res.at(0).at(8).data(), res.at(0).at(8).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_name_color(v);
		}
	}
	if (!res.at(0).at(9).is_null())
	{
		std::string temp(res.at(0).at(9).data(), res.at(0).at(9).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_value(v);
		}
	}
	return 0;
}

int Sqlrank_t::update(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::rank_t *obj = (dhc::rank_t *)data_;
	query << "UPDATE rank_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	{
		uint32_t size = obj->player_guid_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->player_guid(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "player_guid=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->name_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			std::string v = obj->name(i);
			uint32_t len = v.size() + 1;
			ssm.write(reinterpret_cast<char*>(&len), sizeof(uint32_t));
			ssm.write(v.data(), len);
		}
		query << "name=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->sex_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->sex(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "sex=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->level_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->level(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "level=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->avatar_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->avatar(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "avatar=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->toukuang_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->toukuang(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "toukuang=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->region_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->region_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "region_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->name_color_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->name_color(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "name_color=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->value_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->value(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "value=" << mysqlpp::quote << ssm.str();
	}
	query << " WHERE guid=" << boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlrank_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::rank_t *obj = (dhc::rank_t *)data_;
	query << "DELETE FROM rank_t WHERE guid="
		<< boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}
