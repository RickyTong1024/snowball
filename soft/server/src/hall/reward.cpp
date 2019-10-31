#include "reward.h"
#include "item_config.h"

void s_t_rewards::add_reward(int type, int value1, int value2, int value3)
{
	if (type == 1 || type == 2)
	{
		for (int i = 0; i < rewards.size(); ++i)
		{
			if (rewards[i].value1 == value1)
			{
				rewards[i].value2 += value2;
				return;
			}
		}
	}
	else if (type == 1001)
	{
		int id = sItemConfig->get_random_suipian(value1);
		add_reward(2, id, value2);
		return;
	}
	else if (type == 1002)
	{
		int id = sItemConfig->get_random_bitem();
		add_reward(2, id, value1);
		return;
	}
	rewards.push_back(s_t_reward(type, value1, value2, value3));
}

void s_t_rewards::add_reward(const s_t_reward &reward)
{
	add_reward(reward.type, reward.value1, reward.value2, reward.value3);
}

void s_t_rewards::add_reward(const std::vector<s_t_reward> &reward)
{
	for (int i = 0; i < reward.size(); ++i)
	{
		add_reward(reward[i]);
	}
}
