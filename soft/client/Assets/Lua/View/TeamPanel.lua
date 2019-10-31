TeamPanel = {}

local lua_script_

local team_msg_
local team_list_ = {}
local invert_tuijian_list_ = {}
local invert_friend_list_ = {}

local hor_root_
local chat_str_
local chat_str_num
local chatcontent_lb_

local team_leader_
local member_res_
local invert_view_
local view_pos = Vector3(0, 0, 0)
local invert_res_
local chat_msg_res_
local empty_tip_

local chat_input_
local friend_btn_
local random_btn_
local start_btn_

local cur_page = 0

local x_dis = 160
local y_dis = 0

local avatar_pos = Vector3(-148, 0, 0)

local y_pos = 0
local invert_y_height = 122

local exit_code_ = 0

local friend_request = 0

local state_font_ = {"[07D0FA]"..Config.get_t_script_str('TeamPanel_001'), "[e4ac01]"..Config.get_t_script_str('TeamPanel_002'), "[44FC08]"..Config.get_t_script_str('TeamPanel_003'), "[4C6373]"..Config.get_t_script_str('TeamPanel_004'), "[FDAF05]"..Config.get_t_script_str('TeamPanel_005')}
local IconViewP = {}

function TeamPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	GUIRoot.ShowGUI('BackPanel', {3})
	
	local main_panel = obj.transform:Find("main_panel")
	
	hor_root_ = main_panel:Find("Anchor_left/hor_root")
	
	team_leader_ = main_panel:Find("Anchor_left/leader")
	member_res_ = main_panel:Find("Anchor_left/member_res").gameObject
	invert_view_ = main_panel:Find("Anchor_right/invert_view")
	invert_res_ = main_panel:Find("invert_res").gameObject
	chat_msg_res_ = main_panel:Find("Anchor_left/chat_msg").gameObject
	chat_input_ = main_panel:Find("Anchor_bottom_left/chat_input"):GetComponent("UIInput")
	
	empty_tip_ = main_panel:Find("Anchor_right/empty_tip").gameObject
	main_panel:Find("Anchor_bottom_right/bg"):GetComponent("UIWidget").height = GUIRoot.height - 74
	
	invert_view_:GetComponent("UIPanel"):SetRect(0, -22, 430, GUIRoot.height - 210)
	y_pos = (GUIRoot.height - 210) / 2 - invert_y_height / 2 - 24
	view_pos = invert_view_.localPosition
	
	local chat_btn = main_panel:Find("Anchor_bottom_left/chat_btn").gameObject
	start_btn_ = main_panel:Find("Anchor_bottom_right/start_btn").gameObject
	friend_btn_ = main_panel:Find("Anchor_top_right/friend_btn")
	random_btn_ = main_panel:Find("Anchor_top_right/random_btn")
	
	lua_script_:AddButtonEvent(chat_btn, "click", TeamPanel.Click)
	lua_script_:AddButtonEvent(start_btn_, "click", TeamPanel.Click)
	lua_script_:AddButtonEvent(friend_btn_.gameObject, "click", TeamPanel.Click)
	lua_script_:AddButtonEvent(random_btn_.gameObject, "click", TeamPanel.Click)
	
	empty_tip_:SetActive(false)
	
	TeamPanel.RegisterMessage()
	timerMgr:AddRepeatTimer('TeamPanel', TeamPanel.Refresh, 1, 1)
	GUIRoot.UIEffectScalePos(main_panel.gameObject, true, 1)
	chat_str_ = ''
	chat_str_num = 0
	chatcontent_lb_ =  main_panel:Find("Anchor_left/chat_panel/ChatContent"):GetComponent('UILabel')
	TeamPanel.CalChatView(obj)
	TeamPanel.CalTeamIconView()
end

function TeamPanel.CalTeamIconView()
	IconViewP = {}
	--142 142
	local screen_w_ = panelMgr:get_w()
	local screen_h_ = panelMgr:get_h()

	local r_h = screen_h_ / 2 - 98
	local r_w = screen_w_ - 452

	--第一排
	local offset_x = (screen_w_ - 452 - 142 * 3) / 4
	for i = 1,3 do
		table.insert(IconViewP,Vector3(offset_x * i + (i - 1) * 142 + 71,r_h - 80,0))
	end
	
	local down_p = (IconViewP[1] + IconViewP[2]) / 2
	down_p.y = -25
	table.insert(IconViewP,down_p)
	
	down_p = (IconViewP[2] + IconViewP[3]) / 2
	down_p.y = -25
	table.insert(IconViewP,down_p)
	
	team_leader_.transform.localPosition = IconViewP[1]
end

