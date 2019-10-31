#ifndef __ROLE_CONFIG_H__
#define __ROLE_CONFIG_H__

#include "service_inc.h"
#include "protocol_inc.h"
#include "reward.h"

struct s_t_role
{
	int id;
	std::string name;
	int font_color;
	int sex;
	std::vector<int> guanghuan;
	int suipian_id;
	int suipian_num;
	int exp;
	int gz;
};

struct s_t_skill
{
	int id;
	int type;
	int level;
};

struct s_t_role_dress
{
	int id;
	int role;
	int glevel;
};

struct s_t_role_level
{
	int level;
	int card;
	std::vector<int> gold;
};

struct s_t_role_buff
{
	int id;
	s_t_attr attr;
};

class RoleConfig
{
public:
	int parse();

	s_t_role * get_role(int id);

	s_t_skill *get_skill(int id, int level);

	s_t_role_level *get_role_level(int level);

	s_t_role_buff * get_role_buff(int id);

private:
	std::map<int, s_t_role> t_roles_;
	std::map<int, std::map<int, s_t_skill> > t_skill_;
	std::map<int, s_t_role_level> t_role_level_;
	std::map<int, s_t_role_buff> t_role_buff_;
};

#define sRoleConfig (Singleton<RoleConfig>::instance ())

#endif
