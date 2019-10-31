#include "timer.h"
#include <ace/Reactor.h>

CB::CB(Callback task, const std::string &name)
: task_(task)
, name_(name)
{

}

void CB::set_id(int id)
{
	id_ = id;
}

int CB::get_id()
{
	return id_;
}

int CB::handle_timeout (const ACE_Time_Value &tv, const void *arg)
{
	int nRet = task_(tv);
	if(nRet == -1)
		return -1;
	return 0;
}

//////////////////////////////////////////////////////////////////////////

Timer::Timer()
: stop_(true)
{
	
}

Timer::~Timer()
{
	
}

int Timer::init()
{
	timer_queue_ = new ACE_Timer_Heap;
	return 0;
}

int Timer::start()
{
	stop_ = false;
	this->activate();
	return 0;
}

int Timer::stop()
{
	stop_ = true;
	this->wait();
	return 0;
}

int Timer::fini()
{
	std::map<int, CB *>::iterator it = CB_map_.begin();
	while (it != CB_map_.end())
	{
		CB *cb = (*it).second;
		timer_queue_->cancel(cb->get_id());
		delete cb;
		CB_map_.erase(it++);
	}

	if (timer_queue_)
	{
		delete timer_queue_;
		timer_queue_ = 0;
	}

	return 0;
}

int Timer::schedule(Callback task, int expire, const std::string &name)
{
	CB *cb = new CB(task, name);
	ACE_Time_Value tv(0, expire * 1000);
	int id = this->timer_queue_->schedule(cb, name.c_str(), ACE_Time_Value::zero, tv);
	if (id == -1)
	{
		delete cb;
		return -1;
	}
	cb->set_id(id);
	CB_map_[id] = cb;
	return id;
}

int Timer::cancel(int expiry_id)
{
	int num = timer_queue_->cancel(expiry_id);
	if (num != 0)
	{
		CB *cb = CB_map_[expiry_id];
		CB_map_.erase(expiry_id);
		delete cb;
		return 0;
	}
	return -1;
}

uint64_t Timer::now(void)
{
	ACE_UINT64 msec;
	timer_queue_->gettimeofday().msec(msec);
	return msec;
}

int Timer::hour(void)
{
	ACE_Date_Time new_dt;

	return new_dt.hour();
}

int Timer::weekday(void)
{
	ACE_Date_Time new_dt;

	int nw = new_dt.weekday();
	if (nw == 0)
	{
		nw = 7;
	}
	return nw;
}

int Timer::day(void)
{
	ACE_Date_Time new_dt;

	return new_dt.day();
}

int Timer::month(void)
{
	ACE_Date_Time new_dt;

	return new_dt.month();
}

bool Timer::trigger_time(uint64_t old_time, int hour, int minute)
{
	ACE_Time_Value old_tv;
	old_tv.set_msec(old_time);
	ACE_Date_Time old_dt(old_tv);
	uint64_t new_time = now();
	ACE_Time_Value new_tv;
	new_tv.set_msec(new_time);
	ACE_Date_Time new_dt(new_tv);
	
	if (new_time <= old_time)
	{
		return false;
	}

	if (new_time - old_time >= 86400000)
	{
		return true;
	}

	bool old_small = is_small(old_dt, hour, minute);
	bool new_small = is_small(new_dt, hour, minute);

	if (is_same_day(old_dt, new_dt))
	{
		if (old_small && !new_small)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	else
	{
		if (!old_small && !new_small)
		{
			return true;
		}
		else if (old_small && new_small)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	return false;
}

bool Timer::trigger_week_time(uint64_t old_time)
{
	ACE_Time_Value old_tv;
	old_tv.set_msec(old_time);
	ACE_Date_Time old_dt(old_tv);
	uint64_t new_time = now();
	ACE_Time_Value new_tv;
	new_tv.set_msec(new_time);
	ACE_Date_Time new_dt(new_tv);

	if (new_time <= old_time)
	{
		return false;
	}

	if (new_time - old_time >= 86400000 * 7)
	{
		return true;
	}

	int nw = new_dt.weekday();
	if (nw == 0)
	{
		nw = 7;
	}
	int ow = old_dt.weekday();
	if (ow == 0)
	{
		ow = 7;
	}

	if (nw < ow)
	{
		return true;
	}
	else if (new_dt.weekday() == old_dt.weekday())
	{
		if (new_time - old_time >= 86400000 * 6)
		{
			return true;
		}
	}

	return false;
}

bool Timer::trigger_month_time(uint64_t old_time)
{
	ACE_Time_Value old_tv;
	old_tv.set_msec(old_time);
	ACE_Date_Time old_dt(old_tv);
	uint64_t new_time = now();
	ACE_Time_Value new_tv;
	new_tv.set_msec(new_time);
	ACE_Date_Time new_dt(new_tv);

	if (new_time <= old_time)
	{
		return false;
	}

	if (new_time - old_time >= uint64_t(86400000) * 31)
	{
		return true;
	}

	int nw = new_dt.month();
	int ow = old_dt.month();

	if (nw != ow)
	{
		return true;
	}

	return false;
}

int Timer::run_day(uint64_t old_time)
{
	uint64_t now_time = now();
	if (old_time >= now_time)
	{
		return 0;
	}
	uint64_t delta_time = now_time - old_time;
	int day_num = delta_time / 86400000;
	uint64_t ltime = old_time + day_num * 86400000;
	if (trigger_time(ltime, 0, 0))
	{
		day_num++;
	}
	return day_num;
}

bool Timer::is_same_day(ACE_Date_Time &old_dt, ACE_Date_Time &new_dt)
{
	if (old_dt.year() != new_dt.year())
	{
		return false;
	}
	else if (old_dt.month() != new_dt.month())
	{
		return false;
	}
	else if (old_dt.day() != new_dt.day())
	{
		return false;
	}
	return true;
}

bool Timer::is_small(ACE_Date_Time &dt, int hour, int minute)
{
	if (dt.hour() < hour)
	{
		return true;
	}
	else if (dt.hour() == hour)
	{
		if (dt.minute() < minute)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	return false;
}

int Timer::svc()
{
	while (!stop_)
	{
		if (this->timer_queue_->is_empty())
		{
			ACE_Time_Value next_timeout = timer_queue_->gettimeofday();
			next_timeout += ACE_Time_Value(0, 200 * 1000);
			if (-1 == this->timer_.wait(&next_timeout))
			{
				this->timer_queue_->expire();
			}
		}
		else
		{
			ACE_Time_Value max_tv = ACE_Time_Value(0, 200 * 1000);

			ACE_Time_Value *this_timeout = this->timer_queue_->calculate_timeout(&max_tv);
			if (*this_timeout == ACE_Time_Value::zero)
			{
				this->timer_queue_->expire();
			}
			else
			{
				ACE_Time_Value next_timeout = timer_queue_->gettimeofday();
				next_timeout += *this_timeout;
				if (-1 == this->timer_.wait(&next_timeout))
				{
					this->timer_queue_->expire();
				}
			}
		}
	}

	return 0;
}