function TeamPanel.CalChatView(obj)
	local chat_panel = obj.transform:Find('main_panel/Anchor_left/chat_panel')
	local chat_lb = obj.transform:Find('main_panel/Anchor_left/chat_panel/ChatContent'):GetComponent('UILabel')
	local cp = chat_panel:GetComponent('UIPanel')
	
	local h = chat_lb.fontSize * 5 + chat_lb.spacingY * 5 
	local screen_w_ = panelMgr:get_w()
	local screen_h_ = panelMgr:get_h()
	--452 82
	local r_h = screen_h_ / 2 - 82
	local r_w = screen_w_ - 452
	if r_w > 480 then
		r_w = 480
	end
	
	chat_panel.transform.localPosition = Vector3(r_w / 2 + 16,-(r_h - h / 2),0)
	cp:SetRect(0,0,r_w,h)
	chat_lb.width = r_w
	chat_lb.transform.localPosition = Vector3(-r_w / 2,h / 2,0)
end

function TeamPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_TEAM_OTHER_JOIN, TeamPanel.SMSG_TEAM_OTHER_JOIN)
	Message.register_net_handle(opcodes.SMSG_TEAM_EXIT, TeamPanel.SMSG_TEAM_EXIT)
	Message.register_net_handle(opcodes.SMSG_TEAM_KICK, TeamPanel.SMSG_TEAM_KICK)
	Message.register_net_handle(opcodes.SMSG_TEAM_CHAT, TeamPanel.SMSG_TEAM_CHAT)
	Message.register_net_handle(opcodes.SMSG_TEAM_TUIJIAN, TeamPanel.SMSG_TEAM_TUIJIAN)
	Message.register_net_handle(opcodes.SMSG_MULTI_BATTLE, TeamPanel.SMSG_MULTI_BATTLE)
	Message.register_net_handle(opcodes.SMSG_SOCAIL_LOOK, TeamPanel.SMSG_SOCAIL_LOOK)
	Message.register_handle("back_panel_msg", TeamPanel.Back)
	Message.register_handle("back_panel_recharge", TeamPanel.Recharge)
end

function TeamPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_TEAM_OTHER_JOIN, TeamPanel.SMSG_TEAM_OTHER_JOIN)
	Message.remove_net_handle(opcodes.SMSG_TEAM_EXIT, TeamPanel.SMSG_TEAM_EXIT)
	Message.remove_net_handle(opcodes.SMSG_TEAM_KICK, TeamPanel.SMSG_TEAM_KICK)
	Message.remove_net_handle(opcodes.SMSG_TEAM_CHAT, TeamPanel.SMSG_TEAM_CHAT)
	Message.remove_net_handle(opcodes.SMSG_TEAM_TUIJIAN, TeamPanel.SMSG_TEAM_TUIJIAN)
	Message.remove_net_handle(opcodes.SMSG_MULTI_BATTLE, TeamPanel.SMSG_MULTI_BATTLE)
	Message.remove_net_handle(opcodes.SMSG_SOCAIL_LOOK, TeamPanel.SMSG_SOCAIL_LOOK)
	Message.remove_handle("back_panel_msg", TeamPanel.Back)
	Message.remove_handle("back_panel_recharge", TeamPanel.Recharge)
end

function TeamPanel.OnDestroy()
	lua_script_ = nil
	team_msg_ = nil
	exit_code_ = 0
	team_list_ = {}
	invert_friend_list_ = {}
	invert_tuijian_list_ = {}
	friend_request = 0
	TeamPanel.RemoveMessage()
	timerMgr:RemoveRepeatTimer('TeamPanel')
end

function TeamPanel.OnParam(parm)
	team_msg_ = parm[1]
	TeamPanel.InitTeamList()
	TeamPanel.RefreshTeamInf()
	TeamPanel.SelectPage(0)
end
---------------ButtonEvent--------------

function TeamPanel.Click(obj)
	if(obj.name == "friend_btn") then
		friend_request = 0
		TeamPanel.SelectPage(0)
	elseif(obj.name == "random_btn") then
		TeamPanel.SelectPage(1)
	elseif(obj.name == "chat_btn") then
		TeamPanel.SendMsg()
	elseif(obj.name == "start_btn") then
		if(TeamPanel.GetPlayer(0).player.guid == self.player.guid) then
			GameTcp.Send(opcodes.CMSG_MULTI_BATTLE, nil, {opcodes.SMSG_MULTI_BATTLE})
		end
	end
end

function TeamPanel.Back()
	exit_code_ = 1
	GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('TeamPanel_006'), Config.get_t_script_str('TeamPanel_007'), TeamPanel.BackSure, Config.get_t_script_str('TeamPanel_008'), TeamPanel.BackCancel})
end

function TeamPanel.BackSure()
	exit_code_ = 2
	GameTcp.Send(opcodes.CMSG_TEAM_EXIT, nil, {opcodes.SMSG_TEAM_EXIT})
