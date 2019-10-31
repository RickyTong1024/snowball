ShopPanel = {}

local shop_view_
local lua_script_

local chest_panel_
local shop_res_
local effect_res_
local chest_res_
local recharge_res_

local page_btn_ = {}

local view_offset_x = 0
local view_offset_y = 0

local shop_space_x_ = 240
local shop_space_y_ = 260

local shop_x_line = 0

local shop_width = 160
local shop_height = 240

local shop_pos = Vector3(0, 0, 0)

local view_softness = Vector2(0, 20)
local view_max = 0
local shop_arrow_

local recharge_id_ = 0
local recharge_order_id = ""

local page_name_ = {"item_btn", "role_btn", "effect_btn", "chest_btn", "recharge_btn"}
local page_text = {'ShopPanel_001', 'ShopPanel_002', 'ShopPanel_003', 'ShopPanel_004', 'ShopPanel_005','ShopPanel_020'}
local page_icon = {"daoju", "js", "xueqiu", "bx", "zs"}
local dj_frame = {"djd-bule", "djd-pul", "djd-gold", "djd-geeen"}
local bx_frame = {"","bsd-pul", "bsd-gold"}
local purple_font
local yellow_font
local blue_font

local time_speed = 0.01
local chest_space = 160
local is_rolling = false
local roll_speed = 0.01
local roll_time = 0
local speed_add = 0.01
local max_speed = 0.05
local roll_id = 1
local target_id = 1
local max_x_num = 4
local max_y_num = 1
local roll_num = 10
local chest_list = {}
local roll_rewards = {}
local roll_roles = {}
local purchase_id_ = nil

function ShopPanel.Awake(obj)
	roll_num = 2 * max_x_num + 2 * max_y_num
	GUIRoot.UIEffectScalePos(obj, true, 1)
	GUIRoot.ShowGUI('BackPanel', {3})
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	purple_font = obj.transform:Find("purple_font"):GetComponent("UILabel").bitmapFont
	yellow_font = obj.transform:Find("yellow_font"):GetComponent("UILabel").bitmapFont
	blue_font = obj.transform:Find("blue_font"):GetComponent("UILabel").bitmapFont
	
	chest_panel_ = obj.transform:Find("Anchor_top/chest_panel")
	
	shop_view_ = obj.transform:Find('shop_view')
	shop_res_ = obj.transform:Find('shop_res')
	effect_res_ = obj.transform:Find('effect_res')
	chest_res_ = chest_panel_:Find('chest_res')
	recharge_res_ = obj.transform:Find("recharge_res")
	shop_arrow_ = obj.transform:Find("shop_arrow")
	
	shop_arrow_.localPosition = Vector3(70, 60 - GUIRoot.height / 2, 0)
	shop_arrow_:GetComponent("TweenPosition").from = Vector3(70, 60 - GUIRoot.height / 2, 0)
	shop_arrow_:GetComponent("TweenPosition").to = Vector3(70, 40 - GUIRoot.height / 2, 0)
	
	view_offset_x = 175 - (GUIRoot.width / 2 - (GUIRoot.width - 210) / 2)
	view_offset_y = GUIRoot.height / 2 - (GUIRoot.height - 140) / 2 - 50
	
	shop_view_:GetComponent("UIScrollView").panel:SetRect(view_offset_x, view_offset_y, GUIRoot.width - 210, GUIRoot.height - 140)
	shop_view_:GetComponent("UIScrollView").panel.clipSoftness = view_softness
	
	local shop_btn_res = obj.transform:Find('Anchor_left/left_panel/shop_btn_res').gameObject
	local left_panel = obj.transform:Find('Anchor_left/left_panel')
	local fir_pos = GUIRoot.height / 2 - 160
	for i = 1, #page_name_ do
		local btn_t = LuaHelper.Instantiate(shop_btn_res)
		btn_t.transform.parent = left_panel
		btn_t.transform.localPosition = Vector3(-36, fir_pos - (i - 1) * 80, 0)
		btn_t.transform.localScale = Vector3.one
		btn_t.name = page_name_[i]
		btn_t.transform:GetComponent("UISprite").spriteName = "zuolan1"
		btn_t.transform:Find("Label"):GetComponent("UILabel").text = "[c2e5ed]"..Config.get_t_script_str(page_text[i])
		if(i == 5 and platform_config_common.m_platform == "android_yymoon") then
			btn_t.transform:Find("Label"):GetComponent("UILabel").text = "[c2e5ed]"..Config.get_t_script_str(page_text[i+1])
		end
		btn_t.transform:Find('icon'):GetComponent("UISprite").spriteName = page_icon[i]
		btn_t:SetActive(true)
		table.insert(page_btn_, btn_t.transform)
	end
	
	for i = 1, #page_btn_ do
		lua_script_:AddButtonEvent(page_btn_[i].gameObject, "click", ShopPanel.Click)
	end
	
	local open_btn = chest_panel_:Find("open_btn").gameObject
	lua_script_:AddButtonEvent(open_btn, "click", ShopPanel.Click)
	
	lua_script_:AddButtonEvent(shop_view_.gameObject, "onDragFinished", ShopPanel.OnDragFinished)
	
	chest_panel_.gameObject:SetActive(false)
	ShopPanel.RegisterMessage()
	ShopPanel.CountLine()
	ShopPanel.SelectPage(1)
	purchase_id_ = nil
