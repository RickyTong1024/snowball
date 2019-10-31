#ifndef __SOCIAL_POOL_H__
#define __SOCIAL_POOL_H__

#include "service_inc.h"
#include "protocol_inc.h"

#define SOCIAL_GIFT_NUM 10

class SocialPool
{
public:
	void login(dhc::player_t *player);

	void logout(dhc::player_t *player);

	bool is_friend(uint64_t player_guid, uint64_t target_guid) const;

	void add_friend(uint64_t player_guid, uint64_t target_guid);
	
	void remove_friend(uint64_t player_guid, uint64_t target_guid);

private:
	void login_callback(Packet *pck, int error_code, uint64_t player_guid);

private:
	std::map<uint64_t, std::set<uint64_t> > friend_guids_;
};

#define sSocialPool (Singleton<SocialPool>::instance())

#endif