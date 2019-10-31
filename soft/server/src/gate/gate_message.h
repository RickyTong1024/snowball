#ifndef __GATE_MESSAGE_H__
#define __GATE_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class GateMessage
{
public:
	static void send_smsg_error(int hid, operror_t error);

	static void send_push_gate_player_num(int num);

	static void send_push_default(Packet *pck);

	static void send_push_default_login(Packet *pck);

	static void send_terminal_default(Packet *pck, int hid);
};

#endif
