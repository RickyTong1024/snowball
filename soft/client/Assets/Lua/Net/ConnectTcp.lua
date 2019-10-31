ConnectTcp = {}

local is_first_ = true

function ConnectTcp.Connect()
	networkMgr:Connect("ConnectTcp", self.host, self.port)
end

function ConnectTcp.Disconnect()
	networkMgr:Disconnect("ConnectTcp")
end

function ConnectTcp.Send(opcode, data)
	if data then
		networkMgr:SendMessage("ConnectTcp", opcode, data)
	else
		networkMgr:SendMessageNull("ConnectTcp", opcode)
	end
end

function ConnectTcp.OnConnect()
	Message.register_net_handle(opcodes.SMSG_REQUEST_GATE, ConnectTcp.SMSG_REQUEST_GATE)
	ConnectTcp.Send(opcodes.CMSG_REQUEST_GATE)
end

function ConnectTcp.OnConnectFail()
	LoginPanel.Stop()
	GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('ConnectTcp_001'),Config.get_t_script_str('ConnectTcp_002')})
end

function ConnectTcp.OnDisconnect()
	Message.remove_net_handle(opcodes.SMSG_REQUEST_GATE, ConnectTcp.SMSG_REQUEST_GATE)
end

function ConnectTcp.SMSG_REQUEST_GATE(message)
	local msg = msg_connect_pb.smsg_request_gate()
	msg:ParseFromString(message.luabuff)
	ConnectTcp.Disconnect()
	GameTcp.Connect(msg.ip, msg.port)
end
