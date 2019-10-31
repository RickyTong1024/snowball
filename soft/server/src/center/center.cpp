#include "service.h"
#include <ace/ACE.h>
#include "center_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	CenterManager center;
	if (-1 == service::init(argv[1], argv[2], &center))
	{
		return -1;
	}
	if (-1 == center.init())
	{
		return -1;
	}
	service::run();
	center.fini();
	service::fini();

	return 0;
}
