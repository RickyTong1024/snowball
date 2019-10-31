BackPanel = {}

local lua_script_

local jewel_label_
local gold_label_
local snow_label_
local exp_label_
local exp_slider_
local lv_label_
local name_label_

local lv_pro_
local exp_pro_
local cup_btn_

local show_mode_ = 0

local res_time_ = {0, 0, 0}
local res_anim_ = {}

function BackPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	jewel_label_ = obj.transform:Find('Anchor_top_right/top_right_panel/jewel_pro/num'):GetComponent('UILabel')
	gold_label_ = obj.transform:Find('Anchor_top_right/top_right_panel/gold_pro/num'):GetComponent('UILabel')
	snow_label_ = obj.transform:Find('Anchor_top_right/top_right_panel/snow_pro/num'):GetComponent('UILabel')
	
	lv_pro_ = obj.transform:Find('Anchor_top_left/account_info/lv').gameObject
	exp_pro_ = obj.transform:Find('Anchor_top_left/account_info/exp').gameObject
	
	exp_slider_ = obj.transform:Find('Anchor_top_left/account_info/exp'):GetComponent('UISlider')
	exp_label_ = obj.transform:Find('Anchor_top_left/account_info/exp/Label'):GetComponent('UILabel')
	lv_label_ = obj.transform:Find('Anchor_top_left/account_info/lv/Label'):GetComponent('UILabel')
	name_label_ = obj.transform:Find('Anchor_top_left/account_info/user_name'):GetComponent('UILabel')
	
	local jewel_icon = obj.transform:Find('Anchor_top_right/top_right_panel/jewel_pro/jewel').gameObject
	local gold_icon = obj.transform:Find('Anchor_top_right/top_right_panel/gold_pro/gold').gameObject
	local snow_icon = obj.transform:Find('Anchor_top_right/top_right_panel/snow_pro/snow').gameObject
	
	local gold_anim = obj.transform:Find('Anchor_top_right/top_right_panel/gold_pro'):GetComponent("Animator")
	local jewel_anim = obj.transform:Find('Anchor_top_right/top_right_panel/jewel_pro'):GetComponent("Animator")
	local snow_anim = obj.transform:Find('Anchor_top_right/top_right_panel/snow_pro'):GetComponent("Animator")
	local exp_anim = obj.transform:Find("Anchor_top_left/account_info/exp"):GetComponent("Animator")
	local lv_up_anim = obj.transform:Find("Anchor_top_left/account_info/lv"):GetComponent("Animator")
	table.insert(res_anim_, gold_anim)
	table.insert(res_anim_, jewel_anim)
	table.insert(res_anim_, exp_anim)
	table.insert(res_anim_, snow_anim)
	table.insert(res_anim_, lv_up_anim)
	
	local jewel_rechg_btn = obj.transform:Find('Anchor_top_right/top_right_panel/jewel_pro/rechag_btn')
	local back_btn = obj.transform:Find('Anchor_top_left/top_left_panel/back_btn')
	cup_btn_ = obj.transform:Find('Anchor_top_left/account_info/cup_btn')
	
	lua_script_:AddButtonEvent(back_btn.gameObject, "click", BackPanel.Click)
	lua_script_:AddButtonEvent(jewel_rechg_btn.gameObject, "click", BackPanel.Click)
	lua_script_:AddButtonEvent(cup_btn_.gameObject, "click", BackPanel.Click)
	lua_script_:AddButtonEvent(jewel_icon, "click", BackPanel.Click)
	lua_script_:AddButtonEvent(gold_icon, "click", BackPanel.Click)
	lua_script_:AddButtonEvent(snow_icon, "click", BackPanel.Click)
	
	BackPanel.RefreshMoney()
	BackPanel.RefreshInf()
	BackPanel.RegisterMessage()
end

function BackPanel.RegisterMessage()
	
end

function BackPanel.RemoveMessage()
	
end

function BackPanel.OnDestroy()
	lua_script_ = nil
	BackPanel.RemoveMessage()
	res_time_ = {0, 0, 0}
	res_anim_ = {}
	show_mode_ = 0
	timerMgr:RemoveRepeatTimer("BackPanel")
end

function BackPanel.OnParam(parm)
	if(parm == nil) then
		show_mode_ = 0
	else
		show_mode_ = parm[1]
	end
	cup_btn_.gameObject:SetActive(false)
	lv_pro_:SetActive(false)
	exp_pro_:SetActive(false)
	if(show_mode_ == 0) then
		lv_pro_:SetActive(true)
		exp_pro_:SetActive(true)
		cup_btn_.gameObject:SetActive(true)
	elseif(show_mode_ == 1) then
		cup_btn_.gameObject:SetActive(true)
	elseif(show_mode_ == 2) then
		lv_pro_:SetActive(true)
		exp_pro_:SetActive(true)
	end
	BackPanel.RefreshMoney()
	BackPanel.RefreshInf()
end
---------------ButtonEvent--------------

function BackPanel.Click(obj)
	if(obj.name == 'back_btn') then
		BackPanel.Back()
	elseif(obj.name == 'rechag_btn') then
		BackPanel.jewelRechg()
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

function BackPanel.jewelRechg()
	local msg = s_message.New()
	msg.name = "back_panel_recharge"
	Message.add_message(msg)
end

----------------------------------------
function BackPanel.RefreshMoney()
	if(lua_script_ ~= nil) then
		jewel_label_.text = self.player.jewel
		gold_label_.text = self.player.gold
		snow_label_.text = self.player.snow
	end
end

function BackPanel.RefreshInf()
	if(lua_script_ ~= nil) then
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
		lv_label_.text = self.player.level
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
	end
end

function BackPanel.Back()
	local msg = s_message.New()
	msg.name = "back_panel_msg"
	Message.add_message(msg)
end

function BackPanel.LevelUp()
	if(lua_script_ ~= nil) then
		res_anim_[5]:Play("lv_up")
	end
end

function BackPanel.AddResource(type)
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

function BackPanel.GetResourceTransform(type)
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
