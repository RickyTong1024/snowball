#ifndef __CENTER_MESSAGE_H__
#define __CENTER_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class CenterMessage
{
public:
	static void send_req_center_hall_player_look(protocol::game::cmsg_player_look *msg, const std::string &name, ResponseFunc func);

	static void send_rep_hall_center_player_look(const std::string &name, int id, int error_code, protocol::game::smsg_player_look *msg);

	static void send_push_hall_center_chat_horn(const std::string &name, protocol::game::smsg_chat *msg);

	static void send_push_pipe_center_gonggao(const std::string &name, const std::string &gonggao);

	static void send_push_pipe_center_recharge_ali(const std::string &name, protocol::pipe::pmsg_recharge_ali *msg);

	static void send_push_pipe_center_recharge_simulation(const std::string &name, protocol::pipe::pmsg_recharge_simulation1 *msg);

	static void send_push_pipe_center_rank_forbidden(const std::string &name, protocol::pipe::pmsg_rank_forbidden1 *msg);
};

#endif
