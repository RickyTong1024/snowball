#ifndef __TCP_SERVICE_H__
#define __TCP_SERVICE_H__

#include "service_interface.h"
#include <ace/Message_Queue.h>

class TcpAcceptor;
class TcpHandler;

class TcpService : public mmg::TcpService, ACE_Event_Handler
{
public:
	TcpService();

	~TcpService();

	int init(const std::string &name);

	int fini();

	virtual void send_msg(int hid, TPacket *pck);

	virtual void destory(int hid);

	void add_handler(int hid, TcpHandler *th);

	void del_handler(int hid);

	TcpHandler * get_handler(int hid);

	virtual int handle_timeout(const ACE_Time_Value &tv, const void *arg);

private:
	std::string name_;
	int end_;
	TcpAcceptor *acceptor_;
	std::map<int, TcpHandler *> tcp_handlers_;
	ACE_Message_Queue<ACE_MT_SYNCH> *queue_;
	int timer_id_;
};

#endif // !__RPC_SERVICE_H__
