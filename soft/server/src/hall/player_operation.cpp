#include "player_operation.h"
#include "item_operation.h"
#include "role_operation.h"
#include "item_config.h"
#include "player_config.h"
#include "post_operation.h"
#include "role_config.h"
#include "hall_pool.h"
#include "hall_message.h"
#include "chat_pool.h"
#include "social_pool.h"
#include "rank_operation.h"
#include "utils.h"

std::map<uint64_t, std::vector<int> > player_out_attr_;

void PlayerOperation::player_login(dhc::player_t *player)
{
	PlayerOperation::calc_out_attr(player);
	//////////////////////////////////////////////////////////////////////////
	/// 修复代码
	//////////////////////////////////////////////////////////////////////////
	bool flag = false;
	for (int i = 0; i < player->role_guid_size();)
	{
		uint64_t role_guid = player->role_guid(i);
		dhc::role_t *role = POOL_GET(role_guid, dhc::role_t);
		if (!role)
		{
			player->mutable_role_guid()->SwapElements(i, player->role_guid_size() - 1);
			player->mutable_role_guid()->RemoveLast();
			if (role_guid == player->role_on())
			{
				player->set_role_on(0);
			}
		}
		else
		{
			++i;
			if (role_guid == player->role_on())
			{
				flag = true;
			}
		}
	}
	if (!flag)
	{
		player->set_role_on(0);
	}
	if (!player->role_on())
	{
		if (player->role_guid_size() == 0)
		{
			dhc::role_t *role = RoleOperation::role_create(player, 1001);
			player->add_role_guid(role->guid());
		}
		player->set_role_on(player->role_guid(0));
	}
	int index = 0;
	while (index < player->battle_his_guids_size())
	{
		uint64_t battle_his_guid = player->battle_his_guids(index);
		dhc::battle_his_t *battle_his = POOL_GET(battle_his_guid, dhc::battle_his_t);
		if (!battle_his)
		{
			for (int i = index; i < player->battle_his_guids_size() - 1; ++i)
			{
				player->set_battle_his_guids(i, player->battle_his_guids(i + 1));
			}
			player->mutable_battle_his_guids()->RemoveLast();
		}
		else
		{
			index++;
		}
	}
	while (player->box_ids_size() < 4)
	{
		player->add_box_ids(0);
	}
	while (player->fashion_on_size() < 3)
	{
		player->add_fashion_on(0);
	}
	while (player->achieve_time_size() < player->achieve_reward_size())
	{
		player->add_achieve_time(service::timer()->now());
	}
	while (player->toukuang_time_size() < player->toukuang_size())
	{
		player->add_toukuang_time(0);
	}
	if (player->duobao_items_size() < 10)
	{
		refresh_duobao(player);
	}
	for (int i = 0; i < player->duobao_items_size(); ++i)
	{
		s_t_itembox *t_itembox = sItemConfig->get_itembox(3001 + i);
		if (t_itembox)
		{
			if (player->duobao_items(i) < 0 || player->duobao_items(i) >= t_itembox->rewards.size())
			{
				refresh_duobao(player);
				break;
			}
		}
	}
	sHallPool->player_tick(player);

	player_refresh_check(player);
}

void PlayerOperation::client_login(dhc::player_t *player)
{
	player->set_last_login_time(service::timer()->now());
	HallMessage::send_req_hall_rc_has_battle(player, boost::bind(&PlayerOperation::req_hall_rc_has_battle, _1, _2, player->guid()));
	sChatPool->add_player_channel(player->guid());
	sSocialPool->login(player);
	player_check_toukuang(player);
	RankOperation::rank_update_login(player);
}

void PlayerOperation::client_logout(dhc::player_t *player)
{
	sChatPool->remove_player_channel(player->guid());
	HallMessage::send_push_hall_team_exit1(player);
	sSocialPool->logout(player);
}

