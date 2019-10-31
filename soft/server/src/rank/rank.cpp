#include "service.h"
#include <ace/ACE.h>
#include "rank_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	RankManager rank;
	if (-1 == service::init(argv[1], argv[2], &rank))
	{
		return -1;
	}
	if (-1 == rank.init())
	{
		return -1;
	}
	service::run();
	rank.fini();
	service::fini();

	return 0;
}