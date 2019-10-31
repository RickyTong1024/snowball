#include "sig.h"
#include <ace/Reactor.h>
#include <ace/Signal.h>

int DfEventHandler::handle_signal(int signum, siginfo_t*, ucontext_t*)
{
	ACE_Sig_Set sig_set;
	sig_set.sig_add(SIGINT);
	sig_set.sig_add(SIGQUIT);
	sig_set.sig_add(SIGTERM);
	sig_set.sig_add(SIGSEGV);
	ACE_Reactor::instance()->remove_handler(sig_set);

	ACE_Reactor::instance()->end_event_loop();
	return 0;
}

//////////////////////////////////////////////////////////////////////////

Sig::Sig()
: dfe_(0)
{
	
}

Sig::~Sig()
{
	
}

int Sig::init()
{
	dfe_ = new DfEventHandler();
	set_sig_handle();

	return 0;
}

int Sig::fini()
{
	ACE_Sig_Set sig_set;
	sig_set.sig_add(SIGINT);
	sig_set.sig_add(SIGQUIT);
	sig_set.sig_add(SIGTERM);
	sig_set.sig_add(SIGSEGV);
	ACE_Reactor::instance()->remove_handler(sig_set);
	if (dfe_)
	{
		delete dfe_;
		dfe_ = 0;
	}
	return 0;
}

void Sig::set_sig_handle()
{
	ACE_Sig_Action sig(SIG_IGN, SIGPIPE);
	ACE_UNUSED_ARG(sig);
	ACE_Sig_Set sig_set;
	sig_set.sig_add (SIGINT);
	sig_set.sig_add (SIGQUIT);
	sig_set.sig_add (SIGTERM);
	sig_set.sig_add (SIGSEGV);
	ACE_Reactor::instance()->register_handler(sig_set, dfe_);
}