end

function TeamPanel.BackCancel()
	exit_code_ = 0
end

function TeamPanel.ReturnHall()
	if(exit_code_ == 1) then
		GUIRoot.HideGUI("SelectPanel")
	end
	GUIRoot.HideGUI("TeamPanel")
	GUIRoot.HideGUI("BackPanel")
	GUIRoot.ShowGUI('HallPanel')
end

function TeamPanel.Recharge()
	exit_code_ = 1
	GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('TeamPanel_009'), Config.get_t_script_str('TeamPanel_007'), TeamPanel.RechargeSure, Config.get_t_script_str('TeamPanel_008'), TeamPanel.BackCancel})
end

function TeamPanel.RechargeSure()
	exit_code_ = 3
	GameTcp.Send(opcodes.CMSG_TEAM_EXIT, nil, {opcodes.SMSG_TEAM_EXIT})
end

function TeamPanel.JumpRecharge()
	GUIRoot.HideGUI('TeamPanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

----------------刷新队伍界面------------------------

function TeamPanel.InitTeamList()
	local avatar_t = AvaIconPanel.GetAvatarSex("avatar_res", nil, Config.t_avatar_ids[1], nil, Config.t_toukuang_ids[1], 0)
	avatar_t.transform.parent = hor_root_
	avatar_t.transform.localPosition = IconViewP[1]
	avatar_t.transform.localScale = Vector3.one
	avatar_t.name = "avatar_inf"
	avatar_t:SetActive(true)
	for i = 1, 4 do
		local member_t = LuaHelper.Instantiate(member_res_)
		member_t.transform.parent = hor_root_
		member_t.transform.localScale = Vector3.one
		member_t.transform.localPosition = IconViewP[i + 1]
		
		local kick_btn = member_t.transform:Find("kick_btn").gameObject
		lua_script_:AddButtonEvent(kick_btn, "click", TeamPanel.KickPlayer)
		kick_btn.name = i
		avatar_t = AvaIconPanel.GetAvatarSex("avatar_res", nil, Config.t_avatar_ids[1], nil, Config.t_toukuang_ids[1], 0)
		avatar_t.transform.parent = member_t.transform
		avatar_t.transform.localPosition = Vector3(0, 0, 0)
		avatar_t.transform.localScale = Vector3.one
		avatar_t.name = "avatar_inf"
		member_t:SetActive(true)
		table.insert(team_list_, member_t)
	end
end

function TeamPanel.RefreshTeamInf()
	for i = 1, #team_list_ do
		team_list_[i].transform:Find("avatar_inf").gameObject:SetActive(false)
		team_list_[i].transform:Find("empty_tip").gameObject:SetActive(true)
		team_list_[i].transform:Find(i).gameObject:SetActive(false)
		team_list_[i].transform:Find("name"):GetComponent("UILabel").text = ""
		team_list_[i].transform:GetComponent("UISprite").spriteName = "drzd_kqwz"
		start_btn_:SetActive(false)
	end
	if(#team_list_ >= 2) then
		for i = 1, 5 do
			local member_temp = team_msg_.member[i]
			if(member_temp ~= nil) then
				local avatar_inf = nil
				if(member_temp.member_type == 1) then
					if(self.player.guid == member_temp.player.guid) then
						start_btn_:SetActive(true)
					end
					avatar_inf = hor_root_:Find("avatar_inf")
					team_leader_:Find("name"):GetComponent("UILabel").text = member_temp.player.name
					IconPanel.InitVipLabel(team_leader_:Find("name"):GetComponent("UILabel"), member_temp.player.name_color)
				else
					local index = TeamPanel.GetPlayerIndex(member_temp.player.guid)
					avatar_inf = team_list_[index].transform:Find("avatar_inf")
					team_list_[index].transform:Find("name"):GetComponent("UILabel").text = member_temp.player.name
					IconPanel.InitVipLabel(team_list_[index].transform:Find("name"):GetComponent("UILabel"), member_temp.player.name_color)
					if TeamPanel.GetPlayerIndex(self.player.guid) == 0 then
						team_list_[index].transform:Find(index).gameObject:SetActive(true)
					end
					team_list_[index].transform:GetComponent("UISprite").spriteName = "drzd_dy"
					team_list_[index].transform:Find("empty_tip").gameObject:SetActive(false)
				end
				AvaIconPanel.ModifyAvatar(avatar_inf, member_temp.player.avatar,'', member_temp.player.toukuang, member_temp.player.sex)
				avatar_inf.gameObject:SetActive(true)
			end
		end
	end
end

function TeamPanel.KickPlayer(obj)
	local index = tonumber(obj.name)
	local target_guid = TeamPanel.GetPlayer(index).player.guid
	if(target_guid ~= -1) then
		local msg = msg_team_pb.cmsg_team_kick()
		msg.target_guid = target_guid
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_TEAM_KICK, data, {opcodes.SMSG_TEAM_KICK})
	end
end

function TeamPanel.SendMsg()
	if(chat_input_.value ~= '') then
		if(string.find(chat_input_.value, "\n")) then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('TeamPanel_010')})
		else
			local msg = msg_team_pb.cmsg_team_chat()
			msg.text = chat_input_.value
			if(string.len(msg.text) > 60) then
				msg.text = str_sub(msg.text, 60)
			end
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_TEAM_CHAT, data)
		end
		chat_input_.value = ''
	else
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('TeamPanel_011')})
	end
