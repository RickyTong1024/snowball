#include "chat_pool.h"
#include "hall_message.h"

#define CHAT_INTERVAL 3000
#define CHAT_MAX_NUM 500

void ChatPool::init()
{
	channel_id_ = 0;
}

void ChatPool::fini()
{
}

void ChatPool::add_player_channel(uint64_t player_guid)
{
	if (player_channel_.find(player_guid) != player_channel_.end())
	{
		return;
	}
	ChatChannel *cc = 0;
	if (!idle_channel_.empty())
	{
		int channel_id = *(idle_channel_.begin());
		cc = &chat_channel_[channel_id];
		cc->player_guids.insert(player_guid);
		player_channel_[player_guid] = channel_id;
		if (chat_channel_[channel_id].player_guids.size() >= CHAT_MAX_NUM)
		{
			idle_channel_.erase(channel_id);
		}
	}
	else
	{
		channel_id_++;
		cc = &chat_channel_[channel_id_];
		cc->player_guids.insert(player_guid);
		player_channel_[player_guid] = channel_id_;
		idle_channel_.insert(channel_id_);
	}
	for (std::list<protocol::game::smsg_chat *>::iterator it = cc->contents.begin(); it != cc->contents.end(); ++it)
	{
		protocol::game::smsg_chat *msg = *it;
		HallMessage::send_smsg_chat(player_guid, msg);
	}
}

void ChatPool::remove_player_channel(uint64_t player_guid)
{
	if (player_channel_.find(player_guid) == player_channel_.end())
	{
		return;
	}
	int channel_id = player_channel_[player_guid];
	chat_channel_[channel_id].player_guids.erase(player_guid);
	if (idle_channel_.find(channel_id) == idle_channel_.end())
	{
		idle_channel_.insert(channel_id);
	}
	player_channel_.erase(player_guid);
	chat_time_.erase(player_guid);
}

bool ChatPool::refresh_chat_time(uint64_t player_guid)
{
	return true;
	if (chat_time_.find(player_guid) != chat_time_.end())
	{
		uint64_t ctime = chat_time_[player_guid];
		if (ctime + CHAT_INTERVAL < service::timer()->now())
		{
			chat_time_[player_guid] = service::timer()->now();
			return true;
		}
		else
		{
			return false;
		}
	}
	else
	{
		chat_time_[player_guid] = service::timer()->now();
		return true;
	}
}

void ChatPool::chat_normal(dhc::player_t *player, const std::string &text)
{
	if (player_channel_.find(player->guid()) == player_channel_.end())
	{
		return;
	}
	int channel_id = player_channel_[player->guid()];
	std::set<uint64_t> &players = chat_channel_[channel_id].player_guids;

	protocol::game::smsg_chat msg;
	msg.set_player_guid(player->guid());
	msg.set_player_name(player->name());
	msg.set_sex(player->sex());
	msg.set_level(player->level());
	msg.set_avatar(player->avatar_on());
	msg.set_toukuang(player->toukuang_on());
	msg.set_region_id(player->region_id());
	msg.set_name_color(PlayerOperation::player_get_name_color(player));
	msg.set_type(0);
	msg.set_text(text);
	msg.set_time(service::timer()->now());
	
	chat_channel_[channel_id].add_content(&msg);
	for (std::set<uint64_t>::iterator it = players.begin(); it != players.end(); ++it)
	{
		uint64_t guid = *it;
		HallMessage::send_smsg_chat(guid, &msg);
	}
}

void ChatPool::chat_horn(protocol::game::smsg_chat *msg)
{
	for (std::map< int, ChatChannel >::iterator it = chat_channel_.begin(); it != chat_channel_.end(); ++it)
	{
		ChatChannel &cc = (*it).second;
		cc.add_content(msg);
	}
	for (std::map<uint64_t, int>::iterator it = player_channel_.begin(); it != player_channel_.end(); ++it)
	{
		dhc::player_t *player = POOL_GET((*it).first, dhc::player_t);
		if (!player)
		{
			continue;
		}
		HallMessage::send_smsg_chat(player->guid(), msg);
	}
}

void ChatPool::gonggao(protocol::game::smsg_gonggao *msg)
{
	for (std::map<uint64_t, int>::iterator it = player_channel_.begin(); it != player_channel_.end(); ++it)
	{
		dhc::player_t *player = POOL_GET((*it).first, dhc::player_t);
		if (!player)
		{
			continue;
		}
		HallMessage::send_smsg_gonggao(player->guid(), msg);
	}
}

void ChatPool::sys_info(const std::string &text)
{
	for (std::map<uint64_t, int>::iterator it = player_channel_.begin(); it != player_channel_.end(); ++it)
	{
		dhc::player_t *player = POOL_GET((*it).first, dhc::player_t);
		if (!player)
		{
			continue;
		}
		HallMessage::send_smsg_sys_info(player, text);
	}
}
