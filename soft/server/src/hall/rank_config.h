#ifndef __RANK_CONFIG_H__
#define __RANK_CONFIG_H__

#include "service_inc.h"
#include "protocol_inc.h"

class RankConfig
{
public:
	int parse();

	int get_rank_minvalue(int id);

private:
	std::map<int, int> t_rank_minvalue_;
};

#define sRankConfig (Singleton<RankConfig>::instance ())

#endif
