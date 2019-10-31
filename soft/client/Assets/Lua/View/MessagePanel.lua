MessagePanel = {}

local lua_script_
local label_des_
local bg_
local time_ = 0
local ui_panel_

--启动事件--
function MessagePanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	ui_panel_ = obj.transform:GetComponent('UIPanel')
	label_des_ = obj.transform:Find('Anchor_top/baseboard/des'):GetComponent('UILabel')
	bg_ = obj.transform:Find('Anchor_top/baseboard'):GetComponent("UIWidget")
end

function MessagePanel.OnDestroy(obj)
	UpdateBeat:Remove(MessagePanel.Update, MessagePanel)
end

function MessagePanel.OnParam(param)
	label_des_.text = param[1]
	label_des_:ProcessText()
	local height = label_des_.transform:GetComponent('UIWidget').height
	local width = label_des_.transform:GetComponent('UIWidget').width
	if(width > 400) then
		bg_.width = width + 100
	end
	time_ = 5
	UpdateBeat:Remove(MessagePanel.Update, MessagePanel)
	UpdateBeat:Add(MessagePanel.Update, MessagePanel)
	lua_script_:PlaySound("hint")
end

function MessagePanel.Update()
	time_ = time_ - Time.deltaTime
	if time_ <= 0 then
		GUIRoot.HideGUI('MessagePanel')
		return
	end
	if time_ < 2 then
		ui_panel_.alpha = time_ / 2
	else
		ui_panel_.alpha = 1
	end
end
