#include "login_manager.h"
#include "login_message.h"
#include "login_dhc.h"

LoginManager::LoginManager()
{
	
}

LoginManager::~LoginManager()
{
	
}

int LoginManager::init()
{
	if (-1 == sLoginDhc->init())
	{
		return -1;
	}
	return 0;
}

int LoginManager::fini()
{
	sLoginDhc->fini();
	return 0;
}

int LoginManager::terminal_login(Packet *pck, const std::string &name)
{
	protocol::game::cmsg_login msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}

	int serverid = boost::lexical_cast<int>(service::server_env()->get_game_value("qid"));
	int test = boost::lexical_cast<int>(service::server_env()->get_game_value("test"));
	if (test == 1)
	{
		acc_login(msg.username(), serverid, name, pck->hid());
	}
	else
	{
		LoginMessage::send_req_login(msg.username(), msg.password(), msg.pt(), boost::bind(&LoginManager::login_pipe_callback, this, _1, _2, msg.username(), serverid, name, pck->hid()));
	}
	
	return 0;
}

void LoginManager::login_pipe_callback(Packet *packet, int error_code, const std::string &username, int serverid, const std::string &name, int hid)
{
	protocol::pipe::pmsg_rep_login msg;
	if (!packet->parse_protocol(msg))
	{
		LoginMessage::send_smsg_error(name, hid, ERROR_SYSTEM);
		return;
	}
	if (msg.errres() != 0)
	{
		LoginMessage::send_smsg_error(name, hid, ERROR_ACCOUNT_PASSWORD);
		return;
	}
	acc_login(username, serverid, name, hid);
}

void LoginManager::acc_login(const std::string &username, int serverid, const std::string &name, int hid)
{
	dhc::acc_t *acc = new dhc::acc_t;
	acc->set_guid(0);
	acc->set_username(username);
	acc->set_serverid(serverid);
	Request *req = new Request();
	req->add(opc_query, MAKE_GUID(et_acc, 0), acc);
	DB_ACCOUNT->upcall(req, boost::bind(&LoginManager::acc_login_callback, this, _1, name, hid));
}

void LoginManager::acc_login_callback(Request *req, const std::string &name, int hid)
{
	if (req->result() < 0)
	{
		LoginMessage::send_smsg_error(name, hid, ERROR_SYSTEM);
		return;
	}
	dhc::acc_t *acc = (dhc::acc_t *)req->data();
	if (acc->fenghao_time() > service::timer()->now())
	{
		LoginMessage::send_smsg_error(name, hid, ERROR_FENGHAO);
		return;
	}
	LoginMessage::send_smsg_login(name, hid, acc);
}
