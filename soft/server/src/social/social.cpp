#include "service.h"
#include <ace/ACE.h>
#include "social_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	SocialManager social;
	if (-1 == service::init(argv[1], argv[2], &social))
	{
		return -1;
	}
	if (-1 == social.init())
	{
		return -1;
	}
	service::run();
	social.fini();
	service::fini();

	return 0;
}