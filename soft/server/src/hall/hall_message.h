#ifndef __HALL_MESSAGE_H__
#define __HALL_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "player_operation.h"

class HallMessage
{
public:
	static void send_smsg_error(int hid, const std::string &name, operror_t error);

	static void send_smsg_error(uint64_t guid, operror_t error);

	static void send_smsg_error(uint64_t guid, operror_t error, const std::string & text);

	static void send_smsg_success(dhc::player_t *player, uint16_t opcode);
	
	static void send_smsg_login_player(dhc::player_t *player);

	static void send_req_hall_rc_has_battle(dhc::player_t *player, ResponseFunc func);

	static void send_smsg_has_battle(uint64_t guid);

	static void send_req_hall_rc_single_battle(dhc::player_t *player, ResponseFunc func);

	static void send_smsg_single_battle(uint64_t guid, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::string &code, int is_new);

	static void send_smsg_battle_end(uint64_t guid, int box_id, dhc::battle_his_t *battle_his, int gold, int exps, int cup);

	static void send_smsg_offline_battle_end(uint64_t guid, int box_id, int gold, int exps, int cup);

	static void send_smsg_player_look(uint64_t player_guid, dhc::player_t *target);

	static void send_smsg_player_look(uint64_t player_guid, protocol::game::smsg_player_look *msg);

	static void send_req_hall_center_player_look(uint64_t target_guid, ResponseFunc func);

	static void send_rep_center_hall_player_look(int id, int error_code, dhc::player_t *target);

	static void send_smsg_chat(uint64_t player_guid, protocol::game::smsg_chat *msg);

	static void send_push_hall_center_chat_horn(dhc::player_t *player, const std::string &text);

	static void send_smsg_gonggao(uint64_t player_guid, protocol::game::smsg_gonggao *msg);

	static void send_smsg_sys_info(dhc::player_t *player, const std::string &text);

	static void send_smsg_gm_command(dhc::player_t *player, const s_t_rewards& rds);

	static void send_smsg_end_open_box(dhc::player_t *player, uint16_t op, int id, int jewel, const s_t_rewards& rds);

	static void send_smsg_item_buy(dhc::player_t *player, const s_t_rewards& rds);

	static void send_smsg_chat_laba(dhc::player_t *player);

	static void send_smsg_role_hecheng(dhc::player_t *player, dhc::role_t *role);

	static void send_smsg_post_look(dhc::player_t *player);

	static void send_smsg_post_get(dhc::player_t *player, const s_t_rewards& rds);

	static void send_smsg_post_get_all(dhc::player_t *player, const std::vector<uint64_t> post_guids, const s_t_rewards& rds);

	static void send_smsg_post_delete_all(dhc::player_t *player, const std::vector<uint64_t> post_guids);

	static void send_smsg_fenxiang_num(dhc::player_t *player);

	static void send_req_hall_center_libao(const std::string &code, const std::string &use, ResponseFunc func);

	static void send_smsg_libao(dhc::player_t *player, const s_t_rewards& rds);

	static void send_req_hall_center_reharge(const std::vector<std::string> &code, const std::string &pt, ResponseFunc func);

	static void send_smsg_checkdata(dhc::player_t *player);

	static void send_smsg_item_apply(dhc::player_t *player, const s_t_rewards& rds);

	static void send_smsg_post_num(dhc::player_t *player);

	static void send_push_hall_team_create(dhc::player_t *player);

	static void send_smsg_team_create(dhc::player_t *player, const protocol::game::msg_team &team);

	static void send_smsg_team_tuijian(dhc::player_t *player, std::set<uint64_t> &guids);

	static void send_push_hall_team_join(dhc::player_t *player, uint64_t player_guid);

	static void send_smsg_team_join(dhc::player_t *player, const protocol::game::msg_team &team);

	static void send_smsg_team_other_join(dhc::player_t *player, const protocol::game::msg_team_member &team_member);

	static void send_push_hall_team_exit(dhc::player_t *player);

	static void send_push_hall_team_exit1(dhc::player_t *player);

