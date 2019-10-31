#include "room_center_message.h"

void RoomCenterMessage::send_rep_hall_rc_has_battle(const std::string &name, int id, int error_code, int is_new, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::string &code)
{
	protocol::game::rep_hall_rc_has_battle msg;
	msg.set_is_new(is_new);
	msg.set_udp_ip(udp_ip);
	msg.set_udp_port(udp_port);
	msg.set_tcp_ip(tcp_ip);
	msg.set_tcp_port(tcp_port);
	msg.set_code(code);
	Packet *pck = Packet::New((uint16_t)REQ_HALL_RC_HAS_BATTLE, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, error_code);
}

void RoomCenterMessage::send_rep_hall_rc_single_battle(const std::string &name, int id, int error_code, int is_new, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::string &code)
{
	protocol::game::rep_hall_rc_single_battle msg;
	msg.set_udp_ip(udp_ip);
	msg.set_udp_port(udp_port);
	msg.set_tcp_ip(tcp_ip);
	msg.set_tcp_port(tcp_port);
	msg.set_code(code);
	msg.set_is_new(is_new);
	Packet *pck = Packet::New((uint16_t)REQ_HALL_RC_SINGLE_BATTLE, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, error_code);
}

void RoomCenterMessage::send_rep_team_rc_multi_battle(const std::string &name, int id, int error_code, const std::vector<uint64_t> &guids, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::vector<std::string> &codes)
{
	protocol::game::rep_team_rc_multi_battle msg;
	msg.set_udp_ip(udp_ip);
	msg.set_udp_port(udp_port);
	msg.set_tcp_ip(tcp_ip);
	msg.set_tcp_port(tcp_port);
	for (int i = 0; i < guids.size(); ++i)
	{
		msg.add_guid(guids[i]);
		msg.add_code(codes[i]);
	}
	Packet *pck = Packet::New((uint16_t)REQ_TEAM_RC_MULTI_BATTLE, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, error_code);
}

void RoomCenterMessage::send_push_rc_rm_set_player_room(const std::string &name, uint64_t guid, const std::string &code, const protocol::game::msg_battle_player_info *player, uint64_t battle_guid)
{
	protocol::game::push_rc_rm_set_player_room msg;
	msg.set_guid(guid);
	msg.set_battle_guid(battle_guid);
	msg.set_code(code);
	msg.mutable_player()->CopyFrom(*player);
	Packet *pck = Packet::New((uint16_t)PUSH_RC_RM_SET_PLAYER_ROOM, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void RoomCenterMessage::send_req_rc_rm_create_room(const std::string &name, uint64_t battle_guid, int battle_type, ResponseFunc func)
{
	protocol::game::req_rc_rm_create_room msg;
	msg.set_battle_guid(battle_guid);
	msg.set_battle_type(battle_type);
	Packet *pck = Packet::New((uint16_t)REQ_RC_RM_CREATE_ROOM, 0, 0, &msg);
	service::rpc_service()->request(name, pck, func);
}

void RoomCenterMessage::send_push_rc_hall_battle_end(const std::string &name, const std::vector<uint64_t> &guids, const std::string &result, int type)
{
	protocol::game::push_rc_hall_battle_end msg;
	msg.set_type(type);
	for (int i = 0; i < guids.size(); ++i)
	{
		msg.add_guids(guids[i]);
	}
	msg.set_result(result);
	Packet *pck = Packet::New((uint16_t)PUSH_RC_HALL_BATTLE_END, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}
