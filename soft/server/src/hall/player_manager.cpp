#include "player_manager.h"
#include "hall_message.h"
#include "hall_manager.h"
#include "player_config.h"
#include "player_operation.h"
#include "player_loader.h"
#include <boost/algorithm/string.hpp>
#include "item_config.h"
#include "item_operation.h"
#include "utils.h"
#include "post_operation.h"
#include "boost/regex.hpp"
#include "role_config.h"
#include "social_pool.h"
#include "rank_operation.h"

PlayerManager::PlayerManager()
{
}

PlayerManager::~PlayerManager()
{
}

int PlayerManager::init()
{
	if (-1 == sPlayerConfig->parse())
	{
		return -1;
	}
	return 0;
}

int PlayerManager::fini()
{
	sPlayerLoader->save_all();
	return 0;
}

int PlayerManager::terminal_login_player(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_login_player msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	uint64_t player_guid = pck->guid();

	TermInfo *oti = sHallPool->get_terminfo(player_guid);
	if (oti)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_LOGIN_OTHER);
	}

	TermInfo ti;
	ti.guid = player_guid;
	ti.acc = new dhc::acc_t();
	ti.acc->CopyFrom(msg.acc());
	ti.gate_hid = pck->hid();
	ti.gate_name = name;

	sHallPool->add_terminfo(ti.guid, ti);
	dhc::player_t *player = POOL_GET(ti.guid, dhc::player_t);
	if (!player)
	{
		sPlayerLoader->load_player(ti.guid);
	}
	else
	{
		sHallPool->del_player_time(ti.guid);
		HallMessage::send_push_hall_team_exit1(player);
		PlayerOperation::client_login(player);
		HallMessage::send_smsg_login_player(player);
	}
	return 0;
}

int PlayerManager::push_gate_hall_logout(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	sHallPool->add_player_time(player_guid);
	sHallPool->del_terminfo(player_guid);
	PlayerOperation::client_logout(player);
	return 0;
}

int PlayerManager::terminal_player_look(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_player_look msg;
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
	uint64_t target_guid = msg.target_guid();
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	int index = target_guid % names.size();
	int cur_index = pck->guid() % names.size();
	if (index != cur_index)
	{
		HallMessage::send_req_hall_center_player_look(target_guid, boost::bind(&PlayerManager::req_hall_center_player_look_callback, this, _1, _2, player_guid));
		return 0;
	}
	dhc::player_t *target = POOL_GET(target_guid, dhc::player_t);
	if (!target)
	{
		sPlayerLoader->load_player(target_guid, boost::bind(&PlayerManager::terminal_player_look_callback, this, player_guid, target_guid));
		return 0;
	}
	HallMessage::send_smsg_player_look(player_guid, target);
	return 0;
}

void PlayerManager::terminal_player_look_callback(uint64_t player_guid, uint64_t target_guid)
{
	dhc::player_t *target = POOL_GET(target_guid, dhc::player_t);
	if (!target)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_SYSTEM);
		return;
	}
	HallMessage::send_smsg_player_look(player_guid, target);
}

void PlayerManager::req_hall_center_player_look_callback(Packet *pck, int error_code, uint64_t player_guid)
{
	protocol::game::smsg_player_look msg;
	if (!pck->parse_protocol(msg))
	{
		return;
	}
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, (operror_t)error_code);
		return;
	}
	
	HallMessage::send_smsg_player_look(player_guid, &msg);
}

int PlayerManager::req_center_hall_player_look(Packet *pck, const std::string &name, int id)
{
	protocol::game::cmsg_player_look msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t target_guid = msg.target_guid();
	dhc::player_t *target = POOL_GET(target_guid, dhc::player_t);
	if (!target)
	{
		sPlayerLoader->load_player(target_guid, boost::bind(&PlayerManager::req_center_hall_player_look_callback, this, id, target_guid));
		return 0;
	}
	HallMessage::send_rep_center_hall_player_look(id, 0, target);
	return 0;
}

