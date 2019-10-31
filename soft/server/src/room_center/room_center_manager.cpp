#include "room_center_manager.h"
#include "room_center_message.h"
#include "room_center_operation.h"
#include "room_center_dhc.h"
#include "gtool.h"
#include "dhc.h"
#include "mode_config.h"

#define UPDATE_TIME 1000
#define ROOM_TIME 8 * 60000 + 25000
#define ROOM_UN_ENTER_TIME 60000 * 3
#define ROOM_CREATE_TIME 5000

RoomCenterManager::RoomCenterManager()
: timer_id_(-1)
{
	
}

RoomCenterManager::~RoomCenterManager()
{
	
}

int RoomCenterManager::init()
{
	if (-1 == sModeConfig->parse())
	{
		return -1;
	}
	s_t_mode *t_mode = sModeConfig->get_mode(0);
	if (!t_mode)
	{
		return -1;
	}
	max_room_player_ = t_mode->team_member * t_mode->team_number;
	t_mode = sModeConfig->get_mode(1);
	if (!t_mode)
	{
		return -1;
	}
	max_team_num_ = t_mode->team_number;
	max_team_member_ = t_mode->team_member;

	sRoomCenterDhc->init();
	timer_id_ = service::timer()->schedule(boost::bind(&RoomCenterManager::update, this, _1), UPDATE_TIME, "room_center_manager");
	if (timer_id_ == -1)
	{
		return -1;
	}
	std::vector<std::string> names;
	service::server_env()->get_server_names("room_master", names);
	for (int i = 0; i < names.size(); ++i)
	{
		master_room_map_[names[i]].clear();
	}

	service::server_env()->get_server_names("hall", hall_names_);
	return 0;
}

int RoomCenterManager::fini()
{
	if (timer_id_ != -1)
	{
		service::timer()->cancel(timer_id_);
		timer_id_ = -1;
	}
	sRoomCenterDhc->fini();
	return 0;
}

int RoomCenterManager::req_hall_rc_has_battle(Packet *pck, const std::string &name, int id)
{
	protocol::game::req_hall_rc_has_battle msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t rguid = msg.guid();
	if (player_room_map_.find(rguid) != player_room_map_.end())
	{
		/// 已有房间，进入原房间
		uint64_t battle_guid = player_room_map_[rguid];
		RoomInfo *ri = room_info_map_[battle_guid];
		if (ri->room_type == 0)
		{
			for (std::map<uint64_t, RoomPlayer *>::iterator it = ri->room_players.begin(); it != ri->room_players.end(); ++it)
			{
				RoomPlayer *rp = (*it).second;
				if (rp->player->guid() == rguid)
				{
					if (!ri->is_createing)
					{
						RoomCenterMessage::send_rep_hall_rc_has_battle(name, id, 0, 0, ri->udp_ip, ri->udp_port, ri->tcp_ip, ri->tcp_port, rp->code);
					}
					else
					{
						RoomCenterMessage::send_rep_hall_rc_has_battle(name, id, 0, 1, "", 0, "", 0, "");
					}
				}
			}
		}
		else
		{
			for (std::map<uint64_t, RoomTeam *>::iterator it = ri->room_teams.begin(); it != ri->room_teams.end(); ++it)
			{
				RoomTeam *rt = (*it).second;
				for (int i = 0; i < rt->players.size(); ++i)
				{
					if (rt->players[i]->guid() == rguid)
					{
						if (!ri->is_createing)
						{
							RoomCenterMessage::send_rep_hall_rc_has_battle(name, id, 0, 0, ri->udp_ip, ri->udp_port, ri->tcp_ip, ri->tcp_port, rt->codes[i]);
						}
						else
						{
							RoomCenterMessage::send_rep_hall_rc_has_battle(name, id, 0, 1, "", 0, "", 0, "");
						}
					}
				}
			}
		}
	}
	RoomCenterMessage::send_rep_hall_rc_has_battle(name, id, 0, 1, "", 0, "", 0, "");
	return 0;
}

