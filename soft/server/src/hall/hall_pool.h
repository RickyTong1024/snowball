#ifndef __HALL_POOL_H__
#define __HALL_POOL_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct TermInfo
{
	uint64_t guid;
	dhc::acc_t *acc;
	std::string gate_name;
	int gate_hid;
	int battle_state;
	std::string battle_udp_ip;
	int battle_udp_port;
	std::string battle_tcp_ip;
	int battle_tcp_port;
	std::string battle_code;

	TermInfo() : guid(0), acc(0), gate_name(""), gate_hid(0), battle_state(0), battle_udp_ip(""), battle_udp_port(0), battle_tcp_ip(""), battle_tcp_port(0), battle_code("") {}


	void set_battle_state(int state, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port, const std::string &code)
	{
		battle_state = state;
		battle_udp_ip = udp_ip;
		battle_udp_port = udp_port;
		battle_tcp_ip = tcp_ip;
		battle_tcp_port = tcp_port;
		battle_code = code;
	}
};

struct UpdatePlayer
{
	uint64_t guid;
	uint64_t update_time;
};

class HallPool
{
public:
	HallPool();

	~HallPool();

	void add_terminfo(uint64_t guid, TermInfo &ti);

	TermInfo * get_terminfo(uint64_t guid);
	
	void del_terminfo(uint64_t guid);

	void add_player_time(uint64_t guid);

	void del_player_time(uint64_t guid);

	void add_player(dhc::player_t *player, mmg::Pool::estatus es);

	void update();

	void player_tick(dhc::player_t *player);

	void get_random_players(uint64_t player_guid, int num, std::set<uint64_t> &guids);

private:
	uint64_t last_time_;
	std::map<uint64_t, TermInfo> term_info_map_;
	std::map<uint64_t, uint64_t> player_time_map_;
	std::list<UpdatePlayer> update_list_;
	std::map<uint64_t, int> random_player_index_;
	std::vector<uint64_t> random_player_vec_;
};

#define sHallPool (Singleton<HallPool>::instance())

#endif