end

function TeamPanel.GetPlayerIndex(guid)
	local index = 1
	for i = 1, #team_msg_.member do
		if(team_msg_.member[i].player.guid == guid) then
			if(team_msg_.member[i].member_type == 1) then
				index = 0
			end
			return index
		else
			if(team_msg_.member[i].member_type ~= 1) then
				index = index + 1
			end
		end
	end
	return -1
end

function TeamPanel.GetPlayer(index)
	for i = 1, #team_msg_.member do
		if(TeamPanel.GetPlayerIndex(team_msg_.member[i].player.guid) == index) then
			return team_msg_.member[i]
		end
	end
	return nil
end

function TeamPanel.GetPlayerName(guid)
	for i = 1, #team_msg_.member do
		if(team_msg_.member[i].player.guid == guid) then
			return team_msg_.member[i].player.name
		end
	end
	return ''
end

function TeamPanel.RemovePlayer(guid, mode)
	for i = 1, #team_msg_.member do
		if(team_msg_.member[i].player.guid == guid) then
			if(mode == 0) then
				local content = string.format(Config.get_t_script_str('TeamPanel_012'),team_msg_.member[i].player.name)
				GUIRoot.ShowGUI("MessagePanel", {content})
			elseif(mode == 1) then
				local content = string.format(Config.get_t_script_str('TeamPanel_013'),team_msg_.member[i].player.name)
				GUIRoot.ShowGUI("MessagePanel", {content})
			end
			local index = TeamPanel.GetPlayerIndex(guid)
			local chat_msg_t = nil
			if(index == 0) then
				chat_msg_t = team_leader_:Find("chat_msg")
			elseif(index > 0) then
				chat_msg_t = team_list_[index].transform:Find("chat_msg")
			end
			if(chat_msg_t ~= nil) then
				GameObject.Destroy(chat_msg_t.gameObject)
			end
			table.remove(team_msg_.member, i)
			break
		end
	end
end
-----------------------------------------------------



------------------刷新邀请界面-----------------------

function TeamPanel.SelectPage(page)
	friend_btn_:Find("tip").gameObject:SetActive(false)
	random_btn_:Find("tip").gameObject:SetActive(false)
	friend_btn_:GetComponent("UILabel").text = "[4C6373]"..Config.get_t_script_str('TeamPanel_014')
	random_btn_:GetComponent("UILabel").text = "[4C6373]"..Config.get_t_script_str('TeamPanel_015')
	cur_page = page
	if(page == 0) then
		friend_btn_:GetComponent("UILabel").text = "[C0EBF4]"..Config.get_t_script_str('TeamPanel_014')
		friend_btn_:Find("tip").gameObject:SetActive(true)
		if(friend_request == 0) then
			FriendPanel.SocialLook(2)
			friend_request = 1
		else
			TeamPanel.InitFriendList()
		end
	elseif(page == 1) then
		random_btn_:GetComponent("UILabel").text = "[C0EBF4]"..Config.get_t_script_str('TeamPanel_015')
		random_btn_:Find("tip").gameObject:SetActive(true)
		GameTcp.Send(opcodes.CMSG_TEAM_TUIJIAN, nil, {opcodes.SMSG_TEAM_TUIJIAN})
	end
end

