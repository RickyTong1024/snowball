#include "server_env.h"
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/json_parser.hpp>
#include <boost/typeof/typeof.hpp> 

ServerEnv::ServerEnv()
{
	
}

ServerEnv::~ServerEnv()
{

}

int ServerEnv::init(const std::string &name, const std::string &confpath, const std::string &self)
{
	boost::property_tree::ptree pt;
	boost::property_tree::read_json(confpath + "/server.json", pt);

	{
		boost::property_tree::ptree pt_servers = pt.get_child("server");
		for (boost::property_tree::ptree::iterator it = pt_servers.begin(); it != pt_servers.end(); ++it)
		{
			std::string sname = (*it).first;
			server_kinds_.push_back(sname);
			boost::property_tree::ptree pt_ch = (*it).second;
			for (boost::property_tree::ptree::iterator jt = pt_ch.begin(); jt != pt_ch.end(); ++jt)
			{
				boost::property_tree::ptree pt_ch_ch = (*jt).second;
				std::string id = pt_ch_ch.get<std::string>("id");
				server_names_[sname].push_back(id);
				for (BOOST_AUTO(pos, pt_ch_ch.begin()); pos != pt_ch_ch.end(); ++pos)
				{
					server_value_[id][(*pos).first.data()] = (*pos).second.data();
				}
			}
		}
		if (self != "")
		{
			std::vector<std::string> dest;
			split(self, " ", dest);
			server_kinds_.push_back(dest[0]);
			server_value_[name]["id"] = name;
			server_value_[name]["host"] = dest[1];
			server_value_[name]["port"] = dest[2];
			server_value_[name]["udp_host"] = dest[3];
			server_value_[name]["udp_port"] = dest[4];
			server_value_[name]["tcp_host"] = dest[5];
			server_value_[name]["tcp_port"] = dest[6];
		}
	}

	{
		boost::property_tree::ptree pt_dbs = pt.get_child("db");
		for (boost::property_tree::ptree::iterator it = pt_dbs.begin(); it != pt_dbs.end(); ++it)
		{
			std::string sname = (*it).first;
			db_kinds_.push_back(sname);
			boost::property_tree::ptree pt_ch = (*it).second;
			for (boost::property_tree::ptree::iterator jt = pt_ch.begin(); jt != pt_ch.end(); ++jt)
			{
				boost::property_tree::ptree pt_ch_ch = (*jt).second;
				std::string id = pt_ch_ch.get<std::string>("id");
				db_names_[sname].push_back(id);
				for (BOOST_AUTO(pos, pt_ch_ch.begin()); pos != pt_ch_ch.end(); ++pos)
				{
					db_value_[id][(*pos).first.data()] = (*pos).second.data();
				}
			}
		}
	}

	boost::property_tree::ptree pt1;
	boost::property_tree::read_json(confpath + "/game.json", pt1);

	{
		for (BOOST_AUTO(pos, pt1.begin()); pos != pt1.end(); ++pos)
		{
			game_value_[(*pos).first.data()] = (*pos).second.data();
		}
	}

	return 0;
}

int ServerEnv::fini()
{
	return 0;
}

std::string ServerEnv::get_server_value(const std::string &name, const std::string &key)
{
	if (server_value_.find(name) != server_value_.end())
	{
		if (server_value_[name].find(key) != server_value_[name].end())
		{
			return server_value_[name][key];
		}
	}
	return "";
}

void ServerEnv::get_server_names(const std::string &kind, std::vector<std::string> &names)
{
	if (server_names_.find(kind) != server_names_.end())
	{
		names = server_names_[kind];
	}
}

void ServerEnv::get_server_kinds(std::vector<std::string> &kinds)
{
	kinds = server_kinds_;
}

std::string ServerEnv::get_db_value(const std::string &name, const std::string &key)
{
	if (db_value_.find(name) != db_value_.end())
	{
		if (db_value_[name].find(key) != db_value_[name].end())
		{
			return db_value_[name][key];
		}
	}
	return "";
}

void ServerEnv::get_db_names(const std::string &kind, std::vector<std::string> &names)
{
	if (db_names_.find(kind) != db_names_.end())
	{
		names = db_names_[kind];
	}
}

void ServerEnv::get_db_kinds(std::vector<std::string> &kinds)
{
	kinds = db_kinds_;
}

std::string ServerEnv::get_game_value(const std::string &key)
{
	if (game_value_.find(key) != game_value_.end())
	{
		return game_value_[key];
	}
	return "";
}

void ServerEnv::split(const std::string &src, const std::string &separator, std::vector<std::string> &dest)
{
	std::string str = src;
	std::string substring;
	std::string::size_type start = 0, index;

	do
	{
		index = str.find_first_of(separator, start);
		if (index != std::string::npos)
		{
			substring = str.substr(start, index - start);
			dest.push_back(substring);
			start = str.find_first_not_of(separator, index);
			if (start == std::string::npos) return;
		}
	} while (index != std::string::npos);

	//the last token
	substring = str.substr(start);
	dest.push_back(substring);
}
