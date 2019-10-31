#include "scheme.h"
#include "dfa_algorith.h"
#include <ace/OS_NS_stdio.h>
#include <boost/algorithm/string.hpp>
#include <fstream>
#include <boost/lexical_cast.hpp>
#include "service.h"

Scheme::Scheme()
: dfa_(0)
, lang_(0)
{

}

Scheme::~Scheme()
{

}

int Scheme::init()
{
	lang_ = boost::lexical_cast<int>(service::server_env()->get_game_value("lang"));

	DBCFile *dbfile = get_dbc("t_lang.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); i++)
	{
		std::string key = dbfile->Get(i, 0)->pString;
		std::string value = dbfile->Get(i, lang_ + 1)->pString;
		lang_strs_[key] = value;
	}

	dfa_ = new DFA();
	if (lang_ == 0)
	{
		if (-1 == dfa_->init(this))
		{
			return -1;
		}
	}
	else
	{
		if (-1 == dfa_->init_ex())
		{
			return -1;
		}
	}

	dbfile = get_dbc("t_lang_server.txt");
	if(!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); i++)
	{
		std::string key = dbfile->Get(i, 0)->pString;
		std::string value = dbfile->Get(i, lang_ + 1)->pString;
		server_strs_[key] = value;
	}

	return 0;
}

int Scheme::fini()
{
	delete dfa_;
	dfa_ = 0;
	for (std::map<std::string, DBCFile *>::iterator it = dbcfiles_.begin(); it != dbcfiles_.end(); ++it)
	{
		DBCFile * dbc = (*it).second;
		delete dbc;
	}
	dbcfiles_.clear();
	
	return 0;
}

DBCFile * Scheme::get_dbc(const std::string &name, bool reset)
{
	DBCFile *old_dbc = 0;
	if (dbcfiles_.find(name) != dbcfiles_.end())
	{
		old_dbc = dbcfiles_[name];
		if (!reset)
			return old_dbc;
	}

	std::string content;
	if (read_file(name, content, reset) == -1)
	{
		printf ("read resource error. name: <%s>\n", name.c_str());
		return old_dbc;
	}

	DBCFile *dbc = new DBCFile(0);

	if (dbc->OpenFromMemory (content.c_str (), content.c_str () + content.size () + 1))
	{
		dbcfiles_[name] = dbc;
		if (old_dbc)
			delete old_dbc;
		return dbc;
	}
	else
	{
		delete dbc;
		printf ("parse dbc file error. name: <%s>\n", name.c_str());
		return old_dbc;
	}
	return old_dbc;
}

int Scheme::read_file(const std::string &name, std::string &res, bool reset)
{
	std::string path = service::server_env()->get_game_value("conf_path");
	if (filep_.find(name) != filep_.end())
	{
		if (!reset)
		{
			res = filep_[name];
			return 0;
		}
	}

	std::string fname = path + "/" + name;
	std::ifstream ifs(fname.c_str());
	if (!ifs.is_open())
	{
		return -1;
	}
	res.assign((std::istreambuf_iterator<char>(ifs.rdbuf())), std::istreambuf_iterator<char>());
	ifs.close();
	filep_[name] = res;

	return 0;
}

int Scheme::search_illword(const std::string& text)
{
	if (dfa_->search(text) == -1)
	{
		return -1;
	}
	bool flag = false;
	for (int i = 0; i < text.size(); ++i)	// ÊÇ·ñÎª¿Õ×Ö·û
	{
		if (text.substr(i, 1) == " " || text.substr(i, 1) == "\n")
		{
			flag = true;
			break;
		}
	}
	if (flag)
	{
		return -1;
	}
	return 0;
}

void Scheme::change_illword(std::string& text)
{
	dfa_->change(text);
}

std::string Scheme::get_server_str(const char * sstr, ...)
{
	std::string s(sstr);
	if (server_strs_.find(s) == server_strs_.end())
	{
		return "";
	}
	std::string str = server_strs_[s];

	char c[1000];
	va_list args;
	va_start(args, sstr);
	vsprintf(c, str.c_str(), args);
	va_end(args);

	str = std::string(c);
	return str;
}

std::string Scheme::get_lang_str(std::string str)
{
	if (lang_strs_.find(str) == lang_strs_.end())
	{
		return "";
	}
	return lang_strs_[str];
}
