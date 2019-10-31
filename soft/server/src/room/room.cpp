#include "service.h"
#include <ace/ACE.h>
#include "room_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 9)
	{
		return 0;
	}
	std::string self = std::string(argv[3]) + " " + std::string(argv[4]) + " " + std::string(argv[5]) + " "
		+ std::string(argv[6]) + " " + std::string(argv[7]) + " " + std::string(argv[8]) + " " + std::string(argv[9]);
	std::string master_name = std::string(argv[10]);
	uint64_t battle_guid = boost::lexical_cast<uint64_t>(argv[11]);
	int battle_type = boost::lexical_cast<int>(argv[12]);
	RoomManager room;
	if (-1 == service::init(argv[1], argv[2], &room, self))
	{
		return -1;
	}
	if (-1 == room.init(master_name, battle_guid, battle_type))
	{
		return -1;
	}
	service::run();
	room.fini();
	service::fini();

	return 0;
}
