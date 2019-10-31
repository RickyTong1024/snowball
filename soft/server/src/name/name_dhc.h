#ifndef __NAME_DHC_H__
#define __NAME_DHC_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "dhc.h"

class Dhc;

class NameDhc : mmg::DhcRequest
{
public:
	int init();

	int fini();

	Dhc *db_player(uint64_t guid);

	Dhc *db_social();

	virtual int do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

	virtual int do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data);

private:
	std::vector<Dhc *> dhc_players_;
	Dhc *dhc_social_;
};

#define sNameDhc (Singleton<NameDhc>::instance())
#define DB_PLAYER(guid) (sNameDhc->db_player(guid))
#define DB_SOCIAL() (sNameDhc->db_social())

#endif
