#include "team_manager.h"
#include "team_message.h"
#include "team_operation.h"
#include "mode_config.h"

TeamManager::TeamManager()
	: team_id_(0)
	, team_member_(0)
{
	
}

TeamManager::~TeamManager()
{
	
}

int TeamManager::init()
{
	if (-1 == sModeConfig->parse())
	{
		return -1;
	}
	s_t_mode *t_mode = sModeConfig->get_mode(1);
	if (!t_mode)
	{
		return -1;
	}
	team_member_ = t_mode->team_member;
	return 0;
}

int TeamManager::fini()
{
	return 0;
}

int TeamManager::push_hall_team_create(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_team_create msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	const protocol::game::msg_battle_player_info &player = msg.player();
	uint64_t player_guid = player.guid();
	if (player_team_map_.find(player_guid) != player_team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_EXIST, "");
		return -1;
	}
	protocol::game::msg_team *team = new protocol::game::msg_team();
	team->set_team_id(team_id_);
	protocol::game::msg_team_member *member = team->add_member();
	member->set_member_type(1);
	member->mutable_player()->CopyFrom(msg.player());
	team_map_[team_id_] = team;
	player_team_map_[player_guid] = team_id_;
	team_id_++;
	TeamMessage::send_push_team_hall_create(name, player_guid, team);
	return 0;
}

int TeamManager::push_hall_team_join(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_team_join msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	const protocol::game::msg_battle_player_info &player = msg.player();
	uint64_t player_guid = player.guid();
	uint64_t target_guid = msg.player_guid();
	if (player_team_map_.find(player_guid) != player_team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_DUMP, "");
		return -1;
	}
	if (player_team_map_.find(target_guid) == player_team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_NOT_EXIST, "");
		return -1;
	}
	uint64_t team_id = player_team_map_[target_guid];
	if (team_map_.find(team_id) == team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_NOT_EXIST, "");
		return -1;
	}
	protocol::game::msg_team *team = team_map_[team_id];
	if (team->member_size() >= team_member_)
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_FULL, "");
		return -1;
	}
	protocol::game::msg_team_member *member = team->add_member();
	member->set_member_type(0);
	member->mutable_player()->CopyFrom(player);
	player_team_map_[player.guid()] = team->team_id();
	TeamMessage::send_push_team_hall_join(name, player.guid(), team);
	std::vector<uint64_t> members;
	for (int i = 0; i < team->member_size(); ++i)
	{
		if (team->member(i).player().guid() != player.guid())
		{
			members.push_back(team->member(i).player().guid());
		}
	}
	std::map<std::string, std::vector<uint64_t> > name_guids;
	TeamOperation::split_guids_server(members, name_guids);
	for (std::map<std::string, std::vector<uint64_t> >::iterator it = name_guids.begin(); it != name_guids.end(); ++it)
	{
		TeamMessage::send_push_team_hall_other_join((*it).first, (*it).second, member);
	}
	return 0;
}

