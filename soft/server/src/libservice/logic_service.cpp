#include "logic_service.h"
#include "service.h"
#include <ace/Guard_T.h>

LogicService::LogicService()
: timer_(-1)
, tick_(0)
, head_(0)
, tail_(0)
, header_(0)
, next_(0)
, ds_(0)
{
}

LogicService::~LogicService()
{
}

int LogicService::init(mmg::DispathService *ds)
{
	ds_ = ds;
	tick_ = boost::lexical_cast<int>(service::server_env()->get_game_value("tick"));
	timer_ = service::timer()->schedule(boost::bind(&LogicService::update, this, _1), tick_, "com");

	return 0;
}

int LogicService::fini()
{
	if (-1 != timer_)
	{
		service::timer()->cancel(timer_);
		timer_ = -1;
	}
	return 0;
}

int LogicService::add_msg(Packet *pck)
{
	ACE_Guard<ACE_Thread_Mutex> t(chain_);

	if (head_ == NULL)
	{
		tail_ = head_ = pck;
	}
	else
	{
		tail_ = tail_->add_tail(pck);
	}

	return 0;
}

int LogicService::update(const ACE_Time_Value &cur)
{
	uint64_t n1 = service::timer()->now();

	while (service::timer()->now() - n1 < tick_)
	{
		if (!next_)
		{
			if (header_)
			{
				Packet::clear(header_);
			}
			if (fetch (header_) == -1)
			{
				break;
			}
			next_ = header_;
		}
		on_filter(next_);
		next_ = next_->next ();	
	}

	return 0;
}

int LogicService::fetch(Packet*& pck)
{
	ACE_Guard<ACE_Thread_Mutex> t(chain_);

	pck = head_;
	tail_ = head_ = NULL;

	return pck == NULL ? -1 : 0;
}

void LogicService::on_filter(Packet* pck)
{
	if (!ds_)
	{
		return;
	}
	ds_->dispath_packet_handle(pck);
}
