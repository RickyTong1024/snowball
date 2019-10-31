#ifndef __ROOM_CENTER_MANAGER_H__
#define __ROOM_CENTER_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct RoomPlayer
{
	int id;
	std::string code;
	protocol::game::msg_battle_player_info * player;

	RoomPlayer()
	{
		player = new protocol::game::msg_battle_player_info();
	}

	~RoomPlayer()
	{
		delete player;
	}
};

struct RoomTeam
{
	int team_id;
	int id;
	std::vector<std::string> codes;
	std::vector<protocol::game::msg_battle_player_info *> players;

	~RoomTeam()
	{
		for (int i = 0; i < players.size(); ++i)
		{
			delete players[i];
		}
	}
};

struct RoomInfo
{
	uint64_t battle_guid;
	std::string master_name;
	std::string udp_ip;
	int udp_port;
	std::string tcp_ip;
	int tcp_port;
	uint64_t start_time;
	bool is_createing;
	int room_type;
	std::map<uint64_t, RoomPlayer *> room_players;
	std::map<uint64_t, RoomTeam *> room_teams;
	std::vector<int> room_kong;

	~RoomInfo()
	{
		for (std::map<uint64_t, RoomPlayer *>::iterator it = room_players.begin(); it != room_players.end(); ++it)
		{
			delete (*it).second;
		}
		for (std::map<uint64_t, RoomTeam *>::iterator it = room_teams.begin(); it != room_teams.end(); ++it)
		{
			delete (*it).second;
		}
	}
};

class RoomCenterManager : public mmg::DispathService
{
public:
	RoomCenterManager();

	~RoomCenterManager();

	BEGIN_PACKET_MAP
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(PUSH_RM_RC_BATTLE_END, push_rm_rc_battle_end)
	END_PUSH_MAP

	BEGIN_REQ_MAP
		REQ_HANDLER(REQ_HALL_RC_HAS_BATTLE, req_hall_rc_has_battle)
		REQ_HANDLER(REQ_HALL_RC_SINGLE_BATTLE, req_hall_rc_single_battle)
		REQ_HANDLER(REQ_TEAM_RC_MULTI_BATTLE, req_team_rc_multi_battle)
	END_REQ_MAP

	int init();

	int fini();

private:
	int req_hall_rc_has_battle(Packet *pck, const std::string &name, int id);

	int req_hall_rc_single_battle(Packet *pck, const std::string &name, int id);

	int req_team_rc_multi_battle(Packet *pck, const std::string &name, int id);

	void create_room_callback(Packet *pck, int error_code, uint64_t battle_guid);

	int push_rm_rc_battle_end(Packet *pck, const std::string &name);

	std::string get_player_guid_name(uint64_t player_guid);

	int update(const ACE_Time_Value & cur_time);

private:
	std::map<std::string, std::set<int> > master_room_map_;
	std::map<uint64_t, RoomInfo *> room_info_map_;
	std::map<uint64_t, uint64_t> player_room_map_;
	std::set<uint64_t> kx_rooms_;
	std::map<int, std::set<uint64_t> > mkx_rooms_;
	int timer_id_;
	std::vector<std::string> hall_names_;
	int max_room_player_;
	int max_team_num_;
	int max_team_member_;
};

#endif