void PlayerOperation::req_hall_rc_has_battle(Packet *pck, int error_code, uint64_t player_guid)
{
	protocol::game::rep_hall_rc_has_battle msg;
	if (!pck->parse_protocol(msg))
	{
		return;
	}
	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (!ti)
	{
		return;
	}
	ti->set_battle_state(1 - msg.is_new(), msg.udp_ip(), msg.udp_port(), msg.tcp_ip(), msg.tcp_port(), msg.code());
	HallMessage::send_push_social_fight(player_guid, 1 - msg.is_new());
	if (ti->battle_state)
	{
		HallMessage::send_smsg_has_battle(player_guid);
	}
}

void PlayerOperation::player_logout(dhc::player_t *player)
{
	player_out_attr_.erase(player->guid());
}

void PlayerOperation::player_refresh_check(dhc::player_t *player)
{
	uint64_t now = service::timer()->now();
	if (service::timer()->trigger_time(player->last_daily_time(), 5, 0))
	{
		int day = service::timer()->run_day(player->last_daily_time());
		player->set_last_daily_time(now);
		player_refresh(player, day);
	}
	if (service::timer()->trigger_week_time(player->last_week_time()))
	{
		player->set_last_week_time(now);
		player_week_refresh(player);
	}
	if (service::timer()->trigger_month_time(player->last_month_time()))
	{
		player->set_last_month_time(now);
		player_month_refresh(player);
	}
}

void PlayerOperation::player_refresh(dhc::player_t *player, int day)
{
	player->set_box_zd_num(0);
	player->set_box_zd_opened(0);
	player->set_fenxiang_state(0);
	player->set_fenxiang_num(0);
	player->set_sign_finish(0);
	if (service::timer()->trigger_time(player->sign_time() + 86400000, 5, 0))
	{
		player->set_sign_index(0);
	}
	else
	{
		if (player->sign_index() >= 6)
		{
			player->set_sign_index(0);
		}
		else
		{
			player->set_sign_index(player->sign_index() + 1);
		}
	}
	player->clear_daily_id();
	player->clear_daily_num();
	player->clear_daily_reward();
	player->set_daily_point(0);
	player->clear_daily_get_id();
	player->clear_social_golds();
	player->set_yue_reward(0);
	player->set_nian_reward(0);
	player->set_duobao_num(0);
	player->set_advertisement_num(0);
}

void PlayerOperation::player_week_refresh(dhc::player_t *player)
{
	s_t_cup *t_cup = sPlayerConfig->get_cup(player->cup());
	if (!t_cup)
	{
		return;
	}
	std::string dw = t_cup->name;
	std::string title = service::scheme()->get_server_str("cup_title");
	std::string text = service::scheme()->get_server_str("cup_text", dw.c_str());
	std::string sys_name = service::scheme()->get_server_str("sys_name");
	s_t_rewards rewards;
	rewards.add_reward(t_cup->rewards);
	for (int i = 0; i < rewards.rewards.size(); ++i)
	{
		rewards.rewards[i].value2 *= 1 + PlayerOperation::get_out_attr(player, 10);
	}
	PostOperation::post_create(player->guid(), title, text, sys_name, rewards);
	PlayerOperation::player_set_cup(player, t_cup->down);
	player->set_battle_gold(0);
	refresh_duobao(player);
}

void PlayerOperation::player_month_refresh(dhc::player_t *player)
{
	
}

