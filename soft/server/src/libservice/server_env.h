#ifndef __SERVER_ENV_H__
#define __SERVER_ENV_H__

#include "service_interface.h"

class ServerEnv : public mmg::ServerEnv
{
public:
	ServerEnv();

	~ServerEnv();

	int init(const std::string &name, const std::string &confpath, const std::string &self);

	int fini();

	virtual std::string get_server_value(const std::string &name, const std::string &key);

	virtual void get_server_names(const std::string &kind, std::vector<std::string> &names);

	virtual void get_server_kinds(std::vector<std::string> &kinds);

	virtual std::string get_db_value(const std::string &name, const std::string &key);

	virtual void get_db_names(const std::string &kind, std::vector<std::string> &names);

	virtual void get_db_kinds(std::vector<std::string> &kinds);

	virtual std::string get_game_value(const std::string &key);

private:
	void split(const std::string &src, const std::string &separator, std::vector<std::string> &dest);

private:
	std::map< std::string, std::map<std::string, std::string> > server_value_;
	std::map< std::string, std::vector<std::string> > server_names_;
	std::vector<std::string> server_kinds_;
	std::map< std::string, std::map<std::string, std::string> > db_value_;
	std::map< std::string, std::vector<std::string> > db_names_;
	std::vector<std::string> db_kinds_;
	std::map<std::string, std::string> game_value_;
};

#endif
