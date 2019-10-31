#ifndef __GATE_MANAGER_H__
#define __GATE_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct acc_info
{
	dhc::acc_t *acc;
	int code;
	bool is_offline;
	uint64_t offline_time;
};

struct hid_info
{
	int hid;
	uint64_t guid;
	uint64_t time;
};

class GateManager : public mmg::DispathService
{
public:
	GateManager();

	~GateManager();

	BEGIN_PACKET_MAP
		PACKET_HANDLER(CMSG_ENTER_WORLD, terminal_enter_world)
		PACKET_HANDLER(CMSG_LEAVE_WORLD, terminal_leave_world)
		PACKET_HANDLER(CMSG_LOGIN, terminal_login)
		PACKET_HANDLER(CMSG_LOGIN_PLAYER, terminal_login_player)
		PACKET_HANDLER(CMSG_RELOGIN_PLAYER, terminal_relogin_player)
		PACKET_HANDLER(CMSG_BEAT, terminal_beat)
		PACKET_DEFAULT_HANDLE(terminal_default)
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(SMSG_LOGIN, push_login)
		PUSH_DEFAULT_HANDLE(push_default)
	END_PUSH_MAP

	BEGIN_REQ_MAP
	END_REQ_MAP

	int init();

	int fini();

private:
	int terminal_enter_world(Packet *pck);

	int terminal_leave_world(Packet *pck);

	int terminal_login(Packet *pck);

	int terminal_login_player(Packet *pck);

	int terminal_relogin_player(Packet *pck);

	int terminal_beat(Packet *pck);

	int terminal_default(Packet *pck);

	int push_login(Packet *pck, const std::string &name);

	int push_default(Packet *pck, const std::string &name);

	//////////////////////////////////////////////////////////////////////////

	int update(const ACE_Time_Value & cur_time);

private:
	int num_;
	std::map<int, hid_info> hids_;
	std::map<uint64_t, int> guid_hids_;
	std::list<int> hid_list_;
	std::map<uint64_t, acc_info *> accs_;
	std::list<uint64_t> acc_list_;
	int timer_id_;
};

#endif