void PlayerOperation::player_add_reward(dhc::player_t *player, s_t_rewards &strewards)
{
	std::vector<s_t_reward> role_suipian_reward;
	for (int i = 0; i < strewards.rewards.size(); ++i)
	{
		// 资源
		if (strewards.rewards[i].type == 1)
		{
			player_add_resource(player,
				static_cast<resource::resource_t>(strewards.rewards[i].value1),
				strewards.rewards[i].value2);
		}
		// 道具
		else if (strewards.rewards[i].type == 2)
		{
			ItemOperation::item_add_template(player, strewards.rewards[i].value1, strewards.rewards[i].value2);
		}
		// 角色
		else if (strewards.rewards[i].type == 3)
		{
			if (RoleOperation::has_role(player, strewards.rewards[i].value1))
			{
				s_t_item *_item = sItemConfig->get_item(sItemConfig->get_patch(strewards.rewards[i].value1));
				if (_item)
				{
					ItemOperation::item_add_template(player, _item->id, 20);
					role_suipian_reward.push_back(s_t_reward(2, _item->id, 20));
				}
			}
			else
			{
				dhc::role_t *role = RoleOperation::role_create(player, strewards.rewards[i].value1);
				if (role)
				{
					strewards.roles.push_back(role);
				}
			}
		}
		// 头像
		else if (strewards.rewards[i].type == 4)
		{
			PlayerOperation::player_add_avatar(player, strewards.rewards[i].value1);
		}
		// 宝箱
		else if (strewards.rewards[i].type == 5)
		{
			int id = 0;
			if (strewards.rewards[i].value1 == 0)
			{
				id = PlayerOperation::player_add_random_box(player, 0);
				strewards.rewards[i].value1 = id;
			}
			else
			{
				id = strewards.rewards[i].value1;
				PlayerOperation::player_add_box(player, strewards.rewards[i].value1);
			}
			s_t_chest *t_chest = sPlayerConfig->get_chest(id);
			if (t_chest && t_chest->id >= 4)
			{
				std::string text = service::scheme()->get_server_str("sysinfo_aquire", player->name().c_str(), PlayerOperation::get_color_name(t_chest->font_color, t_chest->name).c_str());
				sChatPool->sys_info(text);
			}
		}
		// 头像
		else if (strewards.rewards[i].type == 6)
		{
			PlayerOperation::player_add_toukuang(player, strewards.rewards[i].value1);
		}
		// 皮肤
		else if (strewards.rewards[i].type == 7)
		{
			int sell = PlayerOperation::player_add_fashion(player, strewards.rewards[i].value1);
			if (sell)
			{
				player_add_resource(player, resource::SNOW, sell);
				role_suipian_reward.push_back(s_t_reward(1, 4, sell));
			}
		}
	}
	if (!role_suipian_reward.empty())
	{
		strewards.add_reward(role_suipian_reward);
	}
}

void PlayerOperation::player_add_resource(dhc::player_t *player, resource::resource_t type, int value)
{
	if (value == 0)
	{
		return;
	}
	switch (type)
	{
		case resource::GOLD:player->set_gold(player->gold() + value);
			if (player->gold() < 0)
			{
				player->set_gold(0);
			}
			break;
		case resource::JEWEL:player->set_jewel(player->jewel() + value);
			if (player->jewel() < 0)
			{
				player->set_jewel(0);
			}
			break;
		case resource::EXP:player_mod_exp(player, value);
			break;
		case resource::SNOW:player->set_snow(player->snow() + value);
			if (player->snow() < 0)
			{
				player->set_snow(0);
			}
			break;
		case resource::CUP:PlayerOperation::player_set_cup(player, player->cup() + value);
			break;
		case resource::APOINT:player->set_achieve_point(player->achieve_point() + value);
			if (player->achieve_point() < 0)
			{
				player->set_achieve_point(0);
			}
			break;
		case resource::DPOINT:player->set_daily_point(player->daily_point() + value);
			if (player->daily_point() < 0)
			{
				player->set_daily_point(0);
			}
			break;
		default:
			break;
	}
}

int PlayerOperation::player_get_resource(dhc::player_t *player, resource::resource_t type)
{
	switch (type)
	{
	case resource::GOLD:
		return player->gold();
		break;
	case resource::JEWEL:
		return player->jewel();
		break;
	case resource::EXP:
		return player->exp();
		break;
	case resource::SNOW:
		return player->snow();
		break;
	case resource::CUP:
		return player->cup();
		break;
	case resource::APOINT:
		return player->achieve_point();
		break;
	default:
		break;
	}
	return 0;
}

