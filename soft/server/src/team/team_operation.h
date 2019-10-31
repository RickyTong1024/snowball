#ifndef __TEAM_OPERATION_H__
#define __TEAM_OPERATION_H__

#include "service_inc.h"
#include "protocol_inc.h"

class TeamOperation
{
public:
	static void split_guids_server(std::vector<uint64_t> &guids, std::map<std::string, std::vector<uint64_t> > &name_guids);

	static std::string get_guid_server_name(uint64_t guid);
};

#endif
