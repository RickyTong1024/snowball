#ifndef __SOCIAL_MANAGER_H__
#define __SOCIAL_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

class SocialManager
{
public:
	
public:
	SocialManager();

	~SocialManager();

	int init();

	int fini();

	int terminal_look(Packet *pck, const std::string &name);

	int terminal_delete(Packet *pck, const std::string &name);

	int terminal_social_delete(Packet *pck, const std::string &name);

	int terminal_apply_reject(Packet *pck, const std::string &name);

	int terminal_apply(Packet *pck, const std::string &name);

	int terminal_social_apply(Packet *pck, const std::string &name);

	int terminal_add(Packet *pck, const std::string &name);

	int terminal_social_add(Packet *pck, const std::string &name);

	int terminal_black(Packet *pck, const std::string &name);

	int terminal_social_black(Packet *pck, const std::string &name, int id);

	int terminal_gift(Packet *pck, const std::string &name);

	int terminal_social_gift(Packet *pck, const std::string &name);

	int terminal_chat(Packet *pck, const std::string &name);

	int terminal_social_chat(Packet *pck, const std::string &name);

	int terminal_gift_get(Packet *pck, const std::string &name);

	int terminal_tuijian(Packet *pck, const std::string &name);

	int terminal_search(Packet *pck, const std::string &name);

	int terminal_login(Packet *pck, const std::string &name);

	int terminal_logout(Packet *pck, const std::string &name);

private:
	int social_delete_callback(Packet *pck, int error_code, uint64_t player_guid, uint64_t target_guid);

	int apply_callback(Packet *pck, int error_code, uint64_t player_guid);

	int add_callback(Packet *pck, int error_code, uint64_t player_guid, uint64_t taret_guid, bool agree);

	int black_callback(Packet *pck, int error_code, uint64_t player_guid, uint64_t target_guid);

	int social_black_callback(uint64_t player_guid, uint64_t target_guid, int id);

	int name_callback(Packet *pck, int error_code, uint64_t player_guid);

	int look_callback(Packet *pck, int error_code, uint64_t player_guid);

	int reject_callback(Packet *pck, int error_code, uint64_t player_guid);

	int gold_callback(Packet *pck, int error_code, uint64_t player_guid);

	int gift_callback(Packet *pck, int error_code, uint64_t player_guid, uint64_t target_guid);
};

#endif
