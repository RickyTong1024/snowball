#include "role_config.h"
#include "dbc.h"

int RoleConfig::parse()
{
	DBCFile *dbfile = service::scheme()->get_dbc("t_role.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_role t_role;
		t_role.id = dbfile->Get(i, 0)->iValue;
		t_role.name = dbfile->Get(i, 2)->pString;
		t_role.name = service::scheme()->get_lang_str(t_role.name);
		t_role.font_color = dbfile->Get(i, 3)->iValue;
		t_role.sex = dbfile->Get(i, 4)->iValue;
		for (int j = 0; j < 3; ++j)
		{
			int gh = dbfile->Get(i, 21 + j)->iValue;
			if (gh > 0)
			{
				t_role.guanghuan.push_back(gh);
			}
		}
		t_role.suipian_id = dbfile->Get(i, 27)->iValue;
		t_role.suipian_num = dbfile->Get(i, 28)->iValue;
		t_role.gz = dbfile->Get(i, 30)->iValue;
		
		t_roles_[t_role.id] = t_role;
	}

	dbfile = service::scheme()->get_dbc("t_skill.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_skill t_skill;
		t_skill.id = dbfile->Get(i, 0)->iValue;
		t_skill.level = dbfile->Get(i, 3)->iValue;
		t_skill.type = dbfile->Get(i, 4)->iValue;

		t_skill_[t_skill.id][t_skill.level] = t_skill;
	}

	dbfile = service::scheme()->get_dbc("t_role_level.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_role_level t_role_level;
		t_role_level.level = dbfile->Get(i, 0)->iValue;
		t_role_level.card = dbfile->Get(i, 1)->iValue;
		for (int j = 0; j < 3; ++j)
		{
			int gold = dbfile->Get(i, 2 + j)->iValue;
			t_role_level.gold.push_back(gold);
		}

		t_role_level_[t_role_level.level] = t_role_level;
	}

	dbfile = service::scheme()->get_dbc("t_role_buff.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_role_buff t_role_buff;
		t_role_buff.id = dbfile->Get(i, 0)->iValue;
		t_role_buff.attr.type = dbfile->Get(i, 5)->iValue;
		t_role_buff.attr.param1 = dbfile->Get(i, 6)->iValue;
		t_role_buff.attr.param2 = dbfile->Get(i, 7)->iValue;
		t_role_buff.attr.param3 = dbfile->Get(i, 8)->iValue;
		t_role_buff.attr.param4 = dbfile->Get(i, 9)->iValue;

		t_role_buff_[t_role_buff.id] = t_role_buff;
	}

	return 0;
}

s_t_role * RoleConfig::get_role(int id)
{
	std::map<int, s_t_role>::iterator it = t_roles_.find(id);
	if (it == t_roles_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

s_t_skill * RoleConfig::get_skill(int id, int level)
{
	std::map<int, std::map<int, s_t_skill> >::iterator it = t_skill_.find(id);
	if (it == t_skill_.end())
	{
		return 0;
	}
	else
	{
		std::map<int, s_t_skill> &ss = (*it).second;
		std::map<int, s_t_skill>::iterator jt = ss.find(level);
		if (jt == ss.end())
		{
			return 0;
		}
		else
		{
			return &(*jt).second;
		}
	}
}

s_t_role_level * RoleConfig::get_role_level(int level)
{
	std::map<int, s_t_role_level>::iterator it = t_role_level_.find(level);
	if (it == t_role_level_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}

s_t_role_buff * RoleConfig::get_role_buff(int id)
{
	std::map<int, s_t_role_buff>::iterator it = t_role_buff_.find(id);
	if (it == t_role_buff_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}
