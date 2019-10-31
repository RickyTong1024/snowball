MailPanel = {}

local lua_script_

local mail_list_ = {}

local detail_panel_
local empty_panel_
local mail_res_
local mail_view_
local reward_root_
local view_softness = Vector2(0, 10)
local view_pos_ = Vector3(0, 0, 0)

local mail_select_id_ = 0
local mail_pos_ = 0

local pre_select_mail = nil

local space_y_ = 62

function MailPanel.Awake(obj)
	GUIRoot.UIEffect(obj, 0)
	GUIRoot.ShowGUI('BackPanel',{3})
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	detail_panel_ = obj.transform:Find('detail_panel')
	empty_panel_ = obj.transform:Find('empty_panel')
	mail_res_ = obj.transform:Find('mail_panel/mail_res')
	mail_view_ = obj.transform:Find('mail_panel/mail_view')
	reward_root_ = detail_panel_:Find('reward_root')
	
	view_pos_ = mail_view_.transform.localPosition
	mail_view_:GetComponent("UIPanel").clipSoftness = view_softness
	
	local get_all_btn = obj.transform:Find('mail_panel/get_all_btn')
	local del_all_btn = obj.transform:Find('mail_panel/del_all_btn')
	local get_btn = detail_panel_:Find('get_btn')
	local del_btn = detail_panel_:Find('del_btn')
	
	lua_script_:AddButtonEvent(get_all_btn.gameObject, "click", MailPanel.Click)
	lua_script_:AddButtonEvent(get_btn.gameObject, "click", MailPanel.Click)
	lua_script_:AddButtonEvent(del_all_btn.gameObject, "click", MailPanel.Click)
	lua_script_:AddButtonEvent(del_btn.gameObject, "click", MailPanel.Click)
	lua_script_:AddButtonEvent(mail_view_.gameObject, "onDragFinished", MailPanel.OnDragFinished)
	
	detail_panel_.gameObject:SetActive(false)
	empty_panel_.gameObject:SetActive(false)
	
	timerMgr:AddRepeatTimer("MailPanel", MailPanel.Refresh, 60, 60)
	MailPanel.RegisterMessage()
	
	MailPanel.InitMailList()
end

function MailPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_POST_READ, MailPanel.SMSG_POST_READ)
	Message.register_net_handle(opcodes.SMSG_POST_GET, MailPanel.SMSG_POST_GET)
	Message.register_net_handle(opcodes.SMSG_POST_DELETE, MailPanel.SMSG_POST_DELETE)
	Message.register_net_handle(opcodes.SMSG_POST_GET_ALL, MailPanel.SMSG_POST_GET_ALL)
	Message.register_net_handle(opcodes.SMSG_POST_DELETE_ALL, MailPanel.SMSG_POST_DELETE_ALL)
	Message.register_handle("back_panel_msg", MailPanel.Back)
	Message.register_handle("back_panel_recharge", MailPanel.Recharge)
	Message.register_handle("team_join_msg", MailPanel.TeamJoin)
end

function MailPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_POST_READ, MailPanel.SMSG_POST_READ)
	Message.remove_net_handle(opcodes.SMSG_POST_GET, MailPanel.SMSG_POST_GET)
	Message.remove_net_handle(opcodes.SMSG_POST_DELETE, MailPanel.SMSG_POST_DELETE)
	Message.remove_net_handle(opcodes.SMSG_POST_GET_ALL, MailPanel.SMSG_POST_GET_ALL)
	Message.remove_net_handle(opcodes.SMSG_POST_DELETE_ALL, MailPanel.SMSG_POST_DELETE_ALL)
	Message.remove_handle("back_panel_msg", MailPanel.Back)
	Message.remove_handle("back_panel_recharge", MailPanel.Recharge)
	Message.remove_handle("team_join_msg", MailPanel.TeamJoin)
end

function MailPanel.OnDestroy()
	mail_select_id_ = 0
	pre_select_mail = nil
	mail_pos_ = 0
	mail_list_ = {}
	MailPanel.RemoveMessage()
	timerMgr:RemoveRepeatTimer("MailPanel")
end

function MailPanel.OnParam(parm)
	mail_list_ = MailPanel.RankMail(parm[1])
	MailPanel.InitMailList(true)
end

function MailPanel.Back()
	GUIRoot.HideGUI('MailPanel')
	GUIRoot.HideGUI("BackPanel")
	GUIRoot.ShowGUI('HallPanel')
end

