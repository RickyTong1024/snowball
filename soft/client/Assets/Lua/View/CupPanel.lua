CupPanel = {}

local lua_script_

local main_panel_

local cup_view_
local time_label_

local arrow_left_
local arrow_right_
local cur_tip_
local reward_root_

local view_softness = Vector2(10, 0)
local reward_pos = Vector3(0, -175, 0)

local off_x = 15

local cur_cup_id = 0
local cur_show_cup = nil

local is_show_ = false

function CupPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	main_panel_ = obj.transform:Find("main_panel")
	
	cup_view_ = main_panel_:Find('cup_view')
	arrow_left_ = main_panel_:Find('left_arrow')
	arrow_right_ = main_panel_:Find('right_arrow')
	reward_root_ = main_panel_:Find("reward_root")
	cur_tip_ = main_panel_:Find("cur_tip")
	
	cup_view_:GetComponent("UIPanel").clipSoftness = view_softness
	
	time_label_ = main_panel_:Find("time"):GetComponent("UILabel")
	
	local close_btn = main_panel_:Find('close_btn')
	
	lua_script_:AddButtonEvent(close_btn.gameObject, "click", CupPanel.Click)
	lua_script_:AddButtonEvent(arrow_left_.gameObject, "click", CupPanel.Click)
	lua_script_:AddButtonEvent(arrow_right_.gameObject, "click", CupPanel.Click)
	
	CupPanel.RegisterMessage()
	main_panel_.gameObject:SetActive(false)
	CupPanel.InitCupInf(self.player.cup)
	cur_cup_id = Config.get_t_cup(self.player.cup).duan
	GUIRoot.UIEffectScalePos(main_panel_.gameObject, true, 1)
	CupPanel.Refresh()
	timerMgr:AddRepeatTimer("CupPanel", CupPanel.Refresh, 60, 60)
end

function CupPanel.OnDestroy()
	cur_show_cup = nil
	cur_cup_id = 0
	is_show_ = false
	CupPanel.RemoveMessage()
end

function CupPanel.RegisterMessage()
	Message.register_handle("team_join_msg", CupPanel.TeamJoin)
end

function CupPanel.RemoveMessage()
	Message.remove_handle("team_join_msg", CupPanel.TeamJoin)
end

function CupPanel.TeamJoin()
	GUIRoot.HideGUI('CupPanel')
end

----------------------------初始化界面-----------------------

function CupPanel.InitCupInf(id)
	if(cur_show_cup == nil) then
		local cur_cup = IconPanel.GetCup(id)
		cur_cup.transform.parent = cup_view_
		cur_cup.transform.localScale = Vector3.one
		cur_cup.transform.localPosition = Vector3(0, -70, 0)
		cur_show_cup = cur_cup
		cur_show_cup:SetActive(true)
	end
	cur_tip_.gameObject:SetActive(false)
	local single_label = main_panel_:Find("single"):GetComponent("UILabel")
	local team_label = main_panel_:Find("team"):GetComponent("UILabel")
	local cup_temp = Config.get_t_cup(id)
	if(cup_temp.duan == Config.get_t_cup(self.player.cup).duan) then
		cur_tip_.gameObject:SetActive(true)
	end
	if(reward_root_.childCount > 0) then
		for i = 0, reward_root_.childCount - 1 do
            GameObject.Destroy(reward_root_:GetChild(i).gameObject)
        end
	end
	for i = 1, #cup_temp.rewards do
		local reward_temp = Config.get_t_reward(cup_temp.rewards[i].type, cup_temp.rewards[i].value1)
		if(reward_temp ~= nil) then
			local reward_t = IconPanel.GetIcon("reward_res", nil, reward_temp.icon, reward_temp.color, cup_temp.rewards[i].value2)
			reward_t.transform:Find("icon").name = cup_temp.rewards[i].type.."+"..reward_temp.id
			reward_t.transform.parent = reward_root_
			reward_t.transform.localPosition = Vector3(-(i - 1) * 90, 0, 0)
			reward_t.transform.localScale = Vector3.one
			reward_t.gameObject:SetActive(true)
		end
	end
	local content = string.format(Config.get_t_script_str('CupPanel_001'),cup_temp.sb)
	local single_text =  content --"大乱斗模式排名前"..cup_temp.sb.."，加1星"
	content = string.format(Config.get_t_script_str('CupPanel_002'),single_text,cup_temp.jb)
	if(cup_temp.jb > 0) then
		single_text = content --single_text.."；排名未达到前"..cup_temp.jb.."，减1星"
	end
	local team_text = ""
	if(cup_temp.tsbnum > 0) then
		content = string.format(Config.get_t_script_str('CupPanel_003'),cup_temp.tsb,cup_temp.tsbnum)
		team_text = content --"雪地会战模式队伍排名前"..cup_temp.tsb.."且个人得分达到"..cup_temp.tsbnum.."，加1星"
	else
		content = string.format(Config.get_t_script_str('CupPanel_004'),cup_temp.tsb)
		team_text = content --"雪地会战模式队伍排名前"..cup_temp.tsb.."，加1星"
	end
	if(cup_temp.tjb > 0) then
		content = string.format(Config.get_t_script_str('CupPanel_005'),team_text,cup_temp.tjb)
		team_text = content --team_text.."；队伍排名未达到前"..cup_temp.tjb.."，减1星"
	end
	single_label.text = single_text
	team_label.text = team_text
	CupPanel.RefreshArrow(cup_temp.duan)
	main_panel_.gameObject:SetActive(true)
