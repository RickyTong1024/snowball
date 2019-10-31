#ifndef __LOGIN_DHC_H__
#define __LOGIN_DHC_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "dhc.h"

class Dhc;

class LoginDhc : mmg::DhcRequest
{
public:
	int init();

	int fini();

	Dhc *db_account();

	virtual int do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

private:
	Dhc *dhc_account_;
};

#define sLoginDhc (Singleton<LoginDhc>::instance())
#define DB_ACCOUNT (sLoginDhc->db_account())

#endif
