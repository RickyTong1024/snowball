#ifndef __ROLE_OPERATION_H__
#define __ROLE_OPERATION_H__

#include "service_inc.h"
#include "protocol_inc.h"

class RoleOperation
{
public:
	static dhc::role_t * role_create(dhc::player_t *player, int id);

	static void role_delete(dhc::player_t *player, uint64_t role_guid);

	static bool has_role(dhc::player_t *player, int id);
};

#endif
