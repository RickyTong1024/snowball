#include "room_manager.h"
#include "room_message.h"
#include "room_pool.h"
#include "room_config.h"

RoomManager::RoomManager()
: timer_id_(-1)
{
	
}

RoomManager::~RoomManager()
{
	
}

int RoomManager::init(const std::string &master_name, uint64_t battle_guid, int battle_type)
{
	if (sRoomConfig->parse() == -1)
	{
		return -1;
	}
	timer_id_ = service::timer()->schedule(boost::bind(&RoomManager::update, this, _1), ROOM_TICK, "room");
	if (-1 == timer_id_)
	{
		return -1;
	}
	sRoomPool->init(master_name, battle_guid, battle_type);
	return 0;
}

int RoomManager::fini()
{
	sRoomPool->fini();
	if (timer_id_)
	{
		service::timer()->cancel(timer_id_);
		timer_id_ = -1;
	}
	return 0;
}

int RoomManager::terminal_enter_world_udp(Packet *pck)
{
	int hid = pck->hid();
	hids_.insert(hid);
	return 0;
}

int RoomManager::terminal_leave_world_udp(Packet *pck)
{
	int hid = pck->hid();
	hids_.erase(hid);
	if (hid_guids_.find(hid) != hid_guids_.end())
	{
		uint64_t guid = hid_guids_[hid];
		hid_guids_.erase(hid);
		if (guid_hids_.find(guid) != guid_hids_.end())
		{
			if (guid_hids_[guid] == hid)
			{
				guid_hids_.erase(guid);
				sRoomPool->del_player(guid);
			}
		}
	}
	return 0;
}

int RoomManager::terminal_battle_link(Packet *pck)
{
	protocol::game::cmsg_battle_link msg;
	if (!pck->parse_protocol(msg))
	{
		service::log()->error("terminal_battle_link parse error");
		return -1;
	}
	int hid = pck->hid();
	uint64_t guid = msg.guid();
	if (wait_players_.find(guid) == wait_players_.end())
	{
		service::log()->error("not find wait player guid = %llu", guid);
		service::udp_service()->destory(hid);
		return 0;
	}
	WaitPlayer &wp = wait_players_[guid];
	if (wp.code != msg.code())
	{
		service::log()->error("player code guid = %llu", guid);
		service::udp_service()->destory(hid);
		return 0;
	}
	hid_guids_[hid] = wp.guid;
	guid_hids_[wp.guid] = hid;
	sRoomPool->link(wp.guid, hid, wp);
	return 0;
}

int RoomManager::terminal_battle_in(Packet *pck)
{
	if (hid_guids_.find(pck->hid()) == hid_guids_.end())
	{
		return -1;
	}
	uint64_t guid = hid_guids_[pck->hid()];
	WaitPlayer &wp = wait_players_[guid];
	sRoomPool->add_player(wp.guid, pck->hid(), wp);
	return 0;
}

int RoomManager::terminal_battle_state(Packet *pck)
{
	protocol::game::cmsg_battle_state msg;
	if (!pck->parse_protocol(msg))
	{
		service::log()->error("terminal_battle_state parse error");
		return -1;
	}
	sRoomPool->battle_state(msg.guid(), msg.state());
	return 0;
}

int RoomManager::terminal_battle_reset(Packet *pck)
{
	if (hid_guids_.find(pck->hid()) == hid_guids_.end())
	{
		return -1;
	}
	uint64_t guid = hid_guids_[pck->hid()];
	WaitPlayer &wp = wait_players_[guid];
	sRoomPool->reset(guid, wp);
	return 0;
}

int RoomManager::terminal_battle_op(Packet *pck)
{
	protocol::game::msg_battle_op *msg = new protocol::game::msg_battle_op();
	if (!pck->parse_protocol(*msg))
	{
		delete msg;
		return -1;
	}
	if (hid_guids_.find(pck->hid()) == hid_guids_.end())
	{
		return -1;
	}
	uint64_t guid = hid_guids_[pck->hid()];
	msg->set_guid(guid);
	sRoomPool->add_operation(msg);
	return 0;
}

int RoomManager::terminal_battle_end(Packet *pck)
{
	dhc::battle_result_t msg;
	if (!pck->parse_protocol(msg))
	{
		service::log()->error("terminal_battle_end parse error");
		return -1;
	}
	if (hid_guids_.find(pck->hid()) == hid_guids_.end())
	{
		return -1;
	}
	uint64_t guid = hid_guids_[pck->hid()];
	sRoomPool->battle_end(guid, msg);
	return 0;
}

int RoomManager::terminal_battle_code(Packet *pck)
{
	protocol::game::msg_battle_code msg;
	if (!pck->parse_protocol(msg))
	{
		service::log()->error("terminal_battle_state parse error");
		return -1;
	}
	if (hid_guids_.find(pck->hid()) == hid_guids_.end())
	{
		return -1;
	}
	uint64_t guid = hid_guids_[pck->hid()];
	sRoomPool->battle_code(guid, msg);
	return 0;
}

int RoomManager::push_rc_rm_set_player_room(Packet *pck, const std::string &name)
{
	protocol::game::push_rc_rm_set_player_room msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	WaitPlayer wp;
	wp.guid = msg.guid();
	wp.code = msg.code();
	wp.player.CopyFrom(msg.player());
	wp.is_new = 1;
	wait_players_[wp.guid] = wp;
	return 0;
}

int RoomManager::update(ACE_Time_Value tv)
{
	sRoomPool->update();
	return 0;
}