void PlayerOperation::player_dec_resource(dhc::player_t *player, resource::resource_t type, int value)
{
	if (type == resource::EXP)
	{
		return;
	}
	int jewel = player->jewel();
	PlayerOperation::player_add_resource(player, type, -value);
	if (jewel > player->jewel())
	{
		player->set_total_spend(player->total_spend() + jewel - player->jewel());
	}
}

void PlayerOperation::player_add_avatar(dhc::player_t *player, int avatar)
{
	for (int i = 0; i < player->avatar_size(); ++i)
	{
		if (player->avatar(i) == avatar)
		{
			return;
		}
	}

	s_t_avatar *t_avatar = sPlayerConfig->get_avatar(avatar);
	if (!t_avatar)
	{
		return;
	}

	player->add_avatar(avatar);
}

void PlayerOperation::player_add_toukuang(dhc::player_t *player, int id)
{
	s_t_toukuang *t_toukuang = sPlayerConfig->get_toukuang(id);
	if (!t_toukuang)
	{
		return;
	}
	for (int i = 0; i < player->toukuang_size(); ++i)
	{
		if (player->toukuang(i) == id)
		{
			if (t_toukuang->time > 0)
			{
				if (service::timer()->now() > player->toukuang_time(i))
				{
					player->set_toukuang_time(i, service::timer()->now() + (uint64_t)t_toukuang->time * 86400000);
				}
				else
				{
					player->set_toukuang_time(i, player->toukuang_time(i) + (uint64_t)t_toukuang->time * 86400000);
				}
			}
			return;
		}
	}

	player->add_toukuang(id);
	if (t_toukuang->time > 0)
	{
		player->add_toukuang_time(service::timer()->now() + (uint64_t)t_toukuang->time * 86400000);
	}
	else
	{
		player->add_toukuang_time(0);
	}
	calc_out_attr(player);
}

void PlayerOperation::player_check_toukuang(dhc::player_t *player)
{
	int index = 0;
	while (index < player->toukuang_time_size())
	{
		if (player->toukuang_time(index) > 0 && service::timer()->now() > player->toukuang_time(index))
		{
			if (player->toukuang_on() == player->toukuang(index))
			{
				player->set_toukuang_on(1);
			}
			player->set_toukuang(index, player->toukuang(player->toukuang_size() - 1));
			player->mutable_toukuang()->RemoveLast();

			player->set_toukuang_time(index, player->toukuang_time(player->toukuang_time_size() - 1));
			player->mutable_toukuang_time()->RemoveLast();
		}
		else
		{
			index++;
		}
	}
}

int PlayerOperation::player_add_fashion(dhc::player_t *player, int id)
{
	s_t_fashion *t_fashion = sPlayerConfig->get_fashion(id);
	if (!t_fashion)
	{
		return 0;
	}

	for (int i = 0; i < player->fashion_id_size(); ++i)
	{
		if (player->fashion_id(i) == id)
		{
			return t_fashion->sell;
		}
	}

	player->add_fashion_id(id);
	calc_out_attr(player);
	std::string text = service::scheme()->get_server_str("sysinfo_aquire", player->name().c_str(), PlayerOperation::get_color_name(t_fashion->font_color, t_fashion->name).c_str());
	sChatPool->sys_info(text);
	return 0;
}

void PlayerOperation::player_mod_exp(dhc::player_t *player, int exp)
{
	int last_level = player->level();
	int level = player->level();
	player->set_exp(player->exp() + exp);
	int nexp = player->exp();
	s_t_exp *t_exp = sPlayerConfig->get_exp(level + 1);
	if (!t_exp)
	{
		player->set_exp(0);
		return;
	}
	if (nexp < t_exp->exp)
	{
		/// 没升级
		return;
	}
	while (nexp >= t_exp->exp)
	{
		level += 1;
		nexp -= t_exp->exp;
		t_exp = sPlayerConfig->get_exp(level + 1);
		if (!t_exp)
		{
			nexp = 0;
			break;
		}
	}
	player->set_level(level);
	player->set_exp(nexp);
}

