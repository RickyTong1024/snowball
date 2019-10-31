#ifndef __TCP_HANDLER_H__
#define __TCP_HANDLER_H__

#include "typedefs.h"
#include <ace/Svc_Handler.h>
#include <ace/SOCK_Acceptor.h>
#include <ace/SOCK_Stream.h>
#include <ace/Acceptor.h>

#define RECV_BUFSIZE 32 * 1024
#define MAX_PCK_SIZE 8 * 1024

class TcpService;

class TcpHandler : public ACE_Svc_Handler<ACE_SOCK_STREAM, ACE_NULL_SYNCH>
{
public:
	TcpHandler();

	~TcpHandler();

	void init(int num, TcpService *tcp_service) { hid_ = num; tcp_service_ = tcp_service; }

	virtual int open(void *p);

	virtual int handle_input(ACE_HANDLE handle);

	virtual int handle_output(ACE_HANDLE handle);

	virtual int handle_close(ACE_HANDLE handle, ACE_Reactor_Mask close_mask);

	void add_msg(ACE_Message_Block *msg);

	int send();

	int destory_this();

	int kick();

	int feed(ACE_Message_Block &chunk);

private:
	int hid_;
	TcpService *tcp_service_;
	ACE_Message_Block chunk_;
	ACE_TCHAR peer_name_[512];
	ACE_Message_Queue<ACE_NULL_SYNCH> *queue_;
};

class TcpAcceptor : public ACE_Acceptor<TcpHandler, ACE_SOCK_ACCEPTOR>
{
public:
	TcpAcceptor(TcpService *tcp_service);

	~TcpAcceptor();

	virtual int make_svc_handler(TcpHandler *&sh);

private:
	int hid_;
	TcpService *tcp_service_;
};

#endif // !__TCP_HANLDER_H__
