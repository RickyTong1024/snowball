#ifndef __NAME_MANAGER_H__
#define __NAME_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct TidPlayer
{
	int num;
	std::vector<protocol::game::msg_name_player *> players;
};

class NameManager : public mmg::DispathService
{
public:
	NameManager();

	~NameManager();

	BEGIN_PACKET_MAP
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(PUSH_HALL_NAME_INSERT, push_hall_name_insert)
	END_PUSH_MAP

	BEGIN_REQ_MAP
		REQ_HANDLER(REQ_SOCIAL_NAME_SEARCH, req_social_name_search)
	END_REQ_MAP

	int init();

	int fini();

private:
	int push_hall_name_insert(Packet *pck, const std::string &name);

	int req_social_name_search(Packet *pck, const std::string &name, int id);

	void load_name_list_callback(Request *req, const std::string &name, int id);

	void load_name_player_callback(Request *req, int tid, const std::string &name, int id);

private:
	int tid_;
	std::map<int, TidPlayer> tid_map_;
};

#endif
