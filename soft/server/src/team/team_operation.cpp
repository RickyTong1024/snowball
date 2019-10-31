#include "team_operation.h"

void TeamOperation::split_guids_server(std::vector<uint64_t> &guids, std::map<std::string, std::vector<uint64_t> > &name_guids)
{
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	for (int i = 0; i < guids.size(); ++i)
	{
		int index = guids[i] % names.size();
		std::string name = names[index];
		name_guids[name].push_back(guids[i]);
	}
}

std::string TeamOperation::get_guid_server_name(uint64_t guid)
{
	std::vector<std::string> names;
	service::server_env()->get_server_names("hall", names);
	int index = guid % names.size();
	return names[index];
}
