#ifndef __TEAM_MESSAGE_H__
#define __TEAM_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class TeamMessage
{
public:
	static void send_push_team_hall_error(const std::string &name, uint64_t player_guid, int code, const std::string &text);

	static void send_push_team_hall_create(const std::string &name, uint64_t player_guid, const protocol::game::msg_team *team);

	static void send_push_team_hall_join(const std::string &name, uint64_t player_guid, const protocol::game::msg_team *team);

	static void send_push_team_hall_other_join(const std::string &name, const std::vector<uint64_t> &guids, const protocol::game::msg_team_member *team_member);

	static void send_push_team_hall_exit(const std::string &name, const std::vector<uint64_t> &guids, uint64_t leader_guid, uint64_t player_guid);

	static void send_push_team_hall_kick(const std::string &name, const std::vector<uint64_t> &guids, uint64_t player_guid);

	static void send_push_team_hall_invert(const std::string &name, uint64_t target_guid, const protocol::game::msg_battle_player_info &player);

	static void send_push_team_hall_chat(const std::string &name, const std::vector<uint64_t> &guids, uint64_t player_guid, const std::string &text);

	static void send_req_team_rc_multi_battle(const std::string &name, int team_id, const protocol::game::msg_team *team, ResponseFunc func);

	static void send_push_team_hall_multi_battle(const std::string &name, const std::vector<uint64_t> &guids, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::vector<std::string> &codes, int num);
};

#define ERROR_SYS TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_SYSTEM, make_ertext(__FILE__, __LINE__, __FUNCTION__))

#endif
