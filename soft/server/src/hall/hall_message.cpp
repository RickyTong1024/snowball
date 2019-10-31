#include "hall_message.h"
#include "hall_pool.h"
#include <boost/algorithm/string.hpp>
#include "item_config.h"
#include "role_config.h"
#include "player_config.h"

void HallMessage::send_smsg_error(int hid, const std::string &name, operror_t error)
{
	protocol::game::smsg_error msg;
	msg.set_code((int)error);
	Packet *pck = Packet::New((uint16_t)SMSG_ERROR, hid, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void HallMessage::send_smsg_error(uint64_t guid, operror_t error)
{
	send_smsg_error(guid, error, "");
}

void HallMessage::send_smsg_error(uint64_t guid, operror_t error, const std::string & text)
{
	TermInfo *ti = sHallPool->get_terminfo(guid);
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_error msg;
	msg.set_code((int)error);
	msg.set_text(text);
	Packet *pck = Packet::New((uint16_t)SMSG_ERROR, ti->gate_hid, guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_success(dhc::player_t *player, uint16_t opcode)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	Packet *pck = Packet::New(opcode, ti->gate_hid, player->guid(), 0);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_login_player(dhc::player_t *player)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_login_player msg;
	msg.mutable_player()->CopyFrom(*player);
	for (int i = 0; i < player->role_guid_size(); ++i)
	{
		dhc::role_t *role = POOL_GET(player->role_guid(i), dhc::role_t);
		if (role)
		{
			msg.add_roles()->CopyFrom(*role);
		}
	}
	for (int i = 0; i < player->battle_his_guids_size(); ++i)
	{
		dhc::battle_his_t *battle_his = POOL_GET(player->battle_his_guids(i), dhc::battle_his_t);
		if (battle_his)
		{
			msg.add_battle_his()->CopyFrom(*battle_his);
		}
	}
	int num = 0;
	for (int i = 0; i < player->post_guids_size(); ++i)
	{
		uint64_t post_guid = player->post_guids(i);
		dhc::post_t *post = POOL_GET(post_guid, dhc::post_t);
		if (post && (!post->is_pick() || !post->is_read()))
		{
			num++;
		}
	}
	msg.set_post_num(num);
	msg.set_server_time(service::timer()->now());
	Packet *pck = Packet::New((uint16_t)SMSG_LOGIN_PLAYER, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_req_hall_rc_has_battle(dhc::player_t *player, ResponseFunc func)
{
	protocol::game::req_hall_rc_has_battle msg;
	msg.set_guid(player->guid());
	Packet *pck = Packet::New((uint16_t)REQ_HALL_RC_HAS_BATTLE, 0, 0, &msg);
	service::rpc_service()->request("room_center", pck, func);
}

void HallMessage::send_smsg_has_battle(uint64_t guid)
{
	TermInfo *ti = sHallPool->get_terminfo(guid);
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_has_battle msg;
	msg.set_udp_ip(ti->battle_udp_ip);
	msg.set_udp_port(ti->battle_udp_port);
	msg.set_tcp_ip(ti->battle_tcp_ip);
	msg.set_tcp_port(ti->battle_tcp_port);
	msg.set_code(ti->battle_code);
	Packet *pck = Packet::New((uint16_t)SMSG_HAS_BATTLE, ti->gate_hid, guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_req_hall_rc_single_battle(dhc::player_t *player, ResponseFunc func)
{
	protocol::game::req_hall_rc_single_battle msg;
	protocol::game::msg_battle_player_info *bpi = msg.mutable_player();
	HallMessage::create_battle_player_info(player, bpi);
	Packet *pck = Packet::New((uint16_t)REQ_HALL_RC_SINGLE_BATTLE, 0, 0, &msg);
	service::rpc_service()->request("room_center", pck, func);
}

void HallMessage::send_smsg_single_battle(uint64_t guid, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::string &code, int is_new)
{
	TermInfo *ti = sHallPool->get_terminfo(guid);
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_single_battle msg;
	msg.set_udp_ip(udp_ip);
	msg.set_udp_port(udp_port);
	msg.set_tcp_ip(tcp_ip);
	msg.set_tcp_port(tcp_port);
	msg.set_code(code);
	msg.set_is_new(is_new);
	Packet *pck = Packet::New((uint16_t)SMSG_SINGLE_BATTLE, ti->gate_hid, guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_battle_end(uint64_t guid, int box_id, dhc::battle_his_t *battle_his, int gold, int exps, int cup)
{
	TermInfo *ti = sHallPool->get_terminfo(guid);
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_battle_end msg;
	msg.set_box_id(box_id);
	if (battle_his)
	{
		msg.mutable_battle_his()->CopyFrom(*battle_his);
	}
	msg.set_gold(gold);
	msg.set_exp(exps);
	msg.set_cup(cup);
	Packet *pck = Packet::New((uint16_t)SMSG_BATTLE_END, ti->gate_hid, guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_offline_battle_end(uint64_t guid, int box_id, int gold, int exps, int cup)
{
	TermInfo *ti = sHallPool->get_terminfo(guid);
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_offline_battle_end msg;
	msg.set_box_id(box_id);
	msg.set_gold(gold);
	msg.set_exp(exps);
	msg.set_cup(cup);
	Packet *pck = Packet::New((uint16_t)SMSG_OFFLINE_BATTLE_END, ti->gate_hid, guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_player_look(uint64_t player_guid, dhc::player_t *target)
{
	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (!ti)
	{
		return;
	}

	protocol::game::smsg_player_look msg;
	msg.mutable_player()->CopyFrom(*target);
	for (int i = 0; i < target->battle_his_guids_size(); ++i)
	{
		uint64_t battle_his_guid = target->battle_his_guids(i);
		dhc::battle_his_t *battle = POOL_GET(battle_his_guid, dhc::battle_his_t);
		if (battle)
		{
			msg.add_battle_his()->CopyFrom(*battle);
		}
	}
	Packet *pck = Packet::New((uint16_t)SMSG_PLAYER_LOOK, ti->gate_hid, player_guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_player_look(uint64_t player_guid, protocol::game::smsg_player_look *msg)
{
	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (!ti)
	{
		return;
	}
	Packet *pck = Packet::New((uint16_t)SMSG_PLAYER_LOOK, ti->gate_hid, player_guid, msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_req_hall_center_player_look(uint64_t target_guid, ResponseFunc func)
{
	protocol::game::cmsg_player_look msg;
	msg.set_target_guid(target_guid);
	Packet *pck = Packet::New((uint16_t)REQ_HALL_CENTER_PLAYER_LOOK, 0, 0, &msg);
	service::rpc_service()->request("center", pck, func);
}

void HallMessage::send_rep_center_hall_player_look(int id, int error_code, dhc::player_t *target)
{
	protocol::game::smsg_player_look msg;
	msg.mutable_player()->CopyFrom(*target);
	for (int i = 0; i < target->battle_his_guids_size(); ++i)
	{
		uint64_t battle_his_guid = target->battle_his_guids(i);
		dhc::battle_his_t *battle = POOL_GET(battle_his_guid, dhc::battle_his_t);
		if (battle)
		{
			msg.add_battle_his()->CopyFrom(*battle);
		}
	}
	Packet *pck = Packet::New((uint16_t)REQ_CENTER_HALL_PLAYER_LOOK, 0, 0, &msg);
	service::rpc_service()->response("center", id, pck, error_code);
}

void HallMessage::send_smsg_chat(uint64_t player_guid, protocol::game::smsg_chat *msg)
{
	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (!ti)
	{
		return;
	}

	Packet *pck = Packet::New((uint16_t)SMSG_CHAT, ti->gate_hid, player_guid, msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_center_chat_horn(dhc::player_t *player, const std::string &text)
{
	protocol::game::smsg_chat msg;
	msg.set_player_guid(player->guid());
	msg.set_player_name(player->name());
	msg.set_sex(player->sex());
	msg.set_level(player->level());
	msg.set_avatar(player->avatar_on());
	msg.set_toukuang(player->toukuang_on());
	msg.set_region_id(player->region_id());
	msg.set_name_color(PlayerOperation::player_get_name_color(player));
	msg.set_type(1);
	msg.set_text(text);
	msg.set_time(service::timer()->now());

	Packet *pck = Packet::New((uint16_t)PUSH_HALL_CENTER_CHAT_HORN, 0, 0, &msg);
	service::rpc_service()->push("center", pck);
}

void HallMessage::send_smsg_gonggao(uint64_t player_guid, protocol::game::smsg_gonggao *msg)
{
	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (!ti)
	{
		return;
	}
	Packet *pck = Packet::New((uint16_t)SMSG_GONGGAO, ti->gate_hid, player_guid, msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_sys_info(dhc::player_t *player, const std::string &text)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_sys_info msg;
	msg.set_text(text);
	Packet *pck = Packet::New((uint16_t)SMSG_SYS_INFO, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_gm_command(dhc::player_t *player, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_gm_command msg;
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}

	Packet *pck = Packet::New((uint16_t)SMSG_GM_COMMAND, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_end_open_box(dhc::player_t *player, uint16_t op, int id, int jewel, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_end_open_box msg;
	msg.set_id(id);
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}
	msg.set_jewel(jewel);

	Packet *pck = Packet::New(op, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_item_buy(dhc::player_t *player, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_item_buy msg;
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}

	Packet *pck = Packet::New(SMSG_ITEM_BUY, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_chat_laba(dhc::player_t *player)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	Packet *pck = Packet::New((uint16_t)SMSG_CHAT_LABA, ti->gate_hid, player->guid(), 0);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_role_hecheng(dhc::player_t *player, dhc::role_t *role)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_role_hecheng msg;
	msg.mutable_role()->CopyFrom(*role);
	Packet *pck = Packet::New((uint64_t)SMSG_ROLE_HECHENG, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_post_look(dhc::player_t *player)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_post_look msg;
	for (int i = 0; i < player->post_guids_size(); ++i)
	{
		dhc::post_t *post = POOL_GET(player->post_guids(i), dhc::post_t);
		if (!post)
		{
			continue;
		}
		msg.add_posts()->CopyFrom(*post);
	}
	Packet *pck = Packet::New((uint64_t)SMSG_POST_LOOK, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_post_get(dhc::player_t *player, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_post_get msg;
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}
	Packet *pck = Packet::New((uint64_t)SMSG_POST_GET, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_post_get_all(dhc::player_t *player, const std::vector<uint64_t> post_guids, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_post_get_all msg;
	for (int i = 0; i < post_guids.size(); ++i)
	{
		msg.add_post_guids(post_guids[i]);
	}
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}
	Packet *pck = Packet::New((uint64_t)SMSG_POST_GET_ALL, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_post_delete_all(dhc::player_t *player, const std::vector<uint64_t> post_guids)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_post_delete_all msg;
	for (int i = 0; i < post_guids.size(); ++i)
	{
		msg.add_post_guids(post_guids[i]);
	}
	Packet *pck = Packet::New((uint64_t)SMSG_POST_DELETE_ALL, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_fenxiang_num(dhc::player_t *player)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_fenxiang_num msg;
	msg.set_fenxiang_num(player->fenxiang_num());
	msg.set_fenxiang_total_num(player->fenxiang_total_num());
	Packet *pck = Packet::New((uint64_t)SMSG_FENXIANG_NUM, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_req_hall_center_libao(const std::string &code, const std::string &use, ResponseFunc func)
{
	protocol::pipe::pmsg_req_libao msg;
	msg.set_code(code);
	msg.set_use(use);
	Packet *pck = Packet::New((uint16_t)REQ_HALL_CENTER_LIBAO, 0, 0, &msg);
	service::rpc_service()->request("center", pck, func);
}

void HallMessage::send_smsg_libao(dhc::player_t *player, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_libao msg;
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}
	Packet *pck = Packet::New((uint64_t)SMSG_LIBAO, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_req_hall_center_reharge(const std::vector<std::string> &code, const std::string &pt, ResponseFunc func)
{
	protocol::pipe::pmsg_req_recharge msg;
	msg.set_pt(pt);
	for (int i = 0; i < code.size(); ++i)
	{
		msg.add_code(code[i]);
	}
	Packet *pck = Packet::New((uint16_t)REQ_HALL_CENTER_RECHARGE, 0, 0, &msg);
	service::rpc_service()->request("center", pck, func);
}

void HallMessage::send_smsg_checkdata(dhc::player_t *player)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_checkdata msg;
	msg.mutable_player()->CopyFrom(*player);
	for (int i = 0; i < player->role_guid_size(); ++i)
	{
		dhc::role_t *role = POOL_GET(player->role_guid(i), dhc::role_t);
		if (role)
		{
			msg.add_roles()->CopyFrom(*role);
		}
	}
	Packet *pck = Packet::New((uint16_t)SMSG_CHECKDATA, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_item_apply(dhc::player_t *player, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_item_apply msg;
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}

	Packet *pck = Packet::New((uint16_t)SMSG_ITEM_APPLY, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_post_num(dhc::player_t *player)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	int num = 0;
	for (int i = 0; i < player->post_guids_size(); ++i)
	{
		uint64_t post_guid = player->post_guids(i);
		dhc::post_t *post = POOL_GET(post_guid, dhc::post_t);
		if (post && (!post->is_pick() || !post->is_read()))
		{
			num++;
		}
	}
	protocol::game::smsg_post_num msg;
	msg.set_post_num(num);

	Packet *pck = Packet::New((uint16_t)SMSG_POST_NUM, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_team_create(dhc::player_t *player)
{
	protocol::game::push_hall_team_create msg;
	protocol::game::msg_battle_player_info *bpi = msg.mutable_player();
	HallMessage::create_battle_player_info(player, bpi);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_TEAM_CREATE, 0, 0, &msg);
	service::rpc_service()->push("team", pck);
}

void HallMessage::send_smsg_team_create(dhc::player_t *player, const protocol::game::msg_team &team)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_team_create msg;
	msg.mutable_team()->CopyFrom(team);
	Packet *pck = Packet::New((uint16_t)SMSG_TEAM_CREATE, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_team_tuijian(dhc::player_t *player, std::set<uint64_t> &guids)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_team_tuijian msg;
	for (std::set<uint64_t>::iterator it = guids.begin(); it != guids.end(); ++it)
	{
		dhc::player_t *player1 = POOL_GET(*it, dhc::player_t);
		if (!player1)
		{
			continue;
		}
		protocol::game::msg_team_player *player2 = msg.add_players();
		player2->set_guid(player1->guid());
		player2->set_name(player1->name());
		player2->set_sex(player1->sex());
		player2->set_avatar(player1->avatar_on());
		player2->set_toukuang(player1->toukuang_on());
		player2->set_cup(player1->cup());
		player2->set_name_color(PlayerOperation::player_get_name_color(player1));
	}
	Packet *pck = Packet::New((uint16_t)SMSG_TEAM_TUIJIAN, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_team_join(dhc::player_t *player, uint64_t player_guid)
{
	protocol::game::push_hall_team_join msg;
	msg.set_player_guid(player_guid);
	protocol::game::msg_battle_player_info *bpi = msg.mutable_player();
	HallMessage::create_battle_player_info(player, bpi);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_TEAM_JOIN, 0, 0, &msg);
	service::rpc_service()->push("team", pck);
}

void HallMessage::send_smsg_team_join(dhc::player_t *player, const protocol::game::msg_team &team)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_team_join msg;
	msg.mutable_team()->CopyFrom(team);
	Packet *pck = Packet::New((uint16_t)SMSG_TEAM_JOIN, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_team_other_join(dhc::player_t *player, const protocol::game::msg_team_member &team_member)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_team_other_join msg;
	msg.mutable_member()->CopyFrom(team_member);
	Packet *pck = Packet::New((uint16_t)SMSG_TEAM_OTHER_JOIN, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_team_exit(dhc::player_t *player)
{
	protocol::game::push_hall_team_exit msg;
	msg.set_player_guid(player->guid());
	msg.set_mauto(0);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_TEAM_EXIT, 0, 0, &msg);
	service::rpc_service()->push("team", pck);
}

void HallMessage::send_push_hall_team_exit1(dhc::player_t *player)
{
	protocol::game::push_hall_team_exit msg;
	msg.set_player_guid(player->guid());
	msg.set_mauto(1);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_TEAM_EXIT, 0, 0, &msg);
	service::rpc_service()->push("team", pck);
}

void HallMessage::send_smsg_team_exit(dhc::player_t *player, uint64_t leader_guid, uint64_t player_guid)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_team_exit msg;
	msg.set_leader_guid(leader_guid);
	msg.set_player_guid(player_guid);
	Packet *pck = Packet::New((uint16_t)SMSG_TEAM_EXIT, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_team_kick(dhc::player_t *player, uint64_t target_guid)
{
	protocol::game::push_hall_team_kick msg;
	msg.set_player_guid(player->guid());
	msg.set_target_guid(target_guid);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_TEAM_KICK, 0, 0, &msg);
	service::rpc_service()->push("team", pck);
}

void HallMessage::send_smsg_team_kick(dhc::player_t *player, uint64_t player_guid)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_team_kick msg;
	msg.set_player_guid(player_guid);
	Packet *pck = Packet::New((uint16_t)SMSG_TEAM_KICK, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_team_invert(dhc::player_t *player, uint64_t target_guid)
{
	protocol::game::push_hall_team_invert msg;
	msg.set_player_guid(player->guid());
	msg.set_target_guid(target_guid);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_TEAM_INVERT, 0, 0, &msg);
	service::rpc_service()->push("team", pck);
}

void HallMessage::send_smsg_team_invert(dhc::player_t *player, const protocol::game::msg_team_player &player1)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_team_invert msg;
	msg.mutable_player()->CopyFrom(player1);
	Packet *pck = Packet::New((uint16_t)SMSG_TEAM_INVERT, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_team_chat(dhc::player_t *player, const std::string &text)
{
	protocol::game::push_hall_team_chat msg;
	msg.set_player_guid(player->guid());
	msg.set_text(text);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_TEAM_CHAT, 0, 0, &msg);
	service::rpc_service()->push("team", pck);
}

void HallMessage::send_smsg_team_chat(dhc::player_t *player, uint64_t player_guid, const std::string &text)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_team_chat msg;
	msg.set_player_guid(player_guid);
	msg.set_text(text);
	Packet *pck = Packet::New((uint16_t)SMSG_TEAM_CHAT, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_team_multi_battle(dhc::player_t *player)
{
	protocol::game::push_hall_team_multi_battle msg;
	msg.set_player_guid(player->guid());
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_TEAM_MULTI_BATTLE, 0, 0, &msg);
	service::rpc_service()->push("team", pck);
}

void HallMessage::send_smsg_team_multi_battle(dhc::player_t *player, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::string &code, int num)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_multi_battle msg;
	msg.set_udp_ip(udp_ip);
	msg.set_udp_port(udp_port);
	msg.set_tcp_ip(tcp_ip);
	msg.set_tcp_port(tcp_port);
	msg.set_code(code);
	msg.set_num(num);
	Packet *pck = Packet::New((uint16_t)SMSG_MULTI_BATTLE, ti->gate_hid, ti->guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_guide(dhc::player_t *player)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	Packet *pck = Packet::New((uint16_t)SMSG_GUIDE, ti->gate_hid, ti->guid, 0);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_level_reward(dhc::player_t *player, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_level_reward msg;
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}

	Packet *pck = Packet::New((uint16_t)SMSG_LEVEL_REWARD, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_name_insert(uint64_t guid, const std::string &name)
{
	protocol::game::push_hall_name_insert msg;
	msg.set_guid(guid);
	msg.set_name(name);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_NAME_INSERT, 0, 0, &msg);
	service::rpc_service()->push("name", pck);
}

void HallMessage::send_req_hall_name_search(const std::string &name, ResponseFunc func)
{
	protocol::game::msg_name_list msg;
	msg.set_name(name);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_NAME_SEARCH, 0, 0, &msg);
	service::rpc_service()->request("name", pck, func);
}

void HallMessage::send_smsg_social_look(dhc::player_t *player, uint16_t opcode, const protocol::game::smsg_social_look& smsg)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	Packet *pck = Packet::New(opcode, ti->gate_hid, ti->guid, &smsg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_req_social_delete(uint64_t player_guid, uint64_t target_guid, ResponseFunc func)
{
	protocol::game::req_social_delete msg;
	msg.set_player_guid(player_guid);
	msg.set_target_guid(target_guid);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_DELETE, 0, 0, &msg);
	service::rpc_service()->request("social", pck, func);
}

void HallMessage::send_smsg_social_delete(dhc::player_t *player, uint64_t target_guid)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_social_delete msg;
	msg.set_target_guid(target_guid);
	Packet *pck = Packet::New(SMSG_SOCIAL_DELETE, ti->gate_hid, ti->guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_soical_reject(uint64_t player_guid, bool reject, ResponseFunc func)
{
	protocol::game::rmsg_social_reject msg;
	msg.set_player_guid(player_guid);
	msg.set_reject(reject);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_REJECT, 0, 0, &msg);
	service::rpc_service()->request("social", pck, func);
}

void HallMessage::send_req_social_apply(const dhc::social_t &social, ResponseFunc func)
{
	protocol::game::rmsg_social_apply msg;
	msg.mutable_social()->CopyFrom(social);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_APPLY, 0, 0, &msg);
	service::rpc_service()->request("social", pck, func);
}

void HallMessage::send_smsg_social_apply(dhc::player_t *player, int num)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_social_apply msg;
	msg.set_num(num);
	Packet *pck = Packet::New(SMSG_SOCIAL_APPLY_RECEIVE, ti->gate_hid, ti->guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_social_add(dhc::player_t *player, uint16_t opcode, const dhc::social_t &social)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_social_add msg;
	msg.mutable_social()->CopyFrom(social);
	Packet *pck = Packet::New(opcode, ti->gate_hid, ti->guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_req_social_add(const dhc::social_t& social, ResponseFunc func)
{
	protocol::game::rmsg_social_apply msg;
	msg.mutable_social()->CopyFrom(social);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_ADD, 0, 0, &msg);
	service::rpc_service()->request("social", pck, func);
}

void HallMessage::send_req_social_black(uint64_t player_guid, uint64_t target_guid, ResponseFunc func)
{
	protocol::game::rmsg_social_black msg;
	msg.set_player_guid(player_guid);
	msg.set_target_guid(target_guid);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_BLACK, 0, 0, &msg);
	service::rpc_service()->request("social", pck, func);
}

void HallMessage::send_req_social_gold(uint64_t player_guid, ResponseFunc func)
{
	protocol::game::req_social_gold msg;
	msg.set_player_guid(player_guid);
	Packet *pck = Packet::New((uint16_t)REQ_SOICAL_GOLD, 0, 0, &msg);
	service::rpc_service()->request("social", pck, func);
}

void HallMessage::send_rep_hall_social_black(int id, uint16_t error_code, const dhc::social_t &social)
{
	protocol::game::rmsg_social_apply msg;
	msg.mutable_social()->CopyFrom(social);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_BLACK, 0, 0, &msg);
	service::rpc_service()->response("social", id, pck, error_code);
}

void HallMessage::send_req_social_gift(uint64_t player_guid, uint64_t target_guid, int gold, ResponseFunc func)
{
	protocol::game::pmsg_social_gift msg;
	msg.set_player_guid(player_guid);
	msg.set_target_guid(target_guid);
	msg.set_gold(gold);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_GIFT, 0, 0, &msg);
	service::rpc_service()->request("social", pck, func);
}

void HallMessage::send_push_social_chat(uint64_t player_guid, uint64_t target_guid, const std::string &name, const std::string &text)
{
	protocol::game::pmsg_social_chat msg;
	msg.set_player_guid(player_guid);
	msg.set_guid(target_guid);
	msg.set_name(name);
	msg.set_text(text);
	Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_CHAT, 0, 0, &msg);
	service::rpc_service()->push("social", pck);
}

void HallMessage::send_smsg_social_chat(dhc::player_t *player, const protocol::game::smsg_chat &msg)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	Packet *pck = Packet::New(SMSG_SOCIAL_CHAT, ti->gate_hid, ti->guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_social_gift(dhc::player_t *player, uint64_t target_guid, int gold)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_social_gift_receive msg;
	msg.set_target_guid(target_guid);
	msg.set_gold(gold);
	Packet *pck = Packet::New(SMSG_SOCIAL_GIFT_RECEIVE, ti->gate_hid, ti->guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_soical_data(dhc::player_t *player, const protocol::game::smsg_social_data &msg)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}

	Packet *pck = Packet::New(SMSG_SOCIAL_DATA, ti->gate_hid, ti->guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_social_stat(dhc::player_t *player, uint64_t target_guid, const std::string& pname, int stat)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_social_stat msg;
	msg.set_target_guid(target_guid);
	msg.set_name(pname);
	msg.set_stat(stat);
	Packet *pck = Packet::New(SMSG_SOCIAL_STAT, ti->gate_hid, ti->guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_req_social_look(uint64_t player_guid, int type, ResponseFunc func)
{
	protocol::game::req_social_look msg;
	msg.set_player_guid(player_guid);
	msg.set_type(type);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_LOOK, 0, 0, &msg);
	service::rpc_service()->request("social", pck, func);
}

void HallMessage::send_smsg_social_gold(dhc::player_t *player, int gold)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}

	protocol::game::smsg_social_gold msg;
	msg.set_gold(gold);
	Packet *pck = Packet::New(SMSG_SOCIAL_GOLD, ti->gate_hid, ti->guid, &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::req_social_login(dhc::player_t *player, ResponseFunc func)
{
	protocol::game::req_social_login msg;
	msg.set_player_guid(player->guid());
	msg.set_name(player->name());
	msg.set_cup(player->cup());
	msg.set_avatar(player->avatar_on());
	msg.set_toukuang(player->toukuang_on());
	msg.set_level(player->level());
	msg.set_name_color(PlayerOperation::player_get_name_color(player));
	msg.set_achieve_point(player->achieve_point());
	msg.set_max_score(player->max_score());
	msg.set_max_sha(player->max_sha());
	msg.set_max_lsha(player->max_lsha());
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_LOGIN, 0, 0, &msg);
	service::rpc_service()->request("social", pck, func);
}

void HallMessage::send_push_social_logout(dhc::player_t *player)
{
	protocol::game::req_social_login msg;
	msg.set_player_guid(player->guid());
	msg.set_name(player->name());
	msg.set_cup(player->cup());
	msg.set_avatar(player->avatar_on());
	msg.set_toukuang(player->toukuang_on());
	msg.set_level(player->level());
	msg.set_name_color(PlayerOperation::player_get_name_color(player));
	msg.set_achieve_point(player->achieve_point());
	msg.set_max_score(player->max_score());
	msg.set_max_sha(player->max_sha());
	msg.set_max_lsha(player->max_lsha());
	Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_LOGOUT, 0, 0, &msg);
	service::rpc_service()->push("social", pck);
}

void HallMessage::send_smsg_recharge(dhc::player_t *player, int rid, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_recharge msg;
	msg.set_rid(rid);
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}

	Packet *pck = Packet::New((uint16_t)SMSG_RECHARGE, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_vip_reward(dhc::player_t *player, const s_t_rewards& rds)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_vip_reward msg;
	for (std::vector<dhc::role_t*>::size_type i = 0; i < rds.roles.size(); ++i)
	{
		msg.add_roles()->CopyFrom(*(rds.roles[i]));
	}
	for (std::vector<s_t_reward>::size_type i = 0; i < rds.rewards.size(); ++i)
	{
		msg.add_types(rds.rewards[i].type);
		msg.add_value1s(rds.rewards[i].value1);
		msg.add_value2s(rds.rewards[i].value2);
		msg.add_value3s(rds.rewards[i].value3);
	}

	Packet *pck = Packet::New((uint16_t)SMSG_VIP_REWARD, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_smsg_duobao(dhc::player_t *player, int id)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_duobao msg;
	msg.set_id(id);

	Packet *pck = Packet::New((uint16_t)SMSG_DUOBAO, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_social_fight(uint64_t player_guid, int fight)
{
	protocol::game::push_social_fight msg;
	msg.set_player_guid(player_guid);
	msg.set_fight(fight);
	Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_FIGHT, 0, 0, &msg);
	service::rpc_service()->push("social", pck);
}

void HallMessage::send_push_hall_rank_update(dhc::player_t *player, int id, int value)
{
	protocol::game::push_hall_rank_update msg;
	msg.set_id(id);
	msg.set_player_guid(player->guid());
	msg.set_name(player->name());
	msg.set_sex(player->sex());
	msg.set_level(player->level());
	msg.set_avatar(player->avatar_on());
	msg.set_toukuang(player->toukuang_on());
	msg.set_region_id(player->region_id());
	msg.set_name_color(PlayerOperation::player_get_name_color(player));
	msg.set_value(value);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_RANK_UPDATE, 0, 0, &msg);
	service::rpc_service()->push("rank", pck);
}

void HallMessage::send_smsg_rank(dhc::player_t *player, dhc::rank_t *rank)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	protocol::game::smsg_rank msg;
	msg.mutable_rank()->CopyFrom(*rank);

	Packet *pck = Packet::New((uint16_t)SMSG_RANK, ti->gate_hid, player->guid(), &msg);
	service::rpc_service()->push(ti->gate_name, pck);
}

void HallMessage::send_push_hall_rank_forbidden(uint64_t player_guid)
{
	protocol::game::push_hall_rank_forbidden msg;
	msg.set_guid(player_guid);
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_RANK_FORBIDDEN, 0, 0, &msg);
	service::rpc_service()->push("rank", pck);
}

//////////////////////////////////////////////////////////////////////////

void HallMessage::create_battle_player_info(dhc::player_t *player, protocol::game::msg_battle_player_info *player_info)
{
	dhc::role_t *role = POOL_GET(player->role_on(), dhc::role_t);
	if (!role)
	{
		return;
	}
	std::vector<s_t_attr> attrs;
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
			if (t_role_buff->attr.type < 3)
			{
				s_t_attr attr;
				attr.type = t_role_buff->attr.type;
				attr.param1 = t_role_buff->attr.param1;
				attr.param2 = t_role_buff->attr.param2;
				attr.param3 = t_role_buff->attr.param3 + t_role_buff->attr.param4 * (role1->level() - 1);
				attrs.push_back(attr);
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
			if (t_toukuang->attrs[j].type < 3)
			{
				s_t_attr attr;
				attr.type = t_toukuang->attrs[j].type;
				attr.param1 = t_toukuang->attrs[j].param1;
				attr.param2 = t_toukuang->attrs[j].param2;
				attr.param3 = t_toukuang->attrs[j].param3;
				attrs.push_back(attr);
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
		if (t_fashion->attr.type < 3)
		{
			s_t_attr attr;
			attr.type = t_fashion->attr.type;
			attr.param1 = t_fashion->attr.param1;
			attr.param2 = t_fashion->attr.param2;
			attr.param3 = t_fashion->attr.param3;
			attrs.push_back(attr);
		}
	}
	player_info->set_guid(player->guid());
	player_info->set_name(player->name());
	player_info->set_role_id(role->template_id());
	player_info->set_role_level(role->level());
	player_info->set_sex(player->sex());
	player_info->set_avatar(player->avatar_on());
	player_info->set_toukuang(player->toukuang_on());
	player_info->set_name_color(PlayerOperation::player_get_name_color(player));
	for (int i = 0; i < player->fashion_on_size(); ++i)
	{
		player_info->add_fashion(player->fashion_on(i));
	}
	player_info->set_region_id(player->region_id());
	player_info->set_cup(player->cup());
	for (int i = 0; i < attrs.size(); ++i)
	{
		player_info->add_attr_type(attrs[i].type);
		player_info->add_attr_param1(attrs[i].param1);
		player_info->add_attr_param2(attrs[i].param2);
		player_info->add_attr_param3(attrs[i].param3);
	}
}
