#include "name_manager.h"
#include "name_message.h"
#include "name_dhc.h"

NameManager::NameManager()
	: tid_(0)
{
	
}

NameManager::~NameManager()
{
	
}

int NameManager::init()
{
	if (-1 == sNameDhc->init())
	{
		return -1;
	}
	return 0;
}

int NameManager::fini()
{
	sNameDhc->fini();
	return 0;
}

int NameManager::push_hall_name_insert(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_name_insert msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	dhc::name_t * name_t = new dhc::name_t;
	name_t->set_guid(msg.guid());
	name_t->set_name(msg.name());
	Request *req = new Request();
	req->add(opc_insert, MAKE_GUID(et_name, 0), name_t);
	DB_SOCIAL()->upcall(req, 0);
	return 0;
}

int NameManager::req_social_name_search(Packet *pck, const std::string &name, int id)
{
	protocol::game::msg_name_list msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	protocol::game::msg_name_list * name_list = new protocol::game::msg_name_list;
	name_list->set_name(msg.name());
	Request *req = new Request();
	req->add(opc_query, MAKE_GUID(et_name, 0), name_list);
	DB_SOCIAL()->upcall(req, boost::bind(&NameManager::load_name_list_callback, this, _1, name, id));
	return 0;
}

void NameManager::load_name_list_callback(Request *req, const std::string &name, int id)
{
	if (req->result() < 0)
	{
		NameMessage::send_rep_social_name_search(name, id, 0, std::vector<protocol::game::msg_name_player *>());
		return;
	}

	protocol::game::msg_name_list *name_list = (protocol::game::msg_name_list *)req->data();
	if (name_list->guids_size() <= 0)
	{
		NameMessage::send_rep_social_name_search(name, id, 0, std::vector<protocol::game::msg_name_player *>());
		return;
	}

	for (int i = 0; i < name_list->guids_size(); ++i)
	{
		uint64_t guid = name_list->guids(i);
		Request *req1 = new Request();
		req1->add(opc_query, guid, new protocol::game::msg_name_player);
		DB_PLAYER(guid)->upcall(req1, boost::bind(&NameManager::load_name_player_callback, this, _1, tid_, name, id));
	}
	TidPlayer tp;
	tp.num = name_list->guids_size();
	tid_map_[tid_++] = tp;
}

void NameManager::load_name_player_callback(Request *req, int tid, const std::string &name, int id)
{
	TidPlayer &tp = tid_map_[tid];
	tp.num--;
	if (req->result() < 0)
	{
		
	}
	else
	{
		protocol::game::msg_name_player *player = (protocol::game::msg_name_player *)req->release_data();
		tp.players.push_back(player);
	}
	if (tp.num <= 0)
	{
		NameMessage::send_rep_social_name_search(name, id, 0, tp.players);
		for (int i = 0; i < tp.players.size(); ++i)
		{
			delete tp.players[i];
		}
		tid_map_.erase(tid);
	}
}
