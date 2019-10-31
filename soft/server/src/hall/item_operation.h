#ifndef __ITEM_OPERATION_H__
#define __ITEM_OPERATION_H__

#include "service_inc.h"
#include "protocol_inc.h"

class ItemOperation
{
public:
	static void item_add_template(dhc::player_t *player, int item_id, int item_amount);

	static int item_num_templete(dhc::player_t *player, int item_id);

	static void item_destory_templete(dhc::player_t *player, int item_id, int item_amount);

	static std::string get_color(int color, const std::string &name);
};

#endif
