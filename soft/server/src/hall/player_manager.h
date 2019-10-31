#ifndef __PLAYER_MANAGER_H__
#define __PLAYER_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

class PlayerManager
{
public:
	PlayerManager();
	~PlayerManager();

	int init();

	int fini();

	int update(ACE_Time_Value tv);

	int terminal_login_player(Packet *pck, const std::string &name);

	int push_gate_hall_logout(Packet *pck, const std::string &name);

	int terminal_player_look(Packet *pck, const std::string &name);

	void terminal_player_look_callback(uint64_t player_guid, uint64_t target_guid);

	int terminal_avatar_on(Packet *pck, const std::string &name);

	int terminal_toukuang_on(Packet *pck, const std::string &name);

	void req_hall_center_player_look_callback(Packet *pck, int error_code, uint64_t player_guid);

	int req_center_hall_player_look(Packet *pck, const std::string &name, int id);

	void req_center_hall_player_look_callback(int id, uint64_t target_guid);

	int terminal_gm_command(Packet *pck, const std::string &name);

	int terminal_player_change_name(Packet *pck, const std::string &name);

	int terminal_player_modify_data(Packet *pck, const std::string &name);

	int terminal_start_open_box(Packet *pck, const std::string &name);

	int terminal_end_open_box(Packet *pck, const std::string &name);

	int terminal_open_battle_box(Packet *pck, const std::string &name);

	int terminal_sign(Packet *pck, const std::string &name);

	int terminal_fengxiang(Packet *pck, const std::string &name);

	int terminal_open_fengxiang_box(Packet *pck, const std::string &name);

	int terminal_infomation(Packet *pck, const std::string &name);

	int terminal_battle_achieve(Packet *pck, const std::string &name);

	int terminal_achieve(Packet *pck, const std::string &name);

	int terminal_achieve_reward(Packet *pck, const std::string &name);

	int terminal_battle_task(Packet *pck, const std::string &name);

	int terminal_task(Packet *pck, const std::string &name);

	int terminal_fashion_on(Packet *pck, const std::string &name);

	int terminal_battle_daily(Packet *pck, const std::string &name);

	int terminal_daily(Packet *pck, const std::string &name);

	int terminal_daily_reward(Packet *pck, const std::string &name);

	int terminal_level_reward(Packet *pck, const std::string &name);

	int terminal_vip_reward(Packet *pck, const std::string &name);

	int terminal_advertisement(Packet *pck, const std::string &name);
};

#endif
