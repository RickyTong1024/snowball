#ifndef __SOCIAL_OPERATION_H__
#define __SOCIAL_OPERATION_H__

#include "service_inc.h"
#include "protocol_inc.h"

struct SocialOperation
{
	enum SocialType
	{
		ST_APPLY =  1,
		ST_FRIEND =  2,
		ST_BLACK = 3,
	};

	enum SocialFlag
	{
		SF_ONLINE = 1 << 1,
		SF_GOLD = 1 << 2,
		SF_FIGHT = 1 << 3,
	};
};
#endif