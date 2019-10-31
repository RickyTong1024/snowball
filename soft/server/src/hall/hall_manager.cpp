#include "hall_manager.h"
#include "hall_message.h"
#include "hall_dhc.h"
#include "player_loader.h"
#include "player_operation.h"
#include "item_operation.h"
#include "item_config.h"
#include "player_config.h"
#include "role_config.h"
#include "rank_operation.h"

#define HALL_TIME 1000

HallManager::HallManager()
: timer_id_(-1)
{
	
}

HallManager::~HallManager()
{
	
}

int HallManager::init()
{
	service::scheme()->init();
	if (-1 == sHallDhc->init())
	{
		return -1;
	}
	player_mgr_ = new PlayerManager();
	if (-1 == player_mgr_->init())
	{
		return -1;
	}
	item_mgr_ = new ItemManager();
	if (-1 == item_mgr_->init())
	{
		return -1;
	}
	role_mgr_ = new RoleManager();
	if (-1 == role_mgr_->init())
	{
		return -1;
	}
	chat_mgr_ = new ChatManager();
	if (-1 == chat_mgr_->init())
	{
		return -1;
	}
	post_mgr_ = new PostManager();
	if (-1 == post_mgr_->init())
	{
		return -1;
	}
	social_mgr_ = new SocialManager();
	if (-1 == social_mgr_->init())
	{
		return -1;
	}
	rank_mgr_ = new RankManager();
	if (-1 == rank_mgr_->init())
	{
		return -1;
	}
	timer_id_ = service::timer()->schedule(boost::bind(&HallManager::update, this, _1), HALL_TIME, "hall");
	if (-1 == timer_id_)
	{
		return -1;
	}
	return 0;
}

int HallManager::fini()
{
	if (-1 != timer_id_)
	{
		service::timer()->cancel(timer_id_);
		timer_id_ = -1;
	}
	rank_mgr_->fini();
	delete rank_mgr_;
	social_mgr_->fini();
	delete social_mgr_;
	post_mgr_->fini();
	delete post_mgr_;
	chat_mgr_->fini();
	delete chat_mgr_;
	role_mgr_->fini();
	delete role_mgr_;
	item_mgr_->fini();
	delete item_mgr_;
	player_mgr_->fini();
	delete player_mgr_;
	sHallDhc->fini();
	service::scheme()->fini();
	return 0;
}

int HallManager::terminal_single_battle(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (!ti)
	{
		return -1;
	}
	if (ti->battle_state)
	{
		HallMessage::send_smsg_has_battle(player_guid);
		return 0;
	}
	HallMessage::send_req_hall_rc_single_battle(player, boost::bind(&HallManager::single_battle_callback, this, _1, _2, player_guid));
	return 0;
}

void HallManager::single_battle_callback(Packet *pck, int error_code, uint64_t player_guid)
{
	protocol::game::rep_hall_rc_single_battle msg;
	if (!pck->parse_protocol(msg))
	{
		return;
	}
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return;
	}
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, (operror_t)error_code);
		return;
	}
	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (ti)
	{
		ti->set_battle_state(1, msg.udp_ip(), msg.udp_port(), msg.tcp_ip(), msg.tcp_port(), msg.code());
		HallMessage::send_push_social_fight(player_guid, 1);
	}
	HallMessage::send_smsg_single_battle(player_guid, msg.udp_ip(), msg.udp_port(), msg.tcp_ip(), msg.tcp_port(), msg.code(), msg.is_new());
}

