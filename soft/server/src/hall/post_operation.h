#ifndef __POST_OPERATION_H__
#define __POST_OPERATION_H__

#include "service_inc.h"
#include "reward.h"

class PostOperation
{
public:
	static void post_create(uint64_t player_guid, const std::string &title, const std::string &text,
		const std::string &sender_name, const s_t_rewards &rewards);

	static void post_create_online(dhc::player_t *player, const std::string &title, const std::string &text,
		const std::string &sender_name, const s_t_rewards &rewards);

	static void post_create_offline(uint64_t player_guid, const std::string &title, const std::string &text,
		const std::string &sender_name, const s_t_rewards &rewards);

	static void post_delete(dhc::player_t *player, uint64_t post_guid);

	static void get_new_post(dhc::player_t *player);

	static void get_new_post_callback(Request *req, uint64_t player_guid);

	static void check_post(dhc::player_t *player);

	static void get_share(dhc::player_t *player);

	static void get_share_callback(Request *req, uint64_t player_guid);
};

#endif
