#include "rank_manager.h"
#include "hall_message.h"
#include "rank_operation.h"
#include "rank_config.h"
#include "hall_pool.h"

RankManager::RankManager()
{

}

RankManager::~RankManager()
{

}

int RankManager::init()
{
	if (-1 == sRankConfig->parse())
	{
		return -1;
	}
	return 0;
}

int RankManager::fini()
{
	RankOperation::clear_rank();
	return 0;
}

int RankManager::terminal_rank(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_rank msg;
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

	dhc::rank_t *rank = RankOperation::get_rank(msg.id());
	if (!rank)
	{
		ERROR_SYS;
		return -1;
	}

	HallMessage::send_smsg_rank(player, rank);
	return 0;
}

int RankManager::push_rank_hall_cache(Packet *pck, const std::string &name)
{
	protocol::game::push_rank_hall_cache msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	for (int i = 0; i < msg.ranks_size(); ++i)
	{
		RankOperation::set_rank(msg.id(i), msg.ranks(i));
	}
	return 0;
}

int RankManager::push_pipe_rank_forbidden(Packet *pck, const std::string &name)
{
	protocol::pipe::pmsg_rank_forbidden1 msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	TermInfo *ti = sHallPool->get_terminfo(msg.guid());
	if (ti)
	{
		ti->acc->set_rank_forbid(1);
	}
	HallMessage::send_push_hall_rank_forbidden(msg.guid());

	return 0;
}
