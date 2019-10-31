Message = {}

local handles = {}
local net_handles = {}
local cur_reopcode_ = {}
local beat_time_ = 0
local send_time_ = 0
local adjust_time_ = 0

function Message.Init()
	shareMgr.ShareFunc = Message.ShareSuccess
	timerMgr:AddRepeatTimer('Message', Message.Update , 1, 1)
	Message.register_net_handle(opcodes.SMSG_ERROR, Message.SMSG_ERROR)
	Message.register_net_handle(opcodes.SMSG_GM_COMMAND , Message.SMSG_GM_COMMAND)
	Message.register_net_handle(opcodes.SMSG_LOGIN_PLAYER, Message.SMSG_LOGIN_PLAYER)
	Message.register_net_handle(opcodes.SMSG_BATTLE_END, Message.SMSG_BATTLE_END)
	Message.register_net_handle(opcodes.SMSG_OFFLINE_BATTLE_END, Message.SMSG_OFFLINE_BATTLE_END)
	Message.register_net_handle(opcodes.SMSG_FENXIANG, Message.SMSG_FENXIANG)
	Message.register_net_handle(opcodes.SMSG_CHECKDATA, Message.SMSG_CHECKDATA)
	Message.register_net_handle(opcodes.SMSG_FENXIANG_NUM, Message.SMSG_FENXIANG_NUM)
	Message.register_net_handle(opcodes.SMSG_POST_NUM, Message.SMSG_POST_NUM)
	Message.register_net_handle(opcodes.SMSG_HAS_BATTLE, Message.SMSG_HAS_BATTLE)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_ADD, Message.SMSG_SOCIAL_ADD)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_GIFT_RECEIVE, Message.SMSG_SOCIAL_GIFT_RECEIVE)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_APPLY_RECEIVE, Message.SMSG_SOCIAL_APPLY_RECEIVE)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_DATA, Message.SMSG_SOCIAL_DATA)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_STAT, Message.SMSG_SOCIAL_STAT)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_DELETE, Message.SMSG_SOCIAL_DELETE)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_BLACK, Message.SMSG_SOCIAL_BLACK)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_GOLD, Message.SMSG_SOCIAL_GOLD)
	Message.register_net_handle(opcodes.SMSG_RECHARGE, Message.SMSG_RECHARGE)
	Message.register_handle("edit_gm_command", Message.edit_gm_command)
end

local normal_guis = {"IconPanel","AvaIconPanel","NoticePanel","AchieveAnimation"}

function Message.SMSG_RECHARGE(message)
	if GUIRoot.HasGUI('ShopPanel') then
		return
	end
	
	local msg = msg_hall_pb.smsg_recharge()
	msg:ParseFromString(message.luabuff)
	GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('Message_001')})
	if(self.player.first_recharge == 0) then
		self.player.first_recharge = 1
	end
	local recharge_temp = Config.get_t_recharge(msg.rid)
	local rewards = {}
	if(recharge_temp.type == 1) then
		local reward = {}
		reward.type = 1
		reward.value1 = 2
		reward.value2 = recharge_temp.value
		reward.value3 = 0
		table.insert(rewards, reward)
	else
		rewards = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
		if(recharge_temp.type == 2) then
			self.player.yue_time = tonumber(timerMgr:now_string()) + 86400000 * recharge_temp.value
		else
			self.player.nian_time = tonumber(timerMgr:now_string()) + 86400000 * recharge_temp.value
			if tonumber(self.player.yue_time) > tonumber(timerMgr:now_string()) then
				local reward = {}
				reward.type = 1
				reward.value1 = 2
				reward.value2 = math.floor((tonumber(self.player.yue_time ) - tonumber(timerMgr:now_string())) / 8640000)
				reward.value3 = 0
				table.insert(rewards, reward)
				self.player.yue_time = 0
			end
		end
	end
	for i = 1, #rewards do
		self.add_reward(rewards[i])
	end
	for i = 1, #msg.roles do
		self.add_role(msg.roles[i])
	end
	
	local now_guis = {}
	
	for k,v in pairs(GUIRoot.guis) do
		if normal_guis[k] == nil then
			table.insert(now_guis,k)
		end
	end
	
	if not Battle.is_online then
		if(#msg.roles > 0) then
			GUIRoot.ShowGUI("RoleGetPanel", {msg.roles, now_guis, rewards})
		else
			if(#rewards > 0) then
				GUIRoot.ShowGUI('GainPanel', {rewards})
			end
		end
	end
	
	self.player.total_recharge = self.player.total_recharge + recharge_temp.value
end

function Message.Update()
	if GameTcp.Isconnect() then
		beat_time_ = beat_time_ + 1
		if(beat_time_ >= 5) then
			beat_time_ = 0
			GameTcp.Send(opcodes.CMSG_BEAT)
		end
		if send_time_ > 0 then
			send_time_ = send_time_ - 1
			if send_time_ <= 0 then
				send_time_ = 0
				GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('Message_002'), Config.get_t_script_str('Message_003')})
				GUIRoot.HideGUI('MaskPanel')
			end
		end
		if(self.player ~= nil and platform_config_common.m_debug) then
			adjust_time_ = adjust_time_ + 1
			if(adjust_time_ >= 2) then
				adjust_time_ = 0
				GameTcp.Send(opcodes.CMSG_CHECKDATA)
			end
		end
	end
