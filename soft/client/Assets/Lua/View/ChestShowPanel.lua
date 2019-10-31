ChestShowPanel = {}

local chest_state = {
	empty = 0,
	have = 1,
	lock = 2,
	open = 3,
	unlock = 4,
	close = 5,
}

local lua_script_

local main_panel_

local chest_list_ = {}
local chest_res_
local battle_chest_

function ChestShowPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	GUIRoot.ShowGUI('BackPanel', {3})
	
	main_panel_ = obj.transform:Find("main_panel")
	
	chest_res_ = main_panel_:Find('chest_res')
	
	main_panel_:Find("Anchor_bottom_right/line"):GetComponent("UIWidget").height = GUIRoot.height - 74
	
	battle_chest_ = main_panel_:Find('battle_chest')
	
	battle_chest_:Find("icon"):GetComponent("UISprite").atlas = IconPanel.GetAltas(Config.get_t_chest(201).icon)
	battle_chest_:Find("icon"):GetComponent("UISprite").spriteName = Config.get_t_chest(201).icon
	
	local battle_chest_tip = battle_chest_:Find("tip_task"):GetComponent("UILabel")
	battle_chest_tip.text =	Config.get_t_script_str('ChestShowPanel_001') --"[3AB9FC]打开需要完成[-][57FC5B]3[-][3AB9FC]场大乱斗\n或雪地会战"
	
	lua_script_:AddButtonEvent(battle_chest_:Find("icon").gameObject, "click", ChestShowPanel.Click)
	
	timerMgr:AddRepeatTimer('ChestShowPanel', ChestShowPanel.Refresh, 0.1, 0.1)
	
	ChestShowPanel.InitChest()
	
	for i = 1, #self.player.box_ids do
		ChestShowPanel.RefreshChestPanel(i)
	end
	ChestShowPanel.InitBattleChest()
	
	local x_pos = GUIRoot.width / 2
	local y_pos = GUIRoot.height / 2
	local from = Vector3(x_pos, y_pos, 0) - Vector3(100, 135, 0)
	GUIRoot.UIEffectScalePos(main_panel_.gameObject, true, 0, from)
	
	ChestShowPanel.RegisterMessage()
end

function ChestShowPanel.RegisterMessage()
	Message.register_handle("back_panel_msg", ChestShowPanel.Back)
	Message.register_handle("back_panel_recharge", ChestShowPanel.Recharge)
	Message.register_handle("team_join_msg", ChestShowPanel.TeamJoin)
end

function ChestShowPanel.RemoveMessage()
	Message.remove_handle("back_panel_msg", ChestShowPanel.Back)
	Message.remove_handle("back_panel_recharge", ChestShowPanel.Recharge)
	Message.remove_handle("team_join_msg", ChestShowPanel.TeamJoin)
end

function ChestShowPanel.OnDestroy()
	lua_script_ = nil
	chest_list_ = {}
	ChestShowPanel.RemoveMessage()
	timerMgr:RemoveRepeatTimer("ChestShowPanel")
end

function ChestShowPanel.Back()
	GUIRoot.HideGUI('ChestShowPanel')
	GUIRoot.HideGUI("BackPanel")
	GUIRoot.ShowGUI('HallPanel')
end

