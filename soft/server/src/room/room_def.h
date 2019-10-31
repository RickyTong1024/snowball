#ifndef __ROOM_DEF_H__
#define __ROOM_DEF_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct WaitPlayer
{
	uint64_t guid;
	std::string code;
	protocol::game::msg_battle_player_info player;
	int is_new;
};

#endif
