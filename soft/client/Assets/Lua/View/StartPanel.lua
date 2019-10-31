StartPanel = {}

local login_panel_
local register_panel_
local start_panel_
local acc_panel_
local mod_panel_
local acc_close_btn_
local set_btn_

local log_user_input_
local log_pass_input_

local reg_user_input_
local reg_password_input_
local reg_repassword_input_

local mod_user_input_
local mod_password_input_
local mod_newpassword_input_
local mod_repassword_input_

local reg_username_
local reg_password_

local user_charater = "0123456789abcdefghijklmnopqrstuvwxyz"

local pass_charater = "0123456789"

local user_id_

function StartPanel.Awake(obj)
	local lua_script = obj.transform:GetComponent('LuaUIBehaviour')
	
	pass_charater = stringTotable(pass_charater)
	user_charater = stringTotable(user_charater)
	
	login_panel_ = obj.transform:Find("Anchor_bottom/login_panel")
	register_panel_ = obj.transform:Find("Anchor_bottom/register_panel")
	start_panel_ = obj.transform:Find("start_panel")
	acc_panel_ = obj.transform:Find("acc_panel")
	mod_panel_ = obj.transform:Find("Anchor_bottom/mod_panel")
	
	log_user_input_ = login_panel_:Find("username"):GetComponent("UIInput")
	log_pass_input_ = login_panel_:Find("password"):GetComponent("UIInput")
	
	reg_user_input_ = register_panel_:Find("username"):GetComponent("UIInput")
	reg_password_input_ = register_panel_:Find("password"):GetComponent("UIInput")
	reg_repassword_input_ = register_panel_:Find("repassword"):GetComponent("UIInput")
	
	mod_user_input_ = mod_panel_:Find("username"):GetComponent("UIInput")
	mod_password_input_ = mod_panel_:Find("oldpassword"):GetComponent("UIInput")
	mod_newpassword_input_ = mod_panel_:Find("newpassword"):GetComponent("UIInput")
	mod_repassword_input_ = mod_panel_:Find("repassword"):GetComponent("UIInput")
	
	user_id_ = start_panel_:Find("id"):GetComponent("UILabel")

	ver_text = obj.transform:Find("Anchor_bottom_left/Label"):GetComponent("UILabel")
	
	if(not platform_config_common.m_debug) then
		local ver_path = Util.DataPath.."ver.txt"
		local cur_ver_inf = toolMgr:load_file(ver_path)
		ver_text.text = platform_config_common.version.."("..cur_ver_inf..")"
	else
		ver_text.text = platform_config_common.version
	end
	
	local start_btn = start_panel_:Find("start_btn")
	set_btn_ = start_panel_:Find("Anchor_top_right/set_btn")
	local acc_login_btn = acc_panel_:Find("acc_login_btn")
	local acc_reg_btn = acc_panel_:Find("acc_regist_btn")
	local acc_mod_btn = acc_panel_:Find("acc_mod_btn")
	local login_btn = login_panel_:Find("login_btn")
	local regist_btn = register_panel_:Find("register_btn")
	local mod_btn = mod_panel_:Find("mod_btn")
	local log_close_btn = login_panel_:Find("log_close_btn")
	local reg_close_btn = register_panel_:Find("rg_close_btn")
	local mod_close_btn = mod_panel_:Find("mod_close_btn")
	local quick_reg_btn = register_panel_:Find("Anchor/quick_reg_btn")
	acc_close_btn_ = acc_panel_:Find("Anchor/acc_close_btn")
	
	lua_script:AddButtonEvent(start_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(set_btn_.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(acc_login_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(acc_reg_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(acc_mod_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(login_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(regist_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(log_close_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(reg_close_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(acc_close_btn_.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(mod_close_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(mod_btn.gameObject, "click", StartPanel.Click)
	lua_script:AddButtonEvent(quick_reg_btn.gameObject, "click", StartPanel.Click)
	
	local seed = timerMgr:now_string()
	seed = stringTotable(seed)
	seed = table_convert(seed)
	seed = tableTostring(seed)
	math.randomseed(tonumber(seed))
	StartPanel.CloseUI()
	self.InitServer(1)
end

function StartPanel.OnDestroy()
	reg_username_ = ""
	reg_password_ = ""
end

-----------------------------------

function StartPanel.Click(obj)
	if(obj.name == "start_btn") then
		StartPanel.StartGame()
	elseif(obj.name == "set_btn") then
		StartPanel.CloseUI()
		acc_panel_.gameObject:SetActive(true)
		acc_close_btn_.gameObject:SetActive(true)
	elseif(obj.name == "acc_login_btn") then
		StartPanel.CloseUI()
		login_panel_.gameObject:SetActive(true)
	elseif(obj.name == "acc_regist_btn") then
		StartPanel.CloseUI()
		StartPanel.OneKeyRegister()
		register_panel_.gameObject:SetActive(true)
	elseif(obj.name == "acc_mod_btn") then
		StartPanel.CloseUI()
		mod_panel_.gameObject:SetActive(true)
	elseif(obj.name == "login_btn") then
		StartPanel.InputLogin()
	elseif(obj.name == "register_btn") then
		StartPanel.Register()
	elseif(obj.name == "quick_reg_btn") then
		StartPanel.OneKeyRegister()
	elseif(obj.name == "mod_btn") then
		StartPanel.ModifyPassword()
	elseif(obj.name == "log_close_btn") then
		StartPanel.CloseUI()
		acc_panel_.gameObject:SetActive(true)
		log_user_input_.value = ""
		log_pass_input_.value = ""
	elseif(obj.name == "rg_close_btn") then
		StartPanel.CloseUI()
		acc_panel_.gameObject:SetActive(true)
		reg_user_input_.value = ""
		reg_password_input_.value = ""
		reg_repassword_input_.value = ""
	elseif(obj.name == "acc_close_btn") then
		StartPanel.InitPanel()
	elseif(obj.name == "mod_close_btn") then
		StartPanel.CloseUI()
		acc_panel_.gameObject:SetActive(true)
		mod_user_input_.value = ""
		--mod_password_input_.value = ""
		mod_newpassword_input_.value = ""
		mod_repassword_input_.value = ""
	end
end

function StartPanel.InitPanel()
	StartPanel.CloseUI()
	if(PlayerPrefs.HasKey('username')) then
		user_id_.text = Config.get_t_script_str('StartPanel_001').." "..PlayerPrefs.GetString("username")
		start_panel_.gameObject:SetActive(true)
	else
		if(self.review == 1) then
			StartPanel.OneKeyRegister()
			register_panel_.gameObject:SetActive(true)
		else
			acc_panel_.gameObject:SetActive(true)
			acc_close_btn_.gameObject:SetActive(false)
		end
	end
end

function StartPanel.StartGame()
	ConnectTcp.Disconnect()
	GameTcp.Disconnect()
	self.ClearData()
	GUIRoot.ShowGUI("MaskPanel", {Config.get_t_script_str('StartPanel_002')})
	self.InitServer(2)
end

function StartPanel.InputLogin()
	local username = log_user_input_.value
	local password = log_pass_input_.value
	StartPanel.Login(username, password)
end

function StartPanel.Login(username, password)
	local wwwf = WWWForm.New()
	wwwf:AddField("username", username)
	wwwf:AddField("password", password)
	self.send_http(self.login_url.."login", wwwf, StartPanel.LoginCallBack, Config.get_t_script_str('StartPanel_003'))
end

function StartPanel.LoginCallBack(www)
	GUIRoot.HideGUI("MaskPanel")
	local code = tonumber(www.text)
	if(code == 0) then
		if(log_user_input_.value ~= "") then
			PlayerPrefs.SetString('username', log_user_input_.value)
			PlayerPrefs.SetString('password', log_pass_input_.value)
			PlayerPrefs.Save()
		end
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_004')})
		StartPanel.InitPanel()
	elseif(code == -1) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_005')})
	elseif(code == -2) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_006')})
	elseif(code == -3) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_007')})
	elseif(code == -4) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_008')})
	end
	log_user_input_.value = ""
	log_pass_input_.value = ""
end

function StartPanel.Register()
	local username = reg_user_input_.value
	local result = 0
	if(username == '') then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_009')})
	elseif(string.len(username) < 6) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_010')})
	elseif(string.len(username) > 18) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_011')})
	elseif(string.match(username,'^[a-zA-Z0-9@_]+$') ~= username) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_012')})
	else
		result = result + 1
	end
	local password = reg_password_input_.value
	local repassword = reg_repassword_input_.value
	if(password == '') then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_013')})
	elseif(string.match(password, "^[a-zA-Z0-9]+$") ~= password) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_014')})
	elseif(password ~= repassword) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_015')})
	else
		result = result + 1
	end
	if(result == 2) then
		reg_username_ = reg_user_input_.value
		reg_password_ = reg_password_input_.value
		local wwwf = WWWForm.New()
		wwwf:AddField("username", reg_username_)
		wwwf:AddField("password", reg_password_)
		self.send_http(self.login_url.."reg", wwwf, StartPanel.RegisterCallBack, Config.get_t_script_str('StartPanel_016'))
	end
end

function StartPanel.RegisterCallBack(www)
	GUIRoot.HideGUI("MaskPanel")
	local code = tonumber(www.text)
	if(code == 0) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_017')})
		toolMgr:save_photo(shareMgr:GetShareID(tonumber(self.guid)).."-"..timerMgr:now_string())
		PlayerPrefs.SetString('username', reg_username_)
		PlayerPrefs.SetString('password', reg_password_)
		PlayerPrefs.Save()
		timerMgr:AddTimer("RegisterSuccess", StartPanel.RegisterSuccess, 0.1)
	else
		if(code == -1) then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_005')})
		elseif(code == -2) then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_018')})
		end
		reg_user_input_.value = ""
		reg_password_input_.value = ""
		reg_repassword_input_.value = ""
		reg_username_ = ""
		reg_password_ = ""
	end
end

function StartPanel.RegisterSuccess()
	reg_user_input_.value = ""
	reg_password_input_.value = ""
	reg_repassword_input_.value = ""
	reg_username_ = ""
	reg_password_ = ""
	StartPanel.InitPanel()
end

function StartPanel.OneKeyRegister()
	local user_length = 12
	local pass_length = 6
	local username = "SN"
	local index = 0
	for i = 1, user_length do
		index = math.random(1, #user_charater)
		username = username..user_charater[index]
	end
	local password = ""
	for i = 1, pass_length do
		password = password..pass_charater[math.random(1, #pass_charater)]
	end
	reg_user_input_.value = username
	reg_password_input_.value = password
	reg_repassword_input_.value = password
end

function StartPanel.ModifyPassword()
	local username = mod_user_input_.value
	local password = mod_password_input_.value
	local repassword = mod_repassword_input_.value
	local newpassword = mod_newpassword_input_.value
	if(username == '') then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_009')})
	elseif(password == '') then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_013')})
	elseif(newpassword ~= repassword) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_015')})
	else
		local wwwf = WWWForm.New()
		wwwf:AddField("username", username)
		wwwf:AddField("old_password", password)
		wwwf:AddField("new_password", newpassword)
		self.send_http(self.login_url.."chpwd", wwwf, StartPanel.ModifyPasswordCallBack)
	end
end

function StartPanel.ModifyPasswordCallBack(www)
	GUIRoot.HideGUI("MaskPanel")
	local code = tonumber(www.text)
	if(code == 0) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_019')})
		PlayerPrefs.SetString('username', mod_user_input_.value)
		PlayerPrefs.SetString('password', mod_newpassword_input_.value)
		StartPanel.InitPanel()
	elseif(code == -1) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_005')})
	elseif(code == -2) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_006')})
	elseif(code == -3) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_007')})
	elseif(code == -4) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('StartPanel_008')})
	end
	mod_user_input_.value = ''
	mod_password_input_.value = ''
	mod_newpassword_input_.value = ''
	mod_repassword_input_.value = ''
end

function StartPanel.CloseUI()
	login_panel_.gameObject:SetActive(false)
	register_panel_.gameObject:SetActive(false)
	start_panel_.gameObject:SetActive(false)
	acc_panel_.gameObject:SetActive(false)
	mod_panel_.gameObject:SetActive(false)
end