int HallManager::push_rc_hall_battle_end(Packet *pck, const std::string &name)
{
	protocol::game::push_rc_hall_battle_end msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	dhc::battle_result_t result;
	bool is_re = true;
	if (msg.result() == "")
	{
		is_re = false;
	}
	else
	{
		if (!result.ParseFromString(msg.result()))
		{
			is_re = false;
		}
	}
	for (int i = 0; i < msg.guids_size(); ++i)
	{
		dhc::player_t *player = POOL_GET(msg.guids(i), dhc::player_t);
		if (!player)
		{
			continue;
		}
		int type = msg.type();
		uint64_t battle_his_guid = DB_GTOOL->assign(et_battle_his);
		dhc::battle_his_t *battle_his = new dhc::battle_his_t;
		int gold = 0;
		int exps = 0;
		int bindex = 0;
		if (is_re)
		{
			int index = -1;
			for (int j = 0; j < result.player_guids_size(); ++j)
			{
				if (result.player_guids(j) == player->guid())
				{
					index = j;
					break;
				}
			}
			if (index != -1)
			{
				int rank = index + 1;
				int score = 0;
				if (index < result.scores_size())
				{
					score = result.scores(index);
				}
				if (score > player->max_score())
				{
					player->set_max_score(score);
					RankOperation::rank_update(player, rt_score);
				}
				int trank = index / 5 + 1;
				battle_his->set_guid(battle_his_guid);
				battle_his->set_player_guid(player->guid());
				battle_his->set_battle_guid(result.guid());
				battle_his->set_type(type);
				if (type == 0)
				{
					battle_his->set_rank(rank);
				}
				else
				{
					battle_his->set_rank(trank);
				}
				battle_his->set_score(score);
				if (index < result.role_ids_size())
				{
					battle_his->set_role_id(result.role_ids(index));
				}
				else
				{
					battle_his->set_role_id(0);
				}
				if (index < result.shas_size())
				{
					battle_his->set_sha(result.shas(index));
				}
				else
				{
					battle_his->set_sha(0);
				}
				if (battle_his->sha() > player->max_sha())
				{
					player->set_max_sha(battle_his->sha());
					RankOperation::rank_update(player, rt_sha);
				}
				if (index < result.lshas_size())
				{
					battle_his->set_lsha(result.lshas(index));
				}
				else
				{
					battle_his->set_lsha(0);
				}
				if (battle_his->lsha() > player->max_lsha())
				{
					player->set_max_lsha(battle_his->lsha());
					RankOperation::rank_update(player, rt_lsha);
				}
				if (index < result.dies_size())
				{
					battle_his->set_die(result.dies(index));
				}
				else
				{
					battle_his->set_die(0);
				}
				if (index < result.achieves_size())
				{
					battle_his->set_achieve(result.achieves(index));
				}
				else
				{
					battle_his->set_achieve(0);
				}

				s_t_cup *t_cup = sPlayerConfig->get_cup(player->cup());
				if (t_cup)
				{
					int old_cup = player->cup();
					int cup = player->cup();
					if (type == 0)
					{
						if (rank <= t_cup->sb)
						{
							cup = cup + 1;
						}
						else if (t_cup->jb > 0 && rank > t_cup->jb)
						{
							cup = cup - 1;
						}
					}
					else
					{
						if (trank <= t_cup->tsb && score >= t_cup->tsbnum)
						{
							cup = cup + 1;
						}
						else if (t_cup->tjb > 0 && trank > t_cup->tjb)
						{
							cup = cup - 1;
						}
					}
					if (old_cup < cup)
					{
						PlayerOperation::add_all_type_num(player, 37, 1);
					}
					PlayerOperation::player_set_cup(player, cup);
					RankOperation::rank_update(player, rt_cup);
				}
				int max_gold = PlayerOperation::get_out_attr(player, 8);
				gold = 31 - rank + PlayerOperation::get_out_attr(player, 7);
				exps = 31 - rank;
				if (player->box_zd_num() <= 2)
				{
					gold = gold * 2;
					exps = exps * 2;
				}
				if (player->battle_gold() + gold > max_gold)
				{
					gold = max_gold - player->battle_gold();
				}
				if (gold < 0)
				{
					gold = 0;
				}
				PlayerOperation::player_add_resource(player, resource::GOLD, gold);
				PlayerOperation::player_add_resource(player, resource::EXP, exps);
				player->set_battle_gold(player->battle_gold() + gold);
				bindex = index + 1;
			}
		}
		else
		{
			battle_his = new dhc::battle_his_t;
			battle_his->set_guid(battle_his_guid);
			battle_his->set_battle_guid(0);
			battle_his->set_player_guid(player->guid());
			battle_his->set_type(type);
			bindex = 10;
		}
		int id = PlayerOperation::player_add_random_box(player, bindex);
		player->set_battle_num(player->battle_num() + 1);
		if (player->box_zd_opened() == 0 && player->box_zd_num() < 3)
		{
			player->set_box_zd_num(player->box_zd_num() + 1);
		}
		if (type == 0)
		{
			PlayerOperation::add_all_type_num(player, 30, 1);
		}
		else
		{
			PlayerOperation::add_all_type_num(player, 130, 1);
		}
		battle_his->set_time(service::timer()->now());
		service::pool()->add(battle_his_guid, battle_his, mmg::Pool::state_new);
		player->add_battle_his_guids(battle_his->guid());

		if (player->battle_his_guids_size() > 10)
		{
			battle_his_guid = player->battle_his_guids(0);
			service::pool()->remove(battle_his_guid, player->guid());
			for (int j = 0; j < player->battle_his_guids_size() - 1; ++j)
			{
				player->set_battle_his_guids(j, player->battle_his_guids(j + 1));
			}
			player->mutable_battle_his_guids()->RemoveLast();
		}
		HallMessage::send_smsg_battle_end(player->guid(), id, battle_his, gold, exps, player->cup());
		TermInfo *ti = sHallPool->get_terminfo(player->guid());
		if (ti)
		{
			ti->set_battle_state(0, "", 0, "", 0, "");
			HallMessage::send_push_social_fight(player->guid(), 0);
		}
	}
	return 0;
}

