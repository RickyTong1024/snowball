TopPanel = {}

local lua_script_

local avatar_icon_
local frame_icon_
local jewel_label_
local gold_label_
local snow_label_
local name_label_
local exp_slider_
local exp_label_
local lv_label_
local cup_btn_

local res_time_ = {0, 0, 0}
local res_anim_ = {}

local eff_exp = 0

function TopPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	jewel_label_ = obj.transform:Find('Anchor_top_right/jewel_pro/num'):GetComponent('UILabel')
	gold_label_ = obj.transform:Find('Anchor_top_right/gold_pro/num'):GetComponent('UILabel')
	snow_label_ = obj.transform:Find('Anchor_top_right/snow_pro/num'):GetComponent('UILabel')
	name_label_ = obj.transform:Find('Anchor_top_left/account_info/user_name'):GetComponent('UILabel')
	avatar_icon_ = obj.transform:Find('Anchor_top_left/account_info/avatar_inf')
	exp_slider_ = obj.transform:Find('Anchor_top_left/account_info/exp'):GetComponent('UISlider')
	exp_label_ = obj.transform:Find('Anchor_top_left/account_info/exp/Label'):GetComponent('UILabel')
	lv_label_ = obj.transform:Find('Anchor_top_left/account_info/lv/Label'):GetComponent('UILabel')
	cup_btn_ = obj.transform:Find('Anchor_top_left/account_info/cup_btn')
	
	local jewel_icon = obj.transform:Find('Anchor_top_right/jewel_pro/jewel').gameObject
	local gold_icon = obj.transform:Find('Anchor_top_right/gold_pro/gold').gameObject
	local snow_icon = obj.transform:Find('Anchor_top_right/snow_pro/snow').gameObject
	
	local gold_anim = obj.transform:Find('Anchor_top_right/gold_pro'):GetComponent("Animator")
	local jewel_anim = obj.transform:Find('Anchor_top_right/jewel_pro'):GetComponent("Animator")
	local snow_anim = obj.transform:Find('Anchor_top_right/snow_pro'):GetComponent("Animator")
	local exp_anim = obj.transform:Find("Anchor_top_left/account_info/exp"):GetComponent("Animator")
	local lv_up_anim = obj.transform:Find("Anchor_top_left/account_info/lv"):GetComponent("Animator")
	table.insert(res_anim_, gold_anim)
	table.insert(res_anim_, jewel_anim)
	table.insert(res_anim_, exp_anim)
	table.insert(res_anim_, snow_anim)
	table.insert(res_anim_, lv_up_anim)
	
	local jewel_rechg_btn = obj.transform:Find('Anchor_top_right/jewel_pro/rechag_btn')
	
	lua_script_:AddButtonEvent(avatar_icon_:Find("avatar").gameObject, "click", TopPanel.Click)
	lua_script_:AddButtonEvent(jewel_rechg_btn.gameObject, "click", TopPanel.Click)
	lua_script_:AddButtonEvent(cup_btn_.gameObject, "click", TopPanel.Click)
	lua_script_:AddButtonEvent(jewel_icon, "click", TopPanel.Click)
	lua_script_:AddButtonEvent(gold_icon, "click", TopPanel.Click)
	lua_script_:AddButtonEvent(snow_icon, "click", TopPanel.Click)
	
	TopPanel.RegisterMessage()
	TopPanel.RefreshInf()
	TopPanel.RefreshMoney()
end

function TopPanel.RegisterMessage()
	Message.register_handle("switch_success", TopPanel.SwitchSuccess)
end

function TopPanel.RemoveMessage()
	Message.remove_handle("switch_success", TopPanel.SwitchSuccess)
end

function TopPanel.OnDestroy()
	lua_script_ = nil
	TopPanel.RemoveMessage()
	res_time_ = {0, 0, 0}
	res_anim_ = {}
	timerMgr:RemoveRepeatTimer("TopPanel")
end

-------------ButtonEvent--------------