void PlayerOperation::player_set_cup(dhc::player_t *player, int cup)
{
	s_t_cup *t_old_cup = sPlayerConfig->get_cup(player->cup());
	s_t_cup *t_cup = sPlayerConfig->get_cup(cup);
	if (t_old_cup && t_cup && t_cup->id > t_old_cup->id && t_cup->id == 85)
	{
		std::string text = service::scheme()->get_server_str("sysinfo_cup", player->name().c_str(), PlayerOperation::get_color_name(3, t_cup->name).c_str());
		sChatPool->sys_info(text);
	}
	player->set_cup(cup);
	if (cup > player->max_cup())
	{
		player->set_max_cup(cup);
	}
}

void PlayerOperation::player_add_box(dhc::player_t *player, int id)
{
	s_t_chest *t_chest = sPlayerConfig->get_chest(id);
	if (!t_chest)
	{
		return;
	}
	int index = -1;
	for (int i = 0; i < player->box_ids_size(); ++i)
	{
		if (player->box_ids(i) == 0)
		{
			index = i;
			break;
		}
	}
	if (index == -1)
	{
		return;
	}

	player->set_box_ids(index, id);
}

int PlayerOperation::player_add_random_box(dhc::player_t *player, int rank)
{
	int index = -1;
	for (int i = 0; i < player->box_ids_size(); ++i)
	{
		if (player->box_ids(i) == 0)
		{
			index = i;
			break;
		}
	}
	if (index == -1)
	{
		return 0;
	}
	int id = sPlayerConfig->get_random_chest(player, rank);
	player_add_box(player, id);
	return id;
}

int PlayerOperation::player_recharge(dhc::player_t *player, int rid, int count, s_t_rewards &rewards)
{
	s_t_recharge *t_recharge = sPlayerConfig->get_recharge(rid);
	if (!t_recharge)
	{
		return -1;
	}

	int jewel = 0;
	int rmb = 0;
	if (count > 0 && count != t_recharge->rmb)
	{
		jewel = count * 10;
		rmb = count;
	}
	else
	{
		rmb = t_recharge->rmb;
		if (t_recharge->type == 1)
		{
			jewel = t_recharge->jewel;
		}
		else if (t_recharge->type == 2)
		{
			if (service::timer()->now() < player->yue_time())
			{
				return -1;
			}
			if (service::timer()->now() < player->nian_time())
			{
				return -1;
			}
			player->set_yue_time(service::timer()->now() + (uint64_t)86400000 * t_recharge->jewel);
			if (player->yue_first() == 0)
			{
				s_t_vip_attr *t_vip_attr = sPlayerConfig->get_vip_attr(1);
				if (!t_vip_attr)
				{
					return -1;
				}
				s_t_itembox *t_itembox = sItemConfig->get_itembox(t_vip_attr->first_id);
				if (!t_itembox)
				{
					return -1;
				}
				rewards.add_reward(t_itembox->rewards);
				PlayerOperation::player_add_reward(player, rewards);
			}
			player->set_yue_first(1);
		}
		else if (t_recharge->type == 3)
		{
			if (service::timer()->now() < player->nian_time())
			{
				return -1;
			}
			if (service::timer()->now() < player->yue_time())
			{
				jewel = (player->yue_time() - service::timer()->now()) / 8640000;
				player->set_yue_time(0);
			}
			player->set_nian_time(service::timer()->now() + (uint64_t)86400000 * t_recharge->jewel);
			if (player->nian_first() == 0)
			{
				s_t_vip_attr *t_vip_attr = sPlayerConfig->get_vip_attr(2);
				if (!t_vip_attr)
				{
					return -1;
				}
				s_t_itembox *t_itembox = sItemConfig->get_itembox(t_vip_attr->first_id);
				if (!t_itembox)
				{
					return -1;
				}
				rewards.add_reward(t_itembox->rewards);
				PlayerOperation::player_add_reward(player, rewards);
			}
			player->set_nian_first(1);
		}
	}
	if (player->first_recharge() == 0)
	{
		player->set_first_recharge(1);
	}

	PlayerOperation::player_add_resource(player, resource::JEWEL, jewel);
	player->set_total_recharge(player->total_recharge() + rmb);
	return 0;
}

