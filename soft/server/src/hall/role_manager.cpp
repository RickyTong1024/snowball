#include "role_manager.h"
#include "role_config.h"
#include "role_operation.h"
#include "hall_message.h"
#include "item_operation.h"

RoleManager::RoleManager()
{

}

RoleManager::~RoleManager()
{

}

int RoleManager::init()
{
	if (-1 == sRoleConfig->parse())
	{
		return -1;
	}
	return 0;
}

int RoleManager::fini()
{
	return 0;
}

int RoleManager::terminal_role_on(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_role_on msg;
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
	dhc::role_t *role = POOL_GET(msg.role_guid(), dhc::role_t);
	if (!role)
	{
		ERROR_SYS;
		return -1;
	}
	if (role->player_guid() != player_guid)
	{
		ERROR_SYS;
		return -1;
	}
	player->set_role_on(role->guid());
	HallMessage::send_smsg_success(player, SMSG_ROLE_ON);
	return 0;
}

int RoleManager::terminal_role_hecheng(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_role_hecheng msg;
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
	int role_id = msg.role_id();
	if (RoleOperation::has_role(player, role_id))
	{
		ERROR_SYS;
		return -1;
	}
	s_t_role *t_role = sRoleConfig->get_role(role_id);
	if (!t_role)
	{
		ERROR_SYS;
		return -1;
	}
	if (t_role->gz > 1)
	{
		ERROR_SYS;
		return -1;
	}
	int num = ItemOperation::item_num_templete(player, t_role->suipian_id);
	if (num < t_role->suipian_num)
	{
		ERROR_SYS;
		return -1;
	}

	dhc::role_t *role = RoleOperation::role_create(player, role_id);
	ItemOperation::item_destory_templete(player, t_role->suipian_id, t_role->suipian_num);
	HallMessage::send_smsg_role_hecheng(player, role);
	return 0;
}

int RoleManager::terminal_role_levelup(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_role_levelup msg;
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
	dhc::role_t *role = POOL_GET(msg.role_guid(), dhc::role_t);
	if (!role)
	{
		ERROR_SYS;
		return -1;
	}
	if (role->player_guid() != player_guid)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_role *t_role = sRoleConfig->get_role(role->template_id());
	if (!t_role)
	{
		ERROR_SYS;
		return -1;
	}
	s_t_role_level *t_role_level = sRoleConfig->get_role_level(role->level() + 1);
	if (!t_role_level)
	{
		ERROR_SYS;
		return -1;
	}
	int num = ItemOperation::item_num_templete(player, t_role->suipian_id);
	if (num < t_role_level->card)
	{
		ERROR_SYS;
		return -1;
	}
	int ngold = t_role_level->gold[t_role->font_color - 1];
	if (t_role->font_color == 2)
	{
		ngold = ngold * (100 - PlayerOperation::get_out_attr(player, 3)) / 100;
	}
	else if (t_role->font_color == 3)
	{
		ngold = ngold * (100 - PlayerOperation::get_out_attr(player, 4)) / 100;
	}
	if (PlayerOperation::player_get_resource(player, resource::GOLD) < ngold)
	{
		ERROR_SYS;
		return -1;
	}
	ItemOperation::item_destory_templete(player, t_role->suipian_id, t_role_level->card);
	PlayerOperation::player_dec_resource(player, resource::GOLD, ngold);
	role->set_level(role->level() + 1);
	PlayerOperation::add_all_type_num(player, 36, 1);
	PlayerOperation::calc_out_attr(player);
	HallMessage::send_smsg_success(player, SMSG_ROLE_LEVELUP);
	return 0;
}