void PlayerManager::req_center_hall_player_look_callback(int id, uint64_t target_guid)
{
	dhc::player_t *target = POOL_GET(target_guid, dhc::player_t);
	if (!target)
	{
		return;
	}
	HallMessage::send_rep_center_hall_player_look(id, 0, target);
}

int PlayerManager::terminal_avatar_on(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_avatar_on msg;
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
	int id = msg.id();
	s_t_avatar *avatar = sPlayerConfig->get_avatar(id);
	if (!avatar)
	{
		ERROR_SYS;
		return -1;
	}
	bool flag = false;
	for (int i = 0; i < player->avatar_size(); ++i)
	{
		if (player->avatar(i) == id)
		{
			flag = true;
			break;
		}
	}
	if (!flag)
	{
		ERROR_SYS;
		return -1;
	}
	player->set_avatar_on(id);
	HallMessage::send_smsg_success(player, SMSG_AVATAR_ON);
	return 0;
}

int PlayerManager::terminal_toukuang_on(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_toukuang_on msg;
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
	int id = msg.id();
	s_t_toukuang *toukuang = sPlayerConfig->get_toukuang(id);
	if (!toukuang)
	{
		ERROR_SYS;
		return -1;
	}
	bool flag = false;
	for (int i = 0; i < player->toukuang_size(); ++i)
	{
		if (player->toukuang(i) == id)
		{
			flag = true;
			break;
		}
	}
	if (!flag)
	{
		ERROR_SYS;
		return -1;
	}
	player->set_toukuang_on(id);
	HallMessage::send_smsg_success(player, SMSG_TOUKUANG_ON);
	return 0;
}

int PlayerManager::terminal_gm_command(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_gm_command msg;
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
	int test = boost::lexical_cast<int>(service::server_env()->get_game_value("test"));
	if (test != 1)
	{
		ERROR_SYS;
		return -1;
	}
	std::vector<std::string> texts;
	boost::split(texts, msg.text(), boost::is_any_of(" "));
	if (texts.size() == 0)
	{
		ERROR_SYS;
		return -1;
	}

	s_t_rewards rds;
	if (texts[0] == "addreward")
	{
		if (texts.size() < 5)
		{
			ERROR_SYS;
			return -1;
		}
		try
		{
			s_t_reward t_reward;
			t_reward.type = boost::lexical_cast<int>(texts[1]);
			t_reward.value1 = boost::lexical_cast<int>(texts[2]);
			t_reward.value2 = boost::lexical_cast<int>(texts[3]);
			t_reward.value3 = boost::lexical_cast<int>(texts[4]);
			rds.add_reward(t_reward);
			PlayerOperation::player_add_reward(player, rds);

			HallMessage::send_smsg_gm_command(player, rds);
		}
		catch (...)
		{
			ERROR_SYS;
			return -1;
		}
	}
	else if (texts[0] == "onlinepost" || texts[0] == "offlinepost")
	{
		if (texts.size() >= 5)
		{
			try
			{
				s_t_reward t_reward;
				t_reward.type = boost::lexical_cast<int>(texts[1]);
				t_reward.value1 = boost::lexical_cast<int>(texts[2]);
				t_reward.value2 = boost::lexical_cast<int>(texts[3]);
				t_reward.value3 = boost::lexical_cast<int>(texts[4]);
				rds.add_reward(t_reward);
			}
			catch (...)
			{
				ERROR_SYS;
				return -1;
			}
		}
		s_t_rewards rds1;
		std::string s = boost::lexical_cast<std::string>(Utils::get_int32(0, 999));
		if (texts[0] == "onlinepost")
		{
			PostOperation::post_create_online(player, s, "bbb", "ccc", rds);
			HallMessage::send_smsg_gm_command(player, rds1);
		}
		else
		{
			PostOperation::post_create_offline(player->guid(), s, "bbb", "ccc", rds);
			HallMessage::send_smsg_gm_command(player, rds1);
		}
	}
	return 0;
}

