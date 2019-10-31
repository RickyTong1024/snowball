BattleTcp = {}

local self_dis_ = 0
local ip_
local port_
local reconnect_ = false

function BattleTcp.Connect(ip, port)
	self_dis_ = 0
	reconnect_ = false
	ip_ = ip
	port_ = port
	networkMgr:UdpConnect("BattleTcp", ip, port)
	GUIRoot.ShowGUI('MaskPanel', {Config.get_t_script_str('BattleTcp_001')})
end

function BattleTcp.Disconnect()
	self_dis_ = 1
	networkMgr:Disconnect("BattleTcp")
	BattleStateTcp.Disconnect()
end

function BattleTcp.Disconnect1()
	self_dis_ = 2
	networkMgr:Disconnect("BattleTcp")
	BattleStateTcp.Disconnect()
end

function BattleTcp.Disconnect2()
	self_dis_ = 3
	networkMgr:Disconnect("BattleTcp")
	BattleStateTcp.Disconnect()
end

function BattleTcp.Disconnect3()
	self_dis_ = 4
	networkMgr:Disconnect("BattleTcp")
	BattleStateTcp.Disconnect()
end

function BattleTcp.Isconnect()
	return networkMgr:Isconnect("BattleTcp")
end

function BattleTcp.Send(opcode, data)
	if data then
		networkMgr:SendMessage("BattleTcp", opcode, data)
	else
		networkMgr:SendMessageNull("BattleTcp", opcode)
	end
end

function BattleTcp.GetPing()
	return networkMgr:GetPing("BattleTcp")
end

function BattleTcp.OnConnect()
	GUIRoot.HideGUI('MaskPanel')
	if not reconnect_ then
		State.ChangeState(State.state.ss_battle)
		BattleStateTcp.Connect()
	else
		GUIRoot.ShowGUI("LoadPanel", {false, nil})
		Battle.Start()
	end
end

function BattleTcp.OnConnectFail()
	GUIRoot.HideGUI('MaskPanel')
	if not reconnect_ then
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('BattleTcp_002'), Config.get_t_script_str('BattleTcp_003')})
	else
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('BattleTcp_004'), Config.get_t_script_str('BattleTcp_005'), BattleTcp.DisconnectClick})
	end
end

function BattleTcp.OnDisconnect()
	log("BattleTcp Disconnect")
	if self_dis_ == 0 then
		if not Battle.is_end then
			--GUIRoot.ShowGUI("SelectPanel", {"与服务器断开连接", "重新连接", BattleTcp.Reconnect})
			BattleTcp.Reconnect()
		end
	elseif self_dis_ == 1 then
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('BattleTcp_006'), Config.get_t_script_str('BattleTcp_005'), BattleTcp.DisconnectClick})
	elseif self_dis_ == 2 then
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('BattleTcp_007'), Config.get_t_script_str('BattleTcp_005'), BattleTcp.DisconnectClick})
	elseif self_dis_ == 4 then
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('BattleTcp_008'), Config.get_t_script_str('BattleTcp_005'), BattleTcp.DisconnectClick})
	end
end

function BattleTcp.DisconnectClick()
	State.ChangeState(State.state.ss_hall)
end

function BattleTcp.Reconnect()
	reconnect_ = true
	networkMgr:UdpConnect("BattleTcp", ip_, port_)
	GUIRoot.ShowGUI('MaskPanel', {Config.get_t_script_str('BattleTcp_009')})
end
