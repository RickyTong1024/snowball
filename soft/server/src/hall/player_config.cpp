#include "player_config.h"
#include "dbc.h"
#include "utils.h"
#include "player_operation.h"

int PlayerConfig::parse()
{
	DBCFile *dbfile = service::scheme()->get_dbc("t_avatar.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_avatar t_avatar;
		t_avatar.id = dbfile->Get(i, 0)->iValue;
		t_avatar.role_id = dbfile->Get(i, 3)->iValue;
		t_avatars_[t_avatar.id] = t_avatar;
		t_role_avatars_[t_avatar.role_id] = t_avatar.id;
	}

	dbfile = service::scheme()->get_dbc("t_chest.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_chest t_chest;
		t_chest.id = dbfile->Get(i, 0)->iValue;
		t_chest.name = dbfile->Get(i, 2)->pString;
		t_chest.name = service::scheme()->get_lang_str(t_chest.name);
		t_chest.font_color = dbfile->Get(i, 3)->iValue;
		t_chest.time = dbfile->Get(i, 6)->iValue;
		t_chest.gold_min = dbfile->Get(i, 7)->iValue;
		t_chest.gold_max = dbfile->Get(i, 8)->iValue;
		t_chest.reward_num = dbfile->Get(i, 9)->iValue;
		t_chest.reward_xy_num = dbfile->Get(i, 10)->iValue;
		for (int j = 0; j < 100; ++j)
		{
			s_t_chest_reward t_chest_reward;
			t_chest_reward.reward.type = dbfile->Get(i, 11 + j * 5)->iValue;
			t_chest_reward.reward.value1 = dbfile->Get(i, 12 + j * 5)->iValue;
			t_chest_reward.reward.value2 = dbfile->Get(i, 13 + j * 5)->iValue;
			t_chest_reward.reward.value3 = dbfile->Get(i, 14 + j * 5)->iValue;
			t_chest_reward.rate = dbfile->Get(i, 15 + j * 5)->iValue;
			if (t_chest_reward.reward.type > 0)
			{
				t_chest.normal.push_back(t_chest_reward);
			}
		}
		for (int j = 0; j < 100; ++j)
		{
			s_t_chest_reward t_chest_reward;
			t_chest_reward.reward.type = dbfile->Get(i, 511 + j * 5)->iValue;
			t_chest_reward.reward.value1 = dbfile->Get(i, 512 + j * 5)->iValue;
			t_chest_reward.reward.value2 = dbfile->Get(i, 513 + j * 5)->iValue;
			t_chest_reward.reward.value3 = dbfile->Get(i, 514 + j * 5)->iValue;
			t_chest_reward.rate = dbfile->Get(i, 515 + j * 5)->iValue;
			if (t_chest_reward.reward.type > 0)
			{
				t_chest.xiyou.push_back(t_chest_reward);
			}
		}

		t_chests_[t_chest.id] = t_chest;
	}

	dbfile = service::scheme()->get_dbc("t_chest_rate.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_chest_rate t_chest_rate;
		t_chest_rate.id = dbfile->Get(i, 0)->iValue;
		t_chest_rate.rate = dbfile->Get(i, 2)->iValue;
		t_chest_rate.min = dbfile->Get(i, 3)->iValue;
		t_chest_rate.max = dbfile->Get(i, 4)->iValue;

		t_chest_rates_.push_back(t_chest_rate);
	}

	dbfile = service::scheme()->get_dbc("t_sign.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_sign t_sign;
		t_sign.id = dbfile->Get(i, 0)->iValue;
		t_sign.reward.type = dbfile->Get(i, 3)->iValue;
		t_sign.reward.value1 = dbfile->Get(i, 4)->iValue;
		t_sign.reward.value2 = dbfile->Get(i, 5)->iValue;
		t_sign.reward.value3 = dbfile->Get(i, 6)->iValue;

		t_signs_.push_back(t_sign);
	}

	dbfile = service::scheme()->get_dbc("t_recharge.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_recharge t_recharge;
		t_recharge.id = dbfile->Get(i, 0)->iValue;
		t_recharge.type = dbfile->Get(i, 3)->iValue;
		t_recharge.rmb = dbfile->Get(i, 5)->iValue;
		t_recharge.jewel = dbfile->Get(i, 7)->iValue;
		t_recharge.ios_id = dbfile->Get(i, 8)->pString;
		t_recharge.google_id = dbfile->Get(i, 9)->pString;

		t_recharges_[t_recharge.id] = t_recharge;
	}

	dbfile = service::scheme()->get_dbc("t_achievement.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_achievement t_achievement;
		t_achievement.id = dbfile->Get(i, 0)->iValue;
		t_achievement.atype = dbfile->Get(i, 5)->iValue;
		t_achievement.pre = dbfile->Get(i, 6)->iValue;
		t_achievement.count = dbfile->Get(i, 7)->iValue;
		t_achievement.type = dbfile->Get(i, 8)->iValue;
		t_achievement.param1 = dbfile->Get(i, 9)->iValue;
		t_achievement.param2 = dbfile->Get(i, 10)->iValue;
		t_achievement.param3 = dbfile->Get(i, 11)->iValue;
		t_achievement.param4 = dbfile->Get(i, 12)->iValue;
		t_achievement.point = dbfile->Get(i, 13)->iValue;

		t_achievement_[t_achievement.id] = t_achievement;
		t_achievement_type_[t_achievement.type].push_back(t_achievement.id);
	}

	dbfile = service::scheme()->get_dbc("t_achievement_reward.txt");
	if (!dbfile)
	{
		return -1;
	}
	int tnum = 0;
	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_achievement_reward t_achievement_reward;
		t_achievement_reward.id = dbfile->Get(i, 0)->iValue;
		t_achievement_reward.point = dbfile->Get(i, 1)->iValue + tnum;
		tnum = t_achievement_reward.point;
		for (int j = 0; j < 3; ++j)
		{
			s_t_reward t_reward;
			t_reward.type = dbfile->Get(i, 2 + 4 * j)->iValue;
			t_reward.value1 = dbfile->Get(i, 3 + 4 * j)->iValue;
			t_reward.value2 = dbfile->Get(i, 4 + 4 * j)->iValue;
			t_reward.value3 = dbfile->Get(i, 5 + 4 * j)->iValue;
			if (t_reward.type > 0)
			{
				t_achievement_reward.rewards.push_back(t_reward);
			}
		}

		t_achievement_reward_.push_back(t_achievement_reward);
	}

	dbfile = service::scheme()->get_dbc("t_toukuang.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_toukuang t_toukuang;
		t_toukuang.id = dbfile->Get(i, 0)->iValue;
		t_toukuang.time = dbfile->Get(i, 7)->iValue;
		for (int j = 0; j < 3; ++j)
		{
			s_t_attr t_attr;
			t_attr.type = dbfile->Get(i, 9 + j * 4)->iValue;
			t_attr.param1 = dbfile->Get(i, 10 + j * 4)->iValue;
			t_attr.param2 = dbfile->Get(i, 11 + j * 4)->iValue;
			t_attr.param3 = dbfile->Get(i, 12 + j * 4)->iValue;
			if (t_attr.type > 0)
			{
				t_toukuang.attrs.push_back(t_attr);
			}
		}

		t_toukuang_[t_toukuang.id] = t_toukuang;
	}

	dbfile = service::scheme()->get_dbc("t_exp.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_exp t_exp;
		t_exp.level = dbfile->Get(i, 0)->iValue;
		t_exp.exp = dbfile->Get(i, 1)->iValue;
		for (int j = 0; j < 3; ++j)
		{
			s_t_reward t_reward;
			t_reward.type = dbfile->Get(i, 2 + j * 4)->iValue;
			t_reward.value1 = dbfile->Get(i, 3 + j * 4)->iValue;
			t_reward.value2 = dbfile->Get(i, 4 + j * 4)->iValue;
			t_reward.value3 = dbfile->Get(i, 5 + j * 4)->iValue;
			if (t_reward.type > 0)
			{
				t_exp.rewards.push_back(t_reward);
			}
		}
		for (int j = 0; j < 7; ++j)
		{
			s_t_attr t_attr;
			t_attr.type = dbfile->Get(i, 17 + j * 4)->iValue;
			t_attr.param1 = dbfile->Get(i, 18 + j * 4)->iValue;
			t_attr.param2 = dbfile->Get(i, 19 + j * 4)->iValue;
			t_attr.param3 = dbfile->Get(i, 20 + j * 4)->iValue;
			if (t_attr.type > 0)
			{
				t_exp.attrs.push_back(t_attr);
			}
		}

		t_exp_.push_back(t_exp);
	}

	dbfile = service::scheme()->get_dbc("t_cup.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_cup t_cup;
		t_cup.id = dbfile->Get(i, 0)->iValue;
		t_cup.name = dbfile->Get(i, 2)->pString;
		t_cup.name = service::scheme()->get_lang_str(t_cup.name);
		t_cup.cup = dbfile->Get(i, 3)->iValue;
		t_cup.down = dbfile->Get(i, 9)->iValue;
		t_cup.sb = dbfile->Get(i, 10)->iValue;
		t_cup.jb = dbfile->Get(i, 11)->iValue;
		t_cup.tsb = dbfile->Get(i, 12)->iValue;
		t_cup.tjb = dbfile->Get(i, 13)->iValue;
		t_cup.tsbnum = dbfile->Get(i, 14)->iValue;
		for (int j = 0; j < 3; ++j)
		{
			s_t_reward t_reward;
			t_reward.type = dbfile->Get(i, 15 + j * 4)->iValue;
			t_reward.value1 = dbfile->Get(i, 16 + j * 4)->iValue;
			t_reward.value2 = dbfile->Get(i, 17 + j * 4)->iValue;
			t_reward.value3 = dbfile->Get(i, 18 + j * 4)->iValue;
			if (t_reward.type > 0)
			{
				t_cup.rewards.push_back(t_reward);
			}
		}

		t_cup_.push_back(t_cup);
	}

	dbfile = service::scheme()->get_dbc("t_name.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		std::string s;
		s = dbfile->Get(i, 0)->pString;
		if (s != "")
		{
			t_first_names_.push_back(s);
		}
		s = dbfile->Get(i, 1)->pString;
		if (s != "")
		{
			t_last_names_.push_back(s);
		}
		s = dbfile->Get(i, 2)->pString;
		if (s != "")
		{
			t_3d_names_.push_back(s);
		}
	}

	dbfile = service::scheme()->get_dbc("t_fashion.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_fashion t_fashion;
		t_fashion.id = dbfile->Get(i, 0)->iValue;
		t_fashion.name = dbfile->Get(i, 2)->pString;
		t_fashion.name = service::scheme()->get_lang_str(t_fashion.name);
		t_fashion.type = dbfile->Get(i, 3)->iValue;
		t_fashion.font_color = dbfile->Get(i, 5)->iValue;
		t_fashion.sell = dbfile->Get(i, 14)->iValue;
		t_fashion.attr.type = dbfile->Get(i, 15)->iValue;
		t_fashion.attr.param1 = dbfile->Get(i, 16)->iValue;
		t_fashion.attr.param2 = dbfile->Get(i, 17)->iValue;
		t_fashion.attr.param3 = dbfile->Get(i, 18)->iValue;

		t_fashions_[t_fashion.id] = t_fashion;
	}

	dbfile = service::scheme()->get_dbc("t_task.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_task t_task;
		t_task.id = dbfile->Get(i, 0)->iValue;
		t_task.level = dbfile->Get(i, 3)->iValue;
		t_task.atype = dbfile->Get(i, 4)->iValue;
		t_task.count = dbfile->Get(i, 5)->iValue;
		t_task.type = dbfile->Get(i, 6)->iValue;
		t_task.param1 = dbfile->Get(i, 7)->iValue;
		t_task.param2 = dbfile->Get(i, 8)->iValue;
		t_task.param3 = dbfile->Get(i, 9)->iValue;
		t_task.param4 = dbfile->Get(i, 10)->iValue;
		for (int j = 0; j < 2; ++j)
		{
			s_t_reward t_reward;
			t_reward.type = dbfile->Get(i, 11 + j * 4)->iValue;
			t_reward.value1 = dbfile->Get(i, 12 + j * 4)->iValue;
			t_reward.value2 = dbfile->Get(i, 13 + j * 4)->iValue;
			t_reward.value3 = dbfile->Get(i, 14 + j * 4)->iValue;
			if (t_reward.type > 0)
			{
				t_task.rewards.push_back(t_reward);
			}
		}

		t_task_[t_task.id] = t_task;
		t_task_type_[t_task.type].push_back(t_task.id);
	}

	dbfile = service::scheme()->get_dbc("t_daily.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_daily t_daily;
		t_daily.id = dbfile->Get(i, 0)->iValue;
		t_daily.atype = dbfile->Get(i, 5)->iValue;
		t_daily.count = dbfile->Get(i, 6)->iValue;
		t_daily.type = dbfile->Get(i, 7)->iValue;
		t_daily.param1 = dbfile->Get(i, 8)->iValue;
		t_daily.param2 = dbfile->Get(i, 9)->iValue;
		t_daily.param3 = dbfile->Get(i, 10)->iValue;
		t_daily.param4 = dbfile->Get(i, 11)->iValue;
		for (int j = 0; j < 3; ++j)
		{
			s_t_reward t_reward;
			t_reward.type = dbfile->Get(i, 12 + j * 4)->iValue;
			t_reward.value1 = dbfile->Get(i, 13 + j * 4)->iValue;
			t_reward.value2 = dbfile->Get(i, 14 + j * 4)->iValue;
			t_reward.value3 = dbfile->Get(i, 15 + j * 4)->iValue;
			if (t_reward.type > 0)
			{
				t_daily.rewards.push_back(t_reward);
			}
		}

		t_daily_[t_daily.id] = t_daily;
		t_daily_type_[t_daily.type].push_back(t_daily.id);
	}

	dbfile = service::scheme()->get_dbc("t_daily_reward.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_daily_reward t_daily_reward;
		t_daily_reward.id = dbfile->Get(i, 0)->iValue;
		t_daily_reward.point = dbfile->Get(i, 1)->iValue;
		t_daily_reward.reward.type = dbfile->Get(i, 2)->iValue;
		t_daily_reward.reward.value1 = dbfile->Get(i, 3)->iValue;
		t_daily_reward.reward.value2 = dbfile->Get(i, 4)->iValue;
		t_daily_reward.reward.value3 = dbfile->Get(i, 5)->iValue;

		t_daily_reward_.push_back(t_daily_reward);
	}

	dbfile = service::scheme()->get_dbc("t_vip_attr.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_vip_attr t_vip_attr;
		t_vip_attr.id = dbfile->Get(i, 0)->iValue;
		t_vip_attr.first_id = dbfile->Get(i, 5)->iValue;
		t_vip_attr.day_id = dbfile->Get(i, 6)->iValue;
		for (int j = 0; j < 5; ++j)
		{
			s_t_attr t_attr;
			t_attr.type = dbfile->Get(i, 7 + j * 4)->iValue;
			t_attr.param1 = dbfile->Get(i, 8 + j * 4)->iValue;
			t_attr.param2 = dbfile->Get(i, 9 + j * 4)->iValue;
			t_attr.param3 = dbfile->Get(i, 10 + j * 4)->iValue;
			if (t_attr.type > 0)
			{
				t_vip_attr.attrs.push_back(t_attr);
			}
		}

		t_vip_attr_[t_vip_attr.id] = t_vip_attr;
	}

	dbfile = service::scheme()->get_dbc("t_duobao_rate.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		int rate = dbfile->Get(i, 1)->iValue;
		t_duobao_rate_.push_back(rate);
	}

	return 0;
}

