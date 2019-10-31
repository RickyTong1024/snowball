NoticePanel = {}

local lua_script_

local time_speed = 0.1
local refresh_time_ = 0

local notice_time_ = 10
local notice_msgs_ = {}
local notice_is_rolling = false
local notice_roll_time = 0
local notice_view_
local notice_res_
local notice_bg_

local verify_panel_
local add_guid_
local verify_input_

local invert_view_
local invert_res_
local invert_list_ = {}
local invert_prefab_list_ = {}
local invert_msg_index_ = 0

local social_msg_ = {}

local chat_msg_ = {}
local avatar_pos = Vector3(-132, 0, 0)


function NoticePanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	notice_bg_ = obj.transform:Find('Anchor_top/notice_bg')
	notice_res_ = obj.transform:Find('Anchor_top/notice_res')
	notice_view_ = obj.transform:Find('Anchor_top/notice_view')
	
	invert_view_ = obj.transform:Find('Anchor_right/invert_view')
	invert_res_ = obj.transform:Find('Anchor_right/invert_res').gameObject
	
	verify_panel_ = obj.transform:Find("verify_panel")
	
	verify_input_ = verify_panel_:Find("verify_input"):GetComponent("UIInput")
	
	local ok_btn = verify_panel_:Find("ok").gameObject
	local no_btn = verify_panel_:Find("no").gameObject
	
	lua_script_:AddButtonEvent(ok_btn, "click", NoticePanel.AddFriend)
	lua_script_:AddButtonEvent(no_btn, "click", NoticePanel.AddFriend)
	
	invert_view_:GetComponent("UIPanel"):SetRect(0, -40, 350, GUIRoot.height - 100)
	
	notice_bg_.gameObject:SetActive(false)
	
	NoticePanel.RegisterMessage()
	timerMgr:AddRepeatTimer('NoticePanel', NoticePanel.Refresh, time_speed, time_speed)
end

function NoticePanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_GONGGAO, NoticePanel.SMSG_GONGGAO)
	Message.register_net_handle(opcodes.SMSG_CHAT, NoticePanel.SMSG_CHAT)
	Message.register_net_handle(opcodes.SMSG_SYS_INFO, NoticePanel.SMSG_SYS_INFO)
	Message.register_net_handle(opcodes.SMSG_TEAM_INVERT, NoticePanel.SMSG_TEAM_INVERT)
	Message.register_net_handle(opcodes.SMSG_TEAM_JOIN, NoticePanel.SMSG_TEAM_JOIN)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_APPLY, NoticePanel.SMSG_SOCIAL_APPLY)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_CHAT, NoticePanel.SMSG_SOCIAL_CHAT)
end

function NoticePanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_GONGGAO, NoticePanel.SMSG_GONGGAO)
	Message.remove_net_handle(opcodes.SMSG_CHAT, NoticePanel.SMSG_CHAT)
	Message.remove_net_handle(opcodes.SMSG_SYS_INFO, NoticePanel.SMSG_SYS_INFO)
	Message.remove_net_handle(opcodes.SMSG_TEAM_INVERT, NoticePanel.SMSG_TEAM_INVERT)
	Message.remove_net_handle(opcodes.SMSG_TEAM_JOIN, NoticePanel.SMSG_TEAM_JOIN)
	Message.remove_net_handle(opcodes.SMSG_SOCIAL_APPLY, NoticePanel.SMSG_SOCIAL_APPLY)
	Message.remove_net_handle(opcodes.SMSG_SOCIAL_CHAT, NoticePanel.SMSG_SOCIAL_CHAT)
end

function NoticePanel.OnDestroy()
	notice_is_rolling = false
	notice_roll_time = 0
	invert_msg_index_ = 0
	refresh_time_ = 0
	notice_msgs_ = {}
	chat_msg_ = {}
	invert_list_ = {}
	invert_prefab_list_ = {}
	social_msg_ = {}
	NoticePanel.RemoveMessage()
	timerMgr:RemoveRepeatTimer("NoticePanel")
end


function NoticePanel.Refresh()
	refresh_time_ = refresh_time_ + 1
	if(refresh_time_ >= 10) then
		NoticePanel.RefreshInvertList()
		refresh_time_ = 0
	end
	if(GUIRoot.HasGUI("StartPanel") or GUIRoot.HasGUI("LoadPanel")) then
		if(notice_bg_.gameObject.activeInHierarchy) then
			NoticePanel.FiniNotice()
		end
	else
		NoticePanel.ShowNoticeMsg()
	end
	HallScene.WeaponShow()
end


