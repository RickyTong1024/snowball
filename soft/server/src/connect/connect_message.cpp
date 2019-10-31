#include "connect_message.h"
#include "connect_manager.h"

void ConnectMessage::send_smsg_error(operror_t error, int hid)
{
	protocol::game::smsg_error msg;
	msg.set_code((int)error);
	TPacket *pck = TPacket::New((uint16_t)SMSG_ERROR, &msg);
	service::tcp_service()->send_msg(hid, pck);
}

void ConnectMessage::send_smsg_request_gate(const std::string &ip, int port, int hid)
{
	 protocol::game::smsg_request_gate msg;
	 msg.set_ip(ip);
	 msg.set_port(port);
	 TPacket *pck = TPacket::New((uint16_t)SMSG_REQUEST_GATE, &msg);
	 service::tcp_service()->send_msg(hid, pck);
}
