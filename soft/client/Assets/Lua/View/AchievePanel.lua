AchievePanel = {}

local achieve_view_panel_
local achieve_point_panel_

local achieve_view_
local achieve_res_
local cur_ap_label_
local achieve_point_view_
local achieve_point_res_

local select_view_page_

--当前领取的奖励
local send_achieve_id_
local send_achieve_reward_
local achieve_point_
--特效粒子
local lizi_yellow_obj_
local lizi_panel_
local pos_t_ = nil
--
local acount_btn_label_
local battle_btn_label_
local kill_btn_label_
local skill_btn_label_
local exp_btn_label_

local acount_btn_red_
local battle_btn_red_
local kill_btn_red_
local skill_btn_red_
local exp_btn_red_

function AchievePanel.Awake(obj)
	GUIRoot.ShowGUI('BackPanel', {4})
	GUIRoot.UIEffect(obj, 0)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	achieve_view_panel_ = obj.transform:Find('achieveObj')
	achieve_point_panel_ = obj.transform:Find('achievePoint')
	
	achieve_point_view_ = achieve_point_panel_.transform:Find("achieve_view")
	achieve_point_res_ = achieve_point_panel_.transform:Find("achievePoint_res")
	achieve_view_ = achieve_view_panel_.transform:Find('achieve_view')
	achieve_res_ = achieve_view_panel_.transform:Find('achieve_res')
	cur_ap_label_ = achieve_view_panel_.transform:Find("bottom/point"):GetComponent("UILabel")

	local acount_btn = achieve_view_panel_.transform:Find('left_panel/acountBtn')
	acount_btn_label_ = acount_btn.transform:Find("Label"):GetComponent("UILabel")
	acount_btn_red_ = acount_btn.transform:Find("red").gameObject
	
	local battle_btn = achieve_view_panel_.transform:Find('left_panel/battleBtn')
	battle_btn_label_ = battle_btn.transform:Find("Label"):GetComponent("UILabel")
	battle_btn_red_ = battle_btn.transform:Find("red").gameObject
	
	local kill_btn = achieve_view_panel_.transform:Find('left_panel/killBtn')
	kill_btn_label_ = kill_btn.transform:Find("Label"):GetComponent("UILabel")
	kill_btn_red_ = kill_btn.transform:Find("red").gameObject
	
	local skill_btn = achieve_view_panel_.transform:Find('left_panel/skillBtn')
	skill_btn_label_= skill_btn.transform:Find("Label"):GetComponent("UILabel")
	skill_btn_red_= skill_btn.transform:Find("red").gameObject
	
	local exp_btn = achieve_view_panel_.transform:Find('left_panel/expBtn')
	exp_btn_label_= exp_btn.transform:Find("Label"):GetComponent("UILabel")
	exp_btn_red_ = exp_btn.transform:Find("red").gameObject
	
	local achievePoint_btn = achieve_view_panel_.transform:Find('bottom/achieveBtn')
	local achieve_reward_btn = achieve_point_panel_.transform:Find("bottom/reward_btn")
	
	lizi_panel_ = obj.transform:Find("effectpanel").gameObject
	lizi_yellow_obj_ = lizi_panel_.transform:Find("movePoint1").gameObject
	achieve_point_= obj.transform:Find("achieveObj/bottom/point").gameObject
	
	lua_script_:AddButtonEvent(acount_btn.gameObject, "click", AchievePanel.Click)
	lua_script_:AddButtonEvent(battle_btn.gameObject, "click", AchievePanel.Click)
	lua_script_:AddButtonEvent(kill_btn.gameObject, "click", AchievePanel.Click)
	lua_script_:AddButtonEvent(skill_btn.gameObject, "click", AchievePanel.Click)
	lua_script_:AddButtonEvent(exp_btn.gameObject, "click", AchievePanel.Click)
	lua_script_:AddButtonEvent(achievePoint_btn.gameObject,"click",AchievePanel.Click)
	lua_script_:AddButtonEvent(achieve_reward_btn.gameObject,"click",AchievePanel.Click)
	
	AchievePanel.RegisterMessage()
	
	select_view_page_ = 1
	AchievePanel.InitList()
	pos_t_ = {length = 0,paths = {},r_time={},funcs = {},delays = {}} --特效位置集合
	UpdateBeat:Add(AchievePanel.Update, AchievePanel)
