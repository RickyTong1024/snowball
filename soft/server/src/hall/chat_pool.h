#ifndef __CHAT_POOL_H__
#define __CHAT_POOL_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct ChatChannel
{
	std::set<uint64_t> player_guids;
	std::list<protocol::game::smsg_chat *> contents;

	void add_content(protocol::game::smsg_chat *msg)
	{
		protocol::game::smsg_chat *msg1 = new protocol::game::smsg_chat();
		msg1->CopyFrom(*msg);
		contents.push_back(msg1);
		if (contents.size() > 10)
		{
			protocol::game::smsg_chat *msg2 = contents.front();
			delete msg2;
			contents.pop_front();
		}
	}
};

class ChatPool
{
public:
	void init();

	void fini();

	void add_player_channel(uint64_t player_guid);

	void remove_player_channel(uint64_t player_guid);

	bool refresh_chat_time(uint64_t player_guid);

	void chat_normal(dhc::player_t *player, const std::string &text);

	void chat_horn(protocol::game::smsg_chat *msg);

	void gonggao(protocol::game::smsg_gonggao *msg);

	void sys_info(const std::string &text);

private :
	int channel_id_;
	std::map< int, ChatChannel > chat_channel_;
	std::set<int> idle_channel_;
	std::map<uint64_t, int> player_channel_;
	std::map<uint64_t, uint64_t> chat_time_;
};

#define sChatPool (Singleton<ChatPool>::instance())

#endif
