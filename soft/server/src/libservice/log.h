#ifndef __LOG_H__
#define __LOG_H__

#include "service_interface.h"

class Log : public mmg::Log
{
public:
	Log();

	~Log();

	int init(const std::string &name);

	int fini();

	virtual void enable_file(const std::string &file);

	virtual int debug(const char *text, ...);

	virtual int error(const char *text, ...);

protected:
	std::string get_date();
	
private:
	int log_level_;
	std::string name_;
};

#endif