end

function Message.SMSG_ERROR(message)
	send_time_ = 0
	GUIRoot.HideGUI('MaskPanel')
	cur_reopcode_ = {}
	if State.cur_state == State.state.ss_login then
		LoginPanel.Stop()
	end
	local msg = msg_hall_pb.smsg_error()
	msg:ParseFromString(message.luabuff)
	if msg.code == opcodes.ERROR_SYSTEM then
		GUIRoot.ShowGUI('MessagePanel', {msg.text})
	elseif msg.code == opcodes.ERROR_LOGIN_CODE then
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('Message_004'), Config.get_t_script_str('Message_005'), State.Reset})
	elseif msg.code == opcodes.ERROR_LOGIN_OTHER then
		--账号在别处登录
		if State.cur_state == State.state.ss_login then
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('Message_006'), Config.get_t_script_str('Message_003')})
		elseif State.cur_state == State.state.ss_hall then
			GameTcp.Disconnect()
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('Message_006'), Config.get_t_script_str('Message_005'), GameTcp.ReConnect})
		elseif State.cur_state == State.state.ss_battle then
			GameTcp.Disconnect()
			BattleTcp.Disconnect1()
		elseif State.cur_state == State.state.ss_ofbattle then
			GameTcp.Disconnect()
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('Message_006'), Config.get_t_script_str('Message_007'), BattleTcp.DisconnectClick})
		end
	else
		GUIRoot.ShowGUI('MessagePanel', {Config.get_t_error(msg.code)})
	end
end

function Message.SMSG_LOGIN_PLAYER(message)
	if(State.cur_state == State.state.ss_hall) then
		self.ClearData()
		local msg = msg_hall_pb.smsg_login_player()
		msg:ParseFromString(message.luabuff)
		self.player = msg.player
		self.post_num = msg.post_num
		timerMgr:set_server_time(msg.server_time)
		for i = 1, #msg.roles do
			self.add_role(msg.roles[i])
		end
		for i = 1, #msg.battle_his do
			self.add_battle_result(msg.battle_his[i])
		end
		PlayerData.check_role()
		ShopPanel.RefreshRollReward()
	end
end

