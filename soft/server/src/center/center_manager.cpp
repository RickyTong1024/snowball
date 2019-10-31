#include "center_manager.h"
#include "center_message.h"
#include "utils.h"

CenterManager::CenterManager()
{
	
}

CenterManager::~CenterManager()
{
	
}

int CenterManager::init()
{
	return 0;
}

int CenterManager::fini()
{
	return 0;
}

int CenterManager::terminal_push_hall_center_chat_horn(Packet *pck, const std::string & name)
{
	protocol::game::smsg_chat msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	for (int i = 0; i < names.size(); ++i)
	{
		CenterMessage::send_push_hall_center_chat_horn(names[i], &msg);
	}
	return 0;
}

int CenterManager::terminal_push_pipe_center_gonggao(Packet *pck, const std::string &name)
{
	protocol::pipe::pmsg_gonggao msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	for (int i = 0; i < names.size(); ++i)
	{
		CenterMessage::send_push_pipe_center_gonggao(names[i], msg.gonggao());
	}
	return 0;
}

int CenterManager::terminal_push_pipe_center_recharge_ali(Packet *pck, const std::string &name)
{
	protocol::pipe::pmsg_recharge_ali msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	int index = msg.guid() % names.size();
	CenterMessage::send_push_pipe_center_recharge_ali(names[index], &msg);
	return 0;
}

int CenterManager::req_hall_center_player_look(Packet *pck, const std::string &name, int id)
{
	protocol::game::cmsg_player_look msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	int index = msg.target_guid() % names.size();
 	CenterMessage::send_req_center_hall_player_look(&msg, names[index], boost::bind(&CenterManager::req_hall_center_player_look_callback, this, _1, _2, name, id));
	return 0;
}

void CenterManager::req_hall_center_player_look_callback(Packet *pck, int error_code, const std::string &name, int id)
{
	protocol::game::smsg_player_look msg;
	if (!pck->parse_protocol(msg))
	{
		return;
	}
	CenterMessage::send_rep_hall_center_player_look(name, id, error_code, &msg);
}

int CenterManager::req_center_libao_pipe(Packet *pck, const std::string &name, int id)
{
	Packet *pck1 = Packet::New(pck->release());
	std::vector<std::string> names;
	service::server_env()->get_server_names("libao_pipe", names);
	int index = Utils::get_int32(0, names.size() - 1);
	service::rpc_service()->request(names[index], pck1, boost::bind(&CenterManager::req_center_pipe_callback, this, _1, _2, name, id));
	return 0;
}

int CenterManager::req_center_recharge_pipe(Packet *pck, const std::string &name, int id)
{
	Packet *pck1 = Packet::New(pck->release());
	std::vector<std::string> names;
	service::server_env()->get_server_names("recharge_pipe", names);
	int index = Utils::get_int32(0, names.size() - 1);
	service::rpc_service()->request(names[index], pck1, boost::bind(&CenterManager::req_center_pipe_callback, this, _1, _2, name, id));
	return 0;
}

void CenterManager::req_center_pipe_callback(Packet *pck, int error_code, const std::string &name, int id)
{
	Packet *pck1 = Packet::New(pck->release());
	service::rpc_service()->response(name, id, pck1, error_code, "");
}

int CenterManager::terminal_push_pipe_center_recharge_simulation(Packet *pck, const std::string &name)
{
	protocol::pipe::pmsg_recharge_simulation1 msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	int index = msg.guid() % names.size();
	CenterMessage::send_push_pipe_center_recharge_simulation(names[index], &msg);
	return 0;
}

int CenterManager::terminal_push_pipe_center_rank_forbidden(Packet *pck, const std::string &name)
{
	protocol::pipe::pmsg_rank_forbidden1 msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	int index = msg.guid() % names.size();
	CenterMessage::send_push_pipe_center_rank_forbidden(names[index], &msg);
	return 0;
}