int PlayerManager::terminal_player_change_name(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_player_name msg;
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
	std::string player_name = msg.name();
	if (player_name.empty())
	{
		ERROR_SYS;
		return -1;
	}
	if (player_name.size() > 24 || player_name.size() < 2)
	{
		ERROR_SYS;
		return -1;
	}
	if (service::scheme()->search_illword(player_name) == -1)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_NAME_ILL);
		return -1;
	}
	int num = player->change_name_num();
	if (num > 0)
	{
		int item_num = ItemOperation::item_num_templete(player, 50010002);
		if (item_num < 1)
		{
			ERROR_SYS;
			return -1;
		}
		ItemOperation::item_destory_templete(player, 50010002, 1);
	}
	player->set_name(player_name);
	player->set_change_name_num(num + 1);
	HallMessage::send_smsg_success(player, SMSG_CHANGE_NAME);
	HallMessage::send_push_hall_name_insert(player->guid(), player->name());
	return 0;
}

int PlayerManager::terminal_player_modify_data(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_player_data msg;
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
	int player_sex = msg.sex();
	if (player_sex > 1 || player_sex < 0)
	{
		ERROR_SYS;
		return -1;
	}
	player->set_region_id(msg.region_id());
	player->set_sex(player_sex);
	HallMessage::send_smsg_success(player, SMSG_MODIFY_DATA);
	return 0;
}

int PlayerManager::terminal_start_open_box(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_start_open_box msg;
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
	if (player->box_open_slot() > 0)
	{
		ERROR_SYS;
		return -1;
	}
	int slot = msg.slot();
	if (slot < 1 || slot > player->box_ids_size())
	{
		ERROR_SYS;
		return -1;
	}
	int id = player->box_ids(slot - 1);
	if (id == 0)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_chest *t_chest = sPlayerConfig->get_chest(id);
	if (!t_chest)
	{
		ERROR_SYS;
		return -1;
	}
	player->set_box_open_slot(slot);
	uint64_t time = t_chest->time * 1000;
	time = time * (100 - PlayerOperation::get_out_attr(player, 6)) / 100;
	player->set_box_open_time(service::timer()->now() + time);
	HallMessage::send_smsg_success(player, SMSG_START_OPEN_BOX);
	return 0;
}

int PlayerManager::terminal_end_open_box(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_end_open_box msg;
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
	if (player->box_open_slot() == 0)
	{
		ERROR_SYS;
		return -1;
	}
	int slot = msg.slot();
	if (slot != player->box_open_slot())
	{
		ERROR_SYS;
		return -1;
	}
	int id = player->box_ids(slot - 1);
	if (id == 0)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_chest *t_chest = sPlayerConfig->get_chest(id);
	if (!t_chest)
	{
		ERROR_SYS;
		return -1;
	}
	int snow = 0;
	if (!msg.is_jewel())
	{
		if (service::timer()->now() < player->box_open_time())
		{
			ERROR_SYS;
			return -1;
		}
	}
	else
	{
		if (service::timer()->now() < player->box_open_time())
		{
			int time = player->box_open_time() - service::timer()->now();
			snow = (time + 179999) / 180000;
			if (PlayerOperation::player_get_resource(player, resource::JEWEL) < snow)
			{
				ERROR_SYS;
				return -1;
			}
			PlayerOperation::player_dec_resource(player, resource::JEWEL, snow);
		}
	}
	/// 开宝箱获得物品
	s_t_rewards rewards;
	sPlayerConfig->open_chest(player, id, rewards);
	PlayerOperation::player_add_reward(player, rewards);
	player->set_box_ids(slot - 1, 0);
	player->set_box_open_slot(0);
	HallMessage::send_smsg_end_open_box(player, SMSG_END_OPEN_BOX, id, snow, rewards);
	return 0;
}

