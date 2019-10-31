#include "service.h"
#include <ace/ACE.h>
#include "login_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	LoginManager login;
	if (-1 == service::init(argv[1], argv[2], &login))
	{
		return -1;
	}
	if (-1 == login.init())
	{
		return -1;
	}
	service::run();
	login.fini();
	service::fini();

	return 0;
}
