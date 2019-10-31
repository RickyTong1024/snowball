#ifndef __ITEM_MANAGER_H__
#define __ITEM_MANAGER_H__

#include "service_inc.h"

class ItemManager
{
	
public:
	ItemManager();

	~ItemManager();

	int init();

	int fini();

	int terminal_item_buy(Packet *pack, const std::string &name);

	int terminal_item_sell(Packet *pack, const std::string &name);

	int terminal_item_direct_buy(Packet *pack, const std::string &name);

	int terminal_item_apply(Packet *pck, const std::string &name);

	int terminal_duobao(Packet *pck, const std::string &name);
};

#endif
