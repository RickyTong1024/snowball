#ifndef __SOCIAL_MANAGER_H__
#define __SOCIAL_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

class SocialManager : public mmg::DispathService
{
public:
	SocialManager();

	~SocialManager();

	BEGIN_PACKET_MAP
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(PUSH_SOCIAL_LOGOUT, terminal_push_hall_logout)
		PUSH_HANDLER(PUSH_SOCIAL_CHAT, terminal_push_hall_chat)
		PUSH_HANDLER(PUSH_SOCIAL_GIFT, terminal_push_hall_gift)
		PUSH_HANDLER(PUSH_SOCIAL_FIGHT, terminal_push_hall_fight)
	END_PUSH_MAP

	BEGIN_REQ_MAP
		REQ_HANDLER(REQ_SOCIAL_LOGIN, req_hall_login)
		REQ_HANDLER(REQ_SOCIAL_LOOK, req_hall_look)
		REQ_HANDLER(REQ_SOCIAL_APPLY, req_hall_apply)
		REQ_HANDLER(REQ_SOCIAL_ADD, req_hall_add)
		REQ_HANDLER(REQ_SOCIAL_BLACK, req_social_black)
		REQ_HANDLER(REQ_SOCIAL_DELETE, req_hall_delete)
		REQ_HANDLER(REQ_SOICAL_GOLD, req_hall_gold)
		REQ_HANDLER(REQ_SOCIAL_REJECT, req_hall_reject)
		REQ_HANDLER(REQ_SOCIAL_GIFT, req_hall_gift)
	END_REQ_MAP

	int init();

	int fini();

	int update(const ACE_Time_Value & cur_time);

private:
	int req_hall_login(Packet *pck, const std::string &name, int id);

	int terminal_push_hall_logout(Packet *pck, const std::string &name);

	int req_hall_look(Packet *pck, const std::string &name, int id);

	int req_hall_apply(Packet *pck, const std::string &name, int id);

	void load_apply_callback(const protocol::game::rmsg_social_apply &msg, const std::string &name, int id);

	int req_hall_add(Packet *pck, const std::string &name, int id);

	void add_hall_callback(const protocol::game::rmsg_social_apply &msg, const std::string &name, int id);

	int req_social_black(Packet *pck, const std::string &name, int id);

	int req_social_black_callback(Packet *pck, int error_code, const std::string &name, int id);

	int req_hall_delete(Packet *pck, const std::string &name, int id);

	int terminal_push_hall_gift(Packet *pck, const std::string &name);

	int terminal_push_hall_chat(Packet *pck, const std::string &name);

	int req_hall_login_callback(const dhc::social_t& login_social, const std::string& name, int id);

	int req_hall_gold(Packet *pck, const std::string &name, int id);

	int req_hall_reject(Packet *pck, const std::string &name, int id);

	void look_hall_callback(const protocol::game::req_social_look &msg, const std::string &name, int id);

	int req_hall_gift(Packet *pck, const std::string &name, int id);

	void load_gift_callback(const protocol::game::pmsg_social_gift &msg, const std::string &name, int id);

	int terminal_push_hall_fight(Packet *pck, const std::string &name);

private:
	int timer_id_;
};

#endif