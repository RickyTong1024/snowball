#ifndef __CHAT_MANAGER_H__
#define __CHAT_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

class ChatManager
{
public:
	ChatManager();

	~ChatManager();

	int init();

	int fini();

	void add_player_channel(uint64_t player_guid);

	void remove_player_channel(uint64_t player_guid);

	int terminal_chat_channel(Packet *pck, const std::string &name);

	int push_hall_center_chat_horn(Packet *pck, const std::string &name);

	int push_pipe_center_gonggao(Packet *pck, const std::string &name);

private :
	std::set< std::set<uint64_t>* > chat_channel_;
	std::set< std::set<uint64_t>* > idle_channel_;
	std::map< uint64_t, std::set<uint64_t>* > player_channel_;
	std::map<uint64_t, uint64_t> chat_time_;
};

#endif
