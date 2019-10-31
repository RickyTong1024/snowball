#include "rank_config.h"
#include "dbc.h"

int RankConfig::parse()
{
	DBCFile * dbfile = service::scheme()->get_dbc("t_rank_type.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		int id = dbfile->Get(i, 0)->iValue;
		int value = dbfile->Get(i, 2)->iValue;

		t_rank_minvalue_[id] = value;
	}

	return 0;
}

int RankConfig::get_rank_minvalue(int id)
{
	std::map<int, int>::iterator it = t_rank_minvalue_.find(id);
	if (it == t_rank_minvalue_.end())
	{
		return 0;
	}
	else
	{
		return (*it).second;
	}
}
