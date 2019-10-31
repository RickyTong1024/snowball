RolePanel = {}

local lua_script_

local role_panel_
local pack_panel_
local detail_panel_
local roleinf_panel_
local operate_panel_
local upgrade_panel_
local craft_panel_
local battle_panel_
local role_view_
local patch_label_

local star_res_
local role_res_

local view_softness = Vector2(0, 20)

local cur_role_id = 0
local panel_state = 0
local eff_times_ = 0
local space_x_ = 100
local space_y_ = 130

local y_pos = 0

local font_color_ = {}

function RolePanel.Awake(obj)
	GUIRoot.UIEffect(obj, 0)
	table.insert(font_color_, Config.get_t_lang('t_item_class01'))
	table.insert(font_color_, Config.get_t_lang('t_item_class02'))
	table.insert(font_color_, Config.get_t_lang('t_item_class03'))

	mapMgr:SetTargetCam(-0.533 * panelMgr:get_wh(), 0.6, 3)
	GUIRoot.ShowGUI('BackPanel',{3})
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	role_panel_ = obj.transform:Find('RolePanel').gameObject
	pack_panel_ = obj.transform:Find('RolePanel/Anchor_right/pack_panel')
	detail_panel_ = obj.transform:Find('RolePanel/detail_panel')
	roleinf_panel_ = obj.transform:Find('RolePanel/Anchor_top_right/roleinf_panel')
	operate_panel_ = obj.transform:Find('RolePanel/Anchor_bottom_left/operate_panel')
	patch_label_ = operate_panel_:Find('up_slider/Label')
	upgrade_panel_ = operate_panel_:Find('upgrade_panel')
	craft_panel_ = operate_panel_:Find('craft_panel')
	battle_panel_ = operate_panel_:Find('battle_panel')
	
	role_view_ = pack_panel_:Find('role_view')
	role_view_:GetComponent("UIPanel").clipSoftness = view_softness
	role_res_ = pack_panel_:Find('role_res')
	star_res_ = pack_panel_:Find('star_res')
	
	role_view_:GetComponent("UIPanel"):SetRect(0, -20, 416, GUIRoot.height - 60)
	y_pos = (GUIRoot.height - 60) / 2 - 145
	
	local up_btn = detail_panel_:Find("up_btn")
	local close_btn = detail_panel_:Find("close_btn")
	lua_script_:AddButtonEvent(up_btn.gameObject, "click", RolePanel.Upgrade)
	lua_script_:AddButtonEvent(close_btn.gameObject, "click", RolePanel.CloseDetail)
	
	detail_panel_.gameObject:SetActive(false)
	roleinf_panel_.gameObject:SetActive(false)
	operate_panel_.gameObject:SetActive(false)
	RolePanel.InitRoleList()
	lua_script_:AddEvent("OnDrag")
	RolePanel.RegisterMessage()
end

function RolePanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_ROLE_LEVELUP, RolePanel.SMSG_ROLE_LEVELUP)
	Message.register_net_handle(opcodes.SMSG_ROLE_HECHENG, RolePanel.SMSG_ROLE_HECHENG)
	Message.register_net_handle(opcodes.SMSG_ROLE_ON, RolePanel.SMSG_ROLE_ON)
	Message.register_handle("back_panel_msg", RolePanel.Back)
	Message.register_handle("back_panel_recharge", RolePanel.Recharge)
	Message.register_handle("team_join_msg", RolePanel.TeamJoin)
end

function RolePanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_ROLE_LEVELUP, RolePanel.SMSG_ROLE_LEVELUP)
	Message.remove_net_handle(opcodes.SMSG_ROLE_HECHENG, RolePanel.SMSG_ROLE_HECHENG)
	Message.remove_net_handle(opcodes.SMSG_ROLE_ON, RolePanel.SMSG_ROLE_ON)
	Message.remove_handle("back_panel_msg", RolePanel.Back)
	Message.remove_handle("back_panel_recharge", RolePanel.Recharge)
	Message.remove_handle("team_join_msg", RolePanel.TeamJoin)
end

function RolePanel.OnDestroy()
	cur_role_id = 0
	panel_state = 0
	eff_times_ = 0
	font_color_ = {}
	RolePanel.RemoveMessage()
end

function RolePanel.Back()
	if(panel_state == 1) then
		eff_times_ = 0
		panel_state = 0
		RolePanel.InitRoleList()
		operate_panel_.gameObject:SetActive(false)
		roleinf_panel_.gameObject:SetActive(false)
		pack_panel_.gameObject:SetActive(true)
	elseif(panel_state == 0) then
		cur_role_id = 0
		GUIRoot.HideGUI('BackPanel')
		GUIRoot.HideGUI('RolePanel')
		GUIRoot.ShowGUI('HallPanel')
	else
	end
