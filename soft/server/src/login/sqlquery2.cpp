#include "sqlquery2.h"
#include <sstream>
#include "protocol_inc.h"
#include "utils.h"

int Sqlacc_t::insert(mysqlpp::Connection *conn)
{
	return -1;
}

int Sqlacc_t::query(mysqlpp::Connection *conn)
{
	mysqlpp::Query query = conn->query();
	mysqlpp::Transaction trans(*conn, mysqlpp::Transaction::serializable, mysqlpp::Transaction::session);
	dhc::acc_t *obj = (dhc::acc_t *)data_;
	{
		query << "SELECT guid, gm_level, fenghao_time, rank_forbid FROM acc_t WHERE "
			<< "username=" << mysqlpp::quote << obj->username()
			<< " and "
			<< "serverid=" << boost::lexical_cast<std::string>(obj->serverid());

		mysqlpp::StoreQueryResult res = query.store();

		if (res && res.num_rows() == 1)
		{
			obj->set_guid(res.at(0).at(0));
			obj->set_gm_level(res.at(0).at(1));
			obj->set_fenghao_time(res.at(0).at(2));
			obj->set_rank_forbid(res.at(0).at(3));
		}
		else if (res.num_rows() > 1)
		{
			return -1;
		}
		else
		{
			uint64_t max = 0;
			{
				query << "SELECT max(guid) FROM acc_t where "
					<< "serverid=" << boost::lexical_cast<std::string>(obj->serverid());

				mysqlpp::StoreQueryResult res1 = query.store();

				if (res1 && res1.num_rows() == 1)
				{
					if (!res1.at(0).at(0).is_null())
					{
						max = res1.at(0).at(0);
						max += 1;
					}
					else
					{
						max = MAKE_GUID_EX(et_player, obj->serverid(), 0);
					}
				}
				else
				{
					return -1;
				}
			}

			{
				query << "INSERT INTO acc_t SET "
					<< "guid=" << boost::lexical_cast<std::string>(max)
					<< ","
					<< "username=" << mysqlpp::quote << obj->username()
					<< ","
					<< "serverid=" << boost::lexical_cast<std::string>(obj->serverid())
					<< ","
					<< "gm_level=0"
					<< ","
					<< "fenghao_time=0"
					<< ","
					<< "rank_forbid=0"
					<< ","
					<< "dt=now()";

				mysqlpp::SimpleResult res1 = query.execute();
				if (!res1)
				{
					service::log()->error(query.error());
					return -1;
				}
				obj->set_guid(max);
				obj->set_gm_level(0);
				obj->set_fenghao_time(0);
				obj->set_rank_forbid(0);
			}
		}
	}
	trans.commit();

	return 0;
}

int Sqlacc_t::update(mysqlpp::Connection *conn)
{
	return -1;
}

int Sqlacc_t::remove(mysqlpp::Connection *conn)
{
	return -1;
}
