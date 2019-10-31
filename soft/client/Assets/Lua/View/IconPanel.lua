IconPanel = {}
local lua_script_

local lua_script_
local icon_list_ = {}
local eff_list = {}
local altas_list = {}
local eff_num = 0
local frame_color_ = {'djframe-blue', 'djframe-purple', 'djframe-y'}
local frame_color_two_ = {'djframe-blue_sp', 'djframe-purple_sp', 'djframe-y_sp'}
local icon_panel_

function IconPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	icon_panel_ = obj.transform
	if(icon_panel_.childCount > 0) then
		for i = 0, icon_panel_.childCount - 1 do
			icon_list_[icon_panel_:GetChild(i).name] = icon_panel_:GetChild(i).gameObject
		end
	end
	local altas_root = icon_panel_:Find("altas_root")
	if(altas_root.childCount > 0) then
		for i = 0, altas_root.childCount - 1 do
			table.insert(altas_list, altas_root:GetChild(i):GetComponent("UISprite").atlas)
		end
	end
	UpdateBeat:Add(IconPanel.RefreshEffect, IconPanel)
end

function IconPanel.OnDestroy()
	icon_list_ = {}
	altas_list = {}
end

function IconPanel.GetIcon(icon_name, click_parm, icon_, frame, num)
	if(icon_list_[icon_name] ~= nil) then
		local icon_res = LuaHelper.Instantiate(icon_list_[icon_name])
		local icon = icon_res.transform:Find('icon'):GetComponent('UISprite')
		icon.atlas = IconPanel.GetAltas(icon_)
		icon.spriteName = icon_
		if(click_parm ~= nil) then
			local click_obj = icon_res.transform:Find(click_parm[1])
			lua_script_:AddButtonEvent(click_obj.gameObject, "click", click_parm[2])
		end
		if(num ~= nil) then
			local num_label = icon_res.transform:Find('num'):GetComponent('UILabel')
			if(num > 1) then
				num_label.gameObject:SetActive(true)
				num_label.text = num
			else
				num_label.gameObject:SetActive(false)
			end
		end
		if(frame ~= 0) then
			local frame_icon = icon_res.transform:Find('frame'):GetComponent('UISprite')
			if(frame > 10) then
				frame_icon.spriteName = frame_color_two_[frame % 10]
			else
				frame_icon.spriteName = frame_color_[frame]
			end
		end
		if(icon_name == "reward_res" or icon_name == "gain_reward_res") then
			lua_script_:AddButtonEvent(icon.gameObject, "click", IconPanel.CheckItem)
		end
		return icon_res
	else
		return nil
	end
end