int TeamManager::push_hall_team_exit(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_team_exit msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t player_guid = msg.player_guid();
	if (player_team_map_.find(player_guid) == player_team_map_.end())
	{
		if (msg.mauto() == 0)
		{
			TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_NOT_IN, "");
		}
		return -1;
	}
	int team_id = player_team_map_[player_guid];
	if (team_map_.find(team_id) == team_map_.end())
	{
		if (msg.mauto() == 0)
		{
			TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_NOT_EXIST, "");
		}
		return -1;
	}
	protocol::game::msg_team *team = team_map_[team_id];
	bool flag = false;
	for (int i = 0; i < team->member_size(); ++i)
	{
		const protocol::game::msg_team_member &member = team->member(i);
		if (member.player().guid() == player_guid)
		{
			for (int j = i; j < team->member_size() - 1; ++j)
			{
				team->mutable_member(j)->CopyFrom(team->member(j + 1));
			}
			team->mutable_member()->RemoveLast();
			player_team_map_.erase(player_guid);
			flag = true;
			break;
		}
	}
	if (!flag)
	{
		if (msg.mauto() == 0)
		{
			TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_NOT_IN, "");
		}
		return -1;
	}
	uint64_t leader_guid = 0;
	std::vector<uint64_t> members;
	if (msg.mauto() == 0)
	{
		members.push_back(player_guid);
	}
	if (team->member_size() == 0)
	{
		delete team;
		team_map_.erase(team_id);
	}
	else
	{
		flag = false;
		for (int i = 0; i < team->member_size(); ++i)
		{
			if (team->member(i).member_type() == 1)
			{
				flag = true;
				leader_guid = team->member(i).player().guid();
			}
			members.push_back(team->member(i).player().guid());
		}
		if (!flag)
		{
			team->mutable_member(0)->set_member_type(1);
			leader_guid = team->member(0).player().guid();
		}
	}
	std::map<std::string, std::vector<uint64_t> > name_guids;
	TeamOperation::split_guids_server(members, name_guids);
	for (std::map<std::string, std::vector<uint64_t> >::iterator it = name_guids.begin(); it != name_guids.end(); ++it)
	{
		TeamMessage::send_push_team_hall_exit((*it).first, (*it).second, leader_guid, player_guid);
	}
	return 0;
}

int TeamManager::push_hall_team_kick(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_team_kick msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t player_guid = msg.player_guid();
	uint64_t target_guid = msg.target_guid();
	if (player_team_map_.find(player_guid) == player_team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_NOT_IN, "");
		return -1;
	}
	int team_id = player_team_map_[player_guid];
	if (team_map_.find(team_id) == team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_NOT_EXIST, "");
		return -1;
	}
	protocol::game::msg_team *team = team_map_[team_id];
	bool flag = false;
	for (int i = 0; i < team->member_size(); ++i)
	{
		const protocol::game::msg_team_member &member = team->member(i);
		if (member.player().guid() == player_guid)
		{
			if (member.member_type() != 1)
			{
				TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_NOT_LEADER, "");
				return -1;
			}
			flag = true;
		}
	}
	if (!flag)
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_NOT_IN, "");
		return -1;
	}
	flag = false;
	for (int i = 0; i < team->member_size(); ++i)
	{
		const protocol::game::msg_team_member &member = team->member(i);
		if (member.player().guid() == target_guid)
		{
			for (int j = i; j < team->member_size() - 1; ++j)
			{
				team->mutable_member(j)->CopyFrom(team->member(j + 1));
			}
			team->mutable_member()->RemoveLast();
			player_team_map_.erase(target_guid);
			flag = true;
			break;
		}
	}
	if (!flag)
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_OTHER_NOT_IN, "");
		return -1;
	}
	std::vector<uint64_t> members;
	members.push_back(target_guid);
	for (int i = 0; i < team->member_size(); ++i)
	{
		members.push_back(team->member(i).player().guid());
	}
	std::map<std::string, std::vector<uint64_t> > name_guids;
	TeamOperation::split_guids_server(members, name_guids);
	for (std::map<std::string, std::vector<uint64_t> >::iterator it = name_guids.begin(); it != name_guids.end(); ++it)
	{
		TeamMessage::send_push_team_hall_kick((*it).first, (*it).second, target_guid);
	}
	return 0;
}

int TeamManager::push_hall_team_invert(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_team_invert msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t player_guid = msg.player_guid();
	uint64_t target_guid = msg.target_guid();
	if (player_team_map_.find(player_guid) == player_team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_NOT_IN, "");
		return -1;
	}
	if (player_team_map_.find(target_guid) != player_team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_OTHER_DUMP, "");
		return -1;
	}
	int team_id = player_team_map_[player_guid];
	if (team_map_.find(team_id) == team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_NOT_EXIST, "");
		return -1;
	}
	protocol::game::msg_team *team = team_map_[team_id];
	bool flag = false;
	std::string player_name = "";
	for (int i = 0; i < team->member_size(); ++i)
	{
		const protocol::game::msg_team_member &member = team->member(i);
		if (member.player().guid() == player_guid)
		{
			std::string sname = TeamOperation::get_guid_server_name(target_guid);
			TeamMessage::send_push_team_hall_invert(sname, target_guid, member.player());
			flag = true;
			break;
		}
	}
	if (!flag)
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_NOT_IN, "");
		return -1;
	}
	return 0;
}

