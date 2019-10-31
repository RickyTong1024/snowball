#include "sqlquery2.h"
#include <sstream>
#include "protocol_inc.h"

int Sqlpost_new_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	dhc::post_new_t *obj = (dhc::post_new_t *)data_;
	query << "INSERT INTO post_new_t SET ";
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

	mysqlpp::SimpleResult res = query.execute();

	if (!res)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

int Sqlpost_new_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	protocol::game::msg_post_new_list *obj_list = (protocol::game::msg_post_new_list *)data_;
	query << "SELECT pid,player_guid,send_date,title,text,sender_name,type,value1,value2,value3 FROM post_new_t where player_guid=";
	query << boost::lexical_cast<std::string>(obj_list->player_guid());

	mysqlpp::StoreQueryResult res = query.store();

	if (!res)
	{
		return -1;
	}

	for (int i = 0; i < res.num_rows(); ++i)
	{
		dhc::post_new_t* obj = obj_list->add_post_news();
		if (!res.at(i).at(0).is_null())
		{
			obj->set_pid(res.at(i).at(0));
		}
		if (!res.at(i).at(1).is_null())
		{
			obj->set_player_guid(res.at(i).at(1));
		}
		if (!res.at(i).at(2).is_null())
		{
			obj->set_send_date(res.at(i).at(2));
		}
		if (!res.at(i).at(3).is_null())
		{
			obj->set_title((std::string)res.at(i).at(3));
		}
		if (!res.at(i).at(4).is_null())
		{
			obj->set_text((std::string)res.at(i).at(4));
		}
		if (!res.at(i).at(5).is_null())
		{
			obj->set_sender_name((std::string)res.at(i).at(5));
		}
		if (!res.at(i).at(6).is_null())
		{
			std::string temp(res.at(i).at(6).data(), res.at(i).at(6).length());
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
		if (!res.at(i).at(7).is_null())
		{
			std::string temp(res.at(i).at(7).data(), res.at(i).at(7).length());
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
		if (!res.at(i).at(8).is_null())
		{
			std::string temp(res.at(i).at(8).data(), res.at(i).at(8).length());
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
		if (!res.at(i).at(9).is_null())
		{
			std::string temp(res.at(i).at(9).data(), res.at(i).at(9).length());
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
	}

	return 0;
}

int Sqlpost_new_t::update(mysqlpp::Connection *conn)
{
	return -1;
}

int Sqlpost_new_t::remove(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	protocol::game::msg_post_new_delete *obj = (protocol::game::msg_post_new_delete *)data_;
	query << "DELETE FROM post_new_t where player_guid=";
	query << boost::lexical_cast<std::string>(obj->player_guid());
	mysqlpp::SimpleResult res1 = query.execute();

	if (!res1)
	{
		service::log()->error(query.error());
		return -1;
	}
	return 0;
}

//////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////

int Sqlrecharge_t::insert(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	mysqlpp::Transaction trans(*conn, mysqlpp::Transaction::serializable, mysqlpp::Transaction::session);
	dhc::recharge_t *obj = (dhc::recharge_t *)data_;

	{
		query << "SELECT * FROM recharge_t WHERE "
			<< "orderno=" << mysqlpp::quote << obj->orderno();

		mysqlpp::StoreQueryResult res = query.store();

		if (res && res.num_rows() >= 1)
		{
			return -1;
		}
	}

	{
		query << "INSERT INTO recharge_t SET "
			<< "orderno=" << mysqlpp::quote << obj->orderno()
			<< ","
			<< "rid=" << boost::lexical_cast<std::string>(obj->rid())
			<< ","
			<< "player_guid=" << boost::lexical_cast<std::string>(obj->player_guid())
			<< ","
			<< "dt=now()";

		mysqlpp::SimpleResult res = query.execute();
		if (!res)
		{
			service::log()->error(query.error());
			return -1;
		}
	}
	trans.commit();
	return 0;
}

int Sqlrecharge_t::query(mysqlpp::Connection *conn)
{
	return -1;
}

int Sqlrecharge_t::update(mysqlpp::Connection *conn)
{
	return -1;
}

int Sqlrecharge_t::remove(mysqlpp::Connection *conn)
{
	return -1;
}
