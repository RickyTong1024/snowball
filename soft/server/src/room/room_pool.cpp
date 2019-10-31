#include "room_pool.h"
#include "utils.h"
#include "room_message.h"
#include "room_config.h"

#define MAX_TIME (8 * 60000)
//#define STATE_TIME 50
#define STATE_TIME 30000
#define CODE_TIME 5000
#define CHECK_MAX_TIME 5000

void RoomPool::init(const std::string &master_name, uint64_t battle_guid, int battle_type)
{
	zhen_ = 0;
	rs_.zhen = 0;
	rs_.data = "";
	op_start_index_ = 0;
	is_end_ = 0;
	re_tid_ = 10000000;
	master_name_ = master_name;
	battle_guid_ = battle_guid;
	battle_type_ = battle_type;
	s_t_mode *t_mode = sRoomConfig->get_mode(battle_type_);
	if (t_mode)
	{
		team_num_ = t_mode->team_number;
		member_num_ = t_mode->team_member;
	}
	else
	{
		team_num_ = 2;
		member_num_ = 5;
	}
	seed_ = Utils::get_int32(0, 99999999);
	seed_add_ = Utils::get_int32(0, 999);
	RoomMessage::send_push_r_rm_created(battle_guid, master_name);
#ifndef _WIN32
	std::string s = "../../log/room/room_";
	s = s + boost::lexical_cast<std::string>(battle_guid) + ".out";
	service::log()->enable_file(s);
#endif

}

void RoomPool::fini()
{
	for (int i = 0; i < op_vec_.size(); ++i)
	{
		delete op_vec_[i];
	}
	op_vec_.clear();
	for (int i = 0; i < op_all_.size(); ++i)
	{
		delete op_all_[i];
	}
	op_all_.clear();
}

void RoomPool::battle_state(uint64_t guid, const protocol::game::msg_battle_state &msg)
{
	int zhen = msg.zhen();
	if (up_states_map_.find(zhen) == up_states_map_.end())
	{
		return;
	}
	std::map<uint64_t, std::string> &states = up_states_map_[zhen];
	if (states.find(guid) == states.end())
	{
		return;
	}
	if (states[guid] != "")
	{
		return;
	}
	std::string s;
	msg.SerializeToString(&s);
	states[guid] = s;
	check_state(zhen);
}

