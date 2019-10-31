#include "player_loader.h"
#include "hall_dhc.h"
#include "hall_pool.h"
#include "pool.h"
#include "hall_message.h"
#include "player_operation.h"
#include "role_operation.h"
#include "player_config.h"

int PlayerLoader::load_player(uint64_t player_guid)
{
	if (load_map_.find(player_guid) != load_map_.end())
	{
		load_map_[player_guid].need_smsg = true;
		return 0;
	}
	LoadMap lm;
	lm.need_smsg = true;
	lm.query_num = 1;
	load_map_[player_guid] = lm;
	Request *req = new Request();
	req->add(opc_query, player_guid, new dhc::player_t);
	DB_PLAYER(player_guid)->upcall(req, boost::bind(&PlayerLoader::load_player_callback, this, _1));
	return 0;
}

int PlayerLoader::load_player(uint64_t player_guid, LoaderCallback callback)
{
	if (load_map_.find(player_guid) != load_map_.end())
	{
		load_map_[player_guid].callbacks.push_back(callback);
		return 0;
	}
	LoadMap lm;
	lm.need_smsg = false;
	lm.query_num = 1;
	lm.callbacks.push_back(callback);
	load_map_[player_guid] = lm;
	Request *req = new Request();
	req->add(opc_query, player_guid, new dhc::player_t);
	DB_PLAYER(player_guid)->upcall(req, boost::bind(&PlayerLoader::load_player_callback, this, _1));
	return 0;
}

void PlayerLoader::load_player_callback(Request *req)
{
	if (load_map_.find(req->guid()) == load_map_.end())
	{
		return;
	}
	LoadMap &lm = load_map_[req->guid()];
	if (req->result() >= 0)
	{
		dhc::player_t *player = (dhc::player_t *)req->release_data();
		// 加载player上的其他东西
		for (int i = 0; i < player->role_guid_size(); ++i)
		{
			if (player->role_guid(i) != 0)
			{
				lm.query_num++;
				Request *req = new Request();
				req->add(opc_query, player->role_guid(i), new dhc::role_t);
				DB_PLAYER(player->guid())->upcall(req, boost::bind(&PlayerLoader::load_msg_callback, this, _1, player));
			}
		}
		for (int i = 0; i < player->battle_his_guids_size(); ++i)
		{
			if (player->battle_his_guids(i) != 0)
			{
				lm.query_num++;
				Request *req = new Request();
				req->add(opc_query, player->battle_his_guids(i), new dhc::battle_his_t);
				DB_PLAYER(player->guid())->upcall(req, boost::bind(&PlayerLoader::load_msg_callback, this, _1, player));
			}
		}
		for (int i = 0; i < player->post_guids_size(); ++i)
		{
			if (player->post_guids(i) != 0)
			{
				lm.query_num++;
				Request *req = new Request();
				req->add(opc_query, player->post_guids(i), new dhc::post_t);
				DB_PLAYER(player->guid())->upcall(req, boost::bind(&PlayerLoader::load_msg_callback, this, _1, player));
			}
		}
		load_player_check_end(player);
	}
	else
	{
		TermInfo *ti = sHallPool->get_terminfo(req->guid());
		if (!ti)
		{
			return;
		}
		dhc::player_t *player = create_player(req->guid(), ti->acc->serverid());
		PlayerOperation::client_login(player);
		if (lm.need_smsg)
		{
			HallMessage::send_smsg_login_player(player);
		}
		load_map_.erase(req->guid());
	}
}

void PlayerLoader::load_msg_callback(Request *req, dhc::player_t *player)
{
	LoadMap &lm = load_map_[player->guid()];
	if (req->result() >= 0)
	{
		service::pool()->add(req->guid(), req->release_data(), mmg::Pool::state_none);
	}
	else
	{
		service::log()->error("load_msg_callback error, guid = %llu", req->guid());
	}
	load_player_check_end(player);
}

void PlayerLoader::load_player_check_end(dhc::player_t *player)
{
	LoadMap &lm = load_map_[player->guid()];
	lm.query_num--;
	if (lm.query_num <= 0)
	{
		sHallPool->add_player(player, mmg::Pool::state_none);
		if (!lm.need_smsg)
		{
			sHallPool->add_player_time(player->guid());
		}
		PlayerOperation::player_login(player);

		for (int i = 0; i < lm.callbacks.size(); ++i)
		{
			lm.callbacks[i]();
		}
		if (lm.need_smsg)
		{
			sHallPool->del_player_time(player->guid());
			PlayerOperation::client_login(player);
			HallMessage::send_smsg_login_player(player);
		}
		load_map_.erase(player->guid());
	}
}

