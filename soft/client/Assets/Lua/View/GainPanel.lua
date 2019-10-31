GainPanel = {}

local lua_script_
local main_panel_
local reward_root_
local reward_list_ = {}
local reward_num_ = 0

function GainPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	main_panel_ = obj.transform:Find("main_panel")
	GUIRoot.UIEffectScalePos(main_panel_.gameObject, true, 1)
	reward_root_ = main_panel_:Find('reward_root')
	local bg = obj.transform:Find('bg')
	lua_script_:AddButtonEvent(bg.gameObject, "click", GainPanel.Click)
end

function GainPanel.OnDestroy()
	reward_list_ = {}
	reward_num_ = 0
end

function GainPanel.OnParam(parm)
	reward_list_ = parm[1]
	GainPanel.InitRewardList()
	AchieveAnimation.SetDelayStatus(true)
end

-----------------刷新列表--------------------

function GainPanel.InitRewardList()
	lua_script_:PlaySound("get_item")
	if(reward_root_.childCount > 0) then
		for i = 0, reward_root_.childCount - 1 do
            GameObject.Destroy(reward_root_:GetChild(i).gameObject)
        end
	end
	local num = 0
	if(#reward_list_ - reward_num_ * 3 >= 3) then
		num = 3
	else
		num = #reward_list_ - reward_num_ * 3
	end
	local refer_pos = -(num - 1)/2 * 120
	for i = 1, num do
		local reward_parm = reward_list_[reward_num_ * 3 + i]
		local reward_temp = Config.get_t_reward(reward_parm.type, reward_parm.value1)
		if(reward_parm.type == 6) then
			reward_parm.value2 = 0
		end
		if(reward_temp ~= nil) then
			local reward_t = IconPanel.GetIcon("gain_reward_res", nil, reward_temp.icon, reward_temp.color, reward_parm.value2)
			reward_t.transform.parent = reward_root_
			reward_t.transform.localScale = Vector3.zero
			reward_t.transform.localPosition = Vector3(refer_pos + (i - 1) * 120, 0, 0)
			local icon = reward_t.transform:Find('icon')
			local name_label = reward_t.transform:Find('name'):GetComponent('UILabel')
			icon.name = reward_parm.type.."+"..reward_temp.id
			name_label.text = reward_temp.name
			IconPanel.InitQualityLabel(name_label, reward_temp.color % 10)
			local from = reward_t.transform.localPosition + Vector3(10, 0, 0)
			twnMgr:Add_Tween_Scale(reward_t, 0.5, Vector3(0, 0, 0), Vector3(1, 1, 1), 4, 0.6)
			reward_t:SetActive(true)
		end
	end
	local effect_b = LuaHelper.Instantiate(main_panel_:Find("effect_b").gameObject)
	effect_b.transform.parent = main_panel_
	effect_b.transform.localScale = Vector3.one
	effect_b.transform.localPosition = main_panel_:Find("effect_b").localPosition
	effect_b:SetActive(true)
	GameObject.Destroy(effect_b, 3)
	main_panel_:Find("baseboard"):GetComponent("UIRect").alpha = 0
	twnMgr:Add_Tween_Alpha(main_panel_:Find("baseboard").gameObject, 0.2, 0, 1)
	
	main_panel_:Find("title"):GetComponent("UIRect").alpha = 0
	main_panel_:Find("title2"):GetComponent("UIRect").alpha = 0
	main_panel_:Find("title").localPosition = Vector3(43.125, 123, 0)
	--main_panel_:Find("title2").localPosition = Vector3(97, 118, 0)
	twnMgr:Add_Tween_Alpha(main_panel_:Find("title").gameObject, 0.2, 0, 1, 1, 0.2)
	twnMgr:Add_Tween_Alpha(main_panel_:Find("title2").gameObject, 0.2, 0, 1, 1, 0.2)
	twnMgr:Add_Tween_Postion(main_panel_:Find("title").gameObject, 0.2, Vector3(43.125, 123, 0), Vector3(43.125, 103, 0), 1, 0.4)
	--twnMgr:Add_Tween_Postion(main_panel_:Find("title2").gameObject, 0.2, Vector3(97, 118, 0), Vector3(97, 103, 0), 1, 0.4)
	
	main_panel_:Find("bg").localScale = Vector3.zero
	twnMgr:Add_Tween_Scale(main_panel_:Find("bg").gameObject, 0.2, Vector3(0, 0, 0), Vector3(1, 1, 1), 1, 0.2)
	
	main_panel_:Find("effect"):GetComponent("UIRect").alpha = 0
	twnMgr:Add_Tween_Alpha(main_panel_:Find("effect").gameObject, 0.2, 0, 1, 4, 0.4)
	
	local mark = main_panel_:Find("mark").gameObject
	mark.transform:GetComponent("UIRect").alpha = 0
	local from = mark.transform.localPosition + Vector3(0, 20, 0)
	twnMgr:Add_Tween_Postion(mark, 0.2, from, mark.transform.localPosition, 1, 0.4)
	twnMgr:Add_Tween_Alpha(mark, 0.2, 0, 1, 1, 0.4)
end


---------------------ButtonEvent-------------------------

function GainPanel.Click(obj)
	if(obj.name == 'bg') then
		reward_num_ = reward_num_ + 1
		if(reward_num_ * 3 >= #reward_list_) then
			GainPanel.ClosePanel()
			if self.player.level > self.old_level then
				GUIRoot.ShowGUI('LevelUpPanel',{self.old_level,self.player.level})
				self.old_level = self.player.level
			end
			AchieveAnimation.SetDelayStatus(false)
		else
			GainPanel.InitRewardList()
		end
	else
	end
end

function GainPanel.ClosePanel()
	GUIRoot.HideGUI("GainPanel")
end