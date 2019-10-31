#include "name_message.h"

void NameMessage::send_rep_social_name_search(const std::string &name, int id, int error_code, const std::vector<protocol::game::msg_name_player *> players)
{
	protocol::game::rep_social_name_search msg;
	for (int i = 0; i < players.size(); ++i)
	{
		msg.add_players()->CopyFrom(*players[i]);
	}
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_NAME_SEARCH, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, error_code);
}