end

function RolePanel.Recharge()
	GUIRoot.HideGUI('RolePanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function RolePanel.TeamJoin()
	GUIRoot.HideGUI('RolePanel')
end

---------------------------刷新界面----------------------------

function RolePanel.InitRoleList()
	if(role_view_.childCount > 0) then
		for i = 0, role_view_.childCount - 1 do
            GameObject.Destroy(role_view_:GetChild(i).gameObject)
        end
	end
	local role_list = RolePanel.RankRole()
	for k = 1, #role_list do
		local role_temp = role_list[k]
		local role_t = self.get_role_id(role_temp.id)
		local role = LuaHelper.Instantiate(role_res_.gameObject)
		role.transform.parent = role_view_
		role.transform.localPosition = Vector3((k - 1) % 4 * space_x_ - 150, -(math.floor((k - 1) / 4) * space_y_) + y_pos - view_softness.y, 0)
		role.transform.localScale = Vector3.one
		local role_icon = IconPanel.GetIcon("role_res", {"icon", RolePanel.OperateRole}, role_temp.icon, role_temp.color)
		role_icon.transform.parent = role.transform
		role_icon.name = "role_icon"
		role_icon.transform.localPosition = Vector3(0, 77, 0)
		role_icon.transform.localScale = Vector3.one
		role_icon:SetActive(true)
		----------------------------------------------------
		local name = role.transform:GetComponent('UILabel')
		local mask = role.transform:Find('mask')
		local battle_tip = role.transform:Find('battle_tip')
		local patch_slider = role.transform:Find('patch_slider'):GetComponent('UISlider')
		local patch_label = role.transform:Find('patch_slider/Label'):GetComponent('UILabel')
		local arrow = role.transform:Find('patch_slider/arrow')
		local slider_fg = role.transform:Find('patch_slider/fg')
		local anim = role.transform:Find('patch_slider'):GetComponent('Animator')
		local hasObj = role.transform:Find('has'):GetComponent('UILabel')

		local suipian_num = 0
		local suipian_cost = 0
		battle_tip.gameObject:SetActive(false)
		mask.gameObject:SetActive(false)
		suipian_num = self.get_item_num(role_temp.suipian_id)
		local level = 1
		local star = role_temp.color
		local can_up = false
		if(role_t == nil) then
			mask.gameObject:SetActive(true)
			hasObj.gameObject:SetActive(true)
			if(role_temp.get_type == 0) then
				suipian_cost = role_temp.suipian_cost
				if(suipian_num >= suipian_cost) then
					can_up = true
				end
			else
				local role_level = Config.get_t_role_level(2)
				suipian_cost = role_level.suipian_cost
			end
		else
			hasObj.gameObject:SetActive(false)
			level = role_t.level
			star = level
			local role_level = Config.get_t_role_level(role_t.level + 1)
			if(role_level ~= nil) then
				suipian_cost = role_level.suipian_cost
				local gold_cost_num = role_level.get_gold_cost(role_temp.color)
				if(suipian_num >= suipian_cost) then
					arrow:GetComponent('UISprite').spriteName = 'jiantou-g'
					slider_fg:GetComponent('UISprite').spriteName = 'jdt-14-green'
					if(self.player.gold >= gold_cost_num) then
						can_up = true
					end
				end
			end
			if(role_t.guid == self.player.role_on) then
				battle_tip.gameObject:SetActive(true)
			end
		end
		
		patch_label.text = ''
		
		if(suipian_cost ~= 0) then
			patch_slider.value = suipian_num / suipian_cost
			patch_label.text = tostring(suipian_num).."/"..tostring(suipian_cost)
		else
			patch_slider.value = 1	
			patch_label.text = Config.get_t_script_str('RolePanel_001')--"满星"
		end
		
		
		if(role_temp.get_type ~= 0 and role_t == nil) then
			--patch_slider.value = 1
			if(role_temp.get_type == 1) then
				if(self.player.level < role_temp.suipian_cost) then
					hasObj.text = string.format(Config.get_t_script_str('RolePanel_002'),role_temp.suipian_cost) --多少级领取
				else
					can_up = true	
					hasObj.text = Config.get_t_script_str('RolePanel_003') --"可领取"
				end
			elseif(role_temp.get_type == 2) then
				hasObj.text = Config.get_t_script_str('RolePanel_004') --"商城购买"
			elseif(role_temp.get_type == 3) then
				hasObj.text = Config.get_t_script_str('RolePanel_005') --"首充礼包"
			elseif(role_temp.get_type == 4) then
				hasObj.text = Config.get_t_script_str('RolePanel_006') --"福利礼包"
			end
		end
		role.transform:Find('role_icon/icon').name = role_temp.id
		name.text = role_temp.name
		IconPanel.InitQualityLabel(name, role_temp.color % 10)
		local fir_pos = -(star - 1) / 2 * 12
		for i = 1, star do
			local star = LuaHelper.Instantiate(star_res_.gameObject)
			star.transform.parent = role.transform
			star.transform.localPosition = Vector3(fir_pos + (i - 1) * 12, 45, 0)
			star.transform.localScale = Vector3.one
			star:GetComponent("UISprite").depth = star:GetComponent("UISprite").depth + i
			star:SetActive(true)
		end
		-------------------------------------------------------------------
		role:SetActive(true)
		if(not can_up) then
			anim.enabled = false
		else
			arrow:GetComponent('UISprite').spriteName = 'jiantou-g'
			slider_fg:GetComponent('UISprite').spriteName = 'jdt-14-green'
		end
	end
	pack_panel_.gameObject:SetActive(true)
end

function RolePanel.InitRoleInf(role_temp, role)
	local role_icon = roleinf_panel_:Find('role_icon')
	local star_root = role_icon:Find("icon")
	if(star_root.childCount > 0) then
		for i = 0, star_root.childCount - 1 do
            GameObject.Destroy(star_root:GetChild(i).gameObject)
        end
	end
	local sex_icon = roleinf_panel_:Find('sex_icon'):GetComponent('UISprite')
	local battle_tip = roleinf_panel_:Find('role_icon/battle_tip')
	local role_name = roleinf_panel_:Find('name'):GetComponent('UILabel')
	local quality = roleinf_panel_:Find('quality'):GetComponent('UILabel')
	local role_desc = roleinf_panel_:Find('desc'):GetComponent('UILabel')
	local halo_skill_desc = roleinf_panel_:Find('halo_skill/desc'):GetComponent('UILabel')
	local bat_skill_desc = roleinf_panel_:Find('bat_skill/desc'):GetComponent('UILabel')
	local halo_skill_lv = roleinf_panel_:Find('halo_skill/Label'):GetComponent('UILabel')
	local bat_skill_lv = roleinf_panel_:Find('bat_skill/Label'):GetComponent('UILabel')
	battle_tip.gameObject:SetActive(false)
	IconPanel.ModifyIcon(role_icon, role_temp.icon, role_temp.color)
	if(role_temp.sex == 0) then
		sex_icon.spriteName = 'boy_icon'
	else
		sex_icon.spriteName = 'girl_icon'
	end
	role_name.text = role_temp.name
	IconPanel.InitQualityLabel(role_name, role_temp.color % 10)
	local role_level = 1
	local star = role_temp.color
	quality.text = font_color_[role_temp.color]
	role_desc.text = role_temp.desc
	local can_up = false
	if(role ~= nil) then
		star = role.level
		role_level = role.level
		if(role.guid == self.player.role_on) then
			battle_tip.gameObject:SetActive(true)
		end
		local role_lev = Config.get_t_role_level(role.level + 1)
		local suipian_num = self.get_item_num(role_temp.suipian_id)
		if(role_lev ~= nil) then
			local gold_cost = role_lev.get_gold_cost(role_temp.color)
			if(suipian_num >= role_lev.suipian_cost and self.player.gold >= gold_cost) then
				can_up = true
			end
		end
	end
	local fir_pos = -(star - 1) / 2 * 12
	for i = 1, star do
		local star = LuaHelper.Instantiate(star_res_.gameObject)
		star.transform.parent = star_root
		star.transform.localPosition = Vector3(fir_pos + (i - 1) * 12, -32, 0)
		star.transform.localScale = Vector3.one
		star:GetComponent("UISprite").depth = star:GetComponent("UISprite").depth + i
		star:SetActive(true)
	end
	local skill_t = ""
	for i = 1, #role_temp.gskills do
		local halo_temp = Config.get_t_role_buff(role_temp.gskills[i])
		local desc_t = string.gsub(halo_temp.desc, "{N1}", halo_temp.param_value(star))
		desc_t = string.gsub(desc_t, "{N2}", halo_temp.param4)
 		skill_t = skill_t..self.font_color[role_temp.color]..halo_temp.name.."[-]"..desc_t
	end
	halo_skill_desc.text = skill_t
	skill_t = ""
	for i = 1, #role_temp.bskills do
		local skill_temp = Config.get_t_role_skill(role_temp.bskills[i])
		local attr = skill_temp.attrs[1]
		local jichu_value = attr.param_value(star)
		local add_value = attr.param4
		if(attr.param1 == 87) then
			jichu_value = jichu_value / 100
			add_value = add_value / 100
		end
		local desc_t = string.gsub(skill_temp.desc, "{N1}", jichu_value)
		desc_t = string.gsub(desc_t, "{N2}", add_value)
 		skill_t = skill_t..self.font_color[role_temp.color]..skill_temp.name.."[-]"..desc_t
	end
	bat_skill_desc.text = skill_t
	halo_skill_lv.text = "Lv."..(star)
	bat_skill_lv.text = "Lv."..(star - role_temp.color + 1)
	RolePanel.ShowPropertyValue(role_temp, can_up, role_level)
end

function RolePanel.InitHECHENGPanel(role_temp)
	battle_panel_.gameObject:SetActive(false)
	craft_panel_.gameObject:SetActive(false)
	upgrade_panel_.gameObject:SetActive(false)
	local arrow = operate_panel_:Find('up_slider/arrow')
	local full_tip = operate_panel_:Find('full_tip')
	local slider_fg = operate_panel_:Find('up_slider/fg')
	local patch_slider = operate_panel_:Find('up_slider'):GetComponent('UISlider')
	local patch_label = operate_panel_:Find('up_slider/Label'):GetComponent('UILabel')
	local effect = operate_panel_:Find('up_slider/effec'):GetComponent("Particle2D")
	arrow:GetComponent('UISprite').spriteName = 'jiantou-b'
	slider_fg:GetComponent('UISprite').spriteName = 'jdt-30-blue'
	full_tip.gameObject:SetActive(false)
	local suipian_num = self.get_item_num(role_temp.suipian_id)
	local suipian_cost = role_temp.suipian_cost
	local craft_btn = craft_panel_:Find('craft_btn')
	craft_btn.gameObject:SetActive(true)
	lua_script_:AddButtonEvent(craft_btn.gameObject, "click", RolePanel.HECHENG)
	operate_panel_.gameObject:SetActive(true)
	local can_get = false
	if(role_temp.get_type == 0) then
		craft_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('RolePanel_007')--"合成"
		patch_label.text = tostring(suipian_num).."/"..tostring(suipian_cost)
		patch_slider.value = suipian_num / suipian_cost
		if(suipian_num >= suipian_cost) then
			craft_btn:GetComponent("UISprite").spriteName = "b1_green"
			can_get = true
		else
			craft_btn:GetComponent("UISprite").spriteName = "b1_gray"
		end
	else
		patch_slider.value = suipian_num / suipian_cost
		if(role_temp.get_type == 1) then
			craft_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('RolePanel_008') --"前往领取"
			if(self.player.level >= role_temp.suipian_cost) then
				can_get = true
				patch_label.text = Config.get_t_script_str('RolePanel_003') --"可领取"
				craft_btn:GetComponent("UISprite").spriteName = "b1_green"
			else
				craft_btn:GetComponent("UISprite").spriteName = "b1_gray"
				patch_label.text = string.format(Config.get_t_script_str('RolePanel_002'),role_temp.suipian_cost) --.. --"级领取"
			end
		elseif(role_temp.get_type == 2) then
			craft_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('RolePanel_009') --"购买"
			craft_btn:GetComponent("UISprite").spriteName = "b1_green"
			patch_label.text = Config.get_t_script_str('RolePanel_004') --"商城购买"
		elseif(role_temp.get_type == 3) then
			craft_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('RolePanel_010') --"前往"
			craft_btn:GetComponent("UISprite").spriteName = "b1_green"
			patch_label.text = Config.get_t_script_str('RolePanel_005') --"首充礼包"
		elseif(role_temp.get_type == 4) then
			craft_btn:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('RolePanel_010') --"前往"
			craft_btn:GetComponent("UISprite").spriteName = "b1_green"
			patch_label.text = Config.get_t_script_str('RolePanel_006') --"福利礼包"
		end
	end
	if(can_get) then
		arrow:GetComponent('UISprite').spriteName = 'jiantou-g'
		slider_fg:GetComponent('UISprite').spriteName = 'jdt-30-green'
		patch_slider.transform:GetComponent('Animator'):Play('role_up')
		effect.sprites:RemoveAt(0)
		effect.sprites:Add("lizi")
	else
		effect.sprites:RemoveAt(0)
		effect.sprites:Add("lizi3")
		patch_slider.transform:GetComponent('Animator'):Play('role_normal')
	end
	craft_panel_.gameObject:SetActive(true)
end

function RolePanel.InitUpgradePanel(role_temp, role)
	craft_panel_.gameObject:SetActive(false)
	upgrade_panel_.gameObject:SetActive(false)
	local arrow = operate_panel_:Find('up_slider/arrow')
	local full_tip = operate_panel_:Find('full_tip')
	local slider_fg = operate_panel_:Find('up_slider/fg')
	local patch_slider = operate_panel_:Find('up_slider'):GetComponent('UISlider')
	local path_label = operate_panel_:Find('up_slider/Label'):GetComponent('UILabel')
	local effect = operate_panel_:Find('up_slider/effec'):GetComponent("Particle2D")
	local suipian_num = self.get_item_num(role_temp.suipian_id)
	local role_level = Config.get_t_role_level(role.level + 1)
	arrow:GetComponent('UISprite').spriteName = 'jiantou-b'
	slider_fg:GetComponent('UISprite').spriteName = 'jdt-30-blue'
	full_tip.gameObject:SetActive(false)
	local is_play = false
	if(role_level ~= nil) then
		local suipian_cost = role_level.suipian_cost
		local gold_cost_num = role_level.get_gold_cost(role_temp.color)
		if(suipian_num >= suipian_cost) then
			arrow:GetComponent('UISprite').spriteName = 'jiantou-g'
			slider_fg:GetComponent('UISprite').spriteName = 'jdt-30-green'
			if(self.player.gold >= gold_cost_num) then
				is_play = true
			end
		end
		path_label.text = tostring(suipian_num).."/"..tostring(suipian_cost)
		patch_slider.value = suipian_num / suipian_cost
		local upgrade_btn = upgrade_panel_:Find('upgrade_btn')
		lua_script_:AddButtonEvent(upgrade_btn.gameObject, "click", RolePanel.CheckDetail)
		upgrade_panel_.gameObject:SetActive(true)
	else
		arrow.gameObject:SetActive(false)
		full_tip.gameObject:SetActive(true)
		path_label.text = suipian_num
		patch_slider.value = 1
	end
	operate_panel_.gameObject:SetActive(true)
	if(is_play) then
		effect.sprites:RemoveAt(0)
		effect.sprites:Add("lizi")
		arrow.gameObject:SetActive(true)
		patch_slider.transform:GetComponent('Animator'):Play('role_up')
	else
		effect.sprites:RemoveAt(0)
		effect.sprites:Add("lizi3")
		patch_slider.transform:GetComponent('Animator'):Play('role_normal')
	end
end

function RolePanel.InitbattlePanel(role)
	craft_panel_.gameObject:SetActive(false)
	local battle_btn = battle_panel_:Find('battle_btn')
	local battle_tip = battle_panel_:Find('battle_tip')
	battle_btn.gameObject:SetActive(false)
	battle_tip.gameObject:SetActive(false)
	if(role.guid == self.player.role_on) then
		battle_tip.gameObject:SetActive(true)
	else
		battle_btn.gameObject:SetActive(true)
		lua_script_:AddButtonSoundEvent(battle_btn.gameObject, "click", RolePanel.Batlle, "ready")
	end
	battle_panel_.gameObject:SetActive(true)
end

function RolePanel.ShowPropertyValue(role_temp, can_up, role_level)
	local role_hp = roleinf_panel_:Find('hp_slider'):GetComponent('UISlider')
	local role_hp_text = roleinf_panel_:Find('hp_slider/Label'):GetComponent("UILabel")
	local role_atk = roleinf_panel_:Find('atk_slider'):GetComponent('UISlider')
	local role_atk_text = roleinf_panel_:Find('atk_slider/Label'):GetComponent("UILabel")
	local role_def = roleinf_panel_:Find('def_slider'):GetComponent('UISlider')
	local role_def_text = roleinf_panel_:Find('def_slider/Label'):GetComponent("UILabel")
	local role_speed = roleinf_panel_:Find('speed_value'):GetComponent('UISlider')
	local role_speed_text = roleinf_panel_:Find('speed_value/Label'):GetComponent('UILabel')
	local atk_range = roleinf_panel_:Find('atk_range'):GetComponent('UISlider')
	local atk_range_text = roleinf_panel_:Find('atk_range/Label'):GetComponent('UILabel')
	role_speed.value = 1 - role_temp.speed / 40000
	atk_range.value = 1 - role_temp.range / 50000
	role_speed_text.text = role_temp.speed / 100
	atk_range_text.text = role_temp.range / 100
	local max_hp_value = role_temp.hp + (Config.max_role_lev - 1) * role_temp.hp_add
	local max_atk_value = role_temp.atk + (Config.max_role_lev - 1) * role_temp.atk_add
	local max_def_value = role_temp.def + (Config.max_role_lev - 1) * role_temp.def_add
	
	local role_hp_value = role_temp.hp + (role_level - 1) * role_temp.hp_add
	local role_atk_value = role_temp.atk + (role_level - 1) * role_temp.atk_add
	local role_def_value = role_temp.def + (role_level - 1) * role_temp.def_add
	role_hp.value = 1 -  role_hp_value / max_hp_value
	role_atk.value = 1 - role_atk_value / max_atk_value
	role_def.value = 1 - role_def_value / max_def_value
	role_hp_text.text = role_hp_value
	role_atk_text.text = role_atk_value
	role_def_text.text = role_def_value
end

function RolePanel.InitDetailPanel(role_temp, role)
	local role_icon = detail_panel_:Find("role_icon/icon"):GetComponent("UISprite")
	if(role_icon.transform.childCount > 0) then
		for i = 0, role_icon.transform.childCount - 1 do
            GameObject.Destroy(role_icon.transform:GetChild(i).gameObject)
        end
	end
	local role_name = detail_panel_:Find("name"):GetComponent("UILabel")
	local battle_tip = detail_panel_:Find("role_icon/battle_tip")
	local hp_value = detail_panel_:Find("hp_value/Label"):GetComponent("UILabel")
	local hp_add = detail_panel_:Find("hp_value/add"):GetComponent("UILabel")
	local atk_value = detail_panel_:Find("atk_value/Label"):GetComponent("UILabel")
	local atk_add = detail_panel_:Find("atk_value/add"):GetComponent("UILabel")
	local def_value = detail_panel_:Find("def_value/Label"):GetComponent("UILabel")
	local def_add = detail_panel_:Find("def_value/add"):GetComponent("UILabel")
	local halo_value = detail_panel_:Find("halo_skill/Label"):GetComponent("UILabel")
	local halo_add = detail_panel_:Find("halo_skill/add"):GetComponent("UILabel")
	local bat_value = detail_panel_:Find("bat_skill/Label"):GetComponent("UILabel")
	local bat_add = detail_panel_:Find("bat_skill/add"):GetComponent("UILabel")
	--local exp_add = detail_panel_:Find("exp_add/Label"):GetComponent("UILabel")
	local gold_cost = detail_panel_:Find("gold_cost/Label"):GetComponent("UILabel")
	local up_btn = detail_panel_:Find("up_btn"):GetComponent("UISprite")
	battle_tip.gameObject:SetActive(false)
	IconPanel.ModifyIcon(detail_panel_:Find("role_icon"), role_temp.icon, role_temp.color)
	role_name.text = role_temp.name
	IconPanel.InitQualityLabel(role_name, role_temp.color % 10)
	if(role.guid == self.player.role_on) then
		battle_tip.gameObject:SetActive(true)
	end
	hp_value.text = role_temp.hp + (role.level - 1) * role_temp.hp_add
	hp_add.text = role_temp.hp + role.level * role_temp.hp_add
	atk_value.text = role_temp.atk + (role.level - 1) * role_temp.atk_add
	atk_add.text = role_temp.atk + role.level * role_temp.atk_add
	def_value.text = role_temp.def + (role.level - 1) * role_temp.def_add
	def_add.text = role_temp.def + role.level * role_temp.def_add
	halo_value.text = "Lv."..(role.level)
	halo_add.text = "Lv."..(role.level + 1)
	bat_value.text = "Lv."..(role.level - role_temp.color + 1)
	bat_add.text = "Lv."..(role.level - role_temp.color + 2)
	local role_level = Config.get_t_role_level(role.level + 1)
	--exp_add.text = "+"..role_level.exp[role_temp.color]
	local gold_cost_num = role_level.get_gold_cost(role_temp.color)
	local suipian_cost = role_level.suipian_cost
	local suipian_num = self.get_item_num(role_temp.suipian_id)
	if(suipian_num >= suipian_cost) then
		up_btn.spriteName = "b1_green"
	else
		up_btn.spriteName = "b1_gray"
	end
	if(self.player.gold >= gold_cost_num) then
		gold_cost.text = "[fcf36d]"..gold_cost_num
	else
		gold_cost.text = "[e04b4d]"..gold_cost_num
	end
	local fir_pos = -(role.level) / 2 * 12
	for i = 1, role.level + 1 do
		local star = LuaHelper.Instantiate(star_res_.gameObject)
		star.transform.parent = role_icon.transform
		star.transform.localPosition = Vector3(fir_pos + (i - 1) * 12, -32, 0)
		star.transform.localScale = Vector3.one
		star:GetComponent("UISprite").depth = star:GetComponent("UISprite").depth + i
		if(i == role.level + 1) then
			star.transform:GetComponent("TweenAlpha").enabled = true
		end
		star:SetActive(true)
	end
	GUIRoot.UIEffectScalePos(detail_panel_.gameObject, true, 1)
	detail_panel_.gameObject:SetActive(true)
end

----------------------------------------------------------------


---------------------------ButtonEvent--------------------------

function RolePanel.OnDrag(delta)
	HallScene.Roll(delta.x)
end

function RolePanel.OperateRole(obj)
	cur_role_id = tonumber(obj.name)
	local role_temp = Config.get_t_role(tonumber(obj.name))
	local role = self.get_role_id(role_temp.id)
	HallScene.AddRole(role_temp)
	RolePanel.InitRoleInf(role_temp, role)
	if role ~= nil then
		RolePanel.InitUpgradePanel(role_temp, role)
		RolePanel.InitbattlePanel(role)
	else
		RolePanel.InitHECHENGPanel(role_temp)
	end
	operate_panel_.gameObject:SetActive(true)
	pack_panel_.gameObject:SetActive(false)
	roleinf_panel_.gameObject:SetActive(true)
---------------------------------------------------------------------------------------
	panel_state = 1
end

function RolePanel.Batlle()
	if(panel_state == 1) then
		local role = self.get_role_id(cur_role_id)
		if(role ~= nil) then
			local msg = msg_hall_pb.cmsg_role_on()
			msg.role_guid = role.guid
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_ROLE_ON, data, {opcodes.SMSG_ROLE_ON})
		else
			GUIRoot.ShowGUI('MessagePanel', {Config.get_t_script_str('RolePanel_015')})
		end
	end
