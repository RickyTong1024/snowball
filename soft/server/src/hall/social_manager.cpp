#include "social_manager.h"
#include "social_pool.h"
#include "hall_message.h"
#include "player_loader.h"
#include "hall_pool.h"


SocialManager::SocialManager()
{

}

SocialManager::~SocialManager()
{

}

int SocialManager::init()
{
	return 0;
}

int SocialManager::fini()
{
	return 0;
}

int SocialManager::terminal_look(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_social_look msg;
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

	HallMessage::send_req_social_look(player_guid, msg.type(), boost::bind(&SocialManager::look_callback, this, _1, _2, player_guid));

	return 0;
}

int SocialManager::look_callback(Packet *pck, int error_code, uint64_t player_guid)
{
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, operror_t(error_code));
		return -1;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	protocol::game::smsg_social_look msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	HallMessage::send_smsg_social_look(player, SMSG_SOCAIL_LOOK, msg);

	return 0;
}

int SocialManager::terminal_delete(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_social_delete msg;
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

	HallMessage::send_req_social_delete(player_guid, msg.target_guid(), boost::bind(&SocialManager::social_delete_callback, this, _1, _2, player_guid, msg.target_guid()));

	return 0;
}

int SocialManager::social_delete_callback(Packet *pck, int error_code, uint64_t player_guid, uint64_t target_guid)
{
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, operror_t(error_code));
		return -1;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	sSocialPool->remove_friend(player_guid, target_guid);
	HallMessage::send_smsg_social_delete(player, target_guid);

	return 0;
}

int SocialManager::terminal_social_delete(Packet *pck, const std::string &name)
{
	protocol::game::pmsg_social_delete msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	dhc::player_t *player = POOL_GET(msg.player_guid(), dhc::player_t);
	if (player)
	{
		if (msg.df())
		{
			HallMessage::send_smsg_social_delete(player, msg.target_guid());
		}
		if (msg.sf())
		{
			sSocialPool->remove_friend(msg.player_guid(), msg.target_guid());
			HallMessage::send_smsg_social_gold(player, msg.gold());
		}
	}

	return 0;
}

int SocialManager::terminal_apply_reject(Packet *pck, const std::string &name)
{
	uint64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	HallMessage::send_push_soical_reject(player_guid, false, boost::bind(&SocialManager::reject_callback, this, _1, _2, player_guid));

	return 0;
}

int SocialManager::reject_callback(Packet *pck, int error_code, uint64_t player_guid)
{
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, operror_t(error_code));
		return -1;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	HallMessage::send_smsg_success(player, SMSG_SOCIAL_APPLY_FLAG);

	return 0;
}

int SocialManager::terminal_apply(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_social_apply msg;
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

	if (player->guid() == msg.target_guid())
	{
		return -1;
	}

	dhc::social_t apply_social;
	apply_social.set_player_guid(msg.target_guid());
	apply_social.set_target_guid(player->guid());
	apply_social.set_name(player->name());
	apply_social.set_cup(player->cup());
	apply_social.set_avatar(player->avatar_on());
	apply_social.set_toukuang(player->toukuang_on());
	apply_social.set_region_id(player->region_id());
	apply_social.set_level(player->level());
	apply_social.set_sex(player->sex());
	apply_social.set_name_color(PlayerOperation::player_get_name_color(player));
	apply_social.set_achieve_point(player->achieve_point());
	apply_social.set_max_score(player->max_score());
	apply_social.set_max_sha(player->max_sha());
	apply_social.set_max_lsha(player->max_lsha());
	apply_social.set_verify(msg.verify());
	apply_social.set_stype(1);
	apply_social.set_ttime(service::timer()->now() + 24 * 60 * 60 * 1000);

	HallMessage::send_req_social_apply(apply_social, boost::bind(&SocialManager::apply_callback, this, _1, _2, player_guid));

	return 0;
}

int SocialManager::apply_callback(Packet *pck, int error_code, uint64_t player_guid)
{
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, operror_t(error_code));
		return -1;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	HallMessage::send_smsg_success(player, SMSG_SOCIAL_APPLY);

	return 0;
}