void PlayerOperation::calc_out_attr(dhc::player_t *player)
{
	player_out_attr_.erase(player->guid());
	std::vector<int> out_attr;
	for (int i = 0; i < 100; ++i)
	{
		out_attr.push_back(0);
	}
	for (int i = 0; i < player->role_guid_size(); ++i)
	{
		dhc::role_t *role1 = POOL_GET(player->role_guid(i), dhc::role_t);
		if (!role1)
		{
			continue;
		}
		s_t_role *t_role = sRoleConfig->get_role(role1->template_id());
		if (!t_role)
		{
			continue;
		}
		for (int j = 0; j < t_role->guanghuan.size(); ++j)
		{
			s_t_role_buff *t_role_buff = sRoleConfig->get_role_buff(t_role->guanghuan[j]);
			if (!t_role_buff)
			{
				continue;
			}
			if (t_role_buff->attr.type == 3)
			{
				out_attr[t_role_buff->attr.param1] = out_attr[t_role_buff->attr.param1] + t_role_buff->attr.param3 + t_role_buff->attr.param4 * (role1->level() - 1);
			}
		}
	}
	s_t_exp *t_exp = sPlayerConfig->get_exp(player->level());
	if (t_exp)
	{
		for (int i = 0; i < t_exp->attrs.size(); ++i)
		{
			if (t_exp->attrs[i].type == 3)
			{
				out_attr[t_exp->attrs[i].param1] = out_attr[t_exp->attrs[i].param1] + t_exp->attrs[i].param3;
			}
		}
	}
	for (int i = 0; i < player->toukuang_size(); ++i)
	{
		s_t_toukuang *t_toukuang = sPlayerConfig->get_toukuang(player->toukuang(i));
		if (!t_toukuang)
		{
			continue;
		}
		for (int j = 0; j < t_toukuang->attrs.size(); ++j)
		{
			if (t_toukuang->attrs[j].type == 3)
			{
				out_attr[t_toukuang->attrs[j].param1] = out_attr[t_toukuang->attrs[j].param1] + t_toukuang->attrs[j].param3;
			}
		}
	}
	for (int i = 0; i < player->fashion_id_size(); ++i)
	{
		s_t_fashion *t_fashion = sPlayerConfig->get_fashion(player->fashion_id(i));
		if (!t_fashion)
		{
			continue;
		}
		if (t_fashion->attr.type == 3)
		{
			out_attr[t_fashion->attr.param1] = out_attr[t_fashion->attr.param1] + t_fashion->attr.param3;
		}
	}
	player_out_attr_[player->guid()] = out_attr;
}

int PlayerOperation::get_out_attr(dhc::player_t *player, int id)
{
	if (player_out_attr_.find(player->guid()) == player_out_attr_.end())
	{
		return 0;
	}
	std::vector<int> &out_attr = player_out_attr_[player->guid()];
	if (id < 0 || id >= out_attr.size())
	{
		return 0;
	}
	int a = out_attr[id];
	if (service::timer()->now() < player->yue_time())
	{
		s_t_vip_attr *t_vip_attr = sPlayerConfig->get_vip_attr(1);
		if (t_vip_attr)
		{
			for (int i = 0; i < t_vip_attr->attrs.size(); ++i)
			{
				if (t_vip_attr->attrs[i].param1 == id)
				{
					a += t_vip_attr->attrs[i].param3;
				}
			}
		}
	}
	else if (service::timer()->now() < player->nian_time())
	{
		s_t_vip_attr *t_vip_attr = sPlayerConfig->get_vip_attr(2);
		if (t_vip_attr)
		{
			for (int i = 0; i < t_vip_attr->attrs.size(); ++i)
			{
				if (t_vip_attr->attrs[i].param1 == id)
				{
					a += t_vip_attr->attrs[i].param3;
				}
			}
		}
	}
	return a;
}

