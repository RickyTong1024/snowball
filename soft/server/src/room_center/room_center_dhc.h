#ifndef __ROOM_CENTER_DHC_H__
#define __ROOM_CENTER_DHC_H__

#include "service_inc.h"
#include "protocol_inc.h"

class Dhc;
class GTool;

class RoomCenterDhc : mmg::DhcRequest
{
public:
	int init();

	int fini();

	GTool *db_gtool();

	Dhc *db_battle();

	virtual int do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

private:
	GTool *gtool_;
	Dhc * dhc_battle_;
};

#define sRoomCenterDhc (Singleton<RoomCenterDhc>::instance())
#define DB_GTOOL (sRoomCenterDhc->db_gtool())
#define DB_BATTLE (sRoomCenterDhc->db_battle())

#endif
