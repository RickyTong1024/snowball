#include "rank_operation.h"
#include "hall_message.h"
#include "rank_config.h"
#include "hall_pool.h"

std::map<int, dhc::rank_t *> rank_list_;

void RankOperation::rank_update_login(dhc::player_t *player)
{
	for (int i = rt_cup; i < rt_end; ++i)
	{
		rank_update(player, (e_rank_type)i);
	}
}

void RankOperation::rank_update(dhc::player_t *player, e_rank_type type)
{
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return;
	}
	if (ti->acc->rank_forbid() > 0)
	{
		return;
	}
	int value = 0;
	int cvalue = 0;
	int min_value = sRankConfig->get_rank_minvalue(type);
	if (type == rt_cup)
	{
		cvalue = player->level();
		value = player->cup();
	}
	else if (type == rt_achieve)
	{
		cvalue = player->achieve_point();
		value = cvalue;
	}
	else if (type == rt_score)
	{
		cvalue = player->max_score();
		value = cvalue;
	}
	else if (type == rt_sha)
	{
		cvalue = player->max_sha();
		value = cvalue;
	}
	else if (type == rt_lsha)
	{
		cvalue = player->max_lsha();
		value = cvalue;
	}
	if (cvalue < min_value)
	{
		return;
	}
	HallMessage::send_push_hall_rank_update(player, type, value);
}

dhc::rank_t * RankOperation::get_rank(int id)
{
	if (rank_list_.find(id) == rank_list_.end())
	{
		return 0;
	}
	return rank_list_[id];
}

void RankOperation::set_rank(int id, const dhc::rank_t &rank)
{
	if (rank_list_.find(id) != rank_list_.end())
	{
		delete rank_list_[id];
	}
	dhc::rank_t *nrank = new dhc::rank_t();
	nrank->CopyFrom(rank);
	rank_list_[id] = nrank;
}

void RankOperation::clear_rank()
{
	for (std::map<int, dhc::rank_t *>::iterator it = rank_list_.begin(); it != rank_list_.end(); ++it)
	{
		delete (*it).second;
	}
	rank_list_.clear();
}
