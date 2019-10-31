#include "post_manager.h"
#include "hall_message.h"
#include "post_operation.h"

PostManager::PostManager()
{

}

PostManager::~PostManager()
{

}

int PostManager::init()
{
	return 0;
}

int PostManager::fini()
{
	return 0;
}

int PostManager::terminal_post_look(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	HallMessage::send_smsg_post_look(player);
	return 0;
}

int PostManager::terminal_post_read(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_post_read msg;
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
	dhc::post_t *post = POOL_GET(msg.post_guid(), dhc::post_t);
	if (!post)
	{
		ERROR_SYS;
		return -1;
	}
	if (post->player_guid() != player->guid())
	{
		ERROR_SYS;
		return -1;
	}
	if (post->is_read() != 0)
	{
		ERROR_SYS;
		return -1;
	}
	post->set_is_read(1);
	HallMessage::send_smsg_success(player, SMSG_POST_READ);
	return 0;
}

int PostManager::terminal_post_get(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_post_get msg;
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
	dhc::post_t *post = POOL_GET(msg.post_guid(), dhc::post_t);
	if (!post)
	{
		ERROR_SYS;
		return -1;
	}
	if (post->player_guid() != player->guid())
	{
		ERROR_SYS;
		return -1;
	}
	if (!post->is_read())
	{
		post->set_is_read(1);
	}
	if (post->is_pick() != 0)
	{
		ERROR_SYS;
		return -1;
	}
	post->set_is_pick(1);
	s_t_rewards rewards;
	for (int i = 0; i < post->type_size(); ++i)
	{
		rewards.add_reward(post->type(i), post->value1(i), post->value2(i), post->value3(i));
	}
	PlayerOperation::player_add_reward(player, rewards);
	HallMessage::send_smsg_post_get(player, rewards);
	return 0;
}

int PostManager::terminal_post_delete(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_post_delete msg;
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
	dhc::post_t *post = POOL_GET(msg.post_guid(), dhc::post_t);
	if (!post)
	{
		ERROR_SYS;
		return -1;
	}
	if (post->player_guid() != player->guid())
	{
		ERROR_SYS;
		return -1;
	}
	if (post->is_read() == 0 || post->is_pick() == 0)
	{
		ERROR_SYS;
		return -1;
	}
	PostOperation::post_delete(player, post->guid());
	HallMessage::send_smsg_success(player, SMSG_POST_DELETE);
	return 0;
}

int PostManager::terminal_post_get_all(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	std::vector<uint64_t> guids;
	s_t_rewards rewards;
	for (int i = 0; i < player->post_guids_size(); ++i)
	{
		dhc::post_t *post = POOL_GET(player->post_guids(i), dhc::post_t);
		if (!post)
		{
			continue;
		}
		if (post->is_read() && post->is_pick())
		{
			continue;
		}
		if (!post->is_read())
		{
			post->set_is_read(1);
		}
		if (!post->is_pick())
		{
			post->set_is_pick(1);
			for (int i = 0; i < post->type_size(); ++i)
			{
				rewards.add_reward(post->type(i), post->value1(i), post->value2(i), post->value3(i));
			}
		}
		guids.push_back(post->guid());
	}
	PlayerOperation::player_add_reward(player, rewards);
	HallMessage::send_smsg_post_get_all(player, guids, rewards);
	return 0;
}

int PostManager::terminal_post_delete_all(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	std::vector<uint64_t> guids;
	for (int i = 0; i < player->post_guids_size();)
	{
		dhc::post_t *post = POOL_GET(player->post_guids(i), dhc::post_t);
		if (post)
		{
			if (post->is_read() && post->is_pick())
			{
				guids.push_back(post->guid());
				player->mutable_post_guids()->SwapElements(i, player->post_guids_size() - 1);
				player->mutable_post_guids()->RemoveLast();
				service::pool()->remove(post->guid(), player->guid());
				continue;
			}
		}
		++i;
	}
	HallMessage::send_smsg_post_delete_all(player, guids);
	return 0;
}
