#ifndef __HALL_DHC_H__
#define __HALL_DHC_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "dhc.h"
#include "gtool.h"

class HallDhc : mmg::DhcRequest
{
public:
	int init();

	int fini();

	GTool *db_gtool();

	Dhc *db_player(uint64_t guid);

	virtual int do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

private:
	GTool *gtool_;
	std::vector<Dhc *> dhc_players_;
};

#define sHallDhc (Singleton<HallDhc>::instance())
#define DB_GTOOL (sHallDhc->db_gtool())
#define DB_PLAYER(guid) (sHallDhc->db_player(guid))

#endif
