#include "social_pool.h"
#include "hall_message.h"
#include "hall_pool.h"


void SocialPool::login(dhc::player_t *player)
{
	HallMessage::req_social_login(player, boost::bind(&SocialPool::login_callback, this, _1, _2, player->guid()));
}

void SocialPool::login_callback(Packet *pck, int error_code, uint64_t player_guid)
{
	protocol::game::smsg_social_data msg;
	if (!pck->parse_protocol(msg))
	{
		return;
	}

	TermInfo *ti = sHallPool->get_terminfo(player_guid);
	if (!ti)
	{
		return;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return;
	}

	for (int i = 0; i < msg.friend_guids_size(); ++i)
	{
		add_friend(player_guid, msg.friend_guids(i));
	}

	HallMessage::send_smsg_soical_data(player, msg);
}

void SocialPool::logout(dhc::player_t *player)
{
	HallMessage::send_push_social_logout(player);
	friend_guids_.erase(player->guid());
}

bool SocialPool::is_friend(uint64_t player_guid, uint64_t target_guid) const
{
	std::map<uint64_t, std::set<uint64_t> >::const_iterator it = friend_guids_.find(player_guid);
	if (it == friend_guids_.end())
	{
		return false;
	}
	return it->second.find(target_guid) != it->second.end();
}

void SocialPool::add_friend(uint64_t player_guid, uint64_t target_guid)
{
	friend_guids_[player_guid].insert(target_guid);
}

void SocialPool::remove_friend(uint64_t player_guid, uint64_t target_guid)
{
	std::map<uint64_t, std::set<uint64_t> >::iterator it = friend_guids_.find(player_guid);
	if (it != friend_guids_.end())
	{
		it->second.erase(target_guid);
	}
}