int HallManager::terminal_offline_battle(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_offline_battle msg;
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
	HallMessage::send_push_social_fight(player->guid(), msg.fight());
	return 0;
}

int HallManager::terminal_offline_battle_end(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_offline_battle_end msg;
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
	int rank = msg.rank();
	if (rank < 1 || rank > 20)
	{
		ERROR_SYS;
		return -1;
	}
	if (player->offline_battle_time() + 360000 > service::timer()->now())
	{
		ERROR_SYS;
		return -1;
	}
	player->set_offline_battle_time(service::timer()->now());
	int id = PlayerOperation::player_add_random_box(player, rank);
	int max_gold = PlayerOperation::get_out_attr(player, 8);
	int gold = 31 - rank + PlayerOperation::get_out_attr(player, 7);
	int exps = 0;
	if (player->level() < 3)
	{
		s_t_cup *t_cup = sPlayerConfig->get_cup(player->cup());
		if (t_cup)
		{
			int old_cup = player->cup();
			int cup = player->cup();
			if (rank <= t_cup->sb)
			{
				cup = cup + 1;
			}
			else if (t_cup->jb > 0 && rank > t_cup->jb)
			{
				cup = cup - 1;
			}
			if (old_cup < cup)
			{
				PlayerOperation::add_all_type_num(player, 37, 1);
			}
			PlayerOperation::player_set_cup(player, cup);
		}
		exps = 31 - rank;
		if (player->box_zd_num() <= 2)
		{
			gold = gold * 2;
			exps = exps * 2;
		}
		player->set_box_zd_num(player->box_zd_num() + 1);
	}
	if (player->battle_gold() + gold > max_gold)
	{
		gold = max_gold - player->battle_gold();
	}
	if (gold < 0)
	{
		gold = 0;
	}
	PlayerOperation::player_add_resource(player, resource::EXP, exps);
	PlayerOperation::player_add_resource(player, resource::GOLD, gold);
	player->set_battle_gold(player->battle_gold() + gold);

	PlayerOperation::add_all_type_num(player, 230, 1);
	HallMessage::send_smsg_offline_battle_end(player->guid(), id, gold, exps, player->cup());
	return 0;
}

int HallManager::terminal_libao(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_libao msg;
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

	HallMessage::send_req_hall_center_libao(msg.code(), "0", boost::bind(&HallManager::libao, this, _1, _2, player_guid, msg.code()));
	return 0;
}

void HallManager::libao(Packet *pck, int error_code, uint64_t player_guid, const std::string &code)
{
	protocol::pipe::pmsg_rep_libao msg;
	if (!pck->parse_protocol(msg))
	{
		return;
	}
	if (msg.res() == -1)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_LIBAO_NO);
		return;
	}
	else if (msg.res() == -2)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_LIBAO_PT);
		return;
	}
	else if (msg.res() == -3)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_LIBAO_USED);
		return;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return;
	}

	int pc = msg.pc();
	if (pc != 0)
	{
		for (int i = 0; i < player->libao_nums_size(); ++i)
		{
			if (player->libao_nums(i) == pc)
			{
				HallMessage::send_smsg_error(player_guid, ERROR_LIBAO_NUM);
				return;
			}
		}
	}
	if (msg.cf() == 0)
	{
		HallMessage::send_req_hall_center_libao(code, "1", boost::bind(&HallManager::libao1, this, _1, _2, player_guid));
		return;
	}

	s_t_rewards rds;
	for (int i = 0; i < msg.types_size(); ++i)
	{
		rds.add_reward(msg.types(i), msg.value1(i), msg.value2(i), msg.value3(i));
	}
	PlayerOperation::player_add_reward(player, rds);

	if (pc != 0)
	{
		player->add_libao_nums(pc);
	}

	HallMessage::send_smsg_libao(player, rds);
}

void HallManager::libao1(Packet *pck, int error_code, uint64_t player_guid)
{
	protocol::pipe::pmsg_rep_libao msg;
	if (!pck->parse_protocol(msg))
	{
		return;
	}
	if (msg.res() == -1)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_LIBAO_NO);
		return;
	}
	else if (msg.res() == -2)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_LIBAO_PT);
		return;
	}
	else if (msg.res() == -3)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_LIBAO_USED);
		return;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return;
	}

	int pc = msg.pc();
	if (pc != 0)
	{
		for (int i = 0; i < player->libao_nums_size(); ++i)
		{
			if (player->libao_nums(i) == pc)
			{
				HallMessage::send_smsg_error(player_guid, ERROR_LIBAO_NUM);
				return;
			}
		}
	}

	s_t_rewards rds;
	for (int i = 0; i < msg.types_size(); ++i)
	{
		rds.add_reward(msg.types(i), msg.value1(i), msg.value2(i), msg.value3(i));
	}
	PlayerOperation::player_add_reward(player, rds);

	if (pc != 0)
	{
		player->add_libao_nums(pc);
	}

	HallMessage::send_smsg_libao(player, rds);
}

