#ifndef __ROOM_POOL_H__
#define __ROOM_POOL_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "room_def.h"

#define ROOM_TICK 50

enum e_battle_msg
{
	MSG_BATTLE_IN = 0,
	MSG_BATTLE_OUT,
	MSG_BATTLE_STATE,
	MSG_BATTLE_CODE,
};

struct RoomPlayer
{
	int hid;
	uint64_t guid;
	int no_state_num;
};

struct RoomState
{
	int zhen;
	std::string data;
};

struct tmp_state
{
	int num;
	std::string data;
};

struct tmp_code
{
	int num;
	int data;
};

struct tmp_result
{
	int num;
	std::string data;
};

class RoomPool
{
public:
	void init(const std::string &master_name, uint64_t battle_guid, int battle_type);

	void fini();

	void battle_state(uint64_t guid, const protocol::game::msg_battle_state &msg);

	void check_state(int zhen);

	void remove_state_player(uint64_t guid);

	void update_state();

	void battle_code(uint64_t guid, const protocol::game::msg_battle_code &msg);

	void check_code(int zhen);

	void remove_code_player(uint64_t guid);

	void update_code();

	void battle_end(uint64_t guid, dhc::battle_result_t &battle_result);

	void reset(uint64_t guid, WaitPlayer &player);

	void link(uint64_t guid, int hid, WaitPlayer &player);

	void add_player(uint64_t guid, int hid, WaitPlayer &player);

	void del_player(uint64_t guid);

	int get_hid(uint64_t guid);

	void get_hids(std::vector<int> &hids);

	void add_operation(protocol::game::msg_battle_op *op);

	void do_battle_end();

	void update();

private:
	int zhen_;
	int re_tid_;
	std::map<uint64_t, RoomPlayer> player_map_;
	std::map<uint64_t, int> player_ob_;
	std::vector<protocol::game::msg_battle_op *> op_vec_;
	std::list<int> up_states_list_;
	std::map<int, std::map<uint64_t, std::string> > up_states_map_;
	std::list<int> up_codes_list_;
	std::map<int, std::map<uint64_t, int> > up_codes_map_;
	RoomState rs_;
	int op_start_index_;
	std::vector<protocol::game::msg_battle_op *> op_all_;
	std::vector<int> op_index_;
	int is_end_;
	std::map<uint64_t, std::string> result_map_;
	std::string master_name_;
	uint64_t battle_guid_;
	uint64_t battle_type_;
	int team_num_;
	int member_num_;
	int seed_;
	int seed_add_;
};

#define sRoomPool (Singleton<RoomPool>::instance())

#endif