function IconPanel.GetCup(id, is_show, is_big)
	if(is_show == nil) then
		is_show = false
	end
	if(is_big == nil) then
		is_big = false
	end
	local star_radio = 6125
	local star_y_offset = 40
	local pos_ref = 40
	local cup_t = nil
	if(is_big) then
		star_y_offset = 80
		pos_ref = 60
		star_radio = math.pow(125, 2)
		cup_t = LuaHelper.Instantiate(icon_list_["cup_big_res"])
	else
		cup_t = LuaHelper.Instantiate(icon_list_["cup_res"])
	end
	local cup_temp = Config.get_t_cup(id)
	 
	local star_res = cup_t.transform:Find("star_res")
	local icon = cup_t.transform:GetComponent('UISprite')
	local name = cup_t.transform:Find("name"):GetComponent("UILabel")
	icon.atlas = IconPanel.GetAltas(cup_temp.icon)
	icon.spriteName = cup_temp.icon
	name.text = cup_temp.name
	cup_t.transform:Find('bg'):GetComponent('UISprite').spriteName = cup_temp.dai_icon
	IconPanel.InitCupLabel(cup_temp.dai_icon, name)
	if(Config.get_t_cup(id + 1).id ~= id + 1) then
		local star = LuaHelper.Instantiate(star_res.gameObject)
		star.transform.parent = cup_t.transform
		star.transform.localScale = Vector3.one
		star.transform.localPosition = Vector3(0, math.sqrt(star_radio - math.pow(0, 2)) + star_y_offset, 0)
		star:SetActive(true)
		local lv = nil
		if(is_big) then
			lv = cup_t.transform:Find("lv/lv_view/Label"):GetComponent("UILabel")
		else
			lv = cup_t.transform:Find("lv/Label"):GetComponent("UILabel")
		end
		lv.text = id - cup_temp.id + cup_temp.star
		IconPanel.InitCupLabel(cup_temp.dai_icon, lv)
		cup_t.transform:Find("lv").gameObject:SetActive(true)
	else
		local pos_per = 0
		if(cup_temp.max_star < 5) then
			pos_per = pos_ref
		else
			pos_per = pos_ref - pos_ref / 8
		end
		local fir_pos = -(cup_temp.max_star - 1) / 2 * pos_per
		for k = 1, cup_temp.max_star do
			local star = LuaHelper.Instantiate(star_res.gameObject)
			star.transform.parent = cup_t.transform
			star.transform.localScale = Vector3.one
			star.name = k
			local sj_pos = (5 - cup_temp.max_star) + k
			local x_pos = fir_pos + (k - 1) * pos_per
			if(cup_temp.max_star >= 5 and k % 2 == 0) then
				x_pos = (k - 3) * pos_ref
			end
			star.transform.localEulerAngles = Vector3(0, 0, -x_pos / 2)
			local y_pos = math.sqrt(star_radio - math.pow(x_pos, 2)) + star_y_offset
			star.transform.localPosition = Vector3(x_pos, y_pos, 0)
			if(cup_temp.star < k and not is_show) then
				star.transform:GetComponent("UISprite").spriteName = "star_bg01"
			end
			star:SetActive(true)
		end
	end
	return cup_t
end

function IconPanel.GetCupInf(id)
	local cup_inf_t = LuaHelper.Instantiate(icon_list_["cup_inf_res"])
	local cup_temp = Config.get_t_cup(id)
	local icon = cup_inf_t.transform:Find("cup_icon"):GetComponent("UISprite")
	icon.atlas = IconPanel.GetAltas(cup_temp.icon)
	icon.spriteName = cup_temp.icon
	local cup_name = cup_inf_t.transform:GetComponent("UILabel")
	local cup_num = cup_inf_t.transform:Find("cup_icon/num"):GetComponent("UILabel")
	local star_root = cup_inf_t.transform:Find("cup_icon/star_root")
	cup_name.text = cup_temp.name
	cup_num.text = ""
	IconPanel.InitCupLabel(cup_temp.dai_icon, cup_name)
	for i = 0, star_root.childCount - 1 do
		star_root:GetChild(i):GetComponent("UISprite").spriteName = "star_icon1"
        star_root:GetChild(i).gameObject:SetActive(false)
    end
	if(cup_temp.id == id) then
		for i = 1, cup_temp.max_star do
			star_root:Find(tostring(i)).gameObject:SetActive(true)
			if(cup_temp.star < i) then
				star_root:Find(tostring(i)):GetComponent("UISprite").spriteName = "star_bg01"
			end
		end
	else
		star_root:Find("1").gameObject:SetActive(true)
		cup_num.text = (cup_temp.star + id - cup_temp.id)
		IconPanel.InitCupLabel(cup_temp.dai_icon ,cup_num)
	end
	return cup_inf_t
end

function IconPanel.CheckItem(obj)
	local item_id = obj.name
	GUIRoot.ShowGUI("DetailPanel", {item_id})
end

function IconPanel.ModifyIcon(icon_temp, icon_, frame)
	if(icon_temp ~= nil) then
		local icon = icon_temp:Find('icon'):GetComponent('UISprite')
		icon.spriteName = icon_
		icon.atlas = IconPanel.GetAltas(icon_)
		if(frame ~= nil) then
			local frame_icon = icon_temp:Find('frame'):GetComponent('UISprite')
			if(frame > 10) then
				frame_icon.spriteName = frame_color_two_[frame % 10]
			else
				frame_icon.spriteName = frame_color_[frame]
			end
		end
	end
end

