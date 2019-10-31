#include "login_dhc.h"
#include "sqlquery2.h"

int LoginDhc::init()
{
	dhc_account_ = new Dhc();
	if (-1 == dhc_account_->init(this, "snowball_account"))
	{
		return -1;
	}
	return 0;
}

int LoginDhc::fini()
{
	dhc_account_->fini();
	delete dhc_account_;
	return 0;
}

Dhc *LoginDhc::db_account()
{
	return dhc_account_;
}

int LoginDhc::do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_acc)
	{
		Sqlacc_t s(guid, data);
		return s.insert(conn);
	}
	return -1;
}

int LoginDhc::do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_acc)
	{
		Sqlacc_t s(guid, data);
		return s.query(conn);
	}
	return -1;
}

int LoginDhc::do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_acc)
	{
		Sqlacc_t s(guid, data);
		return s.update(conn);
	}
	return -1;
}

int LoginDhc::do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_acc)
	{
		Sqlacc_t s(guid, data);
		return s.remove(conn);
	}
	return -1;
}
