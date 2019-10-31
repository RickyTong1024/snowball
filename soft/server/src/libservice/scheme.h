#ifndef __SCHEME_H__
#define __SCHEME_H__

#include "service_interface.h"
#include "dbc.h"

class DFA;

class Scheme : public mmg::Scheme
{
public:
	Scheme();

	~Scheme();

	virtual int init();

	virtual int fini();

	virtual DBCFile * get_dbc(const std::string &name, bool reset = false);

	virtual int read_file(const std::string &name, std::string &res, bool reset = false);

	virtual int search_illword(const std::string& text);

	virtual void change_illword(std::string& text);

	virtual std::string  get_server_str(const char * sstr, ...);

	virtual std::string get_lang_str(std::string str);

private:
	int lang_;
	DFA *dfa_;
	std::map<std::string, DBCFile *> dbcfiles_;
	std::map<std::string, std::string> filep_;
	std::map<std::string, std::string> server_strs_;
	std::map<std::string, std::string> lang_strs_;
};

#endif
