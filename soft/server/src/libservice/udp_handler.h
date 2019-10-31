#ifndef __UDP_HANDLER_H__
#define __UDP_HANDLER_H__

#include "typedefs.h"
#include <ace/Message_Block.h>
#include <ace/INET_Addr.h>

#define RECV_BUFSIZE 32 * 1024
#define MAX_PCK_SIZE 8 * 1024

class UdpService;
class ACE_Message_Block;

class UdpHandler
{
public:
	UdpHandler(int hid, const std::string &addr, UdpService *us);

	~UdpHandler();

	void onconnect(const std::string &addr);

	void send(ACE_Message_Block *mb);

	void kick(bool is_send = true);

	int recv(const char *data, int len);

protected:
	int feed(ACE_Message_Block &chunk);

private:
	int hid_;
	UdpService *udp_service_;
	ACE_Message_Block chunk_;
	std::string addr_;
};

#endif // !__UDP_HANLDER_H__
