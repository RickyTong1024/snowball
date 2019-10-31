LevelUpPanel = {}

local old_lb_
local new_lb_
local cj_panel_

function LevelUpPanel.Awake(obj)
	local lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	old_lb_ = obj.transform:Find('old_level/num'):GetComponent('UILabel')
	new_lb_ = obj.transform:Find('new_level/num'):GetComponent('UILabel')
	local bg = obj.transform:Find('bg').gameObject
	lua_script_:AddButtonEvent(bg, "click", LevelUpPanel.OnClick)
	lua_script_:PlaySound('levelup')
end

function LevelUpPanel.OnParam(parm)
	old_lb_.text = parm[1]
	new_lb_.text = parm[2]
	cj_panel_ = parm[3]
end

function LevelUpPanel.OnClick(obj)
	if obj.name == 'bg' then
		GUIRoot.HideGUI('LevelUpPanel')
		if cj_panel_ ~= nil then
			AchieveAnimation.CheckHallAchieve()  --检查下是否有新的成就完成
			cj_panel_ = nil
		end
	end
end

function LevelUpPanel.OnDestroy()
	
end
