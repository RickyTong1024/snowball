#include "item_manager.h"
#include "item_config.h"
#include "hall_message.h"
#include "item_operation.h"
#include "player_operation.h"
#include "player_config.h"
#include "utils.h"

ItemManager::ItemManager()
{

}

ItemManager::~ItemManager()
{

}

int ItemManager::init()
{
	if (-1 == sItemConfig->parse())
	{
		return -1;
	}
	return 0;
}

int ItemManager::fini()
{
	return 0;
}

int ItemManager::terminal_item_buy(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_item_buy msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	int num = msg.num();
	if (num <= 0 || num > 100)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_shop *t_shop = sItemConfig->get_shop(msg.id());
	if (!t_shop)
	{
		ERROR_SYS;
		return -1;
	}
	int pn = PlayerOperation::player_get_resource(player, (resource::resource_t)t_shop->pay_type);
	if (pn < t_shop->price * num)
	{
		ERROR_SYS;
		return -1;
	}
	
	PlayerOperation::player_dec_resource(player, (resource::resource_t)t_shop->pay_type, t_shop->price * num);

	if (t_shop->stype == 5)
	{
		s_t_rewards rewards;
		sPlayerConfig->open_chest(player, t_shop->reward.value1, rewards);
		PlayerOperation::player_add_reward(player, rewards);
		HallMessage::send_smsg_end_open_box(player, SMSG_ITEM_BUY_BOX, t_shop->reward.value1, 0, rewards);
	}
	else
	{
		s_t_rewards rewards;
		rewards.add_reward(t_shop->reward.type, t_shop->reward.value1, t_shop->reward.value2 * num, t_shop->reward.value3);
		PlayerOperation::player_add_reward(player, rewards);
		HallMessage::send_smsg_item_buy(player, rewards);
		if (t_shop->stype == 1)
		{
			PlayerOperation::add_all_type_num(player, 40, 1);
		}
	}

	return 0;
}

int ItemManager::terminal_item_sell(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_item_sell msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	int num = msg.num();
	if (num <= 0)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_item *t_item = sItemConfig->get_item(msg.id());
	if (!t_item)
	{
		ERROR_SYS;
		return -1;
	}
	int item_num = ItemOperation::item_num_templete(player, t_item->id);
	if (num > item_num)
	{
		ERROR_SYS;
		return -1;
	}
	if (t_item->type == 1001)
	{
		PlayerOperation::add_all_type_num(player, 240, 1);
	}
	PlayerOperation::player_add_resource(player, resource::resource_t(t_item->sell_type), num * t_item->sell);
	ItemOperation::item_destory_templete(player, t_item->id, num);
	HallMessage::send_smsg_success(player, SMSG_ITEM_SELL);

	return 0;
}

int ItemManager::terminal_item_direct_buy(Packet *pck, const std::string &name)
{
	return 0;
}

int ItemManager::terminal_item_apply(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_item_apply msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	int num = msg.num();
	if (num <= 0)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_item *t_item = sItemConfig->get_item(msg.item_id());
	if (!t_item)
	{
		ERROR_SYS;
		return -1;
	}
	if (t_item->type != 3001)
	{
		ERROR_SYS;
		return -1;
	}
	if (player->level() < t_item->level)
	{
		ERROR_SYS;
		return -1;
	}
	int item_num = ItemOperation::item_num_templete(player, t_item->id);
	if (num > item_num)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_itembox *t_itembox = sItemConfig->get_itembox(t_item->def1);
	if (!t_itembox)
	{
		ERROR_SYS;
		return -1;
	}

	s_t_rewards rds;
	for (int k = 0; k < num; ++k)
	{
		if (t_itembox->type == 1)
		{
			rds.add_reward(t_itembox->rewards);
		}
		else if (t_itembox->type == 3)
		{
			if (msg.item_index() < 0 || msg.item_index() >= t_itembox->rewards.size())
			{
				ERROR_SYS;
				return -1;
			}
			rds.add_reward(t_itembox->rewards[msg.item_index()]);
		}
		else
		{
			int sum = 0;
			for (int i = 0; i < t_itembox->rates.size(); ++i)
			{
				sum += t_itembox->rates[i];
			}
			if (sum == 0)
			{
				ERROR_SYS;
				return -1;
			}
			int r = Utils::get_int32(0, sum - 1);
			int gl = 0;
			for (int i = 0; i < t_itembox->rates.size(); ++i)
			{
				gl += t_itembox->rates[i];
				if (gl > r)
				{
					rds.add_reward(t_itembox->rewards[i]);
					break;
				}
			}
		}
	}
	PlayerOperation::player_add_reward(player, rds);
	ItemOperation::item_destory_templete(player, t_item->id, num);
	HallMessage::send_smsg_item_apply(player, rds);
	return 0;
}

int ItemManager::terminal_duobao(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	if (PlayerOperation::player_get_resource(player, resource::JEWEL) < 50)
	{
		ERROR_SYS;
		return -1;
	}
	if (player->duobao_num() >= 10)
	{
		ERROR_SYS;
		return -1;
	}
	int id = sPlayerConfig->get_random_duobao();
	s_t_itembox *t_itembox = sItemConfig->get_itembox(3001 + id);
	if (!t_itembox)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_rewards rewards;
	int index = player->duobao_items(id);
	rewards.add_reward(t_itembox->rewards[index]);
	PlayerOperation::player_add_reward(player, rewards);
	PlayerOperation::player_dec_resource(player, resource::JEWEL, 50);
	player->set_duobao_num(player->duobao_num() + 1);
	HallMessage::send_smsg_duobao(player, id + 1);
	return 0;
}