int SocialManager::terminal_social_apply(Packet *pck, const std::string &name)
{
	protocol::game::rmsg_social_apply_flag msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}


	dhc::player_t *player = POOL_GET(msg.player_guid(), dhc::player_t);
	if (player)
	{
		HallMessage::send_smsg_social_apply(player, msg.num());
	}
	
	return 0;
}


int SocialManager::terminal_add(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_social_add msg;
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

	if (player->guid() == msg.target_guid())
	{
		return -1;
	}

	dhc::social_t agree_social;
	agree_social.set_player_guid(msg.target_guid());
	agree_social.set_target_guid(player->guid());
	agree_social.set_name(player->name());
	agree_social.set_cup(player->cup());
	agree_social.set_avatar(player->avatar_on());
	agree_social.set_toukuang(player->toukuang_on());
	agree_social.set_region_id(player->region_id());
	agree_social.set_level(player->level());
	agree_social.set_sex(player->sex());
	agree_social.set_name_color(PlayerOperation::player_get_name_color(player));
	agree_social.set_achieve_point(player->achieve_point());
	agree_social.set_max_score(player->max_score());
	agree_social.set_max_sha(player->max_sha());
	agree_social.set_max_lsha(player->max_lsha());
	agree_social.set_stype(2);
	agree_social.set_sflag(2);
	agree_social.set_gold((msg.agree() ? 0 : 1));

	HallMessage::send_req_social_add(agree_social, boost::bind(&SocialManager::add_callback, this, _1, _2, player->guid(), msg.target_guid(), msg.agree()));

	return 0;
}

int SocialManager::add_callback(Packet *pck, int error_code, uint64_t player_guid, uint64_t taret_guid, bool agree)
{
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, operror_t(error_code));
		return -1;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	protocol::game::rmsg_social_apply msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	if (agree)
	{
		PlayerOperation::add_all_type_num(player, 42, 1);
		HallMessage::send_smsg_social_add(player, SMSG_SOCIAL_ADD, msg.social());
		sSocialPool->add_friend(msg.social().player_guid(), msg.social().target_guid());
	}
	else
	{
		HallMessage::send_smsg_social_delete(player, player_guid);
	}

	return 0;
}

int SocialManager::terminal_social_add(Packet *pck, const std::string &name)
{
	protocol::game::rmsg_social_apply msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	dhc::player_t *player = POOL_GET(msg.social().player_guid(), dhc::player_t);
	if (player)
	{
		if (msg.type() == 1)
		{
			HallMessage::send_smsg_social_delete(player, player->guid());
		}
		PlayerOperation::add_all_type_num(player, 42, 1);
		HallMessage::send_smsg_social_add(player, SMSG_SOCIAL_ADD, msg.social());
		sSocialPool->add_friend(msg.social().player_guid(), msg.social().target_guid());
	}
	
	return 0;
}


int SocialManager::terminal_black(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_social_black msg;
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

	if (player->guid() == msg.target_guid())
	{
		return -1;
	}

	HallMessage::send_req_social_black(msg.target_guid(), player_guid, boost::bind(&SocialManager::black_callback, this, _1, _2, player_guid, msg.target_guid()));

	return 0;
}

int SocialManager::terminal_social_black(Packet *pck, const std::string &name, int id)
{
	protocol::game::rmsg_social_black msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}


	dhc::player_t *player = POOL_GET(msg.player_guid(), dhc::player_t);
	if (!player)
	{
		sPlayerLoader->load_player(msg.player_guid(), boost::bind(&SocialManager::social_black_callback, this, msg.player_guid(), msg.target_guid(), id));
	}

	return social_black_callback(msg.player_guid(), msg.target_guid(), id);
}

int SocialManager::black_callback(Packet *pck, int error_code, uint64_t player_guid, uint64_t target_guid)
{
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, operror_t(error_code));
		return -1;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	protocol::game::rmsg_social_apply msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	if (msg.type() == 1)
	{
		HallMessage::send_smsg_social_delete(player, player_guid);
	}

	HallMessage::send_smsg_social_add(player, SMSG_SOCIAL_BLACK, msg.social());

	return 0;
}

