#include "social_message.h"

void SocialMessage::rep_social_apply(uint16_t error_code, const std::string& name, int id)
{
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_APPLY, 0, 0, 0);
	service::rpc_service()->response(name, id, pck, error_code);
}

void SocialMessage::push_social_apply(uint64_t player_guid, int num)
{
	protocol::game::rmsg_social_apply_flag msg;
	msg.set_player_guid(player_guid);
	msg.set_num(num);
	Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_APPLY, 0, 0, &msg);
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	service::rpc_service()->push(names[player_guid % names.size()], pck);
}

void SocialMessage::rep_social_add(const dhc::social_t& social, uint16_t error_code, const std::string& name, int id)
{
	protocol::game::rmsg_social_apply msg;
	msg.mutable_social()->CopyFrom(social);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_ADD, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, error_code);
}

void SocialMessage::push_social_add(const dhc::social_t& social, int type)
{
	protocol::game::rmsg_social_apply msg;
	msg.mutable_social()->CopyFrom(social);
	msg.set_type(type);
	Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_ADD, 0, 0, &msg);
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	service::rpc_service()->push(names[social.player_guid() % names.size()], pck);
}

void SocialMessage::rep_social_black(const dhc::social_t& social, uint16_t error_code, int type, const std::string &name, int id)
{
	protocol::game::rmsg_social_apply msg;
	msg.mutable_social()->CopyFrom(social);
	msg.set_type(type);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_BLACK, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, error_code);
}

void SocialMessage::req_social_black(uint64_t player_guid, uint64_t target_guid, ResponseFunc func)
{
	protocol::game::rmsg_social_black msg;
	msg.set_player_guid(player_guid);
	msg.set_target_guid(target_guid);
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_BLACK, 0, 0, &msg);
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	service::rpc_service()->request(names[player_guid % names.size()], pck, func);
}

void SocialMessage::rep_social_login(const protocol::game::smsg_social_data &msg, const std::string& name, int id)
{
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_LOGIN, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, 0);
}

void SocialMessage::rep_soical_look(const protocol::game::smsg_social_look &msg, uint16_t error_code, const std::string& name, int id)
{
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_LOOK, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, 0);
}

void SocialMessage::push_social_logout(uint64_t player_guid, const std::map<int, std::vector<std::pair<uint64_t, int> > >& friends)
{
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);

	for (std::map<int, std::vector<std::pair<uint64_t, int> > >::const_iterator it = friends.begin();
		it != friends.end();
		++it)
	{
		protocol::game::pmsg_social_login msg;
		msg.set_player_guid(player_guid);

		for (std::vector<std::pair<uint64_t, int> >::const_iterator jt = it->second.begin();
			jt != it->second.end();
			++jt)
		{
			msg.add_friend_guids(jt->first);
			msg.add_sflags(jt->second);
		}

		Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_LOGOUT, 0, 0, &msg);
		service::rpc_service()->push(names[it->first], pck);
	}
}

void SocialMessage::push_social_login(uint64_t player_guid, const std::string &name, const std::map<int, std::vector<std::pair<uint64_t, int> > >& friends)
{
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);

	for (std::map<int, std::vector<std::pair<uint64_t, int> > >::const_iterator it = friends.begin();
		it != friends.end();
		++it)
	{
		protocol::game::pmsg_social_login msg;
		msg.set_player_guid(player_guid);
		msg.set_name(name);

		for (std::vector<std::pair<uint64_t, int> >::const_iterator jt = it->second.begin();
			jt != it->second.end();
			++jt)
		{
			msg.add_friend_guids(jt->first);
			msg.add_sflags(jt->second);
		}

		Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_LOGIN, 0, 0, &msg);
		service::rpc_service()->push(names[it->first], pck);
	}
}

void SocialMessage::push_soical_delete(uint64_t player_guid, uint64_t target_guid, int gold, bool sf /* = false */, bool df /* = true */)
{
	protocol::game::pmsg_social_delete msg;
	msg.set_player_guid(player_guid);
	msg.set_target_guid(target_guid);
	msg.set_gold(gold);
	msg.set_sf(sf);
	msg.set_df(df);
	Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_DELETE, 0, 0, &msg);
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	service::rpc_service()->push(names[player_guid % names.size()], pck);
}

void SocialMessage::rep_social_delete(const std::string &name, int id, uint64_t error_code)
{
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_DELETE, 0, 0, 0);
	service::rpc_service()->response(name, id, pck, error_code);
}

void SocialMessage::push_social_chat(dhc::social_t *social, const std::string &text)
{
	protocol::game::pmsg_social_chat msg;
	msg.set_guid(social->target_guid());
	msg.set_name(social->name());
	msg.set_sex(social->sex());
	msg.set_level(social->level());
	msg.set_avatar(social->avatar());
	msg.set_toukuang(social->toukuang());
	msg.set_region_id(social->region_id());
	msg.set_text(text);
	msg.set_player_guid(social->player_guid());
	msg.set_name_color(social->name_color());
	Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_CHAT, 0, 0, &msg);
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	service::rpc_service()->push(names[social->player_guid() % names.size()], pck);
}

void SocialMessage::push_social_gift(uint64_t player_guid, uint64_t target_guid, int gold)
{
	protocol::game::pmsg_social_gift msg;
	msg.set_player_guid(player_guid);
	msg.set_target_guid(target_guid);
	msg.set_gold(gold);
	Packet *pck = Packet::New((uint16_t)PUSH_SOCIAL_GIFT, 0, 0, &msg);
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	service::rpc_service()->push(names[player_guid % names.size()], pck);
}

void SocialMessage::rep_soical_gold(uint64_t player_guid, int gold, const std::string &name, int id, uint64_t error_code)
{
	protocol::game::rep_social_gold msg;
	msg.set_player_guid(player_guid);
	msg.set_gold(gold);
	Packet *pck = Packet::New((uint16_t)REQ_SOICAL_GOLD, 0, 0, &msg);
	service::rpc_service()->response(name, id, pck, error_code);
}

void SocialMessage::rep_social_reject(const std::string &name, int id, uint64_t error_code)
{
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_REJECT, 0, 0, 0);
	service::rpc_service()->response(name, id, pck, error_code);
}

void SocialMessage::rep_social_gift(uint64_t error_code, const std::string &name, int id)
{
	Packet *pck = Packet::New((uint16_t)REQ_SOCIAL_GIFT, 0, 0, 0);
	service::rpc_service()->response(name, id, pck, error_code);
}
