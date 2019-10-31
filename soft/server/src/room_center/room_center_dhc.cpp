#include "room_center_dhc.h"
#include "dhc.h"
#include "gtool.h"
#include "sqlquery.h"

int RoomCenterDhc::init()
{
	gtool_ = new GTool();
	if (-1 == gtool_->init("snowball_gtool"))
	{
		return -1;
	}

	dhc_battle_ = new Dhc();
	if (-1 == dhc_battle_->init(this, "snowball_battle"))
	{
		return -1;
	}
	return 0;
}

int RoomCenterDhc::fini()
{
	dhc_battle_->fini();
	delete dhc_battle_;
	gtool_->fini();
	delete gtool_;
	return 0;
}

GTool *RoomCenterDhc::db_gtool()
{
	return gtool_;
}

Dhc *RoomCenterDhc::db_battle()
{
	return dhc_battle_;
}

int RoomCenterDhc::do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_battle_result)
	{
		Sqlbattle_result_t s(guid, data);
		return s.insert(conn);
	}
	return -1;
}

int RoomCenterDhc::do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_battle_result)
	{
		Sqlbattle_result_t s(guid, data);
		return s.query(conn);
	}
	return -1;
}

int RoomCenterDhc::do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_battle_result)
	{
		Sqlbattle_result_t s(guid, data);
		return s.update(conn);
	}
	return -1;
}

int RoomCenterDhc::do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_battle_result)
	{
		Sqlbattle_result_t s(guid, data);
		return s.remove(conn);
	}
	return -1;
}