end

function AchievePanel.RegisterMessage()
	Message.register_handle("back_panel_msg", AchievePanel.Back)
	Message.register_handle("back_panel_recharge",AchievePanel.Recharge)
	Message.register_handle("team_join_msg", AchievePanel.TeamJoin)
end

function AchievePanel.RemoveMessage()
	Message.remove_handle("back_panel_msg", AchievePanel.Back)
	Message.remove_handle("back_panel_recharge",AchievePanel.Recharge)
	Message.remove_handle("team_join_msg", AchievePanel.TeamJoin)
end

function AchievePanel.OnDestroy()
	achieve_list = {}
	AchievePanel.RemoveMessage()
	UpdateBeat:Remove(AchievePanel.Update, AchievePanel)
end

function AchievePanel.Back()
	GUIRoot.HideGUI("AchievePanel")
	GUIRoot.HideGUI("BackPanel")
	AchievePanel.OnDestroy()
	GUIRoot.ShowGUI("HallPanel")
end

function AchievePanel.Update()
	AchievePanel.Animation()
end

function AchievePanel.TeamJoin()
	GUIRoot.HideGUI('AchievePanel')
end

function AchievePanel.GetRewardParticle(from_obj,to_obj,p_num)

	local InitObj = function(obj,parent_obj)
		obj.transform.parent = parent_obj.transform
		obj.transform.localPosition = Vector3.zero
		obj.transform.localScale = Vector3.one
		obj:SetActive(true)
	end
	
	pos_t_.paths[pos_t_.length] = {task = {},IsMulti = true}
	
	if p_num < 1 or p_num > 15 then
		p_num = 2
	end
	
	for i =1,p_num do
		local clone_obj = LuaHelper.Instantiate(lizi_yellow_obj_)
		clone_obj.transform.parent = lizi_panel_.transform
		clone_obj.transform.localScale = Vector3.one
		clone_obj.transform.localPosition = Vector3.zero
		clone_obj:SetActive(true)
		--替换技能 特效
		
		local v = from_obj.transform:TransformPoint(clone_obj.transform.localPosition)
		local from_pos = clone_obj.transform:InverseTransformPoint(v)
		from_pos.z = 2
		
		v = to_obj.transform:TransformPoint(clone_obj.transform.localPosition)
		local to_pos = clone_obj.transform:InverseTransformPoint(v)
		to_pos.z = 2
		

		local x_1 = math.random(from_pos.x,to_pos.x)
		local x_2 = math.random(from_pos.x,to_pos.x)

		local y_1 = math.random(from_pos.y,to_pos.y)
		local y_2 = math.random(from_pos.y,to_pos.y)
		
		local p1 = Vector3(x_1,y_1,2)
		local p2 = Vector3(x_2,y_2,2)
			
		local path = {from_pos,to_pos,p1,p2,0,1,clone_obj}
		pos_t_.paths[pos_t_.length].task[i] = path
	end
	
	local ex = {}
	ex.execute = function ()
		for i =1,p_num do
			AchievePanel.DigitFollow()
		end
	end
	
	pos_t_.r_time[pos_t_.length] = 0
	pos_t_.funcs[pos_t_.length] = ex
	pos_t_.length = pos_t_.length + 1
end

local DigitQueue_ = {length = 0,queues = {},star_pos = 0}

--数字跳动
function AchievePanel.DigitFollow()
	local pos = DigitQueue_.length
	local digit_event = {x_offset = 10,cur_time = 0,total_time = 0.1,CompleteState = false}
	
	digit_event.execute = function(self)
		local timeSpan = Time.deltaTime / Time.timeScale
		local y = self.cur_time / self.total_time * self.x_offset
		cur_ap_label_.transform.localPosition = Vector3(-126,-258,0) + Vector3(0,y,0)
		self.cur_time = self.cur_time + timeSpan
		
		if self.cur_time >= self.total_time then
			self.CompleteState = true
			cur_ap_label_.transform.localPosition = Vector3(-126,-258,0)
			local num = tonumber(cur_ap_label_.text)
			if num == nil then
				num = 0
			end
			cur_ap_label_.text =  num + 1
		end
	end
	
	digit_event.IsComplete = function (self)
		return self.CompleteState
	end

	DigitQueue_.queues[pos] = digit_event
	DigitQueue_.length = DigitQueue_.length + 1
