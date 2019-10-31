#include "team_manager.h"
#include "hall_message.h"
#include "hall_pool.h"
#include "item_operation.h"
#include "social_pool.h"

#define MAX_TUIJIAN_NUM 5
#define ONCE_TUIJIAN_NUM 3

TeamManager::TeamManager()
{

}

TeamManager::~TeamManager()
{

}

int TeamManager::init()
{
	return 0;
}

int TeamManager::fini()
{
	return 0;
}

int TeamManager::push_team_hall_error(Packet *pck, const std::string &name)
{
	protocol::game::push_team_hall_error msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	HallMessage::send_smsg_error(msg.player_guid(), (operror_t)msg.code(), msg.text());
	return 0;
}

int TeamManager::terminal_team_create(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (!ti)
	{
		return -1;
	}
	if (ti->battle_state)
	{
		HallMessage::send_smsg_has_battle(player_guid);
		return 0;
	}
	HallMessage::send_push_hall_team_create(player);
	return 0;
}

int TeamManager::push_team_hall_create(Packet *pck, const std::string &name)
{
	protocol::game::push_team_hall_create msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	dhc::player_t *player = POOL_GET(msg.player_guid(), dhc::player_t);
	if (!player)
	{
		return -1;
	}
	HallMessage::send_smsg_team_create(player, msg.team());
	return 0;
}

int TeamManager::terminal_team_tuijian(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	int count = 0;
	std::set<uint64_t> guids;
	do
	{
		if (guids.size() >= MAX_TUIJIAN_NUM)
		{
			break;
		}
		count++;
		std::set<uint64_t> player_guids;
		sHallPool->get_random_players(player->guid(), ONCE_TUIJIAN_NUM, player_guids);
		for (std::set<uint64_t>::const_iterator it = player_guids.begin();
			it != player_guids.end();
			++it)
		{
			if ((guids.find(*it) != guids.end()) ||
				sSocialPool->is_friend(player_guid, *it))
			{
				continue;
			}
			guids.insert(*it);
		}
	} while (count < ONCE_TUIJIAN_NUM);
	HallMessage::send_smsg_team_tuijian(player, guids);
	return 0;
}

int TeamManager::terminal_team_join(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_team_join msg;
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
	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (!ti)
	{
		return -1;
	}
	if (ti->battle_state)
	{
		HallMessage::send_smsg_has_battle(player_guid);
		return 0;
	}
	HallMessage::send_push_hall_team_join(player, msg.player_guid());
	return 0;
}

int TeamManager::push_team_hall_join(Packet *pck, const std::string &name)
{
	protocol::game::push_team_hall_join msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	dhc::player_t *player = POOL_GET(msg.player_guid(), dhc::player_t);
	if (!player)
	{
		return -1;
	}
	HallMessage::send_smsg_team_join(player, msg.team());
	return 0;
}

int TeamManager::push_team_hall_other_join(Packet *pck, const std::string &name)
{
	protocol::game::push_team_hall_other_join msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	for (int i = 0; i < msg.guids_size(); ++i)
	{
		dhc::player_t *player = POOL_GET(msg.guids(i), dhc::player_t);
		if (!player)
		{
			continue;
		}
		HallMessage::send_smsg_team_other_join(player, msg.member());
	}
	return 0;
}

int TeamManager::terminal_team_exit(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	HallMessage::send_push_hall_team_exit(player);
	return 0;
}

int TeamManager::push_team_hall_exit(Packet *pck, const std::string &name)
{
	protocol::game::push_team_hall_exit msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	for (int i = 0; i < msg.guids_size(); ++i)
	{
		dhc::player_t *player = POOL_GET(msg.guids(i), dhc::player_t);
		if (!player)
		{
			continue;
		}
		HallMessage::send_smsg_team_exit(player, msg.leader_guid(), msg.player_guid());
	}
	return 0;
}

int TeamManager::terminal_team_kick(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_team_kick msg;
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
	HallMessage::send_push_hall_team_kick(player, msg.target_guid());
	return 0;
}

int TeamManager::push_team_hall_kick(Packet *pck, const std::string &name)
{
	protocol::game::push_team_hall_kick msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	for (int i = 0; i < msg.guids_size(); ++i)
	{
		dhc::player_t *player = POOL_GET(msg.guids(i), dhc::player_t);
		if (!player)
		{
			continue;
		}
		HallMessage::send_smsg_team_kick(player, msg.player_guid());
	}
	return 0;
}

int TeamManager::terminal_team_invert(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_team_invert msg;
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
	HallMessage::send_push_hall_team_invert(player, msg.target_guid());
	return 0;
}

int TeamManager::push_team_hall_invert(Packet *pck, const std::string &name)
{
	protocol::game::push_team_hall_invert msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	dhc::player_t *player = POOL_GET(msg.target_guid(), dhc::player_t);
	if (!player)
	{
		return -1;
	}
	TermInfo *ti = sHallPool->get_terminfo(player->guid());
	if (!ti)
	{
		return -1;
	}
	if (ti->battle_state)
	{
		return 0;
	}
	HallMessage::send_smsg_team_invert(player, msg.player());
	return 0;
}

int TeamManager::terminal_team_chat(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_team_chat msg;
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
	HallMessage::send_push_hall_team_chat(player, msg.text());
	return 0;
}

int TeamManager::push_team_hall_chat(Packet *pck, const std::string &name)
{
	protocol::game::push_team_hall_chat msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	for (int i = 0; i < msg.guids_size(); ++i)
	{
		dhc::player_t *player = POOL_GET(msg.guids(i), dhc::player_t);
		if (!player)
		{
			continue;
		}
		HallMessage::send_smsg_team_chat(player, msg.player_guid(), msg.text());
	}
	return 0;
}

int TeamManager::terminal_multi_battle(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	HallMessage::send_push_hall_team_multi_battle(player);
	return 0;
}

int TeamManager::push_team_hall_multi_battle(Packet *pck, const std::string &name)
{
	protocol::game::push_team_hall_multi_battle msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	for (int i = 0; i < msg.guids_size(); ++i)
	{
		dhc::player_t *player = POOL_GET(msg.guids(i), dhc::player_t);
		if (!player)
		{
			continue;
		}
		if (msg.code(i) == "")
		{
			HallMessage::send_smsg_team_multi_battle(player, "", 0, "", 0, "", msg.num());
		}
		else
		{
			TermInfo *ti = sHallPool->get_terminfo(player->guid());
			if (ti)
			{
				ti->set_battle_state(1, msg.udp_ip(), msg.udp_port(), msg.tcp_ip(), msg.tcp_port(), msg.code(i));
				HallMessage::send_push_social_fight(player->guid(), 1);
			}
			if (msg.num() > 1)
			{
				PlayerOperation::add_all_type_num(player, 41, 1);
			}
			HallMessage::send_smsg_team_multi_battle(player, msg.udp_ip(), msg.udp_port(), msg.tcp_ip(), msg.tcp_port(), msg.code(i), msg.num());
		}
	}
	return 0;
}
