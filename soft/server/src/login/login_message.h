#ifndef __login_MESSAGE_H__
#define __login_MESSAGE_H__

#include "service_inc.h"
#include "protocol_inc.h"

class LoginMessage
{
public:
	static void send_smsg_error(const std::string &name, int hid, operror_t error);

	static void send_req_login(const std::string &uid, const std::string &token, const std::string &pt, ResponseFunc func);

	static void send_smsg_login(const std::string &name, int hid, dhc::acc_t *acc);
};

#endif