end
-----------------------刷新列表----------------------- 

function AchievePanel.InitList(updatePoint)
	
	if achieve_point_panel_.gameObject.activeSelf then
		achieve_point_panel_.gameObject:SetActive(false)
	end

	if not achieve_view_panel_.gameObject.activeSelf then
		achieve_view_panel_.gameObject:SetActive(true)
	end
	
	AchievePanel.SetTagSelectColor()
	
	if(achieve_view_.childCount > 1) then
		for i = 0, achieve_view_.childCount - 1 do
            GameObject.Destroy(achieve_view_:GetChild(i).gameObject)
        end
	end
	
	local uv = achieve_view_:GetComponent('UIScrollView')
	if achieve_view_:GetComponent('SpringPanel') ~= nil then
		achieve_view_:GetComponent('SpringPanel').enabled = false
	end
	
	uv.panel.clipOffset = Vector2.zero
	achieve_view_.localPosition = Vector3.zero

	local dt = Config.t_achievement.tags[select_view_page_]	--tag_id
	local showAchieves = {}
	
	for i = 1,#dt do
		local data = dt[i]
		if Config.t_achievement.achieveTrees[data.id] ~= nil then
			
			local reward_state = AchievePanel.GetAchieveReward(data.id)

			if not reward_state then
				table.insert(showAchieves,data)	
			elseif #Config.t_achievement.achieveTrees[data.id] == 0 then
				table.insert(showAchieves,data)
			else
				for j = 1,#Config.t_achievement.achieveTrees[data.id] do				
					local indt = Config.t_achievement.achieveTrees[data.id][j]
					reward_state = AchievePanel.GetAchieveReward(indt.id)
					if not reward_state then
						table.insert(showAchieves,indt)
						break
					elseif j == #Config.t_achievement.achieveTrees[data.id] then
						table.insert(showAchieves,indt)
						break
					end
				end
			end
		end
	end
	
	local comps = function(a, b)
		local state_id_1 = 2
		local state_id_2 = 2
		
		local reward_state_1 = AchievePanel.GetAchieveReward(a.id)
		local reward_state_2 = AchievePanel.GetAchieveReward(b.id)
		
		local num_1 = self.get_achieve_num(a.id)
		local num_2 = self.get_achieve_num(b.id)
		
		if reward_state_1 then
			state_id_1 = 3
		else
			if num_1 >= a.target_num then
				state_id_1 = 1
			end
		end
		
		if reward_state_2 then
			state_id_2 = 3
		else
			if num_2 >= b.target_num then
				state_id_2 = 1
			end
		end
		
		return state_id_1 < state_id_2
	end
	
	table.sort(showAchieves,comps)
	
	
	for i = 0, #showAchieves - 1 do
		local data = showAchieves[i + 1]
		local achieve_t = LuaHelper.Instantiate(achieve_res_.gameObject)
		achieve_t.transform.parent = achieve_view_
		achieve_t.transform.localPosition = Vector3(-271, -110 * i + 175, 0)
		achieve_t.transform.localScale = Vector3.one
		achieve_t.name = data.id
		achieve_t.transform:GetComponent("UISprite").alpha = 0

		local from = achieve_t.transform.localPosition + Vector3(0, 10, 0)
		twnMgr:Add_Tween_Postion(achieve_t, 0.4, from, achieve_t.transform.localPosition, 3, i * 0.1)
		twnMgr:Add_Tween_Alpha(achieve_t, 0.4, 0, 1, 3, i * 0.1)
		
		AchievePanel.SetAchieveItem(achieve_t,data,true)
		
		achieve_t:SetActive(true)
	end	
	
	if updatePoint == nil then
		cur_ap_label_.text = self.player.achieve_point
	end
	AchievePanel.RewardRedState()
end