function IconPanel.CreateEffect(t, from, speed, func, param)
	local name = "gold_effect"
	if t == 2 then
		name = "jewel_effect"
	elseif t == 3 then
		name = "exp_effect"
	end
	local eff_parm = {}
	eff_parm.time = 0
	eff_parm.pos1 = GUIRoot.GetUIPos(from)
	eff_parm.pos2 = eff_parm.pos1 + Vector3(math.random(-100, 100), math.random(-100, 100), 0)
	eff_parm.pos3 = nil
	local tt = nil
	if GUIRoot.HasGUI("BackPanel") then
		tt = BackPanel.GetResourceTransform(t)
	else
		tt = TopPanel.GetResourceTransform(t)
	end
	eff_parm.pos4 = GUIRoot.GetUIPos(tt)
	
	local effect_res = nil
	eff_num = eff_num + 1
	if(icon_list_[name] ~= nil) then
		effect_res = LuaHelper.Instantiate(icon_list_[name])
		effect_res.transform.parent = icon_panel_
		effect_res.transform.localPosition = eff_parm.pos1
		effect_res.transform.localScale = Vector3.one
	end
	eff_parm.obj = effect_res
	eff_parm.state = 0
	eff_parm.id = eff_num
	eff_parm.st = math.random(1, 100) / 100 / speed
	eff_parm.func = func
	eff_parm.param = param
	table.insert(eff_list, eff_parm)
end

