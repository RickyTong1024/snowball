#ifndef __TIMER_H__
#define __TIMER_H__

#include <ace/Task.h>
#include <ace/Timer_Queue.h>
#include <ace/Timer_Heap.h>
#include <ace/Event.h>
#include <ace/Date_Time.h>
#include "service_interface.h"

class CB : public ACE_Event_Handler
{
public:
	CB(Callback task, const std::string &name);

	void set_id(int id);

	int get_id();

	virtual int handle_timeout (const ACE_Time_Value &tv, const void *arg);

private:
	int id_;
	std::string name_;
	Callback task_;
};

class Timer : public ACE_Task<ACE_NULL_SYNCH>, public mmg::Timer
{
public:
	Timer();

	~Timer();

	int init();

	int start();

	int stop();

	int fini();

	virtual int schedule(Callback task, int expire, const std::string &name);

	virtual int cancel(int expiry_id);

	virtual uint64_t now(void);

	virtual int hour(void);

	virtual int weekday(void);

	virtual int day(void);

	virtual int month(void);

	virtual bool trigger_time(uint64_t old_time, int hour, int minute);

	virtual bool trigger_week_time(uint64_t old_time);

	virtual bool trigger_month_time(uint64_t old_time);

	virtual int run_day(uint64_t old_time);

	virtual int svc();

protected:
	bool is_same_day(ACE_Date_Time &old_dt, ACE_Date_Time &new_dt);

	bool is_small(ACE_Date_Time &dt, int hour, int minute);

private:
	ACE_Timer_Queue *timer_queue_;
	ACE_Event timer_;
	std::map<int, CB *> CB_map_;
	bool stop_;
};

#endif
