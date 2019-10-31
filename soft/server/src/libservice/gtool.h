#ifndef __GTOOL_H__
#define __GTOOL_H__

#include "service_interface.h"

class Dhc;

namespace mysqlpp
{
	class Connection;
}

class GTool : mmg::DhcRequest
{
public:
	GTool();

	~GTool();

	int init(const std::string &db);

	int fini();

	virtual uint64_t assign (int et);

	virtual uint64_t start_time();

	virtual int do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

protected:
	int get_guid();

	void get_start_time();

private:
	int qid_;
	int one_add_;
	uint64_t cur_guid_;
	uint64_t next_guid_;
	uint64_t start_time_;
	Dhc *dhc_;
};

#endif
