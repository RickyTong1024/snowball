#ifndef __TEAM_MANAGER_H__
#define __TEAM_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

class TeamManager
{
public:
	TeamManager();

	~TeamManager();

	int init();

	int fini();

	int push_team_hall_error(Packet *pck, const std::string &name);

	int terminal_team_create(Packet *pck, const std::string &name);

	int terminal_team_tuijian(Packet *pck, const std::string &name);

	int push_team_hall_create(Packet *pck, const std::string &name);

	int terminal_team_join(Packet *pck, const std::string &name);

	int push_team_hall_join(Packet *pck, const std::string &name);

	int push_team_hall_other_join(Packet *pck, const std::string &name);

	int terminal_team_exit(Packet *pck, const std::string &name);

	int push_team_hall_exit(Packet *pck, const std::string &name);

	int terminal_team_kick(Packet *pck, const std::string &name);

	int push_team_hall_kick(Packet *pck, const std::string &name);

	int terminal_team_invert(Packet *pck, const std::string &name);

	int push_team_hall_invert(Packet *pck, const std::string &name);

	int terminal_team_chat(Packet *pck, const std::string &name);

	int push_team_hall_chat(Packet *pck, const std::string &name);

	int terminal_multi_battle(Packet *pck, const std::string &name);

	int push_team_hall_multi_battle(Packet *pck, const std::string &name);
};

#endif
