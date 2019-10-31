#ifndef __REWARD_H__
#define __REWARD_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct s_t_reward
{
	int type;
	int value1;
	int value2;
	int value3;

	s_t_reward() {}
	s_t_reward(int type_, int value1_, int value2_, int value3_ = 0) :type(type_), value1(value1_), value2(value2_), value3(value3_) {}

};

struct s_t_rewards
{
	std::vector<s_t_reward> rewards;
	std::vector<dhc::role_t *> roles;

	void add_reward(int type, int value1, int value2, int value3 = 0);

	void add_reward(const s_t_reward &reward);

	void add_reward(const std::vector<s_t_reward> &reward);
};

struct s_t_attr
{
	int type;
	int param1;
	int param2;
	int param3;
	int param4;
};

#endif
