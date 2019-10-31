#include "sqlquery.h"
#include <sstream>
#include "protocol_inc.h"

int Sqlbattle_result_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::battle_result_t *obj = (dhc::battle_result_t *)data_;
	query << "INSERT INTO battle_result_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "type=" << boost::lexical_cast<std::string>(obj->type());
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
		uint32_t size = obj->names_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			std::string v = obj->names(i);
			uint32_t len = v.size() + 1;
			ssm.write(reinterpret_cast<char*>(&len), sizeof(uint32_t));
			ssm.write(v.data(), len);
		}
		query << "names=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->role_ids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->role_ids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "role_ids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->sexs_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->sexs(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "sexs=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->avatars_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->avatars(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "avatars=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->ranks_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->ranks(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "ranks=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->shas_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->shas(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "shas=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->lshas_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->lshas(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "lshas=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->dies_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->dies(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "dies=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->scores_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->scores(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "scores=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->cups_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->cups(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "cups=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->cup_adds_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->cup_adds(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "cup_adds=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieves_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->achieves(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "achieves=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->use_skills_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->use_skills(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "use_skills=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->use_tanlents_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->use_tanlents(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "use_tanlents=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "dt=now()";

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlbattle_result_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::battle_result_t *obj = (dhc::battle_result_t *)data_;
	query << "SELECT * FROM battle_result_t WHERE guid="
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
		obj->set_type(res.at(0).at(1));
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
			obj->add_player_guids(v);
		}
	}
	if (!res.at(0).at(3).is_null())
	{
		std::string temp(res.at(0).at(3).data(), res.at(0).at(3).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint32_t len = 0;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&len), sizeof(uint32_t));
			boost::scoped_array<char> buf(new char[len]);
			ssm.read(buf.get(), len);
			obj->add_names(buf.get(), len);
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
			obj->add_role_ids(v);
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
			obj->add_sexs(v);
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
			obj->add_avatars(v);
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
			obj->add_ranks(v);
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
			obj->add_shas(v);
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
			obj->add_lshas(v);
		}
	}
	if (!res.at(0).at(10).is_null())
	{
		std::string temp(res.at(0).at(10).data(), res.at(0).at(10).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_dies(v);
		}
	}
	if (!res.at(0).at(11).is_null())
	{
		std::string temp(res.at(0).at(11).data(), res.at(0).at(11).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_scores(v);
		}
	}
	if (!res.at(0).at(12).is_null())
	{
		std::string temp(res.at(0).at(12).data(), res.at(0).at(12).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_cups(v);
		}
	}
	if (!res.at(0).at(13).is_null())
	{
		std::string temp(res.at(0).at(13).data(), res.at(0).at(13).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_cup_adds(v);
		}
	}
	if (!res.at(0).at(14).is_null())
	{
		std::string temp(res.at(0).at(14).data(), res.at(0).at(14).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_achieves(v);
		}
	}
	if (!res.at(0).at(15).is_null())
	{
		std::string temp(res.at(0).at(15).data(), res.at(0).at(15).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_use_skills(v);
		}
	}
	if (!res.at(0).at(16).is_null())
	{
		std::string temp(res.at(0).at(16).data(), res.at(0).at(16).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_use_tanlents(v);
		}
	}
	return 0;
}

int Sqlbattle_result_t::update(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::battle_result_t *obj = (dhc::battle_result_t *)data_;
	query << "UPDATE battle_result_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "type=" << boost::lexical_cast<std::string>(obj->type());
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
		uint32_t size = obj->names_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			std::string v = obj->names(i);
			uint32_t len = v.size() + 1;
			ssm.write(reinterpret_cast<char*>(&len), sizeof(uint32_t));
			ssm.write(v.data(), len);
		}
		query << "names=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->role_ids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->role_ids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "role_ids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->sexs_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->sexs(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "sexs=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->avatars_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->avatars(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "avatars=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->ranks_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->ranks(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "ranks=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->shas_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->shas(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "shas=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->lshas_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->lshas(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "lshas=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->dies_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->dies(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "dies=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->scores_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->scores(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "scores=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->cups_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->cups(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "cups=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->cup_adds_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->cup_adds(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "cup_adds=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieves_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->achieves(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "achieves=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->use_skills_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->use_skills(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "use_skills=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->use_tanlents_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->use_tanlents(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "use_tanlents=" << mysqlpp::quote << ssm.str();
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

int Sqlbattle_result_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::battle_result_t *obj = (dhc::battle_result_t *)data_;
	query << "DELETE FROM battle_result_t WHERE guid="
		<< boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}
