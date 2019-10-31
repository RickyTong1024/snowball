#ifndef __SOCIAL_POOL_H__
#define __SOCIAL_POOL_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include <queue>

struct UpdateSocial
{
	uint64_t time;
	uint64_t player_guid;
	uint64_t target_guid;

	bool operator > (const UpdateSocial &rhs) const
	{
		return time > rhs.time;
	}
};


enum SocialConst
{
	SC_UPDATE_TIME	= 3 * 24 * 60 * 60 * 1000,
};

enum SocialType
{
	ST_APPLY	= 1,
	ST_FRIEND	= 2,
	ST_BLACK	= 3,
};

enum SocialFlag
{
	SF_ONLINE	= 1 << 1,
	SF_GOLD		= 1 << 2,
	SF_FIGHT	= 1 << 3,
};

typedef boost::function<void()> LoaderCallback;

class SocialPool
{
public:
	void init();

	void fini();

	void update();

	void load(uint64_t player_guid, LoaderCallback cb);

	void create_social(const dhc::social_t &social);

	void delete_social(dhc::social_t *social);

	void delete_social(uint64_t player_guid, uint64_t target_guid);

	void delete_social(uint64_t player_guid, int type);

	void update_social(dhc::social_t *social);

	dhc::social_t* get_social(uint64_t player_guid, uint64_t target_guid);
	std::map<uint64_t, dhc::social_t*>& get_socials(uint64_t player_guid);

	bool has_social(uint64_t player_guid) const;

	int get_apply_num(uint64_t player_guid) const;

	int get_and_set_gold(uint64_t player_guid);

	int get_gold(uint64_t player_guid) const;

	void set_reject(uint64_t player_guid);

	bool is_reject(uint64_t player_guid) const;

	bool is_online(uint64_t player_guid) const;

	void set_online(uint64_t player_guid, bool online);

	bool is_fight(uint64_t player_guid) const;

	void set_fight(uint64_t player_guid, int fight);

	int get_gold_receive_num(uint64_t player_guid) const;

	void set_gold_receive_num(uint64_t player_guid);

private:
	void save_social(dhc::social_t *social, bool is_new = false);

	void save_social_list(dhc::social_list_t *social, bool is_new = false);

	bool save_player_social(uint64_t player_guid);

	void load_callback(Request *req, uint64_t player_guid, LoaderCallback cb);

	void add_update(uint64_t player_guid, uint64_t target_guid, uint64_t time);

	void init_callback(Request *req);

private:
	typedef std::map<uint64_t, dhc::social_t*> SocialList;
	typedef std::map<uint64_t, SocialList> SocialMap;
	SocialMap socials_;

	SocialList dumy_social_list_;

	std::priority_queue<UpdateSocial, std::vector<UpdateSocial>, std::greater<UpdateSocial> > update_;

	std::set<uint64_t> social_loads_;
	std::set<uint64_t> social_loads_flag_;

	dhc::social_list_t *social_lists_;
	int update_social_lists_count_;

	std::map<uint64_t, std::set<uint64_t> > save_lists_;
	int dump_save_count_;

	std::set<uint64_t> onlines_;
	std::set<uint64_t> fights_;
};

#define sSocialPool (Singleton<SocialPool>::instance())
#endif