function TeamPanel.InitTuiJianList()
	if(invert_view_.childCount > 0) then
		for i = 0, invert_view_.childCount - 1 do
			GameObject.Destroy(invert_view_:GetChild(i).gameObject)
		end
		if invert_view_:GetComponent('SpringPanel') ~= nil then
			invert_view_:GetComponent('SpringPanel').enabled = false
		end
		local uv = invert_view_:GetComponent('UIScrollView')
		uv.panel.clipOffset = Vector2(0, 0)
		invert_view_.localPosition = view_pos
	end
	empty_tip_:SetActive(false)
	if(#invert_tuijian_list_ == 0) then
		empty_tip_:SetActive(true)
	end
	for i = 0, #invert_tuijian_list_ - 1 do
		local invert_temp = invert_tuijian_list_[i + 1]
		local invert_t = LuaHelper.Instantiate(invert_res_)
		invert_t.transform.parent = invert_view_
		invert_t.transform.localPosition = Vector3(0, y_pos - invert_y_height * i, 0)
		invert_t.transform.localScale = Vector3.one
		local name = invert_t.transform:Find("name"):GetComponent("UILabel")
		local state = invert_t.transform:Find("state"):GetComponent("UILabel")
		local invert_btn = invert_t.transform:Find("invert_btn").gameObject
		if(invert_temp.state == 0) then
			state.text = state_font_[1]
		elseif(invert_temp.state == 1) then
			state.text = state_font_[3]
			invert_t.transform:GetComponent("UISprite").spriteName = "nxxt_002"
			invert_btn:SetActive(false)
		end
		name.text = invert_temp.player.name
		IconPanel.InitVipLabel(name, invert_temp.player.name_color)
		invert_btn.name = invert_temp.player.guid
		local cup_inf_t = IconPanel.GetCupInf(invert_temp.player.cup)
		cup_inf_t.transform.parent = invert_t.transform
		cup_inf_t.transform.localPosition = Vector3(-80, -35, 0)
		cup_inf_t.transform.localScale = Vector3.one
		cup_inf_t:SetActive(true)
		local avatar_t = AvaIconPanel.GetAvatarSex("avatar_res", nil, invert_temp.player.avatar,'', invert_temp.player.toukuang, invert_temp.player.sex)
		avatar_t.transform.parent = invert_t.transform
		avatar_t.transform.localPosition = avatar_pos
		avatar_t.transform.localScale = Vector3.one
		avatar_t:SetActive(true)
		lua_script_:AddButtonEvent(invert_btn, "click", TeamPanel.InvertPlayer)
		invert_t:SetActive(true)
	end
end

function TeamPanel.InitFriendList()
	if(invert_view_.childCount > 0) then
		for i = 0, invert_view_.childCount - 1 do
			GameObject.Destroy(invert_view_:GetChild(i).gameObject)
		end
		if invert_view_:GetComponent('SpringPanel') ~= nil then
			invert_view_:GetComponent('SpringPanel').enabled = false
		end
		local uv = invert_view_:GetComponent('UIScrollView')
		uv.panel.clipOffset = Vector2(0, 0)
		invert_view_.localPosition = view_pos
	end
	empty_tip_:SetActive(false)
	if(#invert_friend_list_ == 0) then
		empty_tip_:SetActive(true)
	end
	for i = 0, #invert_friend_list_ - 1 do
		local invert_temp = invert_friend_list_[i + 1]
		local invert_t = LuaHelper.Instantiate(invert_res_)
		invert_t.transform.parent = invert_view_
		invert_t.transform.localPosition = Vector3(0, y_pos - invert_y_height * i, 0)
		invert_t.transform.localScale = Vector3.one
		local name = invert_t.transform:Find("name"):GetComponent("UILabel")
		local state = invert_t.transform:Find("state"):GetComponent("UILabel")
		local invert_btn = invert_t.transform:Find("invert_btn").gameObject
		if(invert_temp.state == 0) then
			if luabit.band(invert_temp.social.sflag, 8) ~= 0 then
				state.text = state_font_[2]
				invert_btn:SetActive(false)
			elseif luabit.band(invert_temp.social.sflag, 2) ~= 0 then
				state.text = state_font_[1]
				invert_btn:SetActive(true)
			else
				state.text = state_font_[4]
				invert_btn:SetActive(false)
			end
		elseif(invert_temp.state == 1) then
			state.text = state_font_[3]
			invert_t.transform:GetComponent("UISprite").spriteName = "nxxt_002"
			invert_btn:SetActive(false)
		end
		local color = NoticePanel.GetVipNameColor(invert_temp.social)
		name.text = invert_temp.social.name
		IconPanel.InitVipLabel(name, color)
		if(TeamPanel.GetPlayerIndex(invert_temp.social.target_guid) ~= -1) then
			state.text = state_font_[5]
			invert_btn:SetActive(false)
		end
		invert_btn.name = invert_temp.social.target_guid
		local cup_inf_t = IconPanel.GetCupInf(invert_temp.social.cup)
		cup_inf_t.transform.parent = invert_t.transform
		cup_inf_t.transform.localPosition = Vector3(-80, -35, 0)
		cup_inf_t.transform.localScale = Vector3.one
		cup_inf_t:SetActive(true)
		local avatar_t = AvaIconPanel.GetAvatarSex("avatar_res", nil, invert_temp.social.avatar,'', invert_temp.social.toukuang, invert_temp.social.sex)
		avatar_t.transform.parent = invert_t.transform
		avatar_t.transform.localPosition = avatar_pos
		avatar_t.transform.localScale = Vector3.one
		avatar_t:SetActive(true)
		lua_script_:AddButtonEvent(invert_btn, "click", TeamPanel.InvertPlayer)
		invert_t:SetActive(true)
	end
end

function TeamPanel.InvertPlayer(obj)
	obj:SetActive(false)
	local invert_t = obj.transform.parent
	invert_t:GetComponent("UISprite").spriteName = "nxxt_002"
	invert_t:Find("state"):GetComponent("UILabel").text = state_font_[3]
	local invert_temp = TeamPanel.GetInvertPlayer(obj.name, 1)
	if(invert_temp ~= nil) then
		invert_temp.state = 1
	end
	invert_temp = TeamPanel.GetInvertPlayer(obj.name, 2)
	if(invert_temp ~= nil) then
		invert_temp.state = 1
	end
	local msg = msg_team_pb.cmsg_team_invert()
	msg.target_guid = obj.name
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_TEAM_INVERT, data)
end

function TeamPanel.NewInvert(player, type, invert_tuijian_list)
	local index = 0
	local is_flag = false
	if(type == 1) then
		for i = 1, #invert_friend_list_ do
			if(invert_friend_list_[i].social.target_guid == player.target_guid) then
				is_flag = true
				index = i
				break
			end
		end
		if(is_flag) then
			invert_friend_list_[index].social = player
		else
			local invert_temp = {}
			invert_temp.social = player
			invert_temp.time = 0
			invert_temp.state = 0
			if(TeamPanel.GetInvertPlayer(player.target_guid, 2) ~= nil) then
				invert_temp.time = TeamPanel.GetInvertPlayer(player.target_guid, 2).time
				invert_temp.state = TeamPanel.GetInvertPlayer(player.target_guid, 2).state
			end
			table.insert(invert_friend_list_, invert_temp)
		end
	else
		if(TeamPanel.GetPlayerIndex(player.guid) ~= -1) then
			return 0
		end
		for i = 1, #invert_tuijian_list do
			if(invert_tuijian_list[i].player.guid == player.guid) then
				is_flag = true
				index = i
				break
			end
		end
		local invert_temp = {}
		invert_temp.player = player
		invert_temp.time = 0
		invert_temp.state = 0
		if(is_flag) then
			invert_temp.state = invert_tuijian_list[index].state
			invert_temp.time = invert_tuijian_list[index].time
		end
		if(TeamPanel.GetInvertPlayer(player.guid, 1) ~= nil) then
			invert_temp.time = TeamPanel.GetInvertPlayer(player.guid, 1).time
			invert_temp.state = TeamPanel.GetInvertPlayer(player.guid, 1).state
		end
		table.insert(invert_tuijian_list_, invert_temp)
	end
end

function TeamPanel.Refresh()
	for i = 1, #invert_friend_list_ do
		if(invert_friend_list_[i].state == 1) then
			invert_friend_list_[i].time = invert_friend_list_[i].time + 1
			if(invert_friend_list_[i].time >= 30) then
				invert_friend_list_[i].state = 0
				invert_friend_list_[i].time = 0
				if(cur_page == 0) then
					TeamPanel.InitFriendList()
				end
			end
		end
	end
	for i = 1, #invert_tuijian_list_ do
		if(invert_tuijian_list_[i].state == 1) then
			invert_tuijian_list_[i].time = invert_tuijian_list_[i].time + 1
			if(invert_tuijian_list_[i].time >= 30) then
				invert_tuijian_list_[i].state = 0
				invert_tuijian_list_[i].time = 0
				if(cur_page == 1) then
					TeamPanel.InitTuiJianList()
				end
			end
		end
	end
end

function TeamPanel.GetInvertPlayer(guid, type)
	if(type == 1) then
		for i = 1, #invert_friend_list_ do
			if(invert_friend_list_[i].social.target_guid == guid) then
				return invert_friend_list_[i]
			end
		end
	elseif(type == 2) then
		for i = 1, #invert_tuijian_list_ do
			if(invert_tuijian_list_[i].player.guid == guid) then
				return invert_tuijian_list_[i]
			end
		end
	end
	return nil
end

function TeamPanel.RemoveInvert(guid, type)
	if(type == 2) then
		for i = 1, #invert_tuijian_list_ do
			if(invert_tuijian_list_[i].player.guid == guid) then
				table.remove(invert_tuijian_list_, i)
				break
			end
		end
	elseif(type == 1) then
		for i = 1, #invert_friend_list_ do
			if(invert_friend_list_[i].social.target_guid == guid) then
				table.remove(invert_friend_list_, i)
				break
			end
		end
	end
end

function TeamPanel.RankFriend(friend_list)
	local comps = function(a, b)
		if(luabit.band(a.social.sflag, 2) == 0 and luabit.band(b.social.sflag, 2) ~= 0) then
			return true
		else
			return false
		end
	end
	for i = 1, #friend_list do
		for j = 1, #friend_list - i do
			if(comps(friend_list[j], friend_list[j + 1])) then
				local social_temp = friend_list[j + 1]
				friend_list[j + 1] = friend_list[j]
				friend_list[j] = social_temp
			end
		end
	end
	return friend_list
end

------------------------------------------------------



-----------------------服务器消息----------------------

function TeamPanel.SMSG_TEAM_OTHER_JOIN(message)
	local msg = msg_team_pb.smsg_team_other_join()
	msg:ParseFromString(message.luabuff)
	table.insert(team_msg_.member, msg.member)
	TeamPanel.RefreshTeamInf()
	if(TeamPanel.GetInvertPlayer(msg.member.player.guid, 1) ~= nil) then
		TeamPanel.GetInvertPlayer(msg.member.player.guid, 1).state = 0
		TeamPanel.GetInvertPlayer(msg.member.player.guid, 1).time = 0
	end
	TeamPanel.RemoveInvert(msg.member.player.guid, 2)
	TeamPanel.SelectPage(cur_page)
	local content = string.format(Config.get_t_script_str('TeamPanel_016'),msg.member.player.name)
	GUIRoot.ShowGUI("MessagePanel", {content})
end

function TeamPanel.SMSG_TEAM_EXIT(message)
	local msg = msg_team_pb.smsg_team_exit()
	msg:ParseFromString(message.luabuff)
	if(msg.player_guid == self.player.guid) then
		if(exit_code_ == 2) then
			TeamPanel.ReturnHall()
		elseif(exit_code_ == 3) then
			TeamPanel.JumpRecharge()
		end
	else
		TeamPanel.RemovePlayer(msg.player_guid, 0)
		if TeamPanel.GetPlayer(0) == nil then
			local index = TeamPanel.GetPlayerIndex(msg.leader_guid)
			local leader = TeamPanel.GetPlayer(index)
			leader.member_type = 1
			local content = string.format(Config.get_t_script_str('TeamPanel_017'),leader.player.name)
			GUIRoot.ShowGUI("MessagePanel", {content})
		end
		TeamPanel.RefreshTeamInf()
		if(TeamPanel.GetInvertPlayer(msg.player_guid, 1) ~= nil) then
			TeamPanel.GetInvertPlayer(msg.player_guid, 1).state = 0
			TeamPanel.GetInvertPlayer(msg.player_guid, 1).time = 0
		end
		if(cur_page == 0) then
			TeamPanel.SelectPage(cur_page)
		end
	end
end

function TeamPanel.SMSG_TEAM_KICK(message)
	local msg = msg_team_pb.smsg_team_kick()
	msg:ParseFromString(message.luabuff)
	if(msg.player_guid == self.player.guid) then
		TeamPanel.ReturnHall()
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('TeamPanel_018')})
	else
		TeamPanel.RemovePlayer(msg.player_guid, 1)
		TeamPanel.RefreshTeamInf()
		if(TeamPanel.GetInvertPlayer(msg.player_guid, 1) ~= nil) then
			TeamPanel.GetInvertPlayer(msg.player_guid, 1).state = 0
			TeamPanel.GetInvertPlayer(msg.player_guid, 1).time = 0
		end
		if(cur_page == 0) then
			TeamPanel.SelectPage(cur_page)
		end
	end
