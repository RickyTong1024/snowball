#include "center_message.h"

void CenterMessage::send_req_center_hall_player_look(protocol::game::cmsg_player_look *msg, const std::string &name, ResponseFunc func)
{
	Packet *pck = Packet::New((uint16_t)REQ_CENTER_HALL_PLAYER_LOOK, 0, 0, msg);
	service::rpc_service()->request(name, pck, func);
}

void CenterMessage::send_rep_hall_center_player_look(const std::string &name, int id, int error_code, protocol::game::smsg_player_look *msg)
{
	Packet *pck = Packet::New((uint16_t)REQ_HALL_CENTER_PLAYER_LOOK, 0, 0, msg);
	service::rpc_service()->response(name, id, pck, error_code);
}

void CenterMessage::send_push_hall_center_chat_horn(const std::string &name, protocol::game::smsg_chat *msg)
{
	Packet *pck = Packet::New((uint16_t)PUSH_HALL_CENTER_CHAT_HORN, 0, 0, msg);
	service::rpc_service()->push(name, pck);
}

void CenterMessage::send_push_pipe_center_gonggao(const std::string &name, const std::string &gonggao)
{
	protocol::game::smsg_gonggao msg;
	msg.set_text(gonggao);
	Packet *pck = Packet::New((uint16_t)PUSH_PIPE_CENTER_GONGGAO, 0, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void CenterMessage::send_push_pipe_center_recharge_ali(const std::string &name, protocol::pipe::pmsg_recharge_ali *msg)
{
	Packet *pck = Packet::New((uint16_t)PUSH_PIPE_CENTER_RECHARGE_ALI, 0, 0, msg);
	service::rpc_service()->push(name, pck);
}

void CenterMessage::send_push_pipe_center_recharge_simulation(const std::string &name, protocol::pipe::pmsg_recharge_simulation1 *msg)
{
	Packet *pck = Packet::New((uint16_t)PUSH_PIPE_CENTER_RECHARGE_SIMULATION, 0, 0, msg);
	service::rpc_service()->push(name, pck);
}

void CenterMessage::send_push_pipe_center_rank_forbidden(const std::string &name, protocol::pipe::pmsg_rank_forbidden1 *msg)
{
	Packet *pck = Packet::New((uint16_t)PUSH_PIPE_CENTER_RANK_FORBIDDEN, 0, 0, msg);
	service::rpc_service()->push(name, pck);
}
