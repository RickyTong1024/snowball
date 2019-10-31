#ifndef __PLAYER_CONFIG_H__
#define __PLAYER_CONFIG_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "reward.h"

struct s_t_avatar
{
	int id;
	int role_id;
};

struct s_t_chest_reward
{
	s_t_reward reward;
	int rate;
};

struct s_t_chest
{
	int id;
	std::string name;
	int font_color;
	int time;
	int gold_min;
	int gold_max;
	int reward_num;
	int reward_xy_num;
	std::vector<s_t_chest_reward> normal;
	std::vector<s_t_chest_reward> xiyou;
};

struct s_t_chest_rate
{
	int id;
	int rate;
	int min;
	int max;
};

struct s_t_sign
{
	int id;
	s_t_reward reward;
};

struct s_t_recharge
{
	int id;
	int type;
	int rmb;
	int jewel;
	std::string ios_id;
	std::string google_id;
};

struct s_t_achievement
{
	int id;
	int atype;
	int pre;
	int count;
	int type;
	int param1;
	int param2;
	int param3;
	int param4;
	int point;
};

struct s_t_achievement_reward
{
	int id;
	int point;
	std::vector<s_t_reward> rewards;
};

struct s_t_toukuang
{
	int id;
	int time;
	std::vector<s_t_attr> attrs;
};

struct s_t_exp
{
	int level;
	int exp;
	std::vector<s_t_reward> rewards;
	std::vector<s_t_attr> attrs;
};

struct s_t_cup
{
	int id;
	std::string name;
	int cup;
	int down;
	int sb;
	int jb;
	int tsb;
	int tjb;
	int tsbnum;
	std::vector<s_t_reward> rewards;
};

struct s_t_task
{
	int id;
	int level;
	int atype;
	int pre;
	int count;
	int type;
	int param1;
	int param2;
	int param3;
	int param4;
	std::vector<s_t_reward> rewards;
};

struct s_t_fashion
{
	int id;
	std::string name;
	int type;
	int font_color;
	int sell;
	s_t_attr attr;
};

struct s_t_daily
{
	int id;
	int atype;
	int count;
	int type;
	int param1;
	int param2;
	int param3;
	int param4;
	int point;
	std::vector<s_t_reward> rewards;
};

struct s_t_daily_reward
{
	int id;
	int point;
	s_t_reward reward;
};

struct s_t_vip_attr
{
	int id;
	int first_id;
	int day_id;
	std::vector<s_t_attr> attrs;
};

class PlayerConfig
{
public:
	int parse();

	s_t_avatar * get_avatar(int id);

	int get_role_avatar(int id);

	s_t_chest * get_chest(int id);

	int get_random_chest(dhc::player_t *player, int rank);

	void open_chest(dhc::player_t *player, int id, s_t_rewards &rewards);

	s_t_sign * get_sign(int id);

	s_t_recharge * get_recharge(int id);

	void get_achievement_by_type(int type, std::vector<int> &ids);

	s_t_achievement * get_achievement(int id);

	s_t_achievement_reward * get_achievement_reward(int id);

	s_t_toukuang * get_toukuang(int id);

	s_t_exp * get_exp(int level);

	s_t_cup * get_cup(int cup);

	int get_sj_cup(int cup, int rank);

	std::string get_random_name();

	std::string get_random_en_name();

	void get_task_by_type(int type, std::vector<int> &ids);

	s_t_task * get_task(int id);

	s_t_fashion * get_fashion(int id);

	void get_daily_by_type(int type, std::vector<int> &ids);

	s_t_daily * get_daily(int id);

	s_t_daily_reward * get_daily_reward(int id);

	s_t_vip_attr * get_vip_attr(int id);

	int get_random_duobao();

private:
	std::map<int, int> t_role_avatars_;
	std::map<int, s_t_avatar> t_avatars_;
	std::map<int, s_t_chest> t_chests_;
	std::vector<s_t_chest_rate> t_chest_rates_;
	std::vector<s_t_sign> t_signs_;
	std::map<int, s_t_recharge> t_recharges_;
	std::map<int, s_t_achievement> t_achievement_;
	std::map<int, std::vector<int> > t_achievement_type_;
	std::vector<s_t_achievement_reward> t_achievement_reward_;
	std::map<int, s_t_toukuang> t_toukuang_;
	std::vector<s_t_exp> t_exp_;
	std::vector<s_t_cup> t_cup_;
	std::vector<std::string> t_first_names_;
	std::vector<std::string> t_last_names_;
	std::vector<std::string> t_3d_names_;
	std::map<int, s_t_task> t_task_;
	std::map<int, std::vector<int> > t_task_type_;
	std::map<int, s_t_fashion> t_fashions_;
	std::map<int, s_t_daily> t_daily_;
	std::map<int, std::vector<int> > t_daily_type_;
	std::vector<s_t_daily_reward> t_daily_reward_;
	std::map<int, s_t_vip_attr> t_vip_attr_;
	std::vector<int> t_duobao_rate_;
};

#define sPlayerConfig (Singleton<PlayerConfig>::instance ())

#endif