function MailPanel.Recharge()
	GUIRoot.HideGUI('MailPanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function MailPanel.TeamJoin()
	GUIRoot.HideGUI('MailPanel')
end

-------------刷新列表-----------------

function MailPanel.InitMailList(is_delete)
	empty_panel_.gameObject:SetActive(false)
	if(is_delete) then
		detail_panel_.gameObject:SetActive(false)
		pre_select_mail = nil
		mail_pos_ = 0
		if(mail_view_.childCount > 0) then
			for i = 0, mail_view_.childCount - 1 do
				GameObject.Destroy(mail_view_:GetChild(i).gameObject)
			end
		end
		if mail_view_:GetComponent('SpringPanel') ~= nil then
			mail_view_:GetComponent('SpringPanel').enabled = false
		end
		local uv = mail_view_:GetComponent('UIScrollView')
		uv.panel.clipOffset = Vector2(0, 0)
		mail_view_.localPosition = view_pos_
	end
	if(#mail_list_ == 0) then
		empty_panel_.gameObject:SetActive(true)
	end
	for i = 0, 9 do
		if(mail_pos_ < #mail_list_) then
			local mail_temp = mail_list_[mail_pos_ + 1]
			local mail_t = LuaHelper.Instantiate(mail_res_.gameObject)
			local mask = mail_t.transform:Find('mask')
			local yd_icon = mail_t.transform:Find('yd_icon')
			local wd_icon = mail_t.transform:Find('wd_icon')
			local pick_tip = mail_t.transform:Find('pick_tip')
			local title = mail_t.transform:Find('title'):GetComponent('UILabel')
			local time_ = mail_t.transform:Find('time'):GetComponent('UILabel')
			yd_icon.gameObject:SetActive(false)
			wd_icon.gameObject:SetActive(false)
			pick_tip.gameObject:SetActive(false)
			if(mail_temp.is_read == 0) then
				wd_icon.gameObject:SetActive(true)
			else
				yd_icon.gameObject:SetActive(true)
			end
			if(#mail_temp.type > 0) then
				pick_tip.gameObject:SetActive(true)
				if(mail_temp.is_pick == 0) then
					mask.gameObject:SetActive(false)
				else
					mask.gameObject:SetActive(true)
				end
			else
				if(mail_temp.is_read == 0) then
					mask.gameObject:SetActive(false)
				else
					mask.gameObject:SetActive(true)
				end
				pick_tip.gameObject:SetActive(false)
			end
			mail_t.transform.parent = mail_view_
			mail_t.transform.localPosition = Vector3(-2, 182 - mail_pos_ * space_y_, 0)
			mail_t.transform.localScale = Vector3.one
			mail_t.name = mail_temp.guid
			title.text = mail_temp.title
			time_.text = get_time_show(mail_temp.send_date)
			lua_script_:AddButtonEvent(mail_t, "click", MailPanel.SelectMail)
			mail_t.gameObject:SetActive(true)
			mail_pos_ = mail_pos_ + 1
		end
	end
end

function MailPanel.InitDetailPanel(mail_id)
	detail_panel_.gameObject:SetActive(false)
	detail_panel_:Find('get_btn').gameObject:SetActive(false)
	detail_panel_:Find('del_btn').gameObject:SetActive(false)
	if(reward_root_.childCount > 0) then
		for i = 0, reward_root_.childCount - 1 do
            GameObject.Destroy(reward_root_:GetChild(i).gameObject)
        end
	end
	local mail_temp = MailPanel.get_mail(mail_id)
	if(mail_temp ~= nil) then
		if(mail_temp.is_read == 0) then
			local msg = msg_hall_pb.cmsg_post_read()
			msg.post_guid = mail_select_id_
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_POST_READ, data, {opcodes.SMSG_POST_READ})
		end
		for i = 1, #mail_temp.type do
			local reward_temp = Config.get_t_reward(mail_temp.type[i], mail_temp.value1[i])
			if(reward_temp ~= nil) then
				local reward_t = IconPanel.GetIcon("reward_res", nil, reward_temp.icon, reward_temp.color, mail_temp.value2[i])
				reward_t.transform:Find("icon").name = mail_temp.type[i].."+"..reward_temp.id
				reward_t.transform.parent = reward_root_
				reward_t.transform.localPosition = Vector3((i - 1) * 90 + 14, 0, 0)
				reward_t.transform.localScale = Vector3.one
				if(mail_temp.is_pick == 1) then
					reward_t.transform:Find("mask").gameObject:SetActive(true)
				end
				reward_t.gameObject:SetActive(true)
			end
		end
		if(mail_temp.is_pick == 0) then
			detail_panel_:Find('get_btn').gameObject:SetActive(true)
		else
			detail_panel_:Find('del_btn').gameObject:SetActive(true)
		end
		local desc_label = detail_panel_:Find('desc'):GetComponent('UILabel')
		local time_label = detail_panel_:Find('limit_time'):GetComponent('UILabel')
		local title_label = detail_panel_:Find('title'):GetComponent('UILabel')
		local time_ = tonumber(mail_temp.send_date) + 7 * 86400000 - tonumber(timerMgr:now_string())
		desc_label.text = mail_temp.text
		title_label.text = mail_temp.title
		time_label.text = count_time_day(time_, 3)
		detail_panel_.gameObject:SetActive(true)
	end
end

---------------------------------------


-------------------------ButtonEvent--------------------
function MailPanel.OnDragFinished()
	local uv = mail_view_:GetComponent('UIScrollView')
	local constraint = uv.panel:CalculateConstrainOffset(uv.bounds.min, uv.bounds.max)
	if(constraint.y < -10) then
		if mail_view_:GetComponent('SpringPanel') ~= nil then
			mail_view_:GetComponent('SpringPanel').enabled = false
		end
		MailPanel.InitMailList(false)
	end
end

function MailPanel.Click(obj)
	if(obj.name == 'get_btn') then
		MailPanel.GetReward()
	elseif(obj.name == 'del_btn') then
		MailPanel.DelMail()
	elseif(obj.name == 'get_all_btn') then
		MailPanel.TipGetAll()
	elseif(obj.name == 'del_all_btn') then
		MailPanel.TipDelAll()
	else
	
	end
end

function MailPanel.GetAll()
	if(#mail_list_ > 0) then
		GameTcp.Send(opcodes.CMSG_POST_GET_ALL, nil, {opcodes.SMSG_POST_GET_ALL})
	else
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('MailPanel_001')})
	end
end

function MailPanel.DelAll()
	if(#mail_list_ > 0) then
		GameTcp.Send(opcodes.CMSG_POST_DELETE_ALL, nil, {opcodes.SMSG_POST_DELETE_ALL})
	else
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('MailPanel_001')})
	end
end

function MailPanel.GetReward()
	local mail_temp = MailPanel.get_mail(mail_select_id_)
	if(mail_temp ~= nil) then
		local msg = msg_hall_pb.cmsg_post_get()
		msg.post_guid = mail_select_id_
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_POST_GET, data, {opcodes.SMSG_POST_GET})
	end
