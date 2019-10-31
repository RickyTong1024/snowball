#ifndef __SIG_H__
#define __SIG_H__

#include "service_interface.h"

class DfEventHandler : public ACE_Event_Handler
{
public:
	int handle_signal(int signum, siginfo_t*, ucontext_t*);
};

class Sig
{
public:
	Sig();

	~Sig();

	int init();

	int fini();

	void set_sig_handle();

private:
	DfEventHandler *dfe_;
};

#endif
