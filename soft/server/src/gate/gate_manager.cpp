#include "gate_manager.h"
#include "gate_message.h"
#include "utils.h"

#define UPDATE_TIME 1000

GateManager::GateManager()
: timer_id_(-1)
, num_(0)
{
	
}

GateManager::~GateManager()
{
	
}

int GateManager::init()
{
	timer_id_ = service::timer()->schedule(boost::bind(&GateManager::update, this, _1), UPDATE_TIME, "gate_manager");
	if (-1 == timer_id_)
	{
		return -1;
	}
	return 0;
}

int GateManager::fini()
{
	if (-1 != timer_id_)
	{
		service::timer()->cancel(timer_id_);
		timer_id_ = -1;
	}
	return 0;
}

int GateManager::terminal_enter_world(Packet *pck)
{
	hid_info hi;
	hi.hid = pck->hid();
	hi.guid = 0;
	hi.time = service::timer()->now();
	hids_[hi.hid] = hi;
	hid_list_.push_back(hi.hid);
	return 0;
}

int GateManager::terminal_leave_world(Packet *pck)
{
	int hid = pck->hid();
	if (hids_.find(hid) != hids_.end())
	{
		hid_info &hi = hids_[hid];
		uint64_t acc_guid = hi.guid;
		if (accs_.find(acc_guid) != accs_.end())
		{
			acc_info *ai = accs_[acc_guid];
			if (!ai->is_offline)
			{
				Packet *pck1 = Packet::New((uint16_t)PUSH_GATE_HALL_LOGOUT, pck->hid(), ai->acc->guid(), 0);
				GateMessage::send_push_default(pck1);
				delete pck1;
				ai->is_offline = true;
				ai->offline_time = service::timer()->now();
			}
		}
		guid_hids_.erase(acc_guid);
		hids_.erase(hid);
	}
	return 0;
}

int GateManager::terminal_login(Packet *pck)
{
	if (hids_.find(pck->hid()) == hids_.end())
	{
		return -1;
	}
	pck->hptr().guid = hids_[pck->hid()].guid;
	GateMessage::send_push_default_login(pck);
	return 0;
}

int GateManager::terminal_login_player(Packet *pck)
{
	if (hids_.find(pck->hid()) == hids_.end())
	{
		return -1;
	}
	pck->hptr().guid = hids_[pck->hid()].guid;
	if (accs_.find(pck->guid()) == accs_.end())
	{
		return -1;
	}
	protocol::game::cmsg_login_player msg;
	msg.mutable_acc()->CopyFrom(*accs_[pck->guid()]->acc);
	Packet *pck1 = Packet::New((uint16_t)CMSG_LOGIN_PLAYER, pck->hid(), pck->guid(), &msg);
	GateMessage::send_push_default(pck1);
	delete pck1;
	return 0;
}

int GateManager::terminal_relogin_player(Packet *pck)
{
	protocol::game::cmsg_relogin_player msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	pck->hptr().guid = msg.guid();
	if (accs_.find(pck->guid()) == accs_.end())
	{
		GateMessage::send_smsg_error(pck->hid(), ERROR_LOGIN_CODE);
		return -1;
	}
	acc_info *ai = accs_[pck->guid()];
	if (ai->code != msg.code())
	{
		GateMessage::send_smsg_error(pck->hid(), ERROR_LOGIN_CODE);
		return -1;
	}
	ai->is_offline = false;
	uint64_t acc_guid = ai->acc->guid();
	if (guid_hids_.find(acc_guid) != guid_hids_.end())
	{
		int hid = guid_hids_[acc_guid];
		hids_[hid].guid = 0;
	}
	hids_[pck->hid()].guid = acc_guid;
	guid_hids_[acc_guid] = pck->hid();

	protocol::game::cmsg_login_player msg1;
	msg1.mutable_acc()->CopyFrom(*ai->acc);
	Packet *pck1 = Packet::New((uint16_t)CMSG_LOGIN_PLAYER, pck->hid(), pck->guid(), &msg1);
	GateMessage::send_push_default(pck1);
	delete pck1;
	return 0;
}