end

function TeamPanel.SMSG_TEAM_CHAT(message)
	local msg = msg_team_pb.smsg_team_chat()
	msg:ParseFromString(message.luabuff)
	local index = TeamPanel.GetPlayerIndex(msg.player_guid)
	local chat_msg_parent = nil
	
	local c_name = TeamPanel.GetPlayerName(msg.player_guid)
	local dialog = c_name..':'..msg.text
	local text_width = NGUIText.CalculatePrintedWidth(chatcontent_lb_.trueTypeFont, chatcontent_lb_.fontSize,dialog)
	local m = math.ceil(text_width / chatcontent_lb_.width)
	
	if chat_str_num > 0 then
		chat_str_ = tostring(chat_str_)..'\n'..dialog
	else
		chat_str_ = dialog
	end
	
	local d_num = 0
	if chat_str_num <= 5 and chat_str_num + m > 5 then
		d_num = chat_str_num + m  - 5
	else
		if chat_str_num + m <= 5 then
			d_num = 0
		else
			d_num = m
		end
	end

	chat_str_num = chat_str_num + m
	chatcontent_lb_.text = chat_str_
	if chat_str_num > 5 then
		chatcontent_lb_.gameObject.transform.localPosition = chatcontent_lb_.gameObject.transform.localPosition + Vector3(0,(chatcontent_lb_.fontSize + chatcontent_lb_.spacingY) * d_num,0)
	end

	
	
	if(index == 0) then
		chat_msg_parent = team_leader_
	elseif(index > 0) then
		chat_msg_parent = team_list_[index].transform
	end
	if(chat_msg_parent ~= nil) then
		local chat_msg_t = chat_msg_parent:Find("chat_msg")
		if(chat_msg_t == nil) then
			chat_msg_t = LuaHelper.Instantiate(chat_msg_res_).transform
			chat_msg_t.parent = chat_msg_parent
			chat_msg_t.name = "chat_msg"
			chat_msg_t.localPosition = Vector3(-23, 55, 0)
			chat_msg_t.localScale = Vector3.one
		end
		chat_msg_t:GetComponent("UISprite").alpha = 1
		local chat_text = chat_msg_t:Find("Label"):GetComponent("UILabel")
		chat_text.text = msg.text
		chat_text:ProcessText()
		local height = chat_text.transform:GetComponent('UIWidget').height
		local width = chat_text.transform:GetComponent('UIWidget').width
		chat_msg_t:GetComponent("UIWidget").width = height + 28
		chat_msg_t:GetComponent("UIWidget").height = width + 26
		twnMgr:Add_Tween_Alpha(chat_msg_t.gameObject, 0.2, 1, 0, 3, 5)
		chat_msg_t.gameObject:SetActive(true)
	end
