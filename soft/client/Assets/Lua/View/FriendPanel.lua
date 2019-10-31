FriendPanel = {}

local lua_script_

local friend_list_ = {}
local unread_list_ = {}
local black_list_ = {}
local search_list_ = {}
local apply_list_ = {}
local look_type_ = 0
local cur_page_ = 0

local social_view_
local search_view_
local apply_view_
local tuijian_view_
local gift_view_

local social_res_
local search_res_
local apply_res_
local gift_res_

local friend_panel_
local black_panel_
local search_panel_
local apply_panel_
local tuijian_panel_
local gift_panel_
local empty_panel_
local chat_panel_

local view_pos = Vector3(0, 0, 0)
local social_pos = Vector3(-233, 138, 0)

---------chat----------------

local show_msg_prefabs_ = {}
local msg_prefab_height = {}
local chat_avatar_other_pos = Vector3(-88, -20, 0)
local chat_avatar_self_pos = Vector3(82, -20, 0)

local chat_view_
local chat_msg_self_res_
local chat_msg_other_res_
local chat_input_
local unread_btn_
local express_panel_
local express_res_
local express_root_

local chat_limit_ = 20
local minh_ = 0
local maxh_ = 0
local viewh_ = 348
local line_ = 30
local pre_height = 0
local first_height = 0
local base_line_width = 190
local space = '        '
-----------------------------

local search_input_
local friend_btn_
local black_btn_
local apply_btn_
local gift_btn_
local apply_flag_

local pre_social_
local cur_social_id_
local cur_gift_social_

local cur_apply_

local avatar_pos = Vector3(50, 0, 0)
local cup_pos = Vector3(135, 0, 0)
local black_flag = false

local state_font_ = {Config.get_t_script_str('FriendPanel_001'), Config.get_t_script_str('FriendPanel_002'), Config.get_t_script_str('FriendPanel_003')}

