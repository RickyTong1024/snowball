SetPanel = {}

local lua_script_

local code_panel_
local mus_panel_
local main_panel_
local gonggao_panel_
local quality_panel_

local code_input_
local mus_slider_
local mus_thumb_
local eff_slider_
local eff_thumb_

local low_btn_
local mid_btn_
local high_btn_

local normal_eff_
local high_eff_

local gonggao_view_
local gonggao_res_
local view_offset = Vector2(0, 0)
local view_pos = Vector3(0, 0, 0)
local gonggao_pos = Vector3(-174, 77, 0)
local space_y_ = 50
local gonggao_select_id = 0

local quality_id_ = 0
local eff_quality = 0

local main_panel_

local open_mode_ = 0
local code_btn_

function SetPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	main_panel_ = obj.transform:Find("main_panel")
	code_panel_ = obj.transform:Find("code_panel")
	mus_panel_ = obj.transform:Find("mus_panel")
	gonggao_panel_ = obj.transform:Find("gonggao_panel")
	quality_panel_ = obj.transform:Find("quality_panel")
	
	code_btn_ = obj.transform:Find('main_panel/btn_root/code_btn').gameObject
	if self.is_ios_sh then
		code_btn_:SetActive(false)
		local share_btn = obj.transform:Find('main_panel/btn_root/share_btn').gameObject
		share_btn:SetActive(false)
		
		local forum_btn = obj.transform:Find('main_panel/btn_root/forum_btn').gameObject
		forum_btn:SetActive(false)
		
		local host_btn = obj.transform:Find('main_panel/btn_root/host_btn').gameObject
		host_btn:SetActive(false)
		
		local active_btn = obj.transform:Find('main_panel/btn_root/active_btn').gameObject
		active_btn:SetActive(false)
		
		local help_btn = obj.transform:Find('main_panel/btn_root/help_btn').gameObject
		help_btn:SetActive(false)
	else
		--code_btn_:SetActive(true)
	end
	
	--ios sh share_btn,forum_btn,host_btn,active_btn,help_btn
	GUIRoot.UIEffectScalePos(main_panel_.gameObject, 1, true)
	
	code_input_ = code_panel_:Find('code_input'):GetComponent("UIInput")
	mus_volum_ = mus_panel_:Find('sound_set/mus_volum')
	mus_thumb_ = mus_panel_:Find('sound_set/mus_volum/thum')
	eff_volum_ = mus_panel_:Find('sound_set/eff_volum')
	eff_thumb_ = mus_panel_:Find('sound_set/eff_volum/thum')
	
	low_btn_ = quality_panel_:Find("low")
	mid_btn_ = quality_panel_:Find("mid")
	high_btn_ = quality_panel_:Find("high")
	
	normal_eff_ = quality_panel_:Find("normal_eff")
	high_eff_ = quality_panel_:Find("high_eff")
	
	gonggao_view_ = gonggao_panel_:Find("list_panel/m_view")
	gonggao_res_ = gonggao_panel_:Find("list_panel/gonggao_res")
	view_offset = gonggao_view_:GetComponent("UIPanel").clipOffset
	view_pos = gonggao_view_.localPosition
	
	local libao_btn = code_panel_:Find("get_btn")
	local code_close_btn = code_panel_:Find("close_btn")
	local mus_close_btn = mus_panel_:Find("close_btn")
	local gonggao_close_btn = gonggao_panel_:Find("list_panel/close_btn")
	local gonggao_detail_btn = gonggao_panel_:Find("detail_panel/hide_btn")
	local quality_close_btn = quality_panel_:Find("close_btn")
	local quality_ok_btn = quality_panel_:Find("quality_ok_btn")
	
	local btn_root = main_panel_:Find("btn_root")
	if(btn_root.childCount > 0) then
		for i = 0, btn_root.childCount - 1 do
			lua_script_:AddButtonEvent(btn_root:GetChild(i).gameObject, "click", SetPanel.Click)
		end
	end
	
	lua_script_:AddButtonEvent(mus_volum_.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(eff_volum_.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(code_close_btn.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(mus_close_btn.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(gonggao_close_btn.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(gonggao_detail_btn.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(libao_btn.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(low_btn_.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(mid_btn_.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(high_btn_.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(normal_eff_.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(high_eff_.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(quality_close_btn.gameObject, "click", SetPanel.Click)
	lua_script_:AddButtonEvent(quality_ok_btn.gameObject, "click", SetPanel.Click)
	
	SetPanel.RegisterMessage()
	SetPanel.CloseAllPanel()
	SetPanel.InitSetPanel()
end

function SetPanel.OnDestroy()
	lua_script_ = nil
	quality_id_ = 0
	SetPanel.RemoveMessage()
end


function SetPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_LIBAO, SetPanel.SMSG_LIBAO)
	Message.register_handle("team_join_msg", SetPanel.TeamJoin)
end

function SetPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_LIBAO, SetPanel.SMSG_LIBAO)
	Message.remove_handle("team_join_msg", SetPanel.TeamJoin)
end

function SetPanel.OnParam(param)
	open_mode_ = param[1]
	if(open_mode_ == 0) then
		main_panel_.gameObject:SetActive(true)
	elseif(open_mode_ == 1) then
		AchieveAnimation.SetDelayStatus(true)
		gonggao_panel_.gameObject:SetActive(true)
		SetPanel.ReadGongGao(param[2])
	end
end

function SetPanel.TeamJoin()
	GUIRoot.HideGUI('SetPanel')
end


function SetPanel.InitSetPanel()
	SetPanel.ShowVolum()
	SetPanel.ShowQuality(self.quality)
end

function SetPanel.InitGongGaoPanel()
	gonggao_select_id = 0
	gonggao_panel_.gameObject:SetActive(true)
	local empty_tip = gonggao_panel_:Find("list_panel/empty_tip")
	empty_tip.gameObject:SetActive(false)
	gonggao_panel_:Find("list_panel").gameObject:SetActive(false)
	gonggao_panel_:Find("detail_panel").gameObject:SetActive(false)
	if(gonggao_view_.childCount > 0) then
		for i = 0, gonggao_view_.childCount - 1 do
			GameObject.Destroy(gonggao_view_:GetChild(i).gameObject)
		end
	end
	if gonggao_view_:GetComponent('SpringPanel') ~= nil then
		gonggao_view_:GetComponent('SpringPanel').enabled = false
	end
	gonggao_view_:GetComponent('UIPanel').clipOffset = view_offset
	gonggao_view_.localPosition = view_pos
	if(#self.gonggao == 0) then
		empty_tip.gameObject:SetActive(true)
	end
	for i = 1, #self.gonggao do
		local gonggao = LuaHelper.Instantiate(gonggao_res_.gameObject)
		gonggao.transform.parent = gonggao_view_
		gonggao.transform.localPosition = Vector3(0, -(i - 1) * space_y_, 0) + gonggao_pos
		gonggao.transform.localScale = Vector3.one
		local title = gonggao.transform:Find("Label"):GetComponent("UILabel")
		if(string.len(self.gonggao[i]["title"]) > 48) then
			title.text = string.sub(self.gonggao[i]["title"], 1, 48).."..."
		else
			title.text = self.gonggao[i]["title"]
		end
		gonggao.name = self.gonggao[i]["id"]
		lua_script_:AddButtonEvent(gonggao, "click", SetPanel.SelectGongGao)
		gonggao:SetActive(true)
	end
	GUIRoot.UIEffectScalePos(gonggao_panel_.gameObject, true, 1)
	gonggao_panel_:Find("list_panel").gameObject:SetActive(true)
end

function SetPanel.SelectGongGao(obj)
	local id = obj.name
	SetPanel.ReadGongGao(id)
end

function SetPanel.ReadGongGao()
	local id = self.gonggao[1]["id"]
	local gonggao = self.get_gonggao(id)
	self.load_weburl(gonggao["url"], SetPanel.ShowGongGao)
	--gonggao_select_id = id
	GUIRoot.ShowGUI("MaskPanel", {Config.get_t_script_str('SetPanel_001')})
end

function SetPanel.ShowGongGao(www)
	GUIRoot.HideGUI("MaskPanel")
	if(www.error == nil) then
		if(lua_script_ ~= nil) then
			local gonggao = self.get_gonggao(gonggao_select_id)
			local m_view = gonggao_panel_:Find("detail_panel/m_view")
			if m_view:GetComponent('SpringPanel') ~= nil then
				m_view:GetComponent('SpringPanel').enabled = false
			end
			m_view.localPosition = Vector3(0, 0, 0)
			m_view:GetComponent("UIPanel").clipOffset = Vector2(0, 0)
			gonggao_panel_:Find("list_panel").gameObject:SetActive(false)
			gonggao_panel_:Find("detail_panel").gameObject:SetActive(false)
			local desc = gonggao_panel_:Find("detail_panel/m_view/desc"):GetComponent("UILabel")
			desc.text = www.text
			desc:ProcessText()
			desc.transform:GetComponent("Collider").size = Vector2(800, desc.localSize.y)
			desc.transform:GetComponent("Collider").center = Vector2(-desc.transform.localPosition.x, -desc.localSize.y / 2)
			self.read_gonggao(gonggao_select_id)
			GUIRoot.UIEffectScalePos(gonggao_panel_.gameObject, true, 1)
			gonggao_panel_.gameObject:SetActive(true)
			gonggao_panel_:Find("detail_panel").gameObject:SetActive(true)
		end
	else
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('SetPanel_002')})
		if(open_mode_ == 1) then
			GUIRoot.HideGUI("SetPanel")
			if(self.player.sign_finish == 0) then
				GUIRoot.ShowGUI("SignPanel")
			end
		end
	end
end

function SetPanel.Click(obj)
	if(obj.name == 'main_close_btn') then
		SetPanel.CloseSetPanel()
	elseif(obj.name == 'mus_volum') then
		SetPanel.SetVolum(0)
	elseif(obj.name == 'eff_volum') then
		SetPanel.SetVolum(1)
	elseif(obj.name == 'acc_btn') then
		GameTcp.Disconnect()
		State.Reset()
	elseif(obj.name == "get_btn") then
		SetPanel.CheckCode()
	elseif(obj.name == "gonggao_btn") then
		SetPanel.CloseAllPanel()
		--SetPanel.InitGongGaoPanel()
		SetPanel.ReadGongGao()
	elseif(obj.name == "mus_btn") then
		SetPanel.CloseAllPanel()
		GUIRoot.UIEffectScalePos(mus_panel_.gameObject, true, 1)
		mus_panel_.gameObject:SetActive(true)
	elseif(obj.name == "code_btn") then
		SetPanel.CloseAllPanel()
		GUIRoot.UIEffectScalePos(code_panel_.gameObject, true, 1)
		code_panel_.gameObject:SetActive(true)
	elseif(obj.name == "quality_btn") then
		SetPanel.CloseAllPanel()
		GUIRoot.UIEffectScalePos(quality_panel_.gameObject, true, 1)
		quality_id_ = self.quality
		SetPanel.ShowQuality()
		quality_panel_.gameObject:SetActive(true)
	elseif(obj.name == "share_btn") then
		GUIRoot.ShowGUI("SharePanel", {1})
		GUIRoot.HideGUI("SetPanel")
	elseif(obj.name == "forum_btn") then
		Application.OpenURL(self.bbs_url)
	elseif(obj.name == "host_btn") then
		Application.OpenURL(self.home_url)
	elseif(obj.name == "active_btn") then
		Application.OpenURL(self.activity_url)
	elseif(obj.name == "help_btn") then
		Application.OpenURL(self.help_url)
	elseif(obj.name == "close_btn") then
		SetPanel.CloseAllPanel()
		GUIRoot.UIEffectScalePos(main_panel_.gameObject, true, 1)
		main_panel_.gameObject:SetActive(true)
	elseif(obj.name == "hide_btn") then
		if(open_mode_ == 1) then
			GUIRoot.HideGUI("SetPanel")
			if(self.player.sign_finish == 0) then
				GUIRoot.ShowGUI("SignPanel")
			end
			AchieveAnimation.SetDelayStatus(false)
			return 0
		end
		SetPanel.CloseAllPanel()
		GUIRoot.UIEffectScalePos(main_panel_.gameObject, true, 1)
		main_panel_.gameObject:SetActive(true)
	elseif(obj.name == "low") then
		quality_id_ = 0
		SetPanel.ShowQuality()
	elseif(obj.name == "mid") then
		quality_id_ = 1
		SetPanel.ShowQuality()
	elseif(obj.name == "high") then
		quality_id_ = 2
		SetPanel.ShowQuality()
	elseif(obj.name == "quality_ok_btn") then
		SetPanel.SetQuality()
	elseif(obj.name == "normal_eff") then
		eff_quality = 0
		SetPanel.ShowQuality()
	elseif(obj.name == "high_eff") then
		eff_quality = 1
		SetPanel.ShowQuality()
	end
end

function SetPanel.CheckCode()
	if(code_input_.value ~= '') then
		local msg = msg_hall_pb.cmsg_libao()
		msg.code = code_input_.value
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_LIBAO, data, {opcodes.SMSG_LIBAO})
		code_input_.value = ""
	else
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('SetPanel_003')})
	end
end

function SetPanel.SetVolum(id)
	if(id == 0) then
		if(self.mus_volum == 0) then
			self.mus_volum = 1
		else
			self.mus_volum = 0
		end
		if(self.mus_volum == 0) then
			soundMgr.Is_play_mus = false
		else
			soundMgr.Is_play_mus = true
		end
	elseif(id == 1) then
		if(self.eff_volum == 0) then
			self.eff_volum = 1
		else
			self.eff_volum = 0
		end
		if(self.eff_volum == 0) then
			soundMgr.Is_play_eff = false
		else
			soundMgr.Is_play_eff = true
		end
	end
	SetPanel.ShowVolum()
	self.save_set()
end

function SetPanel.ShowVolum()
	if(self.mus_volum == 0) then
		mus_volum_:Find('fg').gameObject:SetActive(false)
		mus_thumb_.localPosition = Vector3(10, 0, 0)
	else
		mus_volum_:Find('fg').gameObject:SetActive(true)
		mus_thumb_.localPosition = Vector3(50, 0, 0)
	end
	if(self.eff_volum == 0) then
		eff_volum_:Find('fg').gameObject:SetActive(false)
		eff_thumb_.localPosition = Vector3(10, 0, 0)
	else
		eff_volum_:Find('fg').gameObject:SetActive(true)
		eff_thumb_.localPosition = Vector3(50, 0, 0)
	end
end

function SetPanel.SetQuality()
	if(self.quality ~= quality_id_ or self.eff_quality ~= eff_quality) then
		self.quality = quality_id_
		QualitySettings.SetQualityLevel(self.quality, true)
		self.eff_quality = eff_quality
		self.save_set()
	end
	GUIRoot.HideGUI("SetPanel")
end

function SetPanel.ShowQuality()
	low_btn_:Find("tip").gameObject:SetActive(false)
	high_btn_:Find("tip").gameObject:SetActive(false)
	mid_btn_:Find("tip").gameObject:SetActive(false)
	if(quality_id_ == 0) then
		low_btn_:Find("tip").gameObject:SetActive(true)
	elseif (quality_id_ == 1) then
		mid_btn_:Find("tip").gameObject:SetActive(true)
	elseif (quality_id_ == 2) then
		high_btn_:Find("tip").gameObject:SetActive(true)
	end
	
	if(eff_quality == 0) then
		normal_eff_:Find("tip").gameObject:SetActive(true)
		high_eff_:Find("tip").gameObject:SetActive(false)
	elseif(eff_quality == 1) then
		normal_eff_:Find("tip").gameObject:SetActive(false)
		high_eff_:Find("tip").gameObject:SetActive(true)
	end
end

function SetPanel.CloseSetPanel()
	GUIRoot.HideGUI("SetPanel")
end

function SetPanel.CloseAllPanel()
	code_panel_.gameObject:SetActive(false)
	mus_panel_.gameObject:SetActive(false)
	main_panel_.gameObject:SetActive(false)
	gonggao_panel_.gameObject:SetActive(false)
	quality_panel_.gameObject:SetActive(false)
end

function SetPanel.SMSG_LIBAO(message)
	local libao_rewards = {}
	local msg = msg_hall_pb.smsg_libao()
	msg:ParseFromString(message.luabuff)
	libao_rewards = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
	for i = 1, #libao_rewards do
		self.add_reward(libao_rewards[i])
	end
	GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('SetPanel_004')})
	GUIRoot.ShowGUI("GainPanel", {libao_rewards})
end
