#include "rank_dhc.h"
#include "sqlquery.h"

int RankDhc::init()
{
	gtool_ = new GTool();
	if (-1 == gtool_->init("snowball_gtool"))
	{
		return -1;
	}

	dhc_rank_ = new Dhc();
	if (-1 == dhc_rank_->init(this, "snowball_social"))
	{
		return -1;
	}
	return 0;
}

int RankDhc::fini()
{
	dhc_rank_->fini();
	delete dhc_rank_;
	gtool_->fini();
	delete gtool_;
	return 0;
}

GTool *RankDhc::db_gtool()
{
	return gtool_;
}

Dhc *RankDhc::db_rank()
{
	return dhc_rank_;
}

int RankDhc::do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_rank)
	{
		Sqlrank_t s(guid, data);
		return s.insert(conn);
	}
	return -1;
}

int RankDhc::do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_rank)
	{
		Sqlrank_t s(guid, data);
		return s.query(conn);
	}
	return -1;
}

int RankDhc::do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_rank)
	{
		Sqlrank_t s(guid, data);
		return s.update(conn);
	}
	return -1;
}

int RankDhc::do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_rank)
	{
		Sqlrank_t s(guid, data);
		return s.remove(conn);
	}
	return -1;
}