--标签页的红点状态
function AchievePanel.RewardRedState()
	local st = false
	local can_get = 0
	for i = 1,5 do
		st =  AchievePanel.GetAllRewardForType(i)
		if i == 1 then
			if st > 0 then
				acount_btn_red_.transform:Find('Label'):GetComponent('UILabel').text = st
				acount_btn_red_:SetActive(true)
			else
				acount_btn_red_:SetActive(false)
			end
			can_get = can_get + st
		elseif i == 2 then
			if st > 0 then
				battle_btn_red_.transform:Find('Label'):GetComponent('UILabel').text = st
				battle_btn_red_:SetActive(true)
			else
				battle_btn_red_:SetActive(false)
			end
			can_get = can_get + st
		elseif i == 3 then
			if st > 0 then
				kill_btn_red_.transform:Find('Label'):GetComponent('UILabel').text = st
				kill_btn_red_:SetActive(true)
			else
				kill_btn_red_:SetActive(false)
			end
			can_get = can_get + st
		elseif i == 4 then
			if st > 0 then
				skill_btn_red_.transform:Find('Label'):GetComponent('UILabel').text = st	
				skill_btn_red_:SetActive(true)
			else
				skill_btn_red_:SetActive(false)
			end
			can_get = can_get + st
		elseif i == 5 then
			if st > 0 then
				exp_btn_red_.transform:Find('Label'):GetComponent('UILabel').text = st	
				exp_btn_red_:SetActive(true)
			else
				exp_btn_red_:SetActive(false)
			end
			can_get = can_get + st
		end
	end
	
	--判断成就点奖励 是否可领
	local a_point_total = 0
	for i =1,#Config.t_achievement_reward do
		local data = Config.t_achievement_reward[i]
		if self.player.achieve_point >= data.point and self.player.achieve_index + 1 == data.id then
			a_point_total = a_point_total + 1
		end
	end
	
	if a_point_total > 0 then
		achieve_view_panel_.transform:Find('bottom/achieveBtn/red/Label'):GetComponent('UILabel').text = a_point_total 
		achieve_view_panel_.transform:Find('bottom/achieveBtn/red').gameObject:SetActive(true)
	else
		achieve_view_panel_.transform:Find('bottom/achieveBtn/red').gameObject:SetActive(false)
	end
	
	if can_get > 0 then
		achieve_point_panel_.transform:Find("bottom/reward_btn/red/Label"):GetComponent('UILabel').text = can_get 
		achieve_point_panel_.transform:Find("bottom/reward_btn/red").gameObject:SetActive(true)
	else
		achieve_point_panel_.transform:Find("bottom/reward_btn/red").gameObject:SetActive(false)
	end
end

function AchievePanel.GetAllRewardForType(type_id)
	local reward_total = 0
	for i = 1,#Config.t_achievement.tags[type_id] do
		local v = Config.t_achievement.tags[type_id][i]
		local k = v.id
		if self.get_achieve_num(k) >= v.target_num then
			local sign = true
			for i = 1,#self.player.achieve_reward do
				if self.player.achieve_reward[i] == v.id then
					sign = false
					break
				end
			end

			if sign then
				reward_total = reward_total + 1
			end
		end
	end
	
	return reward_total
end

