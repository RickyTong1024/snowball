#include "service.h"
#include <ace/ACE.h>
#include "name_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	NameManager name;
	if (-1 == service::init(argv[1], argv[2], &name))
	{
		return -1;
	}
	if (-1 == name.init())
	{
		return -1;
	}
	service::run();
	name.fini();
	service::fini();

	return 0;
}
