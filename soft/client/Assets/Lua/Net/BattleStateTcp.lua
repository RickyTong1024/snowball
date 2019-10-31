BattleStateTcp = {}

local ip_
local port_

function BattleStateTcp.Init(ip,port)
	ip_ = ip
	port_ = port
end

function BattleStateTcp.Connect()
	if not Battle.is_end then
		networkMgr:Connect("BattleStateTcp", ip_, port_)
	end
end

function BattleStateTcp.Disconnect()
	networkMgr:Disconnect("BattleStateTcp")
end

function BattleStateTcp.OnDisconnect()
	if BattleTcp.Isconnect() then
		BattleStateTcp.Connect()
	end
end

function BattleStateTcp.OnConnectFail()
	BattleStateTcp.Connect()
end