end

function MailPanel.DelMail()
	local mail_temp = MailPanel.get_mail(mail_select_id_)
	if(mail_temp ~= nil) then
		if(mail_temp.is_pick == 1 and mail_temp.is_read == 1)  then
			local msg = msg_hall_pb.cmsg_post_delete()
			msg.post_guid = mail_select_id_
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_POST_DELETE, data, {opcodes.SMSG_POST_DELETE})
		end
	end
end

function MailPanel.SelectMail(obj)
	if(pre_select_mail ~= nil) then
		pre_select_mail.transform:Find('select_tip').gameObject:SetActive(false)
	end
	obj.transform:Find('select_tip').gameObject:SetActive(true)
	obj.transform:Find('yd_icon').gameObject:SetActive(true)
	obj.transform:Find('wd_icon').gameObject:SetActive(false)
	pre_select_mail = obj
	mail_select_id_ = obj.name
	MailPanel.InitDetailPanel(mail_select_id_)
end

function MailPanel.TipGetAll()
	GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('MailPanel_002'), Config.get_t_script_str('MailPanel_003'), MailPanel.GetAll, Config.get_t_script_str('MailPanel_004')})
end

function MailPanel.TipDelAll()
	GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('MailPanel_005'), Config.get_t_script_str('MailPanel_003'), MailPanel.DelAll, Config.get_t_script_str('MailPanel_004')})
end
----------------------------------------------------------

