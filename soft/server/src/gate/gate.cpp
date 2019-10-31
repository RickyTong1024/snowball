#include "service.h"
#include <ace/ACE.h>
#include "gate_manager.h"

int ACE_TMAIN(int argc, ACE_TCHAR *argv[])
{
	if (argc < 3)
	{
		return 0;
	}
	GateManager gate;
	if (-1 == service::init(argv[1], argv[2], &gate))
	{
		return -1;
	}
	if (-1 == gate.init())
	{
		return -1;
	}
	service::run();
	gate.fini();
	service::fini();

	return 0;
}