int RoomCenterManager::req_hall_rc_single_battle(Packet *pck, const std::string &name, int id)
{
	protocol::game::req_hall_rc_single_battle msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t rguid = msg.player().guid();
	if (player_room_map_.find(rguid) != player_room_map_.end())
	{
		/// 已有房间
		return 0;
	}
	RoomPlayer *rp = new RoomPlayer();
	rp->id = id;
	rp->player->CopyFrom(msg.player());
	rp->code = RoomCenterOperation::make_code();
	if (kx_rooms_.size() == 0)
	{
		/// 没有空房间，创建房间
		RoomInfo *ri = new RoomInfo();
		ri->battle_guid = DB_GTOOL->assign(et_battle_result);
		/// 找到房间数最少的master
		bool flag = false;
		int num = 0;
		std::string master_name = "";
		for (std::map<std::string, std::set<int> >::iterator it = master_room_map_.begin(); it != master_room_map_.end(); ++it)
		{
			if (!flag)
			{
				num = (*it).second.size();
				master_name = (*it).first;
				flag = true;
			}
			else if ((*it).second.size() < num)
			{
				num = (*it).second.size();
				master_name = (*it).first;
			}
		}
		if (!flag)
		{
			return -1;
		}
		ri->master_name = master_name;
		rp->player->set_camp(ri->room_players.size());
		master_room_map_[master_name].insert(ri->battle_guid);
		ri->udp_ip = "";
		ri->udp_port = 0;
		ri->tcp_ip = "";
		ri->tcp_port = 0;
		ri->room_type = 0;
		ri->room_players[rp->player->guid()] = rp;
		player_room_map_[rp->player->guid()] = ri->battle_guid;
		ri->start_time = service::timer()->now();
		ri->is_createing = true;
		room_info_map_[ri->battle_guid] = ri;
		kx_rooms_.insert(ri->battle_guid);
		RoomCenterMessage::send_req_rc_rm_create_room(master_name, ri->battle_guid, ri->room_type, boost::bind(&RoomCenterManager::create_room_callback, this, _1, _2, ri->battle_guid));
	}
	else
	{
		/// 找到空闲房间，进入
		uint64_t battle_guid = *(kx_rooms_.begin());
		RoomInfo *ri = room_info_map_[battle_guid];
		rp->player->set_camp(ri->room_players.size());
		ri->room_players[rp->player->guid()] = rp;
		player_room_map_[rp->player->guid()] = battle_guid;
		if (ri->room_players.size() >= max_room_player_)
		{
			kx_rooms_.erase(battle_guid);
		}
		if (!ri->is_createing)
		{
			RoomCenterMessage::send_push_rc_rm_set_player_room(ri->master_name, rp->player->guid(), rp->code, rp->player, ri->battle_guid);
			RoomCenterMessage::send_rep_hall_rc_single_battle(name, id, 0, 1, ri->udp_ip, ri->udp_port, ri->tcp_ip, ri->tcp_port, rp->code);
		}
	}

	return 0;
}

