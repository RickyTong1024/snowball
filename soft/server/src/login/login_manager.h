#ifndef __login_MANAGER_H__
#define __login_MANAGER_H__

#include "service_inc.h"
#include "protocol_inc.h"

class LoginManager : public mmg::DispathService
{
public:
	LoginManager();

	~LoginManager();

	BEGIN_PACKET_MAP
	END_PACKET_MAP

	BEGIN_PUSH_MAP
		PUSH_HANDLER(CMSG_LOGIN, terminal_login)
	END_PUSH_MAP

	BEGIN_REQ_MAP
	END_REQ_MAP

	int init();

	int fini();

private:
	int terminal_login(Packet *pck, const std::string &name);

	void login_pipe_callback(Packet *packet, int error_code, const std::string &username, int serverid, const std::string &name, int hid);

	void acc_login(const std::string &username, int serverid, const std::string &name, int hid);

	void acc_login_callback(Request *req, const std::string &name, int hid);
};

#endif
