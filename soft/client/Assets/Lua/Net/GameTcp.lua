GameTcp = {}

GameTcp.is_first_ = true

local self_dis_ = false
local ip_ = 0
local port_ = 0
local reconnect_ = false

function GameTcp.Connect(ip, port)
	self_dis_ = false
	networkMgr:Connect("GameTcp", ip, port)
	ip_ = ip
	port_ = port
	reconnect_ = false
end

function GameTcp.Disconnect()
	self_dis_ = true
	networkMgr:Disconnect("GameTcp")
end

function GameTcp.Isconnect()
	return networkMgr:Isconnect("GameTcp")
end

function GameTcp.Send(opcode, data, smsgs, text, time)
	if text == nil then
		text = ""
	end
	if time == nil then
		time = 10
	end
	if smsgs ~= nil and #smsgs > 0 then
		Message.OnMask(smsgs, text, time)
	end
	if data then
		networkMgr:SendMessage("GameTcp", opcode, data)
	else
		networkMgr:SendMessageNull("GameTcp", opcode)
	end
end

function GameTcp.ReConnect()
	reconnect_ = true
	networkMgr:Connect("GameTcp", ip_, port_)
	GUIRoot.ShowGUI('MaskPanel', {Config.get_t_script_str('GameTcp_001')})
end

function GameTcp.OnConnect()
	reconnect_ = false
	if GameTcp.is_first_ then
		LoginPanel.OnCheckLogin()
		GameTcp.is_first_ = false
	else
		local msg = msg_login_pb.cmsg_relogin_player()
		msg.code = self.game_code
		msg.guid = self.guid
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_RELOGIN_PLAYER, data, {opcodes.SMSG_LOGIN_PLAYER},Config.get_t_script_str('GameTcp_001'))
	end
end

function GameTcp.OnConnectFail()
	if (not reconnect_) then
		ConnectTcp.Disconnect()
		GameTcp.Disconnect()
		LoginPanel.Stop()
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('GameTcp_002'),Config.get_t_script_str('GameTcp_003')})
	else
		GUIRoot.HideGUI('MaskPanel')
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('GameTcp_004'),Config.get_t_script_str('GameTcp_005'), GameTcp.ReConnect})
	end
end

function GameTcp.OnDisconnect()
	log('GameTcp Disconnect')
	if not self_dis_ then
		if State.cur_state == State.state.ss_login then
			LoginPanel.Stop()
			ConnectTcp.Disconnect()
			if not GameTcp.is_first_ then
				GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('GameTcp_006'),Config.get_t_script_str('GameTcp_005'), State.Reset})
			end		
		elseif State.cur_state == State.state.ss_hall then
			--GUIRoot.ShowGUI("SelectPanel", {"与服务器断开连接", "重新连接", GameTcp.ReConnect})
			if(not reconnect_) then
				GameTcp.ReConnect()
			end
		elseif State.cur_state == State.state.ss_battle then
			if BattleTcp.Isconnect then
				BattleTcp.Disconnect()
			else
				GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('GameTcp_006'),Config.get_t_script_str('GameTcp_007'), BattleTcp.DisconnectClick})
			end
		elseif State.cur_state == State.state.ss_ofbattle then
			GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('GameTcp_006'),Config.get_t_script_str('GameTcp_007'), BattleTcp.DisconnectClick})
		end
	end
end