int RoomCenterManager::req_team_rc_multi_battle(Packet *pck, const std::string &name, int id)
{
	protocol::game::req_team_rc_multi_battle msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	for (int i = 0; i < msg.player_size(); ++i)
	{
		uint64_t rguid = msg.player(i).guid();
		if (player_room_map_.find(rguid) != player_room_map_.end())
		{
			/// 已有房间
			return 0;
		}
	}
	RoomTeam *rt = new RoomTeam();
	rt->team_id = msg.team_id();
	rt->id = id;
	for (int i = 0; i < msg.player_size(); ++i)
	{
		rt->codes.push_back(RoomCenterOperation::make_code());
		protocol::game::msg_battle_player_info *player = new protocol::game::msg_battle_player_info();
		player->CopyFrom(msg.player(i));
		rt->players.push_back(player);
	}
	
	int player_num = msg.player_size();
	if (mkx_rooms_[player_num].size() == 0)
	{
		/// 没有空房间，创建房间
		RoomInfo *ri = new RoomInfo();
		ri->battle_guid = DB_GTOOL->assign(et_battle_result);
		/// 找到房间数最少的master
		bool flag = false;
		int num = 0;
		std::string master_name = "";
		for (std::map<std::string, std::set<int> >::iterator it = master_room_map_.begin(); it != master_room_map_.end(); ++it)
		{
			if (!flag)
			{
				num = (*it).second.size();
				master_name = (*it).first;
				flag = true;
			}
			else if ((*it).second.size() < num)
			{
				num = (*it).second.size();
				master_name = (*it).first;
			}
		}
		if (!flag)
		{
			return -1;
		}
		ri->master_name = master_name;
		master_room_map_[master_name].insert(ri->battle_guid);
		ri->udp_ip = "";
		ri->udp_port = 0;
		ri->tcp_ip = "";
		ri->tcp_port = 0;
		ri->room_type = 1;
		ri->room_teams[rt->team_id] = rt;
		for (int i = 0; i < rt->players.size(); ++i)
		{
			player_room_map_[rt->players[i]->guid()] = ri->battle_guid;
		}
		ri->start_time = service::timer()->now();
		ri->is_createing = true;
		for (int i = 0; i < max_team_num_; ++i)
		{
			ri->room_kong.push_back(max_team_member_);
		}
		ri->room_kong[0] = ri->room_kong[0] - player_num;
		for (int i = 0; i < rt->players.size(); ++i)
		{
			rt->players[i]->set_camp(0);
		}
		room_info_map_[ri->battle_guid] = ri;
		for (int i = 1; i <= max_team_member_; ++i)
		{
			mkx_rooms_[i].insert(ri->battle_guid);
		}
		RoomCenterMessage::send_req_rc_rm_create_room(master_name, ri->battle_guid, ri->room_type, boost::bind(&RoomCenterManager::create_room_callback, this, _1, _2, ri->battle_guid));
	}
	else
	{
		/// 找到空闲房间，进入
		uint64_t battle_guid = *(mkx_rooms_[player_num].begin());
		RoomInfo *ri = room_info_map_[battle_guid];
		ri->room_teams[rt->team_id] = rt;
		for (int i = 0; i < rt->players.size(); ++i)
		{
			player_room_map_[rt->players[i]->guid()] = ri->battle_guid;
		}
		int min_num = 999;
		int index = -1;
		for (int i = 0; i < max_team_num_; ++i)
		{
			if (ri->room_kong[i] < min_num && ri->room_kong[i] >= player_num)
			{
				index = i;
			}
		}
		if (index == -1)
		{
			return 0;
		}
		ri->room_kong[index] = ri->room_kong[index] - player_num;
		for (int i = 0; i < rt->players.size(); ++i)
		{
			rt->players[i]->set_camp(index);
		}
		int max_num = 0;
		for (int i = 0; i < max_team_num_; ++i)
		{
			if (ri->room_kong[i] > max_num)
			{
				max_num = ri->room_kong[i];
			}
		}
		for (int i = 1; i <= max_team_member_; ++i)
		{
			mkx_rooms_[i].erase(battle_guid);
		}
		for (int i = 1; i <= max_num; ++i)
		{
			mkx_rooms_[i].insert(battle_guid);
		}
		if (!ri->is_createing)
		{
			std::vector<uint64_t> guids;
			for (int i = 0; i < rt->players.size(); ++i)
			{
				guids.push_back(rt->players[i]->guid());
				RoomCenterMessage::send_push_rc_rm_set_player_room(ri->master_name, rt->players[i]->guid(), rt->codes[i], rt->players[i], ri->battle_guid);
			}
			RoomCenterMessage::send_rep_team_rc_multi_battle(name, id, 0, guids, ri->udp_ip, ri->udp_port, ri->tcp_ip, ri->tcp_port, rt->codes);
		}
	}

	return 0;
}

