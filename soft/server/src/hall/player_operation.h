#ifndef __PLAYER_OPERATION_H__
#define __PLAYER_OPERATION_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "reward.h"

struct resource
{
	enum resource_t
	{
		GOLD = 1,
		JEWEL = 2,
		EXP = 3,
		SNOW = 4,
		CUP = 6,
		APOINT = 7,
		DPOINT = 8,
	};
};

class PlayerOperation
{
public:
	static void player_login(dhc::player_t *player);

	static void client_login(dhc::player_t *player);

	static void client_logout(dhc::player_t *player);

	static void req_hall_rc_has_battle(Packet *pck, int error_code, uint64_t player_guid);

	static void player_logout(dhc::player_t *player);

	static void player_refresh_check(dhc::player_t *player);

	static void player_refresh(dhc::player_t *player, int day);

	static void player_week_refresh(dhc::player_t *player);

	static void player_month_refresh(dhc::player_t *player);

	static void player_add_reward(dhc::player_t *player, s_t_rewards &strewards);

	static void player_add_resource(dhc::player_t *player, resource::resource_t type, int value);

	static int player_get_resource(dhc::player_t *player, resource::resource_t type);

	static void player_dec_resource(dhc::player_t *player, resource::resource_t type, int value);

	static void player_add_avatar(dhc::player_t *player, int avatar);

	static void player_add_toukuang(dhc::player_t *player, int id);

	static void player_check_toukuang(dhc::player_t *player);

	static int player_add_fashion(dhc::player_t *player, int id);

	static void player_mod_exp(dhc::player_t * player, int exp);

	static void player_set_cup(dhc::player_t *player, int cup);

	static void player_add_box(dhc::player_t *player, int id);

	static int player_add_random_box(dhc::player_t *player, int rank);

	static int player_recharge(dhc::player_t *player, int rid, int count, s_t_rewards &rewards);

	static void calc_out_attr(dhc::player_t *player);

	static int get_out_attr(dhc::player_t *player, int id);

	static void add_all_type_num(dhc::player_t *player, int type, int num);

	static void add_achieve_type_num(dhc::player_t *player, int type, int num);

	static void add_achieve_num(dhc::player_t *player, int id, int num, bool check);

	static int get_achieve_num(dhc::player_t *player, int id);

	static void del_achieve_num(dhc::player_t *player, int id);

	static void add_task_type_num(dhc::player_t *player, int type, int num);

	static void add_task_num(dhc::player_t *player, int id, int num, bool check);

	static int get_task_num(dhc::player_t *player, int id);

	static void del_task_num(dhc::player_t *player, int id);

	static void add_daily_type_num(dhc::player_t *player, int type, int num);

	static void add_daily_num(dhc::player_t *player, int id, int num, bool check);

	static int get_daily_num(dhc::player_t *player, int id);

	static void del_daily_num(dhc::player_t *player, int id);

	static void refresh_duobao(dhc::player_t *player);

	static int player_get_name_color(dhc::player_t *player);

	static std::string get_color_string(int color);

	static std::string get_color_name(int color, const std::string &text);
};

#endif
