#include "item_operation.h"
#include "item_config.h"

void ItemOperation::item_add_template(dhc::player_t *player, int item_id, int32_t item_amount)
{
	s_t_item *t_item = sItemConfig->get_item(item_id);
	if (!t_item)
	{
		return;
	}
	bool flag = false;
	for (int i = 0; i < player->item_id_size(); ++i)
	{
		if (player->item_id(i) == item_id)
		{
			player->set_item_num(i, player->item_num(i) + item_amount);
			flag = true;
			break;
		}
	}
	if (!flag)
	{
		player->add_item_id(item_id);
		player->add_item_num(item_amount);
	}
}

int ItemOperation::item_num_templete(dhc::player_t *player, int item_id)
{
	for (int i = 0; i < player->item_id_size(); ++i)
	{
		if (player->item_id(i) == item_id)
		{
			return player->item_num(i);
		}
	}
	return 0;
}

void ItemOperation::item_destory_templete(dhc::player_t *player, int item_id, int item_amount)
{
	for (int i = 0; i < player->item_id_size(); ++i)
	{
		if (player->item_id(i) == item_id)
		{
			if (player->item_num(i) > item_amount)
			{
				player->set_item_num(i, player->item_num(i) - item_amount);
			}
			else
			{
				player->mutable_item_id()->SwapElements(i, player->item_id_size() - 1);
				player->mutable_item_id()->RemoveLast();
				player->mutable_item_num()->SwapElements(i, player->item_num_size() - 1);
				player->mutable_item_num()->RemoveLast();
			}
			break;
		}
	}
}

std::string ItemOperation::get_color(int color, const std::string &name)
{
	if (color == 0)
	{
		return "[ffffff]" + name + "[-]";
	}
	else if (color == 1)
	{
		return "[5cf732]" + name + "[-]";
	}
	else if (color == 2)
	{
		return "[32eef7]" + name + "[-]";
	}
	else if (color == 3)
	{
		return "[ff3fbf]" + name + "[-]";
	}
	else if (color == 4)
	{
		return "[ee9900]" + name + "[-]";
	}
	else if (color == 5)
	{
		return "[ffff00]" + name + "[-]";
	}
	return name;
}
