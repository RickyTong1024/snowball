#ifndef __ROOM_MASTER_MANAGER_H__
#define __ROOM_MASTER_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

class ACE_Process;
class RpcClient;

struct Room
{
	uint64_t battle_guid;
	int battle_type;
	std::string name;
	std::string ip;
	int port;
	std::string udp_ip;
	int udp_port;
	std::string tcp_ip;
	int tcp_port;
	RpcClient *rc;
	pid_t pid;
	uint64_t start_time;
	std::string rep_name;
	int rep_id;
	bool is_create;
};

class RoomMasterManager : public mmg::DispathService
{
public:
	RoomMasterManager();

	~RoomMasterManager();

	BEGIN_PACKET_MAP
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(PUSH_RC_RM_SET_PLAYER_ROOM, push_rc_rm_set_player_room)
		PUSH_HANDLER(PUSH_RM_RC_BATTLE_END, push_rm_rc_battle_end)
		PUSH_HANDLER(PUSH_R_RM_CREATED, push_r_rm_created)
	END_PUSH_MAP

	BEGIN_REQ_MAP
		REQ_HANDLER(REQ_RC_RM_CREATE_ROOM, req_rc_rm_create_room)
	END_REQ_MAP

	int init();

	int fini();

	int update(const ACE_Time_Value & cur_time);

private:
	int push_rc_rm_set_player_room(Packet *pck, const std::string &name);

	int req_rc_rm_create_room(Packet *pck, const std::string &name, int id);

	int push_rm_rc_battle_end(Packet *pck, const std::string &name);

	int push_r_rm_created(Packet *pck, const std::string &name);

	int push_default(Packet *pck, const std::string &name);

private:
	std::map<uint64_t, Room> room_map_;
	std::list<int> unused_ports_;
	std::list<pid_t> killed_pids_;
	int timer_id_;
};

#endif
