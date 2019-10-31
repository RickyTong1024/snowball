#ifndef __HALL_MANAGER_H__
#define __HALL_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "hall_pool.h"
#include "item_manager.h"
#include "role_manager.h"
#include "player_manager.h"
#include "chat_manager.h"
#include "post_manager.h"
#include "team_manager.h"
#include "social_manager.h"
#include "rank_manager.h"
#include "dhc.h"

class HallManager : public mmg::DispathService
{
public:
	HallManager();

	~HallManager();

	BEGIN_PACKET_MAP
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(CMSG_LOGIN_PLAYER, player_mgr_->terminal_login_player)
		PUSH_HANDLER(PUSH_GATE_HALL_LOGOUT, player_mgr_->push_gate_hall_logout)
		PUSH_HANDLER(CMSG_SINGLE_BATTLE, terminal_single_battle)
		PUSH_HANDLER(PUSH_RC_HALL_BATTLE_END, push_rc_hall_battle_end)
		PUSH_HANDLER(CMSG_PLAYER_LOOK, player_mgr_->terminal_player_look)
		PUSH_HANDLER(CMSG_CHAT_CHANNEL, chat_mgr_->terminal_chat_channel)
		PUSH_HANDLER(PUSH_HALL_CENTER_CHAT_HORN, chat_mgr_->push_hall_center_chat_horn)
		PUSH_HANDLER(PUSH_PIPE_CENTER_GONGGAO, chat_mgr_->push_pipe_center_gonggao)
		PUSH_HANDLER(CMSG_GM_COMMAND, player_mgr_->terminal_gm_command)
		PUSH_HANDLER(CMSG_AVATAR_ON, player_mgr_->terminal_avatar_on)
		PUSH_HANDLER(CMSG_TOUKUANG_ON, player_mgr_->terminal_toukuang_on)
		PUSH_HANDLER(CMSG_ROLE_ON, role_mgr_->terminal_role_on)
		PUSH_HANDLER(CMSG_ROLE_HECHENG, role_mgr_->terminal_role_hecheng)
		PUSH_HANDLER(CMSG_ROLE_LEVELUP, role_mgr_->terminal_role_levelup)
		PUSH_HANDLER(CMSG_CHANGE_NAME, player_mgr_->terminal_player_change_name)
		PUSH_HANDLER(CMSG_MODIFY_DATA, player_mgr_->terminal_player_modify_data)
		PUSH_HANDLER(CMSG_START_OPEN_BOX, player_mgr_->terminal_start_open_box)
		PUSH_HANDLER(CMSG_END_OPEN_BOX, player_mgr_->terminal_end_open_box)
		PUSH_HANDLER(CMSG_ITEM_BUY, item_mgr_->terminal_item_buy)
		PUSH_HANDLER(CMSG_ITEM_SELL, item_mgr_->terminal_item_sell)
		PUSH_HANDLER(CMSG_ITEM_DIRECT_BUY, item_mgr_->terminal_item_direct_buy)
		PUSH_HANDLER(CMSG_POST_LOOK, post_mgr_->terminal_post_look)
		PUSH_HANDLER(CMSG_POST_READ, post_mgr_->terminal_post_read)
		PUSH_HANDLER(CMSG_POST_GET, post_mgr_->terminal_post_get)
		PUSH_HANDLER(CMSG_POST_DELETE, post_mgr_->terminal_post_delete)
		PUSH_HANDLER(CMSG_POST_GET_ALL, post_mgr_->terminal_post_get_all)
		PUSH_HANDLER(CMSG_POST_DELETE_ALL, post_mgr_->terminal_post_delete_all)
		PUSH_HANDLER(CMSG_OPEN_BATTLE_BOX, player_mgr_->terminal_open_battle_box)
		PUSH_HANDLER(CMSG_SIGN, player_mgr_->terminal_sign)
		PUSH_HANDLER(CMSG_LIBAO, terminal_libao)
		PUSH_HANDLER(CMSG_RECHARGE, terminal_recharge)
		PUSH_HANDLER(PUSH_PIPE_CENTER_RECHARGE_ALI, push_pipe_center_recharge_ali)
		PUSH_HANDLER(CMSG_CHECKDATA, terminal_checkdata)
		PUSH_HANDLER(CMSG_FENXIANG, player_mgr_->terminal_fengxiang)
		PUSH_HANDLER(CMSG_OPEN_FENXIANG_BOX, player_mgr_->terminal_open_fengxiang_box)
		PUSH_HANDLER(CMSG_INFOMATION, player_mgr_->terminal_infomation)
		PUSH_HANDLER(CMSG_ITEM_APPLY, item_mgr_->terminal_item_apply)
		PUSH_HANDLER(CMSG_BATTLE_ACHIEVE, player_mgr_->terminal_battle_achieve)
		PUSH_HANDLER(CMSG_ACHIEVE, player_mgr_->terminal_achieve)
		PUSH_HANDLER(CMSG_ACHIEVE_REWARD, player_mgr_->terminal_achieve_reward)
		PUSH_HANDLER(CMSG_OFFLINE_BATTLE, terminal_offline_battle)
		PUSH_HANDLER(CMSG_OFFLINE_BATTLE_END, terminal_offline_battle_end)
		PUSH_HANDLER(PUSH_TEAM_HALL_ERROR, team_mgr_->push_team_hall_error)
		PUSH_HANDLER(CMSG_TEAM_TUIJIAN, team_mgr_->terminal_team_tuijian)
		PUSH_HANDLER(CMSG_TEAM_CREATE, team_mgr_->terminal_team_create)
		PUSH_HANDLER(PUSH_TEAM_HALL_CREATE, team_mgr_->push_team_hall_create)
		PUSH_HANDLER(CMSG_TEAM_JOIN, team_mgr_->terminal_team_join)
		PUSH_HANDLER(PUSH_TEAM_HALL_JOIN, team_mgr_->push_team_hall_join)
		PUSH_HANDLER(PUSH_TEAM_HALL_OTHER_JOIN, team_mgr_->push_team_hall_other_join)
		PUSH_HANDLER(CMSG_TEAM_EXIT, team_mgr_->terminal_team_exit)
		PUSH_HANDLER(PUSH_TEAM_HALL_EXIT, team_mgr_->push_team_hall_exit)
		PUSH_HANDLER(CMSG_TEAM_KICK, team_mgr_->terminal_team_kick)
		PUSH_HANDLER(PUSH_TEAM_HALL_KICK, team_mgr_->push_team_hall_kick)
		PUSH_HANDLER(CMSG_TEAM_INVERT, team_mgr_->terminal_team_invert)
		PUSH_HANDLER(PUSH_TEAM_HALL_INVERT, team_mgr_->push_team_hall_invert)
		PUSH_HANDLER(CMSG_TEAM_CHAT, team_mgr_->terminal_team_chat)
		PUSH_HANDLER(PUSH_TEAM_HALL_CHAT, team_mgr_->push_team_hall_chat)
		PUSH_HANDLER(CMSG_MULTI_BATTLE, team_mgr_->terminal_multi_battle)
		PUSH_HANDLER(PUSH_TEAM_HALL_MULTI_BATTLE, team_mgr_->push_team_hall_multi_battle)
		PUSH_HANDLER(CMSG_GUIDE, terminal_guide)
		PUSH_HANDLER(CMSG_BATTLE_TASK, player_mgr_->terminal_battle_task)
		PUSH_HANDLER(CMSG_TASK, player_mgr_->terminal_task)
		PUSH_HANDLER(CMSG_BATTLE_DAILY, player_mgr_->terminal_battle_daily)
		PUSH_HANDLER(CMSG_DAILY, player_mgr_->terminal_daily)
		PUSH_HANDLER(CMSG_DAILY_REWARD, player_mgr_->terminal_daily_reward)
		PUSH_HANDLER(CMSG_FASHION_ON, player_mgr_->terminal_fashion_on)
		PUSH_HANDLER(CMSG_LEVEL_REWARD, player_mgr_->terminal_level_reward)
		PUSH_HANDLER(CMSG_SOCIAL_LOOK, social_mgr_->terminal_look)
		PUSH_HANDLER(CMSG_SOCIAL_DELETE, social_mgr_->terminal_delete)
		PUSH_HANDLER(PUSH_SOCIAL_DELETE, social_mgr_->terminal_social_delete)
		PUSH_HANDLER(CMSG_SOCIAL_APPLY_REJECT, social_mgr_->terminal_apply_reject)
		PUSH_HANDLER(CMSG_SOCIAL_APPLY, social_mgr_->terminal_apply)
		PUSH_HANDLER(PUSH_SOCIAL_APPLY, social_mgr_->terminal_social_apply)
		PUSH_HANDLER(CMSG_SOCIAL_ADD, social_mgr_->terminal_add)
		PUSH_HANDLER(PUSH_SOCIAL_ADD, social_mgr_->terminal_social_add)
		PUSH_HANDLER(CMSG_SOCIAL_BLACK, social_mgr_->terminal_black)
		PUSH_HANDLER(CMSG_SOCIAL_GIFT, social_mgr_->terminal_gift)
		PUSH_HANDLER(PUSH_SOCIAL_GIFT, social_mgr_->terminal_social_gift)
		PUSH_HANDLER(CMSG_SOCIAL_CHAT, social_mgr_->terminal_chat)
		PUSH_HANDLER(PUSH_SOCIAL_CHAT, social_mgr_->terminal_social_chat)
		PUSH_HANDLER(CMSG_SOCIAL_GIFT_GET, social_mgr_->terminal_gift_get)
		PUSH_HANDLER(PUSH_SOCIAL_LOGIN, social_mgr_->terminal_login)
		PUSH_HANDLER(PUSH_SOCIAL_LOGOUT, social_mgr_->terminal_logout)
		PUSH_HANDLER(CMSG_SOCIAL_TUIJIAN, social_mgr_->terminal_tuijian)
		PUSH_HANDLER(CMSG_SOCIAL_SEARCH, social_mgr_->terminal_search)
		PUSH_HANDLER(CMSG_VIP_REWARD, player_mgr_->terminal_vip_reward)
		PUSH_HANDLER(CMSG_DUOBAO, item_mgr_->terminal_duobao)
		PUSH_HANDLER(PUSH_PIPE_CENTER_RECHARGE_SIMULATION, push_pipe_center_recharge_simulation)
		PUSH_HANDLER(CMSG_RANK, rank_mgr_->terminal_rank)
		PUSH_HANDLER(PUSH_RANK_HALL_CACHE, rank_mgr_->push_rank_hall_cache)
		PUSH_HANDLER(PUSH_PIPE_CENTER_RANK_FORBIDDEN, rank_mgr_->push_pipe_rank_forbidden)
		PUSH_HANDLER(CMSG_ADVERTISEMENT, player_mgr_->terminal_advertisement)
	END_PUSH_MAP

