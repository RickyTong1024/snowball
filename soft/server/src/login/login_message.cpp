#include "login_message.h"

void LoginMessage::send_smsg_error(const std::string &name, int hid, operror_t error)
{
	protocol::game::smsg_error msg;
	msg.set_code((int)error);
	Packet *pck = Packet::New((uint16_t)SMSG_ERROR, hid, 0, &msg);
	service::rpc_service()->push(name, pck);
}

void LoginMessage::send_req_login(const std::string &uid, const std::string &token, const std::string &pt, ResponseFunc func)
{
	protocol::pipe::pmsg_req_login msg;
	msg.set_uid(uid);
	msg.set_token(token);
	msg.set_pt(pt);
	Packet *pck = Packet::New((uint16_t)REQ_LOGIN, 0, 0, &msg);
	service::rpc_service()->request("login_pipe1", pck, func);
}

void LoginMessage::send_smsg_login(const std::string &name, int hid, dhc::acc_t *acc)
{
	protocol::game::smsg_login msg;
	msg.mutable_acc()->CopyFrom(*acc);
	Packet *pck = Packet::New((uint16_t)SMSG_LOGIN, hid, acc->guid(), &msg);
	service::rpc_service()->push(name, pck);
}
