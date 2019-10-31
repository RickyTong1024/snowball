#ifndef __CENTER_MANAGER_H__
#define __CENTER_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

class CenterManager : public mmg::DispathService
{
public:
	CenterManager();

	~CenterManager();

	BEGIN_PACKET_MAP
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(PUSH_HALL_CENTER_CHAT_HORN, terminal_push_hall_center_chat_horn)
		PUSH_HANDLER(PUSH_PIPE_CENTER_GONGGAO, terminal_push_pipe_center_gonggao)
		PUSH_HANDLER(PUSH_PIPE_CENTER_RECHARGE_ALI, terminal_push_pipe_center_recharge_ali)
		PUSH_HANDLER(PUSH_PIPE_CENTER_RECHARGE_SIMULATION, terminal_push_pipe_center_recharge_simulation)
		PUSH_HANDLER(PUSH_PIPE_CENTER_RANK_FORBIDDEN, terminal_push_pipe_center_rank_forbidden)
	END_PUSH_MAP

	BEGIN_REQ_MAP
		REQ_HANDLER(REQ_HALL_CENTER_PLAYER_LOOK, req_hall_center_player_look)
		REQ_HANDLER(REQ_HALL_CENTER_LIBAO, req_center_libao_pipe)
		REQ_HANDLER(REQ_HALL_CENTER_RECHARGE, req_center_recharge_pipe)
	END_REQ_MAP

	int init();

	int fini();

private:
	int terminal_push_hall_center_chat_horn(Packet *pck, const std::string &name);

	int terminal_push_pipe_center_gonggao(Packet *pck, const std::string &name);

	int terminal_push_pipe_center_recharge_ali(Packet *pck, const std::string &name);

	int req_hall_center_player_look(Packet *pck, const std::string &name, int id);

	void req_hall_center_player_look_callback(Packet *pck, int error_code, const std::string &name, int id);

	int req_center_libao_pipe(Packet *pck, const std::string &name, int id);

	int req_center_recharge_pipe(Packet *pck, const std::string &name, int id);

	void req_center_pipe_callback(Packet *pck, int error_code, const std::string &name, int id);

	int terminal_push_pipe_center_recharge_simulation(Packet *pck, const std::string &name);

	int terminal_push_pipe_center_rank_forbidden(Packet *pck, const std::string &name);
};

#endif