function ChestShowPanel.Recharge()
	GUIRoot.HideGUI('ChestShowPanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function ChestShowPanel.TeamJoin()
	GUIRoot.HideGUI('ChestShowPanel')
end

-------------ButtonEvent--------------

function ChestShowPanel.Click(obj)
	if(obj.name == 'icon') then
		if(self.player.box_zd_opened == 0) then
			GUIRoot.ShowGUI("ChestPanel", {2, 201})
		end
	end
end

function ChestShowPanel.Refresh()
	for i = 1, #self.player.box_ids do
		ChestShowPanel.RefreshChestPanel(i)
	end
	ChestShowPanel.InitBattleChest()
end

--------------------------------------




---------------刷新界面----------------

function ChestShowPanel.InitChest()
	local first_pos = Vector3(GUIRoot.width / 2 - 380, 60, 0)
	for i = 1, 4 do
		local chest = LuaHelper.Instantiate(chest_res_.gameObject)
		chest.transform.parent = main_panel_
		chest.transform.localPosition = Vector3((i - 1) % 2 * 250, -math.floor((i - 1) / 2) * 270, 0) + first_pos
		chest.transform.localScale = Vector3.one
		local bg = chest.transform:Find("bg")
		bg.name = i
		local cost_icon = chest.transform:Find('unlock_panel/open_inf'):GetComponent('UISprite')
		cost_icon.spriteName = Config.get_t_resource(2).small_icon
		lua_script_:AddButtonEvent(bg.gameObject, "click", ChestShowPanel.OpenChest)
		table.insert(chest_list_, chest)
		chest.gameObject:SetActive(true)
	end
end

function ChestShowPanel.RefreshChestPanel(slot)
	local chest_id = self.player.box_ids[slot]
	if(#chest_list_ >= 4) then
		local chest_t = chest_list_[slot]
		if(chest_id ~= 0) then
			local chest_temp = Config.get_t_chest(chest_id)
			local icon = chest_t.transform:Find("icon"):GetComponent('UISprite')
			local name = chest_t.transform:Find("name"):GetComponent('UILabel')
			if (icon.atlas ~= IconPanel.GetAltas(chest_temp.icon)) then
				icon.atlas = IconPanel.GetAltas(chest_temp.icon)
			end
			icon.spriteName = chest_temp.icon
			name.text = chest_temp.name
			ChestShowPanel.InitNameLabel(chest_temp.id, name)
			if(self.player.box_open_slot ~= slot) then
				local time_label = chest_t.transform:Find('tip'):GetComponent('UILabel')
				local content = string.format(Config.get_t_script_str('ChestShowPanel_002'),count_time(chest_temp.get_time() * 1000))
				time_label.text = content --"[C2E5ED]解锁时间 "..count_time(chest_temp.get_time() * 1000)
				if(self.player.box_open_slot == 0) then
					ChestShowPanel.ChestState(chest_t, chest_state.have, slot)
				else
					ChestShowPanel.ChestState(chest_t, chest_state.unlock, slot)
				end
			else
				local open_chest = self.player.box_ids[self.player.box_open_slot]
				if(tonumber(timerMgr:now_string()) >= tonumber(self.player.box_open_time)) then
					ChestShowPanel.ChestState(chest_t, chest_state.open, slot)
				else
					ChestShowPanel.ChestState(chest_t, chest_state.lock, slot)
					local time_label = chest_t.transform:Find('unlock_panel/time'):GetComponent('UILabel')
					local cost_label = chest_t.transform:Find('unlock_panel/open_inf/cost'):GetComponent('UILabel')
					local time_ = tonumber(self.player.box_open_time) - tonumber(timerMgr:now_string())
					cost_label.text = self.font_color[2].."x"..math.floor((time_ + 179999) / 180000)
					time_label.text = count_time(time_)
				end
			end
		else
			ChestShowPanel.ChestState(chest_t, chest_state.empty, slot)
		end
	end
end

function ChestShowPanel.OpenChest(obj)
	local slot = tonumber(obj.name)
	local chest_id = self.player.box_ids[slot]
	if(chest_id ~= 0) then
		GUIRoot.ShowGUI('ChestPanel', {0, slot})
	end
end

function ChestShowPanel.ChestState(chest_t, state, pos)
	chest_t.transform:Find("name").gameObject:SetActive(true)
	chest_t.transform:Find("tip").gameObject:SetActive(true)
	local icon = chest_t.transform:Find("icon")
	local tip = chest_t.transform:Find("tip"):GetComponent("UILabel")
	local bg = chest_t.transform:Find(pos):GetComponent("UISprite")
	local name_bg = chest_t.transform:Find("name/bg"):GetComponent("UISprite")
	local empty = chest_t.transform:Find("Label"):GetComponent("UILabel")
	empty.text = ""
	bg.spriteName = "nxxt_004"
	name_bg.spriteName = "nxxt_005"
	icon.gameObject:SetActive(true)
	if(state == chest_state.empty) then
		icon.gameObject:SetActive(false)
		empty.text = Config.get_t_script_str('ChestShowPanel_003') --"宝箱位"
		chest_t.transform:Find("name").gameObject:SetActive(false)
		chest_t.transform:Find("tip").gameObject:SetActive(false)
		chest_t.transform:Find('unlock_panel').gameObject:SetActive(false)
		chest_t.transform:Find('open_panel').gameObject:SetActive(false)
		chest_t.transform:Find('have_panel').gameObject:SetActive(false)
	elseif (state ==  chest_state.have) then
		chest_t.transform:Find('have_panel').gameObject:SetActive(true)
		chest_t.transform:Find('unlock_panel').gameObject:SetActive(false)
		chest_t.transform:Find('open_panel').gameObject:SetActive(false)
	elseif (state == chest_state.lock) then
		tip.text = ""
		chest_t.transform:Find('unlock_panel').gameObject:SetActive(true)
		chest_t.transform:Find('open_panel').gameObject:SetActive(false)
		chest_t.transform:Find('have_panel').gameObject:SetActive(false)
	elseif (state == chest_state.open) then
		if(sound_play_time == 0) then
			lua_script_:PlaySound("show")
			sound_play_time = sound_play_time + 1
		end
		tip.text = Config.get_t_script_str('ChestShowPanel_004') --"[57FC5B]解锁完毕"
		bg.spriteName = "nxxt_002"
		name_bg.spriteName = "xbxlq_003"
		chest_t.transform:Find('open_panel').gameObject:SetActive(true)
		chest_t.transform:Find('unlock_panel').gameObject:SetActive(false)
		chest_t.transform:Find('have_panel').gameObject:SetActive(false)
	elseif (state == chest_state.unlock) then
		chest_t.transform:Find('unlock_panel').gameObject:SetActive(false)
		chest_t.transform:Find('open_panel').gameObject:SetActive(false)
		chest_t.transform:Find('have_panel').gameObject:SetActive(false)
	elseif (state == chest_state.close) then
		chest_t.transform:Find('unlock_panel').gameObject:SetActive(false)
		chest_t.transform:Find('open_panel').gameObject:SetActive(false)
		chest_t.transform:Find('have_panel').gameObject:SetActive(false)
		chest_t.transform:Find("name").gameObject:SetActive(false)
		icon.gameObject:SetActive(false)
	end
end

function ChestShowPanel.InitBattleChest()
	if(lua_script_ ~= nil) then
		local batt_num = Config.get_t_chest(201).time
		local battle_num = battle_chest_:Find("Label"):GetComponent("UILabel")
		local battle_tip = battle_chest_:Find("tip"):GetComponent("UILabel")
		local battle_slider = battle_chest_:Find("pro"):GetComponent("UISlider")
		battle_chest_:Find("open_tip").gameObject:SetActive(false)
		battle_slider.value = self.player.box_zd_num / batt_num
		if(self.player.box_zd_opened > 0) then
			battle_tip.text = Config.get_t_script_str('ChestShowPanel_005') --"[C2E5ED]今日可开启[-][f01c1c]0[-][C2E5ED]次，每日[-][57FC5B]05:00[-][C2E5ED]重置[-]"
			battle_num.text = Config.get_t_script_str('ChestShowPanel_006') --"已领取"
		else
			battle_tip.text = Config.get_t_script_str('ChestShowPanel_007') --"[C2E5ED]今日可开启[-][57FC5B]1[-][C2E5ED]次，每日[-][57FC5B]05:00[-][C2E5ED]重置[-]"
			battle_num.text = tostring(self.player.box_zd_num).."/"..tostring(batt_num)
			if(self.player.box_zd_num == batt_num) then
				battle_num.text = Config.get_t_script_str('ChestShowPanel_008') --"可领取"
				battle_chest_:Find("open_tip").gameObject:SetActive(true)
			end
		end
	end
end

function ChestShowPanel.InitNameLabel(chest_id, name_label)
	if name_label == nil then
		return
	end
	name_label.applyGradient = true
	name_label.gradientTop = Color(255/ 255,255/ 255,255/ 255)
	name_label.effectColor = Color(3 / 255, 72 / 255, 137 / 255)
	if chest_id == 1 then
		name_label.gradientBottom = Color(200 / 255, 116 / 255, 70 / 255)
	elseif chest_id == 2 then
		name_label.gradientBottom = Color(54 / 255, 202 / 255, 232 / 255)
	elseif chest_id == 3 then
		name_label.gradientBottom = Color(205 / 255, 25 / 255, 26 / 255)
	elseif chest_id == 4 or chest_id == 301 then
		name_label.gradientBottom = Color(231 / 255, 73 / 255, 248 / 255)
	elseif  chest_id == 5 or chest_id == 6 or chest_id == 201 then
		name_label.gradientBottom = Color(246 / 255, 193 / 255, 79 / 255)
		name_label.effectColor = Color(132 / 255, 106 / 255, 13 / 255)
	end
end

---------------------------------------