HallPanel = {}

local lua_script_

local horn_panel_
local item_panel_
local mode_panel_
local mode_detail_
local vip_panel_
local first_panel_

local icon_get_list_ = {0, 0, 0, 0}
local get_time_ = {-1, -1, -1, -1}
local icon_get_res_

local chat_msg_res_
local express_res_
local chat_view_
local chat_msg_pos = Vector3(30, 100, 0)
local msg_prefab_height = {}
local show_msg_prefabs_ = {}
local minh_ = 0
local maxh_ = 0
local viewh_ = 140
local line_ = 10

local chest_btn_

local role_btn
local achiv_btn
local mail_btn_
local dress_btn_
local share_btn_
local friend_btn_
local vip_btn_
local duobao_btn_
local first_btn_
local rewardeview_btn_

local top_right_btns_ = {}

local item_view_
local view_softness = Vector2(0, 10)
local item_pos = Vector3(-97, 155, 0)
local item_view_pos_ = Vector3(0, 0, 0)
local item_space_x_ = 100
local item_space_y_ = 100
local battle_item_btn_
local pre_select_item = nil
local item_select_id = 0

local time_speed = 0.1
local space = '        '

local mode_x_space = 0
local mode_x_pos = 0
local mode_width = 316

local light_color_ = {'light-blue', 'light-purple', 'light-yellow', "[76B1F7]"}

local gonggao_time = 0
local friend_request = 0
local reward_id_ = 0

local m_role_camera_

HallPanel.tip_battle_message = nil
HallPanel.tip_battle_flag = false

