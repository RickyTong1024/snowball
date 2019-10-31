#include "gtool.h"
#include <ace/OS_NS_sys_time.h>
#include "dhc.h"
#include "utils.h"
#include <mysql++.h>
#include "service.h"
#include "service_inc.h"

GTool::GTool()
: dhc_(0)
, qid_(0)
, one_add_(0)
, cur_guid_(0)
, next_guid_(0)
, start_time_(0)
{
	
}

GTool::~GTool()
{

}

int GTool::init(const std::string &db)
{
	dhc_ = new Dhc();
	if (-1 == dhc_->init(this, db, false))
	{
		return -1;
	}

	qid_ = boost::lexical_cast<int>(service::server_env()->get_game_value("qid"));
	one_add_ = boost::lexical_cast<int>(service::server_env()->get_game_value("one_add"));

	get_guid();
	get_start_time();

	return 0;
}

int GTool::fini()
{
	dhc_->fini();
	delete dhc_;
	dhc_ = 0;

	return 0;
}

uint64_t GTool::assign (int et)
{
	uint64_t guid = MAKE_GUID_EX(et, qid_, cur_guid_);
	cur_guid_++;
	if (cur_guid_ > next_guid_)
	{
		get_guid();
	}
	return guid;
}

uint64_t GTool::start_time()
{
	return start_time_;
}

int GTool::get_guid()
{
	Request req;
	req.add(opc_query, MAKE_GUID_EX(et_gtool, qid_, 0), 0);
	
	if (dhc_->do_request(&req) != -1)
	{
		return 0;
	}
	return -1;
}

void GTool::get_start_time()
{
	Request req;
	req.add(opc_query, MAKE_GUID_EX(et_gtool, qid_, 1), 0);

	if (dhc_->do_request(&req) != -1)
	{
		return;
	}
}

int GTool::do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	return -1;
}

int GTool::do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (guid == MAKE_GUID_EX(et_gtool, qid_, 0))
	{
		{
			mysqlpp::Query query = conn->query();
			mysqlpp::Transaction trans(*conn, mysqlpp::Transaction::serializable, mysqlpp::Transaction::session);

			query << "SELECT num FROM gtool_t WHERE guid="
			<< boost::lexical_cast<std::string>(guid);

			mysqlpp::StoreQueryResult res = query.store();

			if (res && res.num_rows() == 1)
			{
				if (!res.at(0).at(0).is_null())
				{
					cur_guid_ = res.at(0).at(0);
					next_guid_ = cur_guid_ + one_add_;
				}

				query << "UPDATE gtool_t SET ";
				query << "num=" << boost::lexical_cast<std::string>(next_guid_);
				query << " WHERE guid=" << boost::lexical_cast<std::string>(guid);

				mysqlpp::SimpleResult res = query.execute();

				if (!res)
				{
					service::log()->error(query.error());
					return -1;
				}
			}
			else
			{
				query << "INSERT INTO gtool_t SET ";
				query << "guid=" << boost::lexical_cast<std::string>(guid);
				query << ",";
				query << "num=" << boost::lexical_cast<std::string>(one_add_);

				mysqlpp::SimpleResult res = query.execute();

				if (!res)
				{
					service::log()->error(query.error());
					return -1;
				}
				cur_guid_ = 0;
				next_guid_ = cur_guid_ + one_add_;
			}
			trans.commit();
		}
	}
	else
	{
		{
			mysqlpp::Query query = conn->query();
			mysqlpp::Transaction trans(*conn, mysqlpp::Transaction::serializable, mysqlpp::Transaction::session);

			query << "SELECT num FROM gtool_t WHERE guid="
				<< boost::lexical_cast<std::string>(guid);

			mysqlpp::StoreQueryResult res = query.store();

			if (res && res.num_rows() == 1)
			{
				if (!res.at(0).at(0).is_null())
				{
					start_time_ = res.at(0).at(0);
				}
			}
			else
			{
				query << "INSERT INTO gtool_t SET ";
				query << "guid=" << boost::lexical_cast<std::string>(guid);
				query << ",";
				query << "num=" << boost::lexical_cast<std::string>(service::timer()->now());

				mysqlpp::SimpleResult res = query.execute();

				if (!res)
				{
					service::log()->error(query.error());
					return -1;
				}
			}
			trans.commit();
		}
	}
	return -1;
}

int GTool::do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	return -1;
}

int GTool::do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	return -1;
}