void PlayerOperation::add_all_type_num(dhc::player_t *player, int type, int num)
{
	add_achieve_type_num(player, type, num);
	add_task_type_num(player, type, num);
	add_daily_type_num(player, type, num);
}

void PlayerOperation::add_achieve_type_num(dhc::player_t *player, int type, int num)
{
	std::vector<int> ids;
	sPlayerConfig->get_achievement_by_type(type, ids);
	for (int i = 0; i < ids.size(); ++i)
	{
		add_achieve_num(player, ids[i], num, false);
	}
}

void PlayerOperation::add_achieve_num(dhc::player_t *player, int id, int num, bool check)
{
	s_t_achievement *t_achievement = sPlayerConfig->get_achievement(id);
	if (!t_achievement)
	{
		return;
	}
	if (check && t_achievement->atype != 2)
	{
		return;
	}
	for (int i = 0; i < player->achieve_reward_size(); ++i)
	{
		if (player->achieve_reward(i) == id)
		{
			return;
		}
	}
	for (int i = 0; i < player->achieve_id_size(); ++i)
	{
		if (player->achieve_id(i) == id)
		{
			num = player->achieve_num(i) + num;
			if (num > t_achievement->count)
			{
				num = t_achievement->count;
			}
			player->set_achieve_num(i, num);
			return;
		}
	}
	if (num > t_achievement->count)
	{
		num = t_achievement->count;
	}
	player->add_achieve_id(id);
	player->add_achieve_num(num);
}

int PlayerOperation::get_achieve_num(dhc::player_t *player, int id)
{
	for (int i = 0; i < player->achieve_id_size(); ++i)
	{
		if (player->achieve_id(i) == id)
		{
			return player->achieve_num(i);
		}
	}
	return 0;
}

void PlayerOperation::del_achieve_num(dhc::player_t *player, int id)
{
	for (int i = 0; i < player->achieve_id_size(); ++i)
	{
		if (player->achieve_id(i) == id)
		{
			player->set_achieve_id(i, player->achieve_id(player->achieve_id_size() - 1));
			player->set_achieve_num(i, player->achieve_num(player->achieve_num_size() - 1));
			player->mutable_achieve_id()->RemoveLast();
			player->mutable_achieve_num()->RemoveLast();
			return;
		}
	}
}

void PlayerOperation::add_task_type_num(dhc::player_t *player, int type, int num)
{
	std::vector<int> ids;
	sPlayerConfig->get_task_by_type(type, ids);
	for (int i = 0; i < ids.size(); ++i)
	{
		add_task_num(player, ids[i], num, false);
	}
}

void PlayerOperation::add_task_num(dhc::player_t *player, int id, int num, bool check)
{
	s_t_task *t_task = sPlayerConfig->get_task(id);
	if (!t_task)
	{
		return;
	}
	if (check && t_task->atype != 2)
	{
		return;
	}
	if (player->level() < t_task->level)
	{
		return;
	}
	for (int i = 0; i < player->task_reward_size(); ++i)
	{
		if (player->task_reward(i) == id)
		{
			return;
		}
	}
	for (int i = 0; i < player->task_id_size(); ++i)
	{
		if (player->task_id(i) == id)
		{
			num = player->task_num(i) + num;
			if (num > t_task->count)
			{
				num = t_task->count;
			}
			player->set_task_num(i, num);
			return;
		}
	}
	if (num > t_task->count)
	{
		num = t_task->count;
	}
	player->add_task_id(id);
	player->add_task_num(num);
}

int PlayerOperation::get_task_num(dhc::player_t *player, int id)
{
	for (int i = 0; i < player->task_id_size(); ++i)
	{
		if (player->task_id(i) == id)
		{
			return player->task_num(i);
		}
	}
	return 0;
}

void PlayerOperation::del_task_num(dhc::player_t *player, int id)
{
	for (int i = 0; i < player->task_id_size(); ++i)
	{
		if (player->task_id(i) == id)
		{
			player->set_task_id(i, player->task_id(player->task_id_size() - 1));
			player->set_task_num(i, player->task_num(player->task_num_size() - 1));
			player->mutable_task_id()->RemoveLast();
			player->mutable_task_num()->RemoveLast();
			return;
		}
	}
}