end

function RolePanel.Upgrade()
	if(panel_state == 1) then
		local role = self.get_role_id(cur_role_id)
		if(role ~= nil) then
			local role_temp = Config.get_t_role(role.template_id)
			local role_level = Config.get_t_role_level(role.level + 1)
			local suipian_num = self.get_item_num(role_temp.suipian_id)
			local suipian_cost = role_level.suipian_cost
			local gold_cost = role_level.get_gold_cost(role_temp.color)
			if(self.player.gold >= gold_cost and suipian_num >= suipian_cost) then
				local msg = msg_hall_pb.cmsg_role_levelup()
				msg.role_guid = role.guid
				local data = msg:SerializeToString()
				GameTcp.Send(opcodes.CMSG_ROLE_LEVELUP, data, {opcodes.SMSG_ROLE_LEVELUP})
			else
				if(suipian_num < suipian_cost) then
					GUIRoot.ShowGUI('MessagePanel', {Config.get_t_script_str('RolePanel_011')})
				elseif(self.player.gold < gold_cost) then
					GUIRoot.ShowGUI('MessagePanel', {Config.get_t_script_str('RolePanel_012')})
				end
			end
		end
		detail_panel_.gameObject:SetActive(false)
	end
end

function RolePanel.HECHENG()
	if(panel_state == 1) then
		local role_temp = Config.get_t_role(cur_role_id)
		local suipian_num = self.get_item_num(role_temp.suipian_id)
		local suipian_cost = role_temp.suipian_cost
		if(role_temp.get_type == 0) then
			if suipian_num >= suipian_cost then
				local msg = msg_hall_pb.cmsg_role_hecheng()
				msg.role_id = role_temp.id
				local data = msg:SerializeToString()
				GameTcp.Send(opcodes.CMSG_ROLE_HECHENG, data, {opcodes.SMSG_ROLE_HECHENG})
			else
				GUIRoot.ShowGUI('MessagePanel', {Config.get_t_script_str('RolePanel_013')})
			end
		elseif(role_temp.get_type == 1) then
			if(self.player.level >= role_temp.suipian_cost) then
				GUIRoot.HideGUI("RolePanel")
				GUIRoot.ShowGUI('LevelTask', {role_temp.suipian_cost})
			else
				local content = string.format(Config.get_t_script_str('RolePanel_014'),role_temp.suipian_cost)
				GUIRoot.ShowGUI('MessagePanel', {content})
			end
		elseif(role_temp.get_type == 2) then
			GUIRoot.ShowGUI("BuyPanel", {role_temp.suipian_cost, {"RolePanel", "BackPanel"}, RolePanel.BuyRoleFinish})
		else
			GUIRoot.HideGUI('BackPanel')
			GUIRoot.HideGUI('RolePanel')
			GUIRoot.ShowGUI('HallPanel')
			HallPanel.RoleToHall(role_temp.get_type)
		end
	end