dhc::player_t *PlayerLoader::create_player(uint64_t player_guid, int serverid)
{
	dhc::player_t *player = new dhc::player_t;
	player->set_guid(player_guid);
	player->set_serverid(serverid);
	int lang = boost::lexical_cast<int>(service::server_env()->get_game_value("lang"));
	if (lang == 0)
	{
		player->set_name(sPlayerConfig->get_random_name());
	}
	else
	{
		player->set_name(sPlayerConfig->get_random_en_name());
	}
	player->set_sex(0);
	player->set_level(1);
	player->set_exp(0);
	player->set_is_guide(1);
	player->add_avatar(1011001);
	player->add_avatar(1011002);
	player->set_avatar_on(1011001);
	player->add_toukuang(1);
	player->set_toukuang_on(1);
	player->set_gold(0);
	player->set_jewel(0);
	player->set_total_recharge(0);
	player->set_total_spend(0);
	player->set_last_login_time(service::timer()->now());
	player->set_birth_time(service::timer()->now());
	player->set_last_daily_time(service::timer()->now());
	player->set_last_week_time(service::timer()->now());
	player->set_last_month_time(service::timer()->now());
	dhc::role_t *role = RoleOperation::role_create(player, 1001);
	RoleOperation::role_create(player, 1002);
	player->set_role_on(role->guid());
	service::pool()->add(player_guid, player, mmg::Pool::state_new);

	PlayerOperation::player_login(player);

	sHallPool->add_player(player, mmg::Pool::state_new);
	save_player(player_guid, false, 0);
	HallMessage::send_push_hall_name_insert(player->guid(), player->name());

	return player;
}

template <typename otype>
int PlayerLoader::save(uint64_t player_guid, uint64_t guid, google::protobuf::Message *object, bool release, Upcaller caller)
{ 
	if (object->changed())
	{
		object->clear_changed();
		mmg::Pool::estatus es = service::pool()->get_state(guid);
		opcmd_t opt = opc_update;
		if (es == mmg::Pool::state_new)
		{
			opt = opc_insert;
			service::pool()->set_state(guid, mmg::Pool::state_none);
		}
		Request *req = new Request();
		otype *cmsg = new otype;
		cmsg->CopyFrom(*object);
		req->add(opt, guid, cmsg);
		DB_PLAYER(player_guid)->upcall(req, caller);
	}
	if (release)
	{
		service::pool()->release(guid);
		delete object;
	}
	return 0;
}

void PlayerLoader::save_player(uint64_t guid, bool release, Upcaller upcall)
{
	dhc::player_t *player = POOL_GET(guid, dhc::player_t);
	if (player)
	{
		// save player身上的东西
		for (int i = 0; i < player->role_guid_size(); ++i)
		{
			uint64_t role_guid = player->role_guid(i);
			if (role_guid != 0)
			{
				dhc::role_t *role = POOL_GET(role_guid, dhc::role_t);
				if (role)
				{
					save<dhc::role_t>(player->guid(), role_guid, role, release, upcall);
				}
			}
		}
		for (int i = 0; i < player->battle_his_guids_size(); ++i)
		{
			uint64_t battle_his_guid = player->battle_his_guids(i);
			if (battle_his_guid != 0)
			{
				dhc::battle_his_t *battle_his = POOL_GET(battle_his_guid, dhc::battle_his_t);
				if (battle_his)
				{
					save<dhc::battle_his_t>(player->guid(), battle_his_guid, battle_his, release, upcall);
				}
			}
		}
		for (int i = 0; i < player->post_guids_size(); ++i)
		{
			uint64_t post_guid = player->post_guids(i);
			if (post_guid != 0)
			{
				dhc::post_t *post = POOL_GET(post_guid, dhc::post_t);
				if (post)
				{
					save<dhc::post_t>(player->guid(), post_guid, post, release, upcall);
				}
			}
		}
		// 删除ref
		std::vector<uint64_t> entitys;
		service::pool()->release_ref(player->guid(), entitys);
		for (int i = 0; i < entitys.size(); ++i)
		{
			Request *req = new Request();
			req->add(opc_remove, entitys[i], 0);
			DB_PLAYER(player->guid())->upcall(req, 0);
		}
		save<dhc::player_t>(player->guid(), player->guid(), player, release, upcall);
	}
}

void PlayerLoader::save_all()
{
	std::vector<uint64_t> guids;
	service::pool()->get_entitys(et_player, guids);
	for (int i = 0; i < guids.size(); ++i)
	{
		dhc::player_t *player = POOL_GET(guids[i], dhc::player_t);
		if (player)
		{
			PlayerOperation::player_logout(player);
			save_player(player->guid(), true, 0);
		}
	}
}