int PlayerManager::terminal_open_battle_box(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	if (player->box_zd_num() < 3)
	{
		ERROR_SYS;
		return -1;
	}
	if (player->box_zd_opened())
	{
		ERROR_SYS;
		return -1;
	}
	player->set_box_zd_opened(1);
	int id = 201;
	s_t_rewards rewards;
	sPlayerConfig->open_chest(player, id, rewards);
	PlayerOperation::player_add_reward(player, rewards);
	HallMessage::send_smsg_end_open_box(player, SMSG_OPEN_BATTLE_BOX, id, 0, rewards);
	return 0;
}

int PlayerManager::terminal_sign(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	if (player->sign_finish())
	{
		ERROR_SYS;
		return -1;
	}
	s_t_sign *t_sign = sPlayerConfig->get_sign(player->sign_index());
	if (!t_sign)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_rewards rewards;
	rewards.add_reward(t_sign->reward);
	for (int i = 0; i < rewards.rewards.size(); ++i)
	{
		rewards.rewards[i].value2 *= 1 + PlayerOperation::get_out_attr(player, 9);
	}
	PlayerOperation::player_add_reward(player, rewards);
	player->set_sign_finish(1);
	player->set_sign_time(service::timer()->now());
	HallMessage::send_smsg_success(player, SMSG_SIGN);
	return 0;
}

int PlayerManager::terminal_fengxiang(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	if (player->fenxiang_state() != 0)
	{
		ERROR_SYS;
		return -1;
	}
	player->set_fenxiang_state(1);
	PlayerOperation::add_all_type_num(player, 43, 1);
	HallMessage::send_smsg_success(player, SMSG_FENXIANG);
	return 0;
}

int PlayerManager::terminal_open_fengxiang_box(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	if (player->fenxiang_state() != 1)
	{
		ERROR_SYS;
		return -1;
	}
	player->set_fenxiang_state(2);
	int id = 301;
	s_t_rewards rewards;
	sPlayerConfig->open_chest(player, id, rewards);
	PlayerOperation::player_add_reward(player, rewards);
	HallMessage::send_smsg_end_open_box(player, SMSG_OPEN_FENXIANG_BOX, id, 0, rewards);
	return 0;
}

int PlayerManager::terminal_infomation(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_infomation msg;
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
	std::string info = msg.info();
	if (info.size() > 40)
	{
		ERROR_SYS;
		return -1;
	}
	player->set_infomation(info);
	HallMessage::send_smsg_success(player, SMSG_INFOMATION);
	return 0;
}

int PlayerManager::terminal_battle_achieve(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_battle_achieve msg;
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
	for (int i = 0; i < msg.id_size(); ++i)
	{
		PlayerOperation::add_achieve_num(player, msg.id(i), msg.num(i), true);
	}
	
	HallMessage::send_smsg_success(player, SMSG_BATTLE_ACHIEVE);
	return 0;
}