function AchievePanel.SetAchieveItem(obj,data,state)
	obj.name = data.id
	
	local tmp_reward = {data.rtype,data.rvalue1,data.rvalue2,data.rvalue3}
	local reward_t = AchievePanel.GetAchievePointIcon(tmp_reward)
	
	if reward_t ~= nil then
		reward_t.transform.parent = obj.transform
		reward_t.transform.localPosition = Vector3(550, 0, 0)
		reward_t.transform.localScale = Vector3.one
		reward_t.transform:Find("icon").name = data.rtype.."+"..data.rvalue1
		reward_t:SetActive(true)
	end
	
	obj.transform:Find("icon"):GetComponent("UISprite").spriteName = data.icon
	
	--按钮 完成状态设置 未完成 未领取 已领取
	local sp = obj.transform:Find("get_achieve_btn"):GetComponent("UISprite")
	local sp_label = obj.transform:Find("get_achieve_btn/Label"):GetComponent("UILabel")
	
	if state ~= nil then
		lua_script_:AddButtonEvent(sp.gameObject,"click",AchievePanel.Click)
	end
	
	local cur_achieve_num = self.get_achieve_num(data.id)
	local reward_state = AchievePanel.GetAchieveReward(data.id)
	local dcobj = obj.transform:Find("dacheng").gameObject
	local lqObj = obj.transform:Find("lqLabel").gameObject
	sp.gameObject:SetActive(true)
	lqObj:SetActive(false)
	sp:GetComponent('BoxCollider').enabled = true
	
	if reward_state then
		dcobj:SetActive(true)
		sp.gameObject:SetActive(false)
		lqObj:SetActive(true)
	elseif cur_achieve_num ~= nil and cur_achieve_num >= data.target_num then
		sp.spriteName = "b1_green-78"
		sp_label.text = "领取"
		dcobj:SetActive(true)
	else
		sp.spriteName = "b1_gray-78"
		sp_label.text = "领取"
		dcobj:SetActive(false)
		sp:GetComponent('BoxCollider').enabled = false
	end
	local nameLabel = obj.transform:Find("name"):GetComponent("UILabel")
	nameLabel.text = data.name
		
	local descLabel = obj.transform:Find("desc"):GetComponent("UILabel")
	descLabel.text = data.desc
		
	--设置 进度条
	local slider = obj.transform:Find("progress/slider"):GetComponent("UISlider")
	local cur_num = self.get_achieve_num(data.id)
	local reward_state = AchievePanel.GetAchieveReward(data.id)
	
	if cur_num == nil and not reward_state then
		cur_num = 0
	elseif reward_state then
		cur_num = data.target_num
	end

	local pert  = cur_num / data.target_num
	if pert <= 0 then
		pert = 0
	elseif pert >= 1 then
		pert = 1
	end
	
	if pert == 1 then
		obj.transform:Find("progress/slider/bg"):GetComponent("UISprite").spriteName = "jdt-16-green"
	else
		obj.transform:Find("progress/slider/bg"):GetComponent("UISprite").spriteName = "jdt-16-blue"
	end
	
	slider.value = pert
		
	--设置 label
	local sliderLabel = obj.transform:Find("progress/slider/desc"):GetComponent("UILabel")
	sliderLabel.text = cur_num.."/"..data.target_num
	--成就点
	local achievePointlabel = obj.transform:Find("achievePointLabel"):GetComponent("UILabel")
	achievePointlabel.text = "[1de7f7]成就点[-][ffe733]+"..data.point.."[-]"
end


function AchievePanel.GetAchieveReward(achieve_id)
	for i =1,#self.player.achieve_reward do
		if self.player.achieve_reward[i] == achieve_id then
			return true
		end
	end
	return false
end