--------------------服务器code----------------------------
function MailPanel.SMSG_POST_READ()
	local mail_temp = MailPanel.get_mail(mail_select_id_)
	if(mail_temp ~= nil) then
		mail_temp.is_read = 1
		if(#mail_temp.type == 0) then
			pre_select_mail.transform:Find('mask').gameObject:SetActive(true)
			self.post_num = self.post_num - 1
			if(self.post_num < 0) then
				self.post_num = 0
			end
		end
	end
end

function MailPanel.SMSG_POST_GET(message)
	local mail_temp = MailPanel.get_mail(mail_select_id_)
	if(mail_temp ~= nil) then
		mail_temp.is_pick = 1
		self.post_num = self.post_num - 1
		if(self.post_num < 0) then
			self.post_num = 0
		end
		pre_select_mail.transform:Find('mask').gameObject:SetActive(true)
		MailPanel.InitDetailPanel(mail_select_id_)
	end
	local msg = msg_hall_pb.smsg_post_get()
	msg:ParseFromString(message.luabuff)
	local mail_rewad = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
	for i = 1, #mail_rewad do
		self.add_reward(mail_rewad[i])
	end
	for i = 1, #msg.roles do
		self.add_role(msg.roles[i])
	end
	if(#mail_rewad > 0) then
		GUIRoot.ShowGUI('GainPanel', {mail_rewad})
	end
	if(#msg.roles > 0) then
		GUIRoot.ShowGUI("RoleGetPanel", {msg.roles, {"MailPanel", "BackPanel"}})
	end
end

function MailPanel.SMSG_POST_DELETE()
	local mail_temp = MailPanel.get_mail(mail_select_id_)
	if(mail_temp ~= nil) then
		MailPanel.remove_mail(mail_select_id_)
		mail_list_ = MailPanel.RankMail(mail_list_)
		MailPanel.InitMailList(true)
	end
end

function MailPanel.SMSG_POST_GET_ALL(message)
	local msg = msg_hall_pb.smsg_post_get_all()
	msg:ParseFromString(message.luabuff)
	for i = 1, #msg.post_guids do
		local mail_temp = MailPanel.get_mail(msg.post_guids[i])
		if(mail_temp ~= nil) then
			mail_temp.is_pick = 1
			mail_temp.is_read = 1
			self.post_num = self.post_num - 1
			if(self.post_num < 0) then
				self.post_num = 0
			end
		end
	end
	local mail_rewad = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
	for i = 1, #mail_rewad do
		self.add_reward(mail_rewad[i])
	end
	if(#mail_rewad > 0) then
		GUIRoot.ShowGUI('GainPanel', {mail_rewad})
	end
	for i = 1, #msg.roles do
		self.add_role(msg.roles[i])
	end
	mail_list_ = MailPanel.RankMail(mail_list_)
	MailPanel.InitMailList(true)
	if(#msg.roles > 0) then
		GUIRoot.ShowGUI("RoleGetPanel", {msg.roles, {"MailPanel", "BackPanel"}})
	end
end

function MailPanel.SMSG_POST_DELETE_ALL(message)
	local msg = msg_hall_pb.smsg_post_delete_all()
	msg:ParseFromString(message.luabuff)
	for i = 1, #msg.post_guids do
		local mail_temp = MailPanel.get_mail(msg.post_guids[i])
		if(mail_temp ~= nil) then
			MailPanel.remove_mail(mail_temp.guid)
		end
	end
	mail_list_ = MailPanel.RankMail(mail_list_)
	MailPanel.InitMailList(true)
end

----------------------------------------------------------


---------------------Function------------------------

function MailPanel.Refresh()
	local dead_mail = {}
	for i = 1, #mail_list_ do
		local mail_temp = mail_list_[i]
		local limit_time = tonumber(mail_temp.send_date) + 7 * 86400000
		if(limit_time <= tonumber(timerMgr:now_string())) then
			table.insert(dead_mail, mail_temp.guid)
		end
	end
	for i = 1, #dead_mail do
		MailPanel.remove_mail(dead_mail[i])
	end
	if(#dead_mail > 0) then
		MailPanel.InitMailList(true)
	end
end

function MailPanel.remove_mail(id)
	local pos = 0
	for i = 1, #mail_list_ do
		if(mail_list_[i].guid == id) then
			pos = i
		end
	end
	if(pos ~= 0) then
		table.remove(mail_list_, pos)
	end
end

function MailPanel.get_mail(id)
	for i = 1, #mail_list_ do
		if(mail_list_[i].guid == id) then
			return mail_list_[i]
		end
	end
	return nil
end

function MailPanel.RankMail(mail_list)
	for i = 1, #mail_list do
		for j = 1, #mail_list - i do
			local result = MailPanel.ComPareMail(mail_list[j], mail_list[j + 1])
			if(result == -1) then
				local mail_t =  mail_list[j + 1]
				mail_list[j + 1] = mail_list[j]
				mail_list[j] = mail_t
			end
		end
	end
	return mail_list
end

function MailPanel.ComPareMail(mail_a, mail_b)
	if(mail_a.is_pick == 0 and mail_b.is_pick == 1) then
		return 1
	elseif(mail_a.is_pick == 1 and mail_b.is_pick == 0) then
		return -1
	elseif(mail_a.is_pick == 1 and mail_b.is_pick == 1 or mail_a.is_pick == 0 and mail_b.is_pick == 0) then
		if(mail_a.send_date > mail_b.send_date) then
			return 1
		else
			return -1
		end
	end
end