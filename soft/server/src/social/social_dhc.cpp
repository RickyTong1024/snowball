#include "social_dhc.h"
#include "sqlquery.h"
#include "sqlquery2.h"

int SocialDhc::init()
{
	gtool_ = new GTool();
	if (-1 == gtool_->init("snowball_gtool"))
	{
		return -1;
	}

	dhc_social_ = new Dhc();
	if (-1 == dhc_social_->init(this, "snowball_social"))
	{
		return -1;
	}
	return 0;
}

int SocialDhc::fini()
{
	dhc_social_->fini();
	delete dhc_social_;
	gtool_->fini();
	delete gtool_;
	return 0;
}

GTool *SocialDhc::db_gtool()
{
	return gtool_;
}

Dhc *SocialDhc::db_social()
{
	return dhc_social_;
}

int SocialDhc::do_insert(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_social)
	{
		Sqlsocial_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_post)
	{
		SqlSocialRemove_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_social_list)
	{
		Sqlsocial_list_t s(guid, data);
		return s.insert(conn);
	}
	if (type_of_guid(guid) == et_player)
	{
		SqlPlayerSocial_t s(guid, data);
		return s.insert(conn);
	}
	return -1;
}

int SocialDhc::do_query(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_social)
	{
		Sqlsocial_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_post)
	{
		SqlSocialRemove_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_social_list)
	{
		Sqlsocial_list_t s(guid, data);
		return s.query(conn);
	}
	if (type_of_guid(guid) == et_player)
	{
		SqlPlayerSocial_t s(guid, data);
		return s.query(conn);
	}
	return -1;
}

int SocialDhc::do_update(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_social)
	{
		Sqlsocial_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_post)
	{
		SqlSocialRemove_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_social_list)
	{
		Sqlsocial_list_t s(guid, data);
		return s.update(conn);
	}
	if (type_of_guid(guid) == et_player)
	{
		SqlPlayerSocial_t s(guid, data);
		return s.update(conn);
	}
	return -1;
}

int SocialDhc::do_remove(mysqlpp::Connection *conn, uint64_t guid, google::protobuf::Message *data)
{
	if (type_of_guid(guid) == et_social)
	{
		Sqlsocial_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_post)
	{
		SqlSocialRemove_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_social_list)
	{
		Sqlsocial_list_t s(guid, data);
		return s.remove(conn);
	}
	if (type_of_guid(guid) == et_player)
	{
		SqlPlayerSocial_t s(guid, data);
		return s.remove(conn);
	}
	return -1;
}
