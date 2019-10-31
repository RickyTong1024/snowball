#ifndef __RANK_MANAGER_H__
#define __RANK_MANAGER_H__

#include "service_inc.h"

class RankManager
{
public:
	RankManager();

	~RankManager();

	int init();

	int fini();

	int terminal_rank(Packet *pck, const std::string &name);

	int push_rank_hall_cache(Packet *pck, const std::string &name);

	int push_pipe_rank_forbidden(Packet *pck, const std::string &name);
};

#endif
