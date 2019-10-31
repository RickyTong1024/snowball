RoleGetPanel = {}

local lua_script_


local hecheng_panel_
local hecheng_name_
local hide_panel_ = {}

local role_list_ = {}
local show_rewards = {}
local cur_index_ = 1

function RoleGetPanel.Awake(obj)
	mapMgr:SetTargetCam(-0.533 * panelMgr:get_wh(), 0.6, 3)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')

	hecheng_panel_ = obj.transform:Find('HechengPanel').gameObject
	
	local ok_btn = obj.transform:Find('HechengPanel/Anchor_bottom/ok_btn').gameObject
	local share_btn = obj.transform:Find('HechengPanel/Anchor_bottom/share_btn').gameObject
	
	lua_script_:AddButtonEvent(ok_btn, "click", RoleGetPanel.HechengOK)
	lua_script_:AddButtonEvent(share_btn, "click", RoleGetPanel.Share)
	
	RoleGetPanel.RegisterMessage()
	hecheng_name_ = obj.transform:Find('HechengPanel/Anchor_bottom/name'):GetComponent('UILabel')
	hecheng_panel_.gameObject:SetActive(false)
end

function RoleGetPanel.OnDestroy()
	hide_panel_ = {}
	role_list_ = {}
	show_rewards = {}
	cur_index_ = 1
	RoleGetPanel.RemoveMessage()
end

function RoleGetPanel.OnParam(parm)
	role_list_ = parm[1]
	local role_temp = Config.get_t_role(role_list_[cur_index_].template_id)
	hide_panel_ = parm[2]
	show_rewards = parm[3]
	if(show_rewards == nil) then
		show_rewards = {}
	end
	mapMgr:SetVCam(0, 0.6, 3)
	mapMgr:SetCurCam(50, 0, 0)
	HallScene.AddShowRole(role_temp.id)
	hecheng_name_.text = role_temp.name
	IconPanel.InitQualityLabel(hecheng_name_, role_temp.color % 10)
	for i = 1, #hide_panel_ do
		GUIRoot.HideGUI(hide_panel_[i], false)
	end
	hecheng_panel_.gameObject:SetActive(true)
end

function RoleGetPanel.TeamJoin()
	GUIRoot.HideGUI("RoleGetPanel")
	mapMgr:SetVCam(-0.533 * panelMgr:get_wh(), 0.6, 3)
	mapMgr:SetCurCam(0, 0, 0)
	HallScene.RemoveShowRole()
	for i = 1, #hide_panel_ do
		GUIRoot.HideGUI(hide_panel_[i], true)
	end
end

function RoleGetPanel.RegisterMessage()
	Message.register_handle("team_join_msg", RoleGetPanel.TeamJoin)
end

function RoleGetPanel.RemoveMessage()
	Message.remove_handle("team_join_msg", RoleGetPanel.TeamJoin)
end

function RoleGetPanel.HechengOK()
	cur_index_ = cur_index_ + 1
	if(cur_index_ > #role_list_) then
		GUIRoot.HideGUI("RoleGetPanel")
		mapMgr:SetVCam(-0.533 * panelMgr:get_wh(), 0.6, 3)
		mapMgr:SetCurCam(0, 0, 0)
		HallScene.RemoveShowRole()
		for i = 1, #hide_panel_ do
			GUIRoot.ShowGUI(hide_panel_[i])
		end

		if(#show_rewards > 0) then
			GUIRoot.ShowGUI('GainPanel', {show_rewards})
		elseif #show_rewards == 0 then
			AchieveAnimation.CheckHallAchieve()
		end
	else
		local role_temp = Config.get_t_role(role_list_[cur_index_].template_id)
		HallScene.AddShowRole(role_temp.id)
		hecheng_name_.text = role_temp.name
	end
end

function RoleGetPanel.Share()
	GUIRoot.ShowGUI("SharePanel", {2, "new_role"})
end

