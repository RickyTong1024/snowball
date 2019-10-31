LoginPanel = {}

local select_text = ""
local cur_ver_inf = ""
local url_ver_inf = ""

function LoginPanel.Init()
	if(self.wh == 1 and not self.is_whitelist(PlayerPrefs.GetString('username'))) then
		GUIRoot.HideGUI("MaskPanel")
		GUIRoot.ShowGUI("SelectPanel", {self.whtext, Config.get_t_script_str('LoginPanel_001')})
	else
		--[=[
		if(platform_config_common.m_debug) then
			LoginPanel.Login()
		else
			local random_ = self.GetNowTimeString()
			local ver_url = platform_config_common.m_common_url.."version/ver"..platform_config_common.version..".json?v="..random_
			self.load_weburl(ver_url, LoginPanel.check_url_version)
		end
		]=]
		LoginPanel.Login()
	end
end

function LoginPanel.check_url_version(www)
	local data = cjson_safe.decode(www.text)
	if(data == nil) then
		log("ver error")
		return 1
	end
	local random_ = self.GetNowTimeString()
	local ver_file_url = data["ver"].."ver.txt?v="..random_
	PlayerData.load_weburl(ver_file_url, LoginPanel.compare_version)
end

function LoginPanel.compare_version(www)
	url_ver_inf = www.text
	local ver_path = Util.DataPath.."ver.txt"
	cur_ver_inf = toolMgr:load_file(ver_path)
	if(ToolManager.CountVerInf(cur_ver_inf) < ToolManager.CountVerInf(url_ver_inf)) then
		GUIRoot.HideGUI("MaskPanel")
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('LoginPanel_002'),Config.get_t_script_str('LoginPanel_001'), LoginPanel.QuitGame})
	else
		LoginPanel.Login()
	end
end

function LoginPanel.QuitGame()
	Application.Quit()
end

function LoginPanel.Login()
	LoginPanel.RegisterMessage()
	timerMgr:AddTimer('LoginPanel', LoginPanel.Fail , 10)
	LoginPanel.OnConnect()
end

function LoginPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_LOGIN, LoginPanel.SMSG_LOGIN)
	Message.register_net_handle(opcodes.SMSG_LOGIN_PLAYER, LoginPanel.SMSG_LOGIN_PLAYER)
end

function LoginPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_LOGIN, LoginPanel.SMSG_LOGIN)
	Message.remove_net_handle(opcodes.SMSG_LOGIN_PLAYER, LoginPanel.SMSG_LOGIN_PLAYER)
end

function LoginPanel.Fini()
	select_text = ""
	cur_ver_inf = ""
	url_ver_inf = ""
	timerMgr:RemoveTimer('LoginPanel')
	LoginPanel.RemoveMessage()
end

-----------------------------------
function LoginPanel.Stop()
	LoginPanel.Fini()
	GUIRoot.HideGUI("MaskPanel")
end

function LoginPanel.Fail()
	GUIRoot.HideGUI("MaskPanel")
	ConnectTcp.Disconnect()
	GameTcp.Disconnect()
	LoginPanel.RemoveMessage()
	if(select_text == Config.get_t_script_str('LoginPanel_003')) then
		GUIRoot.ShowGUI("SelectPanel", {select_text, Config.get_t_script_str('LoginPanel_004'), State.Reset})
	else
		GUIRoot.ShowGUI("SelectPanel", {select_text, Config.get_t_script_str('LoginPanel_001')})
	end
end

function LoginPanel.OnConnect()
	select_text = Config.get_t_script_str('LoginPanel_005') --"连接connect失败"
	GameTcp.is_first_ = true
	ConnectTcp.Connect()
end

function LoginPanel.OnCheckLogin()
	select_text = Config.get_t_script_str('LoginPanel_006') --"账号验证失败"
	local msg = msg_login_pb.cmsg_login()
	msg.username = PlayerPrefs.GetString('username')
	msg.password = PlayerPrefs.GetString('password')
	msg.pt = platform_config_common.m_platform
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_LOGIN, data)
end

function LoginPanel.SMSG_LOGIN(message)
	local msg = msg_login_pb.smsg_login()
	msg:ParseFromString(message.luabuff)
	self.game_code = msg.code
	self.is_guest = msg.acc.is_guest
	self.guid = tostring(msg.acc.guid)
	GameTcp.Send(opcodes.CMSG_LOGIN_PLAYER)
	select_text = Config.get_t_script_str('LoginPanel_007') --"登录游戏失败"
end

function LoginPanel.SMSG_LOGIN_PLAYER(message)
	LuaAchieve.OnInit()  --初始化成就数据
	local msg = msg_hall_pb.smsg_login_player()
	msg:ParseFromString(message.luabuff)
	select_text = Config.get_t_script_str('LoginPanel_008') --"进入大厅失败"
	self.player = msg.player
	self.old_level = self.player.level
	self.post_num = msg.post_num
	self.calc_out_attr()
	self.load_set()
	timerMgr:set_server_time(msg.server_time)
	for i = 1, #msg.roles do
		self.add_role(msg.roles[i])
	end
	for i = 1, #msg.battle_his do
		self.add_battle_result(msg.battle_his[i])
	end
	ShopPanel.RefreshRollReward()
	GUIRoot.HideGUI("MaskPanel")
	GUIRoot.ShowGUI("AchieveAnimation")
	toolMgr:cancel_notify()
	if(self.player.sign_finish == 0) then
		local dt = timerMgr:dtnow()
		toolMgr:create_notify(Config.get_t_script_str('LoginPanel_009'), Config.get_t_script_str('LoginPanel_010'), dt.Hour, dt.Minute + 1)
	end
	
	--State.ChangeState(State.state.ss_hall)
	--GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('LoginPanel_011')})
	local param = self.guid.."|"..self.player.serverid.."|"..self.player.name.."|"..self.player.level
	if self.player.is_guide == 1 then --进入新手引导界面
		State.ChangeState(State.state.ss_newplayerguide)
		platform._instance:on_game_login(param,1)
	else
		State.ChangeState(State.state.ss_hall)
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('LoginPanel_011')})
		platform._instance:on_game_login(param,0)
	end
end

