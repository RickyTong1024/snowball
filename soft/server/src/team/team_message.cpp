#include "team_message.h"

void TeamMessage::send_push_team_hall_error(const std::string &name, uint64_t player_guid, int code, const std::string &text)
{
	protocol::game::push_team_hall_error msg;
	msg.set_player_guid(player_guid);
	msg.set_code(code);
	msg.set_text(text);
	Packet *pck = Packet::New((uint16_t)PUSH_TEAM_HALL_ERROR, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void TeamMessage::send_push_team_hall_create(const std::string &name, uint64_t player_guid, const protocol::game::msg_team *team)
{
	protocol::game::push_team_hall_create msg;
	msg.set_player_guid(player_guid);
	msg.mutable_team()->CopyFrom(*team);
	Packet *pck = Packet::New((uint16_t)PUSH_TEAM_HALL_CREATE, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void TeamMessage::send_push_team_hall_join(const std::string &name, uint64_t player_guid, const protocol::game::msg_team *team)
{
	protocol::game::push_team_hall_join msg;
	msg.set_player_guid(player_guid);
	msg.mutable_team()->CopyFrom(*team);
	Packet *pck = Packet::New((uint16_t)PUSH_TEAM_HALL_JOIN, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void TeamMessage::send_push_team_hall_other_join(const std::string &name, const std::vector<uint64_t> &guids, const protocol::game::msg_team_member *team_member)
{
	protocol::game::push_team_hall_other_join msg;
	for (int i = 0; i < guids.size(); ++i)
	{
		msg.add_guids(guids[i]);
	}
	msg.mutable_member()->CopyFrom(*team_member);
	Packet *pck = Packet::New((uint16_t)PUSH_TEAM_HALL_OTHER_JOIN, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void TeamMessage::send_push_team_hall_exit(const std::string &name, const std::vector<uint64_t> &guids, uint64_t leader_guid, uint64_t player_guid)
{
	protocol::game::push_team_hall_exit msg;
	for (int i = 0; i < guids.size(); ++i)
	{
		msg.add_guids(guids[i]);
	}
	msg.set_leader_guid(leader_guid);
	msg.set_player_guid(player_guid);
	Packet *pck = Packet::New((uint16_t)PUSH_TEAM_HALL_EXIT, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void TeamMessage::send_push_team_hall_kick(const std::string &name, const std::vector<uint64_t> &guids, uint64_t player_guid)
{
	protocol::game::push_team_hall_kick msg;
	for (int i = 0; i < guids.size(); ++i)
	{
		msg.add_guids(guids[i]);
	}
	msg.set_player_guid(player_guid);
	Packet *pck = Packet::New((uint16_t)PUSH_TEAM_HALL_KICK, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void TeamMessage::send_push_team_hall_invert(const std::string &name, uint64_t target_guid, const protocol::game::msg_battle_player_info &player)
{
	protocol::game::push_team_hall_invert msg;
	msg.set_target_guid(target_guid);
	msg.mutable_player()->set_guid(player.guid());
	msg.mutable_player()->set_name(player.name());
	msg.mutable_player()->set_sex(player.sex());
	msg.mutable_player()->set_avatar(player.avatar());
	msg.mutable_player()->set_toukuang(player.toukuang());
	msg.mutable_player()->set_cup(player.cup());
	msg.mutable_player()->set_name_color(player.name_color());
	Packet *pck = Packet::New((uint16_t)PUSH_TEAM_HALL_INVERT, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void TeamMessage::send_push_team_hall_chat(const std::string &name, const std::vector<uint64_t> &guids, uint64_t player_guid, const std::string &text)
{
	protocol::game::push_team_hall_chat msg;
	for (int i = 0; i < guids.size(); ++i)
	{
		msg.add_guids(guids[i]);
	}
	msg.set_player_guid(player_guid);
	msg.set_text(text);
	Packet *pck = Packet::New((uint16_t)PUSH_TEAM_HALL_CHAT, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void TeamMessage::send_req_team_rc_multi_battle(const std::string &name, int team_id, const protocol::game::msg_team *team, ResponseFunc func)
{
	protocol::game::req_team_rc_multi_battle msg;
	msg.set_team_id(team_id);
	for (int i = 0; i < team->member_size(); ++i)
	{
		msg.add_player()->CopyFrom(team->member(i).player());
	}
	Packet *pck = Packet::New((uint16_t)REQ_TEAM_RC_MULTI_BATTLE, 0, 0, &msg);
	service::rpc_service()->request(name, pck, func);
}

void TeamMessage::send_push_team_hall_multi_battle(const std::string &name, const std::vector<uint64_t> &guids, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::vector<std::string> &codes, int num)
{
	protocol::game::push_team_hall_multi_battle msg;
	for (int i = 0; i < guids.size(); ++i)
	{
		msg.add_guids(guids[i]);
		msg.add_code(codes[i]);
	}
	msg.set_udp_ip(udp_ip);
	msg.set_udp_port(udp_port);
	msg.set_tcp_ip(tcp_ip);
	msg.set_tcp_port(tcp_port);
	msg.set_num(num);
	Packet *pck = Packet::New((uint16_t)PUSH_TEAM_HALL_MULTI_BATTLE, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}