end

function ShopPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_RECHARGE, ShopPanel.SMSG_RECHARGE)
	Message.register_net_handle(opcodes.SMSG_DUOBAO, ShopPanel.SMSG_DUOBAO)
	Message.register_handle("back_panel_msg", ShopPanel.Back)
	Message.register_handle("back_panel_recharge", ShopPanel.BackRecharge)
	Message.register_handle("team_join_msg", ShopPanel.TeamJoin)
end

function ShopPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_RECHARGE, ShopPanel.SMSG_RECHARGE)
	Message.remove_net_handle(opcodes.SMSG_DUOBAO, ShopPanel.SMSG_DUOBAO)
	Message.remove_handle("back_panel_msg", ShopPanel.Back)
	Message.remove_handle("back_panel_recharge", ShopPanel.BackRecharge)
	Message.remove_handle("team_join_msg", ShopPanel.TeamJoin)
end

function ShopPanel.OnDestroy()
	if(platform_recharge._instance.Donefunc ~= nil) then
		platform_recharge._instance.Donefunc = nil
	end
	if(platform_recharge._instance.Canelfunc ~= nil) then
		platform_recharge._instance.Canelfunc = nil
	end	
	page_btn_ = {}
	chest_list = {}
	roll_rewards = {}
	roll_roles = {}
	ShopPanel.RemoveMessage()
	recharge_order_id = ""
end

function ShopPanel.OnParam(parm)
	if(parm == nil) then
		ShopPanel.SelectPage(1)
	else
		ShopPanel.SelectPage(parm[1])
	end
end

function ShopPanel.Back()
	if(is_rolling) then
		ShopPanel.StopRoll()
		return
	end
	GUIRoot.HideGUI('ShopPanel')
	GUIRoot.ShowGUI('HallPanel')
	GUIRoot.HideGUI('BackPanel')
end

function ShopPanel.BackRecharge()
	if(is_rolling) then
		ShopPanel.StopRoll()
		return
	end
	ShopPanel.SelectPage(5)
end

function ShopPanel.TeamJoin()
	if(is_rolling) then
		ShopPanel.StopRoll()
		return
	end
	GUIRoot.HideGUI('ShopPanel')
end

----------------------------------刷新列表----------------------------

function ShopPanel.RefreshCurrentEffectInfo()
	if lua_script_ == nil then
		return
	end
	
	for i = 1,shop_view_.childCount do
		local shop_t = shop_view_:GetChild(i - 1).gameObject
		local name = shop_t.name
		local v = Config.get_t_shop(tonumber(name))
		
		local can_show = true
		if(v.pre_item ~= 0) then
			local shop = Config.get_t_shop(v.id)
			if self.get_role_id(shop.value1) == nil then
				can_show = false
			end
		end
		if(v.item_type == 3 and self.get_role_id(v.value1) ~= nil) then
			can_show = false
		end
		if(v.type == 3 and can_show) then	
			local price_icon = shop_t.transform:Find('price_icon'):GetComponent('UISprite')
			local price_label = shop_t.transform:Find('price'):GetComponent('UILabel')

			if(self.has_fashion(v.value1)) then
				price_icon.gameObject:SetActive(false)
				price_label.gameObject:SetActive(false)
				shop_t.transform:Find('tip').gameObject:SetActive(true)
			end
		end
	end
