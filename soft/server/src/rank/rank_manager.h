#ifndef __RANK_MANAGER_H__
#define __RANK_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

enum e_rank_type
{
	rt_cup = 0,
	rt_achieve = 1,
	rt_score = 2,
	rt_sha = 3,
	rt_lsha = 4,

	rt_end,
};

class RankManager : public mmg::DispathService
{
public:
	RankManager();

	~RankManager();

	BEGIN_PACKET_MAP
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(PUSH_HALL_RANK_UPDATE, push_hall_rank_update)
		PUSH_HANDLER(PUSH_HALL_RANK_FORBIDDEN, push_hall_rank_forbidden)
	END_PUSH_MAP

	BEGIN_REQ_MAP
	END_REQ_MAP

	int init();

	int fini();

	void load();

	void load_callback(Request *req, int id);

	void save(int id, bool is_new = false, bool release = false);

	void save_all(bool release = false);

	int push_hall_rank_update(Packet *pck, const std::string &name);

	int push_hall_rank_forbidden(Packet *pck, const std::string &name);

	int update(ACE_Time_Value tv);

private:
	std::map<int, dhc::rank_t *> ranks_;
	int timer_id_;
};

#endif
