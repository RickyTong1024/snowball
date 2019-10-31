#ifndef __SQLQUERY2_H__
#define __SQLQUERY2_H__

#include "sqlquery.h"

class Sqlpost_new_t : public SqlQuery
{
public:
	Sqlpost_new_t(uint64_t guid, google::protobuf::Message *data) : SqlQuery(guid, data) {}
	virtual int insert(mysqlpp::Connection *conn);
	virtual int query(mysqlpp::Connection *conn);
	virtual int update(mysqlpp::Connection *conn);
	virtual int remove(mysqlpp::Connection *conn);
};

class Sqlrecharge_t : public SqlQuery
{
public:
	Sqlrecharge_t(uint64_t guid, google::protobuf::Message *data) : SqlQuery(guid, data) {}
	virtual int insert(mysqlpp::Connection *conn);
	virtual int query(mysqlpp::Connection *conn);
	virtual int update(mysqlpp::Connection *conn);
	virtual int remove(mysqlpp::Connection *conn);
};

#endif