int GateManager::terminal_beat(Packet *pck)
{
	if (hids_.find(pck->hid()) != hids_.end())
	{
		hids_[pck->hid()].time = service::timer()->now();
	}
	return 0;
}

int GateManager::terminal_default(Packet *pck)
{
	if (hids_.find(pck->hid()) == hids_.end())
	{
		return -1;
	}
	if (hids_.find(pck->hid()) == hids_.end())
	{
		return -1;
	}
	pck->hptr().guid = hids_[pck->hid()].guid;
	GateMessage::send_push_default(pck);
	return 0;
}

int GateManager::push_login(Packet *pck, const std::string &name)
{
	int hid = pck->hid();
	protocol::game::smsg_login msg;
	if (!pck->parse_protocol(msg))
	{
		return -1;
	}
	if (hids_.find(pck->hid()) == hids_.end())
	{
		return -1;
	}
	if (hids_[hid].guid != 0)
	{
		Packet *pck1 = Packet::New((uint16_t)PUSH_GATE_HALL_LOGOUT, hid, hids_[hid].guid, 0);
		GateMessage::send_push_default(pck1);
		hids_[hid].guid = 0;
	}
	uint64_t acc_guid = msg.acc().guid();
	if (accs_.find(acc_guid) != accs_.end())
	{
		acc_info *ai = accs_[acc_guid];
		ai->is_offline = false;
		if (guid_hids_.find(acc_guid) != guid_hids_.end())
		{
			int hid1 = guid_hids_[acc_guid];
			hids_[hid1].guid = 0;
		}
		msg.set_code(ai->code);
	}
	else
	{
		acc_info *ai = new acc_info;
		ai->acc = new dhc::acc_t();
		ai->acc->CopyFrom(msg.acc());
		ai->is_offline = false;
		ai->offline_time = 0;
		ai->code = Utils::get_int32(0, 99999999);
		accs_[acc_guid] = ai;
		acc_list_.push_back(acc_guid);
		msg.set_code(ai->code);
	}
	hids_[hid].guid = acc_guid;
	guid_hids_[acc_guid] = hid;

	Packet *pck1 = Packet::New(pck->opcode(), pck->hid(), pck->guid(), &msg);
	GateMessage::send_terminal_default(pck1, hid);
	delete pck1;
	return 0;
}

int GateManager::push_default(Packet *pck, const std::string &name)
{
	int hid = pck->hid();
	GateMessage::send_terminal_default(pck, hid);
	return 0;
}

//////////////////////////////////////////////////////////////////////////

int GateManager::update(const ACE_Time_Value & cur_time)
{
	num_++;
	if (num_ >= 10)
	{
		num_ = 0;
		GateMessage::send_push_gate_player_num(hids_.size());
	}
	int num = 500;
	if (hid_list_.size() < num)
	{
		num = hid_list_.size();
	}
	while (num--)
	{
		int hid = hid_list_.front();
		hid_list_.pop_front();
		if (hids_.find(hid) != hids_.end())
		{
			hid_info &hi = hids_[hid];
			if (hi.time + 600000 < service::timer()->now())
			{
				service::tcp_service()->destory(hid);
			}
			else
			{
				hid_list_.push_back(hid);
			}
		}
	}
	
	num = 500;
	if (acc_list_.size() < num)
	{
		num = acc_list_.size();
	}
	while (num--)
	{
		uint64_t guid = acc_list_.front();
		acc_list_.pop_front();
		if (accs_.find(guid) != accs_.end())
		{
			acc_info *ai = accs_[guid];
			if (ai->is_offline && ai->offline_time + 1800000 < service::timer()->now())
			{
				accs_.erase(ai->acc->guid());
				delete ai->acc;
				delete ai;
			}
			else
			{
				acc_list_.push_back(guid);
			}
		}
	}
	return 0;
}