s_t_avatar * PlayerConfig::get_avatar(int id)
{
	std::map<int, s_t_avatar>::iterator it = t_avatars_.find(id);
	if (it == t_avatars_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

int PlayerConfig::get_role_avatar(int id)
{
	std::map<int, int>::iterator it = t_role_avatars_.find(id);
	if (it == t_role_avatars_.end())
	{
		return 0;
	}
	else
	{
		return (*it).second;
	}
}

s_t_chest * PlayerConfig::get_chest(int id)
{
	std::map<int, s_t_chest>::iterator it = t_chests_.find(id);
	if (it == t_chests_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

int PlayerConfig::get_random_chest(dhc::player_t *player, int rank)
{
	int sum = 0;
	for (int i = 0; i < t_chest_rates_.size(); ++i)
	{
		if (rank == 0 || (rank <= t_chest_rates_[i].max && rank >= t_chest_rates_[i].min))
		{
			int rate = t_chest_rates_[i].rate;
			if (t_chest_rates_[i].id == 3)
			{
				rate = rate * (100 + PlayerOperation::get_out_attr(player, 1)) / 100;
			}
			else if (t_chest_rates_[i].id == 4)
			{
				rate = rate * (100 + PlayerOperation::get_out_attr(player, 2)) / 100;
			}
			sum += rate;
			
		}
	}
	if (sum == 0)
	{
		return 0;
	}
	int r = Utils::get_int32(0, sum - 1);
	int gl = 0;
	for (int i = 0; i < t_chest_rates_.size(); ++i)
	{
		if (rank == 0 || (rank <= t_chest_rates_[i].max && rank >= t_chest_rates_[i].min))
		{
			int rate = t_chest_rates_[i].rate;
			if (t_chest_rates_[i].id == 3)
			{
				rate = rate * (100 + PlayerOperation::get_out_attr(player, 1)) / 100;
			}
			else if (t_chest_rates_[i].id == 4)
			{
				rate = rate * (100 + PlayerOperation::get_out_attr(player, 2)) / 100;
			}
			gl += rate;
			if (gl > r)
			{
				return t_chest_rates_[i].id;
			}
		}
	}
	return 0;
}

void PlayerConfig::open_chest(dhc::player_t *player, int id, s_t_rewards &rewards)
{
	s_t_chest *t_chest = get_chest(id);
	if (!t_chest)
	{
		return;
	}
	s_t_reward reward;
	int gold = PlayerOperation::get_out_attr(player, 5);
	reward.type = 1;
	reward.value1 = 1;
	reward.value2 = Utils::get_int32(t_chest->gold_min + gold, t_chest->gold_max + gold + 1);
	reward.value3 = 0;
	rewards.add_reward(reward);
	for (int i = 0; i < t_chest->reward_xy_num; ++i)
	{
		int sum = 0;
		for (int j = 0; j < t_chest->xiyou.size(); ++j)
		{
			sum += t_chest->xiyou[j].rate;
		}
		if (sum == 0)
		{
			continue;
		}
		int r = Utils::get_int32(0, sum - 1);
		int gl = 0;
		for (int j = 0; j < t_chest->xiyou.size(); ++j)
		{
			gl += t_chest->xiyou[j].rate;
			if (gl > r)
			{
				rewards.add_reward(t_chest->xiyou[j].reward);
				break;
			}
		}
	}
	for (int i = 0; i < t_chest->reward_num - t_chest->reward_xy_num; ++i)
	{
		int sum = 0;
		for (int j = 0; j < t_chest->normal.size(); ++j)
		{
			sum += t_chest->normal[j].rate;
		}
		if (sum == 0)
		{
			continue;
		}
		int r = Utils::get_int32(0, sum - 1);
		int gl = 0;
		for (int j = 0; j < t_chest->normal.size(); ++j)
		{
			gl += t_chest->normal[j].rate;
			if (gl > r)
			{
				rewards.add_reward(t_chest->normal[j].reward);
				break;
			}
		}
	}
}

s_t_sign * PlayerConfig::get_sign(int id)
{
	if (id < 0 || id >= t_signs_.size())
	{
		return 0;
	}
	return &t_signs_[id];
}

s_t_recharge * PlayerConfig::get_recharge(int id)
{
	std::map<int, s_t_recharge>::iterator it = t_recharges_.find(id);
	if (it == t_recharges_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

void PlayerConfig::get_achievement_by_type(int type, std::vector<int> &ids)
{
	std::map<int, std::vector<int> >::iterator it = t_achievement_type_.find(type);
	if (it == t_achievement_type_.end())
	{
		return;
	}
	else
	{
		ids = t_achievement_type_[type];
	}
}

s_t_achievement * PlayerConfig::get_achievement(int id)
{
	std::map<int, s_t_achievement>::iterator it = t_achievement_.find(id);
	if (it == t_achievement_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

s_t_achievement_reward * PlayerConfig::get_achievement_reward(int id)
{
	if (id < 0 || id >= t_achievement_reward_.size())
	{
		return 0;
	}
	return &t_achievement_reward_[id];
}

s_t_toukuang * PlayerConfig::get_toukuang(int id)
{
	std::map<int, s_t_toukuang>::iterator it = t_toukuang_.find(id);
	if (it == t_toukuang_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

s_t_exp * PlayerConfig::get_exp(int level)
{
	if (level <= 0 || level > t_exp_.size())
	{
		return 0;
	}
	return &t_exp_[level - 1];
}

s_t_cup * PlayerConfig::get_cup(int cup)
{
	if (cup < 0)
	{
		return 0;
	}
	if (cup >= t_cup_.size())
	{
		cup = t_cup_.size() - 1;
	}
	return &t_cup_[cup];
}

std::string PlayerConfig::get_random_name()
{
	return t_first_names_[Utils::get_int32(0, t_first_names_.size() - 1)] + t_last_names_[Utils::get_int32(0, t_last_names_.size() - 1)];
}

std::string PlayerConfig::get_random_en_name()
{
	return t_3d_names_[Utils::get_int32(0, t_3d_names_.size() - 1)];
}


void PlayerConfig::get_task_by_type(int type, std::vector<int> &ids)
{
	std::map<int, std::vector<int> >::iterator it = t_task_type_.find(type);
	if (it == t_task_type_.end())
	{
		return;
	}
	else
	{
		ids = t_task_type_[type];
	}
}

s_t_task * PlayerConfig::get_task(int id)
{
	std::map<int, s_t_task>::iterator it = t_task_.find(id);
	if (it == t_task_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

s_t_fashion * PlayerConfig::get_fashion(int id)
{
	std::map<int, s_t_fashion>::iterator it = t_fashions_.find(id);
	if (it == t_fashions_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

void PlayerConfig::get_daily_by_type(int type, std::vector<int> &ids)
{
	std::map<int, std::vector<int> >::iterator it = t_daily_type_.find(type);
	if (it == t_daily_type_.end())
	{
		return;
	}
	else
	{
		ids = t_daily_type_[type];
	}
}

s_t_daily * PlayerConfig::get_daily(int id)
{
	std::map<int, s_t_daily>::iterator it = t_daily_.find(id);
	if (it == t_daily_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

s_t_daily_reward * PlayerConfig::get_daily_reward(int id)
{
	id = id - 1;
	if (id < 0 || id >= t_daily_reward_.size())
	{
		return 0;
	}
	return &t_daily_reward_[id];
}

s_t_vip_attr * PlayerConfig::get_vip_attr(int id)
{
	std::map<int, s_t_vip_attr>::iterator it = t_vip_attr_.find(id);
	if (it == t_vip_attr_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

int PlayerConfig::get_random_duobao()
{
	int sum = 0;
	for (int j = 0; j < t_duobao_rate_.size(); ++j)
	{
		sum += t_duobao_rate_[j];
	}
	if (sum == 0)
	{
		return 0;
	}
	int r = Utils::get_int32(0, sum - 1);
	int gl = 0;
	for (int j = 0; j < t_duobao_rate_.size(); ++j)
	{
		gl += t_duobao_rate_[j];
		if (gl > r)
		{
			return j;
		}
	}
	return 0;
}