end

function ShopPanel.InitItemList(type)
	if(type == nil) then
		type = 2
	end
	local i = 0
	for k, v in pairsByKeys(Config.t_shop) do
		local can_show = true
		if(v.pre_item ~= 0) then
			local shop = Config.get_t_shop(v.id)
			if self.get_role_id(shop.value1) == nil then
				can_show = false
			end
		end
		if(v.item_type == 3 and self.get_role_id(v.value1) ~= nil) then
			can_show = false
		end
		if(v.type == type and can_show) then
			local item_temp = Config.get_t_reward(v.item_type, v.value1)
			local shop_res = shop_res_
			if(type == 3) then
				shop_res = effect_res_
			end
			local shop_t = LuaHelper.Instantiate(shop_res.gameObject)
			shop_t.transform.parent = shop_view_.transform
			shop_t.transform.localPosition = Vector3(i % shop_x_line * shop_space_x_, -(math.floor(i / shop_x_line) * shop_space_y_), 0) + shop_pos
			shop_t.transform.localScale = Vector3.one
			local name_label = shop_t.transform:Find('name'):GetComponent('UILabel')
			local price_icon = shop_t.transform:Find('price_icon'):GetComponent('UISprite')
			local frame = shop_t.transform:Find('frame'):GetComponent('UISprite')
			local bg = shop_t.transform:Find('bg'):GetComponent('UISprite')
			local price_label = shop_t.transform:Find('price'):GetComponent('UILabel')
			local res_temp = Config.get_t_resource(v.past_type)
			local icon = shop_t.transform:Find('icon'):GetComponent('UISprite')
			icon.atlas = IconPanel.GetAltas(item_temp.icon)
			icon.gameObject:SetActive(true)
			price_icon.spriteName = res_temp.small_icon
			price_label.bitmapFont = yellow_font
			local color = v.color
			if(v.past_type == 2) then
				price_label.bitmapFont = purple_font
			elseif(v.past_type == 4) then
				price_label.bitmapFont = blue_font
			end
			if(type == 3) then
				color = 4
				if(self.has_fashion(v.value1)) then
					price_icon.gameObject:SetActive(false)
					price_label.gameObject:SetActive(false)
					shop_t.transform:Find('tip').gameObject:SetActive(true)
				end
				if(item_temp.type == 2) then
					icon.transform:GetComponent("UIWidget").round = true
					icon.transform:GetComponent("UIWidget").height = 135
					icon.transform:GetComponent("UIWidget").depth = 1
				end
			end
			frame.spriteName = dj_frame[color]
			icon.spriteName = item_temp.icon
			icon.gameObject.name = v.id
			name_label.text = item_temp.name
			IconPanel.InitQualityLabel(name_label, item_temp.color % 10)
			price_label.text = v.price
			lua_script_:AddButtonEvent(icon.gameObject, "click", ShopPanel.BuyItem)
			
			if type == 3 then
				shop_t.name = v.id
			end
			
			shop_t:SetActive(true)
			i = i + 1
		end
	end
	view_max = math.floor((i - 1) / shop_x_line) * shop_space_y_ + shop_height + 20
	ShopPanel.OnDragFinished()
end

