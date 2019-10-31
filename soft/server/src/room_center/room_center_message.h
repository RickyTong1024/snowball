#ifndef __ROOM_CENTER_MESSAGE_H__
#define __ROOM_CENTER_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class RoomCenterMessage
{
public:
	static void send_rep_hall_rc_has_battle(const std::string &name, int id, int error_code, int is_new, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::string &code);

	static void send_rep_hall_rc_single_battle(const std::string &name, int id, int error_code, int is_new, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::string &code);

	static void send_rep_team_rc_multi_battle(const std::string &name, int id, int error_code, const std::vector<uint64_t> &guids, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::vector<std::string> &codes);

	static void send_push_rc_rm_set_player_room(const std::string &name, uint64_t guid, const std::string &code, const protocol::game::msg_battle_player_info *player, uint64_t battle_guid);

	static void send_req_rc_rm_create_room(const std::string &name, uint64_t battle_guid, int battle_type, ResponseFunc func);

	static void send_push_rc_hall_battle_end(const std::string &name, const std::vector<uint64_t> &guids, const std::string &result, int type);
};

#endif
