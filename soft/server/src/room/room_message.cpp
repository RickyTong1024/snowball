#include "room_message.h"
#include "room_pool.h"

std::map<int, uint64_t> hid_time;

void RoomMessage::send_smsg_battle_link(uint64_t guid, uint64_t battle_guid, int type, int team_num, int member_num, int zhen, int rszhen, const std::string &rsdata, const std::vector<protocol::game::msg_battle_op *> &op_all, int op_start_index, bool is_no_body, int seed, int seed_add, WaitPlayer &player)
{
	int hid = sRoomPool->get_hid(guid);
	if (-1 == hid)
	{
		return;
	}
	if (hid_time.find(hid) != hid_time.end())
	{
		if (hid_time[hid] + 500 > service::timer()->now())
		{
			return;
		}
	}
	hid_time[hid] = service::timer()->now();

	protocol::game::smsg_battle_link msg;
	msg.set_battle_guid(battle_guid);
	msg.set_type(type);
	msg.set_team_num(team_num);
	msg.set_member_num(member_num);
	msg.set_zhen(zhen);
	msg.set_seed(seed);
	msg.set_seed_add(seed_add);
	if (is_no_body)
	{
		msg.set_is_state(0);
		msg.mutable_state()->set_zhen(zhen);
		msg.mutable_state()->set_tid(0);
		msg.mutable_state()->set_init_item(false);
	}
	else if (rsdata == "")
	{
		msg.set_is_state(1);
		msg.mutable_state()->set_zhen(rszhen);
		msg.mutable_state()->set_tid(0);
		msg.mutable_state()->set_init_item(false);
		for (int i = op_start_index; i < op_all.size(); ++i)
		{
			msg.add_ops()->CopyFrom(*op_all[i]);
		}
	}
	else
	{
		msg.set_is_state(2);
		msg.mutable_state()->ParseFromString(rsdata);
		for (int i = op_start_index; i < op_all.size(); ++i)
		{
			msg.add_ops()->CopyFrom(*op_all[i]);
		}
	}
	msg.set_self_camp(player.player.camp());
	msg.set_is_new(player.is_new);
	TPacket *pck = TPacket::New((uint16_t)SMSG_BATTLE_LINK, &msg);
	service::udp_service()->send_msg(hid, pck);
}

void RoomMessage::send_smsg_battle_op(protocol::game::msg_battle_op *op)
{
	protocol::game::msg_battle_op msg;
	msg.CopyFrom(*op);
	std::vector<int> hids;
	sRoomPool->get_hids(hids);
	for (int i = 0; i < hids.size(); ++i)
	{
		int hid = hids[i];
		TPacket *pck = TPacket::New((uint16_t)SMSG_BATTLE_OP, &msg);
		service::udp_service()->send_msg(hid, pck);
	}
}

void RoomMessage::send_smsg_battle_zhen()
{
	std::vector<int> hids;
	sRoomPool->get_hids(hids);
	for (int i = 0; i < hids.size(); ++i)
	{
		int hid = hids[i];
		TPacket *pck = TPacket::New((uint16_t)SMSG_BATTLE_ZHEN, 0);
		service::udp_service()->send_msg(hid, pck);
	}
}

void RoomMessage::send_smsg_battle_finish()
{
	std::vector<int> hids;
	sRoomPool->get_hids(hids);
	for (int i = 0; i < hids.size(); ++i)
	{
		int hid = hids[i];
		TPacket *pck = TPacket::New((uint16_t)SMSG_BATTLE_FINISH, 0);
		service::udp_service()->send_msg(hid, pck);
	}
}

void RoomMessage::send_push_rm_rc_battle_end(uint64_t battle_guid, const std::string &master_name, const std::string &result)
{
	protocol::game::push_rm_rc_battle_end msg;
	msg.set_battle_guid(battle_guid);
	msg.set_result(result);
	Packet *pck = Packet::New((uint16_t)PUSH_RM_RC_BATTLE_END, 0, 0, &msg);
	service::rpc_service()->push(master_name, pck);
}

void RoomMessage::send_push_r_rm_created(uint64_t battle_guid, const std::string &master_name)
{
	protocol::game::push_r_rm_created msg;
	msg.set_battle_guid(battle_guid);
	Packet *pck = Packet::New((uint16_t)PUSH_R_RM_CREATED, 0, 0, &msg);
	service::rpc_service()->push(master_name, pck);
}
