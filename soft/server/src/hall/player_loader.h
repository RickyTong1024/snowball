#ifndef __PLAYER_LOADER_H__
#define __PLAYER_LOADER_H__

#include "service_inc.h"
#include "protocol_inc.h"

typedef boost::function<void()> LoaderCallback;

struct LoadMap
{
	bool need_smsg;
	int query_num;
	std::vector<LoaderCallback> callbacks;

	LoadMap() : need_smsg(false), query_num(0) {}
};

struct UnloadMap
{
	int query_num;
	std::vector<LoaderCallback> callbacks;

	UnloadMap() : query_num(0) {}
};

class PlayerLoader
{
public:
	int load_player(uint64_t player_guid);

	int load_player(uint64_t player_guid, LoaderCallback callback);

	void save_player(uint64_t guid, bool release, Upcaller upcall = 0);

	void save_all();

protected:
	void load_msg_callback(Request *req, dhc::player_t *player);

	void load_player_callback(Request *req);

	void load_player_check_end(dhc::player_t *player);

	dhc::player_t * create_player(uint64_t player_guid, int serverid);

	template <typename otype>
	int save(uint64_t player_guid, uint64_t guid, google::protobuf::Message *object, bool release, Upcaller caller);

private:
	std::map<uint64_t, LoadMap> load_map_;
	std::map<uint64_t, UnloadMap> unload_map_;
};

#define sPlayerLoader (Singleton<PlayerLoader>::instance())

#endif
