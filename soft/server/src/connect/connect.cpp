#include "service.h"
#include <ace/ACE.h>
#include "connect_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	ConnectManager connect;
	if (-1 == service::init(argv[1], argv[2], &connect))
	{
		return -1;
	}
	if (-1 == connect.init())
	{
		return -1;
	}
	service::run();
	connect.fini();
	service::fini();

	return 0;
}