end

function TeamPanel.SMSG_TEAM_TUIJIAN(message)
	local msg = msg_team_pb.smsg_team_tuijian()
	msg:ParseFromString(message.luabuff)
	local invert_tuijian_list = invert_tuijian_list_
	invert_tuijian_list_ = {}
	for i = 1, #msg.players do
		TeamPanel.NewInvert(msg.players[i], 2, invert_tuijian_list)
	end
	TeamPanel.InitTuiJianList()
end

function TeamPanel.SMSG_MULTI_BATTLE(message)
	local msg = msg_team_pb.smsg_multi_battle()
	msg:ParseFromString(message.luabuff)
	TeamPanel.ReturnHall()
	if(msg.code == "") then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('TeamPanel_019')})
	else
		self.battle_code = msg.code
		-- 扣战场道具
		if msg.num > 1 then
			self.add_all_type_num(41, 1)
		end
		--self.player.battle_reset_skill_num = 0
		BattleStateTcp.Init(msg.tcp_ip,msg.tcp_port)
		BattleTcp.Connect(msg.udp_ip, msg.udp_port)
	end
end

function TeamPanel.SMSG_SOCAIL_LOOK(message)
	local msg = msg_social_pb.smsg_social_look()
	msg:ParseFromString(message.luabuff)
	for i = 1, #msg.socials do
		TeamPanel.NewInvert(msg.socials[i], 1)
	end
	invert_friend_list_ = TeamPanel.RankFriend(invert_friend_list_)
	TeamPanel.SelectPage(0)
