#include "service.h"
#include <ace/ACE.h>
#include "room_master_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	RoomMasterManager room_master;
	if (-1 == service::init(argv[1], argv[2], &room_master))
	{
		return -1;
	}
	if (-1 == room_master.init())
	{
		return -1;
	}
	service::run();
	room_master.fini();
	service::fini();

	return 0;
}