int TeamManager::push_hall_team_chat(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_team_chat msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t player_guid = msg.player_guid();
	if (player_team_map_.find(player_guid) == player_team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_NOT_IN, "");
		return -1;
	}
	int team_id = player_team_map_[player_guid];
	if (team_map_.find(team_id) == team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_NOT_EXIST, "");
		return -1;
	}
	protocol::game::msg_team *team = team_map_[team_id];
	std::vector<uint64_t> members;
	for (int i = 0; i < team->member_size(); ++i)
	{
		members.push_back(team->member(i).player().guid());
	}
	std::map<std::string, std::vector<uint64_t> > name_guids;
	TeamOperation::split_guids_server(members, name_guids);
	for (std::map<std::string, std::vector<uint64_t> >::iterator it = name_guids.begin(); it != name_guids.end(); ++it)
	{
		TeamMessage::send_push_team_hall_chat((*it).first, (*it).second, player_guid, msg.text());
	}
	return 0;
}

int TeamManager::push_hall_team_multi_battle(Packet *pck, const std::string &name)
{
	protocol::game::push_hall_team_multi_battle msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t player_guid = msg.player_guid();
	if (player_team_map_.find(player_guid) == player_team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_MEMBER_NOT_IN, "");
		return -1;
	}
	int team_id = player_team_map_[player_guid];
	if (team_map_.find(team_id) == team_map_.end())
	{
		TeamMessage::send_push_team_hall_error(name, player_guid, ERROR_TEAM_NOT_EXIST, "");
		return -1;
	}
	protocol::game::msg_team *team = team_map_[team_id];
	TeamMessage::send_req_team_rc_multi_battle("room_center", team->team_id(), team, boost::bind(&TeamManager::req_team_rc_multi_battle_callback, this, _1, _2, team_id));
	return 0;
}

void TeamManager::req_team_rc_multi_battle_callback(Packet *pck, int error_code, int team_id)
{
	protocol::game::rep_team_rc_multi_battle msg;
	if (!pck->parse_protocol(msg))
	{
		return;
	}
	if (team_map_.find(team_id) == team_map_.end())
	{
		return;
	}
	protocol::game::msg_team *team = team_map_[team_id];
	std::vector<uint64_t> members;
	for (int i = 0; i < team->member_size(); ++i)
	{
		members.push_back(team->member(i).player().guid());
	}
	std::map<std::string, std::vector<uint64_t> > name_guids;
	TeamOperation::split_guids_server(members, name_guids);
	for (std::map<std::string, std::vector<uint64_t> >::iterator it = name_guids.begin(); it != name_guids.end(); ++it)
	{
		std::vector<uint64_t> &guids = (*it).second;
		std::vector<std::string> codes;
		for (int i = 0; i < guids.size(); ++i)
		{
			bool flag = false;
			for (int j = 0; j < msg.guid_size(); ++j)
			{
				if (guids[i] == msg.guid(j))
				{
					flag = true;
					codes.push_back(msg.code(j));
					break;
				}
			}
			if (!flag)
			{
				codes.push_back("");
			}
		}
		TeamMessage::send_push_team_hall_multi_battle((*it).first, guids, msg.udp_ip(), msg.udp_port(), msg.tcp_ip(), msg.tcp_port(), codes, members.size());
	}
	for (int i = 0; i < team->member_size(); ++i)
	{
		player_team_map_.erase(team->member(i).player().guid());
	}
	team_map_.erase(team_id);
}