end

function RolePanel.BuyRoleFinish()
	if(lua_script_ ~= nil) then
		local role_temp = Config.get_t_role(cur_role_id)
		local role = self.get_role_id(role_temp.id)
		RolePanel.InitUpgradePanel(role_temp, role)
		RolePanel.InitbattlePanel(role)
	end
end

function RolePanel.CheckDetail()
	local role_temp = Config.get_t_role(cur_role_id)
	local role = self.get_role_id(role_temp.id)
	RolePanel.InitDetailPanel(role_temp, role)
end

function RolePanel.CloseDetail()
	detail_panel_.gameObject:SetActive(false)
end
------------------------------------------------------------------


----------------------------服务器code----------------------------

function RolePanel.SMSG_ROLE_ON()
	if(panel_state == 1) then
		local role = self.get_role_id(cur_role_id)
		self.role_on(role)
		local role_temp = Config.get_t_role(role.template_id)
		RolePanel.InitRoleInf(role_temp, role)
		RolePanel.InitbattlePanel(role)
	end
end

function RolePanel.SMSG_ROLE_LEVELUP()
	if(panel_state == 1) then
		local role = self.get_role_id(cur_role_id)
		self.role_upgrage(role)
		self.add_all_type_num(36, 1)
		role = self.get_role_id(cur_role_id)
		platform._instance:on_game_user_upgrade(role.level)
		if(role ~= nil) then
			local role_temp = Config.get_t_role(role.template_id)
			local role_level = Config.get_t_role_level(role.level)
			local suipian_cost = role_level.suipian_cost
			local suipian_num = self.get_item_num(role_temp.suipian_id)
			local gold_cost = role_level.get_gold_cost(role_temp.color)
			if(self.player.gold >= gold_cost and suipian_num >= suipian_cost) then
				self.add_resource(1, -gold_cost)
				self.delete_item_num(role_temp.suipian_id, suipian_cost)
				--local exp_add = role_level.exp[role_temp.color]
				--self.add_effect_resource(3, exp_add, 20, patch_label_, 1)
				RolePanel.ShowEffect()
				---------------------------------------------------------------------------------
				RolePanel.InitRoleInf(role_temp, role)
				RolePanel.InitUpgradePanel(role_temp, role)
				--
				LuaAchieve.RoleStarLevelUp()				
			end
		end
		
	end
