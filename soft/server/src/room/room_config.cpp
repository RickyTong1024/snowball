#include "room_config.h"
#include "utils.h"
#include "dbc.h"

int RoomConfig::parse()
{
	DBCFile * dbfile = service::scheme()->get_dbc("t_mode.txt");
	if (!dbfile)
	{
		return -1;
	}

	for (int i = 0; i < dbfile->GetRecordsNum(); ++i)
	{
		s_t_mode t_mode;
		t_mode.id = dbfile->Get(i, 0)->iValue;
		t_mode.team_member = dbfile->Get(i, 9)->iValue;
		t_mode.team_number = dbfile->Get(i, 10)->iValue;

		t_modes_[t_mode.id] = t_mode;
	}

	return 0;
}

s_t_mode * RoomConfig::get_mode(int id)
{
	std::map<int, s_t_mode>::iterator it = t_modes_.find(id);
	if (it == t_modes_.end())
	{
		return 0;
	}
	else
	{
		return &(*it).second;
	}
}
