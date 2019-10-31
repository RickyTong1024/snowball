#include "room_master_message.h"
#include "rpc_client.h"
#include "rpc.pb.h"

void RoomMasterMessage::send_rep_rc_rm_create_room(const std::string &name, int id, int error_code, const std::string &udp_ip, int udp_port, const std::string &tcp_ip, int tcp_port)
{
	protocol::game::rep_rc_rm_create_room msg;
	msg.set_udp_ip(udp_ip);
	msg.set_udp_port(udp_port);
	msg.set_tcp_ip(tcp_ip);
	msg.set_tcp_port(tcp_port);
	Packet *pck = Packet::New((uint16_t)REQ_RC_RM_CREATE_ROOM, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, error_code);
}

void RoomMasterMessage::send_push_default_room(Packet *pck, RpcClient *rc)
{
	ACE_Message_Block *mb = pck->release();

	rpcproto::rpc rpc;
	rpc.set_type(rpcproto::PUSH);
	rpcproto::push *ph = rpc.mutable_ph();
	ph->set_name(service::get_name());
	ph->set_msg(mb->rd_ptr(), mb->length());
	mb->release();

	std::string s;
	rpc.SerializeToString(&s);
	rc->putq(s);
}

void RoomMasterMessage::send_push_default_room_center(Packet *pck)
{
	Packet *pck1 = Packet::New(pck->release());
	service::rpc_service()->push("room_center", pck1);
}

void RoomMasterMessage::send_push_rm_rc_battle_end(uint64_t battle_guid)
{
	protocol::game::push_rm_rc_battle_end msg;
	msg.set_battle_guid(battle_guid);
	msg.set_result("");
	Packet *pck = Packet::New((uint16_t)PUSH_RM_RC_BATTLE_END, 0, 0, &msg);
	service::rpc_service()->push("room_center", pck);
}
