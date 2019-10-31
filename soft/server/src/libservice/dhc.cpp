#include "dhc.h"
#include "mysql++.h"
#include <ace/OS_NS_unistd.h>
#include "service.h"

Dhc::Dhc()
: conn_(0)
, stop_(true)
, timer_id_(-1)
{

}

Dhc::~Dhc()
{

}

int Dhc::init(mmg::DhcRequest *dr, const std::string &db, bool async)
{
	dr_ = dr;
	async_ = async;
	conn_ = new mysqlpp::Connection;
	conn_->disable_exceptions();
	conn_->set_option(new mysqlpp::ReconnectOption(true));
	conn_->set_option(new mysqlpp::SetCharsetNameOption("utf8"));
	int port = boost::lexical_cast<int>(service::server_env()->get_db_value(db, "port"));
	int ok = conn_->connect(service::server_env()->get_db_value(db, "db").c_str(), service::server_env()->get_db_value(db, "host").c_str(), service::server_env()->get_db_value(db, "username").c_str(), service::server_env()->get_db_value(db, "password").c_str(), port);
	if (!ok)
	{
		printf("Failed to connect database.\n");
		return -1;
	}

	if (async_)
	{
		timer_id_ = service::timer()->schedule(boost::bind(&Dhc::update, this, _1), 30, "dhc");
		if (-1 == timer_id_)
		{
			return -1;
		}

		stop_ = false;
		this->activate();
	}
	return 0;
}

int Dhc::fini()
{
	if (async_)
	{
		stop_ = true;
		this->wait();

		if (-1 != timer_id_)
		{
			service::timer()->cancel(timer_id_);
			timer_id_ = -1;
		}
	}
	conn_->disconnect();
	delete conn_;
	conn_ = 0;
	return 0;
}

int Dhc::do_request(Request *req)
{
	conn_->ping();
	int res = -1;
	switch(req->op())
	{
	case opc_insert:
		res = dr_->do_insert(conn_, req->guid(), req->data());
		break;
	case opc_query:
		res = dr_->do_query(conn_, req->guid(), req->data());
		if (req->data())
		{
			req->data()->clear_changed();
		}
		break;
	case opc_update:
		res = dr_->do_update(conn_, req->guid(), req->data());
		break;
	case opc_remove:
		res = dr_->do_remove(conn_, req->guid(), req->data());
		break;
	}

	req->set_result(res);
	return res;
}

bool Dhc::full()
{
	ACE_Guard<ACE_Thread_Mutex> t(chain_);
	return upcaller_.size() > 2000;
}

int Dhc::upcall(Request *req, Upcaller caller)
{
	ACE_Guard<ACE_Thread_Mutex> t(chain_);
	upcaller_.push_back(std::pair<Request *, Upcaller>(req, caller));
	return 0;
}

int Dhc::svc()
{
	upcall_svc();
	return 0;
}

int Dhc::upcall_svc()
{
	while (1)
	{
		bool sflag = false;
		{
			ACE_Guard<ACE_Thread_Mutex> t(chain_);
			if (upcaller_.empty())
			{
				sflag = true;
			}
		}
		if (stop_ && sflag)
		{
			break;
		}
		bool flag = true;
		while (flag)
		{
			std::pair<Request *, Upcaller> ru;
			flag = false;
			{
				ACE_Guard<ACE_Thread_Mutex> t(chain_);
				if (!upcaller_.empty())
				{
					ru = upcaller_.front();
					upcaller_.pop_front();
					flag = true;
				}
			}

			if (flag)
			{
				do_request(ru.first);
				{
					ACE_Guard<ACE_Thread_Mutex> t(chain1_);
					docaller_.push_back(ru);
				}
			}
		}
		ACE_Time_Value tv(0, 30 * 1000);
		ACE_OS::sleep(tv);
	}
	return 0;
}

int Dhc::update(const ACE_Time_Value &cur)
{
	doupcall();
	return 0;
}

int Dhc::doupcall()
{
	while (!stop_)
	{
		std::pair<Request *, Upcaller> ru;
		{
			ACE_Guard<ACE_Thread_Mutex> t(chain1_);
			if (docaller_.empty())
			{
				break;
			}
			ru = docaller_.front();
			docaller_.pop_front();
		}
		Upcaller &up = ru.second;
		if (up != 0)
		{
			up(ru.first);
		}
		delete ru.first;
	}
	return 0;
}
