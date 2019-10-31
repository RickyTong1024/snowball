#include "role_operation.h"
#include "role_config.h"
#include "hall_dhc.h"
#include "player_operation.h"
#include "player_config.h"
#include "chat_pool.h"

dhc::role_t * RoleOperation::role_create(dhc::player_t *player, int id)
{
	s_t_role *t_role = sRoleConfig->get_role(id);
	if (!t_role)
	{
		return 0;
	}
	
	if (RoleOperation::has_role(player, id))
	{
		return 0;
	}

	uint64_t role_guid = DB_GTOOL->assign(et_role);
	dhc::role_t *role = new dhc::role_t;
	role->set_guid(role_guid);
	role->set_player_guid(player->guid());
	role->set_template_id(id);
	role->set_level(t_role->font_color);
	service::pool()->add(role_guid, role, mmg::Pool::state_new);
	player->add_role_guid(role->guid());
	PlayerOperation::calc_out_attr(player);
	int tid = sPlayerConfig->get_role_avatar(id);
	if (tid)
	{
		PlayerOperation::player_add_avatar(player, tid);
	}
	if (t_role->font_color >= 2)
	{
		std::string text = service::scheme()->get_server_str("sysinfo_aquire", player->name().c_str(), PlayerOperation::get_color_name(t_role->font_color, t_role->name).c_str());
		sChatPool->sys_info(text);
	}
	return role;
}

void RoleOperation::role_delete(dhc::player_t *player, uint64_t role_guid)
{
	dhc::role_t *role = POOL_GET(role_guid, dhc::role_t);
	if (!role)
	{
		return;
	}
	service::pool()->remove(role_guid, player->guid());
	for (int i = 0; i < player->role_guid_size(); ++i)
	{
		if (player->role_guid(i) == role_guid)
		{
			player->mutable_role_guid()->SwapElements(i, player->role_guid_size() - 1);
			player->mutable_role_guid()->RemoveLast();
			break;
		}
	}
	PlayerOperation::calc_out_attr(player);
}

bool RoleOperation::has_role(dhc::player_t *player, int id)
{
	for (int i = 0; i < player->role_guid_size(); ++i)
	{
		dhc::role_t *role = POOL_GET(player->role_guid(i), dhc::role_t);
		if (!role)
		{
			continue;
		}
		if (role->template_id() == id)
		{
			return true;
		}
	}
	return false;
}