function AchievePanel.InitAchievePoint()
	if achieve_view_panel_.gameObject.activeSelf then
		achieve_view_panel_.gameObject:SetActive(false)
	end
	
	if not achieve_point_panel_.gameObject.activeSelf then
		achieve_point_panel_.gameObject:SetActive(true)
	end
	
	local uv = achieve_point_view_:GetComponent('UIScrollView')
	if achieve_point_view_:GetComponent('SpringPanel') ~= nil then
		achieve_point_view_:GetComponent('SpringPanel').enabled = false
	end
	
	uv.panel.clipOffset = Vector2.zero
	achieve_point_view_.localPosition = Vector3.zero
	
	if achieve_point_view_.childCount > 0 then
		for i =1,achieve_point_view_.childCount do
			GameObject.Destroy(achieve_point_view_:GetChild(i - 1).gameObject)
		end
	end
	
	local first_reward_index = nil
	local total_reward_num = 0
	
	for k,v in pairsByKeys(Config.t_achievement_reward) do
		total_reward_num = total_reward_num + 1
		if self.player.achieve_point >= v.point then
			if v.id == (self.player.achieve_index + 1) then
				first_reward_index = total_reward_num
			end
		end
	end
	
	local offset_num = 0
	if total_reward_num > 4 and  first_reward_index ~= nil then
		if first_reward_index <= 2 then
			offset_num = 0
		elseif first_reward_index == total_reward_num then
			offset_num = first_reward_index - 4
		else
			offset_num = first_reward_index - 3
		end
	end
	

	local index = 0
	for k,v in pairsByKeys(Config.t_achievement_reward) do
		local cloneObj = LuaHelper.Instantiate(achieve_point_res_.gameObject)
		cloneObj.transform.parent = achieve_point_view_.transform
		cloneObj.transform.localPosition = Vector3(-271, -110 * index + 175 + offset_num * 110, 0)
		cloneObj.transform.localScale = Vector3.one
		cloneObj.transform:GetComponent("UISprite").alpha = 0
		cloneObj.name = k
		local descLabel = cloneObj.transform:Find("desc"):GetComponent("UILabel")
		descLabel.text = v.point
		
		--奖励
		for j = 1, #v.rewards do
			local reward_t = AchievePanel.GetAchievePointIcon(v.rewards[j])
			reward_t.transform.parent = cloneObj.transform
			reward_t.transform.localPosition = Vector3(100 + j * 90, 0, 0)
			reward_t.transform.localScale = Vector3.one
			reward_t.transform:Find("icon").name = v.rewards[j][1].."+"..v.rewards[j][2]
			reward_t:SetActive(true)
		end
		
		local from = cloneObj.transform.localPosition + Vector3(0, 10, 0)
		if index - offset_num < 4 and index - offset_num >= 0 then
			local t = index - offset_num
			twnMgr:Add_Tween_Postion(cloneObj, 0.4, from, cloneObj.transform.localPosition, 3, t * 0.1)
			twnMgr:Add_Tween_Alpha(cloneObj, 0.4, 0, 1, 3, t * 0.1)
		else
			twnMgr:Add_Tween_Postion(cloneObj, 0.4, from, cloneObj.transform.localPosition, 3, 0.1)
			twnMgr:Add_Tween_Alpha(cloneObj, 0.4, 0, 1, 3, 0.1)
		end
		--设置button
		local reward_btn = cloneObj.transform:Find("get_point_reward_btn"):GetComponent("UISprite")
		local reward_label = cloneObj.transform:Find("get_point_reward_btn/Label"):GetComponent("UILabel")
		lua_script_:AddButtonEvent(reward_btn.gameObject,"click",AchievePanel.Click)
		
		local dcobj = cloneObj.transform:Find("dacheng").gameObject
		local lqObj = cloneObj.transform:Find("lqLabel").gameObject
		lqObj:SetActive(false)
		reward_btn.gameObject:SetActive(true)
		reward_btn:GetComponent('BoxCollider').enabled = true
		
		if self.player.achieve_point >= v.point then
			if v.id == (self.player.achieve_index + 1) then
				--可以领取
				reward_btn.spriteName = "b1_green-78"
				reward_label.text = "领取"
				dcobj:SetActive(true)
				if not sign then
					first_reward = index
				end
			elseif v.id > (self.player.achieve_index + 1) then
				reward_btn.spriteName = "b1_gray-78"
				reward_label.text = "领取"
				dcobj:SetActive(false)
				reward_btn:GetComponent('BoxCollider').enabled = false
			else
				dcobj:SetActive(true)
				reward_btn.gameObject:SetActive(false)
				lqObj:SetActive(true)
			end
		else
			reward_btn.spriteName = "b1_gray-78"
			reward_label.text = "领取"
			reward_btn:GetComponent('BoxCollider').enabled = false
		end
		
		cloneObj:SetActive(true)
		index = index + 1
	end
		
	--成就点数
	local point_label = achieve_point_panel_.transform:Find("bottom/point"):GetComponent("UILabel")
	point_label.text = self.player.achieve_point
	AchievePanel.RewardRedState()
end

function AchievePanel.GetAchievePointIcon(data)
	--{_type,value1,value2,value3}

	local amount = data[3]
	local attr = data[4]
	
	local color = nil
	local icon = nil
	
	local t_reward = Config.get_t_reward(data[1],data[2])
	
	local reward_obj = nil	
	if t_reward ~= nil then
		if t_reward.color == nil then
			reward_obj = IconPanel.GetIcon("reward_res", nil,t_reward.icon, 1, amount)
		else
			reward_obj = IconPanel.GetIcon("reward_res", nil,t_reward.icon,t_reward.color, amount)
		end
	end

	return reward_obj
end

