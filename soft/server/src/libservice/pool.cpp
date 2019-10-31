#include "pool.h"
#include <ace/OS_NS_unistd.h>
#include "service_inc.h"

Pool::Pool()
{

}

Pool::~Pool()
{
	
}

int Pool::init()
{
	return 0;
}

int Pool::fini()
{
	std::map< int, std::map<uint64_t, Object> >::iterator it = entity_map_.begin();
	while (it != entity_map_.end())
	{
		std::map<uint64_t, Object> &entity_type_map = (*it).second;
		std::map<uint64_t, Object>::iterator jt = entity_type_map.begin();
		while (jt != entity_type_map.end())
		{
			google::protobuf::Message *entity = (*jt).second.entity;
			delete entity;
			entity_type_map.erase(jt++);
		}
		entity_map_.erase(it++);
	}
	return 0;
}

int Pool::add (uint64_t guid, google::protobuf::Message *entity, estatus es)
{
	int type = type_of_guid(guid);
	std::map<uint64_t, Object> &entity_type_map = entity_map_[type];
	std::map<uint64_t, Object>::iterator it = entity_type_map.find(guid);
	if (it == entity_type_map.end())
	{
		if (es == state_none)
		{
			entity->clear_changed();
		}
		Object ob;
		ob.entity = entity;
		ob.es = es;
		entity_type_map[guid] = ob;
		return 0;
	}
	return -1;
}

int Pool::remove (uint64_t guid, uint64_t ref_guid)
{
	int type = type_of_guid(guid);
	std::map< int, std::map<uint64_t, Object> >::iterator jt = entity_map_.find(type);
	if (jt != entity_map_.end())
	{
		std::map<uint64_t, Object> &entity_type_map = (*jt).second;
		std::map<uint64_t, Object>::iterator it = entity_type_map.find(guid);
		if (it != entity_type_map.end())
		{
			google::protobuf::Message *entity = (*it).second.entity;
			delete entity;
			entity_type_map.erase(it);
			if (ref_guid != 0)
			{
				ref_map_[ref_guid].push_back(guid);
			}
			return 0;
		}
	}
	return -1;
}

google::protobuf::Message * Pool::release (uint64_t guid)
{
	int type = type_of_guid(guid);
	std::map< int, std::map<uint64_t, Object> >::iterator jt = entity_map_.find(type);
	if (jt != entity_map_.end())
	{
		std::map<uint64_t, Object> &entity_type_map = (*jt).second;
		std::map<uint64_t, Object>::iterator it = entity_type_map.find(guid);
		if (it != entity_type_map.end())
		{
			google::protobuf::Message *entity = (*it).second.entity;
			entity_type_map.erase(it);
			return entity;
		}
	}
	return 0;
}

google::protobuf::Message * Pool::get (uint64_t guid)
{
	int type = type_of_guid(guid);
	std::map< int, std::map<uint64_t, Object> >::iterator jt = entity_map_.find(type);
	if (jt != entity_map_.end())
	{
		std::map<uint64_t, Object> &entity_type_map = (*jt).second;
		std::map<uint64_t, Object>::iterator it = entity_type_map.find(guid);
		if (it != entity_type_map.end())
		{
			return (*it).second.entity;
		}
	}
	return 0;
}

mmg::Pool::estatus Pool::get_state (uint64_t guid)
{
	int type = type_of_guid(guid);
	std::map< int, std::map<uint64_t, Object> >::iterator jt = entity_map_.find(type);
	if (jt != entity_map_.end())
	{
		std::map<uint64_t, Object> &entity_type_map = (*jt).second;
		std::map<uint64_t, Object>::iterator it = entity_type_map.find(guid);
		if (it != entity_type_map.end())
		{
			return (*it).second.es;
		}
	}
	return mmg::Pool::state_none;
}

void Pool::set_state (uint64_t guid, estatus es)
{
	int type = type_of_guid(guid);
	std::map< int, std::map<uint64_t, Object> >::iterator jt = entity_map_.find(type);
	if (jt != entity_map_.end())
	{
		std::map<uint64_t, Object> &entity_type_map = (*jt).second;
		std::map<uint64_t, Object>::iterator it = entity_type_map.find(guid);
		if (it != entity_type_map.end())
		{
			(*it).second.es = es;
		}
	}
}

void Pool::get_entitys (int type, std::vector<uint64_t> &guids)
{
	std::map< int, std::map<uint64_t, Object> >::iterator jt = entity_map_.find(type);
	if (jt != entity_map_.end())
	{
		std::map<uint64_t, Object> &entity_type_map = (*jt).second;
		for (std::map<uint64_t, Object>::iterator it = entity_type_map.begin(); it != entity_type_map.end(); ++it)
		{
			guids.push_back((*it).first);
		}
	}
}

void Pool::release_ref(uint64_t ref_guid, std::vector<uint64_t> &entitys)
{
	if (ref_map_.find(ref_guid) == ref_map_.end())
	{
		return;
	}
	std::vector<uint64_t> &es = ref_map_[ref_guid];
	for (int i = 0; i < es.size(); ++i)
	{
		entitys.push_back(es[i]);
	}
	ref_map_.erase(ref_guid);
}
