#include "rank_message.h"

void RankMessage::push_rank_hall_cache(std::map<int, dhc::rank_t *> ranks)
{
	protocol::game::push_rank_hall_cache msg;
	for (std::map<int, dhc::rank_t *>::iterator it = ranks.begin(); it != ranks.end(); ++it)
	{
		msg.add_id((*it).first);
		msg.add_ranks()->CopyFrom(*((*it).second));
	}
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	for (int i = 0; i < names.size(); ++i)
	{
		Packet *pck = Packet::New((uint16_t)PUSH_RANK_HALL_CACHE, 0, 0, &msg);
		service::rpc_service()->push(names[i], pck);
	}
}
