#include "sqlquery.h"
#include <sstream>
#include "protocol_inc.h"

int Sqlplayer_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::player_t *obj = (dhc::player_t *)data_;
	query << "INSERT INTO player_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "serverid=" << boost::lexical_cast<std::string>(obj->serverid());
	query << ",";
	query << "name=" << mysqlpp::quote << obj->name();
	query << ",";
	query << "region_id=" << boost::lexical_cast<std::string>(obj->region_id());
	query << ",";
	query << "infomation=" << mysqlpp::quote << obj->infomation();
	query << ",";
	query << "sex=" << boost::lexical_cast<std::string>(obj->sex());
	query << ",";
	query << "birth_time=" << boost::lexical_cast<std::string>(obj->birth_time());
	query << ",";
	query << "last_daily_time=" << boost::lexical_cast<std::string>(obj->last_daily_time());
	query << ",";
	query << "last_week_time=" << boost::lexical_cast<std::string>(obj->last_week_time());
	query << ",";
	query << "last_month_time=" << boost::lexical_cast<std::string>(obj->last_month_time());
	query << ",";
	query << "last_login_time=" << boost::lexical_cast<std::string>(obj->last_login_time());
	query << ",";
	query << "last_check_time=" << boost::lexical_cast<std::string>(obj->last_check_time());
	query << ",";
	query << "is_guide=" << boost::lexical_cast<std::string>(obj->is_guide());
	query << ",";
	query << "gold=" << boost::lexical_cast<std::string>(obj->gold());
	query << ",";
	query << "jewel=" << boost::lexical_cast<std::string>(obj->jewel());
	query << ",";
	query << "level=" << boost::lexical_cast<std::string>(obj->level());
	query << ",";
	query << "exp=" << boost::lexical_cast<std::string>(obj->exp());
	query << ",";
	query << "cup=" << boost::lexical_cast<std::string>(obj->cup());
	query << ",";
	query << "snow=" << boost::lexical_cast<std::string>(obj->snow());
	query << ",";
	query << "battle_gold=" << boost::lexical_cast<std::string>(obj->battle_gold());
	query << ",";
	query << "max_cup=" << boost::lexical_cast<std::string>(obj->max_cup());
	query << ",";
	query << "max_score=" << boost::lexical_cast<std::string>(obj->max_score());
	query << ",";
	query << "max_sha=" << boost::lexical_cast<std::string>(obj->max_sha());
	query << ",";
	query << "max_lsha=" << boost::lexical_cast<std::string>(obj->max_lsha());
	query << ",";
	query << "box_zd_num=" << boost::lexical_cast<std::string>(obj->box_zd_num());
	query << ",";
	query << "box_zd_opened=" << boost::lexical_cast<std::string>(obj->box_zd_opened());
	query << ",";
	{
		uint32_t size = obj->box_ids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->box_ids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "box_ids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "box_open_slot=" << boost::lexical_cast<std::string>(obj->box_open_slot());
	query << ",";
	query << "box_open_time=" << boost::lexical_cast<std::string>(obj->box_open_time());
	query << ",";
	query << "sign_time=" << boost::lexical_cast<std::string>(obj->sign_time());
	query << ",";
	query << "sign_index=" << boost::lexical_cast<std::string>(obj->sign_index());
	query << ",";
	query << "sign_finish=" << boost::lexical_cast<std::string>(obj->sign_finish());
	query << ",";
	{
		uint32_t size = obj->role_guid_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->role_guid(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "role_guid=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "role_on=" << boost::lexical_cast<std::string>(obj->role_on());
	query << ",";
	{
		uint32_t size = obj->item_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->item_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "item_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->item_num_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->item_num(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "item_num=" << mysqlpp::quote << ssm.str();
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
	query << "avatar_on=" << boost::lexical_cast<std::string>(obj->avatar_on());
	query << ",";
	{
		uint32_t size = obj->battle_his_guids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->battle_his_guids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "battle_his_guids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->post_guids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->post_guids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "post_guids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieve_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->achieve_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "achieve_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieve_num_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->achieve_num(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "achieve_num=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieve_reward_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->achieve_reward(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "achieve_reward=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieve_time_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->achieve_time(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "achieve_time=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "achieve_point=" << boost::lexical_cast<std::string>(obj->achieve_point());
	query << ",";
	query << "achieve_index=" << boost::lexical_cast<std::string>(obj->achieve_index());
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
		uint32_t size = obj->toukuang_time_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->toukuang_time(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "toukuang_time=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "toukuang_on=" << boost::lexical_cast<std::string>(obj->toukuang_on());
	query << ",";
	{
		uint32_t size = obj->task_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->task_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "task_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->task_num_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->task_num(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "task_num=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->task_reward_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->task_reward(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "task_reward=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->fashion_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->fashion_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "fashion_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->fashion_on_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->fashion_on(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "fashion_on=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->daily_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->daily_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "daily_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->daily_num_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->daily_num(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "daily_num=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->daily_reward_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->daily_reward(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "daily_reward=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "daily_point=" << boost::lexical_cast<std::string>(obj->daily_point());
	query << ",";
	{
		uint32_t size = obj->daily_get_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->daily_get_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "daily_get_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->level_reward_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->level_reward(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "level_reward=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->social_golds_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->social_golds(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "social_golds=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "total_recharge=" << boost::lexical_cast<std::string>(obj->total_recharge());
	query << ",";
	query << "total_spend=" << boost::lexical_cast<std::string>(obj->total_spend());
	query << ",";
	query << "change_name_num=" << boost::lexical_cast<std::string>(obj->change_name_num());
	query << ",";
	query << "fenxiang_num=" << boost::lexical_cast<std::string>(obj->fenxiang_num());
	query << ",";
	query << "fenxiang_total_num=" << boost::lexical_cast<std::string>(obj->fenxiang_total_num());
	query << ",";
	query << "fenxiang_state=" << boost::lexical_cast<std::string>(obj->fenxiang_state());
	query << ",";
	{
		uint32_t size = obj->libao_nums_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->libao_nums(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "libao_nums=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "battle_num=" << boost::lexical_cast<std::string>(obj->battle_num());
	query << ",";
	query << "offline_battle_time=" << boost::lexical_cast<std::string>(obj->offline_battle_time());
	query << ",";
	query << "first_recharge=" << boost::lexical_cast<std::string>(obj->first_recharge());
	query << ",";
	query << "yue_time=" << boost::lexical_cast<std::string>(obj->yue_time());
	query << ",";
	query << "yue_reward=" << boost::lexical_cast<std::string>(obj->yue_reward());
	query << ",";
	query << "yue_first=" << boost::lexical_cast<std::string>(obj->yue_first());
	query << ",";
	query << "nian_time=" << boost::lexical_cast<std::string>(obj->nian_time());
	query << ",";
	query << "nian_reward=" << boost::lexical_cast<std::string>(obj->nian_reward());
	query << ",";
	query << "nian_first=" << boost::lexical_cast<std::string>(obj->nian_first());
	query << ",";
	{
		uint32_t size = obj->duobao_items_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->duobao_items(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "duobao_items=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "duobao_num=" << boost::lexical_cast<std::string>(obj->duobao_num());
	query << ",";
	query << "advertisement_num=" << boost::lexical_cast<std::string>(obj->advertisement_num());
	query << ",";
	query << "advertisement_time=" << boost::lexical_cast<std::string>(obj->advertisement_time());

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlplayer_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::player_t *obj = (dhc::player_t *)data_;
	query << "SELECT * FROM player_t WHERE guid="
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
		obj->set_serverid(res.at(0).at(1));
	}
	if (!res.at(0).at(2).is_null())
	{
		obj->set_name((std::string)res.at(0).at(2));
	}
	if (!res.at(0).at(3).is_null())
	{
		obj->set_region_id(res.at(0).at(3));
	}
	if (!res.at(0).at(4).is_null())
	{
		obj->set_infomation((std::string)res.at(0).at(4));
	}
	if (!res.at(0).at(5).is_null())
	{
		obj->set_sex(res.at(0).at(5));
	}
	if (!res.at(0).at(6).is_null())
	{
		obj->set_birth_time(res.at(0).at(6));
	}
	if (!res.at(0).at(7).is_null())
	{
		obj->set_last_daily_time(res.at(0).at(7));
	}
	if (!res.at(0).at(8).is_null())
	{
		obj->set_last_week_time(res.at(0).at(8));
	}
	if (!res.at(0).at(9).is_null())
	{
		obj->set_last_month_time(res.at(0).at(9));
	}
	if (!res.at(0).at(10).is_null())
	{
		obj->set_last_login_time(res.at(0).at(10));
	}
	if (!res.at(0).at(11).is_null())
	{
		obj->set_last_check_time(res.at(0).at(11));
	}
	if (!res.at(0).at(12).is_null())
	{
		obj->set_is_guide(res.at(0).at(12));
	}
	if (!res.at(0).at(13).is_null())
	{
		obj->set_gold(res.at(0).at(13));
	}
	if (!res.at(0).at(14).is_null())
	{
		obj->set_jewel(res.at(0).at(14));
	}
	if (!res.at(0).at(15).is_null())
	{
		obj->set_level(res.at(0).at(15));
	}
	if (!res.at(0).at(16).is_null())
	{
		obj->set_exp(res.at(0).at(16));
	}
	if (!res.at(0).at(17).is_null())
	{
		obj->set_cup(res.at(0).at(17));
	}
	if (!res.at(0).at(18).is_null())
	{
		obj->set_snow(res.at(0).at(18));
	}
	if (!res.at(0).at(19).is_null())
	{
		obj->set_battle_gold(res.at(0).at(19));
	}
	if (!res.at(0).at(20).is_null())
	{
		obj->set_max_cup(res.at(0).at(20));
	}
	if (!res.at(0).at(21).is_null())
	{
		obj->set_max_score(res.at(0).at(21));
	}
	if (!res.at(0).at(22).is_null())
	{
		obj->set_max_sha(res.at(0).at(22));
	}
	if (!res.at(0).at(23).is_null())
	{
		obj->set_max_lsha(res.at(0).at(23));
	}
	if (!res.at(0).at(24).is_null())
	{
		obj->set_box_zd_num(res.at(0).at(24));
	}
	if (!res.at(0).at(25).is_null())
	{
		obj->set_box_zd_opened(res.at(0).at(25));
	}
	if (!res.at(0).at(26).is_null())
	{
		std::string temp(res.at(0).at(26).data(), res.at(0).at(26).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_box_ids(v);
		}
	}
	if (!res.at(0).at(27).is_null())
	{
		obj->set_box_open_slot(res.at(0).at(27));
	}
	if (!res.at(0).at(28).is_null())
	{
		obj->set_box_open_time(res.at(0).at(28));
	}
	if (!res.at(0).at(29).is_null())
	{
		obj->set_sign_time(res.at(0).at(29));
	}
	if (!res.at(0).at(30).is_null())
	{
		obj->set_sign_index(res.at(0).at(30));
	}
	if (!res.at(0).at(31).is_null())
	{
		obj->set_sign_finish(res.at(0).at(31));
	}
	if (!res.at(0).at(32).is_null())
	{
		std::string temp(res.at(0).at(32).data(), res.at(0).at(32).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint64_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
			obj->add_role_guid(v);
		}
	}
	if (!res.at(0).at(33).is_null())
	{
		obj->set_role_on(res.at(0).at(33));
	}
	if (!res.at(0).at(34).is_null())
	{
		std::string temp(res.at(0).at(34).data(), res.at(0).at(34).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_item_id(v);
		}
	}
	if (!res.at(0).at(35).is_null())
	{
		std::string temp(res.at(0).at(35).data(), res.at(0).at(35).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_item_num(v);
		}
	}
	if (!res.at(0).at(36).is_null())
	{
		std::string temp(res.at(0).at(36).data(), res.at(0).at(36).length());
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
	if (!res.at(0).at(37).is_null())
	{
		obj->set_avatar_on(res.at(0).at(37));
	}
	if (!res.at(0).at(38).is_null())
	{
		std::string temp(res.at(0).at(38).data(), res.at(0).at(38).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint64_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
			obj->add_battle_his_guids(v);
		}
	}
	if (!res.at(0).at(39).is_null())
	{
		std::string temp(res.at(0).at(39).data(), res.at(0).at(39).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint64_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
			obj->add_post_guids(v);
		}
	}
	if (!res.at(0).at(40).is_null())
	{
		std::string temp(res.at(0).at(40).data(), res.at(0).at(40).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_achieve_id(v);
		}
	}
	if (!res.at(0).at(41).is_null())
	{
		std::string temp(res.at(0).at(41).data(), res.at(0).at(41).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_achieve_num(v);
		}
	}
	if (!res.at(0).at(42).is_null())
	{
		std::string temp(res.at(0).at(42).data(), res.at(0).at(42).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_achieve_reward(v);
		}
	}
	if (!res.at(0).at(43).is_null())
	{
		std::string temp(res.at(0).at(43).data(), res.at(0).at(43).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint64_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
			obj->add_achieve_time(v);
		}
	}
	if (!res.at(0).at(44).is_null())
	{
		obj->set_achieve_point(res.at(0).at(44));
	}
	if (!res.at(0).at(45).is_null())
	{
		obj->set_achieve_index(res.at(0).at(45));
	}
	if (!res.at(0).at(46).is_null())
	{
		std::string temp(res.at(0).at(46).data(), res.at(0).at(46).length());
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
	if (!res.at(0).at(47).is_null())
	{
		std::string temp(res.at(0).at(47).data(), res.at(0).at(47).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint64_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
			obj->add_toukuang_time(v);
		}
	}
	if (!res.at(0).at(48).is_null())
	{
		obj->set_toukuang_on(res.at(0).at(48));
	}
	if (!res.at(0).at(49).is_null())
	{
		std::string temp(res.at(0).at(49).data(), res.at(0).at(49).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_task_id(v);
		}
	}
	if (!res.at(0).at(50).is_null())
	{
		std::string temp(res.at(0).at(50).data(), res.at(0).at(50).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_task_num(v);
		}
	}
	if (!res.at(0).at(51).is_null())
	{
		std::string temp(res.at(0).at(51).data(), res.at(0).at(51).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_task_reward(v);
		}
	}
	if (!res.at(0).at(52).is_null())
	{
		std::string temp(res.at(0).at(52).data(), res.at(0).at(52).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_fashion_id(v);
		}
	}
	if (!res.at(0).at(53).is_null())
	{
		std::string temp(res.at(0).at(53).data(), res.at(0).at(53).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_fashion_on(v);
		}
	}
	if (!res.at(0).at(54).is_null())
	{
		std::string temp(res.at(0).at(54).data(), res.at(0).at(54).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_daily_id(v);
		}
	}
	if (!res.at(0).at(55).is_null())
	{
		std::string temp(res.at(0).at(55).data(), res.at(0).at(55).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_daily_num(v);
		}
	}
	if (!res.at(0).at(56).is_null())
	{
		std::string temp(res.at(0).at(56).data(), res.at(0).at(56).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_daily_reward(v);
		}
	}
	if (!res.at(0).at(57).is_null())
	{
		obj->set_daily_point(res.at(0).at(57));
	}
	if (!res.at(0).at(58).is_null())
	{
		std::string temp(res.at(0).at(58).data(), res.at(0).at(58).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_daily_get_id(v);
		}
	}
	if (!res.at(0).at(59).is_null())
	{
		std::string temp(res.at(0).at(59).data(), res.at(0).at(59).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_level_reward(v);
		}
	}
	if (!res.at(0).at(60).is_null())
	{
		std::string temp(res.at(0).at(60).data(), res.at(0).at(60).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		uint64_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(uint64_t));
			obj->add_social_golds(v);
		}
	}
	if (!res.at(0).at(61).is_null())
	{
		obj->set_total_recharge(res.at(0).at(61));
	}
	if (!res.at(0).at(62).is_null())
	{
		obj->set_total_spend(res.at(0).at(62));
	}
	if (!res.at(0).at(63).is_null())
	{
		obj->set_change_name_num(res.at(0).at(63));
	}
	if (!res.at(0).at(64).is_null())
	{
		obj->set_fenxiang_num(res.at(0).at(64));
	}
	if (!res.at(0).at(65).is_null())
	{
		obj->set_fenxiang_total_num(res.at(0).at(65));
	}
	if (!res.at(0).at(66).is_null())
	{
		obj->set_fenxiang_state(res.at(0).at(66));
	}
	if (!res.at(0).at(67).is_null())
	{
		std::string temp(res.at(0).at(67).data(), res.at(0).at(67).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_libao_nums(v);
		}
	}
	if (!res.at(0).at(68).is_null())
	{
		obj->set_battle_num(res.at(0).at(68));
	}
	if (!res.at(0).at(69).is_null())
	{
		obj->set_offline_battle_time(res.at(0).at(69));
	}
	if (!res.at(0).at(70).is_null())
	{
		obj->set_first_recharge(res.at(0).at(70));
	}
	if (!res.at(0).at(71).is_null())
	{
		obj->set_yue_time(res.at(0).at(71));
	}
	if (!res.at(0).at(72).is_null())
	{
		obj->set_yue_reward(res.at(0).at(72));
	}
	if (!res.at(0).at(73).is_null())
	{
		obj->set_yue_first(res.at(0).at(73));
	}
	if (!res.at(0).at(74).is_null())
	{
		obj->set_nian_time(res.at(0).at(74));
	}
	if (!res.at(0).at(75).is_null())
	{
		obj->set_nian_reward(res.at(0).at(75));
	}
	if (!res.at(0).at(76).is_null())
	{
		obj->set_nian_first(res.at(0).at(76));
	}
	if (!res.at(0).at(77).is_null())
	{
		std::string temp(res.at(0).at(77).data(), res.at(0).at(77).length());
		std::stringstream ssm(temp);
		uint32_t size = 0;
		ssm.read(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		int32_t v;
		for (uint32_t i = 0; i < size; i++)
		{
			ssm.read(reinterpret_cast<char*>(&v), sizeof(int32_t));
			obj->add_duobao_items(v);
		}
	}
	if (!res.at(0).at(78).is_null())
	{
		obj->set_duobao_num(res.at(0).at(78));
	}
	if (!res.at(0).at(79).is_null())
	{
		obj->set_advertisement_num(res.at(0).at(79));
	}
	if (!res.at(0).at(80).is_null())
	{
		obj->set_advertisement_time(res.at(0).at(80));
	}
	return 0;
}

int Sqlplayer_t::update(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::player_t *obj = (dhc::player_t *)data_;
	query << "UPDATE player_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "serverid=" << boost::lexical_cast<std::string>(obj->serverid());
	query << ",";
	query << "name=" << mysqlpp::quote << obj->name();
	query << ",";
	query << "region_id=" << boost::lexical_cast<std::string>(obj->region_id());
	query << ",";
	query << "infomation=" << mysqlpp::quote << obj->infomation();
	query << ",";
	query << "sex=" << boost::lexical_cast<std::string>(obj->sex());
	query << ",";
	query << "birth_time=" << boost::lexical_cast<std::string>(obj->birth_time());
	query << ",";
	query << "last_daily_time=" << boost::lexical_cast<std::string>(obj->last_daily_time());
	query << ",";
	query << "last_week_time=" << boost::lexical_cast<std::string>(obj->last_week_time());
	query << ",";
	query << "last_month_time=" << boost::lexical_cast<std::string>(obj->last_month_time());
	query << ",";
	query << "last_login_time=" << boost::lexical_cast<std::string>(obj->last_login_time());
	query << ",";
	query << "last_check_time=" << boost::lexical_cast<std::string>(obj->last_check_time());
	query << ",";
	query << "is_guide=" << boost::lexical_cast<std::string>(obj->is_guide());
	query << ",";
	query << "gold=" << boost::lexical_cast<std::string>(obj->gold());
	query << ",";
	query << "jewel=" << boost::lexical_cast<std::string>(obj->jewel());
	query << ",";
	query << "level=" << boost::lexical_cast<std::string>(obj->level());
	query << ",";
	query << "exp=" << boost::lexical_cast<std::string>(obj->exp());
	query << ",";
	query << "cup=" << boost::lexical_cast<std::string>(obj->cup());
	query << ",";
	query << "snow=" << boost::lexical_cast<std::string>(obj->snow());
	query << ",";
	query << "battle_gold=" << boost::lexical_cast<std::string>(obj->battle_gold());
	query << ",";
	query << "max_cup=" << boost::lexical_cast<std::string>(obj->max_cup());
	query << ",";
	query << "max_score=" << boost::lexical_cast<std::string>(obj->max_score());
	query << ",";
	query << "max_sha=" << boost::lexical_cast<std::string>(obj->max_sha());
	query << ",";
	query << "max_lsha=" << boost::lexical_cast<std::string>(obj->max_lsha());
	query << ",";
	query << "box_zd_num=" << boost::lexical_cast<std::string>(obj->box_zd_num());
	query << ",";
	query << "box_zd_opened=" << boost::lexical_cast<std::string>(obj->box_zd_opened());
	query << ",";
	{
		uint32_t size = obj->box_ids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->box_ids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "box_ids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "box_open_slot=" << boost::lexical_cast<std::string>(obj->box_open_slot());
	query << ",";
	query << "box_open_time=" << boost::lexical_cast<std::string>(obj->box_open_time());
	query << ",";
	query << "sign_time=" << boost::lexical_cast<std::string>(obj->sign_time());
	query << ",";
	query << "sign_index=" << boost::lexical_cast<std::string>(obj->sign_index());
	query << ",";
	query << "sign_finish=" << boost::lexical_cast<std::string>(obj->sign_finish());
	query << ",";
	{
		uint32_t size = obj->role_guid_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->role_guid(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "role_guid=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "role_on=" << boost::lexical_cast<std::string>(obj->role_on());
	query << ",";
	{
		uint32_t size = obj->item_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->item_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "item_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->item_num_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->item_num(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "item_num=" << mysqlpp::quote << ssm.str();
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
	query << "avatar_on=" << boost::lexical_cast<std::string>(obj->avatar_on());
	query << ",";
	{
		uint32_t size = obj->battle_his_guids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->battle_his_guids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "battle_his_guids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->post_guids_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->post_guids(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "post_guids=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieve_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->achieve_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "achieve_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieve_num_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->achieve_num(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "achieve_num=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieve_reward_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->achieve_reward(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "achieve_reward=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->achieve_time_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->achieve_time(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "achieve_time=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "achieve_point=" << boost::lexical_cast<std::string>(obj->achieve_point());
	query << ",";
	query << "achieve_index=" << boost::lexical_cast<std::string>(obj->achieve_index());
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
		uint32_t size = obj->toukuang_time_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->toukuang_time(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "toukuang_time=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "toukuang_on=" << boost::lexical_cast<std::string>(obj->toukuang_on());
	query << ",";
	{
		uint32_t size = obj->task_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->task_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "task_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->task_num_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->task_num(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "task_num=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->task_reward_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->task_reward(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "task_reward=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->fashion_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->fashion_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "fashion_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->fashion_on_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->fashion_on(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "fashion_on=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->daily_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->daily_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "daily_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->daily_num_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->daily_num(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "daily_num=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->daily_reward_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->daily_reward(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "daily_reward=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "daily_point=" << boost::lexical_cast<std::string>(obj->daily_point());
	query << ",";
	{
		uint32_t size = obj->daily_get_id_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->daily_get_id(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "daily_get_id=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->level_reward_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->level_reward(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "level_reward=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->social_golds_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			uint64_t v = obj->social_golds(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(uint64_t));
		}
		query << "social_golds=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "total_recharge=" << boost::lexical_cast<std::string>(obj->total_recharge());
	query << ",";
	query << "total_spend=" << boost::lexical_cast<std::string>(obj->total_spend());
	query << ",";
	query << "change_name_num=" << boost::lexical_cast<std::string>(obj->change_name_num());
	query << ",";
	query << "fenxiang_num=" << boost::lexical_cast<std::string>(obj->fenxiang_num());
	query << ",";
	query << "fenxiang_total_num=" << boost::lexical_cast<std::string>(obj->fenxiang_total_num());
	query << ",";
	query << "fenxiang_state=" << boost::lexical_cast<std::string>(obj->fenxiang_state());
	query << ",";
	{
		uint32_t size = obj->libao_nums_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->libao_nums(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "libao_nums=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "battle_num=" << boost::lexical_cast<std::string>(obj->battle_num());
	query << ",";
	query << "offline_battle_time=" << boost::lexical_cast<std::string>(obj->offline_battle_time());
	query << ",";
	query << "first_recharge=" << boost::lexical_cast<std::string>(obj->first_recharge());
	query << ",";
	query << "yue_time=" << boost::lexical_cast<std::string>(obj->yue_time());
	query << ",";
	query << "yue_reward=" << boost::lexical_cast<std::string>(obj->yue_reward());
	query << ",";
	query << "yue_first=" << boost::lexical_cast<std::string>(obj->yue_first());
	query << ",";
	query << "nian_time=" << boost::lexical_cast<std::string>(obj->nian_time());
	query << ",";
	query << "nian_reward=" << boost::lexical_cast<std::string>(obj->nian_reward());
	query << ",";
	query << "nian_first=" << boost::lexical_cast<std::string>(obj->nian_first());
	query << ",";
	{
		uint32_t size = obj->duobao_items_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->duobao_items(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "duobao_items=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "duobao_num=" << boost::lexical_cast<std::string>(obj->duobao_num());
	query << ",";
	query << "advertisement_num=" << boost::lexical_cast<std::string>(obj->advertisement_num());
	query << ",";
	query << "advertisement_time=" << boost::lexical_cast<std::string>(obj->advertisement_time());
	query << " WHERE guid=" << boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlplayer_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::player_t *obj = (dhc::player_t *)data_;
	query << "DELETE FROM player_t WHERE guid="
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

int Sqlrole_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::role_t *obj = (dhc::role_t *)data_;
	query << "INSERT INTO role_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "player_guid=" << boost::lexical_cast<std::string>(obj->player_guid());
	query << ",";
	query << "template_id=" << boost::lexical_cast<std::string>(obj->template_id());
	query << ",";
	query << "level=" << boost::lexical_cast<std::string>(obj->level());

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlrole_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::role_t *obj = (dhc::role_t *)data_;
	query << "SELECT * FROM role_t WHERE guid="
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
		obj->set_template_id(res.at(0).at(2));
	}
	if (!res.at(0).at(3).is_null())
	{
		obj->set_level(res.at(0).at(3));
	}
	return 0;
}

int Sqlrole_t::update(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::role_t *obj = (dhc::role_t *)data_;
	query << "UPDATE role_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "player_guid=" << boost::lexical_cast<std::string>(obj->player_guid());
	query << ",";
	query << "template_id=" << boost::lexical_cast<std::string>(obj->template_id());
	query << ",";
	query << "level=" << boost::lexical_cast<std::string>(obj->level());
	query << " WHERE guid=" << boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlrole_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::role_t *obj = (dhc::role_t *)data_;
	query << "DELETE FROM role_t WHERE guid="
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

int Sqlbattle_his_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::battle_his_t *obj = (dhc::battle_his_t *)data_;
	query << "INSERT INTO battle_his_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "player_guid=" << boost::lexical_cast<std::string>(obj->player_guid());
	query << ",";
	query << "battle_guid=" << boost::lexical_cast<std::string>(obj->battle_guid());
	query << ",";
	query << "role_id=" << boost::lexical_cast<std::string>(obj->role_id());
	query << ",";
	query << "type=" << boost::lexical_cast<std::string>(obj->type());
	query << ",";
	query << "rank=" << boost::lexical_cast<std::string>(obj->rank());
	query << ",";
	query << "sha=" << boost::lexical_cast<std::string>(obj->sha());
	query << ",";
	query << "lsha=" << boost::lexical_cast<std::string>(obj->lsha());
	query << ",";
	query << "die=" << boost::lexical_cast<std::string>(obj->die());
	query << ",";
	query << "score=" << boost::lexical_cast<std::string>(obj->score());
	query << ",";
	query << "time=" << boost::lexical_cast<std::string>(obj->time());
	query << ",";
	query << "achieve=" << boost::lexical_cast<std::string>(obj->achieve());

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlbattle_his_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::battle_his_t *obj = (dhc::battle_his_t *)data_;
	query << "SELECT * FROM battle_his_t WHERE guid="
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
		obj->set_battle_guid(res.at(0).at(2));
	}
	if (!res.at(0).at(3).is_null())
	{
		obj->set_role_id(res.at(0).at(3));
	}
	if (!res.at(0).at(4).is_null())
	{
		obj->set_type(res.at(0).at(4));
	}
	if (!res.at(0).at(5).is_null())
	{
		obj->set_rank(res.at(0).at(5));
	}
	if (!res.at(0).at(6).is_null())
	{
		obj->set_sha(res.at(0).at(6));
	}
	if (!res.at(0).at(7).is_null())
	{
		obj->set_lsha(res.at(0).at(7));
	}
	if (!res.at(0).at(8).is_null())
	{
		obj->set_die(res.at(0).at(8));
	}
	if (!res.at(0).at(9).is_null())
	{
		obj->set_score(res.at(0).at(9));
	}
	if (!res.at(0).at(10).is_null())
	{
		obj->set_time(res.at(0).at(10));
	}
	if (!res.at(0).at(11).is_null())
	{
		obj->set_achieve(res.at(0).at(11));
	}
	return 0;
}

int Sqlbattle_his_t::update(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::battle_his_t *obj = (dhc::battle_his_t *)data_;
	query << "UPDATE battle_his_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "player_guid=" << boost::lexical_cast<std::string>(obj->player_guid());
	query << ",";
	query << "battle_guid=" << boost::lexical_cast<std::string>(obj->battle_guid());
	query << ",";
	query << "role_id=" << boost::lexical_cast<std::string>(obj->role_id());
	query << ",";
	query << "type=" << boost::lexical_cast<std::string>(obj->type());
	query << ",";
	query << "rank=" << boost::lexical_cast<std::string>(obj->rank());
	query << ",";
	query << "sha=" << boost::lexical_cast<std::string>(obj->sha());
	query << ",";
	query << "lsha=" << boost::lexical_cast<std::string>(obj->lsha());
	query << ",";
	query << "die=" << boost::lexical_cast<std::string>(obj->die());
	query << ",";
	query << "score=" << boost::lexical_cast<std::string>(obj->score());
	query << ",";
	query << "time=" << boost::lexical_cast<std::string>(obj->time());
	query << ",";
	query << "achieve=" << boost::lexical_cast<std::string>(obj->achieve());
	query << " WHERE guid=" << boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlbattle_his_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::battle_his_t *obj = (dhc::battle_his_t *)data_;
	query << "DELETE FROM battle_his_t WHERE guid="
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

int Sqlpost_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::post_t *obj = (dhc::post_t *)data_;
	query << "INSERT INTO post_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "player_guid=" << boost::lexical_cast<std::string>(obj->player_guid());
	query << ",";
	query << "send_date=" << boost::lexical_cast<std::string>(obj->send_date());
	query << ",";
	query << "title=" << mysqlpp::quote << obj->title();
	query << ",";
	query << "text=" << mysqlpp::quote << obj->text();
	query << ",";
	query << "sender_name=" << mysqlpp::quote << obj->sender_name();
	query << ",";
	{
		uint32_t size = obj->type_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->type(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "type=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->value1_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->value1(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "value1=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->value2_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->value2(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "value2=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->value3_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->value3(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "value3=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "is_read=" << boost::lexical_cast<std::string>(obj->is_read());
	query << ",";
	query << "is_pick=" << boost::lexical_cast<std::string>(obj->is_pick());

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlpost_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::post_t *obj = (dhc::post_t *)data_;
	query << "SELECT * FROM post_t WHERE guid="
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
		obj->set_send_date(res.at(0).at(2));
	}
	if (!res.at(0).at(3).is_null())
	{
		obj->set_title((std::string)res.at(0).at(3));
	}
	if (!res.at(0).at(4).is_null())
	{
		obj->set_text((std::string)res.at(0).at(4));
	}
	if (!res.at(0).at(5).is_null())
	{
		obj->set_sender_name((std::string)res.at(0).at(5));
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
			obj->add_type(v);
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
			obj->add_value1(v);
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
			obj->add_value2(v);
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
			obj->add_value3(v);
		}
	}
	if (!res.at(0).at(10).is_null())
	{
		obj->set_is_read(res.at(0).at(10));
	}
	if (!res.at(0).at(11).is_null())
	{
		obj->set_is_pick(res.at(0).at(11));
	}
	return 0;
}

int Sqlpost_t::update(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::post_t *obj = (dhc::post_t *)data_;
	query << "UPDATE post_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "player_guid=" << boost::lexical_cast<std::string>(obj->player_guid());
	query << ",";
	query << "send_date=" << boost::lexical_cast<std::string>(obj->send_date());
	query << ",";
	query << "title=" << mysqlpp::quote << obj->title();
	query << ",";
	query << "text=" << mysqlpp::quote << obj->text();
	query << ",";
	query << "sender_name=" << mysqlpp::quote << obj->sender_name();
	query << ",";
	{
		uint32_t size = obj->type_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->type(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "type=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->value1_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->value1(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "value1=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->value2_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->value2(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "value2=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	{
		uint32_t size = obj->value3_size();
		std::stringstream ssm;
		ssm.write(reinterpret_cast<char*>(&size), sizeof(uint32_t));
		for (uint32_t i = 0; i < size; i++)
		{
			int32_t v = obj->value3(i);
			ssm.write(reinterpret_cast<char*>(&v), sizeof(int32_t));
		}
		query << "value3=" << mysqlpp::quote << ssm.str();
	}
	query << ",";
	query << "is_read=" << boost::lexical_cast<std::string>(obj->is_read());
	query << ",";
	query << "is_pick=" << boost::lexical_cast<std::string>(obj->is_pick());
	query << " WHERE guid=" << boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlpost_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::post_t *obj = (dhc::post_t *)data_;
	query << "DELETE FROM post_t WHERE guid="
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

int Sqlshare_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::share_t *obj = (dhc::share_t *)data_;
	query << "INSERT INTO share_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "num=" << boost::lexical_cast<std::string>(obj->num());
	query << ",";
	query << "ctime=" << boost::lexical_cast<std::string>(obj->ctime());

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlshare_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::share_t *obj = (dhc::share_t *)data_;
	query << "SELECT * FROM share_t WHERE guid="
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
		obj->set_num(res.at(0).at(1));
	}
	if (!res.at(0).at(2).is_null())
	{
		obj->set_ctime(res.at(0).at(2));
	}
	return 0;
}

int Sqlshare_t::update(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::share_t *obj = (dhc::share_t *)data_;
	query << "UPDATE share_t SET ";
	query << "guid=" << boost::lexical_cast<std::string>(obj->guid());
	query << ",";
	query << "num=" << boost::lexical_cast<std::string>(obj->num());
	query << ",";
	query << "ctime=" << boost::lexical_cast<std::string>(obj->ctime());
	query << " WHERE guid=" << boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlshare_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::share_t *obj = (dhc::share_t *)data_;
	query << "DELETE FROM share_t WHERE guid="
		<< boost::lexical_cast<std::string>(guid_);

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}
