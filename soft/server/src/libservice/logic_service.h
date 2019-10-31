#ifndef __LOGIC_SERVICE_H__
#define __LOGIC_SERVICE_H__

#include "service_interface.h"
#include <ace/Thread_Mutex.h>

class LogicService : public mmg::LogicService
{
public:
	LogicService();

	~LogicService();

	int init(mmg::DispathService *ds);

	int fini();

	virtual int add_msg(Packet *pck);

protected:
	int update(const ACE_Time_Value &cur);

	int fetch(Packet*& pck);

	void on_filter(Packet* pck);

private:
	int timer_;
	int tick_;
	Packet *head_;
	Packet *tail_;
	Packet *header_;
	Packet *next_;
	ACE_Thread_Mutex chain_;
	mmg::DispathService *ds_;
};

#endif
