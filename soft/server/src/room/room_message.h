#ifndef __ROOM_MESSAGE_H__
#define __ROOM_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "room_def.h"

class RoomMessage
{
public:
	static void send_smsg_battle_link(uint64_t guid, uint64_t battle_guid, int type, int team_num, int member_num, int zhen, int rszhen, const std::string &rsdata, const std::vector<protocol::game::msg_battle_op *> &op_all, int op_start_index, bool is_no_body, int seed, int seed_add, WaitPlayer &player);

	static void send_smsg_battle_op(protocol::game::msg_battle_op *op);

	static void send_smsg_battle_zhen();

	static void send_smsg_battle_finish();

	static void send_push_rm_rc_battle_end(uint64_t battle_guid, const std::string &master_name, const std::string &result);

	static void send_push_r_rm_created(uint64_t battle_guid, const std::string &master_name);
};

#endif