function Message.SMSG_GM_COMMAND(message)
	local msg = msg_hall_pb.smsg_gm_command()
	msg:ParseFromString(message.luabuff)
	local item_reward = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
	for i = 1, #item_reward do
		self.add_reward(item_reward[i])
	end
	for i = 1, #msg.roles do
		self.add_role(msg.roles[i])
	end
	if (#item_reward > 0) then
		GUIRoot.ShowGUI("GainPanel", {item_reward})
	end
end

function Message.SMSG_BATTLE_END(message)
	local msg = msg_battle_pb.smsg_battle_end()
	msg:ParseFromString(message.luabuff)
	self.add_battle_result(msg.battle_his)
	self.add_chest(msg.box_id)
	if(self.player.box_zd_opened == 0 and self.player.box_zd_num < 3) then
		self.player.box_zd_num = self.player.box_zd_num + 1
	end
	local old_cup = self.player.cup
	self.set_cup(msg.cup)
	if old_cup < msg.cup then
		self.add_all_type_num(37, 1)
	end
	self.add_resource(1, msg.gold)
	self.add_resource(3, msg.exp)
	self.player.battle_gold = self.player.battle_gold + msg.gold
	self.player.battle_num = self.player.battle_num + 1
	if msg.battle_his.type == 0 then
		self.add_all_type_num(30, 1)
	else
		self.add_all_type_num(130, 1)
	end
	LuaAchieve.TTypeChange()
	HallPanel.tip_battle_message = nil
	HallPanel.tip_battle_flag = false
end

function Message.SMSG_OFFLINE_BATTLE_END(message)
	local msg = msg_battle_pb.smsg_offline_battle_end()
	msg:ParseFromString(message.luabuff)
	if(self.player.level < 3) then
		if(self.player.box_zd_opened == 0 and self.player.box_zd_num < 3) then
			self.player.box_zd_num = self.player.box_zd_num + 1
		end
		local old_cup = self.player.cup
		self.set_cup(msg.cup)
		if old_cup < msg.cup then
			self.add_all_type_num(37, 1)
		end
		self.add_resource(3, msg.exp)
	end
	self.add_chest(msg.box_id)
	self.add_resource(1, msg.gold)
	self.player.battle_gold = self.player.battle_gold + msg.gold
	self.add_all_type_num(230, 1)
	LuaAchieve.TTypeChange()
end

function Message.SMSG_FENXIANG_NUM(message)
	local msg = msg_hall_pb.smsg_fenxiang_num()
	msg:ParseFromString(message.luabuff)
	self.add_resource(4, msg.fenxiang_num - self.player.fenxiang_num)
	self.player.fenxiang_num = msg.fenxiang_num
	self.player.fenxiang_total_num = msg.fenxiang_total_num
	SharePanel.InitPanel()
end

function Message.SMSG_CHECKDATA(message)
	local msg = msg_hall_pb.smsg_checkdata()
	msg:ParseFromString(message.luabuff)
	self.adjust_data(msg.player, msg.roles)
end

function Message.SMSG_POST_NUM(message)
	local msg = msg_hall_pb.smsg_post_num()
	msg:ParseFromString(message.luabuff)
	self.post_num = msg.post_num
	HallPanel.ShowTip()
end

function Message.SMSG_FENXIANG()
	if(self.player.fenxiang_state == 0) then
		self.player.fenxiang_state = 1
		self.add_all_type_num(43, 1)
		HallPanel.ShowTip()
	end
end

function Message.SMSG_HAS_BATTLE(message)
	HallPanel.tip_battle_message = message
	HallPanel.tip_battle_flag = true
end

function Message.SMSG_SOCIAL_ADD(message)
	local msg = msg_social_pb.smsg_social_add()
	msg:ParseFromString(message.luabuff)
	if(State.cur_state == State.state.ss_hall and not GUIRoot.HasGUI("LoadPanel")) then
		local content = string.format(Config.get_t_script_str('Message_008'),msg.social.name)
		GUIRoot.ShowGUI("MessagePanel", {content})
	end
	self.del_social_guid(3, msg.social.target_guid)
	self.add_social_guid(2, msg.social.target_guid)
	self.add_all_type_num(42,1)
	ZonePanel.AddFriendEnd(msg.social.target_guid)
	FriendPanel.SMSG_SOCIAL_ADD(msg)
	TeamPanel.SMSG_SOCIAL_ADD(msg)
	NoticePanel.SaveSocialList()
end

function Message.SMSG_SOCIAL_DELETE(message)
	local msg = msg_social_pb.smsg_social_delete()
	msg:ParseFromString(message.luabuff)
	if(self.social_type(msg.target_guid) == 2) then
		self.del_social_guid(2, msg.target_guid)
		self.unread_msg_num = self.unread_msg_num - FriendPanel.GetUnreadNum(msg.target_guid)
		FriendPanel.AddUnreadMsg(msg.target_guid, -FriendPanel.GetUnreadNum(msg.target_guid))
		NoticePanel.DelSocialMsg(msg.target_guid)
		NoticePanel.SaveSocialList()
	end
	if(msg.target_guid == self.player.guid) then
		self.apply_num = self.apply_num - 1
		if(self.apply_num < 0) then
			self.apply_num = 0
		end
	end
	FriendPanel.SMSG_SOCIAL_DELETE(msg)
	TeamPanel.SMSG_SOCIAL_DELETE(msg)
	HallPanel.ShowTip()
	FriendPanel.ShowTip()
end

function Message.SMSG_SOCIAL_BLACK(message)
	local msg = msg_social_pb.smsg_social_add()
	msg:ParseFromString(message.luabuff)
	
	local content = string.format(Config.get_t_script_str('Message_009'),msg.social.name)
	GUIRoot.ShowGUI("MessagePanel", {content})
	
	FriendPanel.AddBlack(msg.social)
	NoticePanel.DelSocialMsg(msg.social.target_guid)
	ZonePanel.SMSG_SOCIAL_BLACK()
	TeamPanel.SMSG_SOCIAL_BLACK(msg)
	NoticePanel.SaveSocialList()
	HallPanel.ShowTip()
	FriendPanel.ShowTip()
end

function Message.SMSG_SOCIAL_GIFT_RECEIVE(message)
	local msg = msg_social_pb.smsg_social_gift_receive()
	msg:ParseFromString(message.luabuff)
	self.social_gold = true
	FriendPanel.ReceiveGift(msg)
	HallPanel.ShowTip()
	FriendPanel.ShowTip()
end

function Message.SMSG_SOCIAL_APPLY_RECEIVE(message)
	local msg = msg_social_pb.smsg_social_apply()
	msg:ParseFromString(message.luabuff)
	self.apply_num = msg.num
	HallPanel.ShowTip()
	FriendPanel.ShowTip()
end

function Message.SMSG_SOCIAL_DATA(message)
	local msg = msg_social_pb.smsg_social_data()
	msg:ParseFromString(message.luabuff)
	self.apply_num = msg.apply_num
	self.friend_guids = msg.friend_guids
	self.black_guids = msg.black_guids
	self.social_gold = msg.social_gold
	self.unread_msg_num = self.unread_msg_num + msg.msg_num
	self.reject = msg.reject
	NoticePanel.CheckSocialList()
	HallPanel.ShowTip()
	FriendPanel.ShowTip()
end

function Message.SMSG_SOCIAL_STAT(message)
	local msg = msg_social_pb.smsg_social_stat()
	msg:ParseFromString(message.luabuff)
	if(State.cur_state == State.state.ss_hall and not GUIRoot.HasGUI("LoadPanel")) then
		if(self.social_type(msg.target_guid) == 2 and luabit.band(msg.stat, 2) ~= 0) then
			local content = string.format(Config.get_t_script_str('Message_010'),msg.name)
			GUIRoot.ShowGUI("MessagePanel", {content})
		end
		FriendPanel.SMSG_SOCIAL_STAT(msg)
		TeamPanel.SMSG_SOCIAL_STAT(msg)
	end
end

function Message.SMSG_SOCIAL_GOLD(message)
	local msg = msg_social_pb.smsg_social_gold()
	msg:ParseFromString(message.luabuff)
	if(msg.gold > 0) then
		self.social_gold = true
	else
		self.social_gold = false
	end
end

function Message.ShareSuccess()
	if GameTcp.Isconnect() and self.player.fenxiang_state == 0 then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('Message_011')})
		GameTcp.Send(opcodes.CMSG_FENXIANG, nil, {opcodes.SMSG_FENXIANG})
	end