end

function RolePanel.SMSG_ROLE_HECHENG(message)
	if(panel_state == 1) then
		local msg = msg_hall_pb.smsg_role_hecheng()
		msg:ParseFromString(message.luabuff)
		local role = msg.role
		self.add_role(role)
		local role_temp = Config.get_t_role(cur_role_id)
		local suipian_num = self.get_item_num(role_temp.suipian_id)
		local suipian_cost = role_temp.suipian_cost
		if(suipian_num >= suipian_cost) then
			--local exp_add = role_temp.exp
			--self.add_resource(3, exp_add)
			self.delete_item_num(role_temp.suipian_id, suipian_cost)
			RolePanel.InitRoleInf(role_temp, role)
			RolePanel.InitUpgradePanel(role_temp, role)
			RolePanel.InitbattlePanel(role)
			--
			LuaAchieve.RoleAmountChange(true)
		end
		GUIRoot.ShowGUI("RoleGetPanel", {{msg.role}, {"RolePanel", "BackPanel"}})
	end
end

-------------------------------------------------------------------


----------------------------Function-------------------------------

function RolePanel.ShowEffect()
	local eff = resMgr:CreateEffect("Unit_createCharacter",false)
	local efft = eff.transform
	efft.parent = resMgr.UnitRoot
	efft.localPosition = Vector3(0, 0, 0)
	efft.localEulerAngles = Vector3(0, 0, 0)
	efft.localScale = Vector3.one
	GameObject.Destroy(eff, 3)
end

function RolePanel.ComPareRole(role_t_one, role_t_two)
	local role_one = self.get_role_id(role_t_one.id)
	local role_two = self.get_role_id(role_t_two.id)
	if(role_one == nil and role_two == nil or role_one ~= nil and role_two ~= nil) then
		if(role_t_one.color > role_t_two.color) then
			return -1
		else
			return 1
		end
	end
	if(role_one ~= nil and role_two == nil) then
		return 1
	end
	if(role_one == nil and role_two ~= nil) then
		return -1
	end
end

function RolePanel.RankRole()
	local role_list = {}
	for k, v in pairsByKeys(Config.t_role) do
		if v.type ~= 1 and v.get_type ~= 99 then
			table.insert(role_list, v)
		end
	end
	for i = 1, #role_list do
		for j = 1, #role_list - i do
			local result = RolePanel.ComPareRole(role_list[j], role_list[j + 1])
			if(result == -1) then
				local role_tp =  role_list[j + 1]
				role_list[j + 1] = role_list[j]
				role_list[j] = role_tp
			end
		end
	end
	return role_list
end