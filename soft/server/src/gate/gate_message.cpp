#include "gate_message.h"
#include "utils.h"

void GateMessage::send_smsg_error(int hid, operror_t error)
{
	protocol::game::smsg_error msg;
	msg.set_code((int)error);
	TPacket *pck = TPacket::New((uint16_t)SMSG_ERROR, &msg);
	service::tcp_service()->send_msg(hid, pck);
}

void GateMessage::send_push_gate_player_num(int num)
{
	std::string host = service::server_env()->get_server_value(service::get_name(), "tcp_host");
	std::string port = service::server_env()->get_server_value(service::get_name(), "tcp_port");
	int porti = boost::lexical_cast<int>(port);

	protocol::game::push_gate_connect_player_num msg;
	msg.set_ip(host);
	msg.set_port(porti);
	msg.set_num(num);
	Packet *pck = Packet::New((uint16_t)PUSH_GATE_CONNECT_PLAYER_NUM, 0, 0, &msg);
	service::rpc_service()->push("connect", pck);
}

void GateMessage::send_push_default(Packet *pck)
{
	Packet *pck1 = Packet::New(pck->release());
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	int index = pck->guid() % names.size();
	service::rpc_service()->push(names[index], pck1);
}

void GateMessage::send_push_default_login(Packet *pck)
{
	Packet *pck1 = Packet::New(pck->release());
	std::vector<std::string> names;
	service::server_env()->get_server_names("login", names);
	int index = Utils::get_int32(0, names.size() - 1);
	service::rpc_service()->push(names[index], pck1);
}

void GateMessage::send_terminal_default(Packet *pck, int hid)
{
	TPacket *pck1 = TPacket::New(pck->opcode(), pck->body(), pck->size(), pck->compress());
	service::tcp_service()->send_msg(hid, pck1);
}