void RoomPool::check_state(int zhen)
{
	if (up_states_map_.find(zhen) == up_states_map_.end())
	{
		return;
	}
	std::map<uint64_t, std::string> &states = up_states_map_[zhen];
	bool flag = true;
	bool flag1 = false;
	for (std::map<uint64_t, std::string>::iterator it = states.begin(); it != states.end(); ++it)
	{
		if ((*it).second == "")
		{
			flag = false;
			break;
		}
	}
	if (flag)
	{
		flag1 = true;
	}
	else if (zhen_ - zhen >= CHECK_MAX_TIME / ROOM_TICK)
	{
		for (std::map<uint64_t, std::string>::iterator it = states.begin(); it != states.end();)
		{
			if ((*it).second == "")
			{
				uint64_t guid = (*it).first;
				/// 移出guid
				if (player_map_.find(guid) != player_map_.end())
				{
					RoomPlayer &rp = player_map_[guid];
					rp.no_state_num++;
					if (rp.no_state_num >= 3)
					{
						service::log()->error("state timeout destory zhen = %d, guid = %llu", zhen, guid);
						service::udp_service()->destory(rp.hid);
					}
					else
					{
						service::log()->error("state timeout zhen = %d, guid = %llu", zhen, guid);
					}
				}
				states.erase(it++);
			}
			else
			{
				++it;
			}
		}
		flag1 = true;
	}
	if (flag1)
	{
		std::vector<tmp_state> tmp_states;
		for (std::map<uint64_t, std::string>::iterator it = states.begin(); it != states.end(); ++it)
		{
			uint64_t guid = (*it).first;
			std::string s = (*it).second;
			if (player_map_.find(guid) != player_map_.end())
			{
				RoomPlayer &rp = player_map_[guid];
				rp.no_state_num = 0;
			}
			bool flag = false;
			for (int i = 0; i < tmp_states.size(); ++i)
			{
				if (tmp_states[i].data == s)
				{
					tmp_states[i].num++;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				tmp_state ts;
				ts.num = 1;
				ts.data = s;
				tmp_states.push_back(ts);
			}
		}
		bool flag = false;
		int num = 0;
		std::string data = "";
		for (int i = 0; i < tmp_states.size(); ++i)
		{
			if (!flag)
			{
				flag = true;
				num = tmp_states[i].num;
				data = tmp_states[i].data;
			}
			else if (tmp_states[i].num > num)
			{
				num = tmp_states[i].num;
				data = tmp_states[i].data;
			}
		}
		if (flag)
		{
			for (std::map<uint64_t, std::string>::iterator it = states.begin(); it != states.end(); ++it)
			{
				uint64_t guid = (*it).first;
				std::string s = (*it).second;
				if (data != s)
				{
					/// 移出guid
					service::log()->error("bu tong bu zhen = %d, guid = %llu", zhen, guid);
					protocol::game::msg_battle_state msg1;
					msg1.ParseFromString(data);
					protocol::game::msg_battle_state msg2;
					msg2.ParseFromString(s);
					if (msg1.players_size() != msg2.players_size())
					{
						printf("player_size\n");
					}
					else
					{
						for (int i = 0; i < msg1.players_size(); ++i)
						{
							std::string s1;
							const protocol::game::msg_battle_player &player1 = msg1.players(i);
							player1.SerializeToString(&s1);
							std::string s2;
							const protocol::game::msg_battle_player &player2 = msg2.players(i);
							player2.SerializeToString(&s2);
							if (s1 != s2)
							{
								printf("player\n");
							}
						}
					}
					if (msg1.boss_size() != msg2.boss_size())
					{
						printf("boss_size\n");
					}
					else
					{
						for (int i = 0; i < msg1.boss_size(); ++i)
						{
							std::string s1;
							const protocol::game::msg_battle_boss &player1 = msg1.boss(i);
							player1.SerializeToString(&s1);
							std::string s2;
							const protocol::game::msg_battle_boss &player2 = msg2.boss(i);
							player2.SerializeToString(&s2);
							if (s1 != s2)
							{
								printf("boss\n");
							}
						}
					}
					if (msg1.monsters_size() != msg2.monsters_size())
					{
						printf("monsters_size\n");
					}
					else
					{
						for (int i = 0; i < msg1.monsters_size(); ++i)
						{
							std::string s1;
							const protocol::game::msg_battle_monster &player1 = msg1.monsters(i);
							player1.SerializeToString(&s1);
							std::string s2;
							const protocol::game::msg_battle_monster &player2 = msg2.monsters(i);
							player2.SerializeToString(&s2);
							if (s1 != s2)
							{
								printf("monsters\n");
							}
						}
					}
					if (msg1.effects_size() != msg2.effects_size())
					{
						printf("effects_size\n");
					}
					else
					{
						for (int i = 0; i < msg1.effects_size(); ++i)
						{
							std::string s1;
							const protocol::game::msg_battle_effect &effect1 = msg1.effects(i);
							effect1.SerializeToString(&s1);
							std::string s2;
							const protocol::game::msg_battle_effect &effect2 = msg2.effects(i);
							effect2.SerializeToString(&s2);
							if (s1 != s2)
							{
								printf("effect\n");
							}
						}
					}
					if (msg1.bases_size() != msg2.bases_size())
					{
						printf("bases_size\n");
					}
					else
					{
						for (int i = 0; i < msg1.bases_size(); ++i)
						{
							std::string s1;
							const protocol::game::msg_battle_item_base &item1 = msg1.bases(i);
							item1.SerializeToString(&s1);
							std::string s2;
							const protocol::game::msg_battle_item_base &item2 = msg2.bases(i);
							item2.SerializeToString(&s2);
							if (s1 != s2)
							{
								printf("bases_size\n");
							}
						}
					}
					if (msg1.items_size() != msg2.items_size())
					{
						printf("item_size\n");
					}
					else
					{
						for (int i = 0; i < msg1.items_size(); ++i)
						{
							std::string s1;
							const protocol::game::msg_battle_item &item1 = msg1.items(i);
							item1.SerializeToString(&s1);
							std::string s2;
							const protocol::game::msg_battle_item &item2 = msg2.items(i);
							item2.SerializeToString(&s2);
							if (s1 != s2)
							{
								printf("item\n");
							}
						}
					}
					uint64_t guid = (*it).first;
					if (player_map_.find(guid) != player_map_.end())
					{
						service::udp_service()->destory(player_map_[guid].hid);
					}
				}
			}
			rs_.data = data;
			rs_.zhen = zhen;
			op_start_index_ = op_index_[zhen];
		}
		for (std::list<int>::iterator it = up_states_list_.begin(); it != up_states_list_.end();)
		{
			int zhen1 = *it;
			if (zhen1 <= zhen)
			{
				up_states_list_.erase(it++);
				up_states_map_.erase(zhen1);
			}
			else
			{
				break;
			}
		}
	}
}

void RoomPool::remove_state_player(uint64_t guid)
{
	for (std::map<int, std::map<uint64_t, std::string> >::iterator jt = up_states_map_.begin(); jt != up_states_map_.end(); ++jt)
	{
		std::map<uint64_t, std::string> &states = (*jt).second;
		states.erase(guid);
	}
}

void RoomPool::update_state()
{
	if (zhen_ % (STATE_TIME / ROOM_TICK) == 0 && player_map_.size() > 0)
	{
		std::map<uint64_t, std::string> states;
		for (std::map<uint64_t, RoomPlayer>::iterator jt = player_map_.begin(); jt != player_map_.end(); ++jt)
		{
			states[(*jt).first] = "";
		}
		/*for (std::map<uint64_t, int>::iterator jt = player_ob_.begin(); jt != player_ob_.end(); ++jt)
		{
			states[(*jt).first] = "";
		}*/
		up_states_map_[zhen_] = states;
		up_states_list_.push_back(zhen_);
		protocol::game::msg_battle_op *op = new protocol::game::msg_battle_op();
		op->set_opcode(MSG_BATTLE_STATE);
		op->set_guid(0);
		op->set_zhen(zhen_);
		RoomMessage::send_smsg_battle_op(op);
	}
	if (up_states_list_.empty())
	{
		return;
	}
	int zhen = *(up_states_list_.begin());
	check_state(zhen);
}

void RoomPool::battle_code(uint64_t guid, const protocol::game::msg_battle_code &msg)
{
	int zhen = msg.zhen();
	if (up_codes_map_.find(zhen) == up_codes_map_.end())
	{
		return;
	}
	std::map<uint64_t, int> &codes = up_codes_map_[zhen];
	if (codes.find(guid) == codes.end())
	{
		return;
	}
	if (codes[guid] != 0)
	{
		return;
	}
	codes[guid] = msg.code();
	check_code(zhen);
}

void RoomPool::check_code(int zhen)
{
	if (up_codes_map_.find(zhen) == up_codes_map_.end())
	{
		return;
	}
	std::map<uint64_t, int> &codes = up_codes_map_[zhen];
	bool flag = true;
	bool flag1 = false;
	for (std::map<uint64_t, int>::iterator it = codes.begin(); it != codes.end(); ++it)
	{
		if ((*it).second == 0)
		{
			flag = false;
			break;
		}
	}
	if (flag)
	{
		flag1 = true;
	}
	else if (zhen_ - zhen >= CHECK_MAX_TIME / ROOM_TICK)
	{
		for (std::map<uint64_t, int>::iterator it = codes.begin(); it != codes.end();)
		{
			if ((*it).second == 0)
			{
				uint64_t guid = (*it).first;
				/// 移出guid
				if (player_map_.find(guid) != player_map_.end())
				{
					RoomPlayer &rp = player_map_[guid];
					rp.no_state_num++;
					if (rp.no_state_num >= 3)
					{
						service::log()->error("state timeout destory zhen = %d, guid = %llu", zhen, guid);
						service::udp_service()->destory(rp.hid);
					}
					else
					{
						service::log()->error("state timeout zhen = %d, guid = %llu", zhen, guid);
					}
				}
				codes.erase(it++);
			}
			else
			{
				++it;
			}
		}
		flag1 = true;
	}
	if (flag1)
	{
		std::vector<tmp_code> tmp_codes;
		for (std::map<uint64_t, int>::iterator it = codes.begin(); it != codes.end(); ++it)
		{
			uint64_t guid = (*it).first;
			int s = (*it).second;
			if (player_map_.find(guid) != player_map_.end())
			{
				RoomPlayer &rp = player_map_[guid];
				rp.no_state_num = 0;
			}
			bool flag = false;
			for (int i = 0; i < tmp_codes.size(); ++i)
			{
				if (tmp_codes[i].data == s)
				{
					tmp_codes[i].num++;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				tmp_code ts;
				ts.num = 1;
				ts.data = s;
				tmp_codes.push_back(ts);
			}
		}
		bool flag = false;
		int num = 0;
		int data = 0;
		for (int i = 0; i < tmp_codes.size(); ++i)
		{
			if (!flag)
			{
				flag = true;
				num = tmp_codes[i].num;
				data = tmp_codes[i].data;
			}
			else if (tmp_codes[i].num > num)
			{
				num = tmp_codes[i].num;
				data = tmp_codes[i].data;
			}
		}
		if (flag)
		{
			for (std::map<uint64_t, int>::iterator it = codes.begin(); it != codes.end(); ++it)
			{
				uint64_t guid = (*it).first;
				int s = (*it).second;
				if (data != s)
				{
					/// 移出guid
					service::log()->error("bu tong bu zhen = %d, guid = %llu", zhen, guid);
					if (player_map_.find(guid) != player_map_.end())
					{
						service::udp_service()->destory(player_map_[guid].hid);
					}
				}
			}
		}
		for (std::list<int>::iterator it = up_codes_list_.begin(); it != up_codes_list_.end();)
		{
			int zhen1 = *it;
			if (zhen1 <= zhen)
			{
				up_codes_list_.erase(it++);
				up_codes_map_.erase(zhen1);
			}
			else
			{
				break;
			}
		}
	}
}

void RoomPool::remove_code_player(uint64_t guid)
{
	for (std::map<int, std::map<uint64_t, int> >::iterator jt = up_codes_map_.begin(); jt != up_codes_map_.end(); ++jt)
	{
		std::map<uint64_t, int> &codes = (*jt).second;
		codes.erase(guid);
	}
}

void RoomPool::update_code()
{
	if (zhen_ % (CODE_TIME / ROOM_TICK) == 0 && player_map_.size() > 0)
	{
		std::map<uint64_t, int> codes;
		for (std::map<uint64_t, RoomPlayer>::iterator jt = player_map_.begin(); jt != player_map_.end(); ++jt)
		{
			codes[(*jt).first] = 0;
		}
		/*for (std::map<uint64_t, int>::iterator jt = player_ob_.begin(); jt != player_ob_.end(); ++jt)
		{
			codes[(*jt).first] = 0;
		}*/
		up_codes_map_[zhen_] = codes;
		up_codes_list_.push_back(zhen_);
		protocol::game::msg_battle_op *op = new protocol::game::msg_battle_op();
		op->set_opcode(MSG_BATTLE_CODE);
		op->set_guid(0);
		op->set_zhen(zhen_);
		RoomMessage::send_smsg_battle_op(op);
	}
	if (up_codes_list_.empty())
	{
		return;
	}
	int zhen = *(up_codes_list_.begin());
	check_code(zhen);
}

void RoomPool::battle_end(uint64_t guid, dhc::battle_result_t &battle_result)
{
	battle_result.set_guid(battle_guid_);
	battle_result.set_type(battle_type_);
	std::string s;
	battle_result.SerializeToString(&s);
	result_map_[guid] = s;
	service::log()->debug("player set_end guid = <%llu>", guid);
}

void RoomPool::reset(uint64_t guid, WaitPlayer &player)
{
	remove_state_player(guid);
	remove_code_player(guid);
	RoomMessage::send_smsg_battle_link(guid, battle_guid_, battle_type_, team_num_, member_num_, zhen_, rs_.zhen, rs_.data, op_all_, op_start_index_, false, seed_, seed_add_, player);
}

void RoomPool::link(uint64_t guid, int hid, WaitPlayer &player)
{
	bool is_no_body = false;
	if (player_map_.empty() && player_ob_.empty())
	{
		for (int i = 0; i < op_vec_.size(); ++i)
		{
			delete op_vec_[i];
		}
		op_vec_.clear();
		rs_.zhen = zhen_;
		rs_.data = "";
		op_start_index_ = op_all_.size();
		is_no_body = true;
	}
	player_ob_[guid] = hid;
	RoomMessage::send_smsg_battle_link(guid, battle_guid_, battle_type_, team_num_, member_num_, zhen_, rs_.zhen, rs_.data, op_all_, op_start_index_, is_no_body, seed_, seed_add_, player);
}

void RoomPool::add_player(uint64_t guid, int hid, WaitPlayer &player)
{
	if (player_ob_.find(guid) == player_ob_.end())
	{
		return;
	}
	player_ob_.erase(guid);

	del_player(guid);

	RoomPlayer rp;
	rp.hid = hid;
	rp.guid = guid;
	rp.no_state_num = 0;
	player_map_[guid] = rp;

	protocol::game::msg_battle_op *op = new protocol::game::msg_battle_op();
	op->set_opcode(MSG_BATTLE_IN);
	op->set_guid(guid);
	op->add_param_ints(player.player.camp());
	op->add_param_ints(player.player.role_id());
	op->add_param_ints(player.player.role_level());
	op->add_param_strings(player.player.name());
	op->add_param_ints(player.player.sex());
	op->add_param_ints(player.player.avatar());
	op->add_param_ints(player.player.cup());
	op->add_param_ints(player.player.toukuang());
	op->add_param_ints(player.player.region_id());
	op->add_param_ints(re_tid_);
	op->add_param_ints(player.player.name_color());
	re_tid_ += 100000;
	op->add_param_ints(player.player.attr_type_size());
	for (int i = 0; i < player.player.attr_type_size(); ++i)
	{
		op->add_param_ints(player.player.attr_type(i));
		op->add_param_ints(player.player.attr_param1(i));
		op->add_param_ints(player.player.attr_param2(i));
		op->add_param_ints(player.player.attr_param3(i));
	}
	op->add_param_ints(player.player.fashion_size());
	for (int i = 0; i < player.player.fashion_size(); ++i)
	{
		op->add_param_ints(player.player.fashion(i));
	}
	add_operation(op);
	player.is_new = 0;
}

void RoomPool::del_player(uint64_t guid)
{
	if (player_map_.find(guid) != player_map_.end())
	{
		player_map_.erase(guid);
		remove_state_player(guid);
		remove_code_player(guid);
		protocol::game::msg_battle_op *op = new protocol::game::msg_battle_op();
		op->set_opcode(MSG_BATTLE_OUT);
		op->set_guid(guid);
		add_operation(op);
	}
	if (player_ob_.find(guid) != player_ob_.end())
	{
		player_ob_.erase(guid);
	}
}

int RoomPool::get_hid(uint64_t guid)
{
	if (player_ob_.find(guid) != player_ob_.end())
	{
		return player_ob_[guid];
	}
	if (player_map_.find(guid) != player_map_.end())
	{
		return player_map_[guid].hid;
	}
	return -1;
}

void RoomPool::get_hids(std::vector<int> &hids)
{
	for (std::map<uint64_t, int>::iterator it = player_ob_.begin(); it != player_ob_.end(); ++it)
	{
		hids.push_back((*it).second);
	}
	for (std::map<uint64_t, RoomPlayer>::iterator it = player_map_.begin(); it != player_map_.end(); ++it)
	{
		hids.push_back((*it).second.hid);
	}
}

void RoomPool::add_operation(protocol::game::msg_battle_op *op)
{
	op_vec_.push_back(op);
}

void RoomPool::do_battle_end()
{
	std::vector<tmp_result> tmp_results;
	for (std::map<uint64_t, std::string>::iterator it = result_map_.begin(); it != result_map_.end(); ++it)
	{
		std::string s = (*it).second;
		bool flag = false;
		for (int i = 0; i < tmp_results.size(); ++i)
		{
			if (tmp_results[i].data == s)
			{
				tmp_results[i].num++;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			tmp_result tr;
			tr.num = 1;
			tr.data = s;
			tmp_results.push_back(tr);
		}
	}
	bool flag = false;
	int num = 0;
	std::string data = "";
	for (int i = 0; i < tmp_results.size(); ++i)
	{
		if (!flag)
		{
			flag = true;
			num = tmp_results[i].num;
			data = tmp_results[i].data;
		}
		else if (tmp_results[i].num > num)
		{
			num = tmp_results[i].num;
			data = tmp_results[i].data;
		}
	}
	if (flag)
	{
		RoomMessage::send_push_rm_rc_battle_end(battle_guid_, master_name_, data);
	}
	else
	{
		service::log()->error("result empty battle_guid = <%llu>", battle_guid_);
		RoomMessage::send_push_rm_rc_battle_end(battle_guid_, master_name_, "");
	}
}

void RoomPool::update()
{
	if (zhen_ > MAX_TIME / ROOM_TICK)
	{
		if (is_end_ == 0)
		{
			RoomMessage::send_smsg_battle_finish();
			is_end_ = 1;
		}
		if (zhen_ >= (MAX_TIME + 5000) / ROOM_TICK && is_end_ == 1)
		{
			do_battle_end();
			is_end_ = 2;
		}
		zhen_++;
		return;
	}
	op_index_.push_back(op_all_.size());
	for (int i = 0; i < op_vec_.size(); ++i)
	{
		protocol::game::msg_battle_op *op = op_vec_[i];
		op->set_zhen(zhen_);
		RoomMessage::send_smsg_battle_op(op);
		op_all_.push_back(op);
	}
	op_vec_.clear();
	RoomMessage::send_smsg_battle_zhen();

	zhen_++;
	if (zhen_ % (STATE_TIME / ROOM_TICK) == 0)
	{
		update_state();
	}
	if (zhen_ % (CODE_TIME / ROOM_TICK) == 0)
	{
		update_code();
	}
}
