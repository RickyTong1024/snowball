#ifndef __ROOM_CONFIG_H__
#define __ROOM_CONFIG_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct s_t_mode
{
	int id;
	int team_member;
	int team_number;
};

class RoomConfig
{
public:
	int parse();

	s_t_mode * get_mode(int id);

private:
	std::map<int, s_t_mode> t_modes_;
};

#define sRoomConfig (Singleton<RoomConfig>::instance ())

#endif
