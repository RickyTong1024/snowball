#ifndef __RANK_MESSAGE_H__
#define __RANK_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class RankMessage
{
public:
	static void push_rank_hall_cache(std::map<int, dhc::rank_t *> ranks);
};

#endif
