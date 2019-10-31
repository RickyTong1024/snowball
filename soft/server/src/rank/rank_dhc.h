#ifndef __RANK_DHC_H__
#define __RANK_DHC_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "gtool.h"
#include "dhc.h"

class RankDhc : mmg::DhcRequest
{
public:
	int init();

	int fini();

	GTool *db_gtool();

	Dhc *db_rank();

	virtual int do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

private:
	GTool *gtool_;
	Dhc * dhc_rank_;
};

#define sRankDhc (Singleton<RankDhc>::instance())
#define DB_GTOOL (sRankDhc->db_gtool())
#define DB_RANK (sRankDhc->db_rank())

#endif