function ShopPanel.InitRechargeList()
	local i = 0
	
	local recharge_list = {}
	for k,v in pairsByKeys(Config.t_recharge) do
		if self.is_ios_sh then
			if v.check ~= 1 then
				table.insert(recharge_list,v)
			end	
		else
			if v.check ~= 2 then
				table.insert(recharge_list,v)
			end
		end
	end
	
	for k, v in pairsByKeys(recharge_list) do
		local recharge_temp = v
		local recharge_t = LuaHelper.Instantiate(recharge_res_.gameObject)
		recharge_t.transform.parent = shop_view_.transform
		recharge_t.transform.localPosition = Vector3(i % shop_x_line * shop_space_x_, -(math.floor(i / shop_x_line) * shop_space_y_), 0) + shop_pos
		recharge_t.transform.localScale = Vector3.one
		local name_label = recharge_t.transform:Find('name'):GetComponent('UILabel')
		local price_label = recharge_t.transform:Find('price'):GetComponent('UILabel')
		local icon = recharge_t.transform:Find('icon'):GetComponent('UISprite')
		local mask = recharge_t.transform:Find('mask')
		icon.spriteName = recharge_temp.icon
		icon.gameObject.name = recharge_temp.id
		name_label.text = recharge_temp.name
		IconPanel.InitQualityLabel(name_label, 2)
		price_label.text = recharge_temp.price
		icon:MakePixelPerfect()
		if tonumber(self.player.yue_time) > tonumber(timerMgr:now_string()) and recharge_temp.type == 2 then
			local time = tonumber(self.player.yue_time) - tonumber(timerMgr:now_string())
			mask:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('ShopPanel_006').."\n"..count_time_day(time, 1)
			mask.gameObject:SetActive(true)
			HallPanel.GrayLabel(name_label)
		end
		if tonumber(self.player.nian_time) > tonumber(timerMgr:now_string()) and recharge_temp.type > 1 then
			local time = tonumber(self.player.nian_time) - tonumber(timerMgr:now_string())
			mask:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('ShopPanel_006').."\n"..count_time_day(time, 1)
			mask.gameObject:SetActive(true)
			HallPanel.GrayLabel(name_label)
		end
		lua_script_:AddButtonEvent(icon.gameObject, "click", ShopPanel.Recharge)
		recharge_t:SetActive(true)
		i = i + 1
	end
	view_max = math.floor((i - 1) / shop_x_line) * shop_space_y_ + shop_height + 20
	ShopPanel.OnDragFinished()
end

-----------------------------------------------------------------------

---------------------------转盘------------------------------------------------

function ShopPanel.InitChestPanel()
	shop_arrow_.gameObject:SetActive(false)
	is_rolling = false
	if #chest_list > 0 then
		for i = 1, #chest_list do
			GameObject.Destroy(chest_list[i].gameObject)
		end
	end
	local shop_temp = Config.get_t_shop(5001)
	local res_temp = Config.get_t_resource(shop_temp.past_type)
	local open_btn = chest_panel_:Find("open_btn")
	local cost_icon = open_btn:Find("cost"):GetComponent("UISprite")
	local cost_num = open_btn:Find("cost/price"):GetComponent("UILabel")
	cost_num.bitmapFont = yellow_font
	if(shop_temp.past_type == 2) then
		cost_num.bitmapFont = purple_font
	elseif(v.past_type == 4) then
		cost_num.bitmapFont = blue_font
	end
	cost_icon.spriteName = res_temp.small_icon
	cost_num.text = shop_temp.price
	local time_label = chest_panel_:Find("time"):GetComponent("UILabel")
	local time = tonumber(self.player.last_week_time) + 7 * 86400000 - tonumber(timerMgr:now_string())
	time_label.text = Config.get_t_script_str('ShopPanel_007').." "..count_time_day(time, 1)
	if(self.player.duobao_num < 10) then
		local content = string.format(Config.get_t_script_str('ShopPanel_008'),self.font_color[1],self.font_color[6],self.player.duobao_num)
		chest_panel_:Find("num"):GetComponent("UILabel").text = content
	else
		local content = string.format(Config.get_t_script_str('ShopPanel_008'),self.font_color[1],self.font_color[4],self.player.duobao_num)
		chest_panel_:Find("num"):GetComponent("UILabel").text = content
	end
	chest_list = {}
	local refer_pos_x = -(max_x_num - 1) / 2 * chest_space
	local refer_pos_y = (max_y_num - 1) / 2 * chest_space
	for i = 1, roll_num do
		local chest_t = LuaHelper.Instantiate(chest_res_.gameObject)
		chest_t.transform.parent = chest_panel_
		chest_t.transform.localScale = Vector3.one
		local chest_pos = Vector3(0, 0, 0)
		if(i <= max_x_num) then
			chest_pos = Vector3((i - 1) * chest_space + refer_pos_x, refer_pos_y + chest_space, 0)
		elseif(i <= max_x_num + max_y_num) then
			chest_pos = Vector3(refer_pos_x + (max_x_num - 1) * chest_space, refer_pos_y - (i - max_x_num - 1) * chest_space, 0)
		elseif(i <= max_x_num * 2 + max_y_num) then
			chest_pos = Vector3(-refer_pos_x - (i - max_x_num - max_y_num - 1) * chest_space, refer_pos_y - (max_y_num) * chest_space, 0)
		else
			chest_pos = Vector3(refer_pos_x, (roll_num - i) * chest_space + refer_pos_y - (max_y_num - 1) * chest_space, 0)
		end
		chest_t.transform.localPosition = chest_pos
		local reward_temp = Config.get_t_reward(self.chest_rewards[i].type, self.chest_rewards[i].value1)
		local icon = chest_t.transform:Find("icon"):GetComponent("UISprite")
		local num = chest_t.transform:Find("num"):GetComponent("UILabel")
		local name = chest_t.transform:Find("name"):GetComponent("UILabel")
		num.text = ""
		if(self.chest_rewards[i].value2 > 0) then
			num.text = "x"..self.chest_rewards[i].value2
		end
		if(self.chest_rewards[i].type ~= 7) then
			chest_t.transform:GetComponent("UISprite").spriteName = "xyzpdb_01"
		end
		name.text = reward_temp.name
		icon.atlas = IconPanel.GetAltas(reward_temp.icon)
		icon.spriteName = reward_temp.icon
		IconPanel.InitQualityLabel(name, reward_temp.color)
		IconPanel.InitQualityLabel(num, reward_temp.color)
		icon.gameObject.name = self.chest_rewards[i].type.."+"..self.chest_rewards[i].value1
		lua_script_:AddButtonEvent(icon.gameObject, "click", ShopPanel.CheckItem)
		table.insert(chest_list, chest_t.transform)
		chest_t:SetActive(true)
	end
	chest_panel_.gameObject:SetActive(true)
