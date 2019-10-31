#include "service.h"
#include "service_interface.h"
#include "server_env.h"
#include "timer.h"
#include "logic_service.h"
#include "rpc_service.h"
#include "sig.h"
#include "udp_service.h"
#include "tcp_service.h"
#include "log.h"
#include "pool.h"
#include "scheme.h"
#include <ace/Reactor.h>

#ifdef _WIN32
#include <ace/Select_Reactor.h>
#else
#include <ace/Select_Reactor.h>
#include <ace/Dev_Poll_Reactor.h>
#endif

namespace service
{
	std::string name_;
	ServerEnv *server_env_;
	Log *log_;
	Timer *timer_;
	LogicService *logic_service_;
	RpcService *rs_;
	Sig *sig_;
	TcpService *ts_;
	UdpService *us_;
	Pool *pool_;
	Scheme *scheme_;
	bool stop_ = false;

	int init(const std::string &name, const std::string &confpath, mmg::DispathService *ds, const std::string &self)
	{
		name_ = name;
		server_env_ = new ServerEnv();
		if (-1 == server_env_->init(name, confpath, self))
		{
			return -1;
		}

#ifdef _WIN32
		ACE_Select_Reactor *select_reactor = new ACE_Select_Reactor();
		ACE_Reactor *reactor = new ACE_Reactor(select_reactor);
		ACE_Reactor::instance(reactor, true);
#else

		if (server_env_->get_server_value(name, "reactor") == "epoll")
		{
			ACE_Dev_Poll_Reactor *dev_reactor = new ACE_Dev_Poll_Reactor();
			ACE_Reactor *reactor = new ACE_Reactor(dev_reactor);
			ACE_Reactor::instance(reactor, true);
		}
		else
		{
			ACE_Select_Reactor *select_reactor = new ACE_Select_Reactor();
			ACE_Reactor *reactor = new ACE_Reactor(select_reactor);
			ACE_Reactor::instance(reactor, true);
		}
#endif

		log_ = new Log();
		if (-1 == log_->init(name))
		{
			return -1;
		}

		timer_ = new Timer();
		if (-1 == timer_->init())
		{
			return -1;
		}

		rs_ = new RpcService();
		if (-1 == rs_->init(name, ds))
		{
			return -1;
		}

		sig_ = new Sig();
		if (-1 == sig_->init())
		{
			return -1;
		}

		ts_ = new TcpService();
		if (-1 == ts_->init(name))
		{
			return -1;
		}

		us_ = new UdpService();
		if (-1 == us_->init(name))
		{
			return -1;
		}

		logic_service_ = new LogicService();
		if (-1 == logic_service_->init(ds))
		{
			return -1;
		}

		pool_ = new Pool();
		if (-1 == pool_->init())
		{
			return -1;
		}

		scheme_ = new Scheme();

		printf("%s init\n", name_.c_str());

		return 0;
	}

	int run()
	{
		timer_->start();
		ACE_Reactor::instance()->run_reactor_event_loop();
		timer_->stop();
		return 0;
	}

	int fini()
	{
		stop_ = true;

		delete scheme_;

		pool_->fini();
		delete pool_;

		logic_service_->fini();
		delete logic_service_;

		us_->fini();
		delete us_;

		ts_->fini();
		delete ts_;

		sig_->fini();
		delete sig_;

		rs_->fini();
		delete rs_;

		timer_->fini();
		delete timer_;

		log_->fini();
		delete log_;

		server_env_->fini();
		delete server_env_;

		printf("%s fini\n", name_.c_str());
		return 0;
	}

	bool is_stop()
	{
		return stop_;
	}

	mmg::ServerEnv * server_env()
	{
		return server_env_;
	}

	mmg::Timer *timer()
	{
		return timer_;
	}

	mmg::LogicService *logic_service()
	{
		return logic_service_;
	}

	mmg::RpcService * rpc_service()
	{
		return rs_;
	}

	mmg::TcpService * tcp_service()
	{
		return ts_;
	}

	mmg::UdpService * udp_service()
	{
		return us_;
	}

	mmg::Log *log()
	{
		return log_;
	}

	mmg::Pool *pool()
	{
		return pool_;
	}

	mmg::Scheme *scheme()
	{
		return scheme_;
	}

	const std::string & get_name()
	{
		return name_;
	}
}
