#include "log.h"
#include "service.h"
#include "ace/Log_Msg.h"
#include <boost/date_time/posix_time/posix_time.hpp>
#include <boost/format.hpp>
#include <fstream>

Log::Log()
: log_level_(0)
{

}

Log::~Log()
{

}

int Log::init(const std::string &name)
{
	name_ = name;
	std::string ll = service::server_env()->get_game_value("log");
	if (ll == "debug")
	{
		log_level_ = 0;
	}
	else
	{
		log_level_ = 1;
	}
	
	return 0;
}

int Log::fini()
{
	return 0;
}

void Log::enable_file(const std::string &file)
{
	ACE_OSTREAM_TYPE *output = new std::ofstream(file.c_str(), std::ios::app|std::ios::out);
	ACE_LOG_MSG->msg_ostream(output, 1);
	ACE_LOG_MSG->set_flags(ACE_Log_Msg::OSTREAM);
	ACE_LOG_MSG->clr_flags(ACE_Log_Msg::STDERR | ACE_Log_Msg::LOGGER);
}

int Log::debug(const char *text, ...)
{
	if (log_level_ > 0)
	{
		return -1;
	}

	char c[1000];
	va_list args;
	va_start(args, text);
	vsprintf(c, text, args);
	va_end(args);
	std::string t = name_ + " " + get_date() + " debug: " + std::string(c) + "\n";
	ACE_DEBUG((LM_INFO, ACE_TEXT(t.c_str())));
	return 0;
}

int Log::error(const char *text, ...)
{
	char c[1000];
	va_list args;
	va_start(args, text);
	vsprintf(c, text, args);
	va_end(args);
	std::string t = name_ + " " + get_date() + " error: " + std::string(c) + "\n";
	ACE_ERROR((LM_INFO, ACE_TEXT(t.c_str())));
	return 0;
}

std::string Log::get_date()
{
	std::string tm = boost::posix_time::to_iso_extended_string(boost::posix_time::second_clock::local_time());

	int pos = tm.find('T');
	tm.replace(pos, 1, std::string("-"));

	return tm;
}
