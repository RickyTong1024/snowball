#include "service.h"
#include <ace/ACE.h>
#include "room_center_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	RoomCenterManager room_center;
	if (-1 == service::init(argv[1], argv[2], &room_center))
	{
		return -1;
	}
	if (-1 == room_center.init())
	{
		return -1;
	}
	service::run();
	room_center.fini();
	service::fini();

	return 0;
}