function FriendPanel.Awake(obj)
	GUIRoot.UIEffect(obj, 0)
	GUIRoot.ShowGUI('BackPanel', {1})
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	friend_panel_ = obj.transform:Find("friend_panel")
	black_panel_ = obj.transform:Find("black_panel")
	search_panel_ = obj.transform:Find("search_panel")
	apply_panel_ = obj.transform:Find("apply_panel")
	tuijian_panel_ = obj.transform:Find("tuijian_panel")
	gift_panel_ = obj.transform:Find("gift_panel")
	empty_panel_ = obj.transform:Find("empty_panel")
	chat_panel_ = obj.transform:Find("chat_panel")
	express_panel_ = obj.transform:Find("express_panel")
	
	chat_view_ = chat_panel_:Find("chat_view")
	chat_msg_self_res_ = chat_panel_:Find("chat_msg_self_res").gameObject
	chat_msg_other_res_ = chat_panel_:Find("chat_msg_other_res").gameObject
	chat_input_ = chat_panel_:Find("chat_input"):GetComponent("UIInput")
	unread_btn_ = chat_panel_:Find("unread_btn")
	
	social_view_ = obj.transform:Find("social_view")
	search_view_ = search_panel_:Find("search_view")
	apply_view_ = apply_panel_:Find("apply_view")
	tuijian_view_ = tuijian_panel_:Find("tuijian_view")
	gift_view_ = gift_panel_:Find("gift_view")
	
	social_res_ = obj.transform:Find("social_res").gameObject
	search_res_ = search_panel_:Find("search_res").gameObject
	apply_res_ = apply_panel_:Find("apply_res").gameObject
	gift_res_ = gift_panel_:Find("gift_res").gameObject
	express_res_ = express_panel_:Find("express_res").gameObject
	express_root_ = express_panel_:Find("bg")
	
	view_pos = social_view_.transform.localPosition
	
	search_input_ = search_panel_:Find("search_input"):GetComponent("UIInput")
	
	friend_btn_ = obj.transform:Find("left_panel/friend_btn")
	black_btn_ = obj.transform:Find("left_panel/black_btn")
	apply_flag_ = apply_panel_:Find("apply_flag")
	
	local search_btn = friend_panel_:Find("search_btn").gameObject
	apply_btn_ = friend_panel_:Find("apply_btn")
	local tuijian_btn = friend_panel_:Find("tuijian_btn").gameObject
	local change_btn = tuijian_panel_:Find("change_btn").gameObject
	local search_begin = search_panel_:Find("search_begin").gameObject
	gift_btn_ = friend_panel_:Find("gift_btn")
	local get_gift_btn = gift_panel_:Find("get_gift_btn").gameObject
	local friend_del = friend_panel_:Find("friend_del").gameObject
	local black_del = black_panel_:Find("black_del").gameObject
	local clear_btn = black_panel_:Find("clear_btn").gameObject
	local apply_close_btn = apply_panel_:Find("close_btn").gameObject
	local tuijian_close_btn = tuijian_panel_:Find("close_btn").gameObject
	local gift_close_btn = gift_panel_:Find("close_btn").gameObject
	local search_close_btn = search_panel_:Find("close_btn").gameObject
	local send_btn = chat_panel_:Find("send_btn").gameObject
	local express_btn = chat_panel_:Find("express_btn").gameObject
	
	lua_script_:AddButtonEvent(friend_btn_.gameObject, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(black_btn_.gameObject, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(search_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(apply_btn_.gameObject, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(change_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(search_begin, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(gift_btn_.gameObject, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(get_gift_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(tuijian_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(friend_del, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(black_del, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(clear_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(apply_close_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(tuijian_close_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(gift_close_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(search_close_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(send_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(express_btn, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(unread_btn_.gameObject, "click", FriendPanel.Click)
	lua_script_:AddButtonEvent(apply_flag_.gameObject, "click", FriendPanel.Click)
	
	lua_script_:AddButtonEvent(chat_view_.gameObject ,'onDragFinished', FriendPanel.OnDragFinished)
	FriendPanel.RegisterMessage()
	FriendPanel.InitExpressPanel()
	FriendPanel.ClosePanel()
	FriendPanel.ShowTip()
	FriendPanel.InitApplyFlag()
end

function FriendPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_SOCIAL_GIFT, FriendPanel.SMSG_SOCIAL_GIFT)
	Message.register_net_handle(opcodes.SMSG_SOCAIL_LOOK, FriendPanel.SMSG_SOCAIL_LOOK)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_GIFT_GET, FriendPanel.SMSG_SOCIAL_GIFT_GET)
	Message.register_net_handle(opcodes.SMSG_PLAYER_LOOK, FriendPanel.SMSG_PLAYER_LOOK)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_APPLY_FLAG, FriendPanel.SMSG_SOCIAL_APPLY_FLAG)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_TUIJIAN, FriendPanel.SMSG_SOCIAL_TUIJIAN)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_SEARCH, FriendPanel.SMSG_SOCIAL_SEARCH)
	Message.register_handle("back_panel_msg", FriendPanel.Back)
	Message.register_handle("back_panel_recharge", FriendPanel.Recharge)
	Message.register_handle("team_join_msg", FriendPanel.TeamJoin)
end

function FriendPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_SOCIAL_GIFT, FriendPanel.SMSG_SOCIAL_GIFT)
	Message.remove_net_handle(opcodes.SMSG_SOCAIL_LOOK, FriendPanel.SMSG_SOCAIL_LOOK)
	Message.remove_net_handle(opcodes.SMSG_SOCIAL_GIFT_GET, FriendPanel.SMSG_SOCIAL_GIFT_GET)
	Message.remove_net_handle(opcodes.SMSG_PLAYER_LOOK, FriendPanel.SMSG_PLAYER_LOOK)
	Message.remove_net_handle(opcodes.SMSG_SOCIAL_APPLY_FLAG, FriendPanel.SMSG_SOCIAL_APPLY_FLAG)
	Message.remove_net_handle(opcodes.SMSG_SOCIAL_TUIJIAN, FriendPanel.SMSG_SOCIAL_TUIJIAN)
	Message.remove_net_handle(opcodes.SMSG_SOCIAL_SEARCH, FriendPanel.SMSG_SOCIAL_SEARCH)
	Message.remove_handle("back_panel_msg", FriendPanel.Back)
	Message.remove_handle("back_panel_recharge", FriendPanel.Recharge)
	Message.remove_handle("team_join_msg", FriendPanel.TeamJoin)
end

function FriendPanel.OnDestroy()
	cur_social_id_ = 0
	lua_script_ = nil
	friend_list_ = {}
	black_list_ = {}
	cur_apply_ = nil
	black_flag = false
	FriendPanel.RemoveMessage()
end

function FriendPanel.OnParam(parm)
	friend_list_ = parm[1]
	friend_list_ = FriendPanel.RankSocial(friend_list_)
	FriendPanel.InitChatMsg()
	FriendPanel.CountUnreadNum()
	FriendPanel.SelectPage(0)
	FriendPanel.CountOnlineNum()
end

function FriendPanel.Back()
	if(not GUIRoot.HasGUI("ZonePanel")) then
		GUIRoot.HideGUI('FriendPanel')
		GUIRoot.ShowGUI('HallPanel')
		GUIRoot.HideGUI('BackPanel')
	end
end

function FriendPanel.Recharge()
	GUIRoot.HideGUI('FriendPanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function FriendPanel.TeamJoin()
	GUIRoot.HideGUI('FriendPanel')
end


-----------------刷新列表--------------------

function FriendPanel.SelectPage(page)
	friend_panel_.gameObject:SetActive(false)
	black_panel_.gameObject:SetActive(false)
	friend_panel_:Find("friend_del").gameObject:SetActive(false)
	black_panel_:Find("black_del").gameObject:SetActive(false)
	friend_btn_:Find("highlight").gameObject:SetActive(false)
	black_btn_:Find("highlight").gameObject:SetActive(false)
	friend_btn_:Find("Label"):GetComponent("UILabel").text = "[7E90A4]"..Config.get_t_script_str('FriendPanel_004')
	black_btn_:Find("Label"):GetComponent("UILabel").text = "[7E90A4]"..Config.get_t_script_str('FriendPanel_005')
	cur_page_ = page
	if(page == 0) then
		friend_btn_:Find("highlight").gameObject:SetActive(true)
		friend_btn_:Find("Label"):GetComponent("UILabel").text = "[E8FCFF]"..Config.get_t_script_str('FriendPanel_004')
		FriendPanel.InitSocialList(friend_list_, 0)
		friend_panel_.gameObject:SetActive(true)
	elseif(page == 1) then
		black_btn_:Find("Label"):GetComponent("UILabel").text = "[E8FCFF]"..Config.get_t_script_str('FriendPanel_005')
		black_btn_:Find("highlight").gameObject:SetActive(true)
		if(black_flag) then
			FriendPanel.InitSocialList(black_list_, 1)
			black_panel_.gameObject:SetActive(true)
		else
			black_flag = true
			FriendPanel.SocialLook(3)
		end
	end
	FriendPanel.ShowTip()
end

function FriendPanel.InitSocialList(social_list, page)
	pre_social_ = nil
	cur_gift_social_ = nil
	local pre_social = nil
	chat_panel_.gameObject:SetActive(false)
	black_panel_:Find("tip").gameObject:SetActive(false)
	express_panel_.gameObject:SetActive(false)
	if(social_view_.childCount > 0) then
		for i = 0, social_view_.childCount - 1 do
			GameObject.Destroy(social_view_:GetChild(i).gameObject)
		end
		if social_view_:GetComponent('SpringPanel') ~= nil then
			social_view_:GetComponent('SpringPanel').enabled = false
		end
		local uv = social_view_:GetComponent('UIScrollView')
		uv.panel.clipOffset = Vector2(0, 0)
		social_view_.localPosition = view_pos
	end
	empty_panel_.gameObject:SetActive(false)
	empty_panel_.parent:Find("line").gameObject:SetActive(true)
	
	local comp = function(a,b)
		local a_s = 0
		local b_s = 0
		
		if luabit.band(a.sflag, 8) ~= 0 then
			a_s = 8
		elseif luabit.band(a.sflag, 2) ~= 0 then
			a_s = 2
		end
		
		if luabit.band(b.sflag, 8) ~= 0 then
			b_s = 8
		elseif luabit.band(b.sflag, 2) ~= 0 then
			b_s = 2
		end
		
		return a_s > b_s
    end
	table.sort(social_list,comp)
	
	if(#social_list == 0) then
		cur_social_id_ = 0
		if(page == 0) then
			empty_panel_:Find("tip/Label"):GetComponent("UILabel").text = Config.get_t_script_str('FriendPanel_006') --"您当前没有任何好友\n可通过推荐添加好友或者搜索好友"
		elseif(page == 1) then
			empty_panel_:Find("tip/Label"):GetComponent("UILabel").text = Config.get_t_script_str('FriendPanel_007') --"黑名单空空如也"
		end
		empty_panel_.gameObject:SetActive(true)
		empty_panel_.parent:Find("line").gameObject:SetActive(false)
	end
	for i = 0, #social_list - 1 do
		local social_temp = social_list[i + 1]
		local social_t = LuaHelper.Instantiate(social_res_)
		social_t.transform.parent = social_view_
		social_t.transform.localPosition = social_pos - Vector3(0, 102 * i, 0)
		social_t.transform.localScale = Vector3.one
		local name = social_t.transform:Find("name"):GetComponent("UILabel")
		local state = social_t.transform:Find("state"):GetComponent("UILabel")
		local lv = social_t.transform:Find("lv/Label"):GetComponent("UILabel")
		local gold_btn = social_t.transform:Find("gold_btn").gameObject
		social_t.transform:Find("tip").gameObject:SetActive(false)
		local color = NoticePanel.GetVipNameColor(social_temp)
		name.text = social_temp.name
		IconPanel.InitVipLabel(name, color)
		lv.text = social_temp.level
		if luabit.band(social_temp.sflag, 8) ~= 0 then
			state.text = state_font_[2]
		elseif luabit.band(social_temp.sflag, 2) ~= 0 then
			state.text = state_font_[1]
		else
			state.text = state_font_[3]
		end
		if social_temp.stype == 3 or self.send_social_golds(social_temp.target_guid) then
			gold_btn:SetActive(false)
		end
		if(unread_list_[social_temp.target_guid] ~= nil and unread_list_[social_temp.target_guid] > 0) then
			social_t.transform:Find("tip").gameObject:SetActive(true)
			social_t.transform:Find("tip/Label"):GetComponent("UILabel").text = unread_list_[social_temp.target_guid]
		end
		if(social_temp.target_guid == cur_social_id_) then
			pre_social = social_t
		end
		local cup_inf_t = IconPanel.GetCupInf(social_temp.cup)
		cup_inf_t.transform.parent = social_t.transform
		cup_inf_t.transform.localPosition = Vector3(100, 0, 0)
		cup_inf_t.transform.localScale = Vector3.one
		cup_inf_t:SetActive(true)
		local avatar_t = AvaIconPanel.GetAvatarSex("social_res", FriendPanel.OpenZonePanel, social_temp.avatar,'', social_temp.toukuang, social_temp.sex)
		avatar_t.transform.parent = social_t.transform
		avatar_t.transform.localPosition = avatar_pos
		avatar_t.transform.localScale = Vector3.one
		avatar_t:SetActive(true)
		avatar_t.transform:Find("avatar").name = social_temp.target_guid
		social_t.name = social_temp.target_guid
		lua_script_:AddButtonEvent(gold_btn, "click", FriendPanel.SendGift)
		lua_script_:AddButtonEvent(social_t, "click", FriendPanel.ChoseSocial)
		social_t:SetActive(true)
	end
	if(pre_social ~= nil) then
		FriendPanel.ChoseSocial(pre_social)
	else
		cur_social_id_ = 0
	end
end

function FriendPanel.InitSearchList(show_type, search_list)
	local search_view = nil
	local y_pos = 0
	local show_panel = nil
	if(show_type == 1) then
		search_view = search_view_
		y_pos = 50
		show_panel = search_panel_
	elseif(show_type == 2) then
		search_view = tuijian_view_
		y_pos = 120
		show_panel = tuijian_panel_
	end
	if(search_view.childCount > 0) then
		for i = 0, search_view.childCount - 1 do
			GameObject.Destroy(search_view:GetChild(i).gameObject)
		end
		if search_view:GetComponent('SpringPanel') ~= nil then
			search_view:GetComponent('SpringPanel').enabled = false
		end
		local uv = search_view:GetComponent('UIScrollView')
		uv.panel.clipOffset = Vector2(0, 0)
		search_view.localPosition = Vector3(0, 0, 0)
	end
	show_panel:Find("empty_tip").gameObject:SetActive(false)
	if(#search_list == 0) then
		show_panel:Find("empty_tip").gameObject:SetActive(true)
	end
	for i = 0, #search_list - 1 do
		local search_temp = search_list[i + 1]
		local search_t = LuaHelper.Instantiate(search_res_)
		search_t.transform.parent = search_view
		search_t.transform.localPosition = Vector3(0, y_pos - 125 * i, 0)
		search_t.transform.localScale = Vector3.one
		local name = search_t.transform:Find("name"):GetComponent("UILabel")
		local lv = search_t.transform:Find("lv/Label"):GetComponent("UILabel")
		local add_btn = search_t.transform:Find("add_btn").gameObject
		local cup_inf_t = IconPanel.GetCupInf(search_temp.cup)
		local color = NoticePanel.GetVipNameColor(search_temp)
		name.text = search_temp.name
		IconPanel.InitVipLabel(name, color)
		lv.text = search_temp.level
		cup_inf_t.transform.parent = search_t.transform
		cup_inf_t.transform.localPosition = Vector3(-130, -25, 0)
		cup_inf_t.transform.localScale = Vector3.one
		cup_inf_t:SetActive(true)
		local avatar_t = AvaIconPanel.GetAvatarSex("avatar_res", FriendPanel.OpenZonePanel, search_temp.avatar,'', search_temp.toukuang, search_temp.sex)
		avatar_t.transform.parent = search_t.transform
		avatar_t.transform.localPosition = Vector3(-240, 0, 0)
		avatar_t.transform.localScale = Vector3.one
		avatar_t:SetActive(true)
		avatar_t.transform:Find("avatar").name = search_temp.target_guid
		add_btn.name = search_temp.target_guid
		lua_script_:AddButtonEvent(add_btn, "click", FriendPanel.AddFriend)
		search_t:SetActive(true)
	end
	show_panel.gameObject:SetActive(true)
end

function FriendPanel.InitApplyList(apply_list)
	if(apply_view_.childCount > 0) then
		for i = 0, apply_view_.childCount - 1 do
			GameObject.Destroy(apply_view_:GetChild(i).gameObject)
		end
		if apply_view_:GetComponent('SpringPanel') ~= nil then
			apply_view_:GetComponent('SpringPanel').enabled = false
		end
		local uv = apply_view_:GetComponent('UIScrollView')
		uv.panel.clipOffset = Vector2(0, 0)
		apply_view_.localPosition = Vector3(0, 0, 0)
	end
	apply_panel_:Find("empty_tip").gameObject:SetActive(false)
	if(#apply_list == 0) then
		apply_panel_:Find("empty_tip").gameObject:SetActive(true)
	end
	for i = 0, #apply_list - 1 do
		local apply_temp = apply_list[i + 1]
		local apply_t = LuaHelper.Instantiate(apply_res_)
		apply_t.transform.parent = apply_view_
		apply_t.transform.localPosition = Vector3(0, 115 - 125 * i, 0)
		apply_t.transform.localScale = Vector3.one
		local name = apply_t.transform:Find("name"):GetComponent("UILabel")
		local lv = apply_t.transform:Find("lv/Label"):GetComponent("UILabel")
		local verify = apply_t.transform:Find("verify"):GetComponent("UILabel")
		local ok_btn = apply_t.transform:Find("ok").gameObject
		local no_btn = apply_t.transform:Find("no").gameObject
		local color = NoticePanel.GetVipNameColor(apply_temp)
		name.text = apply_temp.name
		IconPanel.InitVipLabel(name, color)
		lv.text = apply_temp.level
		local content = string.format(Config.get_t_script_str('FriendPanel_008'),apply_temp.verify)
		verify.text = content --"验证消息："..apply_temp.verify
		apply_t.transform:Find("tip").gameObject:SetActive(false)
		local cup_inf_t = IconPanel.GetCupInf(apply_temp.cup)
		cup_inf_t.transform.parent = apply_t.transform
		cup_inf_t.transform.localPosition = Vector3(-215, 0, 0)
		cup_inf_t.transform.localScale = Vector3.one
		cup_inf_t:SetActive(true)
		local avatar_t = AvaIconPanel.GetAvatarSex("avatar_res", FriendPanel.OpenZonePanel, apply_temp.avatar,'', apply_temp.toukuang, apply_temp.sex)
		avatar_t.transform.parent = apply_t.transform
		avatar_t.transform.localPosition = Vector3(-325, 0, 0)
		avatar_t.transform.localScale = Vector3.one
		avatar_t:SetActive(true)
		avatar_t.transform:Find("avatar").name = apply_temp.target_guid
		lua_script_:AddButtonEvent(ok_btn, "click", FriendPanel.DealApply)
		lua_script_:AddButtonEvent(no_btn, "click", FriendPanel.DealApply)
		apply_t.name = apply_temp.target_guid
		apply_t:SetActive(true)
	end
	apply_panel_.gameObject:SetActive(true)
end

function FriendPanel.InitGiftList()
	if(gift_view_.childCount > 0) then
		for i = 0, gift_view_.childCount - 1 do
			GameObject.Destroy(gift_view_:GetChild(i).gameObject)
		end
		if gift_view_:GetComponent('SpringPanel') ~= nil then
			gift_view_:GetComponent('SpringPanel').enabled = false
		end
		local uv = gift_view_:GetComponent('UIScrollView')
		uv.panel.clipOffset = Vector2(0, 0)
		gift_view_.localPosition = Vector3(0, 0, 0)
	end
	gift_panel_:Find("empty_tip").gameObject:SetActive(false)
	gift_panel_:Find("get_gift_btn"):GetComponent("UISprite").spriteName = "b1"
	local num = 0
	for i = 1, #friend_list_ do
		local gift_temp = friend_list_[i]
		if(gift_temp.gold > 0) then
			local gift_t = LuaHelper.Instantiate(gift_res_)
			gift_t.transform.parent = gift_view_
			gift_t.transform.localPosition = Vector3(0, 120 - 125 * num, 0)
			gift_t.transform.localScale = Vector3.one
			local name = gift_t.transform:Find("name"):GetComponent("UILabel")
			local lv = gift_t.transform:Find("lv/Label"):GetComponent("UILabel")
			local gold = gift_t.transform:Find("gold"):GetComponent("UILabel")
			local cup_inf_t = IconPanel.GetCupInf(gift_temp.cup)
			local color = NoticePanel.GetVipNameColor(gift_temp)
			name.text = gift_temp.name
			IconPanel.InitVipLabel(name, color)
			lv.text = gift_temp.level
			gold.text = self.font_color[3]..(gift_temp.gold)
			cup_inf_t.transform.parent = gift_t.transform
			cup_inf_t.transform.localPosition = Vector3(-130, -25, 0)
			cup_inf_t.transform.localScale = Vector3.one
			cup_inf_t:SetActive(true)
			local avatar_t = AvaIconPanel.GetAvatarSex("avatar_res", nil, gift_temp.avatar, '', gift_temp.toukuang, gift_temp.sex)
			avatar_t.transform.parent = gift_t.transform
			avatar_t.transform.localPosition = Vector3(-240, 0, 0)
			avatar_t.transform.localScale = Vector3.one
			avatar_t:SetActive(true)
			gift_t:SetActive(true)
			num = num + 1
		end
	end
	if(num == 0) then
		gift_panel_:Find("empty_tip").gameObject:SetActive(true)
		gift_panel_:Find("get_gift_btn"):GetComponent("UISprite").spriteName = "b1_gray"
	end
	gift_panel_.gameObject:SetActive(true)
end

function FriendPanel.SocialLook(type)
	local msg = msg_social_pb.cmsg_social_look()
	msg.type = type
	look_type_ = type
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_SOCIAL_LOOK, data, {opcodes.SMSG_SOCAIL_LOOK})
end

function FriendPanel.ChoseSocial(obj)
	if(pre_social_ ~= nil) then
		pre_social_.transform:GetComponent("UISprite").spriteName = "nxxt_004"
	end
	cur_social_id_ = obj.name
	if(cur_page_ == 0 and pre_social_ ~= obj) then
		FriendPanel.InitChatPanel()
		friend_panel_:Find("friend_del").gameObject:SetActive(true)
		obj.transform:Find("tip").gameObject:SetActive(false)
	elseif(cur_page_ == 1) then
		black_panel_:Find("tip").gameObject:SetActive(true)
		black_panel_:Find("black_del").gameObject:SetActive(true)
	end
	pre_social_ = obj
	obj.transform:GetComponent("UISprite").spriteName = "nxxt_002"
end

function FriendPanel.DelSocial(page)
	if(page == 0) then
		local social = FriendPanel.GetSocialGuid(friend_list_, cur_social_id_)
		if(social ~= nil) then
			local content = string.format(Config.get_t_script_str('FriendPanel_009'),social.name)
			GUIRoot.ShowGUI("SelectPanel", {content,Config.get_t_script_str('FriendPanel_010'), FriendPanel.DelFriend,Config.get_t_script_str('FriendPanel_011')})
		end
	elseif(page == 1) then
		local social = FriendPanel.GetSocialGuid(black_list_, cur_social_id_)
		if(social ~= nil) then
			local content = string.format(Config.get_t_script_str('FriendPanel_012'),social.name)
			GUIRoot.ShowGUI("SelectPanel", {content,Config.get_t_script_str('FriendPanel_010'), FriendPanel.DelBlack,Config.get_t_script_str('FriendPanel_011')})
		end
	end
end

function FriendPanel.DelFriend()
	local msg = msg_social_pb.cmsg_social_delete()
	msg.target_guid = cur_social_id_
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_SOCIAL_DELETE, data, {opcodes.SMSG_SOCIAL_DELETE})
end

function FriendPanel.DelBlack()
	local msg = msg_social_pb.cmsg_social_delete()
	msg.target_guid = cur_social_id_
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_SOCIAL_DELETE, data, {opcodes.SMSG_SOCIAL_DELETE})
end

function FriendPanel.SendGift(obj)
	if(#self.player.social_golds >= 10) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('FriendPanel_013')})
	else
		cur_gift_social_ = obj.transform.parent
		local msg = msg_social_pb.cmsg_social_gift()
		msg.target_guid = obj.transform.parent.name
		msg.gold = 5
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_SOCIAL_GIFT, data, {opcodes.SMSG_SOCIAL_GIFT})
	end
end

function FriendPanel.AddFriend(obj)
	NoticePanel.InitAddPanel(obj.name)
end


function FriendPanel.AddPlayerBlack(player)
	if(#self.black_guids >= 50) then
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('FriendPanel_014')})
		return 0
	end
	local msg = msg_social_pb.cmsg_social_black()
	msg.target_guid = player.guid
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_SOCIAL_BLACK, data, {opcodes.SMSG_SOCIAL_BLACK})
end

function FriendPanel.DealApply(obj)
	cur_apply_ = obj.transform.parent
	local target_guid = obj.transform.parent.name
	local msg = msg_social_pb.cmsg_social_add()
	msg.target_guid = target_guid
	if(obj.name == "ok") then
		if(#self.friend_guids >= 50) then
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('FriendPanel_015'),Config.get_t_script_str('FriendPanel_010')})
			return 0
		end
		msg.agree = true
		cur_apply_:Find("tip"):GetComponent("UILabel").text = "[57FC5B]"..Config.get_t_script_str('FriendPanel_017')
	elseif(obj.name == "no") then
		msg.agree = false
		cur_apply_:Find("tip"):GetComponent("UILabel").text = "[f01c1c]"..Config.get_t_script_str('FriendPanel_016')
	end
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_SOCIAL_ADD, data, {opcodes.SMSG_SOCIAL_ADD, opcodes.SMSG_SOCIAL_DELETE})
end

function FriendPanel.ClearBlack()
	local msg = msg_social_pb.cmsg_social_delete()
	msg.target_guid = 0
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_SOCIAL_DELETE, data, {opcodes.SMSG_SOCIAL_DELETE})
end
----------------------------------------------


----------------------刷新界面-------------------------

function FriendPanel.ClosePanel()
	search_panel_.gameObject:SetActive(false)
	apply_panel_.gameObject:SetActive(false)
	tuijian_panel_.gameObject:SetActive(false)
	gift_panel_.gameObject:SetActive(false)
	search_input_.value = ""
	cur_apply_ = nil
	search_panel_:Find("empty_tip").gameObject:SetActive(false)
	if(search_view_.childCount > 0) then
		for i = 0, search_view_.childCount - 1 do
			GameObject.Destroy(search_view_:GetChild(i).gameObject)
		end
		if search_view_:GetComponent('SpringPanel') ~= nil then
			search_view_:GetComponent('SpringPanel').enabled = false
		end
		local uv = search_view_:GetComponent('UIScrollView')
		uv.panel.clipOffset = Vector2(0, 0)
		search_view_.localPosition = Vector3(0, 0, 0)
	end
end

function FriendPanel.RankSocial(social_list)
	local comps = function(a, b)
		if(luabit.band(a.sflag, 2) == 0 and luabit.band(b.sflag, 2) ~= 0) then
			return true
		else
			return false
		end
	end
	for i = 1, #social_list do
		for j = 1, #social_list - i do
			if(comps(social_list[j], social_list[j + 1])) then
				local social_temp = social_list[j + 1]
				social_list[j + 1] = social_list[j]
				social_list[j] = social_temp
			end
		end
	end
	return social_list
end


function FriendPanel.InitChatPanel()
	show_msg_prefabs_ = {}
	msg_prefab_height = {}
	pre_height = 0
	minh_ = 0
	maxh_ = 0
	unread_msg = 0
	express_panel_.gameObject:SetActive(false)
	chat_input_.value = ""
	if(chat_view_.childCount > 0) then
		for i = 0, chat_view_.childCount - 1 do
			GameObject.Destroy(chat_view_:GetChild(i).gameObject)
		end
		if chat_view_:GetComponent('SpringPanel') ~= nil then
			chat_view_:GetComponent('SpringPanel').enabled = false
		end
		local uv = chat_view_:GetComponent('UIScrollView')
		uv.panel.clipOffset = Vector2(0, 0)
		chat_view_.localPosition = Vector3(226, 0, 0)
	end
	local social = FriendPanel.GetSocialGuid(friend_list_, cur_social_id_)
	self.unread_msg_num = self.unread_msg_num - unread_list_[cur_social_id_]
	unread_list_[social.target_guid] = 0
	FriendPanel.ShowTip()
	NoticePanel.SaveSocialMsg(social.target_guid)
	chat_panel_.gameObject:SetActive(true)
	local chat_msg = NoticePanel.GetSocialMsg(social.target_guid)
	if(#chat_msg >= line_) then
		for i = 1, line_ do
			local msg = chat_msg[#chat_msg - line_ + i]
			FriendPanel.NewMesssage(msg)
		end
	else
		for i = 1, #chat_msg do
			local msg = chat_msg[i]
			FriendPanel.NewMesssage(msg)
		end
	end
end

function FriendPanel.CheckLatestMsg()
	if chat_view_:GetComponent('SpringPanel') ~= nil then
		chat_view_:GetComponent('SpringPanel').enabled = false
	end	
	local uv = chat_view_:GetComponent('UIScrollView')
	local offy = uv.panel.clipOffset.y
	local tmaxh = maxh_ + offy
	if(tmaxh > viewh_) then
		uv.panel.clipOffset = Vector2(0, viewh_ - maxh_ - pre_height + 86)
		chat_view_.localPosition = Vector3(226, maxh_ - viewh_ + pre_height - 86, 0)
	end
	unread_msg = 0
	unread_btn_.gameObject:SetActive(false)
end

function FriendPanel.NewMesssage(msg)
	local chat_prefab = nil
	local chat_msg_res = nil
	local chat_msg_x_pos = 0
	local chat_avatar_pos = Vector3(0, 0, 0)
	local collider_off_x = 0
	local player = {}
	if(msg.player_guid == self.player.guid) then
		chat_msg_res = chat_msg_self_res_
		chat_msg_x_pos = 83
		chat_avatar_pos = chat_avatar_self_pos
		collider_off_x = -50
	else
		chat_msg_res = chat_msg_other_res_
		chat_msg_x_pos = -16
		chat_avatar_pos = chat_avatar_other_pos
		collider_off_x = 50
	end
	chat_prefab = LuaHelper.Instantiate(chat_msg_res)
	chat_prefab.transform.parent = chat_view_
	chat_prefab.transform.localScale = Vector3.one
	local avatar_res = AvaIconPanel.GetAvatarSex("social_res", FriendPanel.OpenZonePanel, msg.avatar,'', msg.toukuang, msg.sex)
	avatar_res.transform.parent = chat_prefab.transform
	avatar_res.transform.localScale = Vector3.one
	avatar_res.transform.localPosition = chat_avatar_pos
	avatar_res.transform:Find("avatar").name = msg.player_guid
	avatar_res:SetActive(true)
	chat_prefab.transform:Find('lv'):GetComponent('UILabel').text = msg.level
	chat_prefab.transform:GetComponent('UILabel').text = msg.player_name
	IconPanel.InitVipLabel(chat_prefab.transform:GetComponent('UILabel'), msg.name_color)
	chat_prefab.transform:Find('time'):GetComponent('UILabel').text = get_time_show(msg.time, 1)
	local text_label = chat_prefab.transform:Find('text'):GetComponent('UILabel')
	local last_start_index = 1
	local row = 0
	local text = {}
	local str = msg.text
	base_line_width = 190
	text_label.overflowWidth = base_line_width
	text = stringTotable(msg.text)
	local count = #text
	local ex_num = 0
	for i = 1, count do
		if(text[i] == '[' and i + 7 <= #text and text[i + 1] == '#') then
			local ex_name = string.sub(str, i + 2, i + 6)
			local ex_id = tonumber(ex_name)
			local ex_pos = Vector3.zero
			local fx = 0
			for  j = 0, 7 do 
				table.remove(text, i)
			end
			table.insert(text, i, space)
			str = tableTostring(text)
			text = stringTotable(str)
			local text_width = NGUIText.CalculatePrintedWidth(text_label.trueTypeFont, text_label.fontSize, string.sub(str, last_start_index, i + 7))
			if(text_width  > base_line_width) then
				ex_num = ex_num + 1
				fx = 16
				local space_num  = math.ceil((base_line_width - text_width) / 4)
				for  j = 1, space_num + 1 do 
					table.insert(text, i, " ")
				end
				count = count + space_num
				last_start_index = i + space_num
				row = row + 1
				ex_pos.x = fx
				ex_pos.y = -row * 22 - 7
				ex_pos.z = 0
			else
				fx = text_width - 16
				if(ex_num > 0) then
					ex_pos.x = fx - row * 8
				else
					ex_pos.x = fx
				end
				ex_pos.y = -row * 22 - 7
				ex_pos.z = 0
			end
			local express = Config.get_t_biaoqing(ex_id)
			if(express ~= nil) then
				local express_t = LuaHelper.Instantiate(express_res_.gameObject)
				express_t.transform:GetComponent("Collider").enabled = false
				express_t.transform.parent = text_label.transform
				express_t.transform.localPosition = ex_pos
				express_t.transform.localScale = Vector3.one
				express_t.transform:GetComponent('UISprite').spriteName = express.icon
				express_t:SetActive(true)
			end
		else
			local cur_width = NGUIText.CalculatePrintedWidth(text_label.trueTypeFont, text_label.fontSize, string.sub(str, last_start_index, i))
			if(cur_width > base_line_width) then
				last_start_index = i
				row = row + 1
			end
		end
	end
	text_label.text = str
	text_label:ProcessText()
	local height = text_label.transform:GetComponent('UIWidget').height
	local width = text_label.transform:GetComponent('UIWidget').width
	if(msg.player_guid == self.player.guid) then
		text_label.transform.localPosition = Vector3(21 - width, -28, 0)
	end
	chat_prefab.transform:Find('bg'):GetComponent('UIWidget').height = height + 10
	chat_prefab.transform:Find('bg'):GetComponent('UIWidget').width = width + 40
	height = height + 75
	chat_prefab.transform:GetComponent('Collider').size = Vector3(360, height + 75, 0)
	chat_prefab.transform:GetComponent('Collider').center = Vector3(collider_off_x, -75 - (height - 80) / 2, 0)
	if(pre_height == 0) then
		pre_height = height
		first_height = height
	end
	height = height + 20
	msg_prefab_height[chat_prefab] = pre_height
	-----------
	local uv = chat_view_:GetComponent('UIScrollView')
	local offy = uv.panel.clipOffset.y
	local pmaxh = maxh_
	maxh_ = maxh_ + pre_height
	local tpmaxh = pmaxh + offy
	local tmaxh = maxh_ + offy
	chat_prefab.transform.localPosition = Vector3(chat_msg_x_pos, -(maxh_ - first_height) + 130, 0)
	if tpmaxh <= viewh_ + 20 and tmaxh > viewh_ - 20 then
		--翻页
		if chat_view_:GetComponent('SpringPanel') ~= nil then
			chat_view_:GetComponent('SpringPanel').enabled = false
		end
		uv.panel.clipOffset = Vector2(0, viewh_ - maxh_- height + 86)
		chat_view_.localPosition = Vector3(226, maxh_ - viewh_ + height - 86, 0)
	end
	pre_height = height
	table.insert(show_msg_prefabs_, 1, chat_prefab)
	chat_prefab:SetActive(true)
	-----------
	if(#show_msg_prefabs_ > line_) then
		msg_prefab = show_msg_prefabs_[#show_msg_prefabs_]
		table.remove(show_msg_prefabs_, #show_msg_prefabs_)
		if(msg_prefab ~= nil) then
			GameObject.Destroy(msg_prefab)
		end
		minh_ = minh_ + msg_prefab_height[msg_prefab]
		local tminh = minh_ + offy
		if tminh >= 0 then
			if chat_view_:GetComponent('SpringPanel') ~= nil then
				chat_view_:GetComponent('SpringPanel').enabled = false
			end
			uv.panel.clipOffset = Vector2(0, -minh_)
			chat_view_.localPosition = Vector3(226, minh_, 0)
		end
	end
	-----------
end


function FriendPanel.SendMessage()
	if(chat_input_.value ~= '') then
		if(string.find(chat_input_.value, "\n")) then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('FriendPanel_018')})
		else
			local msg = msg_social_pb.cmsg_social_chat()
			msg.text = chat_input_.value
			if(string.len(chat_input_.value) > 60) then
				local str = str_sub(msg.text, 60)
				local str_tab = stringTotable(str)
				local pos = 0
				for i = #str_tab - 6, #str_tab do
					if(str_tab[i] == "[") then
						if(i == #str_tab) then
							table.remove(str_tab, i)
						elseif(str_tab[i + 1] == "#") then
							for k = 1, #str_tab - i + 1 do
								table.remove(str_tab, i)
							end
						end
					end
				end
				msg.text = tableTostring(str_tab)
			end
			msg.target_guid = cur_social_id_
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_SOCIAL_CHAT, data)
		end
		chat_input_.value = ''
	else
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('FriendPanel_019')})
	end
end

function FriendPanel.InitExpressPanel()
	local express_num = 0
	for k, v in pairsByKeys(Config.t_biaoqing) do
		express_num = express_num + 1
	end
	local express_width = express_res_:GetComponent('UIWidget').width
	if(express_num > 0) then
		local row = math.floor((express_num - 1) / 6 + 1)
		local height = (row - 1) * 50 + express_width + 18
		local width = 5 * 50 + express_width + 18
		express_root_:GetComponent('UIWidget').height = height
		express_root_:GetComponent('UIWidget').width = width
		local fir_pos_y = height - 9 - express_width / 2
		local fir_pos_x = express_width / 2 + 9
		local i = 0
		for k, v in pairsByKeys(Config.t_biaoqing) do
			local express = v
			local express_t = LuaHelper.Instantiate(express_res_)
			express_t.transform.parent = express_root_
			express_t.transform.localPosition = Vector3(i % 6 * 50 + fir_pos_x, -(math.floor(i / 6) * 50) + fir_pos_y, 0)
			express_t.transform.localScale = Vector3.one
			express_t.transform:GetComponent('UISprite').spriteName = express.icon
			express_t.name = express.id
			lua_script_:AddButtonEvent(express_t, "click", FriendPanel.SelectExpress)
			express_t:SetActive(true)
			i = i + 1
		end
	end
end

function FriendPanel.SelectExpress(obj)
	local express_id = tonumber(obj.name)
	local text_label = chat_input_.transform:Find("Label"):GetComponent("UILabel")
	local text_width = NGUIText.CalculatePrintedWidth(text_label.trueTypeFont, text_label.fontSize, string.sub(chat_input_.value, 1, -1))
	if(text_width <= chat_limit_ * 16 - 32) then
		chat_input_.value = chat_input_.value..'[#'..obj.name..']'
	end
	express_panel_.gameObject:SetActive(false)
end

function FriendPanel.OnDragFinished()
	local uv = chat_view_:GetComponent('UIScrollView')
	local constraint = uv.panel:CalculateConstrainOffset(uv.bounds.min, uv.bounds.max)
	local offy = uv.panel.clipOffset.y
	if(offy < viewh_ - maxh_ + 50) then
		unread_msg = 0
		unread_btn_.gameObject:SetActive(false)
	end
end


function FriendPanel.ShowTip()
	if(lua_script_ ~= nil) then
		apply_btn_:Find("tip").gameObject:SetActive(false)
		gift_btn_:Find("tip").gameObject:SetActive(false)
		friend_btn_:Find("tip").gameObject:SetActive(false)
		if(self.apply_num > 0) then
			apply_btn_:Find("tip").gameObject:SetActive(true)
		end
		if(self.social_gold) then
			gift_btn_:Find("tip").gameObject:SetActive(true)
		end
		if(self.unread_msg_num > 0) then
			friend_btn_:Find("tip/Label"):GetComponent("UILabel").text = self.unread_msg_num
			friend_btn_:Find("tip").gameObject:SetActive(true)
		end
	end
end
-------------------------------------------------------

---------------------ButtonEvent-------------------------

function FriendPanel.Click(obj)
	if(obj.name == "friend_btn") then
		cur_social_id_ = 0
		FriendPanel.SocialLook(2)
	elseif(obj.name == "black_btn") then
		cur_social_id_ = 0
		FriendPanel.SelectPage(1)
	elseif(obj.name == "friend_del") then
		FriendPanel.DelSocial(0)
	elseif(obj.name == "black_del") then
		FriendPanel.DelSocial(1)
	elseif(obj.name == "tuijian_btn") then
		GameTcp.Send(opcodes.CMSG_SOCIAL_TUIJIAN, nil, {opcodes.SMSG_SOCIAL_TUIJIAN})
	elseif(obj.name == "change_btn") then
		GameTcp.Send(opcodes.CMSG_SOCIAL_TUIJIAN, nil, {opcodes.SMSG_SOCIAL_TUIJIAN})
	elseif(obj.name == "search_btn") then
		search_panel_.gameObject:SetActive(true)
	elseif(obj.name == "gift_btn") then
		FriendPanel.ClosePanel()
		FriendPanel.InitGiftList()
	elseif(obj.name == "get_gift_btn") then
		FriendPanel.GetAllGift()
	elseif(obj.name == "apply_btn") then
		--FriendPanel.InitApplyList(friend_list_)
		FriendPanel.SocialLook(1)
	elseif(obj.name == "clear_btn") then
		if(#black_list_ > 0) then
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('FriendPanel_020'),Config.get_t_script_str('FriendPanel_010'), FriendPanel.ClearBlack, Config.get_t_script_str('FriendPanel_011')})
		else
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('FriendPanel_021'),Config.get_t_script_str('FriendPanel_010')})
		end
	elseif(obj.name == "close_btn") then
		FriendPanel.ClosePanel()
	elseif(obj.name == "send_btn") then
		FriendPanel.SendMessage()
	elseif(obj.name == "express_btn") then
		if(express_panel_.gameObject.activeInHierarchy) then
			express_panel_.gameObject:SetActive(false)
		else
			express_panel_.gameObject:SetActive(true)
		end
	elseif(obj.name == "unread_btn") then
		FriendPanel.CheckLatestMsg()
	elseif(obj.name == "apply_flag") then
		GameTcp.Send(opcodes.CMSG_SOCIAL_APPLY_REJECT, nil, {opcodes.SMSG_SOCIAL_APPLY_FLAG})
	elseif(obj.name == "search_begin") then
		FriendPanel.BeginSearch()
	end
end

function FriendPanel.BeginSearch()
	if(search_input_.value ~= '') then
		local player_name = search_input_.value
		local is_ill = IsMatch(player_name, "^[A-Za-z0-9@_\\u4e00-\\u9fa5]+$")
		if(string.len(player_name) > 24 or string.len(player_name) < 4) then
			GUIRoot.ShowGUI('MessagePanel', {Config.get_t_script_str('FriendPanel_022')})
		elseif(not is_ill) then
			GUIRoot.ShowGUI('MessagePanel', {Config.get_t_script_str('FriendPanel_023')})
		else
			local msg = msg_social_pb.cmsg_soical_search()
			msg.name = search_input_.value
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_SOCIAL_SEARCH, data, {opcodes.SMSG_SOCIAL_SEARCH})
		end
	else
		GUIRoot.ShowGUI('MessagePanel', {Config.get_t_script_str('FriendPanel_024')})
	end
end

function FriendPanel.InitApplyFlag()
	if(not self.reject) then
		apply_flag_:Find("tip").gameObject:SetActive(false)
	else
		apply_flag_:Find("tip").gameObject:SetActive(true)
	end
end

function FriendPanel.GetAllGift()
	FriendPanel.CountGoldNum()
	if(self.social_gold) then
		GameTcp.Send(opcodes.CMSG_SOCIAL_GIFT_GET, nil, {opcodes.SMSG_SOCIAL_GIFT_GET})
	end
end

function FriendPanel.ReceiveGift(msg)
	if(lua_script_ ~= nil) then
		local social = FriendPanel.GetSocialGuid(friend_list_, msg.target_guid)
		if(social ~= nil) then
			social.gold = msg.gold
		end
	end
end

function FriendPanel.OpenZonePanel(obj)
	local guid = obj.name
	if(self.player.guid == guid) then
		GUIRoot.HideGUI("FriendPanel")
		GUIRoot.ShowGUI("ZonePanel", {self.player, self.battle_results})
	else
		local msg = msg_hall_pb.cmsg_player_look()
		msg.target_guid = guid
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_PLAYER_LOOK, data, {opcodes.SMSG_PLAYER_LOOK})
	end
end

function FriendPanel.GetSocialGuid(social_list, target_guid)
	for i = 1, #social_list do
		if(social_list[i].target_guid == target_guid) then
			return social_list[i]
		end
	end
	return nil
end

function FriendPanel.RemoveSocialGuid(social_list, target_guid)
	for i = 1, #social_list do
		if(social_list[i].target_guid == target_guid) then
			table.remove(social_list, i)
			break
		end
	end
end

function FriendPanel.RemoveBlack(target_guid)
	if(lua_script_ ~= nil) then
		FriendPanel.RemoveSocialGuid(black_list_, target_guid)
		if(cur_page_ == 1) then
			FriendPanel.SelectPage(1)
		end
	end
end

function FriendPanel.AddSocial(social)
	if(#self.friend_guids >= 50) then
		return 0
	end
	if(lua_script_ ~= nil) then
		FriendPanel.RemoveSocialGuid(black_list_, social.target_guid)
		for i = 1, #friend_list_ do
			if(friend_list_[i].guid == social.guid) then
				return 0
			end
		end
		unread_list_[social.target_guid] = 0
		table.insert(friend_list_, social)
		friend_list_ = FriendPanel.RankSocial(friend_list_)
		FriendPanel.SelectPage(cur_page_)
	end
end

function FriendPanel.AddBlack(social)
	if(self.social_type(social.target_guid) == 2) then
		self.unread_msg_num = self.unread_msg_num - FriendPanel.GetUnreadNum(social.target_guid)
		unread_list_[social.target_guid] = 0
	end
	self.del_social_guid(2, social.target_guid)
	self.add_social_guid(3, social.target_guid)
	if(lua_script_ ~= nil) then
		FriendPanel.RemoveSocialGuid(friend_list_, social.target_guid)
		for i = 1, #black_list_ do
			if(black_list_[i].guid == social.guid) then
				return 0
			end
		end
		table.insert(black_list_, social)
		black_list_ = FriendPanel.RankSocial(black_list_)
		FriendPanel.SelectPage(cur_page_)
		FriendPanel.CountOnlineNum()
	end
end

function FriendPanel.InitChatMsg()
	for i = 1, #friend_list_ do
		FriendPanel.AddUnreadMsg(friend_list_[i].target_guid, #friend_list_[i].msgs)
		self.unread_msg_num = self.unread_msg_num + #friend_list_[i].msgs
		if(#friend_list_[i].msgs > 0) then
			for j = 1, #friend_list_[i].msgs do
				local msg = {}
				msg.time = friend_list_[i].msgtimes[j]
				msg.text = friend_list_[i].msgs[j]
				msg.player_name = friend_list_[i].name
				msg.player_guid = friend_list_[i].target_guid
				msg.sex = friend_list_[i].sex
				msg.level = friend_list_[i].level
				msg.avatar = friend_list_[i].avatar
				msg.toukuang = friend_list_[i].toukuang
				msg.name_color = NoticePanel.GetVipNameColor(friend_list_[i])
				NoticePanel.AddSocialMsg(msg.player_guid, msg)
			end
		end
		friend_list_[i].msgs:clear()
		friend_list_[i].msgtimes:clear()
		NoticePanel.RankSocialMsg(friend_list_[i].target_guid)
	end
end

function FriendPanel.AddUnreadMsg(target_guid, num)
	if unread_list_[target_guid] == nil then
		unread_list_[target_guid] = 0
	end
	unread_list_[target_guid] = unread_list_[target_guid] + num
	
	if(unread_list_[target_guid] > 30) then
		self.unread_msg_num = self.unread_msg_num - unread_list_[target_guid] + 30
		unread_list_[target_guid] = 30
	end
end

function FriendPanel.CountUnreadNum()
	self.unread_msg_num = 0
	for k, v in pairs(unread_list_) do
		self.unread_msg_num = self.unread_msg_num + v
	end
	PlayerPrefs.SetString(self.player.guid.."_total_unread_num", self.unread_msg_num)
	PlayerPrefs.Save()
end

function FriendPanel.GetUnreadNum(target_guid)
	if unread_list_[target_guid] == nil then
		unread_list_[target_guid] = 0
	end
	return unread_list_[target_guid]
end

function FriendPanel.ClearUnreadList()
	unread_list_ = {}
end

function FriendPanel.CountGoldNum()
	local total_gold = 0
	for i = 1, #friend_list_ do
		if(friend_list_[i].gold > 0) then
			total_gold = total_gold + friend_list_[i].gold
		end
	end
	if(total_gold > 0) then
		self.social_gold = true
	else
		self.social_gold = false
	end
end

function FriendPanel.CountOnlineNum()
	local num = 0
	for i = 1, #friend_list_ do
		if luabit.band(friend_list_[i].sflag, 2) ~= 0 then
			num = num + 1
		end
	end
	local online_num = friend_panel_:Find("num"):GetComponent("UILabel")
	online_num.text = Config.get_t_script_str('FriendPanel_025').." "..num.." / "..#friend_list_
end
----------------------------------------------------------


------------------服务器消息------------------------------

function FriendPanel.SMSG_SOCIAL_ADD(msg)
	if(lua_script_ ~= nil) then
		FriendPanel.AddSocial(msg.social)
		if(cur_apply_ ~= nil) then
			self.apply_num = self.apply_num - 1
			if(self.apply_num < 0) then
				self.apply_num = 0
			end
			cur_apply_:Find("ok").gameObject:SetActive(false)
			cur_apply_:Find("no").gameObject:SetActive(false)
			cur_apply_:Find("tip").gameObject:SetActive(true)
			FriendPanel.ShowTip()
		end
		FriendPanel.CountOnlineNum()
	end
end

function FriendPanel.SMSG_SOCAIL_LOOK(message)
	local msg = msg_social_pb.smsg_social_look()
	msg:ParseFromString(message.luabuff)
	if(look_type_ == 3) then
		black_list_ = msg.socials
		black_list_ = FriendPanel.RankSocial(black_list_)
		FriendPanel.SelectPage(1)
	elseif(look_type_ == 1) then
		apply_list_ = msg.socials
		FriendPanel.InitApplyList(msg.socials)
	elseif look_type_ == 2 then
		friend_list_ = msg.socials
		FriendPanel.SelectPage(0)
	end
end

function FriendPanel.SMSG_SOCIAL_GIFT()
	if(cur_gift_social_ ~= nil) then
		self.player.social_golds:append(cur_gift_social_.name)
		cur_gift_social_:Find("gold_btn").gameObject:SetActive(false)
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('FriendPanel_026')})
	end
end

function FriendPanel.SMSG_SOCIAL_CHAT(msg)
	if(lua_script_ ~= nil) then
		if(msg.player_guid == cur_social_id_ or msg.player_guid == self.player.guid) then
			FriendPanel.NewMesssage(msg)
			if(msg.player_guid == self.player.guid) then
				NoticePanel.AddSocialMsg(cur_social_id_, msg)
				FriendPanel.CheckLatestMsg()
			else
				local uv = chat_view_:GetComponent('UIScrollView')
				local offy = uv.panel.clipOffset.y
				if(offy >= (viewh_ - maxh_) - pre_height + 80 + pre_height / 3) then
					unread_msg = unread_msg + 1
					local content = string.format(Config.get_t_script_str('FriendPanel_027'),tostring(unread_msg))
					unread_btn_:Find('Label'):GetComponent('UILabel').text = content --'您有'..tostring(unread_msg)..'条未读信息！'
					unread_btn_.gameObject:SetActive(true)
				end
			end
		else
			FriendPanel.AddUnreadMsg(msg.player_guid, 1)
			self.unread_msg_num = self.unread_msg_num + 1
			if(cur_page_ == 0) then
				social_view_:Find(msg.player_guid):Find("tip").gameObject:SetActive(true)
				social_view_:Find(msg.player_guid):Find("tip/Label"):GetComponent("UILabel").text = unread_list_[msg.player_guid]
			end
			FriendPanel.ShowTip()
		end
	else
		FriendPanel.AddUnreadMsg(msg.player_guid, 1)
		self.unread_msg_num = self.unread_msg_num + 1
	end
end

function FriendPanel.SMSG_SOCIAL_GIFT_GET()
	FriendPanel.ClosePanel()
	local total_gold = 0
	for i = 1, #friend_list_ do
		if(friend_list_[i].gold > 0) then
			total_gold = total_gold + friend_list_[i].gold
			friend_list_[i].gold = 0
		end
	end
	if(total_gold > 0) then
		self.add_resource(1, total_gold)
		local reward = {}
		reward.type = 1
		reward.value1 = 1
		reward.value2 = total_gold
		reward.value3 = 0
		GUIRoot.ShowGUI("GainPanel", {{reward}})
	end
	self.social_gold = false
	FriendPanel.ShowTip()
end

function FriendPanel.SMSG_SOCIAL_DELETE(msg)
	if(lua_script_ ~= nil) then
		if(msg.target_guid == "0") then
			black_list_ = {}
			self.black_guids = {}
			FriendPanel.SelectPage(1)
		else
			if(self.social_type(msg.target_guid) == 3) then
				self.del_social_guid(3, msg.target_guid)
				FriendPanel.RemoveSocialGuid(black_list_, msg.target_guid)
				black_list_ = FriendPanel.RankSocial(black_list_)
				FriendPanel.SelectPage(1)
			elseif(msg.target_guid == self.player.guid and cur_apply_ ~= nil) then
				cur_apply_:Find("ok").gameObject:SetActive(false)
				cur_apply_:Find("no").gameObject:SetActive(false)
				cur_apply_:Find("tip").gameObject:SetActive(true)
			elseif(FriendPanel.GetSocialGuid(friend_list_, msg.target_guid) ~= nil) then
				FriendPanel.RemoveSocialGuid(friend_list_, msg.target_guid)
				friend_list_ = FriendPanel.RankSocial(friend_list_)
				FriendPanel.CountGoldNum()
				if(gift_panel_.gameObject.activeInHierarchy) then
					FriendPanel.InitGiftList()
				end
				if(cur_page_ == 0) then
					FriendPanel.SelectPage(0)
				end
			end
		end
		FriendPanel.CountOnlineNum()
		FriendPanel.ShowTip()
	end
end

function FriendPanel.SMSG_PLAYER_LOOK(message)
	local msg = msg_hall_pb.smsg_player_look()
	msg:ParseFromString(message.luabuff)
	GUIRoot.ShowGUI("ZonePanel", {msg.player, msg.battle_his, 1})
	GUIRoot.HideGUI("FriendPanel", false)
end

function FriendPanel.SMSG_SOCIAL_APPLY_FLAG()
	if(self.reject) then
		self.reject = false
	else
		self.reject = true
	end
	FriendPanel.InitApplyFlag()
end

function FriendPanel.SMSG_SOCIAL_STAT(msg)
	if(lua_script_ ~= nil) then
		if(self.social_type(msg.target_guid) == 2) then
			local social = FriendPanel.GetSocialGuid(friend_list_, msg.target_guid)
			social.sflag = msg.stat
			friend_list_ = FriendPanel.RankSocial(friend_list_)
			if(cur_page_ == 0) then
				FriendPanel.SelectPage(0)
			end
			FriendPanel.CountOnlineNum()
		elseif(self.social_type(msg.target_guid) == 3) then
			if(#black_list_ > 0) then
				local social = FriendPanel.GetSocialGuid(black_list_, msg.target_guid)
				social.sflag = msg.stat
				black_list_ = FriendPanel.RankSocial(black_list_)
				if(cur_page_ == 1) then
					FriendPanel.SelectPage(1)
				end
			end
		end
	end
end

function FriendPanel.SMSG_SOCIAL_SEARCH(message)
	local msg = msg_social_pb.smsg_social_look()
	msg:ParseFromString(message.luabuff)
	search_input_.value = ""
	FriendPanel.InitSearchList(1, msg.socials)
end

function FriendPanel.SMSG_SOCIAL_TUIJIAN(message)
	local msg = msg_social_pb.smsg_social_look()
	msg:ParseFromString(message.luabuff)
	FriendPanel.InitSearchList(2, msg.socials)
end
-----------------------------------------------------------