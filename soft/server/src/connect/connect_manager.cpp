#include "connect_manager.h"
#include "connect_message.h"

#define GATE_KICK_TIME 30000
#define PLAYER_KICK_TIME 10000
#define UPDATE_TIME 1000

ConnectManager::ConnectManager()
: timer_id_(-1)
{
	
}

ConnectManager::~ConnectManager()
{
	
}

int ConnectManager::init()
{
	timer_id_ = service::timer()->schedule(boost::bind(&ConnectManager::update, this, _1), UPDATE_TIME, "connect_manager");
	if (timer_id_ == -1)
	{
		return -1;
	}
	
	return 0;
}

int ConnectManager::fini()
{
	if (timer_id_ != -1)
	{
		service::timer()->cancel(timer_id_);
		timer_id_ = -1;
	}
	return 0;
}

int ConnectManager::terminal_enter_world(Packet *pck)
{
	int hid = pck->hid();
	uint64_t curtime= service::timer()->now();
	hid_times_[hid] = curtime;
	return 0;
}

int ConnectManager::terminal_leave_world(Packet *pck)
{
	int hid = pck->hid();
	hid_times_.erase(hid);
	return 0;
}

int ConnectManager::terminal_request_gate(Packet *pck)
{
	int hid = pck->hid();
	if (gate_datas_.size() == 0)
	{
		ConnectMessage::send_smsg_error(ERROR_EMPTY_GATE, hid);
		return -1;
	}
	std::string name;
	bool flag = false;
	int minnum = 0;
	std::map<std::string, GateData>::iterator it;
	for (it = gate_datas_.begin(); it != gate_datas_.end(); it++)
	{
		GateData &gd = ((*it).second);
		if (!flag)
		{
			minnum = gd.num;
			name = (*it).first;
			flag = true;
		}
		else
		{
			if (gd.num < minnum)
			{
				minnum = gd.num;
				name = (*it).first;
			}
		}
	}
	GateData &gd2 = gate_datas_[name];
	ConnectMessage::send_smsg_request_gate(gd2.ip, gd2.port, hid);
	return 0;
}

int ConnectManager::push_gate_connect_player_num(Packet *pck, const std::string &name)
{
	protocol::game::push_gate_connect_player_num msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t curtime = service::timer()->now();
	std::string ip = msg.ip();
	int port = msg.port();
	int num = msg.num();
	if (gate_datas_.find(name) != gate_datas_.end())
	{
		GateData &gd = gate_datas_[name];
		gd.pretime = curtime;
		gd.ip = ip;
		gd.num = num;
		gd.port = port;
	}
	else
	{
		GateData gd;
		gd.pretime = curtime;
		gd.ip = ip;
		gd.num = num;
		gd.port = port;
		gate_datas_[name] = gd;
	}

	return 0;
}

//////////////////////////////////////////////////////////////////////////

int ConnectManager::time_clear_user()
{
	uint64_t curtime = service::timer()->now();
	std::map<int, uint64_t>::iterator it;
	for (it = hid_times_.begin(); it != hid_times_.end(); it++)
	{
		uint64_t pretime = (*it).second;
		if (curtime >= PLAYER_KICK_TIME + pretime)
		{
			service::tcp_service()->destory((*it).first);
		}
	}

	return 0;
}

int ConnectManager::time_refresh_gate_data()
{
	uint64_t curtime = service::timer()->now();
	std::map<std::string, GateData>::iterator it;
	for (it = gate_datas_.begin(); it != gate_datas_.end();)
	{
		GateData &gd = (*it).second;
		if (curtime > gd.pretime + GATE_KICK_TIME)
		{
			gate_datas_.erase(it++);
		}
		else
		{
			it++;
		}
	}
	return 0;
   
}

int ConnectManager::update(const ACE_Time_Value & cur_time)
{
	time_clear_user();
	time_refresh_gate_data();

	return 0;
}
