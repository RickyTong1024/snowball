#ifndef __SERVICE_H__
#define __SERVICE_H__

#include <string>

namespace mmg
{
	class ServerEnv;
	class Log;
	class Timer;
	class LogicService;
	class RpcService;
	class TcpService;
	class UdpService;
	class Pool;
	class DispathService;
	class Scheme;
}

namespace service
{
	int init(const std::string &name, const std::string &confpath, mmg::DispathService *ds, const std::string &self = "");

	int run();

	int fini();

	bool is_stop();

	mmg::ServerEnv * server_env();

	mmg::Log *log();

	mmg::Timer * timer();

	mmg::LogicService *logic_service();

	mmg::RpcService * rpc_service();

	mmg::TcpService * tcp_service();

	mmg::UdpService * udp_service();

	mmg::Pool * pool();

	mmg::Scheme * scheme();

	const std::string & get_name();
}

#endif
