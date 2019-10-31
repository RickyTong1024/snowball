#include "service.h"
#include <ace/ACE.h>
#include "team_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	TeamManager team;
	if (-1 == service::init(argv[1], argv[2], &team))
	{
		return -1;
	}
	if (-1 == team.init())
	{
		return -1;
	}
	service::run();
	team.fini();
	service::fini();

	return 0;
}
