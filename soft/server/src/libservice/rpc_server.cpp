#include "rpc_server.h"
#include <zmq.hpp>
#include <ace/OS_NS_unistd.h>

RpcServer::RpcServer(const std::string &name, const std::string &addr)
	: name_(name)
	, stop_(false)
	, addr_(addr)
{
	this->activate();
}

RpcServer::~RpcServer()
{
	stop_ = true;
	this->wait();
}

void RpcServer::gutq(std::list<std::string> &msgs)
{
	ACE_Guard<ACE_Thread_Mutex> t(chain_);
	msgs = msg_list_;
	msg_list_.clear();
}

int RpcServer::svc()
{
	zmq::context_t context(1);
	zmq::socket_t sub_sock(context, ZMQ_PULL);
	sub_sock.bind(addr_.c_str());
	while (!stop_)
	{
		zmq_pollitem_t items [] = { { sub_sock, 0, ZMQ_POLLIN, 0 } };
		zmq_poll(items, 1, 200);

		if (items[0].revents & ZMQ_POLLIN)
		{
			zmq::message_t message;
			sub_sock.recv(&message);
			std::string rstr((char *)message.data(), message.size());
			{
				ACE_Guard<ACE_Thread_Mutex> t(chain_);
				msg_list_.push_back(rstr);
			}
		}
	}
	sub_sock.close();
	return 0;
}
