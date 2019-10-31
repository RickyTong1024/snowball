#ifndef __POST_MANAGER_H__
#define __POST_MANAGER_H__

#include "service_inc.h"

class PostManager
{
public:
	PostManager();

	~PostManager();

	int init();

	int fini();

	int terminal_post_look(Packet *pck, const std::string &name);

	int terminal_post_read(Packet *pck, const std::string &name);

	int terminal_post_get(Packet *pck, const std::string &name);

	int terminal_post_delete(Packet *pck, const std::string &name);

	int terminal_post_get_all(Packet *pck, const std::string &name);

	int terminal_post_delete_all(Packet *pck, const std::string &name);
};

#endif
