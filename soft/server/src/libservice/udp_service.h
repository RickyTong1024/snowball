#ifndef __UDP_SERVICE_H__
#define __UDP_SERVICE_H__

#include "service_interface.h"
#include <ace/Message_Queue.h>
#include <ace/SOCK_Dgram.h>

class UdpHandler;
namespace RakNet
{
	class RakPeerInterface;
}

class UdpService : public mmg::UdpService, ACE_Event_Handler
{
public:
	UdpService();

	~UdpService();

	int init(const std::string &name);

	int fini();

	virtual void send_msg(int hid, TPacket *pck);

	virtual void destory(int hid);

	RakNet::RakPeerInterface *get_peer();

	UdpHandler * add_handler(const std::string &saddr);

	void del_handler(int hid);

	UdpHandler * get_handler(int hid);

	virtual int handle_timeout(const ACE_Time_Value &tv, const void *arg);

private:
	std::string name_;
	int end_;
	RakNet::RakPeerInterface *peer_;
	std::map<int, UdpHandler *> udp_handlers_;
	std::map<int, std::string> hid_addr_;
	std::map<std::string, int> addr_hid_;
	int hid_;
	ACE_Message_Queue<ACE_MT_SYNCH> *queue_;
	int timer_id_;
};

#endif
