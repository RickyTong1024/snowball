#ifndef __RANK_OPERATION_H__
#define __RANK_OPERATION_H__

#include "service_inc.h"
#include "protocol_inc.h"

enum e_rank_type
{
	rt_cup		= 0,
	rt_achieve	= 1,
	rt_score	= 2,
	rt_sha		= 3,
	rt_lsha		= 4,

	rt_end,
};

class RankOperation
{
public:
	static void rank_update_login(dhc::player_t *player);

	static void rank_update(dhc::player_t *player, e_rank_type type);

	static dhc::rank_t * get_rank(int id);

	static void set_rank(int id, const dhc::rank_t &rank);

	static void clear_rank();
};

#endif