int PlayerManager::terminal_achieve(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_achieve msg;
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
	s_t_achievement *t_achievement = sPlayerConfig->get_achievement(msg.id());
	if (!t_achievement)
	{
		ERROR_SYS;
		return -1;
	}
	bool flag = false;
	for (int i = 0; i < player->achieve_reward_size(); ++i)
	{
		if (player->achieve_reward(i) == t_achievement->id)
		{
			ERROR_SYS;
			return -1;
		}
		if (player->achieve_reward(i) == t_achievement->pre)
		{
			flag = true;
		}
	}
	if (t_achievement->pre > 0 && !flag)
	{
		ERROR_SYS;
		return -1;
	}
	if (t_achievement->type == 1)
	{
		int num = 0;
		for (int i = 0; i < player->role_guid_size(); ++i)
		{
			uint64_t role_guid = player->role_guid(i);
			dhc::role_t *role = POOL_GET(role_guid, dhc::role_t);
			if (!role)
			{
				continue;
			}
			s_t_role *t_role = sRoleConfig->get_role(role->template_id());
			if (!t_role)
			{
				continue;
			}
			if (t_achievement->param1 != 2 && t_achievement->param1 != t_role->sex)
			{
				continue;
			}
			if (t_achievement->param2 != 0 && t_achievement->param2 != t_role->font_color)
			{
				continue;
			}
			num++;
		}
		if (num < t_achievement->count)
		{
			ERROR_SYS;
			return -1;
		}
	}
	else if (t_achievement->type == 2)
	{
		if (player->level() < t_achievement->count)
		{
			ERROR_SYS;
			return -1;
		}
	}
	else if (t_achievement->type == 3)
	{
		for (int i = 0; i < player->role_guid_size(); ++i)
		{
			uint64_t role_guid = player->role_guid(i);
			dhc::role_t *role = POOL_GET(role_guid, dhc::role_t);
			if (!role)
			{
				continue;
			}
			if (role->template_id() == t_achievement->param1)
			{
				if (role->level() < t_achievement->count)
				{
					ERROR_SYS;
					return -1;
				}
				break;
			}
		}
	}
	else if (t_achievement->type == 35)
	{
		if (player->cup() < t_achievement->count)
		{
			ERROR_SYS;
			return -1;
		}
	}
	else
	{
		int num = PlayerOperation::get_achieve_num(player, t_achievement->id);
		if (num < t_achievement->count)
		{
			ERROR_SYS;
			return -1;
		}
	}
	PlayerOperation::del_achieve_num(player, t_achievement->id);
	player->add_achieve_reward(t_achievement->id);
	player->add_achieve_time(service::timer()->now());
	player->set_achieve_point(player->achieve_point() + t_achievement->point);
	RankOperation::rank_update(player, rt_achieve);
	HallMessage::send_smsg_success(player, SMSG_ACHIEVE);
	return 0;
}

int PlayerManager::terminal_achieve_reward(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	s_t_achievement_reward *t_achievement_reward = sPlayerConfig->get_achievement_reward(player->achieve_index() + 1);
	if (!t_achievement_reward)
	{
		ERROR_SYS;
		return -1;
	}
	if (player->achieve_point() < t_achievement_reward->point)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_rewards rewards;
	rewards.add_reward(t_achievement_reward->rewards);
	PlayerOperation::player_add_reward(player, rewards);
	player->set_achieve_index(player->achieve_index() + 1);
	HallMessage::send_smsg_success(player, SMSG_ACHIEVE_REWARD);
	return 0;
}

int PlayerManager::terminal_battle_task(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_battle_task msg;
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
	for (int i = 0; i < msg.id_size(); ++i)
	{
		PlayerOperation::add_task_num(player, msg.id(i), msg.num(i), true);
	}

	HallMessage::send_smsg_success(player, SMSG_BATTLE_TASK);
	return 0;
}

