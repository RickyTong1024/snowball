#include "item_config.h"
#include "dbc.h"
#include "utils.h"
#include "role_operation.h"

int ItemConfig::parse()
{
	DBCFile * dbfile = service::scheme()->get_dbc("t_item.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_item t_item;
		t_item.id = dbfile->Get(i, 0)->iValue;
		t_item.name = dbfile->Get(i, 2)->pString;
		t_item.name = service::scheme()->get_lang_str(t_item.name);
		t_item.font_color = dbfile->Get(i, 3)->iValue;
		t_item.type = dbfile->Get(i, 4)->iValue;
		t_item.level = dbfile->Get(i, 5)->iValue;
		t_item.desc = dbfile->Get(i, 7)->pString;
		t_item.desc = service::scheme()->get_lang_str(t_item.desc);
		t_item.icon = dbfile->Get(i, 8)->pString;
		t_item.price = dbfile->Get(i, 9)->iValue;
		t_item.sell_type = dbfile->Get(i, 10)->iValue;
		t_item.sell = dbfile->Get(i, 11)->iValue;
		t_item.def1 = dbfile->Get(i, 12)->iValue;
		t_item.def2 = dbfile->Get(i, 13)->iValue;
		t_item.def3 = dbfile->Get(i, 14)->iValue;
		t_item.def4 = dbfile->Get(i, 15)->iValue;

		if (t_item.type == 1001)
		{
			t_role_patches_[t_item.def1] = t_item.id;
			t_color_suipians_[t_item.font_color].push_back(t_item.id);
		}
		else if (t_item.type == 2001)
		{
			t_bitems_.push_back(t_item.id);
		}

		t_items_[t_item.id] = t_item;
	}

	dbfile = service::scheme()->get_dbc("t_shop.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_shop t_shop;
		t_shop.id = dbfile->Get(i, 0)->iValue;
		t_shop.stype = dbfile->Get(i, 1)->iValue;
		t_shop.reward.type = dbfile->Get(i, 2)->iValue;
		t_shop.reward.value1 = dbfile->Get(i, 3)->iValue;
		t_shop.reward.value2 = dbfile->Get(i, 4)->iValue;
		t_shop.reward.value3 = dbfile->Get(i, 5)->iValue;
		t_shop.pay_type = dbfile->Get(i, 7)->iValue;
		t_shop.price = dbfile->Get(i, 8)->iValue;
		
		t_shops_[t_shop.id] = t_shop;
	}

	dbfile = service::scheme()->get_dbc("t_itembox.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_itembox t_itembox;
		t_itembox.id = dbfile->Get(i, 0)->iValue;
		t_itembox.type = dbfile->Get(i, 2)->iValue;
		for (int j = 0; j < 200; ++j)
		{
			s_t_reward t_reward;
			t_reward.type = dbfile->Get(i, 3 + j * 5)->iValue;
			t_reward.value1 = dbfile->Get(i, 4 + j * 5)->iValue;
			t_reward.value2 = dbfile->Get(i, 5 + j * 5)->iValue;
			t_reward.value3 = dbfile->Get(i, 6 + j * 5)->iValue;
			if (t_reward.type)
			{
				t_itembox.rewards.push_back(t_reward);
				int rate = dbfile->Get(i, 7 + j * 5)->iValue;
				t_itembox.rates.push_back(rate);
			}
		}

		t_itemboxs_[t_itembox.id] = t_itembox;
	}


	return 0;
}

s_t_item * ItemConfig::get_item(int id)
{
	std::map<int, s_t_item>::iterator it = t_items_.find(id);
	if (it == t_items_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

int ItemConfig::get_patch(int role_id)
{
	if (t_role_patches_.find(role_id) == t_role_patches_.end())
	{
		return 0;
	}
	return t_role_patches_[role_id];
}

int ItemConfig::get_random_suipian(int font_color)
{
	if (t_color_suipians_.find(font_color) == t_color_suipians_.end())
	{
		return 0;
	}
	std::vector<int> &v = t_color_suipians_[font_color];
	return v[Utils::get_int32(0, v.size() - 1)];
}

int ItemConfig::get_random_bitem()
{
	return t_bitems_[Utils::get_int32(0, t_bitems_.size() - 1)];
}

s_t_shop * ItemConfig::get_shop(int id)
{
	std::map<int, s_t_shop>::iterator it = t_shops_.find(id);
	if (it == t_shops_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

s_t_itembox * ItemConfig::get_itembox(int id)
{
	std::map<int, s_t_itembox>::iterator it = t_itemboxs_.find(id);
	if (it == t_itemboxs_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}
