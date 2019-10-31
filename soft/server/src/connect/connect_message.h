#ifndef __CONNECT_MESSAGE_H__
#define __CONNECT_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class ConnectMessage
{
public:
	static void send_smsg_error(operror_t error, int hid);

	static void send_smsg_request_gate(const std::string &ip, int port, int hid);
};
#endif
