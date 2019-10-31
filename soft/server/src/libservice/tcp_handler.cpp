#include "tcp_handler.h"
#include "tcp_service.h"
#include "packet.h"
#include "service.h"

TcpHandler::TcpHandler()
: chunk_(RECV_BUFSIZE)
{
	queue_ = ACE_Message_Queue_Factory<ACE_NULL_SYNCH>::create_static_message_queue(8 * 1024, 8 * 1024);
}

TcpHandler::~TcpHandler()
{
	delete queue_;
}

int TcpHandler::open(void *p)
{
	if (ACE_Svc_Handler<ACE_SOCK_STREAM, ACE_NULL_SYNCH>::open(p) == -1)
	{
		return -1;
	}

	if (this->reactor()->register_handler(this, ACE_Event_Handler::WRITE_MASK))
	{
		return -1;
	}
	this->reactor()->cancel_wakeup(this, ACE_Event_Handler::WRITE_MASK);

	ACE_INET_Addr peer_addr;
	if (this->peer().get_remote_addr(peer_addr) == 0 && peer_addr.addr_to_string(peer_name_, 512) == 0)
	{
		service::log()->debug("tcp connect from %s", peer_name_);
	}
	tcp_service_->add_handler(hid_, this);
	Packet *pck = Packet::New(30001, hid_, 0, "");
	service::logic_service()->add_msg(pck);

	return 0;
}

int TcpHandler::handle_input(ACE_HANDLE handle)
{
	ssize_t cnt = peer().recv(chunk_.wr_ptr(), chunk_.space());
	if (cnt <= 0 && errno == EWOULDBLOCK)
	{
		return 0;
	}
	else if (cnt <= 0)
	{
		this->destory_this();
		return 0;
	}
	else
	{
		chunk_.wr_ptr(cnt);
		int res = feed(chunk_);
		if (res == -1)
		{
			this->kick();
			return 0;
		}
		chunk_.crunch();
	}
	return 0;
}

int TcpHandler::handle_output(ACE_HANDLE handle)
{
	while (!queue_->is_empty())
	{
		int res = send();
		if (res == -1)
		{
			return 0;
		}
		else if (res == 1)
		{
			break;
		}
	}

	if (queue_->is_empty())
	{
		this->reactor()->cancel_wakeup(this, ACE_Event_Handler::WRITE_MASK);
	}
	return 0;
}

int TcpHandler::handle_close(ACE_HANDLE handle, ACE_Reactor_Mask close_mask)
{
	if (!service::is_stop())
	{
		Packet *pck = Packet::New(30002, hid_, 0, "");
		service::logic_service()->add_msg(pck);
		tcp_service_->del_handler(hid_);
	}
	return ACE_Svc_Handler<ACE_SOCK_STREAM, ACE_NULL_SYNCH>::handle_close(handle, close_mask);
}

void TcpHandler::add_msg(ACE_Message_Block *msg)
{
	bool output_off = queue_->is_empty();
	ACE_Time_Value nowait(ACE_Time_Value::zero);
	if (-1 == queue_->enqueue(msg, &nowait))
	{
		msg->release();
		this->kick();
		return;
	}
	int res = send();
	if (res == -1)
	{
		return;
	}

	if (output_off && !queue_->is_empty())
	{
		this->reactor()->schedule_wakeup(this, ACE_Event_Handler::WRITE_MASK);
	}
}

int TcpHandler::send()
{
	ACE_Message_Block *mb;
	queue_->dequeue(mb);
	ssize_t send_cnt = this->peer().send(mb->rd_ptr(), mb->length());
	if (send_cnt <= 0 && errno == EWOULDBLOCK)
	{
		queue_->enqueue_head(mb);
		return 1;
	}
	else if (send_cnt <= 0)
	{
		mb->release();
		this->destory_this();
		return -1;
	}
	else
	{
		mb->rd_ptr(send_cnt);
		if (mb->length() > 0)
		{
			queue_->enqueue_head(mb);
			return 1;
		}
		else
		{
			mb->release();
		}
	}
	return 0;
}

int TcpHandler::destory_this()
{
	service::log()->debug("disconnect from %s", peer_name_);
	this->close();
	return 0;
}

int TcpHandler::kick()
{
	service::log()->debug("kick from %s", peer_name_);
	this->close();
	return 0;
}

int TcpHandler::feed(ACE_Message_Block &chunk)
{
	do
	{
		int size = TPacket::TestSize(chunk_);
		if (size > MAX_PCK_SIZE)
		{
			service::log()->error("pck too large");
			return -1;
		}
		TPacket *pck = TPacket::Extract(chunk_);
		if (!pck)
		{
			break;
		}
		Packet *pck1 = Packet::New(pck->opcode(), hid_, 0, pck->body(), pck->size(), pck->compress());
		delete pck;
		service::logic_service()->add_msg(pck1);
	} while (1);

	return 0;
}

//////////////////////////////////////////////////////////////////////////

TcpAcceptor::TcpAcceptor(TcpService *tcp_service)
: hid_(0)
, tcp_service_(tcp_service)
{

}

TcpAcceptor::~TcpAcceptor()
{

}

int TcpAcceptor::make_svc_handler(TcpHandler *&sh)
{
	ACE_Acceptor<TcpHandler, ACE_SOCK_ACCEPTOR>::make_svc_handler(sh);
	sh->init(hid_, tcp_service_);
	hid_++;
	return 0;
}