	BEGIN_REQ_MAP
		REQ_HANDLER(REQ_CENTER_HALL_PLAYER_LOOK, player_mgr_->req_center_hall_player_look)
		REQ_HANDLER(REQ_SOCIAL_BLACK, social_mgr_->terminal_social_black)
	END_REQ_MAP

	int init();

	int fini();

private:
	int terminal_single_battle(Packet *pck, const std::string &name);

	void single_battle_callback(Packet *pck, int error_code, uint64_t player_guid);

	int push_rc_hall_battle_end(Packet *pck, const std::string &name);

	int terminal_offline_battle(Packet *pck, const std::string &name);

	int terminal_offline_battle_end(Packet *pck, const std::string &name);

	int terminal_libao(Packet *pck, const std::string &name);

	void libao(Packet *pck, int error_code, uint64_t player_guid, const std::string &code);

	void libao1(Packet *pck, int error_code, uint64_t player_guid);

	int terminal_recharge(Packet *pck, const std::string &name);

	void recharge(uint64_t player_guid, int rid, int count);

	void recharge_apple(Packet *pck, int error_code, uint64_t player_guid, int rid);

	void recharge_google(Packet *pck, int error_code, uint64_t player_guid, int rid);

	void recharge_dhc_callback(Request *req, uint64_t player_guid, int rid, int count);

	int push_pipe_center_recharge_ali(Packet *pck, const std::string &name);

	int terminal_checkdata(Packet *pck, const std::string &name);

	int terminal_guide(Packet *pck, const std::string &name);

	int update(ACE_Time_Value tv);

	int push_pipe_center_recharge_simulation(Packet *pck, const std::string &name);

private:
	int timer_id_;
	PlayerManager *player_mgr_;
	ItemManager *item_mgr_;
	RoleManager *role_mgr_;
	ChatManager *chat_mgr_;
	PostManager *post_mgr_;
	TeamManager *team_mgr_;
	SocialManager *social_mgr_;
	RankManager *rank_mgr_;
};

#endif