end

function ShopPanel.Refresh()
	if #chest_list > 0 and is_rolling then
		roll_time = roll_time + time_speed
		if(roll_time >= roll_speed) then
			roll_time = 0
			roll_id = roll_id + 1
			lua_script_:PlaySoundEX("du")
			if(roll_id > roll_num) then
				roll_id = 1
				roll_speed = roll_speed + speed_add
				if(roll_speed >= max_speed) then
					roll_speed = max_speed
				end
			end
			local pre_id = 0
			if(roll_id == 1) then
				pre_id = roll_num
			else
				pre_id = roll_id - 1
			end
			chest_list[pre_id]:Find("select").gameObject:SetActive(false)
			chest_list[roll_id]:Find("select").gameObject:SetActive(true)
			if(roll_id == target_id and roll_speed == max_speed) then
				chest_list[roll_id]:Find("effect").gameObject:SetActive(true)
				lua_script_:PlaySoundEX("ding")
				timerMgr:RemoveRepeatTimer("ShopPanel")
				timerMgr:AddTimer("RollEnd", ShopPanel.RollEnd, 0.7)
			end
		end
	end
end

function ShopPanel.IsRolling()
	return is_rolling
end

function ShopPanel.RollEnd()
	chest_list[target_id]:Find("effect").gameObject:SetActive(false)
	if(self.player.duobao_num < 10) then
		local content = string.format(Config.get_t_script_str('ShopPanel_008'),self.font_color[1],self.font_color[6],self.player.duobao_num)
		chest_panel_:Find("num"):GetComponent("UILabel").text = content
	else
		local content = string.format(Config.get_t_script_str('ShopPanel_008'),self.font_color[1],self.font_color[4],self.player.duobao_num)
		chest_panel_:Find("num"):GetComponent("UILabel").text = content
	end
	is_rolling = false
	if(#roll_rewards > 0) then
		GUIRoot.ShowGUI('GainPanel', {roll_rewards})
	end
	if(#roll_roles > 0) then
		GUIRoot.ShowGUI("RoleGetPanel", {roll_roles, {"ShopPanel", "BackPanel"}})
	end
	BackPanel.RefreshInf()
	BackPanel.RefreshMoney()
end

function ShopPanel.StopRoll()
	ShopPanel.RollEnd()
	ShopPanel.InitChestPanel()
	chest_list[target_id]:Find("select").gameObject:SetActive(true)
end

function ShopPanel.RefreshRollReward()
	self.chest_rewards = {}
	for i = 1, 10 do
		local reward = Config.get_t_item_box(3000 + i).rewards[self.player.duobao_items[i] + 1]
		table.insert(self.chest_rewards, reward)
	end
end
-----------------------------------------------------------------------


----------------------------------ButtonEvent--------------------------

function ShopPanel.Click(obj)
	if(obj.name == "item_btn") then
		ShopPanel.SelectPage(1)
	elseif(obj.name == "role_btn") then
		ShopPanel.SelectPage(2)
	elseif(obj.name == "chest_btn") then
		ShopPanel.SelectPage(4)
	elseif(obj.name == "recharge_btn") then
		ShopPanel.SelectPage(5)
	elseif(obj.name == "effect_btn") then
		ShopPanel.SelectPage(3)
	elseif(obj.name == "open_btn") then
		if(is_rolling) then
			ShopPanel.StopRoll()
			return
		end
		if(self.player.duobao_num >= 10) then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ShopPanel_009')})
			return
		end
		local shop_temp = Config.get_t_shop(5001)
		if(self.get_resource(shop_temp.past_type) >= shop_temp.price) then
			GameTcp.Send(opcodes.CMSG_DUOBAO, nil, {opcodes.SMSG_DUOBAO})
		else
			if(shop_temp.past_type == 2) then
				if(platform_config_common.m_platform == "android_yymoon") then
					local res_temp = Config.get_t_resource(shop_temp.past_type)
					local content = string.format(Config.get_t_script_str('ShopPanel_013'),self.font_color[res_temp.color],res_temp.name)
					GUIRoot.ShowGUI("MessagePanel", {content})					
				else			
					local content = string.format(Config.get_t_script_str('ShopPanel_010'),self.font_color[2],Config.get_t_resource(2).name)
					GUIRoot.ShowGUI("SelectPanel", {content, Config.get_t_script_str('ShopPanel_011'), ShopPanel.BackRecharge, Config.get_t_script_str('ShopPanel_012')})
				end				
			else
				local res_temp = Config.get_t_resource(shop_temp.past_type)
				local content = string.format(Config.get_t_script_str('ShopPanel_013'),self.font_color[res_temp.color],res_temp.name)
				GUIRoot.ShowGUI("MessagePanel", {content})
			end
		end
	end
end

function ShopPanel.OnDragFinished()
	local uv = shop_view_:GetComponent('UIScrollView')
	local offy = math.abs(uv.panel.clipOffset.y)
	if(offy < view_max - GUIRoot.height + 40) then
		shop_arrow_.gameObject:SetActive(true)
	else
		shop_arrow_.gameObject:SetActive(false)
	end
end

function ShopPanel.SelectPage(page)
	if(is_rolling) then
		ShopPanel.StopRoll()
		return
	end
	chest_panel_.gameObject:SetActive(false)
	if(shop_view_.childCount > 0) then
		for i = 0, shop_view_.childCount - 1 do
            GameObject.Destroy(shop_view_:GetChild(i).gameObject)
        end
	end
	local uv = shop_view_:GetComponent('UIScrollView')
	if shop_view_:GetComponent('SpringPanel') ~= nil then
		shop_view_:GetComponent('SpringPanel').enabled = false
	end
	uv.panel.clipOffset = Vector2(0, 0)
	shop_view_.localPosition = Vector3(0, 0, 0)
	local fir_pos = GUIRoot.height / 2 - 160
	for i = 1, #page_btn_ do
		page_btn_[i].localPosition = Vector3(-36, fir_pos - (i - 1) * 80, 0)
		page_btn_[i]:GetComponent("UISprite").spriteName = "zuolan1"
		page_btn_[i]:Find("Label"):GetComponent("UILabel").text = "[c2e5ed]"..Config.get_t_script_str(page_text[i])
		if(i == 5 and platform_config_common.m_platform == "android_yymoon") then
			page_btn_[i]:Find("Label"):GetComponent("UILabel").text = "[c2e5ed]"..Config.get_t_script_str(page_text[i+1])
		end
		page_btn_[i]:Find('icon'):GetComponent("UISprite").spriteName = page_icon[i]
	end
	page_btn_[page].localPosition = Vector3(-16, fir_pos - (page - 1) * 80, 0)
	page_btn_[page]:GetComponent("UISprite").spriteName = "zuolan2"
	page_btn_[page]:Find('icon'):GetComponent("UISprite").spriteName = page_icon[page].."1"
	page_btn_[page]:Find("Label"):GetComponent("UILabel").text = "[eebe6c]"..Config.get_t_script_str(page_text[page])
	if(page == 5 and platform_config_common.m_platform == "android_yymoon") then
		page_btn_[page]:Find("Label"):GetComponent("UILabel").text = "[c2e5ed]"..Config.get_t_script_str(page_text[page+1])
	end
	if(page == 1) then
		ShopPanel.InitItemList(2)
	elseif(page == 2) then
		ShopPanel.InitItemList(1)
	elseif(page == 3) then
		ShopPanel.InitItemList(3)
	elseif(page == 4) then
		ShopPanel.InitChestPanel()
	elseif(page == 5) then
		if(platform_config_common.m_platform == "android_yymoon") then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ShopPanel_019')})			
		else
			if(platform_recharge._instance.Donefunc == nil) then
				platform_recharge._instance.Donefunc = ShopPanel.RechargeDone
			end
			if(platform_recharge._instance.Canelfunc == nil) then
				platform_recharge._instance.Canelfunc = ShopPanel.CanelRecharge
			end
			ShopPanel.InitRechargeList()
		end		
	end
end

function ShopPanel.RefreshEffectList()
	ShopPanel.SelectPage(3)
end

function ShopPanel.RefreshRoleList()
	ShopPanel.SelectPage(2)
end

function ShopPanel.BuyItem(obj)
	local shop_id = tonumber(obj.name)
	local shop_temp = Config.get_t_shop(shop_id)
	if(shop_temp.item_type == 7) then
		GUIRoot.ShowGUI('BuyPanel', {shop_id, nil, ShopPanel.RefreshCurrentEffectInfo})
	elseif(shop_temp.item_type == 3) then
		GUIRoot.ShowGUI('BuyPanel', {shop_id, {"ShopPanel", "BackPanel"}, ShopPanel.RefreshRoleList})
	else
		GUIRoot.ShowGUI('BuyPanel', {shop_id})
	end
end

function ShopPanel.CheckItem(obj)
	local item_id = obj.name
	GUIRoot.ShowGUI("DetailPanel", {item_id})
end

function ShopPanel.Recharge(obj)
	local id = tonumber(obj.name)
	local recharge_temp = Config.get_t_recharge(id)
	if(recharge_temp ~= nil) then
		recharge_id_ = id
		recharge_order_id = shareMgr:GetShareID(self.guid).."-"..timerMgr:now_string()
		if(recharge_temp.type == 2) then
			if tonumber(self.player.nian_time) > tonumber(timerMgr:now_string()) then
				local time = tonumber(self.player.nian_time) - tonumber(timerMgr:now_string())
				local content = string.format(Config.get_t_script_str('ShopPanel_014'),count_time_day(time, 1))
				GUIRoot.ShowGUI("MessagePanel", {content})
				return
			else
				if tonumber(self.player.yue_time) > tonumber(timerMgr:now_string()) then
					local time = tonumber(self.player.yue_time) - tonumber(timerMgr:now_string())
					local content = string.format(Config.get_t_script_str('ShopPanel_014'),count_time_day(time, 1))
					GUIRoot.ShowGUI("MessagePanel", {content})
					return
				end
			end
		elseif(recharge_temp.type == 3) then
			if tonumber(self.player.nian_time) > tonumber(timerMgr:now_string()) then
				local time = tonumber(self.player.nian_time) - tonumber(timerMgr:now_string())
				local content = string.format(Config.get_t_script_str('ShopPanel_014'),count_time_day(time, 1))
				GUIRoot.ShowGUI("MessagePanel", {content})
				return
			end
		end
		if(platform_config_common.m_platform ~= "test") then
			if(platform_config_common.m_platform == "ios_yymoon") then
				GUIRoot.ShowGUI('MaskPanel', {Config.get_t_script_str('ShopPanel_015')})
				local recharge_param =self.guid.."|"..self.player.serverid.."|"..recharge_temp.id.."|"..recharge_temp.name.."|"..recharge_temp.type.."|"..recharge_temp.check.."|"..recharge_temp.price.."|"..recharge_temp.code.."|"..recharge_temp.desc
			    platform_recharge._instance : do_buy(recharge_param, 0, 0)
			elseif (platform_config_common.m_platform == "android_yymoon") then
				GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ShopPanel_019')})
			end
		else
			print('lua test pur print')	
		end
	end
end

function ShopPanel.CanelRecharge()
	recharge_id_ = 0
	GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ShopPanel_017')})
	GUIRoot.HideGUI('MaskPanel')
end



function ShopPanel.RechargeDone(platform, code)
	if(recharge_id_ ~= 0) then
		local msg = msg_hall_pb.cmsg_recharge()
		if(platform == "android_yymoon") then
			msg.id = recharge_id_
			msg.code = code
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_RECHARGE, data, {opcodes.SMSG_RECHARGE}, Config.get_t_script_str('ShopPanel_015'), 60)
		elseif(platform == "apple") then
			local co = self.Split(code,'|')
			local c = {}
			c[1] = recharge_id_
			c[2] = co[2]
			self.ios_purchase[co[1]] = c
			purchase_id_ = co[1]
			msg.id = recharge_id_
			msg.pt = platform_config_common._instance.m_platform
			msg.code:append(co[2])
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_RECHARGE, data, {opcodes.SMSG_RECHARGE}, Config.get_t_script_str('ShopPanel_015'), 60)
		end
	end
