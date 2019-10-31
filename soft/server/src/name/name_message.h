#ifndef __NAME_MESSAGE_H__
#define __NAME_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class NameMessage
{
public:
	static void send_rep_social_name_search(const std::string &name, int id, int error_code, const std::vector<protocol::game::msg_name_player *> players);
};

#endif
