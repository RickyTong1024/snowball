#ifndef __ROOM_MANAGER_H__
#define __ROOM_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "room_def.h"

class RoomManager : public mmg::DispathService
{
public:
	RoomManager();

	~RoomManager();

	BEGIN_PACKET_MAP
		PACKET_HANDLER(CMSG_ENTER_WORLD_UDP, terminal_enter_world_udp)
		PACKET_HANDLER(CMSG_LEAVE_WORLD_UDP, terminal_leave_world_udp)
		PACKET_HANDLER(CMSG_BATTLE_LINK, terminal_battle_link)
		PACKET_HANDLER(CMSG_BATTLE_IN, terminal_battle_in)
		PACKET_HANDLER(CMSG_BATTLE_STATE, terminal_battle_state)
		PACKET_HANDLER(CMSG_BATTLE_RESET, terminal_battle_reset)
		PACKET_HANDLER(CMSG_BATTLE_OP, terminal_battle_op)
		PACKET_HANDLER(CMSG_BATTLE_END, terminal_battle_end)
		PACKET_HANDLER(CMSG_BATTLE_CODE, terminal_battle_code)
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(PUSH_RC_RM_SET_PLAYER_ROOM, push_rc_rm_set_player_room)
	END_PUSH_MAP

	BEGIN_REQ_MAP
	END_REQ_MAP

	int init(const std::string &master_name, uint64_t battle_guid, int battle_type);

	int fini();

private:
	int terminal_enter_world_udp(Packet *pck);

	int terminal_leave_world_udp(Packet *pck);

	int terminal_battle_link(Packet *pck);

	int terminal_battle_in(Packet *pck);

	int terminal_battle_state(Packet *pck);

	int terminal_battle_reset(Packet *pck);

	int terminal_battle_op(Packet *pck);

	int terminal_battle_end(Packet *pck);

	int terminal_battle_code(Packet *pck);

	int push_rc_rm_set_player_room(Packet *pck, const std::string &name);

	int update(ACE_Time_Value tv);

private:
	int timer_id_;
	std::set<int> hids_;
	std::map<int, uint64_t> hid_guids_;
	std::map<uint64_t, int> guid_hids_;
	std::map<uint64_t, WaitPlayer> wait_players_;
};

#endif