end

-----------------------------------------------------------

function CupPanel.Refresh()
	local time = tonumber(self.player.last_week_time) + 7 * 86400000 - tonumber(timerMgr:now_string())
	if(time < 0) then
		time = 0
	end
	time_label_.text = count_time_day(time, 2)
end

function CupPanel.Click(obj)
	if(obj.name == "close_btn") then
		GUIRoot.HideGUI("CupPanel")
	elseif(obj.name == "left_arrow") then
		CupPanel.SwitchPage(true)
	elseif(obj.name == "right_arrow") then
		CupPanel.SwitchPage(false)
	end
end

function CupPanel.SwitchPage(is_left)
	if(is_show_) then
		return 0
	end
	if(is_left) then
		cur_cup_id = cur_cup_id - 1
	else
		cur_cup_id = cur_cup_id + 1
	end
	local pre_cup = cur_show_cup
	if(cur_cup_id == Config.get_t_cup(self.player.cup).duan) then
		cur_show_cup = IconPanel.GetCup(self.player.cup)
		CupPanel.InitCupInf(self.player.cup)
	else
		if(cur_cup_id < Config.get_t_cup(self.player.cup).duan) then
			cur_show_cup = IconPanel.GetCup(Config.get_t_cup_lv(cur_cup_id).id, true)
		else
			cur_show_cup = IconPanel.GetCup(Config.get_t_cup_lv(cur_cup_id).id)
		end
		CupPanel.InitCupInf(Config.get_t_cup_lv(cur_cup_id).id)
	end
	cur_show_cup.transform.parent = cup_view_
	cur_show_cup.transform.localScale = Vector3.one
	if(is_left) then
		cur_show_cup.transform.localPosition = Vector3(-200, -70, 0)
		twnMgr:Add_Tween_Postion(pre_cup, 0.2, pre_cup.transform.localPosition, Vector3(200, -70, 0))
		twnMgr:Add_Tween_Postion(cur_show_cup, 0.2, cur_show_cup.transform.localPosition, Vector3(0, -70, 0))
	else
		cur_show_cup.transform.localPosition = Vector3(200, -70, 0)
		twnMgr:Add_Tween_Postion(pre_cup, 0.2, pre_cup.transform.localPosition, Vector3(-200, -70, 0))
		twnMgr:Add_Tween_Postion(cur_show_cup, 0.2, cur_show_cup.transform.localPosition, Vector3(0, -70, 0))
	end
	GameObject.Destroy(pre_cup, 1)
	cur_show_cup:SetActive(true)
	timerMgr:AddTimer("CupPanel", CupPanel.EndShow, 0.2)
	is_show_ = true
end

function CupPanel.EndShow()
	is_show_ = false
end

function CupPanel.RefreshArrow(duan)
	arrow_left_.gameObject:SetActive(true)
	arrow_right_.gameObject:SetActive(true)
	if(Config.get_t_cup_lv(duan + 1) == nil) then
		arrow_right_.gameObject:SetActive(false)
	end
	if(Config.get_t_cup_lv(duan - 1) == nil) then
		arrow_left_.gameObject:SetActive(false)
	end
end