function NoticePanel.ShowNoticeMsg()
	if( not notice_is_rolling) then
		if(#(notice_msgs_) > 0) then
			local msg = notice_msgs_[1]
			local notice_msg = LuaHelper.Instantiate(notice_res_.gameObject)
			local view_width = notice_view_:GetComponent("UIPanel").width
			notice_msg.transform.parent = notice_view_
			notice_msg.transform.localPosition = Vector3(view_width / 2, 0, 0)
			notice_msg.transform.localScale = Vector3.one
			local name = notice_msg.transform:GetComponent('UILabel')
			local text = notice_msg.transform:Find("text"):GetComponent('UILabel')
			if(msg.type == 1) then
				name.text = msg.player_name
				text.text = "[c2e5ed]: "..msg.text
				IconPanel.InitVipLabel(name, msg.name_color)
			else
				name.text = ""
				text.text = msg.text
			end
			name:ProcessText()
			text:ProcessText()
			local name_width = notice_msg.transform:GetComponent('UIWidget').width
			local text_width = text.transform:GetComponent('UIWidget').width
			text.transform.localPosition = Vector3(name_width, 0, 0)
			local width = name_width + text_width + view_width
			local from = notice_msg.transform.localPosition
			local to = notice_msg.transform.localPosition - Vector3(width, 0, 0)
			twnMgr:Add_Tween_Postion(notice_msg, notice_time_, from, to, 0, 0)
			notice_msg.gameObject:SetActive(true)
			notice_bg_.gameObject:SetActive(true)
			GameObject.Destroy(notice_msg, notice_time_)
			table.remove(notice_msgs_, 1)
			notice_is_rolling = true
		else
			if(notice_bg_.gameObject.activeInHierarchy) then
				notice_bg_.gameObject:SetActive(false)
			end
		end
	else
		notice_roll_time = notice_roll_time + time_speed
		if(notice_roll_time >= notice_time_) then
			notice_roll_time = 0
			notice_is_rolling = false
		end
	end
end

function NoticePanel.FiniNotice()
	if(notice_view_.childCount > 0) then
		for i = 0, notice_view_.childCount - 1 do
            GameObject.Destroy(notice_view_:GetChild(i).gameObject)
        end
	end
	notice_roll_time = 0
	notice_is_rolling = false
	notice_bg_.gameObject:SetActive(false)
end

function NoticePanel.InitInvertList()
	if(invert_view_.childCount > 0) then
		for i = 0, invert_view_.childCount - 1 do
            GameObject.Destroy(invert_view_:GetChild(i).gameObject)
        end
	end
	invert_prefab_list_ = {}
	if invert_view_:GetComponent('SpringPanel') ~= nil then
		invert_view_:GetComponent('SpringPanel').enabled = false
	end
	invert_view_.localPosition = Vector3(-150, 0, 0)
	invert_view_:GetComponent('UIPanel').clipOffset = Vector2(0, 0)
	for i = 1, #invert_list_ do
		local invert_msg = invert_list_[i].msg
		local invert_t = LuaHelper.Instantiate(invert_res_)
		invert_t.transform.parent = invert_view_
		invert_t.transform.localPosition = Vector3(15, -(GUIRoot.height - 100) / 2 + 50 + (i - 1) * 155, 0)
		invert_t.transform.localScale = Vector3.one
		local name = invert_t.transform:Find("name"):GetComponent("UILabel")
		name.text = invert_msg.player.name
		IconPanel.InitVipLabel(name, invert_msg.player.name_color)
		invert_t.name = i
		local ok_btn = invert_t.transform:Find("ok_btn").gameObject
		local no_btn = invert_t.transform:Find("no_btn").gameObject
		lua_script_:AddButtonEvent(ok_btn, "click", NoticePanel.InvertChose)
		lua_script_:AddButtonEvent(no_btn, "click", NoticePanel.InvertChose)
		local cup_inf_t = IconPanel.GetCupInf(invert_msg.player.cup)
		cup_inf_t.transform.parent = invert_t.transform
		cup_inf_t.transform.localPosition = Vector3(-60, -15, 0)
		cup_inf_t.transform.localScale = Vector3.one
		cup_inf_t:SetActive(true)
		local avatar_t = AvaIconPanel.GetAvatarSex("avatar_res", nil, invert_msg.player.avatar,'', invert_msg.player.toukuang, invert_msg.player.sex)
		avatar_t.transform.parent = invert_t.transform
		avatar_t.transform.localPosition = avatar_pos
		avatar_t.transform.localScale = Vector3.one
		avatar_t:SetActive(true)
		if(invert_list_[i].is_new) then
			invert_t.transform:GetComponent("UISprite").alpha = 0
			local from = invert_t.transform.localPosition + Vector3(50, 0, 0)
			twnMgr:Add_Tween_Postion(invert_t, 0.2, from, invert_t.transform.localPosition, 3, i * 0.1)
			twnMgr:Add_Tween_Alpha(invert_t, 0.2, 0, 1, 3, i * 0.1)
			invert_list_[i].is_new = false
		end
		invert_t:SetActive(true)
		table.insert(invert_prefab_list_, invert_t)
	end
end

function NoticePanel.RefreshInvertList()
	local deadline_invert = {}
	for i = 1, #invert_list_ do
		invert_list_[i].time = invert_list_[i].time + time_speed * 10
		if(invert_prefab_list_[i] ~= nil) then
			invert_prefab_list_[i].transform:Find("time"):GetComponent("UILabel").text = "("..(30 - invert_list_[i].time).."s)"
		end
		if(invert_list_[i].time >= 30) then
			table.insert(deadline_invert, invert_list_[i].msg)
		end
	end
	for i = 1, #deadline_invert do
		NoticePanel.RemoveInvert(deadline_invert[i])
	end
end

function NoticePanel.InvertChose(obj)
	if(ShopPanel.IsRolling()) then
		ShopPanel.StopRoll()
		return
	end
	
	if GUIRoot.HasGUI('RankPanel') and obj.name == "ok_btn" then
		GUIRoot.HideGUI('RankPanel')
	end
	
	local index = tonumber(obj.transform.parent.name)
	local invert_msg = invert_list_[index]
	if(invert_msg == nil) then
		log(index)
		--return 0
	end
	invert_msg_index_ = index
	if(obj.name == "ok_btn") then
		local msg = msg_team_pb.cmsg_team_join()
		msg.player_guid = invert_msg.msg.player.guid
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_TEAM_JOIN, data, {opcodes.SMSG_TEAM_JOIN})
	elseif(obj.name == "no_btn") then
		table.remove(invert_list_, invert_msg_index_)
		local invert_t = obj.transform.parent.gameObject
		local to = invert_t.transform.localPosition + Vector3(50, 0, 0)
		twnMgr:Add_Tween_Postion(invert_t, 0.2, invert_t.transform.localPosition, to)
		twnMgr:Add_Tween_Alpha(invert_t, 0.2, 1, 0)
		timerMgr:AddTimer("InvertList", NoticePanel.InitInvertList, 0.2)
	end