--收到成就点奖励
function AchievePanel.RecvPointReward()
	if send_achieve_reward_ == nil then
		return
	end
	AchievePanel.UpdateAchievePointBtnState()
end

function AchievePanel.UpdateAchievePointBtnState()
	for i = 1,achieve_point_view_.transform.childCount do
		local obj = achieve_point_view_.transform:GetChild(i - 1).gameObject
		local id = tonumber(obj.name)

		local t_achieve_reward = Config.get_t_achievement_reward(id)
		local reward_btn = obj.transform:Find("get_point_reward_btn"):GetComponent("UISprite")
		local reward_label = obj.transform:Find("get_point_reward_btn/Label"):GetComponent("UILabel")
		
		local dcobj = obj.transform:Find("dacheng").gameObject
		local lqObj = obj.transform:Find("lqLabel").gameObject
		lqObj:SetActive(false)
		reward_btn.gameObject:SetActive(true)
		reward_btn:GetComponent('BoxCollider').enabled = true
		
		if self.player.achieve_point >= t_achieve_reward.point then
			if t_achieve_reward.id == (self.player.achieve_index + 1) then
				--可以领取
				reward_btn.spriteName = "b1_green-78"
				reward_label.text = "领取"
				dcobj:SetActive(true)
			elseif t_achieve_reward.id > (self.player.achieve_index + 1) then
				reward_btn.spriteName = "b1_gray-78"
				reward_label.text = "领取"
				reward_btn:GetComponent('BoxCollider').enabled = false
				dcobj:SetActive(false)
			else
				reward_btn.gameObject:SetActive(false)
				dcobj:SetActive(true)
				lqObj:SetActive(true)
			end
		else
			reward_btn.spriteName = "b1_gray-78"
			reward_label.text = "领取"
			dcobj:SetActive(false)
			reward_btn:GetComponent('BoxCollider').enabled = false
		end
	end
	AchievePanel.RewardRedState()
end
-------------------------------------------------------