end

function Message.edit_gm_command(message)
	local s = tostring(message.m_object[0])
	local msg = msg_hall_pb.cmsg_gm_command()
	msg.text = s
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_GM_COMMAND, data)
end

function Message.register_handle(name, func)
	if handles[name] == nil then
		handles[name] = { func }
	else
		table.insert(handles[name], func)
	end
end

function Message.remove_handle(name, func)
	if handles[name] ~= nil then
		for i = 1, #handles[name] do
			if (handles[name][i] == func) then
				table.remove(handles[name], i)
				break
			end
		end
	end
end

function Message.add_message(message)
	messageMgr:AddMessage(message)
end

function Message.register_net_handle(opcode, func)
	local name = tostring(opcode)
	if net_handles[name] == nil then
		net_handles[name] = { func }
	else
		table.insert(net_handles[name], func)
	end
end

function Message.remove_net_handle(opcode, func)
	local name = tostring(opcode)
	if net_handles[name] ~= nil then
		for i = 1, #net_handles[name] do
			if (net_handles[name][i] == func) then
				table.remove(net_handles[name], i)
				break
			end
		end
	end
end

function Message.OnMessage(message)
	local name = message.name
	if handles[name] ~= nil then
		for i = 1, #handles[name] do
			handles[name][i](message)
		end
	end 
end

function Message.OnNetMessage(message)
	local opcode = message.opcode
	local name = tostring(opcode)
	for i = 1, #cur_reopcode_ do
		if cur_reopcode_[i] == opcode then
			GUIRoot.HideGUI('MaskPanel')
			send_time_ = 0
			cur_reopcode_ = {}
			break
		end
	end
	if net_handles[name] ~= nil then
		for i = 1, #net_handles[name] do
			net_handles[name][i](message)
		end
	end
end

function Message.OnMask(smsgs, text, time)
	cur_reopcode_ = smsgs
	send_time_ = time
	GUIRoot.ShowGUI('MaskPanel', {text})
end
