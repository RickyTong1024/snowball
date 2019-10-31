#include "udp_service.h"
#include "service.h"
#include "udp_handler.h"
#include <boost/lexical_cast.hpp>
#include <ace/Reactor.h>
#include <ace/Synch.h>
#include "RakPeerInterface.h"
#include "MessageIdentifiers.h"
#include "BitStream.h"

UdpService::UdpService()
: peer_(0)
, end_(0)
, hid_(0)
, queue_(0)
{
	
}

UdpService::~UdpService()
{
	
}

int UdpService::init(const std::string &name)
{
	name_ = name;
	std::string end = service::server_env()->get_server_value(name, "udp_port");
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

	peer_ = RakNet::RakPeerInterface::GetInstance();
	RakNet::SocketDescriptor sd(end_, 0);
	peer_->Startup(128, &sd, 1);
	peer_->SetMaximumIncomingConnections(128);
	peer_->SetTimeoutTime(3000, RakNet::UNASSIGNED_SYSTEM_ADDRESS);

	return 0;
}

int UdpService::fini()
{
	if (end_ == 0)
	{
		return 0;
	}
	RakNet::RakPeerInterface::DestroyInstance(peer_);
	if (-1 != timer_id_)
	{
		ACE_Reactor::instance()->cancel_timer(timer_id_);
		timer_id_ = -1;
	}
	delete queue_;

	return 0;
}

void UdpService::send_msg(int hid, TPacket *pck)
{
	/// 非同一线程
	ACE_Message_Block *mb = new ACE_Message_Block(sizeof(int));
	mb->copy((char *)&hid, sizeof(int));
	ACE_Message_Block *mb1 = pck->release();
	mb->cont(mb1);
	queue_->enqueue(mb);
	delete pck;
}

void UdpService::destory(int hid)
{
	/// 非同一线程
	ACE_Message_Block *mb = new ACE_Message_Block(sizeof(int));
	mb->copy((char *)&hid, sizeof(int));
	ACE_Message_Block *mb1 = new ACE_Message_Block(sizeof(int));
	mb1->set_flags(ACE_Message_Block::MB_STOP);
	mb->cont(mb1);
	queue_->enqueue(mb);
}

RakNet::RakPeerInterface * UdpService::get_peer()
{
	return peer_;
}

UdpHandler * UdpService::add_handler(const std::string &saddr)
{
	hid_addr_[hid_] = saddr;
	addr_hid_[saddr] = hid_;
	UdpHandler *uh = new UdpHandler(hid_, saddr, this);
	udp_handlers_[hid_] = uh;
	hid_++;
	return uh;
}

void UdpService::del_handler(int hid)
{
	if (udp_handlers_.find(hid) != udp_handlers_.end())
	{
		udp_handlers_.erase(hid);
		addr_hid_.erase(hid_addr_[hid]);
		hid_addr_.erase(hid);
	}
}

UdpHandler * UdpService::get_handler(int hid)
{
	if (udp_handlers_.find(hid) == udp_handlers_.end())
	{
		return 0;
	}
	return udp_handlers_[hid];
}

int UdpService::handle_timeout(const ACE_Time_Value &tv, const void *arg)
{
	for (RakNet::Packet *p = peer_->Receive(); p; peer_->DeallocatePacket(p), p = peer_->Receive())
	{
		switch (p->data[0])
		{
		case ID_DISCONNECTION_NOTIFICATION:
		case ID_CONNECTION_LOST:
			// Connection lost normally
		{
			std::string addr = p->systemAddress.ToString(true);
			if (addr_hid_.find(addr) != addr_hid_.end())
			{
				int hid = addr_hid_[addr];
				UdpHandler *tuh = get_handler(hid);
				tuh->kick(false);
			}
			break;
		}
		case ID_NEW_INCOMING_CONNECTION:
			// Somebody connected.  We have their IP now
		{
			std::string addr = p->systemAddress.ToString(true);
			if (addr_hid_.find(addr) != addr_hid_.end())
			{
				int hid = addr_hid_[addr];
				UdpHandler *tuh = get_handler(hid);
				tuh->kick();
			}
			UdpHandler *uh = add_handler(addr);
			uh->onconnect(addr);
			break;
		}
		case ID_USER_PACKET_ENUM:
		{
			std::string addr = p->systemAddress.ToString(true);
			if (addr_hid_.find(addr) != addr_hid_.end())
			{
				int hid = addr_hid_[addr];
				UdpHandler *uh = get_handler(hid);
				if (uh)
				{
					uh->recv((char *)p->data + 1, p->length - 1);
				}
				else
				{
					service::log()->error("no UdpHandler hid = %d", hid);
				}
			}
			else
			{
				service::log()->error("no addr hid addr = %s", addr.c_str());
			}
			break;
		}
		}
	}

	while (!queue_->is_empty())
	{
		ACE_Message_Block *mb;
		queue_->dequeue(mb);
		int hid = 0;
		memcpy(&hid, mb->rd_ptr(), sizeof(int));
		UdpHandler * uh = get_handler(hid);
		if (!uh)
		{
			mb->release();
			continue;
		}
		ACE_Message_Block *mb1 = mb->cont();
		mb->cont(0);
		mb->release();
		if (mb1->flags() & ACE_Message_Block::MB_STOP)
		{
			uh->kick();
		}
		else
		{
			uh->send(mb1);
		}
		mb1->release();
	}
	return 0;
}