end

function TeamPanel.SMSG_SOCIAL_STAT(msg)
	if(lua_script_ ~= nil) then
		if(TeamPanel.GetInvertPlayer(msg.target_guid, 1) ~= nil) then
			TeamPanel.GetInvertPlayer(msg.target_guid, 1).social.sflag = msg.stat
		end
		invert_friend_list_ = TeamPanel.RankFriend(invert_friend_list_)
		if(cur_page == 0) then
			TeamPanel.SelectPage(cur_page)
		end
	end
end

function TeamPanel.SMSG_SOCIAL_DELETE(msg)
	if(lua_script_ ~= nil) then
		TeamPanel.RemoveInvert(msg.target_guid, 1)
		invert_friend_list_ = TeamPanel.RankFriend(invert_friend_list_)
		if(cur_page == 0) then
			TeamPanel.SelectPage(cur_page)
		end
	end
end

function TeamPanel.SMSG_SOCIAL_ADD(msg)
	if(lua_script_ ~= nil) then
		TeamPanel.NewInvert(msg.social, 1)
		invert_friend_list_ = TeamPanel.RankFriend(invert_friend_list_)
		if(cur_page == 0) then
			TeamPanel.SelectPage(cur_page)
		end
	end
end

function TeamPanel.SMSG_SOCIAL_BLACK(msg)
	if(lua_script_ ~= nil) then
		TeamPanel.RemoveInvert(msg.social.target_guid, 1)
		invert_friend_list_ = TeamPanel.RankFriend(invert_friend_list_)
		if(cur_page == 0) then
			TeamPanel.SelectPage(cur_page)
		end
	end
end
-------------------------------------------------------
