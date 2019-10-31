#ifndef __SOCIAL_DHC_H__
#define __SOCIAL_DHC_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "dhc.h"
#include "gtool.h"

class SocialDhc : mmg::DhcRequest
{
public:
	int init();

	int fini();

	GTool *db_gtool();

	Dhc *db_social();

	virtual int do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

private:
	GTool *gtool_;
	Dhc * dhc_social_;
};

#define sSocialDhc (Singleton<SocialDhc>::instance())
#define DB_GTOOL (sSocialDhc->db_gtool())
#define DB_SOCIAL (sSocialDhc->db_social())

#endif