int PlayerManager::terminal_task(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_task msg;
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
	s_t_task *t_task = sPlayerConfig->get_task(msg.id());
	if (!t_task)
	{
		ERROR_SYS;
		return -1;
	}
	bool flag = false;
	for (int i = 0; i < player->task_reward_size(); ++i)
	{
		if (player->task_reward(i) == t_task->id)
		{
			ERROR_SYS;
			return -1;
		}
		if (player->task_reward(i) == t_task->pre)
		{
			flag = true;
		}
	}
	if (t_task->pre > 0 && !flag)
	{
		ERROR_SYS;
		return -1;
	}
	if (t_task->type == 1)
	{
		int num = 0;
		for (int i = 0; i < player->role_guid_size(); ++i)
		{
			uint64_t role_guid = player->role_guid(i);
			dhc::role_t *role = POOL_GET(role_guid, dhc::role_t);
			if (!role)
			{
				continue;
			}
			s_t_role *t_role = sRoleConfig->get_role(role->template_id());
			if (!t_role)
			{
				continue;
			}
			if (t_task->param1 != 2 && t_task->param1 != t_role->sex)
			{
				continue;
			}
			if (t_task->param2 != 0 && t_task->param2 != t_role->font_color)
			{
				continue;
			}
			num++;
		}
		if (num < t_task->count)
		{
			ERROR_SYS;
			return -1;
		}
	}
	else if (t_task->type == 2)
	{
		if (player->level() < t_task->count)
		{
			ERROR_SYS;
			return -1;
		}
	}
	else if (t_task->type == 3)
	{
		int num = 0;
		for (int i = 0; i < player->role_guid_size(); ++i)
		{
			uint64_t role_guid = player->role_guid(i);
			dhc::role_t *role = POOL_GET(role_guid, dhc::role_t);
			if (role && role->level() >= 5)
			{
				num++;
			}
		}
		if (num < t_task->count)
		{
			ERROR_SYS;
			return -1;
		}
	}
	else if (t_task->type == 35)
	{
		if (player->cup() < t_task->count)
		{
			ERROR_SYS;
			return -1;
		}
	}
	else
	{
		int num = PlayerOperation::get_task_num(player, t_task->id);
		if (num < t_task->count)
		{
			ERROR_SYS;
			return -1;
		}
	}
	PlayerOperation::del_task_num(player, t_task->id);
	player->add_task_reward(t_task->id);
	s_t_rewards rewards;
	rewards.add_reward(t_task->rewards);
	PlayerOperation::player_add_reward(player, rewards);
	HallMessage::send_smsg_success(player, SMSG_TASK);
	return 0;
}

int PlayerManager::terminal_fashion_on(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_fashion_on msg;
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
	int id = msg.id();
	int type = msg.type();
	if (type < 1 || type > player->fashion_on_size())
	{
		ERROR_SYS;
		return -1;
	}
	if (id != 0)
	{
		s_t_fashion *fashion = sPlayerConfig->get_fashion(id);
		if (!fashion)
		{
			ERROR_SYS;
			return -1;
		}
		if (fashion->type != type)
		{
			ERROR_SYS;
			return -1;
		}
		bool flag = false;
		for (int i = 0; i < player->fashion_id_size(); ++i)
		{
			if (player->fashion_id(i) == id)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			ERROR_SYS;
			return -1;
		}
	}
	
	player->set_fashion_on(type - 1, id);
	HallMessage::send_smsg_success(player, SMSG_FASHION_ON);
	return 0;
}

int PlayerManager::terminal_battle_daily(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_battle_daily msg;
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
	for (int i = 0; i < msg.id_size(); ++i)
	{
		PlayerOperation::add_daily_num(player, msg.id(i), msg.num(i), true);
	}
	HallMessage::send_smsg_success(player, SMSG_BATTLE_DAILY);
	return 0;
}

int PlayerManager::terminal_daily(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_daily msg;
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
	s_t_daily *t_daily = sPlayerConfig->get_daily(msg.id());
	if (!t_daily)
	{
		ERROR_SYS;
		return -1;
	}
	
	int num = PlayerOperation::get_daily_num(player, t_daily->id);
	if (num < t_daily->count)
	{
		ERROR_SYS;
		return -1;
	}
	PlayerOperation::del_daily_num(player, t_daily->id);
	player->add_daily_reward(t_daily->id);
	s_t_rewards rewards;
	rewards.add_reward(t_daily->rewards);
	PlayerOperation::player_add_reward(player, rewards);
	HallMessage::send_smsg_success(player, SMSG_DAILY);
	return 0;
}

int PlayerManager::terminal_daily_reward(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_daily_reward msg;
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
	s_t_daily_reward *t_daily_reward = sPlayerConfig->get_daily_reward(msg.id());
	if (!t_daily_reward)
	{
		ERROR_SYS;
		return -1;
	}
	if (player->daily_point() < t_daily_reward->point)
	{
		ERROR_SYS;
		return -1;
	}
	for (int i = 0; i < player->daily_get_id_size(); ++i)
	{
		if (player->daily_get_id(i) == t_daily_reward->id)
		{
			ERROR_SYS;
			return -1;
		}
	}
	s_t_rewards rewards;
	rewards.add_reward(t_daily_reward->reward);
	PlayerOperation::player_add_reward(player, rewards);
	player->add_daily_get_id(t_daily_reward->id);
	HallMessage::send_smsg_success(player, SMSG_DAILY_REWARD);
	return 0;
}

