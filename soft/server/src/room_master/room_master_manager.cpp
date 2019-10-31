#include "room_master_manager.h"
#include "room_master_message.h"
#include <ace/Process_Manager.h>
#include "rpc_client.h"

#define UPDATE_TIME 1000
#define ROOM_TIME 8 * 60000 + 15000
#define ROOM_CREATE_TIME 5000

RoomMasterManager::RoomMasterManager()
: timer_id_(-1)
{
	
}

RoomMasterManager::~RoomMasterManager()
{
	
}

int RoomMasterManager::init()
{
	timer_id_ = service::timer()->schedule(boost::bind(&RoomMasterManager::update, this, _1), UPDATE_TIME, "room_master_manager");
	if (timer_id_ == -1)
	{
		return -1;
	}
	for (int i = 10000; i < 20000; i += 2)
	{
		unused_ports_.push_back(i);
	}
	return 0;
}

int RoomMasterManager::fini()
{
	if (-1 != timer_id_)
	{
		service::timer()->cancel(timer_id_);
		timer_id_ = -1;
	}
	for (std::map<uint64_t, Room>::iterator it = room_map_.begin(); it != room_map_.end(); ++it)
	{
		Room &room = (*it).second;
		ACE_Process_Manager::instance()->terminate(room.pid);
	}
	for (std::map<uint64_t, Room>::iterator it = room_map_.begin(); it != room_map_.end(); ++it)
	{
		Room &room = (*it).second;
		ACE_Process_Manager::instance()->wait(room.pid);
		delete room.rc;
	}
	room_map_.clear();
	killed_pids_.clear();
	return 0;
}

int RoomMasterManager::push_rc_rm_set_player_room(Packet *pck, const std::string &name)
{
	protocol::game::push_rc_rm_set_player_room msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t battle_guid = msg.battle_guid();
	if (room_map_.find(battle_guid) == room_map_.end())
	{
		return -1;
	}
	Room &room = room_map_[battle_guid];
	RoomMasterMessage::send_push_default_room(pck, room.rc);
	return 0;
}

int RoomMasterManager::req_rc_rm_create_room(Packet *pck, const std::string &name, int id)
{
	protocol::game::req_rc_rm_create_room msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t battle_guid = msg.battle_guid();
	if (room_map_.find(battle_guid) != room_map_.end())
	{
		return -1;
	}
	if (unused_ports_.empty())
	{
		return -1;
	}
	Room room;
	room.battle_guid = battle_guid;
	room.battle_type = msg.battle_type();
	room.name = "room" + boost::lexical_cast<std::string>(battle_guid);
	room.ip = service::server_env()->get_server_value(service::get_name(), "host");
	room.port = unused_ports_.front();
	unused_ports_.pop_front();
	room.udp_ip = service::server_env()->get_server_value(service::get_name(), "room_host");
	room.udp_port = room.port + 10000;
	room.tcp_ip = service::server_env()->get_server_value(service::get_name(), "room_host");
	room.tcp_port = room.port + 10001;
	ACE_Process_Options options;
#ifdef _WIN32
	std::string cmd = "./room.exe";
#else
	std::string cmd = "./room";
#endif
	cmd += " " + room.name + " ../conf room " + room.ip
		+ " " + boost::lexical_cast<std::string>(room.port) + " " + room.udp_ip + " " + boost::lexical_cast<std::string>(room.udp_port)
		+ " " + room.tcp_ip + " " + boost::lexical_cast<std::string>(room.tcp_port) + " " + service::get_name() + " "
		+ boost::lexical_cast<std::string>(room.battle_guid) + " " + boost::lexical_cast<std::string>(room.battle_type);
	service::log()->debug(cmd.c_str());
	options.command_line(cmd.c_str());
	pid_t pid = ACE_Process_Manager::instance()->spawn(options);
	room.pid = pid;
	std::string addr = "tcp://" + room.ip + ":" + boost::lexical_cast<std::string>(room.port);
	room.rc = new RpcClient(room.name, addr);
	room.start_time = service::timer()->now();
	room.rep_name = name;
	room.rep_id = id;
	room.is_create = true;
	room_map_[battle_guid] = room;

	return 0;
}

int RoomMasterManager::push_rm_rc_battle_end(Packet *pck, const std::string &name)
{
	protocol::game::push_rm_rc_battle_end msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t battle_guid = msg.battle_guid();
	if (room_map_.find(battle_guid) == room_map_.end())
	{
		return -1;
	}
	Room &room = room_map_[battle_guid];
	unused_ports_.push_back(room.port);
	ACE_Process_Manager::instance()->terminate(room.pid);
	killed_pids_.push_back(room.pid);
	delete room.rc;
	room_map_.erase(battle_guid);
	if (msg.result() == "")
	{
		RoomMasterMessage::send_push_rm_rc_battle_end(battle_guid);
	}
	else
	{
		RoomMasterMessage::send_push_default_room_center(pck);
	}
	return 0;
}

int RoomMasterManager::push_r_rm_created(Packet *pck, const std::string &name)
{
	protocol::game::push_r_rm_created msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t battle_guid = msg.battle_guid();
	if (room_map_.find(battle_guid) == room_map_.end())
	{
		return -1;
	}
	Room &room = room_map_[battle_guid];
	room.is_create = false;
	RoomMasterMessage::send_rep_rc_rm_create_room(room.rep_name, room.rep_id, 0, room.udp_ip, room.udp_port, room.tcp_ip, room.tcp_port);
	return 0;
}

int RoomMasterManager::update(const ACE_Time_Value & cur_time)
{
	uint64_t now = service::timer()->now();
	for (std::map<uint64_t, Room>::iterator it = room_map_.begin(); it != room_map_.end();)
	{
		Room &room = (*it).second;
		uint64_t battle_guid = room.battle_guid;
		bool ql = false;
		if (now > ROOM_TIME + room.start_time)
		{
			ql = true;
			service::log()->error("clear1 room guid = <%llu>", battle_guid);
		}
		if (room.is_create && now > ROOM_CREATE_TIME + room.start_time)
		{
			ql = true;
			service::log()->error("clear2 room guid = <%llu>", battle_guid);
		}
		if (ql)
		{
			unused_ports_.push_back(room.port);
			ACE_Process_Manager::instance()->terminate(room.pid);
			killed_pids_.push_back(room.pid);
			delete room.rc;
			room_map_.erase(it++);
			RoomMasterMessage::send_push_rm_rc_battle_end(battle_guid);
			continue;
		}
		it++;
	}
	int num = 100;
	if (num > killed_pids_.size())
	{
		num = killed_pids_.size();
	}
	for (int i = 0; i < num; ++i)
	{
		pid_t pid = killed_pids_.front();
		killed_pids_.pop_front();
		int res = ACE_Process_Manager::instance()->wait(pid, ACE_Time_Value::zero);
		if (res == -1)
		{
			killed_pids_.push_back(pid);
		}
	}
	return 0;
}