function IconPanel.RefreshEffect()
	local done_effect = {}
	for i = 1, #eff_list do
		local eff_parm = eff_list[i]
		eff_parm.time = eff_parm.time + Time.deltaTime
		if eff_parm.state == 0 then
			local p = eff_parm.time / eff_parm.st
			if p > 1 then
				p = 1
				eff_parm.state = 1
				eff_parm.time = 0
				eff_parm.obj:SetActive(true)
			end
		elseif eff_parm.state == 1 then
			local p = eff_parm.time * 2
			local v = eff_parm.pos1 + (eff_parm.pos2 - eff_parm.pos1) * math.log10(p * 10 + 1)
			eff_parm.obj.transform.localPosition = v
			if p > 1 then
				p = 1
				eff_parm.state = 2
				eff_parm.time = 0
				eff_parm.pos3 = v
			end
		elseif eff_parm.state == 2 then
			local p = eff_parm.time * 1.5
			eff_parm.obj.transform.localPosition = eff_parm.pos3 + (eff_parm.pos4 - eff_parm.pos3) * p * p
			if p > 1 then
				p = 1
				eff_parm.state = 3
				eff_parm.time = 0
			end
		end
		if eff_parm.state == 3 then
			if(eff_parm.func ~= nil) then
				lua_script_:PlaySound("count")
				eff_parm.func(eff_parm.param)
				if(eff_parm.obj ~= nil) then
					eff_parm.obj:SetActive(false)
				end
			end
			table.insert(done_effect, eff_parm)
		end
	end
	for i = 1, #done_effect do
		IconPanel.RemoveEffect(done_effect[i].id)
		if(done_effect[i].obj ~= nil) then
			GameObject.Destroy(done_effect[i].obj)
		end
	end
	if(#eff_list == 0 and eff_num > 0) then
		eff_num = 0
	end
	if(State.cur_state == State.state.ss_login or State.cur_state == State.state.ss_hall) then
		if Input.GetMouseButtonDown(0) then
			local touch_pos = Input.mousePosition
			local touch_effect = LuaHelper.Instantiate(icon_list_["touch_effect_res"])
			touch_effect.transform.parent = icon_panel_:Find("show_panel")
			touch_effect.transform.localPosition = IconPanel.GetRealViewPos(touch_pos)
			touch_effect.transform.localScale = Vector3.one
			touch_effect:SetActive(true)
			GameObject.Destroy(touch_effect, 0.7)
		end
	end
end

function IconPanel.GetRealViewPos(pos)
	local width = icon_panel_:GetComponent("UIPanel").width
	local height = icon_panel_:GetComponent("UIPanel").height
	local real_x_pos = GUIRoot.width / width * pos.x
	local real_y_pos = GUIRoot.height / height * pos.y
	return Vector3(real_x_pos - GUIRoot.width / 2, real_y_pos - GUIRoot.height / 2, 0)
end

function IconPanel.RemoveEffect(id)
	for i = 1, #eff_list do
		if(id == eff_list[i].id) then
			table.remove(eff_list, i)
			return 0
		end
	end
end

function IconPanel.InitCupLabel(icon_name, cup_label)
	if cup_label == nil then
		return
	end
	cup_label.applyGradient = true
	cup_label.gradientTop = Color(255/ 255,255/ 255,255/ 255)

	if icon_name == 'jb011' then
		cup_label.gradientBottom = Color(247 / 255, 137 / 255, 75 / 255)
		cup_label.effectColor = Color(132 / 255, 66 / 255, 10 / 255)
	elseif icon_name == 'jb022' then
		cup_label.gradientBottom = Color(194 / 255, 204 / 255, 210 / 255)
		cup_label.effectColor = Color(94 / 255, 105/ 255, 114/ 255)
	elseif icon_name == 'jb033' then
		cup_label.gradientBottom = Color(246 / 255, 191 / 255, 73 / 255)
		cup_label.effectColor = Color(132 / 255, 104 / 255, 10 / 255) 
	elseif icon_name == 'jb044' then
		cup_label.gradientBottom = Color(73 / 255, 246 / 255, 214 / 255)
		cup_label.effectColor = Color(10 / 255, 132 / 255, 120 / 255)
	elseif icon_name == 'jb055' then
		cup_label.gradientBottom = Color(74 / 255, 204 / 255, 246 / 255)
		cup_label.effectColor = Color(10 / 255, 91 / 255, 132 / 255)
	elseif icon_name == 'jb066' then
		cup_label.gradientBottom = Color(255 / 255, 192 / 255, 82 / 255)
		cup_label.effectColor = Color(163 / 255, 112 / 255, 3 / 255)
	elseif icon_name == 'jb077' then
		cup_label.gradientBottom = Color(184 / 255, 100 / 255, 255 / 255)
		cup_label.effectColor = Color(98 / 255, 13 / 255, 204 / 255)
	elseif icon_name == 'jb088' then
		cup_label.gradientBottom = Color(255 / 255, 137 / 255, 105 / 255)
		cup_label.effectColor = Color(204 / 255, 13 / 255, 32 / 255)
	end
end

local quality_color = {Color(74/ 255, 204/ 255, 246/ 255), Color(184/ 255,100/ 255,255/ 255), Color(255/ 255,192/ 255,82/ 255)}
local effect_color = {Color(10/ 255, 91/ 255, 132/ 255), Color(98/ 255,13/ 255,204/ 255), Color(163/ 255,112/ 255,3/ 255)}

function IconPanel.InitQualityLabel(name_label, color)
	if name_label == nil then
		return
	end
	name_label.applyGradient = true
	name_label.gradientTop = Color(255/ 255,255/ 255,255/ 255)
	name_label.gradientBottom = quality_color[color]
	name_label.effectStyle = UILabel.Effect.Outline8
	name_label.effectColor = effect_color[color]
	name_label.gameObject:GetComponent("UIWidget").color = Color(255/ 255,255/ 255,255/ 255)
end

local vip_color = {Color(241 / 255, 30/ 255, 241/ 255), Color(255/ 255, 206/ 255, 1/ 255), Color(194 / 255, 229 / 255, 237 / 255)}

function IconPanel.InitVipLabel(name_label, color)
	if name_label == nil then
		return
	end
	name_label.effectStyle = UILabel.Effect.None
	if(color > 0) then
		name_label.applyGradient = true
		name_label.gradientTop = Color(255/ 255,255/ 255,255/ 255)
		name_label.gradientBottom = vip_color[color]
		name_label.gameObject:GetComponent("UIWidget").color = Color(255/ 255,255/ 255,255/ 255)
	else
		name_label.applyGradient = false
		name_label.gameObject:GetComponent("UIWidget").color = vip_color[3]
	end
end

function IconPanel.GetAltas(icon)
	for i = 1, #altas_list do
		if(altas_list[i]:GetSprite(icon) ~= nil) then
			return altas_list[i]
		end
	end
	return altas_list[1]
end