function AchievePanel.Recharge()
	GUIRoot.HideGUI('AchievePanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function AchievePanel.SetTagSelectColor()
	acount_btn_label_.text = "[7E90A4]账号"
	battle_btn_label_.text = "[7E90A4]战场"
	kill_btn_label_.text = "[7E90A4]击杀"
	skill_btn_label_.text = "[7E90A4]技能"
	exp_btn_label_.text = "[7E90A4]特殊"
	
	if select_view_page_ == 1 then
		acount_btn_label_.text = "[E8FCFF]账号"
	elseif select_view_page_ == 2 then
		battle_btn_label_.text = "[E8FCFF]战场"
	elseif select_view_page_ == 3 then
		kill_btn_label_.text = "[E8FCFF]击杀"
	elseif select_view_page_ == 4 then
		skill_btn_label_.text = "[E8FCFF]技能"
	elseif select_view_page_ == 5 then
		exp_btn_label_.text = "[E8FCFF]特殊"
	end
end

------------------------ButtonEvent--------------------
--1账号 2战场 3击杀 4技能 5特殊 
function AchievePanel.Click(obj)
	if obj.name == "acountBtn" then
		select_view_page_ = 1
		AchievePanel.InitList()
	elseif obj.name == "battleBtn" then
		select_view_page_ = 2
		AchievePanel.InitList()
	elseif obj.name == "killBtn" then
		select_view_page_ = 3
		AchievePanel.InitList()
	elseif obj.name == "skillBtn" then
		select_view_page_ = 4
		AchievePanel.InitList()
	elseif obj.name == "expBtn" then
		select_view_page_ = 5
		AchievePanel.InitList()
	elseif obj.name == "achieveBtn" then
		AchievePanel.InitAchievePoint()
	elseif obj.name == "reward_btn" then
		AchievePanel.InitList()
	elseif obj.name == "get_achieve_btn" then	
		local reward_id = tonumber(obj.transform.parent.name)
		local dt = Config.get_t_achievement(reward_id)
		if dt == nil then
			return
		end

		--判断当前 是否处于完成状态 
		local sign = false
		for i = 1,#self.player.achieve_reward do
			if self.player.achieve_reward[i] == reward_id then
				sign = true
				break
			end
		end
			
		if sign then
			GUIRoot.ShowGUI("MessagePanel", {"奖励已领取"})
			return
		end
		
		local cur_num = self.get_achieve_num(reward_id)
		if cur_num ~= nil and cur_num >= dt.target_num then
			send_achieve_id_ = reward_id
			BattleAchieve.CMSG_ACHIEVE(reward_id)
		else
			GUIRoot.ShowGUI("MessagePanel", {"成就未完成"})
		end
	elseif obj.name == "get_point_reward_btn" then
		local reward_id = tonumber(obj.transform.parent.name)
		local t_achieve_reward = Config.get_t_achievement_reward(reward_id)
		if self.player.achieve_point < t_achieve_reward.point then
			GUIRoot.ShowGUI("MessagePanel", {"成就点数不够"})
		elseif self.player.achieve_point >= t_achieve_reward.point and t_achieve_reward.id > self.player.achieve_index+1 then
			GUIRoot.ShowGUI("MessagePanel", {"请先领取前置奖励"})
		elseif self.player.achieve_index >= t_achieve_reward.id then
			GUIRoot.ShowGUI("MessagePanel", {"奖励已领取"})
		else
			send_achieve_reward_  = t_achieve_reward
			BattleAchieve.CMSG_ACHIEVE_REWARD(send_achieve_reward_.id)
		end
	end
end


function AchievePanel.RecvReward()

	local t_achieve = Config.get_t_achievement(send_achieve_id_)
	local old_point = t_achieve.point
	local obj = achieve_view_.transform:Find(tostring(send_achieve_id_))
	local sp = obj.transform:Find("get_achieve_btn"):GetComponent("UISprite")
	AchievePanel.GetRewardParticle(sp.gameObject,achieve_point_,old_point)
	
	AchievePanel.InitList(true)
	
	AchievePanel.RewardRedState()
end

--------------------------AchievePanel---------------------------------
function AchievePanel.Animation()

	if DigitQueue_.length > DigitQueue_.star_pos then
		local pos = DigitQueue_.star_pos
		if not DigitQueue_.queues[pos]:IsComplete() then
			DigitQueue_.queues[pos]:execute()
		else
			DigitQueue_.star_pos = DigitQueue_.star_pos + 1
		end
	end

	if pos_t_.length <= 0 then
		return
	end
	
	for k,v in pairs(pos_t_.paths) do
		if v.IsMulti then
			local allComplete = true
			for in_k,in_v in pairs(v.task) do
				if in_v[5] < in_v[6] then
					local t = in_v[5] / in_v[6]
					local p = BezierCurve(in_v[1],in_v[3],in_v[4],in_v[2],t)
					in_v[7].transform.localPosition = p
					in_v[5] = in_v[5] + Time.deltaTime / Time.timeScale
					allComplete = false
				else
					--执行完成执行 结束函数
					in_v[7].transform.localPosition = in_v[2]
					GameObject.Destroy(in_v[7].gameObject)
					pos_t_.paths[k][in_k] = nil
				end
			end
			if allComplete then
				if pos_t_.funcs[k] ~= nil then
					pos_t_.funcs[k]:execute()
				end
				pos_t_.paths[k] = nil
			end
		else
			if pos_t_.r_time[k] < v[5] then
				local t = pos_t_.r_time[k] / v[5]
				local p = BezierCurve(v[1],v[3],v[4],v[2],t)
				v[6].transform.localPosition = p
				pos_t_.r_time[k] = pos_t_.r_time[k] + Time.deltaTime / Time.timeScale
			else
				--执行完成执行 结束函数
				v[6].transform.localPosition = v[2]
				GameObject.Destroy(v[6].gameObject)
				if pos_t_.funcs[k] ~= nil then
					pos_t_.funcs[k]:execute()
				end
				pos_t_.paths[k] = nil
			end
		end
	end
	--几秒后删除
	for k,v in pairs(pos_t_.delays) do
		if pos_t_.r_time[k] < v.time then
			pos_t_.r_time[k] = pos_t_.r_time[k] + Time.deltaTime / Time.timeScale
		else
			--执行完成执行 结束函数
			if v.fe ~= nil then
				v.fe.execute()
			end
			GameObject.Destroy(v.obj)
			pos_t_.delays[k] = nil
		end
	end
end