end

function NoticePanel.FiniInvert()
	invert_list_ = {}
	NoticePanel.InitInvertList()
end

function NoticePanel.RemoveInvert(msg)
	for i = 1, #invert_list_ do
		if(invert_list_[i].msg.player.guid == msg.player.guid) then
			table.remove(invert_list_, i)
			break
		end
	end
	NoticePanel.InitInvertList()
end


function NoticePanel.GetSocialMsg(guid)
	if(social_msg_[guid] ~= nil) then
		return social_msg_[guid]
	end
	return {}
end

function NoticePanel.AddSocialMsg(guid, msg)
	if social_msg_[guid] == nil then
		social_msg_[guid] = {}
	end
	table.insert(social_msg_[guid], msg)
	if(#social_msg_[guid] > 30) then
		table.remove(social_msg_[guid], 1)
	end
	NoticePanel.SaveSocialMsg(guid)
end

function NoticePanel.DelSocialMsg(guid)
	social_msg_[guid] = nil
	if(PlayerPrefs.HasKey(self.player.guid..guid.."record")) then
		PlayerPrefs.DeleteKey(self.player.guid..guid.."record")
	end
	if(PlayerPrefs.HasKey(self.player.guid..guid.."unread")) then
		PlayerPrefs.DeleteKey(self.player.guid..guid.."unread")
	end
	PlayerPrefs.SetString(self.player.guid.."_total_unread_num", self.unread_msg_num)
	PlayerPrefs.Save()
end

function NoticePanel.SaveSocialMsg(guid)
	local social_msg = NoticePanel.GetSocialMsg(guid)
	local save_msg = ""
	for i = 1, #social_msg do
		local is_self = 0
		if(social_msg[#social_msg - i + 1].player_guid == self.player.guid) then
			is_self = 1
		end
		if(i < #social_msg) then
			save_msg = save_msg..is_self.."|"..social_msg[#social_msg - i + 1].text.."|"..social_msg[#social_msg - i + 1].time..";"
		else
			save_msg = save_msg..is_self.."|"..social_msg[#social_msg - i + 1].text.."|"..social_msg[#social_msg - i + 1].time
		end
	end
	if(#social_msg > 0) then
		PlayerPrefs.SetString(self.player.guid..guid.."record", save_msg)
	end
	local unread_num = FriendPanel.GetUnreadNum(guid)
	PlayerPrefs.SetString(self.player.guid..guid.."unread", unread_num)
	PlayerPrefs.SetString(self.player.guid.."_total_unread_num", self.unread_msg_num)
	PlayerPrefs.Save()
end

function NoticePanel.SaveSocialList()
	local social_guids = ""
	for i = 1, #self.friend_guids do
		if(i < #self.friend_guids) then
			social_guids = social_guids..self.friend_guids[i]..";"
		else
			social_guids = social_guids..self.friend_guids[i]
		end
	end
	PlayerPrefs.SetString(self.player.guid.."social_guids", social_guids)
	PlayerPrefs.Save()
end

function NoticePanel.CheckSocialList()
	if(PlayerPrefs.HasKey(self.player.guid.."social_guids")) then
		local social_list = string_split(PlayerPrefs.GetString(self.player.guid.."social_guids"), ";")
		for i = 1, #social_list do
			if self.social_type(social_list[i]) ~= 2 then
				if(social_msg_[guid] ~= nil) then
					social_msg_[guid] = nil
				end
				if(PlayerPrefs.HasKey(self.player.guid..social_list[i].."record")) then
					PlayerPrefs.DeleteKey(self.player.guid..social_list[i].."record")
				end
				if(PlayerPrefs.HasKey(self.player.guid..social_list[i].."unread")) then
					self.unread_msg_num = self.unread_msg_num - tonumber(PlayerPrefs.GetString(self.player.guid..social_list[i].."unread"))
					PlayerPrefs.DeleteKey(self.player.guid..social_list[i].."unread")
				end
			end
		end
		PlayerPrefs.SetString(self.player.guid.."_total_unread_num", self.unread_msg_num)
		PlayerPrefs.Save()
	end
end

function NoticePanel.GetVipNameColor(social)
	if social.name_color == nil then
		local color = 0
		if(tonumber(social.yue_time) > tonumber(timerMgr:now_string())) then
			color = 1
		end
		if(tonumber(social.nian_time) > tonumber(timerMgr:now_string())) then
			color = 2
		end
		return color
	elseif social.name_color ~= nil then
		return social.name_color
	else
		return 0
	end
end

function NoticePanel.LoadSocialMsg(social)
	if(PlayerPrefs.HasKey(self.player.guid..social.target_guid.."record")) then
		local social_msg = PlayerPrefs.GetString(self.player.guid..social.target_guid.."record")
		local msg_list = string_split(social_msg, ";")
		for i = 1, #msg_list do
			local msg_inf = string_split(msg_list[i], "|")
			if(#msg_inf == 3) then
				local msg = {}
				msg.text = msg_inf[2]
				msg.time = msg_inf[3]
				if(msg_inf[1] == "0") then
					msg.player_name = social.name
					msg.player_guid = social.target_guid
					msg.sex = social.sex
					msg.level = social.level
					msg.avatar = social.avatar
					msg.toukuang = social.toukuang
					msg.name_color = NoticePanel.GetVipNameColor(social)
				else
					msg.player_name = self.player.name
					msg.player_guid = self.player.guid
					msg.sex = self.player.sex
					msg.level = self.player.level
					msg.avatar = self.player.avatar_on
					msg.toukuang = self.player.toukuang_on
					msg.name_color = NoticePanel.GetVipNameColor(self.player)
				end
				if social_msg_[social.target_guid] == nil then
					social_msg_[social.target_guid] = {}
				end
				table.insert(social_msg_[social.target_guid], 1, msg)
			end
		end
	end
	if(PlayerPrefs.HasKey(self.player.guid..social.target_guid.."unread")) then
		local num = tonumber(PlayerPrefs.GetString(self.player.guid..social.target_guid.."unread"))
		FriendPanel.AddUnreadMsg(social.target_guid, num)
	end
end

function NoticePanel.RankSocialMsg(guid)
	if(social_msg_[guid] ~= nil) then
		local comp = function(a, b)
			return tonumber(a.time) < tonumber(b.time)
		end
		table.sort(social_msg_[guid], comp)
	end
end

function NoticePanel.InitAddPanel(target_guid)
	if(lua_script_ ~= nil) then
		if(#self.friend_guids >= 50) then
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('NoticePanel_001'),Config.get_t_script_str('NoticePanel_002')})
		else
			add_guid_ = target_guid
			verify_input_.value = Config.get_t_script_str('NoticePanel_003')--"交个朋友吧~~"
			verify_panel_.gameObject:SetActive(true)
		end
	end
end

function NoticePanel.AddFriend(obj)
	if(obj.name == "ok") then
		if(verify_input_.value == "") then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('NoticePanel_004')})
			return 0
		end
		local msg = msg_social_pb.cmsg_social_apply()
		msg.target_guid = add_guid_
		msg.verify = str_sub(verify_input_.value, 45)
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_SOCIAL_APPLY, data, {opcodes.SMSG_SOCIAL_APPLY})
	end
	verify_panel_.gameObject:SetActive(false)
end

---------------------------服务器消息--------------------------
function NoticePanel.SMSG_GONGGAO(message)
	local msg = msg_hall_pb.smsg_gonggao()
	msg:ParseFromString(message.luabuff)
	local mesg = {}
	mesg.text = msg.text
	mesg.type = 2
	table.insert(notice_msgs_, mesg)
end

function NoticePanel.SMSG_SOCIAL_APPLY()
	GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('NoticePanel_005')})
	self.del_social_guid(3, add_guid_)
	FriendPanel.RemoveBlack(add_guid_)
	ZonePanel.AddFriend()
end

function NoticePanel.SMSG_CHAT(message)
	local msg = msg_hall_pb.smsg_chat()
	msg:ParseFromString(message.luabuff)
	if(msg.type == 0 or msg.type == 1) then
		table.insert(chat_msg_, {data=msg,type_id=1})
		HallPanel.SMSG_CHAT(msg)
		ChatPanel.SMSG_CHAT(msg)
		if(msg.type == 1) then
			table.insert(notice_msgs_, msg)
		end
	end
end

function NoticePanel.SMSG_SYS_INFO(message)
	local msg = msg_hall_pb.smsg_sys_info()
	msg:ParseFromString(message.luabuff)
	table.insert(chat_msg_, {data=msg,type_id=2})
	HallPanel.SMSG_SYS_INFO(msg)
	ChatPanel.SMSG_SYS_INFO(msg)
end

function NoticePanel.SMSG_SOCIAL_CHAT(message)
	local msg = msg_hall_pb.smsg_chat()
	msg:ParseFromString(message.luabuff)
	local mssg = {}
	mssg.time = timerMgr:now_string()
	mssg.text = msg.text
	mssg.player_name = msg.player_name
	mssg.player_guid = msg.player_guid
	mssg.sex = msg.sex
	mssg.level = msg.level
	mssg.avatar = msg.avatar
	mssg.toukuang = msg.toukuang
	mssg.name_color = msg.name_color
	FriendPanel.SMSG_SOCIAL_CHAT(mssg)
	HallPanel.ShowTip()
	if(msg.player_guid ~= self.player.guid) then
		NoticePanel.AddSocialMsg(msg.player_guid, mssg)
	end
end

function NoticePanel.SMSG_TEAM_INVERT(message)
	if State.cur_state == State.state.ss_hall then
		local msg = msg_team_pb.smsg_team_invert()
		msg:ParseFromString(message.luabuff)
		for i = 1, #invert_list_ do
			if(invert_list_[i].msg.player.guid == msg.player.guid) then
				table.remove(invert_list_, i)
				break
			end
		end
		local invert_msg = {}
		invert_msg.msg = msg
		invert_msg.is_new = true
		invert_msg.time = 0
		table.insert(invert_list_, 1, invert_msg)
		if(#invert_list_ > 5) then
			table.remove(invert_list_, #invert_list_)
		end
		NoticePanel.InitInvertList()
	end
end

function NoticePanel.SMSG_TEAM_JOIN(message)
	local msg = msg_team_pb.smsg_team_join()
	msg:ParseFromString(message.luabuff)
	NoticePanel.FiniInvert()
	local mssg = s_message.New()
	mssg.name = "team_join_msg"
	Message.add_message(mssg)
	GUIRoot.ShowGUI("TeamPanel", {msg.team})
end

function NoticePanel.GetMsg(pos)
	if(pos ~= nil and pos <= #chat_msg_ and pos > 0) then
		return chat_msg_[pos]
	end
	return chat_msg_
end

function NoticePanel.GetMsgType(type_id)
	if type_id == nil then
		return chat_msg_
	else
		local tb = {}
		for i =1,#chat_msg_ do
			if chat_msg_[i].type_id == type_id then
				table.insert(tb,chat_msg_[i])
			end
		end
		return tb
	end
end

function NoticePanel.ClearMsg()
	notice_msgs_ = {}
	chat_msg_ = {}
	social_msg_ = {}
	notice_is_rolling = false
	notice_roll_time = 0
end