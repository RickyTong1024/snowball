#include "tcp_service.h"
#include "service.h"
#include "tcp_handler.h"
#include <ace/INET_Addr.h>
#include <boost/lexical_cast.hpp>
#include "packet.h"

TcpService::TcpService()
: end_(0)
, acceptor_(0)
, queue_(0)
, timer_id_(-1)
{
	
}

TcpService::~TcpService()
{
	
}

int TcpService::init(const std::string &name)
{
	name_ = name;
	std::string end = service::server_env()->get_server_value(name, "tcp_port");
	int tick = boost::lexical_cast<int>(service::server_env()->get_game_value("tick"));
	if (end != "")
	{
		end_ = boost::lexical_cast<int>(end);
	}
	else
	{
		return 0;
	}

	queue_ = ACE_Message_Queue_Factory<ACE_MT_SYNCH>::create_static_message_queue(64 * 1024 * 1024, 64 * 1024 * 1024);
	ACE_Time_Value tv(0, 30 * 1000);
	timer_id_ = ACE_Reactor::instance()->schedule_timer(this, 0, ACE_Time_Value::zero, tv);

	ACE_INET_Addr addr(end_);
	acceptor_ = new TcpAcceptor(this);
	if (acceptor_->open(addr, ACE_Reactor::instance(), ACE_NONBLOCK) == -1)
	{
		return -1;
	}

	return 0;
}

int TcpService::fini()
{
	if (!end_)
	{
		return 0;
	}

	delete acceptor_;
	if (-1 != timer_id_)
	{
		ACE_Reactor::instance()->cancel_timer(timer_id_);
		timer_id_ = -1;
	}
	delete queue_;

	return 0;
}

void TcpService::send_msg(int hid, TPacket *pck)
{
	/// 非同一线程
	ACE_Message_Block *mb = new ACE_Message_Block(sizeof(int));
	mb->copy((char *)&hid, sizeof(int));
	ACE_Message_Block *mb1 = pck->release();
	mb->cont(mb1);
	queue_->enqueue(mb);
	delete pck;
}

void TcpService::destory(int hid)
{
	/// 非同一线程
	ACE_Message_Block *mb = new ACE_Message_Block(sizeof(int));
	mb->copy((char *)&hid, sizeof(int));
	ACE_Message_Block *mb1 = new ACE_Message_Block(sizeof(int));
	mb1->set_flags(ACE_Message_Block::MB_STOP);
	mb->cont(mb1);
	queue_->enqueue(mb);
}

void TcpService::add_handler(int hid, TcpHandler *th)
{
	tcp_handlers_[hid] = th;
}

void TcpService::del_handler(int hid)
{
	tcp_handlers_.erase(hid);
}

TcpHandler * TcpService::get_handler(int hid)
{
	if (tcp_handlers_.find(hid) == tcp_handlers_.end())
	{
		return 0;
	}
	return tcp_handlers_[hid];
}

int TcpService::handle_timeout(const ACE_Time_Value &tv, const void *arg)
{
	while (!queue_->is_empty())
	{
		ACE_Message_Block *mb;
		queue_->dequeue(mb);
		int hid = 0;
		memcpy(&hid, mb->rd_ptr(), sizeof(int));
		TcpHandler * th = get_handler(hid);
		if (!th)
		{
			mb->release();
			continue;
		}
		ACE_Message_Block *mb1 = mb->cont();
		mb->cont(0);
		mb->release();
		if (mb1->flags() & ACE_Message_Block::MB_STOP)
		{
			th->kick();
			mb1->release();
		}
		else
		{
			th->add_msg(mb1);
		}
	}
	return 0;
}