int PlayerManager::terminal_level_reward(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_level_reward msg;
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
	s_t_exp *t_exp = sPlayerConfig->get_exp(msg.level());
	if (!t_exp)
	{
		ERROR_SYS;
		return -1;
	}
	if (player->level() < t_exp->level)
	{
		ERROR_SYS;
		return -1;
	}
	for (int i = 0; i < player->level_reward_size(); ++i)
	{
		if (player->level_reward(i) == t_exp->level)
		{
			ERROR_SYS;
			return -1;
		}
	}
	s_t_rewards rewards;
	rewards.add_reward(t_exp->rewards);
	PlayerOperation::player_add_reward(player, rewards);
	player->add_level_reward(t_exp->level);
	HallMessage::send_smsg_level_reward(player, rewards);
	return 0;
}

int PlayerManager::terminal_vip_reward(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_vip_reward msg;
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
	int type = msg.type();
	if (type < 0 || type > 2)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_rewards rewards;
	if (type == 0)
	{
		if (player->first_recharge() != 1)
		{
			ERROR_SYS;
			return -1;
		}
		s_t_itembox *t_itembox = sItemConfig->get_itembox(1001);
		if (!t_itembox)
		{
			ERROR_SYS;
			return -1;
		}
		rewards.add_reward(t_itembox->rewards);
		PlayerOperation::player_add_reward(player, rewards);
		player->set_first_recharge(2);
	}
	else if (type == 1)
	{
		if (service::timer()->now() > player->yue_time())
		{
			ERROR_SYS;
			return -1;
		}
		if (player->yue_reward() > 0)
		{
			ERROR_SYS;
			return -1;
		}
		s_t_vip_attr *t_vip_attr = sPlayerConfig->get_vip_attr(1);
		if (!t_vip_attr)
		{
			return -1;
		}
		s_t_itembox *t_itembox = sItemConfig->get_itembox(t_vip_attr->day_id);
		if (!t_itembox)
		{
			ERROR_SYS;
			return -1;
		}
		rewards.add_reward(t_itembox->rewards);
		PlayerOperation::player_add_reward(player, rewards);
		player->set_yue_reward(1);
	}
	else if (type == 2)
	{
		if (service::timer()->now() > player->nian_time())
		{
			ERROR_SYS;
			return -1;
		}
		if (player->nian_reward() > 0)
		{
			ERROR_SYS;
			return -1;
		}
		s_t_vip_attr *t_vip_attr = sPlayerConfig->get_vip_attr(2);
		if (!t_vip_attr)
		{
			return -1;
		}
		s_t_itembox *t_itembox = sItemConfig->get_itembox(t_vip_attr->day_id);
		if (!t_itembox)
		{
			ERROR_SYS;
			return -1;
		}
		rewards.add_reward(t_itembox->rewards);
		PlayerOperation::player_add_reward(player, rewards);
		player->set_nian_reward(1);
	}
	HallMessage::send_smsg_vip_reward(player, rewards);
	return 0;
}

int PlayerManager::terminal_advertisement(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	if (player->advertisement_num() >= 5)
	{
		ERROR_SYS;
		return -1;
	}
	if (player->advertisement_time() + 300000 > service::timer()->now())
	{
		ERROR_SYS;
		return -1;
	}
	PlayerOperation::player_add_resource(player, resource::JEWEL, 20);
	player->set_advertisement_num(player->advertisement_num() + 1);
	player->set_advertisement_time(service::timer()->now());
	HallMessage::send_smsg_success(player, SMSG_ADVERTISEMENT);
	return 0;
}
