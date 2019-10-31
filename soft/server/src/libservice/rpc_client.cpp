#include "rpc_client.h"
#include <zmq.hpp>
#include <ace/OS_NS_unistd.h>

RpcClient::RpcClient(const std::string &name, const std::string &addr)
	: name_(name)
	, stop_(true)
	, addr_(addr)
{
	this->activate();
}

RpcClient::~RpcClient()
{
	if (!stop_)
	{
		stop_ = true;
		this->wait();
	}
}

void RpcClient::putq(const std::string &msg)
{
	ACE_Guard<ACE_Thread_Mutex> t(chain_);
	msg_list_.push_back(msg);
}

int RpcClient::svc()
{
	zmq::context_t context(1);
	zmq::socket_t sub_sock(context, ZMQ_PUSH);
	int queue_length = 1;
	sub_sock.connect(addr_.c_str());
	stop_ = false;
	while (!stop_)
	{
		bool flag = true;
		while(flag)
		{
			flag = false;
			std::string s;
			{
				ACE_Guard<ACE_Thread_Mutex> t(chain_);
				if (!msg_list_.empty())
				{
					flag = true;
					s = msg_list_.front();
					msg_list_.pop_front();
				}
			}

			if (flag)
			{
				zmq::message_t message(s.size());
				memcpy(message.data(), s.data(), s.size());
				sub_sock.send(message, ZMQ_DONTWAIT);
			}
		}

		ACE_Time_Value tv(0, 20000);
		ACE_OS::sleep(tv);
	}
	return 0;
}