function TopPanel.Click(obj)
	if(obj.name == 'avatar') then
		TopPanel.OpenUserZonePanel()
	elseif(obj.name == 'rechag_btn') then
		GUIRoot.HideGUI("HallPanel")
		GUIRoot.ShowGUI("ShopPanel", {5})
	elseif(obj.name == 'cup_btn') then
		GUIRoot.ShowGUI("CupPanel")
	elseif(obj.name == 'jewel') then
		GUIRoot.ShowGUI("DetailPanel", {"1+2"})
	elseif(obj.name == 'gold') then
		GUIRoot.ShowGUI("DetailPanel", {"1+1"})
	elseif(obj.name == 'snow') then
		GUIRoot.ShowGUI("DetailPanel", {"1+4"})
	end
end

function TopPanel.OpenUserZonePanel()
	GUIRoot.HideGUI('HallPanel')
	GUIRoot.ShowGUI("ZonePanel", {self.player, self.battle_results})
end

--------------------------------------

---------------刷新界面----------------

function TopPanel.RefreshMoney()
	if(lua_script_ ~= nil) then
		jewel_label_.text = self.player.jewel
		gold_label_.text = self.player.gold
		snow_label_.text = self.player.snow
	end
end

function TopPanel.RefreshInf()
	if(lua_script_ ~= nil) then
		AvaIconPanel.ModifyAvatar(avatar_icon_, self.player.avatar_on,'', self.player.toukuang_on)
		local color = NoticePanel.GetVipNameColor(self.player)
		name_label_.text = self.player.name
		IconPanel.InitVipLabel(name_label_, color)
		local exp_lv = Config.get_t_exp(self.player.level + 1)
		if(exp_lv ~= nil) then
			exp_slider_.value = self.player.exp / exp_lv.exp
			exp_label_.text = tostring(self.player.exp)..'/'..tostring(exp_lv.exp)
		else
			exp_slider_.value = 1
			exp_label_.text = ""
		end
		local cup_temp = Config.get_t_cup(self.player.cup)
		cup_btn_:GetComponent("UISprite").atlas = IconPanel.GetAltas(cup_temp.icon)
		cup_btn_:GetComponent("UISprite").spriteName = cup_temp.icon
		local cup_name = cup_btn_:Find("name"):GetComponent("UILabel")
		local cup_num = cup_btn_:Find("num"):GetComponent("UILabel")
		local star_root = cup_btn_:Find("star_root")
		cup_name.text = cup_temp.name
		cup_num.text = ""
		IconPanel.InitCupLabel(cup_temp.dai_icon, cup_name)
		for i = 0, star_root.childCount - 1 do
			star_root:GetChild(i):GetComponent("UISprite").spriteName = "star_icon1"
            star_root:GetChild(i).gameObject:SetActive(false)
        end
		if(cup_temp.id == self.player.cup) then
			for i = 1, cup_temp.max_star do
				star_root:Find(tostring(i)).gameObject:SetActive(true)
				if(cup_temp.star < i) then
					star_root:Find(tostring(i)):GetComponent("UISprite").spriteName = "star_bg01"
				end
			end
		else
			star_root:Find("1").gameObject:SetActive(true)
			cup_num.text = (cup_temp.star + self.player.cup - cup_temp.id)
			IconPanel.InitCupLabel(cup_temp.dai_icon ,cup_num)
		end
		lv_label_.text = self.player.level
	end
end

function TopPanel.LevelUp()
	if(lua_script_ ~= nil) then
		res_anim_[5]:Play("lv_up")
	end
end

function TopPanel.AddResource(type)
	if(lua_script_ ~= nil) then
		if(type == 1) then
			res_anim_[type]:Play("jewel_up")
		elseif(type == 2) then
			res_anim_[type]:Play("jewel_up")
		elseif(type == 3) then
			res_anim_[type]:Play("exp_add")
		elseif(type == 4) then
			res_anim_[type]:Play("jewel_up")
		end
	end
end

function TopPanel.GetResourceTransform(type)
	local t = nil
	if type == 1 then
		t = gold_label_.gameObject.transform.parent
	elseif type == 2 then
		t = jewel_label_.gameObject.transform.parent
	elseif type == 3 then
		t = lv_label_.gameObject.transform.parent
	elseif type == 4 then
		t = snow_label_.gameObject.transform.parent
	else
		return nil
	end
	return t
end

function TopPanel.SwitchSuccess()
	TopPanel.RefreshInf()
	TopPanel.RefreshMoney()
end
---------------------------------------