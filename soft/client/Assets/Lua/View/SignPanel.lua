SignPanel = {}

local lua_script_

local main_panel_

local sign_res_
local sign_btn_

local sign_pos_ = Vector3(-140, 75, 0)

local space_x_ = 145
local space_y_ = 158

function SignPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	main_panel_ = obj.transform:Find("main_panel")
	
	GUIRoot.UIEffectScalePos(main_panel_.gameObject, 1, true)
	
	sign_res_ = main_panel_:Find("sign_res")
	
	sign_btn_ = main_panel_:Find("sign_btn")
	
	lua_script_:AddButtonEvent(sign_btn_.gameObject, "click", SignPanel.Click)
	main_panel_.gameObject:SetActive(false)
	SignPanel.InitSignPanel()
	SignPanel.RegisterMessage()
end

function SignPanel.OnDestroy()
	lua_script_ = nil
	SignPanel.RemoveMessage()
end

function SignPanel.RegisterMessage()
	Message.register_handle("team_join_msg", SignPanel.TeamJoin)
	Message.register_net_handle(opcodes.SMSG_SIGN, SignPanel.SMSG_SIGN)
end

function SignPanel.RemoveMessage()
	Message.remove_handle("team_join_msg", SignPanel.TeamJoin)
	Message.remove_net_handle(opcodes.SMSG_SIGN, SignPanel.SMSG_SIGN)
end

function SignPanel.TeamJoin()
	GUIRoot.HideGUI('SignPanel')
end

---------------------初始化----------------------

function SignPanel.InitSignPanel()
	for i = 0, 5 do
		local sign_t = LuaHelper.Instantiate(sign_res_.gameObject)
		sign_t.transform.parent = main_panel_
		sign_t.transform.localPosition = Vector3(i % 4 * space_x_, -(math.floor(i / 4) * space_y_), 0) + sign_pos_
		sign_t.transform.localScale = Vector3.one
		local icon = sign_t.transform:Find("icon"):GetComponent("UISprite")
		local title = sign_t.transform:Find("day"):GetComponent("UILabel")
		local title_bg = sign_t.transform:Find("title"):GetComponent("UISprite")
		local num = sign_t.transform:Find("Label"):GetComponent("UILabel")
		local mask = sign_t.transform:Find("mask")
		local sign_temp = Config.get_t_sign(i + 1)
		local reward_temp = Config.get_t_reward(sign_temp.type, sign_temp.value1)
		icon.spriteName = reward_temp.icon
		icon.gameObject.name = tostring(sign_temp.type).."+"..tostring(sign_temp.value1)
		title.text = sign_temp.day
		num.text = self.font_color[reward_temp.color % 10]..reward_temp.name.."[-] [c2e5ed]x "..sign_temp.value2
		if(self.player.sign_index > i) then
			mask.gameObject:SetActive(true)
		end
		if(self.player.sign_index == i) then
			sign_t.transform:GetComponent("UISprite").spriteName = "xindi1"
			title_bg.spriteName = "qd-4"
		end
		sign_t.name = i
		lua_script_:AddButtonEvent(icon.gameObject, "click", SignPanel.CheckItem)
		sign_t.gameObject:SetActive(true)
	end
	local last_day = main_panel_:Find(6)
	local icon = last_day:Find("icon"):GetComponent("UISprite")
	local title = last_day:Find("day"):GetComponent("UILabel")
	local title_bg = last_day:Find("title"):GetComponent("UISprite")
	local num = last_day:Find("Label"):GetComponent("UILabel")
	local effect = last_day:Find("effect")
	local mask = last_day:Find("mask")
	local sign_temp = Config.get_t_sign(7)
	local reward_temp = Config.get_t_reward(sign_temp.type, sign_temp.value1)
	icon.spriteName = reward_temp.icon
	icon.gameObject.name = tostring(sign_temp.type).."+"..tostring(sign_temp.value1)
	title.text = sign_temp.day
	num.text = self.font_color[reward_temp.color % 10]..reward_temp.name.."[-] [c2e5ed]x "..sign_temp.value2
	if(self.player.sign_index == 6) then
		last_day:GetComponent("UISprite").spriteName = "xindi1"
		title_bg.spriteName = "qd-4"
		if(self.player.sign_finish == 1) then
			mask.gameObject:SetActive(true)
			effect.gameObject:SetActive(false)
		end
	end
	if(self.player.sign_finish == 1) then
		sign_btn_:GetComponent("UISprite").spriteName = "big_tybutton_hui"
	end
	lua_script_:AddButtonEvent(icon.gameObject, "click", SignPanel.CheckItem)
	main_panel_.gameObject:SetActive(true)
end


-------------------------------------------

---------------ButtonEvent--------------

function SignPanel.Click(obj)
	if(obj.name == 'sign_btn') then
		SignPanel.GetReward()
	end
end

function SignPanel.GetReward()
	if(self.player.sign_finish == 1) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('SignPanel_001')})
	else
		GameTcp.Send(opcodes.CMSG_SIGN, nil, {opcodes.SMSG_SIGN})
	end
end

function SignPanel.CheckItem(obj)
	GUIRoot.ShowGUI("DetailPanel", {obj.name})
end

function SignPanel.ClosePanel()
	GUIRoot.HideGUI("SignPanel")
end
----------------------------------------

function SignPanel.SMSG_SIGN()
	sign_btn_:GetComponent("UISprite").spriteName = "big_tybutton_hui"
	self.player.sign_finish = 1
	local sign = main_panel_:Find(self.player.sign_index)
	sign:Find("mask").gameObject:SetActive(true)
	if(self.player.sign_index == 6) then
		sign:Find("effect").gameObject:SetActive(false)
	end
	local sign_temp = Config.get_t_sign(self.player.sign_index + 1)
	local reward = {}
	reward.type = sign_temp.type
	reward.value1 = sign_temp.value1
	reward.value2 = sign_temp.value2
	if(tonumber(self.player.yue_time) > tonumber(timerMgr:now_string()) or tonumber(self.player.nian_time) > tonumber(timerMgr:now_string())) then
		reward.value2 = reward.value2 + 2 * sign_temp.value2
	end
	reward.value3 = sign_temp.value3
	self.add_reward(reward)
	SignPanel.ClosePanel()
	GUIRoot.ShowGUI("GainPanel", {{reward}})
end