void RoomCenterManager::create_room_callback(Packet *pck, int error_code, uint64_t battle_guid)
{
	protocol::game::rep_rc_rm_create_room msg;
	if (!pck->parse_protocol(msg))
	{
		return;
	}
	if (room_info_map_.find(battle_guid) == room_info_map_.end())
	{
		return;
	}
	RoomInfo *ri = room_info_map_[battle_guid];
	ri->tcp_ip = msg.tcp_ip();
	ri->tcp_port = msg.tcp_port();
	ri->udp_ip = msg.udp_ip();
	ri->udp_port = msg.udp_port();
	ri->is_createing = false;
	if (ri->room_type == 0)
	{
		for (std::map<uint64_t, RoomPlayer *>::iterator it = ri->room_players.begin(); it != ri->room_players.end(); ++it)
		{
			RoomPlayer *rp = (*it).second;
			RoomCenterMessage::send_push_rc_rm_set_player_room(ri->master_name, rp->player->guid(), rp->code, rp->player, ri->battle_guid);
			RoomCenterMessage::send_rep_hall_rc_single_battle(get_player_guid_name(rp->player->guid()), rp->id, 0, 1, ri->udp_ip, ri->udp_port, ri->tcp_ip, ri->tcp_port, rp->code);
		}
	}
	else
	{
		for (std::map<uint64_t, RoomTeam *>::iterator it = ri->room_teams.begin(); it != ri->room_teams.end(); ++it)
		{
			RoomTeam *rt = (*it).second;
			std::vector<uint64_t> guids;
			for (int i = 0; i < rt->players.size(); ++i)
			{
				guids.push_back(rt->players[i]->guid());
				RoomCenterMessage::send_push_rc_rm_set_player_room(ri->master_name, rt->players[i]->guid(), rt->codes[i], rt->players[i], ri->battle_guid);
			}
			RoomCenterMessage::send_rep_team_rc_multi_battle("team", rt->id, 0, guids, ri->udp_ip, ri->udp_port, ri->tcp_ip, ri->tcp_port, rt->codes);
		}
	}
}

int RoomCenterManager::push_rm_rc_battle_end(Packet *pck, const std::string &name)
{
	protocol::game::push_rm_rc_battle_end msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	uint64_t battle_guid = msg.battle_guid();
	if (room_info_map_.find(battle_guid) == room_info_map_.end())
	{
		return -1;
	}
	RoomInfo *ri = room_info_map_[battle_guid];
	std::map<std::string, std::vector<uint64_t> > guids;
	int type = ri->room_type;
	if (type == 0)
	{
		for (std::map<uint64_t, RoomPlayer *>::iterator jt = ri->room_players.begin(); jt != ri->room_players.end(); ++jt)
		{
			uint64_t guid = (*jt).first;
			player_room_map_.erase(guid);
			guids[get_player_guid_name(guid)].push_back(guid);
		}
		kx_rooms_.erase(ri->battle_guid);
	}
	else
	{
		for (std::map<uint64_t, RoomTeam *>::iterator it = ri->room_teams.begin(); it != ri->room_teams.end(); ++it)
		{
			RoomTeam *rt = (*it).second;
			for (int i = 0; i < rt->players.size(); ++i)
			{
				uint64_t guid = rt->players[i]->guid();
				player_room_map_.erase(guid);
				guids[get_player_guid_name(guid)].push_back(guid);
			}
		}
		for (int i = 1; i <= max_team_member_; ++i)
		{
			mkx_rooms_[i].erase(ri->battle_guid);
		}
	}
	master_room_map_[ri->master_name].erase(ri->battle_guid);
	delete ri;
	room_info_map_.erase(battle_guid);
	for (std::map<std::string, std::vector<uint64_t> >::iterator it = guids.begin(); it != guids.end(); ++it)
	{
		std::vector<uint64_t> &tguids = (*it).second;
		RoomCenterMessage::send_push_rc_hall_battle_end((*it).first, tguids, msg.result(), type);
	}
	if (msg.result() != "")
	{
		dhc::battle_result_t *result = new dhc::battle_result_t();
		if (result->ParseFromString(msg.result()))
		{
			Request *req = new Request();
			req->add(opc_insert, result->guid(), result);
			DB_BATTLE->upcall(req, 0);
		}
		else
		{
			delete result;
		}
	}
	return 0;
}

