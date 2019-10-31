DetailPanel = {}

local lua_script_
local item_icon_
local desc_label_
local name_label_
local num_label_
local type_label_

local normal_obj_
local exp_obj_

local exp_icon_
local exp_name_
local exp_desc_

function DetailPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	normal_obj_ = obj.transform:Find('normal_des').gameObject
	item_icon_ = normal_obj_.transform:Find('item_icon')
	name_label_ = normal_obj_.transform:Find('name'):GetComponent('UILabel')
	num_label_ = normal_obj_.transform:Find('num'):GetComponent('UILabel')
	desc_label_ = normal_obj_.transform:Find('desc'):GetComponent('UILabel')
	type_label_ = normal_obj_.transform:Find('type'):GetComponent('UILabel')
	
	exp_obj_ = obj.transform:Find('exp_des').gameObject
	exp_icon_ = exp_obj_.transform:Find('icon'):GetComponent('UISprite')
	exp_name_ = exp_obj_.transform:Find('name'):GetComponent('UILabel')
	exp_desc_ = exp_obj_.transform:Find('desc'):GetComponent('UILabel')
	
	local close_btn = obj.transform:Find('bg')
	
	lua_script_:AddButtonEvent(close_btn.gameObject, "click", DetailPanel.Close)
end

function DetailPanel.OnDestroy()
	
end

function DetailPanel.OnParam(parm)
	local item_type_id = parm[1]
	item_type_id = string_split(item_type_id, "+")
	local item_type = tonumber(item_type_id[1])
	local item_id = tonumber(item_type_id[2])
	if item_type < 10 then
		normal_obj_:SetActive(true)
		exp_obj_:SetActive(false)
		local item_temp = Config.get_t_reward(item_type, item_id)
		local type_text = ""
		local num_text = ""
		if(item_type == 1) then
			type_text = Config.get_t_script_str('DetailPanel_001')   --"资源"
		elseif(item_type == 2) then
			if(item_temp.type == 1001) then
				type_text = Config.get_t_script_str('DetailPanel_002') --"角色"
			else
				type_text = Config.get_t_script_str('DetailPanel_003') --"道具"
			end
			local item_num = self.get_item_num(item_id)
			local content = string.format(Config.get_t_script_str('DetailPanel_004'),item_num)
			num_text = content --"[C2E5ED]拥有  "..item_num
		elseif(item_type == 3) then
			type_text = Config.get_t_script_str('DetailPanel_002') --"角色"
			if(self.get_role_id(item_id) ~= nil) then
				num_text = Config.get_t_script_str('DetailPanel_005') --"[57FC5B]已拥有"
			else
				num_text = Config.get_t_script_str('DetailPanel_006') --"[7E90A4]未拥有"
			end
		elseif(item_type == 6) then
			if(self.has_toukuang(item_id)) then
				num_text = Config.get_t_script_str('DetailPanel_005') --"[57FC5B]已拥有"
			else
				num_text = Config.get_t_script_str('DetailPanel_006') --"[7E90A4]未拥有"
			end
			type_text = Config.get_t_script_str('DetailPanel_007')    --"头像框"
		elseif(item_type == 7) then
			if(self.has_fashion(item_id)) then
				num_text = Config.get_t_script_str('DetailPanel_005') --"[57FC5B]已拥有"
			else
				num_text = Config.get_t_script_str('DetailPanel_006') --"[7E90A4]未拥有"
			end
			if(item_temp.type == 1) then
				type_text = Config.get_t_script_str('DetailPanel_008') --"雪球皮肤"
			elseif(item_temp.type == 2) then
				type_text = Config.get_t_script_str('DetailPanel_009') --"蓄力特效"
			elseif(item_temp.type == 3) then
				type_text = Config.get_t_script_str('DetailPanel_010') --"雪炮皮肤"
			end
		end
		IconPanel.ModifyIcon(item_icon_, item_temp.icon, item_temp.color)
		type_label_.text = type_text
		num_label_.text = num_text
		name_label_.text = item_temp.name
		IconPanel.InitQualityLabel(name_label_, item_temp.color % 10)
		desc_label_.text = item_temp.desc
	else
		normal_obj_:SetActive(false)
		exp_obj_:SetActive(true)
		--item_type == 11：天赋
		if item_type == 11 then
			local t_talent = Config.get_t_talent(item_id)
			local val = 0
			if t_talent.id == 3 then
				val = t_talent.param3 / 100
			else
				val = t_talent.param3
			end
			exp_icon_.spriteName = t_talent.icon
			
			exp_name_.text = string.gsub(t_talent.desc1, "{N1}",1)
			exp_desc_.text = string.gsub(t_talent.desc2, "{N2}",val)
		end
	end
end

function DetailPanel.Close()
	GUIRoot.HideGUI("DetailPanel")
end