int SocialManager::social_black_callback(uint64_t player_guid, uint64_t target_guid, int id)
{
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		HallMessage::send_rep_hall_social_black(id, ERROR_SYSTEM, dhc::social_t());
		return -1;
	}

	dhc::social_t black_social;
	black_social.set_player_guid(target_guid);
	black_social.set_target_guid(player->guid());
	black_social.set_name(player->name());
	black_social.set_cup(player->cup());
	black_social.set_avatar(player->avatar_on());
	black_social.set_toukuang(player->toukuang_on());
	black_social.set_region_id(player->region_id());
	black_social.set_level(player->level());
	black_social.set_sex(player->sex());
	black_social.set_name_color(PlayerOperation::player_get_name_color(player));
	black_social.set_stype(3);

	HallMessage::send_rep_hall_social_black(id, 0, black_social);

	return 0;
}

int SocialManager::terminal_gift(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_social_gift msg;
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

	if (!sSocialPool->is_friend(player_guid, msg.target_guid()))
	{
		return -1;
	}

	for (int i = 0; i < player->social_golds_size(); ++i)
	{
		if (player->social_golds(i) == msg.target_guid())
		{
			return -1;
		}
	}

	if (player->social_golds_size() >= SOCIAL_GIFT_NUM)
	{
		return -1;
	}

	
	HallMessage::send_req_social_gift(msg.target_guid(), player_guid, msg.gold(), boost::bind(&SocialManager::gift_callback, this, _1, _2, player_guid, msg.target_guid()));

	return 0;
}

int SocialManager::gift_callback(Packet *pck, int error_code, uint64_t player_guid, uint64_t target_guid)
{
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, operror_t(error_code));
		return -1;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	
	if (!sSocialPool->is_friend(player_guid, target_guid))
	{
		return -1;
	}

	for (int i = 0; i < player->social_golds_size(); ++i)
	{
		if (player->social_golds(i) == target_guid)
		{
			return -1;
		}
	}

	if (player->social_golds_size() >= SOCIAL_GIFT_NUM)
	{
		return -1;
	}

	player->add_social_golds(target_guid);
	HallMessage::send_smsg_success(player, SMSG_SOCIAL_GIFT);
	return 0;
}

int SocialManager::terminal_social_gift(Packet *pck, const std::string &name)
{
	protocol::game::pmsg_social_gift msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	dhc::player_t *player = POOL_GET(msg.player_guid(), dhc::player_t);
	if (player)
	{
		HallMessage::send_smsg_social_gift(player, msg.target_guid(), msg.gold());
	}
	return 0;
}

int SocialManager::terminal_chat(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_social_chat msg;
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

	protocol::game::smsg_chat smsg;
	smsg.set_player_guid(player->guid());
	smsg.set_player_name(player->name());
	smsg.set_sex(player->sex());
	smsg.set_level(player->level());
	smsg.set_avatar(player->avatar_on());
	smsg.set_toukuang(player->toukuang_on());
	smsg.set_region_id(player->region_id());
	smsg.set_name_color(PlayerOperation::player_get_name_color(player));
	smsg.set_type(0);
	smsg.set_text(msg.text());
	smsg.set_time(service::timer()->now());
	HallMessage::send_smsg_social_chat(player, smsg);

	HallMessage::send_push_social_chat(msg.target_guid(), player_guid, player->name(), msg.text());

	return 0;
}

int SocialManager::terminal_social_chat(Packet *pck, const std::string &name)
{
	protocol::game::pmsg_social_chat msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	dhc::player_t *player = POOL_GET(msg.player_guid(), dhc::player_t);
	if (player)
	{
		protocol::game::smsg_chat smsg;
		smsg.set_player_guid(msg.guid());
		smsg.set_player_name(msg.name());
		smsg.set_sex(msg.sex());
		smsg.set_level(msg.level());
		smsg.set_avatar(msg.avatar());
		smsg.set_toukuang(msg.toukuang());
		smsg.set_region_id(msg.region_id());
		smsg.set_type(0);
		smsg.set_text(msg.text());
		smsg.set_time(service::timer()->now());
		smsg.set_name_color(msg.name_color());
		HallMessage::send_smsg_social_chat(player, smsg);
	}
	
	return 0;
}

int SocialManager::terminal_gift_get(Packet *pck, const std::string &name)
{
	int64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}
	
	HallMessage::send_req_social_gold(player_guid, boost::bind(&SocialManager::gold_callback, this, _1, _2, player_guid));

	return 0;
}

