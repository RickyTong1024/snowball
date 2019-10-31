#ifndef __ROLE_MANAGER_H__
#define __ROLE_MANAGER_H__

#include "service_inc.h"

class RoleManager
{
public:
	RoleManager();

	~RoleManager();

	int init();

	int fini();

	int terminal_role_on(Packet *pck, const std::string &name);

	int terminal_role_hecheng(Packet *pck, const std::string &name);

	int terminal_role_levelup(Packet *pck, const std::string &name);
};

#endif