std::string RoomCenterManager::get_player_guid_name(uint64_t player_guid)
{
	int index = player_guid % hall_names_.size();
	return hall_names_[index];
}

int RoomCenterManager::update(const ACE_Time_Value & cur_time)
{
	uint64_t now = service::timer()->now();
	for (std::set<uint64_t>::iterator it = kx_rooms_.begin(); it != kx_rooms_.end();)
	{
		uint64_t battle_guid = *it;
		if (room_info_map_.find(battle_guid) == room_info_map_.end())
		{
			kx_rooms_.erase(it++);
			continue;
		}
		RoomInfo *ri = room_info_map_[battle_guid];
		if (now > ri->start_time + ROOM_UN_ENTER_TIME)
		{
			kx_rooms_.erase(it++);
			continue;
		}
		it++;
	}
	for (int i = 1; i <= max_team_member_; ++i)
	{
		for (std::set<uint64_t>::iterator it = mkx_rooms_[i].begin(); it != mkx_rooms_[i].end();)
		{
			uint64_t battle_guid = *it;
			if (room_info_map_.find(battle_guid) == room_info_map_.end())
			{
				mkx_rooms_[i].erase(it++);
				continue;
			}
			RoomInfo *ri = room_info_map_[battle_guid];
			if (now > ri->start_time + ROOM_UN_ENTER_TIME)
			{
				mkx_rooms_[i].erase(it++);
				continue;
			}
			it++;
		}
	}
	for (std::map<uint64_t, RoomInfo *>::iterator it = room_info_map_.begin(); it != room_info_map_.end();)
	{
		RoomInfo *ri = (*it).second;
		int ql = 0;
		int type = ri->room_type;
		if (now > ri->start_time + ROOM_TIME)
		{
			ql = 1;
			service::log()->error("clear1 room guid = <%llu>", ri->battle_guid);
		}
		if (ri->is_createing && now > ri->start_time + ROOM_CREATE_TIME)
		{
			ql = 2;
			service::log()->error("clear2 room guid = <%llu>", ri->battle_guid);
		}
		if (ql)
		{
			/// 清理超时房间
			std::map<std::string, std::vector<uint64_t> > guids;
			if (ri->room_type == 0)
			{
				for (std::map<uint64_t, RoomPlayer *>::iterator jt = ri->room_players.begin(); jt != ri->room_players.end(); ++jt)
				{
					uint64_t guid = (*jt).first;
					player_room_map_.erase(guid);
					guids[get_player_guid_name(guid)].push_back(guid);
				}
				kx_rooms_.erase(ri->battle_guid);
			}
			else
			{
				for (std::map<uint64_t, RoomTeam *>::iterator it = ri->room_teams.begin(); it != ri->room_teams.end(); ++it)
				{
					RoomTeam *rt = (*it).second;
					for (int i = 0; i < rt->players.size(); ++i)
					{
						uint64_t guid = rt->players[i]->guid();
						player_room_map_.erase(guid);
						guids[get_player_guid_name(guid)].push_back(guid);
					}
				}
				for (int i = 1; i <= max_team_member_; ++i)
				{
					mkx_rooms_[i].erase(ri->battle_guid);
				}
			}
			master_room_map_[ri->master_name].erase(ri->battle_guid);
			delete ri;
			room_info_map_.erase(it++);

			if (ql == 1)
			{
				for (std::map<std::string, std::vector<uint64_t> >::iterator it = guids.begin(); it != guids.end(); ++it)
				{
					std::vector<uint64_t> &tguids = (*it).second;
					RoomCenterMessage::send_push_rc_hall_battle_end((*it).first, tguids, "", type);
				}
			}
			continue;
		}
		it++;
	}
	return 0;
}
