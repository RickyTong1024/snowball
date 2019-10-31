#ifndef __SOCIAL_MESSAGE_H__
#define __SOCIAL_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class SocialMessage
{
public:
	static void rep_social_apply(uint16_t error_code, const std::string& name, int id);

	static void push_social_apply(uint64_t player_guid, int num);

	static void rep_social_add(const dhc::social_t& social, uint16_t error_code, const std::string& name, int id);

	static void push_social_add(const dhc::social_t& social, int type);

	static void rep_social_black(const dhc::social_t& social, uint16_t error_code, int type, const std::string &name, int id);

	static void req_social_black(uint64_t player_guid, uint64_t target_guid, ResponseFunc func);

	static void push_soical_delete(uint64_t player_guid, uint64_t target_guid, int gold = 0, bool sf = false, bool df = true);

	static void rep_social_delete(const std::string &name, int id, uint64_t error_code);

	static void rep_social_login(const protocol::game::smsg_social_data &msg, const std::string& name, int id);

	static void rep_soical_look(const protocol::game::smsg_social_look &msg, uint16_t error_code, const std::string& name, int id);

	static void push_social_logout(uint64_t player_guid, const std::map<int, std::vector<std::pair<uint64_t, int> > >& friends);

	static void push_social_login(uint64_t player_guid, const std::string &name, const std::map<int, std::vector<std::pair<uint64_t, int> > >& friends);

	static void push_social_chat(dhc::social_t *social, const std::string &text);

	static void push_social_gift(uint64_t player_guid, uint64_t target_guid, int gold);

	static void rep_soical_gold(uint64_t player_guid, int gold, const std::string &name, int id, uint64_t error_code);

	static void rep_social_reject(const std::string &name, int id, uint64_t error_code);	

	static void rep_social_gift(uint64_t error_code, const std::string &name, int id);
};

#endif
