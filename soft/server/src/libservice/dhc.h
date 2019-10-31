#ifndef __DHC_H__
#define __DHC_H__

#include <ace/Task.h>
#include "service_interface.h"

class Request;

namespace mysqlpp
{
	class Connection;
}

class Dhc : public ACE_Task<ACE_NULL_SYNCH>
{
public:
	Dhc();

	~Dhc();

	int init(mmg::DhcRequest *dr, const std::string &name, bool async = true);

	int fini();

	int do_request(Request *req);

	bool full();

	int upcall(Request *req, Upcaller caller);

	virtual int svc();

protected:
	int upcall_svc();

	int update(const ACE_Time_Value &cur);

	int doupcall();

private:
	mysqlpp::Connection *conn_;
	mmg::DhcRequest *dr_;
	bool stop_;
	int timer_id_;
	std::list< std::pair<Request *, Upcaller> > upcaller_;
	ACE_Thread_Mutex chain_;
	std::list< std::pair<Request *, Upcaller> > docaller_;
	ACE_Thread_Mutex chain1_;
	bool async_;
};

#endif
