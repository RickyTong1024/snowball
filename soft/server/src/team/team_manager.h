#ifndef __TEAM_MANAGER_H__
#define __TEAM_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

class TeamManager : public mmg::DispathService
{
public:
	TeamManager();

	~TeamManager();

	BEGIN_PACKET_MAP
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(PUSH_HALL_TEAM_CREATE, push_hall_team_create)
		PUSH_HANDLER(PUSH_HALL_TEAM_JOIN, push_hall_team_join)
		PUSH_HANDLER(PUSH_HALL_TEAM_EXIT, push_hall_team_exit)
		PUSH_HANDLER(PUSH_HALL_TEAM_KICK, push_hall_team_kick)
		PUSH_HANDLER(PUSH_HALL_TEAM_INVERT, push_hall_team_invert)
		PUSH_HANDLER(PUSH_HALL_TEAM_CHAT, push_hall_team_chat)
		PUSH_HANDLER(PUSH_HALL_TEAM_MULTI_BATTLE, push_hall_team_multi_battle)
	END_PUSH_MAP

	BEGIN_REQ_MAP
	END_REQ_MAP

	int init();

	int fini();

private:
	int push_hall_team_create(Packet *pck, const std::string &name);

	int push_hall_team_join(Packet *pck, const std::string &name);

	int push_hall_team_exit(Packet *pck, const std::string &name);

	int push_hall_team_kick(Packet *pck, const std::string &name);

	int push_hall_team_invert(Packet *pck, const std::string &name);

	int push_hall_team_chat(Packet *pck, const std::string &name);

	int push_hall_team_multi_battle(Packet *pck, const std::string &name);

	void req_team_rc_multi_battle_callback(Packet *pck, int error_code, int team_id);

private:
	std::map<uint64_t, int> player_team_map_;
	std::map<int, protocol::game::msg_team *> team_map_;
	int team_id_;
	int team_member_;
};

#endif