end

function ShopPanel.CountLine()
	for i = 1, math.ceil((GUIRoot.width - 210) / shop_width) do
		if((GUIRoot.width - 210 - shop_width ) / i >= shop_width + 20) then
			shop_x_line = i + 1
		end
	end
	shop_space_x_ = math.floor((GUIRoot.width - 210 - shop_width ) / (shop_x_line - 1))
	local shop_x_pos = -(GUIRoot.width - 210) / 2 + shop_width / 2 + view_offset_x
	local shop_y_pos = (GUIRoot.height - 140) / 2 - shop_height / 2 + view_offset_y - 10
	shop_pos = Vector3(shop_x_pos + view_softness.x,  shop_y_pos - view_softness.y, 0)
	-------------------------------
end

---------------------------------------------------------


--------------------------服务器code------------------------------

function ShopPanel.SMSG_DUOBAO(message)
	timerMgr:AddRepeatTimer('ShopPanel', ShopPanel.Refresh, time_speed, time_speed)
	local msg = msg_hall_pb.smsg_duobao()
	msg:ParseFromString(message.luabuff)
	target_id = msg.id
	roll_speed = 0.01
	is_rolling = true
	roll_rewards = {}
	self.player.duobao_num = self.player.duobao_num + 1
	local shop_temp = Config.get_t_shop(5001)
	self.add_resource(shop_temp.past_type, -shop_temp.price)
	local reward = self.chest_rewards[msg.id]
	if(reward.type == 7 and self.has_fashion(reward.value1)) then
		local reward_b = {}
		reward_b.type = 1
		reward_b.value1 = 4
		reward_b.value2 = Config.get_t_fashion(reward.value1).buchang
		reward_b.value3 = 0
		table.insert(roll_rewards, reward_b)
	else
		table.insert(roll_rewards, reward)
	end
	for i = 1, #roll_rewards do
		self.add_reward(roll_rewards[i], false)
	end
end

function ShopPanel.SMSG_RECHARGE(message)
	local msg = msg_hall_pb.smsg_recharge()
	GUIRoot.HideGUI("MaskPanel")
	msg:ParseFromString(message.luabuff)
	if (platform_config_common.m_platform == "android_yymoon") then
		GUIRoot.HideGUI("MaskPanel")
	end
	if purchase_id_ ~= nil and platform_config_common.m_platform == "ios_yymoon" then
		if self.ios_purchase[purchase_id_] ~= nil then
			self.ios_purchase[purchase_id_] = nil
		end
		self.update_ios_purchase()
	end
	
	--TDGAVirtualCurrency.OnChargeSuccess(recharge_order_id)
	GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ShopPanel_018')})
	if(self.player.first_recharge == 0) then
		self.player.first_recharge = 1
	end
	local recharge_temp = Config.get_t_recharge(msg.rid)
	local date=os.date("%Y-%m-%d%H:%M:%S")
	platform._instance:on_purchase(date.."_"..self.guid,recharge_temp.price,recharge_temp.value)
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
	recharge_id_ = 0
	ShopPanel.SelectPage(5)
end