function HallPanel.Awake(obj)
	GUIRoot.ShowGUI("TopPanel")
	HallScene.AddRole()
	HallScene.AddFashion()
	mapMgr:SetTargetCam(0, 0.6, 3)
	
	if(GameObject.Find("Camera_UIRole") ~= nil) then
		m_role_camera_ = GameObject.Find("Camera_UIRole").transform:GetComponent("Camera")
		m_role_camera_.fieldOfView = GUIRoot.height / 640 * 47
	end
	
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	item_panel_ = obj.transform:Find('item_panel')
	mode_panel_ = obj.transform:Find('mode_panel')
	mode_detail_ = obj.transform:Find('mode_detail')
	vip_panel_ = obj.transform:Find('vip_panel')
	first_panel_ = obj.transform:Find('first_panel')
	
	mode_panel_:Find("main_panel/mode_view"):GetComponent("UIPanel"):SetRect(0, 0, GUIRoot.width, 500)
	
	icon_get_res_ = obj.transform:Find('show_panel/icon_get')
	
	item_res_ = item_panel_:Find('main_panel/item_res')
	item_view_ = item_panel_:Find('main_panel/item_view')
	item_view_:GetComponent("UIPanel").clipSoftness = view_softness
	chat_view_ = obj.transform:Find('Anchor_bottom_left/chat_panel/chat_view')
	chat_msg_res_ = obj.transform:Find('Anchor_bottom_left/chat_panel/chat_msg_res')
	express_res_ = obj.transform:Find('Anchor_bottom_left/chat_panel/express')
	
	item_view_pos_ = item_view_.transform.localPosition

	chest_btn_ = obj.transform:Find('Anchor_top_right/top_right_panel/chest_btn')
	--table.insert(top_right_btns_, chest_btn_)
	role_btn = obj.transform:Find('Anchor_bottom_left/chat_panel/role_btn')
	local item_btn = obj.transform:Find('Anchor_bottom_left/chat_panel/item_btn')
	achiv_btn = obj.transform:Find('Anchor_bottom_left/chat_panel/achiv_btn')
	dress_btn_ = obj.transform:Find('Anchor_bottom_left/chat_panel/dress_btn')
	local chat_btn = obj.transform:Find('Anchor_bottom_left/chat_panel/chat_btn')
	local online_btn = obj.transform:Find('Anchor_bottom_right/right_bottom_panel/online_btn')
	local shop_btn = obj.transform:Find('Anchor_bottom_right/right_bottom_panel/shop_btn')
	mail_btn_ = obj.transform:Find('Anchor_left/left_panel/mail_btn')
	share_btn_ = obj.transform:Find('Anchor_top_right/top_right_panel/share_btn')
	--table.insert(top_right_btns_, share_btn_)
	local set_btn = obj.transform:Find('Anchor_left/left_panel/set_btn')
	friend_btn_ = obj.transform:Find('Anchor_left/left_panel/friend_btn')
	local close_item_btn = item_panel_:Find('main_panel/close_btn')
	local mode_detail_close = mode_detail_:Find("mode_detail_close").gameObject
	local vip_close_btn = vip_panel_:Find("main_panel/Anchor/vip_close_btn").gameObject
	local vip_y_btn = vip_panel_:Find("main_panel/vip_y_btn").gameObject
	local vip_m_btn = vip_panel_:Find("main_panel/vip_m_btn").gameObject
	first_btn_ = obj.transform:Find('Anchor_top_right/top_right_panel/first_btn')
	if(platform_config_common.m_platform == "android_yymoon") then
		first_btn_.gameObject:SetActive(false);
	end
	vip_btn_ = obj.transform:Find('Anchor_top_right/top_right_panel/vip_btn')
	if(platform_config_common.m_platform == "android_yymoon") then
		vip_btn_.gameObject:SetActive(false);
	end
	rewardeview_btn_ = obj.transform:Find('Anchor_top_right/top_right_panel/rewarde_vedio')
	duobao_btn_ = obj.transform:Find('Anchor_top_right/top_right_panel/duobao_btn')
	--table.insert(top_right_btns_, first_btn_)
	--table.insert(top_right_btns_, vip_btn_)
	--table.insert(top_right_btns_, duobao_btn_)
	--table.insert(top_right_btns_, rewardeview_btn_)
	local first_recharg_btn = first_panel_:Find("main_panel/first_recharge_btn").gameObject
	local first_close_btn = first_panel_:Find("main_panel/first_close_btn").gameObject
	
	battle_item_btn_ = obj.transform:Find('Anchor_bottom_left/chat_panel/battle_item_btn')
	local rank_btn = obj.transform:Find('Anchor_bottom_left/chat_panel/rank_btn')
	
	if self.is_ios_sh then
		rank_btn.gameObject:SetActive(false)
		battle_item_btn_.localPosition = Vector3(424.3,20,0)
	else	
		battle_item_btn_.localPosition = Vector3(534.6,20,0)
		rank_btn.localPosition = Vector3(424.3,20,0)
		rank_btn.gameObject:SetActive(true)
	end
	
	
	local mode_close_btn = mode_panel_:Find("main_panel/Anchor_top_left/mode_close_btn").gameObject
	
	lua_script_:AddButtonEvent(role_btn.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(item_btn.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(dress_btn_.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(chat_btn.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(shop_btn.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonSoundEvent(online_btn.gameObject, "click", HallPanel.Click, "battle_start")
	lua_script_:AddButtonEvent(achiv_btn.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(share_btn_.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(mail_btn_.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(set_btn.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(friend_btn_.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(battle_item_btn_.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(rank_btn.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(close_item_btn.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(chest_btn_.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(mode_close_btn.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(mode_detail_close, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(vip_close_btn, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(vip_y_btn, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(vip_m_btn, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(vip_btn_.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(first_btn_.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(first_recharg_btn, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(first_close_btn, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(duobao_btn_.gameObject, "click", HallPanel.Click)
	lua_script_:AddButtonEvent(rewardeview_btn_.gameObject, "click", HallPanel.Click)
	
	item_panel_.gameObject:SetActive(false)
	mode_panel_.gameObject:SetActive(false)
	mode_detail_.gameObject:SetActive(false)
	vip_panel_.gameObject:SetActive(false)
	first_panel_.gameObject:SetActive(false)
	
	timerMgr:AddRepeatTimer('HallPanel', HallPanel.Refresh, time_speed, time_speed)
	HallPanel.RegisterMessage()
	lua_script_:AddEvent("OnDrag")
	lua_script_:AddEvent("OnClick")
	HallPanel.RefreshChest()
	HallPanel.InitChatMsg()
	HallPanel.CheckConnect()
	HallPanel.ShowGongGao()
	HallPanel.ShowTip()
	HallPanel.CountModeSpace()
	HallPanel.CheckIOSPurchase()
end

local ios_purchase_ = nil

function HallPanel.CheckIOSPurchase()
	if platform_config_common.m_platform == "ios_yymoon" then
		ios_purchase_ = nil
		for k,v in pairs(self.ios_purchase) do
			ios_purchase_ = k
			local msg = msg_hall_pb.cmsg_recharge()
			msg.id = v[1]
			msg.code = v[2]
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_RECHARGE, data, {opcodes.SMSG_RECHARGE},Config.get_t_script_str('HallPanel_001'), 60)
			return
		end
	end
end

function HallPanel.SMSG_RECHARGE(message)
	if platform_config_common.m_platform ~= "ios_yymoon" then
		return
	end
	
	local msg = msg_hall_pb.smsg_recharge()
	msg:ParseFromString(message.luabuff)

	GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('HallPanel_002')})
	if(self.player.first_recharge == 0) then
		self.player.first_recharge = 1
	end
	local recharge_temp = Config.get_t_recharge(recharge_id_)
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
	if(#msg.roles > 0) then
		GUIRoot.ShowGUI("RoleGetPanel", {msg.roles, {"ShopPanel", "BackPanel"}, rewards})
	else
		if(#rewards > 0) then
			GUIRoot.ShowGUI('GainPanel', {rewards})
		end
	end
	self.player.total_recharge = self.player.total_recharge + recharge_temp.value
	
	self.ios_purchase[ios_purchase_] = nil
	self.update_ios_purchase()
	
	if #self.ios_purchase > 0 then
		HallPanel.CheckIOSPurchase()
	end
end

function HallPanel.ERROR_RECHARGE()
	if platform_config_common.m_platform ~= "ios_yymoon" then
		return
	end
	
	self.ios_purchase[ios_purchase_] = nil
	self.update_ios_purchase()
	
	if #self.ios_purchase > 0 then
		HallPanel.CheckIOSPurchase()
	end
end

function HallPanel.RegisterMessage()
	Message.register_net_handle(opcodes.ERROR_RECHARGE, HallPanel.ERROR_RECHARGE)
	Message.register_net_handle(opcodes.SMSG_RECHARGE, HallPanel.SMSG_RECHARGE)
	Message.register_net_handle(opcodes.SMSG_SINGLE_BATTLE, HallPanel.SMSG_SINGLE_BATTLE)
	Message.register_net_handle(opcodes.SMSG_POST_LOOK, HallPanel.SMSG_POST_LOOK)
	Message.register_net_handle(opcodes.SMSG_TEAM_CREATE, HallPanel.SMSG_TEAM_CREATE)
	Message.register_net_handle(opcodes.SMSG_SOCAIL_LOOK, HallPanel.SMSG_SOCAIL_LOOK)
	Message.register_net_handle(opcodes.SMSG_TEAM_TUIJIAN, HallPanel.SMSG_TEAM_TUIJIAN)
	Message.register_net_handle(opcodes.SMSG_VIP_REWARD, HallPanel.SMSG_VIP_REWARD)
	Message.register_net_handle(opcodes.SMSG_ADVERTISEMENT, HallPanel.SMSG_ADVERTISEMENT)
	Message.register_handle("team_join_msg", HallPanel.TeamJoin)
end

function HallPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.ERROR_RECHARGE, HallPanel.ERROR_RECHARGE)
	Message.remove_net_handle(opcodes.SMSG_RECHARGE, HallPanel.SMSG_RECHARGE)
	Message.remove_net_handle(opcodes.SMSG_SINGLE_BATTLE, HallPanel.SMSG_SINGLE_BATTLE)
	Message.remove_net_handle(opcodes.SMSG_POST_LOOK, HallPanel.SMSG_POST_LOOK)
	Message.remove_net_handle(opcodes.SMSG_TEAM_CREATE, HallPanel.SMSG_TEAM_CREATE)
	Message.remove_net_handle(opcodes.SMSG_SOCAIL_LOOK, HallPanel.SMSG_SOCAIL_LOOK)
	Message.remove_net_handle(opcodes.SMSG_TEAM_TUIJIAN, HallPanel.SMSG_TEAM_TUIJIAN)
	Message.remove_net_handle(opcodes.SMSG_VIP_REWARD, HallPanel.SMSG_VIP_REWARD)
	Message.remove_net_handle(opcodes.SMSG_ADVERTISEMENT, HallPanel.SMSG_ADVERTISEMENT)
	Message.remove_handle("team_join_msg", HallPanel.TeamJoin)
end

function HallPanel.OnDestroy()
	HallScene.RemoveFashion()
	horn_msgs_ = {}
	horn_is_rolling = false
	horn_roll_time = 0
	item_select_id = 0
	reward_id_ = 0
	--get_time_ = {-1, -1, -1, -1}
	icon_get_list_ = {0, 0, 0, 0}
	mode_page_ = 0
	pre_select_item = nil
	lua_script_ = nil
	HallPanel.RemoveMessage()
	timerMgr:RemoveRepeatTimer('HallPanel')
	GUIRoot.HideGUI("TopPanel")
end

function HallPanel.TeamJoin()
	GUIRoot.HideGUI('HallPanel')
end

-----------------宝箱---------------------

function HallPanel.Update()
	for i = 1, #get_time_ do
		if(get_time_[i] ~= -1) then
			if(icon_get_list_[i] == 0) then
				icon_get_list_[i] = LuaHelper.Instantiate(icon_get_res_.gameObject)
				icon_get_list_[i].transform.parent = icon_get_res_.parent
				icon_get_list_[i].transform.localPosition = Vector3(0, 0, 0)
				icon_get_list_[i].transform.localScale = Vector3(1, 1, 1)
			end
			local icon_p = icon_get_list_[i]
			if(get_time_[i] == 0) then
				lua_script_:PlaySound("get_chest1")
				local chest_temp = Config.get_t_chest(self.player.box_ids[i])
				local icon = icon_get_list_[i].transform:Find("icon")
				icon:GetComponent("UISprite").atlas = IconPanel.GetAltas(chest_temp.icon)
				icon:GetComponent("UISprite").spriteName = chest_temp.icon
				icon:GetComponent("UISprite"):MakePixelPerfect()
				icon_p.transform.localScale = Vector3(0.4, 0.4, 1)
				icon_get_list_[i].transform.localPosition = Vector3(0, 0, 0)
				local effect = icon:Find("light/effect/effect"):GetComponent("UISprite")
				effect.spriteName = light_color_[chest_temp.id]
				icon:GetComponent("Animator"):Play("chest_get")
				local x_pos = GUIRoot.width / 2
				local y_pos = GUIRoot.height / 2
				local to = Vector3(x_pos, y_pos, 0) - Vector3(40, 135, 0)
				twnMgr:Add_Tween_Postion(icon_p, 0.6, icon_p.transform.localPosition, to, 2, 1.8)
				icon_p:SetActive(true)
			end
			get_time_[i] = get_time_[i] + time_speed
			if(get_time_[i] >= 2) then
				get_time_[i] = -1
				icon_p:SetActive(false)
				chest_btn_:GetComponent('Animator'):Play('get_end')
			end
		end
	end
	if HallPanel.tip_battle_message ~= nil then
		HallPanel.SMSG_HAS_BATTLE()
	end
end

function HallPanel.GetChest(slot)
	get_time_[slot] = 0
end

function HallPanel.RefreshChest()
	local tip = chest_btn_:Find("tip").gameObject
	tip:SetActive(false)
	if(self.player.box_open_slot == 0) then
		local id = 0
		for i = 1, #self.player.box_ids do
			if(self.player.box_ids[i] ~= 0) then
				id = self.player.box_ids[i]
			end
		end
		if(id > 0) then
			tip:SetActive(true)
		end
	else
		if(tonumber(timerMgr:now_string()) >= tonumber(self.player.box_open_time)) then
			tip:SetActive(true)
		end
	end
	local batt_num = Config.get_t_chest(201).time
	if(self.player.box_zd_opened == 0 and self.player.box_zd_num == batt_num) then
		tip:SetActive(true)
	end
end

----------------------------------------
function HallPanel.UpdateVideo()
	if ((self.player.advertisement_num < 5) and (tonumber(self.player.advertisement_time) + 300000) <= tonumber(timerMgr:now_string())) then
		rewardeview_btn_:Find("tip").gameObject:SetActive(true)
		rewardeview_btn_.gameObject:SetActive(true)
	else
		rewardeview_btn_:Find("tip").gameObject:SetActive(false)
		rewardeview_btn_.gameObject:SetActive(false)
	end

end
----------------------------------------


--------初始化主界面---

function HallPanel.InitChatMsg()
	local chat_msg = NoticePanel.GetMsg()
	msg_prefab_height = {}
	show_msg_prefabs_ = {}
	minh_ = 0
	maxh_ = 0
	if(chat_view_.childCount > 0) then
			for i = 0, chat_view_.childCount - 1 do
				GameObject.Destroy(chat_view_:GetChild(i).gameObject)
			end
		end
	if chat_view_:GetComponent('SpringPanel') ~= nil then
		chat_view_:GetComponent('SpringPanel').enabled = false
	end
	local uv = chat_view_:GetComponent('UIScrollView')
	uv.panel.clipOffset = Vector2(0, 0)
	chat_view_.localPosition = Vector3(0, 76, 0)
	if(#chat_msg >= line_) then
		for i = 1, line_ do
			local msg = chat_msg[#chat_msg - line_ + i]
			if msg.type_id == 1 then
				HallPanel.NewMesssage(msg.data)
			elseif msg.type_id == 2 then
				HallPanel.NewSysMessage(msg.data)
			end
		end
	else
		for i = 1, #chat_msg do
			local msg = chat_msg[i]
			if msg.type_id == 1 then
				HallPanel.NewMesssage(msg.data)
			elseif msg.type_id == 2 then
				HallPanel.NewSysMessage(msg.data)
			end
		end
	end
end

function HallPanel.NewSysMessage(msg)
	local chat_prefab = nil
	chat_prefab = LuaHelper.Instantiate(chat_msg_res_.gameObject)
	chat_prefab.transform.parent = chat_view_
	chat_prefab.transform.localScale = Vector3.one
	
	local tip = chat_prefab.transform:Find('tip').gameObject
	local region = chat_prefab.transform:Find('region').gameObject
	
	if tip.activeSelf then
		tip:SetActive(false)
	end
	if region.activeSelf then
		region:SetActive(false)
	end
	
	chat_prefab.transform:Find('title'):GetComponent('UILabel').text = Config.get_t_script_str('HallPanel_003')--'系统'
	local name = chat_prefab.transform:Find('name'):GetComponent('UILabel')
	name.text = msg.text
	name.transform.localPosition = Vector3(30,0,0)
	
	local text_label = chat_prefab.transform:Find('text'):GetComponent('UILabel')
	text_label.text = ''
	local name_width = name.transform:GetComponent('UIWidget').width + 10
	text_label.transform.localPosition = Vector3(name.transform.localPosition.x + name_width, 7, 0)
	
	local height = text_label.transform:GetComponent('UIWidget').height
	local width = text_label.transform:GetComponent('UIWidget').width
	height = height + 20
	msg_prefab_height[chat_prefab] = height
	chat_prefab.transform.localPosition = Vector3(0, -maxh_, 0) + chat_msg_pos
	pre_height = height
	-----------
	local uv = chat_view_:GetComponent('UIScrollView')
	local offy = uv.panel.clipOffset.y
	local pmaxh = maxh_
	maxh_ = maxh_ + pre_height
	local tpmaxh = pmaxh + offy
	local tmaxh = maxh_ + offy

	if tpmaxh <= viewh_ + 16 and tmaxh > viewh_ then
		if chat_view_:GetComponent('SpringPanel') ~= nil then
			chat_view_:GetComponent('SpringPanel').enabled = false
		end
		uv.panel.clipOffset = Vector2(0, viewh_ - maxh_)
		chat_view_.localPosition = Vector3(0, maxh_ - viewh_ + 76, 0)
	end
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
			chat_view_.localPosition = Vector3(0, minh_ + 76, 0)
		end
	end
end

function HallPanel.NewMesssage(msg)
	local chat_prefab = nil
	chat_prefab = LuaHelper.Instantiate(chat_msg_res_.gameObject)
	chat_prefab.transform.parent = chat_view_
	chat_prefab.transform.localScale = Vector3.one
	local tip = chat_prefab.transform:Find('tip')
	local name = chat_prefab.transform:Find('name'):GetComponent('UILabel')
	local region = chat_prefab.transform:Find('region'):GetComponent('UISprite')
	
	chat_prefab.transform:Find('title'):GetComponent('UILabel').text = Config.get_t_script_str('HallPanel_004') --'频道'
	name.text = msg.player_name..":"
	region.spriteName = Config.get_t_foregion(msg.region_id).icon
	
	IconPanel.InitVipLabel(name, msg.name_color)
	if(msg.type == 1) then
		tip.gameObject:SetActive(true)
		region.transform.localPosition = Vector3(61, -1, 0)
		name.transform.localPosition = Vector3(101, -1, 0)
	else
		tip.gameObject:SetActive(false)
		region.transform.localPosition = Vector3(30, -1, 0)
		name.transform.localPosition = Vector3(71, -1, 0)
	end
	 
	local text_label = chat_prefab.transform:Find('text'):GetComponent('UILabel')
	name:ProcessText()
	local name_width = name.transform:GetComponent('UIWidget').width + 10
	text_label.transform.localPosition = Vector3(name.transform.localPosition.x + name_width, 7, 0)
	local title = chat_prefab.transform:Find('title'):GetComponent('UILabel')
	local last_start_index = 1
	local row = 0
	local text = {}
	local str = msg.text
	local base_line_width = 362 - name_width
	if msg.type == 1 then
		base_line_width = base_line_width - 32
	end
	text_label.overflowWidth = base_line_width
	text = stringTotable(msg.text)
	local ex_num = 0
	local count = #text
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
			if(text_width > base_line_width - 4) then
				ex_num = ex_num + 1
				fx = 16
				local space_num  = (base_line_width - text_width) / 4
				for  j = 1, space_num do 
					table.insert(text, i, " ")
				end
				last_start_index = i + space_num
				count = count + space_num
				row = row + 1
				ex_pos.x = fx
				ex_pos.y = -row * 28 - 7
				ex_pos.z = 0
			else
				fx = text_width - 16
				if(ex_num > 0) then
					ex_pos.x = fx - row * 8
				else
					ex_pos.x = fx
				end
				ex_pos.y = -row * 28 - 7
				ex_pos.z = 0
			end
			local express = Config.get_t_biaoqing(ex_id)
			if(express ~= nil) then
				local express_t = LuaHelper.Instantiate(express_res_.gameObject)
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
	height = height + 20
	msg_prefab_height[chat_prefab] = height
	chat_prefab.transform.localPosition = Vector3(0, -maxh_, 0) + chat_msg_pos
	pre_height = height
	-----------
	local uv = chat_view_:GetComponent('UIScrollView')
	local offy = uv.panel.clipOffset.y
	local pmaxh = maxh_
	maxh_ = maxh_ + pre_height
	local tpmaxh = pmaxh + offy
	local tmaxh = maxh_ + offy

	if tpmaxh <= viewh_ + 16 and tmaxh > viewh_ then
		--翻页
		if chat_view_:GetComponent('SpringPanel') ~= nil then
			chat_view_:GetComponent('SpringPanel').enabled = false
		end
		uv.panel.clipOffset = Vector2(0, viewh_ - maxh_)
		chat_view_.localPosition = Vector3(0, maxh_ - viewh_ + 76, 0)
	end
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
			chat_view_.localPosition = Vector3(0, minh_ + 76, 0)
		end
	end
	-----------
end

function HallPanel.ShowTip()
	if(lua_script_ ~= nil) then
		self.check_role()
		self.check_achieve()
		role_btn:Find("tip").gameObject:SetActive(false)
		mail_btn_:Find("tip").gameObject:SetActive(false)
		share_btn_:Find("tip").gameObject:SetActive(false)
		achiv_btn:Find("tip").gameObject:SetActive(false)
		dress_btn_:Find("tip").gameObject:SetActive(false)
		friend_btn_:Find("tip").gameObject:SetActive(false)
		vip_btn_:Find("tip").gameObject:SetActive(false)
		first_btn_:Find("tip").gameObject:SetActive(false)
		duobao_btn_:Find("tip").gameObject:SetActive(false)
		rewardeview_btn_:Find("tip").gameObject:SetActive(false)
		if(self.is_role_show > 0) then
			role_btn:Find("tip").gameObject:SetActive(true)
			role_btn:Find("tip/Label"):GetComponent("UILabel").text = self.is_role_show
		end
		if(self.post_num > 0) then
			mail_btn_:Find("tip").gameObject:SetActive(true)
			mail_btn_:Find("tip/Label"):GetComponent("UILabel").text = self.post_num
		end
		if(self.player.fenxiang_state == 1) then
			share_btn_:Find("tip").gameObject:SetActive(true)
		end
		if self.is_achiv_show > 0 then
			achiv_btn:Find("tip/Label"):GetComponent("UILabel").text = self.is_achiv_show
			achiv_btn:Find("tip").gameObject:SetActive(true)
		end
		
		if self.player.level > self.old_level then
			GUIRoot.ShowGUI('LevelUpPanel',{self.old_level,self.player.level,true})
			self.old_level = self.player.level
		else
			AchieveAnimation.CheckHallAchieve(false)  --检查下是否有新的成就完成
		end
		
		if(self.apply_num > 0 or self.social_gold or self.unread_msg_num > 0) then
			friend_btn_:Find("tip").gameObject:SetActive(true)
		end
		
		local task_daily_num = self.check_task_daily_num()
		if task_daily_num > 0 then
			dress_btn_:Find("tip").gameObject:SetActive(true)
			dress_btn_:Find('tip/Label'):GetComponent('UILabel').text = task_daily_num
		else
			dress_btn_:Find("tip").gameObject:SetActive(false)
		end
		
		if(self.player.first_recharge == 1) then
			first_btn_:Find("tip").gameObject:SetActive(true)
		elseif(self.player.first_recharge == 2) then
			first_btn_.gameObject:SetActive(false)
		end
			
		if(self.player.duobao_num < 10) then
			duobao_btn_:Find("tip").gameObject:SetActive(true)
		end
		if ((self.player.advertisement_num < 5) and (tonumber(self.player.advertisement_time) + 300000) < tonumber(timerMgr:now_string())) then
			rewardeview_btn_:Find("tip").gameObject:SetActive(true)
			rewardeview_btn_.gameObject:SetActive(true)
		else
			rewardeview_btn_:Find("tip").gameObject:SetActive(false)
			rewardeview_btn_.gameObject:SetActive(false)
		end
		--local ix = 0
		--local iy_index = 0
		--for i = 1, #top_right_btns_ do
		--	if top_right_btns_[i].gameObject.activeSelf then
		--		top_right_btns_[i].transform.localPosition = Vector3(-4 + ix, -135 - 90 *  math.modf(iy_index / 3), 0)
		--		iy_index = iy_index + 1
		--		ix = - 100 * (iy_index % 3)
		--	end
		--end
	end
end

function HallPanel.AchieveRedSign()
	if(lua_script_ ~= nil) then
		achiv_btn:Find("tip").gameObject:SetActive(true)
	end
end

-----------------------

---------战斗道具-------
function HallPanel.OpenItemPanel()
	local x_pos = GUIRoot.width / 2
	local y_pos = GUIRoot.height / 2
	local from = Vector3(-x_pos, -y_pos, 0) - Vector3(-444, -20, 0)
	GUIRoot.UIEffectScalePos(item_panel_:Find('main_panel').gameObject, true, 0, from)
	HallPanel.InitItemPanel()
end

function HallPanel.InitItemPanel()
	item_panel_:Find('bg').gameObject:SetActive(true)
	pre_select_item = nil
	if(item_view_.childCount >= 1) then
		for i = 0, item_view_.childCount - 1 do
            GameObject.Destroy(item_view_:GetChild(i).gameObject)
        end
	end
	if item_view_:GetComponent('SpringPanel') ~= nil then
		item_view_:GetComponent('SpringPanel').enabled = false
	end
	item_view_:GetComponent('UIPanel').clipOffset = Vector2(0, 0)
	item_view_.localPosition = item_view_pos_
	local j = 0
	for i = 1, #Config.t_item_ids do
		local item_temp = Config.get_t_item(Config.t_item_ids[i])
		if(item_temp.type == 2001) then
			local item = IconPanel.GetIcon("battle_item_res", {"icon", HallPanel.SelectItem}, item_temp.icon, item_temp.color, 0)
			item.transform.parent = item_view_.transform
			item.transform.localPosition = Vector3(j % 3 * item_space_x_, -(math.floor(j / 3) * item_space_y_), 0) + item_pos
			item.transform.localScale = Vector3.one
			local icon = item.transform:Find('icon')
			icon.name = item_temp.id
			if(pre_select_item == nil) then
				pre_select_item = icon.gameObject
			end
			item:SetActive(true)
			j = j + 1
		end
	end
	if(pre_select_item ~= nil) then
		HallPanel.SelectItem(pre_select_item)
	end
	item_panel_.gameObject:SetActive(true)
end

function HallPanel.SelectItem(obj)
	if(pre_select_item ~= nil and pre_select_item ~= obj) then
		pre_select_item.transform.parent:Find('select_icon').gameObject:SetActive(false)
	end
	local name = item_panel_:Find("main_panel/name"):GetComponent("UILabel")
	local desc = item_panel_:Find("main_panel/desc"):GetComponent("UILabel")
	obj.transform.parent:Find('select_icon').gameObject:SetActive(true)
	item_select_id = tonumber(obj.name)
	pre_select_item = obj
	local item_temp = Config.get_t_item(item_select_id)
	name.text = "[bd985b]"..item_temp.name
	local desc_t = string_replace(item_temp.desc, "[c2e5ed]", "[47515c]")
	desc_t = string_replace(desc_t, "[5bd300]", "[c06800]")
	desc_t = string_replace(desc_t, "[e4ac01]", "[695e00]")
	desc.text = desc_t
end

function HallPanel.CloseItemPanel()
	item_panel_.gameObject:SetActive(false)
end

function HallPanel.HideItemPanel()
	local x_pos = GUIRoot.width / 2
	local y_pos = GUIRoot.height / 2
	local from = Vector3(-x_pos, -y_pos, 0) - Vector3(-444, -20, 0)
	GUIRoot.UIEffectScalePos(item_panel_:Find('main_panel').gameObject, false, 0, from)
	timerMgr:AddTimer("HallPanel", HallPanel.CloseItemPanel, 0.2)
	item_select_id = 0
	pre_select_item = nil
end
-------------------------


----------vip------------------
function HallPanel.OpenVipPanel()
	HallPanel.InitVipPanel()
	-- local x_pos = GUIRoot.width / 2
	-- local y_pos = GUIRoot.height / 2
	-- local from = Vector3(x_pos, y_pos, 0) - Vector3(100, 310, 0)
	-- GUIRoot.UIEffectScalePos(vip_panel_:Find('main_panel').gameObject, true, 0, from)
end

function HallPanel.InitVipPanel()
	local vip_y_btn = vip_panel_:Find("main_panel/vip_y_btn")
	local vip_m_btn = vip_panel_:Find("main_panel/vip_m_btn")
	local reward_y_root = vip_panel_:Find("main_panel/vip_y/reward_root")
	local reward_m_root = vip_panel_:Find("main_panel/vip_m/reward_root")
	local vip_y_price = vip_panel_:Find("main_panel/vip_y/price"):GetComponent("UILabel")
	local vip_m_price = vip_panel_:Find("main_panel/vip_m/price"):GetComponent("UILabel")
	local vip_y_time = vip_panel_:Find("main_panel/vip_y/time"):GetComponent("UILabel")
	local vip_m_time = vip_panel_:Find("main_panel/vip_m/time"):GetComponent("UILabel")
	vip_y_price.text = "$"..Config.get_t_recharge(2002).price
	vip_m_price.text = "$"..Config.get_t_recharge(2001).price
	if tonumber(self.player.yue_time) > tonumber(timerMgr:now_string()) then
		vip_m_price.text = ""
		local time = tonumber(self.player.yue_time) - tonumber(timerMgr:now_string())
		vip_m_time.text = Config.get_t_script_str('HallPanel_005')..count_time_day(time, 1) --"剩余时间："..count_time_day(time, 1)
		if(self.player.yue_reward == 0) then
			vip_m_btn:GetComponent("UISprite").spriteName = "b1_green"
			vip_m_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_006') --"立即领取"
		else
			vip_m_btn:GetComponent("UISprite").spriteName = "b1_gray"
			vip_m_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_007') --"已领取"
		end
	else
		vip_m_time.text = ""
		vip_m_btn:GetComponent("UISprite").spriteName = "b1"
		vip_m_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_008') --"成为月卡会员"
	end
	
	if tonumber(self.player.nian_time) > tonumber(timerMgr:now_string()) then
		vip_m_btn:GetComponent("UISprite").spriteName = "b1_gray"
		vip_y_price.text = ""
		local time = tonumber(self.player.nian_time) - tonumber(timerMgr:now_string())
		vip_y_time.text = Config.get_t_script_str('HallPanel_005')..count_time_day(time, 1)
		if(self.player.nian_reward == 0) then
			vip_y_btn:GetComponent("UISprite").spriteName = "b1_green"
			vip_y_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_006') --"立即领取"
		else
			vip_y_btn:GetComponent("UISprite").spriteName = "b1_gray"
			vip_y_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_007') --"已领取"
		end
	else
		vip_y_time.text = ""
		vip_y_btn:GetComponent("UISprite").spriteName = "b1_yellow"
		vip_y_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_009') --"成为年卡会员"
	end
	
	if(reward_y_root.childCount >= 1) then
		for i = 0, reward_y_root.childCount - 1 do
            GameObject.Destroy(reward_y_root:GetChild(i).gameObject)
        end
	end
	if(reward_m_root.childCount >= 1) then
		for i = 0, reward_m_root.childCount - 1 do
            GameObject.Destroy(reward_m_root:GetChild(i).gameObject)
        end
	end
	local y_rewards = Config.get_t_item_box(100301).rewards
	local m_rewards = Config.get_t_item_box(100201).rewards
	for i = 1, #y_rewards do
		local reward_temp = Config.get_t_reward(y_rewards[i].type, y_rewards[i].value1)
		local reward_t = IconPanel.GetIcon("reward_res", nil, reward_temp.icon, reward_temp.color, y_rewards[i].value2)
		reward_t.transform:Find("icon").name = y_rewards[i].type.."+"..reward_temp.id
		reward_t.transform.parent = reward_y_root
		reward_t.transform.localPosition = Vector3((i - 1) * 95, 0, 0)
		reward_t.transform.localScale = Vector3.one
		reward_t.gameObject:SetActive(true)
	end
	for i = 1, #m_rewards do
		local reward_temp = Config.get_t_reward(m_rewards[i].type, m_rewards[i].value1)
		local reward_t = IconPanel.GetIcon("reward_res", nil, reward_temp.icon, reward_temp.color, m_rewards[i].value2)
		reward_t.transform:Find("icon").name = m_rewards[i].type.."+"..reward_temp.id
		reward_t.transform.parent = reward_m_root
		reward_t.transform.localPosition = Vector3((i - 1) * 95, 0, 0)
		reward_t.transform.localScale = Vector3.one
		reward_t.gameObject:SetActive(true)
	end
	vip_panel_.gameObject:SetActive(true)
	HallScene.AddUIRole(3005, Vector3(-33, -0.55, -1))
	HallScene.AddUIRole(3006, Vector3(-37, -0.55, -1), Vector3(0, 45, 0))
end

function HallPanel.CloseVipPanel()
	vip_panel_.gameObject:SetActive(false)
end

function HallPanel.HideVipPanel()
	-- local x_pos = GUIRoot.width / 2
	-- local y_pos = GUIRoot.height / 2
	-- local from = Vector3(x_pos, y_pos, 0) - Vector3(100, 310, 0)
	-- GUIRoot.UIEffectScalePos(vip_panel_:Find('main_panel').gameObject, false, 0, from)
	-- timerMgr:AddTimer("HallPanel", HallPanel.CloseVipPanel, 0.2)
	HallPanel.CloseVipPanel()
	HallScene.RemoveUIRole()
end

function HallPanel.VipOperation(mode)
	if(mode == 1) then
		if tonumber(self.player.nian_time) > tonumber(timerMgr:now_string()) then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('HallPanel_010')})
		else
			if tonumber(self.player.yue_time) > tonumber(timerMgr:now_string()) then
				if(self.player.yue_reward == 0) then
					local msg = msg_hall_pb.cmsg_vip_reward()
					msg.type = 1
					reward_id_ = 1
					local data = msg:SerializeToString()
					GameTcp.Send(opcodes.CMSG_VIP_REWARD, data, {opcodes.SMSG_VIP_REWARD})
				else
					GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('HallPanel_011')})
				end
			else
				GUIRoot.HideGUI('HallPanel')
				GUIRoot.ShowGUI("ShopPanel", {5})
				HallScene.RemoveUIRole()
			end
		end
	else
		if tonumber(self.player.nian_time) > tonumber(timerMgr:now_string()) then
			if(self.player.nian_reward == 0) then
				local msg = msg_hall_pb.cmsg_vip_reward()
				msg.type = 2
				reward_id_ = 2
				local data = msg:SerializeToString()
				GameTcp.Send(opcodes.CMSG_VIP_REWARD, data, {opcodes.SMSG_VIP_REWARD})
			else
				GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('HallPanel_011')})
			end
		else
			GUIRoot.HideGUI('HallPanel')
			GUIRoot.ShowGUI("ShopPanel", {5})
			HallScene.RemoveUIRole()
		end
	end
end

-------------------------------


----------首充-------------------

function HallPanel.OpenFirstPanel()
	HallPanel.InitFirstPanel()
	--local x_pos = GUIRoot.width / 2
	--local y_pos = GUIRoot.height / 2
	--local from = Vector3(x_pos, y_pos, 0) - Vector3(80, 135, 0)
	--GUIRoot.UIEffectScalePos(first_panel_:Find('main_panel').gameObject, true, 0, from)
end

function HallPanel.InitFirstPanel()
	local reward_root = first_panel_:Find("main_panel/reward_root")
	local first_recharg_btn = first_panel_:Find("main_panel/first_recharge_btn")
	if(self.player.first_recharge == 0) then
		first_recharg_btn:GetComponent("UISprite").spriteName = "b1_yellow"
		first_recharg_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_012') --"立即充值"
	elseif(self.player.first_recharge == 1) then
		first_recharg_btn:GetComponent("UISprite").spriteName = "b1_green"
		first_recharg_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_013') --"立即领取"
	end
	if(reward_root.childCount >= 1) then
		for i = 0, reward_root.childCount - 1 do
            GameObject.Destroy(reward_root:GetChild(i).gameObject)
        end
	end
	local first_rewards = Config.get_t_item_box(1001).rewards
	local j = 0
	for i = 1, #first_rewards do
		if(first_rewards[i].type ~= 3) then
			local reward_temp = Config.get_t_reward(first_rewards[i].type, first_rewards[i].value1)
			local reward_t = IconPanel.GetIcon("reward_res", nil, reward_temp.icon, reward_temp.color, first_rewards[i].value2)
			reward_t.transform:Find("icon").name = first_rewards[i].type.."+"..reward_temp.id
			reward_t.transform.parent = reward_root
			reward_t.transform.localPosition = Vector3(j * 100, 0, 0)
			reward_t.transform.localScale = Vector3.one
			reward_t.gameObject:SetActive(true)
			j = j + 1
		end
	end
	first_panel_.gameObject:SetActive(true)
	local role_pos = Vector3(-33.7 , 0, 0)
	HallScene.AddUIRole(2006, role_pos)
end

function HallPanel.CloseFirstPanel()
	first_panel_.gameObject:SetActive(false)
end

function HallPanel.HideFirstPanel()
	-- local x_pos = GUIRoot.width / 2
	-- local y_pos = GUIRoot.height / 2
	-- local from = Vector3(x_pos, y_pos, 0) - Vector3(80, 135, 0)
	-- GUIRoot.UIEffectScalePos(first_panel_:Find('main_panel').gameObject, false, 0, from)
	-- timerMgr:AddTimer("HallPanel", HallPanel.CloseFirstPanel, 0.2)
	first_panel_.gameObject:SetActive(false)
	HallScene.RemoveUIRole()
end

function HallPanel.RoleToHall(type)
	if(type == 3) then
		HallPanel.OpenFirstPanel()
	elseif type == 4 then
		HallPanel.OpenVipPanel()
	end
end

---------------------------------

--------------战斗模式------------

function HallPanel.OpenModePanel()
	HallPanel.InitModePanel()
	mode_panel_:Find("main_panel/Anchor/tip"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_014') --"[c2e5ed]战斗结算双倍收益剩余次数（[ffcd00]3级[-]后自嗨模式结算不享受双倍收益）"
	mode_panel_:Find("main_panel/Anchor/time_tip"):GetComponent("UILabel").text = Config.get_t_script_str('HallPanel_015') --"[c2e5ed]每日[57FC5B] 5:00 [-]重置双倍次数"
	local x_pos = GUIRoot.width / 2
	local y_pos = GUIRoot.height / 2
	local from = Vector3(x_pos, -y_pos, 0) - Vector3(120, -160, 0)
	mode_panel_.gameObject:SetActive(true)
	GUIRoot.UIEffectScalePos(mode_panel_:Find('main_panel').gameObject, true, 0, from)
end

function HallPanel.InitModePanel()
	local double_pro = mode_panel_:Find("main_panel/Anchor/pro"):GetComponent("UISlider")
	local double_num = mode_panel_:Find("main_panel/Anchor/pro/num"):GetComponent("UILabel")
	double_pro.value = (3 - self.player.box_zd_num) / 3
	double_num.text = (3 - self.player.box_zd_num).."/"..(3)
	local mode_view = mode_panel_:Find("main_panel/mode_view")
	local mode_res = mode_panel_:Find("main_panel/mode_res").gameObject
	if(mode_view.childCount > 0) then
		for i = 0, mode_view.childCount - 1 do
            GameObject.Destroy(mode_view:GetChild(i).gameObject)
        end
	end
	mode_view.localPosition = Vector3(0, 0, 0)
	if mode_view:GetComponent('SpringPanel') ~= nil then
		mode_view:GetComponent('SpringPanel').enabled = false
	end
	mode_view:GetComponent('UIPanel').clipOffset = Vector2(0, 0)
	for i = 1, #Config.t_mode do
		local mode_temp = Config.t_mode[i]
		local mode_t = LuaHelper.Instantiate(mode_res)
		mode_t.transform.parent = mode_view
		mode_t.transform.localPosition = Vector3(mode_x_pos + (i - 1) * mode_x_space, 15, 0)
		mode_t.transform.localScale = Vector3.one
		mode_t.name = i
		mode_t.transform:GetComponent("UISprite").spriteName = mode_temp.icon
		mode_t.transform:Find("limit"):GetComponent("UILabel").text = ""
		mode_t.transform:Find("limit").gameObject:SetActive(false)
		if(self.player.level < mode_temp.level) then
			mode_t.transform:Find("limit").gameObject:SetActive(true)
			IconPanel.InitCupLabel('jb055', mode_t.transform:Find("limit"):GetComponent("UILabel"))
			HallPanel.GrayLabel(mode_t.transform:Find("Label"):GetComponent("UILabel"))
			HallPanel.GrayLabel(mode_t.transform:Find("eng"):GetComponent("UILabel"))
			mode_t.transform:GetComponent("UISprite").IsGray = true
			mode_t.transform:Find("limit"):GetComponent("UILabel").text = string.format(Config.get_t_script_str('HallPanel_016'),mode_temp.level)
		end
		mode_t.transform:GetComponent("UIWidget").depth = i
		mode_t.transform:Find("Label"):GetComponent("UIWidget").depth = i + 1
		mode_t.transform:Find("eng"):GetComponent("UIWidget").depth = i + 1
		mode_t.transform:Find("limit"):GetComponent("UIWidget").depth = i + 2
		mode_t.transform:Find("tip"):GetComponent("UIWidget").depth = i + 1
		mode_t.transform:Find("limit/bg"):GetComponent("UIWidget").depth = i + 1
		mode_t.transform:Find("Label"):GetComponent("UILabel").text = mode_temp.name1
		mode_t.transform:Find("eng"):GetComponent("UILabel").text = mode_temp.name2
		local mode_detail_btn = mode_t.transform:Find("tip").gameObject
		lua_script_:AddButtonEvent(mode_t, "click", HallPanel.StartGame)
		lua_script_:AddButtonEvent(mode_detail_btn, "click", HallPanel.ModeDetail)
		mode_detail_btn.name = i
		mode_t:SetActive(true)
		mode_t.transform:GetComponent("UISprite").alpha = 0
		local from = mode_t.transform.localPosition + Vector3(100, 0, 0)
		twnMgr:Add_Tween_Postion(mode_t, 0.1, from, mode_t.transform.localPosition, 4, i * 0.1 + 0.1)
		twnMgr:Add_Tween_Alpha(mode_t, 0.1, 0, 1, 4, i * 0.1 + 0.1)
	end
end

function HallPanel.CountModeSpace()
	local mode_x_line = 0
	for i = 1, math.ceil(GUIRoot.width / mode_width) do
		if((GUIRoot.width - 100 - mode_width ) / i >= mode_width + 40) then
			mode_x_line = i + 1
		end
	end
	mode_x_space = GUIRoot.width / 2 - 20 -  mode_width / 2
	mode_x_pos = -GUIRoot.width / 2 + mode_width / 2 + 20
end

function HallPanel.StartGame(mode)
	mode = tonumber(mode.name)
	local mode_temp = Config.t_mode[mode]
	if(self.player.level < mode_temp.level) then
		local content = string.format(Config.get_t_script_str('HallPanel_017'),mode_temp.level)
		GUIRoot.ShowGUI("MessagePanel", {content})
		return
	end
	if(mode == 2) then
		if self.is_ios_sh then
			State.ChangeState(State.state.ss_ofbattle)
		else
			GameTcp.Send(opcodes.CMSG_SINGLE_BATTLE, nil, {opcodes.SMSG_SINGLE_BATTLE, opcodes.SMSG_HAS_BATTLE})
		end	
	elseif(mode == 3) then
		if	self.is_ios_sh then
			State.ChangeState(State.state.ss_ofbattle)
		else
			GameTcp.Send(opcodes.CMSG_TEAM_CREATE, nil, {opcodes.SMSG_TEAM_CREATE, opcodes.SMSG_HAS_BATTLE})
		end	
	elseif(mode == 1) then
		State.ChangeState(State.state.ss_ofbattle)
	end
end

function HallPanel.CloseModePanel()
	mode_panel_.gameObject:SetActive(false)
end

function HallPanel.HideModePanel()
	mode_page_ = 0
	local x_pos = GUIRoot.width / 2
	local y_pos = GUIRoot.height / 2
	local from = Vector3(x_pos, -y_pos, 0) - Vector3(120, -160, 0)
	GUIRoot.UIEffectScalePos(mode_panel_:Find('main_panel').gameObject, false, 0, from)
	timerMgr:AddTimer("HallPanel", HallPanel.CloseModePanel, 0.2)
end

function HallPanel.ModeDetail(mode)
	mode = tonumber(mode.name)
	local main_panel = mode_detail_:Find("main_panel")
	local name = main_panel:Find("name"):GetComponent("UILabel")
	local desc = main_panel:Find("desc"):GetComponent("UILabel")
	local mode_temp = Config.t_mode[mode]
	name.text = mode_temp.name1
	desc.text = mode_temp.desc
	GUIRoot.UIEffectScalePos(main_panel.gameObject, true, 1)
	mode_detail_.gameObject:SetActive(true)
end

function HallPanel.GrayLabel(label)
	label.applyGradient = false
	label.effectStyle = UILabel.Effect.None
	label.color = Color(181/ 255,181/ 255,181/ 255)
end
----------------------------------


--------------ButtonEvent---------

function HallPanel.Click(obj)
	if(obj.name == 'item_btn') then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI('ItemPanel')
	elseif(obj.name == 'dress_btn') then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI('LevelTask')
	elseif(obj.name == 'role_btn') then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI('RolePanel')
	elseif(obj.name == 'chat_btn') then
		GUIRoot.ShowGUI('ChatPanel')
	elseif(obj.name == 'online_btn') then
		HallPanel.OpenModePanel()
	elseif(obj.name == 'offline_btn') then
		HallPanel.StartOfflineGame()
	elseif(obj.name == 'share_btn') then
		GUIRoot.ShowGUI('SharePanel', {1})
	elseif(obj.name == 'shop_btn') then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI('ShopPanel')
	elseif(obj.name == 'close_btn') then -- 道具
		HallPanel.HideItemPanel()
	elseif(obj.name == 'achiv_btn') then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI('NewAchievePanel')
	elseif(obj.name == 'battle_item_btn') then
		HallPanel.OpenItemPanel()
	elseif(obj.name == 'mail_btn') then
		GameTcp.Send(opcodes.CMSG_POST_LOOK, nil, {opcodes.SMSG_POST_LOOK})
	elseif(obj.name == 'set_btn') then
		GUIRoot.ShowGUI('SetPanel', {0})
	elseif(obj.name == 'friend_btn') then
		FriendPanel.SocialLook(2)
		--GameTcp.Send(opcodes.CMSG_TEAM_TUIJIAN, nil, {opcodes.SMSG_TEAM_TUIJIAN})
	elseif(obj.name == 'chest_btn') then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI('ChestShowPanel')
	elseif(obj.name == 'mode_close_btn') then
		HallPanel.HideModePanel()
	elseif(obj.name == 'mode_detail_close') then
		mode_detail_.gameObject:SetActive(false)
	elseif(obj.name == 'vip_close_btn') then
		HallPanel.HideVipPanel()
	elseif(obj.name == 'vip_btn') then
		HallPanel.OpenVipPanel()
	elseif(obj.name == 'vip_m_btn') then
		HallPanel.VipOperation(1)
	elseif(obj.name == 'vip_y_btn') then
		HallPanel.VipOperation(2)
	elseif(obj.name == 'first_btn') then
		HallPanel.OpenFirstPanel()
	elseif(obj.name == 'first_close_btn') then
		HallPanel.HideFirstPanel()
	elseif(obj.name == 'first_recharge_btn') then
		if(self.player.first_recharge == 0) then
			GUIRoot.HideGUI('HallPanel')
			GUIRoot.ShowGUI("ShopPanel", {5})
			HallScene.RemoveUIRole()
		elseif(self.player.first_recharge == 1) then
			local msg = msg_hall_pb.cmsg_vip_reward()
			msg.type = 0
			reward_id_ = 0
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_VIP_REWARD, data, {opcodes.SMSG_VIP_REWARD})
		end
	elseif(obj.name == 'duobao_btn') then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI("ShopPanel", {4})
	elseif obj.name == 'rank_btn' then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI('RankPanel')
	elseif obj.name == 'rewarde_vedio' then
		if (self.player.advertisement_num < 5) and ((tonumber(self.player.advertisement_time) + 300000) < tonumber(timerMgr:now_string())) then
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('HallPanel_026'),Config.get_t_script_str('HallPanel_023'), HallPanel.OpenVideo, Config.get_t_script_str('ShopPanel_012')})
			if UnityAdsHelper.Donefunc == nil then
				UnityAdsHelper.Donefunc = HallPanel.handleFinished
				UnityAdsHelper.Skipfunc = HallPanel.handleSkipped
			end
		end
	end
end

function HallPanel.OnDrag(delta)
	HallScene.Roll(delta.x)
end

----------------------------------

--------------Function--------------------

function HallPanel.Refresh()
	HallPanel.RefreshChest()
	HallPanel.Update()
	HallPanel.UpdateVideo()
end

function HallPanel.ShowGongGao()
	if(#self.gonggao > 0 and gonggao_time == 0 and tonumber(self.gonggao[1]["id"]) > PlayerData.max_gonggao_id) then
		gonggao_time = gonggao_time + 1
		GUIRoot.ShowGUI("SetPanel", {1, self.gonggao[1]["id"]})
	else
		if(self.player.sign_finish == 0) then
			GUIRoot.ShowGUI("SignPanel")
		end
	end
end

function HallPanel.CheckConnect()
	if not GameTcp.Isconnect() then
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('HallPanel_018'),Config.get_t_script_str('HallPanel_019'), GameTcp.ReConnect})
	end
end

function HallPanel.OpenVideo()
	if (tonumber(self.player.advertisement_time)) <= tonumber(timerMgr:now_string()) then
		if UnityAdsHelper.isInitialized then
			if UnityAdsHelper.IsReady("rewardedVideo") then
				UnityAdsHelper.ShowAd("rewardedVideo")
			else
				HallPanel.handleFinished()
			end
		end
	else
		local _time = tonumber(self.player.advertisement_time) - tonumber(timerMgr:now_string())
		GUIRoot.ShowGUI("MessagePanel", {string.format(Config.get_t_script_str('HallPanel_028'),count_time(_time))})
	end
end

function HallPanel.handleFinished()
	GameTcp.Send(opcodes.CMSG_ADVERTISEMENT, data, {opcodes.SMSG_ADVERTISEMENT}, Config.get_t_script_str('HallPanel_025'), 60)
end

function HallPanel.handleSkipped()
	GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('HallPanel_027')})
end
-------------------------------------------


-----------服务器ocde-------------

function HallPanel.SMSG_ADVERTISEMENT(message)
	self.player.advertisement_num = self.player.advertisement_num + 1
	self.player.advertisement_time = tonumber(timerMgr:now_string())
	local reward = {}
	reward.type = 1
	reward.value1 = 2
	reward.value2 = 20
	reward.value3 = 0
	self.add_reward(reward)
	GUIRoot.ShowGUI("GainPanel", {{reward}})
end

function HallPanel.SMSG_SINGLE_BATTLE(message)
	local msg = msg_team_pb.smsg_single_battle()
	msg:ParseFromString(message.luabuff)
	self.battle_code = msg.code
	-- 扣战场道具
	--if msg.is_new == 1 then
	--	self.player.battle_reset_skill_num = 0
	--end
	BattleStateTcp.Init(msg.tcp_ip,msg.tcp_port)
	BattleTcp.Connect(msg.udp_ip, msg.udp_port)
end

function HallPanel.SMSG_TEAM_CREATE(message)
	local msg = msg_team_pb.smsg_team_create()
	msg:ParseFromString(message.luabuff)
	NoticePanel.FiniInvert()
	GUIRoot.ShowGUI("TeamPanel", {msg.team})
	GUIRoot.HideGUI('HallPanel')
end

function HallPanel.SMSG_HAS_BATTLE()
	local msg = msg_team_pb.smsg_has_battle()
	msg:ParseFromString(HallPanel.tip_battle_message.luabuff)
	GUIRoot.HideGUI("SignPanel")
	GUIRoot.HideGUI("SetPanel")
	gonggao_time = 0
	self.battle_code = msg.code
	HallPanel.tip_battle_message = nil
	local rebattle = function()
		if HallPanel.tip_battle_flag then
			BattleStateTcp.Init(msg.tcp_ip,msg.tcp_port)
			BattleTcp.Connect(msg.udp_ip, msg.udp_port)
		else
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('HallPanel_020'),Config.get_t_script_str('HallPanel_023')})
		end
	end
	GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('HallPanel_021'), Config.get_t_script_str('HallPanel_022'), rebattle})
end

function HallPanel.SMSG_CHAT(msg)
	if(lua_script_ == nil) then
		return 0
	end
	HallPanel.NewMesssage(msg)
end

function HallPanel.SMSG_SYS_INFO(msg)
	if(lua_script_ == nil) then
		return 0
	end
	HallPanel.NewSysMessage(msg)
end

function HallPanel.SMSG_POST_LOOK(message)
	local msg = msg_hall_pb.smsg_post_look()
	msg:ParseFromString(message.luabuff)
	GUIRoot.HideGUI('HallPanel')
	GUIRoot.ShowGUI('MailPanel', {msg.posts})
end

function HallPanel.SMSG_SOCAIL_LOOK(message)
	local msg = msg_social_pb.smsg_social_look()
	msg:ParseFromString(message.luabuff)
	if(friend_request == 0) then
		for i = 1, #msg.socials do
			NoticePanel.LoadSocialMsg(msg.socials[i])
		end
		friend_request = friend_request + 1
	end
	GUIRoot.HideGUI('HallPanel')
	GUIRoot.ShowGUI('FriendPanel', {msg.socials})
end

function HallPanel.SMSG_TEAM_TUIJIAN(message)
	local msg = msg_team_pb.smsg_team_tuijian()
	msg:ParseFromString(message.luabuff)
	local socials = {}
	for i = 1, #msg.players do
		local social = {}
		social.guid = msg.players[i].guid
		social.player_guid = msg.players[i].guid
		social.target_guid = msg.players[i].guid
		social.name = msg.players[i].name
		social.cup = msg.players[i].cup
		social.avatar = msg.players[i].avatar
		social.toukuang = msg.players[i].toukuang
		social.level = 1
		social.sex = msg.players[i].sex
		social.stype = 3
		social.gold = 50
		social.verify = Config.get_t_script_str('HallPanel_024') --"交个朋友吧~~"
		table.insert(socials, social)
	end
	
	if not BattleTcp.Isconnect() then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI('FriendPanel', {socials})
	end
end

function HallPanel.SMSG_VIP_REWARD(message)
	local msg = msg_hall_pb.smsg_vip_reward()
	msg:ParseFromString(message.luabuff)
	if(reward_id_ == 0) then
		self.player.first_recharge = 2
		--HallPanel.CloseFirstPanel()
		HallScene.RemoveUIRole()
	elseif(reward_id_ == 1) then
		self.player.yue_reward = 1
		HallScene.RemoveUIRole()
		HallPanel.InitVipPanel()
	elseif(reward_id_ == 2) then
		self.player.nian_reward = 1
		HallScene.RemoveUIRole()
		HallPanel.InitVipPanel()
	end
	--HallPanel.ShowTip()
	
	local rewards = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
	for i = 1, #rewards do
		self.add_reward(rewards[i])
	end
	for i = 1, #msg.roles do
		self.add_role(msg.roles[i])
	end
	if(#msg.roles > 0) then
		GUIRoot.HideGUI('HallPanel')
		GUIRoot.ShowGUI("RoleGetPanel", {msg.roles, {"HallPanel", "TopPanel"}, rewards})
	else
		if(#rewards > 0) then
			GUIRoot.ShowGUI('GainPanel', {rewards})
		end
	end
end
