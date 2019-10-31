#ifndef __SQLQUERY2_H__
#define __SQLQUERY2_H__

#include "service_inc.h"
#include "mysql++.h"

class SqlQuery
{
public:
	SqlQuery(uint64_t guid, google::protobuf::Message *data) : guid_(guid), data_(data) {}
	virtual int insert(mysqlpp::Connection *conn) = 0;
	virtual int query(mysqlpp::Connection *conn) = 0;
	virtual int update(mysqlpp::Connection *conn) = 0;
	virtual int remove(mysqlpp::Connection *conn) = 0;

protected:
	google::protobuf::Message *data_;
	uint64_t guid_;
};

class Sqlacc_t : public SqlQuery
{
public:
	Sqlacc_t(uint64_t guid, google::protobuf::Message *data) : SqlQuery(guid, data) {}
	virtual int insert(mysqlpp::Connection *conn);
	virtual int query(mysqlpp::Connection *conn);
	virtual int update(mysqlpp::Connection *conn);
	virtual int remove(mysqlpp::Connection *conn);
};

#endif