void PlayerOperation::add_daily_type_num(dhc::player_t *player, int type, int num)
{
	std::vector<int> ids;
	sPlayerConfig->get_daily_by_type(type, ids);
	for (int i = 0; i < ids.size(); ++i)
	{
		add_daily_num(player, ids[i], num, false);
	}
}

void PlayerOperation::add_daily_num(dhc::player_t *player, int id, int num, bool check)
{
	s_t_daily *t_daily = sPlayerConfig->get_daily(id);
	if (!t_daily)
	{
		return;
	}
	if (check && t_daily->atype != 2)
	{
		return;
	}
	for (int i = 0; i < player->daily_reward_size(); ++i)
	{
		if (player->daily_reward(i) == id)
		{
			return;
		}
	}
	for (int i = 0; i < player->daily_id_size(); ++i)
	{
		if (player->daily_id(i) == id)
		{
			num = player->daily_num(i) + num;
			if (num > t_daily->count)
			{
				num = t_daily->count;
			}
			player->set_daily_num(i, num);
			return;
		}
	}
	if (num > t_daily->count)
	{
		num = t_daily->count;
	}
	player->add_daily_id(id);
	player->add_daily_num(num);
}

int PlayerOperation::get_daily_num(dhc::player_t *player, int id)
{
	for (int i = 0; i < player->daily_id_size(); ++i)
	{
		if (player->daily_id(i) == id)
		{
			return player->daily_num(i);
		}
	}
	return 0;
}

void PlayerOperation::del_daily_num(dhc::player_t *player, int id)
{
	for (int i = 0; i < player->daily_id_size(); ++i)
	{
		if (player->daily_id(i) == id)
		{
			player->set_daily_id(i, player->daily_id(player->daily_id_size() - 1));
			player->set_daily_num(i, player->daily_num(player->daily_num_size() - 1));
			player->mutable_daily_id()->RemoveLast();
			player->mutable_daily_num()->RemoveLast();
			return;
		}
	}
}

void PlayerOperation::refresh_duobao(dhc::player_t *player)
{
	player->clear_duobao_items();
	for (int i = 0; i < 10; ++i)
	{
		s_t_itembox *t_itembox = sItemConfig->get_itembox(3001 + i);
		if (t_itembox)
		{
			int sum = 0;
			for (int j = 0; j < t_itembox->rates.size(); ++j)
			{
				sum += t_itembox->rates[j];
			}
			if (sum != 0)
			{
				int r = Utils::get_int32(0, sum - 1);
				int gl = 0;
				for (int j = 0; j < t_itembox->rates.size(); ++j)
				{
					gl += t_itembox->rates[j];
					if (gl > r)
					{
						player->add_duobao_items(j);
						break;
					}
				}
			}
			else
			{
				player->add_duobao_items(0);
			}
		}
		else
		{
			player->add_duobao_items(0);
		}
	}
}

int PlayerOperation::player_get_name_color(dhc::player_t *player)
{
	if (player->nian_time() > service::timer()->now())
	{
		return 2;
	}
	else if (player->yue_time() > service::timer()->now())
	{
		return 1;
	}
	return 0;
}

std::vector<std::string> colors_;
std::string PlayerOperation::get_color_string(int color)
{
	if (colors_.size() == 0)
	{
		colors_.push_back("[3defff]");
		colors_.push_back("[ff41da]");
		colors_.push_back("[ffcd00]");
		colors_.push_back("[f01c1c]");
		colors_.push_back("[c2e5ed]");
		colors_.push_back("[57FC5B]");
	}
	if (color - 1 < 0 || color - 1 >= colors_.size())
	{
		return colors_[0];
	}
	return colors_[color - 1];
}

std::string PlayerOperation::get_color_name(int color, const std::string &text)
{
	std::string c = get_color_string(color);
	return c + text + "[-]";
}