	static void send_smsg_team_exit(dhc::player_t *player, uint64_t leader_guid, uint64_t player_guid);

	static void send_push_hall_team_kick(dhc::player_t *player, uint64_t target_guid);

	static void send_smsg_team_kick(dhc::player_t *player, uint64_t player_guid);

	static void send_push_hall_team_invert(dhc::player_t *player, uint64_t target_guid);

	static void send_smsg_team_invert(dhc::player_t *player, const protocol::game::msg_team_player &player1);

	static void send_push_hall_team_chat(dhc::player_t *player, const std::string &text);

	static void send_smsg_team_chat(dhc::player_t *player, uint64_t player_guid, const std::string &text);

	static void send_push_hall_team_multi_battle(dhc::player_t *player);

	static void send_smsg_team_multi_battle(dhc::player_t *player, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::string &code, int num);

	static void send_smsg_guide(dhc::player_t *player);

	static void send_smsg_level_reward(dhc::player_t *player, const s_t_rewards& rds);

	static void send_push_hall_name_insert(uint64_t guid, const std::string &name);

	static void send_req_hall_name_search(const std::string &name, ResponseFunc func);

	static void req_social_login(dhc::player_t *player, ResponseFunc func);

	static void	send_push_social_logout(dhc::player_t *player);

	static void send_smsg_social_look(dhc::player_t *player, uint16_t opcode, const protocol::game::smsg_social_look& smsg);

	static void send_req_social_delete(uint64_t player_guid, uint64_t target_guid, ResponseFunc func);

	static void send_smsg_social_delete(dhc::player_t *player, uint64_t target_guid);

	static void send_push_soical_reject(uint64_t player_guid, bool reject, ResponseFunc func);

	static void send_req_social_apply(const dhc::social_t &social, ResponseFunc func);

	static void send_smsg_social_apply(dhc::player_t *player, int num);

	static void send_smsg_social_add(dhc::player_t *player, uint16_t opcode, const dhc::social_t &social);

	static void send_req_social_add(const dhc::social_t& social, ResponseFunc func);

	static void send_req_social_gold(uint64_t player_guid, ResponseFunc func);

	static void send_req_social_black(uint64_t player_guid, uint64_t target_guid, ResponseFunc func);

	static void send_rep_hall_social_black(int id, uint16_t error_code, const dhc::social_t &social);

	static void send_req_social_gift(uint64_t player_guid, uint64_t target_guid, int gold, ResponseFunc func);

	static void send_push_social_chat(uint64_t player_guid, uint64_t target_guid, const std::string &name, const std::string &text);

	static void send_smsg_social_chat(dhc::player_t *player, const protocol::game::smsg_chat &msg);

	static void send_smsg_social_gift(dhc::player_t *player, uint64_t target_guid, int gold);

	static void send_smsg_soical_data(dhc::player_t *player, const protocol::game::smsg_social_data &msg);

	static void send_smsg_social_stat(dhc::player_t *player, uint64_t target_guid, const std::string& pname, int stat);

	static void send_req_social_look(uint64_t player_guid, int type, ResponseFunc func);

	static void send_smsg_social_gold(dhc::player_t *player, int gold);

	static void send_smsg_recharge(dhc::player_t *player, int rid, const s_t_rewards& rds);

	static void send_smsg_vip_reward(dhc::player_t *player, const s_t_rewards& rds);

	static void send_smsg_duobao(dhc::player_t *player, int id);

	static void send_push_social_fight(uint64_t player_guid, int fight);

	static void send_push_hall_rank_update(dhc::player_t *player, int id, int value);

	static void send_smsg_rank(dhc::player_t *player, dhc::rank_t *rank);

	static void send_push_hall_rank_forbidden(uint64_t player_guid);
	
private:
	static void create_battle_player_info(dhc::player_t *player, protocol::game::msg_battle_player_info *player_info);
};

#define ERROR_SYS HallMessage::send_smsg_error(player_guid, ERROR_SYSTEM, make_ertext(__FILE__, __LINE__, __FUNCTION__))

#endif
