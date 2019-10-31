#include "hall_pool.h"
#include "hall_dhc.h"
#include "player_operation.h"
#include "role_operation.h"
#include "player_loader.h"
#include "post_operation.h"
#include "utils.h"

#define PLAYER_UPDATE_TIME 60000
#define NORMAL_OFF_TIME 3600000
#define BUSY_OFF_TIME 300000

HallPool::HallPool()
{
	last_time_ = service::timer()->now();
}

HallPool::~HallPool()
{

}

void HallPool::add_terminfo(uint64_t guid, TermInfo &ti)
{
	del_terminfo(guid);
	term_info_map_[guid] = ti;
	random_player_index_[guid] = random_player_vec_.size();
	random_player_vec_.push_back(guid);
}

TermInfo * HallPool::get_terminfo(uint64_t guid)
{
	if (term_info_map_.find(guid) != term_info_map_.end())
	{
		TermInfo &ti = term_info_map_[guid];
		return &ti;
	}
	return 0;
}

void HallPool::del_terminfo(uint64_t guid)
{
	if (term_info_map_.find(guid) != term_info_map_.end())
	{
		delete term_info_map_[guid].acc;
		term_info_map_.erase(guid);
		int index = random_player_index_[guid];
		uint64_t oguid = random_player_vec_[random_player_vec_.size() - 1];
		random_player_index_[oguid] = index;
		random_player_index_.erase(guid);
		random_player_vec_[index] = oguid;
		random_player_vec_.pop_back();
	}
}

void HallPool::add_player(dhc::player_t *player, mmg::Pool::estatus es)
{
	service::pool()->add(player->guid(), player, es);

	UpdatePlayer up;
	up.guid = player->guid();
	up.update_time = service::timer()->now();
	update_list_.push_back(up);
}

void HallPool::add_player_time(uint64_t guid)
{
	player_time_map_[guid] = service::timer()->now();
}

void HallPool::del_player_time(uint64_t guid)
{
	player_time_map_.erase(guid);
}

void HallPool::update()
{
	/// 日常刷新
	uint64_t now = service::timer()->now();
	if (service::timer()->trigger_time(last_time_, 5, 0))
	{
		std::vector<uint64_t> player_guids;
		service::pool()->get_entitys(et_player, player_guids);
		for (int i = 0; i < player_guids.size(); ++i)
		{
			dhc::player_t *player = POOL_GET(player_guids[i], dhc::player_t);
			if (player)
			{
				player->set_last_daily_time(now);
				PlayerOperation::player_refresh(player, 1);
			}
		}
	}

	if (service::timer()->trigger_week_time(last_time_))
	{
		std::vector<uint64_t> player_guids;
		service::pool()->get_entitys(et_player, player_guids);
		for (int i = 0; i < player_guids.size(); ++i)
		{
			dhc::player_t *player = POOL_GET(player_guids[i], dhc::player_t);
			if (player)
			{
				player->set_last_week_time(now);
				PlayerOperation::player_week_refresh(player);
			}
		}
	}

	if (service::timer()->trigger_month_time(last_time_))
	{
		std::vector<uint64_t> player_guids;
		service::pool()->get_entitys(et_player, player_guids);
		for (int i = 0; i < player_guids.size(); ++i)
		{
			dhc::player_t *player = POOL_GET(player_guids[i], dhc::player_t);
			if (player)
			{
				player->set_last_month_time(now);
				PlayerOperation::player_month_refresh(player);
			}
		}
	}

	last_time_ = now;

	/// 数据保存&&玩家下线
	int player_num = term_info_map_.size();
	int busy_num = boost::lexical_cast<int>(service::server_env()->get_game_value("busy"));
	int upnum = 0;
	while (upnum < 10)
	{
		if (update_list_.empty())
		{
			break;
		}
		UpdatePlayer up = update_list_.front();
		if (now - up.update_time < PLAYER_UPDATE_TIME)
		{
			break;
		}
		dhc::player_t *player = POOL_GET(up.guid, dhc::player_t);
		if (player)
		{
			upnum++;
			update_list_.pop_front();
			if (!DB_PLAYER(player->guid())->full())
			{
				if (player_time_map_.find(player->guid()) != player_time_map_.end())
				{
					uint64_t ptime = player_time_map_[player->guid()];
					if (player_num < busy_num)
					{
						if (now > ptime + NORMAL_OFF_TIME)
						{
							PlayerOperation::player_logout(player);
							sPlayerLoader->save_player(player->guid(), true, 0);
							player_time_map_.erase(up.guid);
							continue;
						}
					}
					else
					{
						if (now > ptime + BUSY_OFF_TIME)
						{
							PlayerOperation::player_logout(player);
							sPlayerLoader->save_player(player->guid(), true, 0);
							player_time_map_.erase(up.guid);
							continue;
						}
					}
				}
				sPlayerLoader->save_player(player->guid(), false, 0);
			}
			player_tick(player);
			up.update_time = now;
			update_list_.push_back(up);
		}
		else
		{
			update_list_.pop_front();
		}
	}
}

void HallPool::player_tick(dhc::player_t *player)
{
	PostOperation::get_new_post(player);
	PostOperation::check_post(player);
	PostOperation::get_share(player);
}

void HallPool::get_random_players(uint64_t player_guid, int num, std::set<uint64_t> &guids)
{
	for (int i = 0; i < num; ++i)
	{
		uint64_t guid = random_player_vec_[Utils::get_int32(0, random_player_vec_.size() - 1)];
		if (guids.find(guid) == guids.end() && guid != player_guid)
		{
			guids.insert(guid);
		}
	}
}