int HallManager::terminal_recharge(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	int test = boost::lexical_cast<int>(service::server_env()->get_game_value("test"));
	if (test == 1)
	{
		protocol::game::cmsg_recharge msg;
		if (!pck->parse_protocol(msg))
		{
			return -1;
		}
		recharge(player_guid, msg.id(), 0);
		return 0;
	}
	else
	{
		protocol::game::cmsg_recharge msg;
		if (!pck->parse_protocol(msg))
		{
			return -1;
		}
		std::string pt = msg.pt();
		std::vector<std::string> code;
		for (int i = 0; i < msg.code().size(); ++i)
		{
			code.push_back(msg.code()[i]);
		}
		HallMessage::send_req_hall_center_reharge(code, pt, boost::bind(&HallManager::recharge_apple, this, _1, _2, player_guid, msg.id()));
	}
	return 0;
}

void HallManager::recharge(uint64_t player_guid, int rid, int count)
{
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return;
	}

	s_t_rewards rewards;
	int res = PlayerOperation::player_recharge(player, rid, count, rewards);
	if (res == -1)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_RECHARGE);
		return;
	}

	HallMessage::send_smsg_recharge(player, rid, rewards);
}

void HallManager::recharge_apple(Packet *pck, int error_code, uint64_t player_guid, int rid)
{
	protocol::pipe::pmsg_rep_recharge msg;
	if (!pck->parse_protocol(msg))
	{
		HallMessage::send_smsg_error(player_guid, ERROR_RECHARGE);
		return;
	}

	if (msg.res() != 0)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_RECHARGE);
		return;
	}

	s_t_recharge *t_recharge = sPlayerConfig->get_recharge(rid);
	if (!t_recharge)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_RECHARGE);
		return;
	}
	if (t_recharge->ios_id != msg.product_id())
	{
		HallMessage::send_smsg_error(player_guid, ERROR_RECHARGE);
		return;
	}

	dhc::recharge_t *rh = new dhc::recharge_t;
	rh->set_orderno(msg.orderid());
	rh->set_rid(rid);
	rh->set_player_guid(player_guid);
	Request *req = new Request();
	req->add(opc_insert, MAKE_GUID(et_reharge, 0), rh);
	DB_PLAYER(player_guid)->upcall(req, boost::bind(&HallManager::recharge_dhc_callback, this, _1, player_guid, rid, 0));
}

void HallManager::recharge_dhc_callback(Request *req, uint64_t player_guid, int rid, int count)
{
	if (req->result() < 0)
	{
		HallMessage::send_smsg_error(player_guid, ERROR_RECHARGE);
		return;
	}

	recharge(player_guid, rid, count);
}

int HallManager::push_pipe_center_recharge_ali(Packet *pck, const std::string &name)
{
	protocol::pipe::pmsg_recharge_ali msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	int rid = msg.rid();
	uint64_t player_guid = msg.guid();
	s_t_recharge *t_recharge = sPlayerConfig->get_recharge(rid);
	if (!t_recharge)
	{
		return -1;
	}

	dhc::recharge_t *rh = new dhc::recharge_t;
	rh->set_orderno(msg.orderno());
	rh->set_rid(rid);
	rh->set_player_guid(player_guid);
	Request *req = new Request();
	req->add(opc_insert, MAKE_GUID(et_reharge, 0), rh);
	DB_PLAYER(player_guid)->upcall(req, boost::bind(&HallManager::recharge_dhc_callback, this, _1, player_guid, rid, msg.amount()));
	return 0;
}

int HallManager::terminal_checkdata(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	HallMessage::send_smsg_checkdata(player);
	return 0;
}

int HallManager::terminal_guide(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	if (player->is_guide() == 0)
	{
		ERROR_SYS;
		return -1;
	}
	player->set_is_guide(0);
	HallMessage::send_smsg_guide(player);
	return 0;
}


int HallManager::push_pipe_center_recharge_simulation(Packet *pck, const std::string &name)
{
	protocol::pipe::pmsg_recharge_simulation1 msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	int rid = msg.rid();
	uint64_t player_guid = msg.guid();
	s_t_recharge *t_recharge = sPlayerConfig->get_recharge(rid);
	if (!t_recharge)
	{
		return -1;
	}
	recharge(player_guid, rid, 0);
	return 0;
}

int HallManager::update(ACE_Time_Value tv)
{
	sHallPool->update();
	return 0;
}
