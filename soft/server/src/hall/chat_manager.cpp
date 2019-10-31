#include "chat_manager.h"
#include "hall_message.h"
#include "chat_pool.h"
#include "item_operation.h"

ChatManager::ChatManager()
{

}

ChatManager::~ChatManager()
{

}

int ChatManager::init()
{
	sChatPool->init();
	return 0;
}

int ChatManager::fini()
{
	sChatPool->fini();
	return 0;
}

int ChatManager::terminal_chat_channel(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_chat msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	if (msg.type() < 0 || msg.type() > 1)
	{
		ERROR_SYS;
		return -1;
	}
	if (msg.text().length() > 128)
	{
		ERROR_SYS;
		return -1;
	}
	std::string text = msg.text();
	service::scheme()->change_illword(text);
	if (msg.type() == 0)
	{
		if (!sChatPool->refresh_chat_time(player_guid))
		{
			HallMessage::send_smsg_error(player->guid(), ERROR_CHAT_INTERVAL);
			return -1;
		}
		sChatPool->chat_normal(player, text);
	}
	else if (msg.type() == 1)
	{
		int num = ItemOperation::item_num_templete(player, 50010001);
		if (num < 1)
		{
			ERROR_SYS;
			return -1;
		}
		ItemOperation::item_destory_templete(player, 50010001, 1);
		PlayerOperation::add_all_type_num(player, 140, 1);
		HallMessage::send_smsg_chat_laba(player);
		HallMessage::send_push_hall_center_chat_horn(player, text);
	}
	return 0;
}

int ChatManager::push_hall_center_chat_horn(Packet *pck, const std::string &name)
{
	protocol::game::smsg_chat msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	sChatPool->chat_horn(&msg);
	return 0;
}

int ChatManager::push_pipe_center_gonggao(Packet *pck, const std::string &name)
{
	protocol::game::smsg_gonggao msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	sChatPool->gonggao(&msg);
	return 0;
}
