#ifndef __CONNECT_MANAGER_H__
#define __CONNECT_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct GateData
{
	std::string ip;
	int port;
	uint64_t pretime;
	int num;
};

class ConnectManager : public mmg::DispathService
{	
public:
	ConnectManager();

	~ConnectManager();

	BEGIN_PACKET_MAP
		PACKET_HANDLER(CMSG_ENTER_WORLD, terminal_enter_world)
		PACKET_HANDLER(CMSG_LEAVE_WORLD, terminal_leave_world)
		PACKET_HANDLER(CMSG_REQUEST_GATE, terminal_request_gate)
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(PUSH_GATE_CONNECT_PLAYER_NUM, push_gate_connect_player_num);
	END_PUSH_MAP

	BEGIN_REQ_MAP
	END_REQ_MAP

	int init();

	int fini();

private:
	int terminal_enter_world(Packet *pck);

	int terminal_leave_world(Packet *pck);

	int terminal_request_gate(Packet *pck);

	int push_gate_connect_player_num(Packet *pck, const std::string &name);

	//////////////////////////////////////////////////////////////////////////

	int time_clear_user();

	int time_refresh_gate_data();

	int update(const ACE_Time_Value & cur_time);

private:
	int timer_id_;
	std::map<int, uint64_t> hid_times_;
	std::map<std::string, GateData> gate_datas_;
};

#endif
