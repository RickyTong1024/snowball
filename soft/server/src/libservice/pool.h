#ifndef __POOL_H__
#define __POOL_H__

#include <ace/Task.h>
#include "service_interface.h"
#include <ace/Thread_Mutex.h>

class Pool : public mmg::Pool
{
public:

	Pool();

	~Pool();

	int init();

	int fini();

	virtual int add (uint64_t guid, google::protobuf::Message *entity, estatus es);

	virtual int remove(uint64_t guid, uint64_t ref_guid);

	virtual google::protobuf::Message * release (uint64_t guid);

	virtual google::protobuf::Message * get (uint64_t guid);

	virtual estatus get_state (uint64_t guid);

	virtual void set_state (uint64_t guid, estatus es);

	virtual void get_entitys (int type, std::vector<uint64_t> &guids);

	virtual void release_ref(uint64_t ref_guid, std::vector<uint64_t> &entitys);

private:
	struct Object
	{
		google::protobuf::Message *entity;
		mmg::Pool::estatus es;
	};

	std::map< int, std::map<uint64_t, Object> > entity_map_;
	std::map< uint64_t, std::vector<uint64_t> > ref_map_;
};

#endif
