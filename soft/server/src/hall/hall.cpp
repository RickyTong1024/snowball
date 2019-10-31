#include "service.h"
#include <ace/ACE.h>
#include "hall_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	HallManager hall;
	if (-1 == service::init(argv[1], argv[2], &hall))
	{
		return -1;
	}
	if (-1 == hall.init())
	{
		return -1;
	}
	service::run();
	hall.fini();
	service::fini();

	return 0;
}
