#ifndef __ROOM_MASTER_MESSAGE_H__
#define __ROOM_MASTER_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class RpcClient;

class RoomMasterMessage
{
public:
	static void send_rep_rc_rm_create_room(const std::string &name, int id, int error_code, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port);

	static void send_push_default_room(Packet *pck, RpcClient *rc);

	static void send_push_default_room_center(Packet *pck);

	static void send_push_rm_rc_battle_end(uint64_t battle_guid);
};

#endif