int SocialManager::gold_callback(Packet *pck, int error_code, uint64_t player_guid)
{
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, operror_t(error_code));
		return -1;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	protocol::game::rep_social_gold msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	PlayerOperation::player_add_resource(player, resource::GOLD, msg.gold());
	HallMessage::send_smsg_success(player, SMSG_SOCIAL_GIFT_GET);
	return 0;
}

int SocialManager::terminal_login(Packet *pck, const std::string &name)
{
	protocol::game::pmsg_social_login msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	dhc::player_t *player = 0;
	for (int i = 0; i < msg.friend_guids_size(); ++i)
	{
		player = POOL_GET(msg.friend_guids(i), dhc::player_t);
		if (player)
		{
			HallMessage::send_smsg_social_stat(player, msg.player_guid(), msg.name(), msg.sflags(i));
		}
	}

	return 0;
}

int SocialManager::terminal_logout(Packet *pck, const std::string &name)
{
	protocol::game::pmsg_social_login msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	dhc::player_t *player = 0;
	for (int i = 0; i < msg.friend_guids_size(); ++i)
	{
		player = POOL_GET(msg.friend_guids(i), dhc::player_t);
		if (player)
		{
			HallMessage::send_smsg_social_stat(player, msg.player_guid(), msg.name(), msg.sflags(i));
		}
	}

	return 0;
}
int SocialManager::terminal_tuijian(Packet *pck, const std::string &name)
{
	int64_t player_guid = pck->guid();
	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	int count = 0;
	std::set<uint64_t> guids;
	do 
	{
		if (guids.size() >= 3)
		{
			break;
		}

		count++;

		std::set<uint64_t> player_guids;
		sHallPool->get_random_players(player_guid, 3, player_guids);
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

	} while (count < 3);

	protocol::game::smsg_social_look smsg;
	dhc::player_t *target = 0;
	dhc::social_t *social = 0;
	for (std::set<uint64_t>::const_iterator it = guids.begin();
		it != guids.end();
		++it)
	{
		target = POOL_GET(*it, dhc::player_t);
		if (target)
		{
			social = smsg.add_socials();
			if (social)
			{
				social->set_player_guid(player_guid);
				social->set_target_guid(target->guid());
				social->set_name(target->name());
				social->set_cup(target->cup());
				social->set_avatar(target->avatar_on());
				social->set_toukuang(target->toukuang_on());
				social->set_region_id(target->region_id());
				social->set_level(target->level());
				social->set_sex(target->sex());
				social->set_name_color(PlayerOperation::player_get_name_color(target));
			}
		}
	}

	HallMessage::send_smsg_social_look(player, SMSG_SOCIAL_TUIJIAN, smsg);
	return 0;
}

int SocialManager::terminal_search(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_soical_search msg;
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

	HallMessage::send_req_hall_name_search(msg.name(), boost::bind(&SocialManager::name_callback, this, _1, _2, player_guid));

	return 0;
}

int SocialManager::name_callback(Packet *pck, int error_code, uint64_t player_guid)
{
	if (error_code > 0)
	{
		HallMessage::send_smsg_error(player_guid, (operror_t)error_code);
		return 0;
	}

	dhc::player_t *player = POOL_GET(player_guid, dhc::player_t);
	if (!player)
	{
		return -1;
	}

	protocol::game::rep_social_name_search msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	protocol::game::smsg_social_look smsg;
	dhc::social_t *social = 0;
	for (int i = 0; i < msg.players_size(); ++i)
	{
		const protocol::game::msg_name_player &pl = msg.players(i);
		if (pl.guid() == player_guid)
		{
			continue;
		}
		social = smsg.add_socials();
		if (social)
		{
			social->set_target_guid(pl.guid());
			social->set_name(pl.name());
			social->set_cup(pl.cup());
			social->set_avatar(pl.avatar_on());
			social->set_toukuang(pl.toukuang_on());
			social->set_region_id(pl.region_id());
			social->set_level(pl.level());
			social->set_sex(pl.sex());
			social->set_name_color(pl.name_color());
		}
	}
	HallMessage::send_smsg_social_look(player, SMSG_SOCIAL_SEARCH, smsg);

	return 0;
}
