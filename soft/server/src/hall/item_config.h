#ifndef __ITEM_CONFIG_H__
#define __ITEM_CONFIG_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "reward.h"

struct s_t_item
{
	int id;
	std::string name;
	int font_color;
	int type;
	int level;
	std::string icon;
	std::string desc;
	int sell_type;
	int sell;
	int price;
	int def1;
	int def2;
	int def3;
	int def4;
};

struct s_t_shop
{
	int id;
	int stype;
	s_t_reward reward;
	int pay_type;
	int price;
};

struct s_t_itembox
{
	int id;
	int type;
	std::vector<s_t_reward> rewards;
	std::vector<int> rates;
};

class ItemConfig
{
public:
	int parse();

	s_t_item * get_item(int id);

	int get_patch(int role_id);

	int get_random_suipian(int font_color);

	int get_random_bitem();

	s_t_shop * get_shop(int id);

	s_t_itembox * get_itembox(int id);

private:
	std::map<int, s_t_item> t_items_;
	std::map<int, int> t_role_patches_;
	std::map<int, std::vector<int> > t_color_suipians_;
	std::vector<int> t_bitems_;
	std::map<int, s_t_shop> t_shops_;
	std::map<int, s_t_itembox> t_itemboxs_;
};

#define sItemConfig (Singleton<ItemConfig>::instance ())

#endif
