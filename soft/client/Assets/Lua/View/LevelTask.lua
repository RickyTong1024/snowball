LevelTask = {}
LevelTask.select_tab = 0  -- 0=成长历程 1=每日任务
LevelTask.bobjpool = nil

local lua_script_
local level_task_obj_
local daily_task_obj_
local level_task_lb_
local daily_task_lb_
local level_task_redObj_
local level_daily_redObj_


function LevelTask.Awake(obj)
	GUIRoot.ShowGUI('BackPanel', {2})
	GUIRoot.UIEffect(obj, 0)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	LevelTask.bobjpool = bobjpool()
	level_task_obj_ = obj.transform:Find('level_task_page').gameObject
	daily_task_obj_ = obj.transform:Find('daily_task_page').gameObject
	local level_task_btn = obj.transform:Find('left_menu/level_task')
	local daily_task_btn = obj.transform:Find('left_menu/daily_task')
	level_task_lb_  = level_task_btn:Find('label'):GetComponent('UILabel')
	daily_task_lb_  = daily_task_btn:Find('label'):GetComponent('UILabel')	
	level_task_redObj_ = level_task_btn:Find('red').gameObject
	level_daily_redObj_ = daily_task_btn:Find('red').gameObject
	
	lua_script_:AddButtonEvent(level_task_btn.gameObject, "click", LevelTask.OnClick)
	lua_script_:AddButtonEvent(daily_task_btn.gameObject, "click", LevelTask.OnClick)
	
	LevelTask.current_level = 1
	
	LevelTask.RegisterMessage()
	LevelTask.InitLevelTask(obj)
	LevelTask.InitDailyTask(obj)
	LevelTask.Show()
end

function LevelTask.RegisterMessage()
	Message.register_handle("back_panel_msg", LevelTask.Back)
	Message.register_handle("back_panel_recharge", LevelTask.Recharge)
	Message.register_handle("team_join_msg", LevelTask.TeamJoin)
	Message.register_net_handle(opcodes.SMSG_LEVEL_REWARD, LevelTask.SMSG_LEVEL_REWARD)
	Message.register_net_handle(opcodes.SMSG_TASK, LevelTask.SMSG_TASK)
	Message.register_net_handle(opcodes.SMSG_DAILY, LevelTask.SMSG_DAILY)
	Message.register_net_handle(opcodes.SMSG_DAILY_REWARD, LevelTask.SMSG_DAILY_REWARD)
	
end

function LevelTask.RemoveMessage()
	Message.remove_handle("back_panel_msg", LevelTask.Back)
	Message.remove_handle("back_panel_recharge", LevelTask.Recharge)
	Message.remove_handle("team_join_msg", LevelTask.TeamJoin)
	Message.remove_net_handle(opcodes.SMSG_LEVEL_REWARD, LevelTask.SMSG_LEVEL_REWARD)
	Message.remove_net_handle(opcodes.SMSG_TASK, LevelTask.SMSG_TASK)
	Message.remove_net_handle(opcodes.SMSG_DAILY, LevelTask.SMSG_DAILY)
	Message.remove_net_handle(opcodes.SMSG_DAILY_REWARD, LevelTask.SMSG_DAILY_REWARD)
end

-----------------------------------等级奖励--------------------------------------
local level_item_top_res_ = nil
local level_item_bottom_res_ = nil 
local level_panel_ = nil
local level_task_pools_ = nil
local level_desc_res = nil
local exp_reward_res_= nil
local add_scroll_ = nil
local level_task_adds = nil
local level_task_title_ = nil
local level_reward_get_btn_ = nil
local lyObj_


local level_right_top_lb_
local exp_level_tasks_ = nil
local level_task_rewards_
local frame_color_ = {'djframe-blue', 'djframe-purple', 'djframe-y'}
local frame_color_two_ = {'djframe-blue_sp', 'djframe-purple_sp', 'djframe-y_sp'}
local frame_true_color = {Color(61/255,239/255,1),Color(1,65/255,218/255),Color(1,205/255,0)}
local level_exp_task_obj_
local level_exp_notask_obj_
local can_get_reward

local level_task_load_Pos = 0
LevelTask.current_level = 1

function LevelTask.InitLevelTask(obj)
	level_item_top_res_ = obj.transform:Find('level_task_page/level_item_top_res').gameObject
	level_item_bottom_res_ = obj.transform:Find('level_task_page/level_item_bottom_res').gameObject
	level_panel_ = obj.transform:Find('level_task_page/bottomArea')
	level_desc_res = obj.transform:Find('level_task_page/leftArea/desc_res').gameObject
	add_scroll_ = obj.transform:Find('level_task_page/leftArea/add_scrollview')
	level_task_title_ = obj.transform:Find('level_task_page/top/level_title'):GetComponent('UILabel')
	level_reward_get_btn_ = obj.transform:Find('level_task_page/leftArea/level_reward/level_btn'):GetComponent('UISprite')
	lua_script_:AddButtonEvent(level_reward_get_btn_.gameObject, "click", LevelTask.OnClick)
	lyObj_ = obj.transform:Find('level_task_page/leftArea/level_reward/lyLabel').gameObject
	
	level_right_top_lb_ = obj.transform:Find('level_task_page/rightArea/right_top_icon/label'):GetComponent('UILabel')
	exp_reward_res_ = obj.transform:Find('level_task_page/rightArea/exp_reward_res').gameObject
	can_get_reward = obj.transform:Find('level_task_page/rightArea/go_btn').gameObject
	lua_script_:AddButtonEvent(can_get_reward, "click", LevelTask.OnClick)
	
	level_exp_task_obj_ = obj.transform:Find('level_task_page/rightArea/exp_task').gameObject
	level_exp_notask_obj_ = obj.transform:Find('level_task_page/rightArea/no_exp_task').gameObject

	local left_1 = obj.transform:Find('level_task_page/rightArea/exp_task/exp_task_1/task/get_btn')
	local left_2 = obj.transform:Find('level_task_page/rightArea/exp_task/exp_task_2/task/get_btn')
	local left_3 = obj.transform:Find('level_task_page/rightArea/exp_task/exp_task_3/task/get_btn')
	lua_script_:AddButtonEvent(left_1.gameObject, "click", LevelTask.OnClick)
	lua_script_:AddButtonEvent(left_2.gameObject, "click", LevelTask.OnClick)
	lua_script_:AddButtonEvent(left_3.gameObject, "click", LevelTask.OnClick)
	
	level_task_rewards_ = {}
	for i = 1,3 do
		local ts = obj.transform:Find('level_task_page/leftArea/level_reward/reward_'..i)
		table.insert(level_task_rewards_,ts)
	end
	
	exp_level_tasks_ = {}
	for i = 1,3 do
		local et = obj.transform:Find('level_task_page/rightArea/exp_task/exp_task_'..i)
		table.insert(exp_level_tasks_,et)
	end
end

function LevelTask.OpenLevelTaskPage() --打开 成长历程 页面
	if daily_task_obj_.activeSelf then
		daily_task_obj_:SetActive(false)
	end
		
	if not level_task_obj_.activeSelf then
		level_task_obj_:SetActive(true)
	end

	--构造所有等级 Config.max_level
	level_task_load_Pos = level_task_load_Pos + 20
	if level_task_load_Pos > Config.max_level then
		level_task_load_Pos = Config.max_level
	end
	
	for i = 1,level_task_load_Pos do
		LevelTask.AppendLevelItem(i)
	end
	LevelTask.RefreshLevelAddDesc()
end

function LevelTask.LoadLevelItem()
	local is_end = NavUtil.IsScrollviewEnd(level_panel_.gameObject)
	if not is_end then
		return
	end
	if level_task_load_Pos < Config.max_level then
		local st = level_task_load_Pos + 1
		local ed = level_task_load_Pos + 20
		if ed > Config.max_level then
			ed = Config.max_level
		end
		
		local update_objt = level_panel_:Find(tostring(level_task_load_Pos))
		if update_objt ~= nil then
			local nextlineObj = update_objt:Find('next_lines').gameObject	
			if not nextlineObj.activeSelf then
				nextlineObj:SetActive(true)
			end
		end
		
		level_task_load_Pos = ed
		for i = st,ed do
			LevelTask.AppendLevelItem(i)
		end
	end
end

function LevelTask.AppendLevelItem(level_id)
	local bobj = nil
	if level_id % 2 == 1 then
		bobj= LevelTask.GetObjByName(level_item_top_res_)
	else
		bobj = LevelTask.GetObjByName(level_item_bottom_res_)
	end
	bobj.obj:SetActive(true)
	bobj.obj.name = level_id
	bobj.objt.parent = level_panel_
	local x = -275 + (level_id - 1) * 140
	LevelTask.bobjpool:set_localScale(bobj.objid,1,1,1)
	LevelTask.bobjpool:set_localPosition(bobj.objid,x,0,0)
	local circle = bobj.objt:Find('circle')
	local icon = bobj.objt:Find('icon')
	circle:GetChild(0).name = level_id
	lua_script_:AddButtonEvent(circle.gameObject, "click", LevelTask.OnClick)
	bobj.objt:Find('level'):GetComponent('UILabel').text = level_id
	
	lua_script_:AddButtonEvent(bobj.obj,'drag',LevelTask.LoadLevelItem)
	lua_script_:AddButtonEvent(circle.gameObject,'drag',LevelTask.LoadLevelItem)
	
	local nextlineObj = bobj.objt:Find('next_lines').gameObject
	if level_task_load_Pos == level_id then
		if nextlineObj.activeSelf then
			nextlineObj:SetActive(false)
		end
	else
		if not nextlineObj.activeSelf then
			nextlineObj:SetActive(true)
		end
	end
	LevelTask.UpdateSelectLevelIcon(level_id,bobj.objt)
end

function LevelTask.OnParam(param)
	if param ~= nil then
		local need_level = tonumber(param[1])
		if need_level >= level_task_load_Pos then
			local old_num = nil
			if need_level == level_task_load_Pos then
				old_num = level_task_load_Pos + 1
			else
				old_num = level_task_load_Pos
			end
			level_task_load_Pos = Config.max_level
			for j = old_num,Config.max_level do
				LevelTask.AppendLevelItem(j)
			end
		end
		local now_pos = -275 + (need_level - 1) * 140
		local sy_len = (level_task_load_Pos- need_level) * 140
		
		if sy_len < 140 * 5 then
			NavUtil.AdjustScrollview(level_panel_.gameObject,-275,225,now_pos,1)
		else
			NavUtil.AdjustScrollview(level_panel_.gameObject,-275,225,now_pos,0)
		end

		local old = LevelTask.current_level
		LevelTask.current_level = need_level
		LevelTask.UpdateSelectLevelIcon(old)
		LevelTask.UpdateSelectLevelIcon(LevelTask.current_level)
		LevelTask.RefreshLevelAddDesc()
	end
end

function LevelTask.RefreshLevelAddDesc()
	if level_task_adds ~= nil then
		for i=1,#level_task_adds do
			level_task_adds[i].obj:SetActive(false)
			level_task_adds[i].state = false
		end
	end
	
	level_task_adds = {}
	
	local t_exp = Config.get_t_exp(LevelTask.current_level)
	for i = 1,#t_exp.level_add do
		local t_attr = Config.get_t_acc_add(t_exp.level_add[i].type,t_exp.level_add[i].param1)
		if(t_attr ~= nil) then
			local att_desc = string.gsub(t_attr.desc, "{N1}", t_exp.level_add[i].param3)
			local bobj = LevelTask.GetObjByName(level_desc_res)
			bobj.obj:SetActive(true)
			bobj.objt.parent = add_scroll_ 
			local y = 40 - (i - 1) * 25
			bobj.objt:Find('label'):GetComponent('UILabel').text = att_desc
			LevelTask.bobjpool:set_localScale(bobj.objid,1,1,1)
			LevelTask.bobjpool:set_localPosition(bobj.objid,0,y,0)
			table.insert(level_task_adds,bobj)
		end
	end
	
	level_task_title_.text = Config.get_t_script_str('LevelTask_001')..LevelTask.current_level
	level_right_top_lb_.text = Config.get_t_script_str('LevelTask_001')..LevelTask.current_level..Config.get_t_script_str('LevelTask_002')
	
	for i = 1,3 do
		if #t_exp.rewards >= i then
			local t_reward = Config.get_t_reward(t_exp.rewards[i].type,t_exp.rewards[i].value1)
			local frame = 1
			if t_reward.color ~= nil then
				frame = t_reward.color
			end
			local frameSp = level_task_rewards_[i]:GetComponent('UISprite')
			
			if(frame > 10) then
				frameSp.spriteName = frame_color_two_[frame % 10]
			else
				frameSp.spriteName = frame_color_[frame]
			end
			level_task_rewards_[i].gameObject:SetActive(true)
			
			local icon = level_task_rewards_[i]:Find('level_reward_icon')
			icon:GetComponent('UISprite').spriteName =t_reward.icon
			icon:GetChild(0).name = t_exp.rewards[i].type.."+"..t_exp.rewards[i].value1
			lua_script_:AddButtonEvent(icon.gameObject, "click", LevelTask.OnClick)
			
			local num_lb = level_task_rewards_[i]:Find('num'):GetComponent('UILabel')
			if t_exp.rewards[i].value2 > 1 then
				num_lb.text = t_exp.rewards[i].value2
				num_lb.gameObject:SetActive(true)
			else
				num_lb.gameObject:SetActive(false)
			end
		else
			level_task_rewards_[i].gameObject:SetActive(false)
		end
	end

	if #t_exp.rewards > 0 then
		--按钮 的变化
		local sign = false
		for i = 1,#self.player.level_reward do
			if self.player.level_reward[i] == LevelTask.current_level then
				sign = true
				break
			end
		end
		
		if not sign then
			if LevelTask.current_level > self.player.level then
				level_reward_get_btn_.gameObject:SetActive(false)
				lyObj_:SetActive(false)
			else
				level_reward_get_btn_.gameObject:SetActive(true)
				lyObj_:SetActive(false)
			end
		else
			level_reward_get_btn_.gameObject:SetActive(false)
			lyObj_:SetActive(true)
		end
	else
		level_reward_get_btn_.gameObject:SetActive(false)
		lyObj_:SetActive(false)
	end
	
	
	--刷新每个等级的 任务 达到任务等级开放
	if self.player.level >= LevelTask.current_level then
		level_exp_task_obj_:SetActive(true)
		level_exp_notask_obj_:SetActive(false)
		for i = 1,3 do
			if i <= #t_exp.tasks then
				local task_Objt = exp_level_tasks_[i]:Find('task')
				task_Objt.gameObject:SetActive(true)
				exp_level_tasks_[i]:Find('no_task').gameObject:SetActive(false)
				local t_task = Config.get_t_task(t_exp.tasks[i])
				task_Objt:Find('desc_label'):GetComponent('UILabel').text = t_task.desc
				
				local reward_p = task_Objt:Find('task_rewards')
				for i = 1,reward_p.childCount do
					reward_p:GetChild(i - 1).gameObject:SetActive(false)
				end
				
				for i = 1,#t_task.rewards do
					local bobj = LevelTask.GetObjByName(exp_reward_res_)
					bobj.objt.parent = reward_p
					LevelTask.bobjpool:set_localPosition(bobj.objid,-145 + (i - 1) * 110,-14,0)
					LevelTask.bobjpool:set_localScale(bobj.objid,1,1,1)
					bobj.obj:SetActive(true)
					
					local frameSp = bobj.objt:GetComponent('UISprite')
					local t_reward = Config.get_t_reward(t_task.rewards[i].type,t_task.rewards[i].value1)
					local frame = 1
					if t_reward.color ~= nil then
						frame = t_reward.color
					end
					
					if(frame > 10) then
						frameSp.spriteName = frame_color_two_[frame % 10]
					else
						frameSp.spriteName = frame_color_[frame]
					end
					
					local icon = bobj.objt:Find('level_reward_icon'):GetComponent('UISprite')
					icon.spriteName =t_reward.icon
					icon.transform:GetChild(0).name = t_task.rewards[i].type.."+"..t_task.rewards[i].value1
					lua_script_:AddButtonEvent(icon.gameObject, "click", LevelTask.OnClick)
					local num_lb = bobj.objt:Find('num_label'):GetComponent('UILabel')
					if t_task.rewards[i].value2 > 0 then
						num_lb.text = 'x'..t_task.rewards[i].value2
						num_lb.color = frame_true_color[frame % 10]
						num_lb.gameObject:SetActive(true)
					else
						num_lb.gameObject:SetActive(false)
					end
				end
				
				local sign = false
				for i = 1,#self.player.task_reward do
					if self.player.task_reward[i] == t_task.id then
						sign = true
						break
					end
				end
				
				local num = self.get_task_num(t_task.id)
				if sign then
					task_Objt:Find('score_label'):GetComponent('UILabel').text = t_task.target_num..'/'..t_task.target_num
				else
					task_Objt:Find('score_label'):GetComponent('UILabel').text = num..'/'..t_task.target_num
				end
				
				local cp = false
				for i = 1,#self.player.task_reward do
					if self.player.task_reward[i] == t_task.id then
						cp = true
						break
					end
				end
				if cp then
					task_Objt:Find('get_btn').gameObject:SetActive(false)
					task_Objt:Find('lyLabel').gameObject:SetActive(true)
				elseif not cp and num < t_task.target_num then
					task_Objt:Find('get_btn'):GetComponent('UISprite').spriteName = 'b1_gray-78'
					task_Objt:Find('get_btn'):GetComponent('BoxCollider').enabled = false
					task_Objt:Find('get_btn').gameObject:SetActive(true)
					task_Objt:Find('lyLabel').gameObject:SetActive(false)
				else
					task_Objt:Find('get_btn'):GetComponent('UISprite').spriteName = 'b1-78'
					task_Objt:Find('get_btn'):GetComponent('BoxCollider').enabled = true
					task_Objt:Find('get_btn').gameObject:SetActive(true)
					task_Objt:Find('lyLabel').gameObject:SetActive(false)
				end
				
				--task_id
				task_Objt:Find('task_id'):GetComponent('UILabel').text = t_task.id
			else
				exp_level_tasks_[i]:Find('task').gameObject:SetActive(false)
				exp_level_tasks_[i]:Find('no_task').gameObject:SetActive(true)
			end
		end
	else
		level_exp_task_obj_:SetActive(false)
		level_exp_notask_obj_:SetActive(true)
	end
	
	local red_num = LevelTask.GetCompleteTaskNum()
	if red_num > 0 then
		level_task_redObj_:SetActive(true)
		level_task_redObj_.transform:Find('num'):GetComponent('UILabel').text = red_num
	else
		level_task_redObj_:SetActive(false)
	end
	
	--是否显示 前往领奖
	can_get_reward.gameObject:SetActive(false)
	for i = 1,Config.max_level do
		local sign = LevelTask.IsComplete(i)
		if sign then
			if not can_get_reward.gameObject.activeSelf then
				can_get_reward.gameObject:SetActive(true)
				break
			end
		end
	end
end

function LevelTask.UpdateSelectLevelIcon(level_id,objt)
	if objt == nil then
		objt = level_panel_:Find(tostring(level_id))
	end
	
	local circle = objt:Find('circle')
	local icon = objt:Find('icon')
	local t_exp = Config.get_t_exp(level_id)
	local animator = circle:GetComponent("Animator")
	local level_lb = objt:Find('level'):GetComponent('UILabel')
	
	local redObj = objt:Find('red').gameObject
	local re = LevelTask.IsComplete(level_id)
	if re then
		redObj:SetActive(true)
	else
		redObj:SetActive(false)
	end

	if level_id <= self.player.level then
		if level_id == LevelTask.current_level then
			icon:GetComponent('UISprite').spriteName = 'jycz_new'
			circle:GetComponent('UISprite').spriteName = 'zcz_002'
			animator.enabled = true
			animator:Rebind()
			--金色数字
			level_lb.effectColor = Color(171/255,51/255,0)
			level_lb.gradientTop = Color(249/255,231/255,145/255)
			level_lb.gradientBottom = Color.white
		else
			icon:GetComponent('UISprite').spriteName = 'jycz_new2'
			circle:GetComponent('UISprite').spriteName = 'zcz_003'
			animator.enabled = false
			level_lb.effectColor = Color(21/255,118/255,137/255)
			level_lb.gradientTop = Color(173/255,1,251/255)
			level_lb.gradientBottom = Color.white
		end
	else
		icon:GetComponent('UISprite').spriteName = 'jycz_new3'
		
		if level_id == LevelTask.current_level then
			circle:GetComponent('UISprite').spriteName = 'zcz_002'
			animator.enabled = true
			animator:Rebind()
			level_lb.effectColor = Color(101/255,101/255,101/255)
			level_lb.gradientTop = Color(218/255,218/255,218/255)
			level_lb.gradientBottom = Color.white
		else
			circle:GetComponent('UISprite').spriteName = 'zcz_004'
			animator.enabled = false
			level_lb.effectColor = Color(101/255,101/255,101/255)
			level_lb.gradientTop = Color(218/255,218/255,218/255)
			level_lb.gradientBottom = Color.white
		end
	end
end

--成长任务可领取奖励的总数
function LevelTask.GetCompleteTaskNum()
	local count = 0
	for i = 1,Config.max_level do
		if LevelTask.IsComplete(i) then
			count = count + 1
		end
	end
	return count
end

--判断该等级的任务 和 额外任务有没有完成
function LevelTask.IsComplete(level)
	if self.player.level < level then
		return false
	end
	
	local sign = false
	for i = 1,#self.player.level_reward do
		if self.player.level_reward[i] == level then
			sign = true
			break
		end
	end
	
	local t_exp = Config.get_t_exp(level)
	
	if not sign and #t_exp.rewards > 0 then
		return true
	end
	
	
	for i = 1,#t_exp.tasks do
		local sign = false
		for j = 1,#self.player.task_reward do
			if self.player.task_reward[j] == t_exp.tasks[i] then
				sign = true
				break
			end
		end
		
		if not sign then
			local t_task = Config.get_t_task(t_exp.tasks[i])
			local num = self.get_task_num(t_exp.tasks[i]) 
			if num >= t_task.target_num then
				return true
			end
		end
	end
	
	return false
end

function LevelTask.GetObjByName(ori_obj)
	local name = ori_obj.name

	if level_task_pools_ == nil or level_task_pools_[name] == nil then
		--构建这个 对象
		local obj = LuaHelper.Instantiate(ori_obj)
		local objid = LevelTask.bobjpool:add(obj)
		local objt = obj.transform
		LevelTask.bobjpool:set_localScale(objid,1,1,1)	
		
		if level_task_pools_ == nil then
			level_task_pools_ = {}
		end
		
		if level_task_pools_[name] == nil then
			level_task_pools_[name] = {}
		end
		local dt = {obj = obj,objt = objt,objid = objid,state = true}
		table.insert(level_task_pools_[name],dt)
		return dt
	elseif level_task_pools_[name]~= nil then
		for i = 1,#level_task_pools_[name] do
			if level_task_pools_[name][i].state == false then
				level_task_pools_[name][i].state = true
				return level_task_pools_[name][i]
			end
		end
		
		local obj = LuaHelper.Instantiate(ori_obj)
		local objid = LevelTask.bobjpool:add(obj)
		local objt = obj.transform
		LevelTask.bobjpool:set_localScale(objid,1,1,1)
		
		local dt = {obj = obj,objt = objt,objid = objid,state = true}
		table.insert(level_task_pools_[name],dt)
		return dt
	end
end

-----------------------------------每日任务--------------------------------------
local daily_task_panel_
local daily_task_res_
local daily_active_degree_lb_
local daily_probar_
local daily_reward_res_
local daily_activedegree_panel_ 	--不同活跃度奖励

local sign_res_
local active_degree_sign_panel_

local daily_degrees = {}	--每日活跃度list
local daily_tasks = {}		--每日任务list
local daily_task_pools_ = {}

function LevelTask.InitDailyTask(obj)
	daily_task_panel_ = obj.transform:Find('daily_task_page/daily_task_panel')
	daily_task_res_ = obj.transform:Find('daily_task_page/daily_task_res').gameObject
	daily_active_degree_lb_ = obj.transform:Find('daily_task_page/top/active_degree_pic/active_degree_value'):GetComponent('UILabel')
	daily_probar_ = obj.transform:Find('daily_task_page/top/active_degree_bar'):GetComponent('UIProgressBar')
	daily_reward_res_ = obj.transform:Find('daily_task_page/top/reward_res').gameObject
	daily_activedegree_panel_ = obj.transform:Find('daily_task_page/top/active_degree_reward')
	active_degree_sign_panel_ = obj.transform:Find('daily_task_page/top/active_degree_sign')
	sign_res_ = obj.transform:Find('daily_task_page/top/sign_res').gameObject
end

--获取 每日任务完成的数目
function LevelTask.GetDailyTaskCompleteNumber()
	local count = 0
	
	for k,v in pairs(Config.t_daily) do
		local sign = false
		for j = 1,#self.player.daily_reward do
			if self.player.daily_reward[j] == k then
				sign = true
				break
			end
		end
		
		if not sign then
			if self.get_daily_num(k) >= v.target_num then
				count = count + 1
			end
		end
	end
	
	for k,v in pairs(Config.t_daily_reward) do
		local sign = false
		for j = 1,#self.player.daily_get_id do
			if self.player.daily_get_id[j] == k then
				sign = true
				break
			end
		end
		
		if not sign then
			if self.player.daily_point >= v.point then
				count = count + 1
			end
		end
	end
	
	return count
end

function LevelTask.OpenDailyTaskPage()	--打开 每日任务 页面
	if	level_task_obj_.activeSelf then
		level_task_obj_:SetActive(false)
	end
	
	if	not daily_task_obj_.activeSelf then
		daily_task_obj_:SetActive(true)
	end	
	daily_task_pools_ = {}
	
	daily_degrees = {}
	for k,v in pairs(Config.t_daily_reward) do
		table.insert(daily_degrees,v)
	end
	
	daily_tasks = {}
	for k,v in pairs(Config.t_daily) do
		table.insert(daily_tasks,v)
	end
	
	local comp = function(a,b)
		return a.id < b.id
	end
	
	local comp2 = function(a,b)
		local a1 = 1
		local b1 = 1
		
		if self.get_daily_num(a.id) >= a.target_num then
			a1 = 2
		end
		
		if self.get_daily_num(b.id) >= b.target_num then
			b1 = 2
		end
		
		for i = 1,#self.player.daily_reward do
			if self.player.daily_reward[i] == a.id then
				a1 = 0
			elseif self.player.daily_reward[i] == b.id then
				b1 = 0
			end
		end
		
		return a1 > b1
	end
	
	table.sort(daily_degrees,comp)
	table.sort(daily_tasks,comp2)
	
	
	--生成 相应数目的 活跃度奖励
	for i = 1,#daily_degrees do
		local bobj = LevelTask.GetObjByName(daily_reward_res_)
		table.insert(daily_task_pools_,bobj)
		bobj.objt.parent = daily_activedegree_panel_
		bobj.objt.name = daily_degrees[i].id
		
		local t_reward = Config.get_t_reward(daily_degrees[i].type,daily_degrees[i].value1)
		local frame = 1
		if t_reward.color ~= nil then
			frame = t_reward.color
		end
		
		local frameSp = bobj.objt:GetComponent('UISprite')
		if(frame > 10) then
			frameSp.spriteName = frame_color_two_[frame % 10]
		else
			frameSp.spriteName = frame_color_[frame]
		end
		bobj.objt.gameObject:SetActive(true)
		
		local icon = bobj.objt:Find('degree_reward')
		icon:GetComponent('UISprite').spriteName =t_reward.icon
		icon:GetChild(0).name = daily_degrees[i].type.."+"..daily_degrees[i].value1
		local num_lb = bobj.objt:Find('num'):GetComponent('UILabel')
		if daily_degrees[i].value2 > 1 then
			num_lb.text = daily_degrees[i].value2
			num_lb.gameObject:SetActive(true)
		else
			num_lb.gameObject:SetActive(false)
		end
		lua_script_:AddButtonEvent(icon.gameObject, "click", LevelTask.OnClick)
		local x = 550 * daily_degrees[i].point / 110
		LevelTask.bobjpool:set_localPosition(bobj.objid,x,0,0)
		LevelTask.bobjpool:set_localScale(bobj.objid,1,1,1)
		bobj.obj:SetActive(true)
	end
	
	--生成对应数目的 活跃度 
	for i = 1,#daily_degrees do
		local bobj = LevelTask.GetObjByName(sign_res_)
		table.insert(daily_task_pools_,bobj)
		bobj.objt.parent = active_degree_sign_panel_
		
		bobj.objt:Find('label'):GetComponent('UILabel').text = daily_degrees[i].point
		
		local x = 550 * daily_degrees[i].point / 110
		LevelTask.bobjpool:set_localPosition(bobj.objid,x,0,0)
		LevelTask.bobjpool:set_localScale(bobj.objid,1,1,1)
		bobj.obj:SetActive(true)
	end
	
	--创建每日任务
	daily_task_panel_.transform.localPosition = Vector3(89,-100.4,0)
	daily_task_panel_:GetComponent('UIPanel').clipOffset = Vector2(0, 6)
	for i = 1,#daily_tasks do
		local bobj = LevelTask.GetObjByName(daily_task_res_)
		table.insert(daily_task_pools_,bobj)
		bobj.objt.parent = daily_task_panel_
		bobj.objt.name = daily_tasks[i].id  --名字
		local y = 120 - (i -1) * 109 
		local btn = bobj.objt:Find('daily_get_btn')
		lua_script_:AddButtonEvent(btn.gameObject, "click", LevelTask.OnClick)
		
		LevelTask.bobjpool:set_localPosition(bobj.objid,0,y,0)
		LevelTask.bobjpool:set_localScale(bobj.objid,1,1,1)
		bobj.obj:SetActive(true)
	end
	
	LevelTask.RefreshDailyTask()
end

function LevelTask.RefreshDailyTask()
	daily_active_degree_lb_.text = self.player.daily_point
	daily_probar_.value = self.player.daily_point / 110
	
	--刷新当前 奖励 的状态
	for i = 1,daily_activedegree_panel_.childCount do
		local objt = daily_activedegree_panel_:GetChild(i - 1)
		local degree_id = tonumber(objt.name)
		local t_daily_reward = Config.get_t_daily_reward(degree_id)
		local sign = false
		for j = 1,#self.player.daily_get_id do
			if self.player.daily_get_id[j] == degree_id then
				sign = true
				break
			end
		end
		
		local maskObj = objt:Find('mask').gameObject
		local effectObj = objt:Find('effect').gameObject
		if sign then
			if not maskObj.activeSelf then
				maskObj:SetActive(true)
			end
			if effectObj.activeSelf then
				effectObj:SetActive(false)
			end
		else
			if self.player.daily_point >= t_daily_reward.point then
				if not effectObj.activeSelf then
					effectObj:SetActive(true)
				end
				if maskObj.activeSelf then
					maskObj:SetActive(false)
				end
			else
				if effectObj.activeSelf then
					effectObj:SetActive(false)
				end
				if maskObj.activeSelf then
					maskObj:SetActive(false)
				end
			end
		end
	end
	
	
	
	--刷新当前任务的状态
	for i = 1,daily_task_panel_.childCount do
		local objt = daily_task_panel_:GetChild(i - 1)
		local t_daily = Config.get_t_daily(tonumber(objt.name))

		objt:Find('title'):GetComponent('UILabel').text = t_daily.name
		objt:Find('desc'):GetComponent('UILabel').text = t_daily.desc
		local rewardt = objt:Find('rewards')
		
		for i = 1,rewardt.childCount do
			GameObject.Destroy(rewardt:GetChild(i - 1).gameObject)
		end
		
		for i =1,#t_daily.rewards do
			local t_reward = Config.get_t_reward(t_daily.rewards[i].type,t_daily.rewards[i].value1)
			local reward_obj = nil	
			if t_reward ~= nil then
				if t_reward.color == nil then
					reward_obj = IconPanel.GetIcon("reward_res", nil,t_reward.icon, 1, t_daily.rewards[i].value2)
				else
					reward_obj = IconPanel.GetIcon("reward_res", nil,t_reward.icon,t_reward.color, t_daily.rewards[i].value2)
				end
			end
			reward_obj.transform:Find("icon").name = t_daily.rewards[i].type.."+"..t_daily.rewards[i].value1
			reward_obj.transform.parent = rewardt
			reward_obj.transform.localScale = Vector3.one
			local x = (i - 1) * -90
			reward_obj.transform.localPosition = Vector3(x,0,0)
			reward_obj:SetActive(true)
		end
				
		--判断任务是否完成
		local sign = false
		for i = 1,#self.player.daily_reward do
			if self.player.daily_reward[i] == t_daily.id then
				sign = true
			end
		end
		
		local lqObj = objt:Find('yl_label').gameObject
		local getObj = objt:Find('daily_get_btn').gameObject
		local time_lb  = objt:Find('times_label')
		if sign then
			lqObj:SetActive(true)
			getObj:SetActive(false)
			time_lb.gameObject:SetActive(false)
		else
			local num = self.get_daily_num(t_daily.id)
			local sp = getObj:GetComponent('UISprite')
			if t_daily.target_num > num then
				sp.spriteName = 'b1_gray-78'
				sp:GetComponent('BoxCollider').enabled = false
			else
				sp.spriteName = 'b1-78'
				sp:GetComponent('BoxCollider').enabled = true
			end
			getObj.transform:Find('task_id'):GetComponent('UILabel').text = t_daily.id
			time_lb:GetComponent('UILabel').text = num..'/'..t_daily.target_num
			time_lb.gameObject:SetActive(true)
			lqObj:SetActive(false)
			getObj:SetActive(true)
		end
	end
	
	local red_num = LevelTask.GetDailyTaskCompleteNumber()
	if red_num > 0 then
		level_daily_redObj_:SetActive(true)
		level_daily_redObj_.transform:Find('num'):GetComponent('UILabel').text = red_num
	else
		level_daily_redObj_:SetActive(false)
	end
end

----------------------------------------------------------------------------------
function LevelTask.Back()
	GUIRoot.HideGUI("LevelTask")
	GUIRoot.HideGUI("BackPanel")
	LevelTask.Fini()
	GUIRoot.ShowGUI("HallPanel")
end

function LevelTask.Recharge()
	GUIRoot.HideGUI('LevelTask')
	LevelTask.Fini()
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function LevelTask.TeamJoin()
	GUIRoot.HideGUI('LevelTask')
	LevelTask.Fini()
end

function LevelTask.OnClick(obj)
	if obj.name == 'level_task' then
		if LevelTask.select_tab == 0 then
			return
		end
		LevelTask.ClearCurrent()
		LevelTask.select_tab = 0
		LevelTask.Show()
	elseif obj.name == 'daily_task' then
		if LevelTask.select_tab == 1 then
			return
		end
		LevelTask.ClearCurrent()
		LevelTask.select_tab = 1
		LevelTask.Show()
	elseif obj.name == 'circle' then
		local level = obj.transform:GetChild(0).name
		local now_level = tonumber(level)
		local old_level = LevelTask.current_level
		if now_level == old_level then
			return
		else
			LevelTask.current_level = now_level
			LevelTask.UpdateSelectLevelIcon(old_level)
			LevelTask.UpdateSelectLevelIcon(now_level)
			LevelTask.RefreshLevelAddDesc()
		end	
	elseif obj.name == 'level_reward_icon' then
		local item_id = obj.transform:GetChild(0).name
		GUIRoot.ShowGUI("DetailPanel", {item_id})
	elseif obj.name == 'go_btn' then
		for i = 1,Config.max_level do
			local sign = LevelTask.IsComplete(i)
			
			if sign then
				--判断当前页面有没有这个长度
				if i > level_task_load_Pos then
					local old_num = level_task_load_Pos
					level_task_load_Pos = Config.max_level
					for j = old_num,Config.max_level do
						LevelTask.AppendLevelItem(j)
					end
				end
				
				local now_pos = -275 + (i - 1) * 140
				local sy_len = (level_task_load_Pos- i) * 140
				
				if sy_len < 140 * 5 then
					NavUtil.AdjustScrollview(level_panel_.gameObject,-275,225,now_pos,1)
				else
					NavUtil.AdjustScrollview(level_panel_.gameObject,-275,225,now_pos,0)
				end

				local old = LevelTask.current_level
				LevelTask.current_level = i
				LevelTask.UpdateSelectLevelIcon(old)
				LevelTask.UpdateSelectLevelIcon(LevelTask.current_level)
				LevelTask.RefreshLevelAddDesc()
				return
			end
		end
	elseif obj.name == 'level_btn' then
		if self.player.level >= LevelTask.current_level then
			LevelTask.CMSG_LEVEL_REWARD(LevelTask.current_level)
		end
	elseif obj.name == 'get_btn' then
		local task_id = obj.transform.parent:Find('task_id'):GetComponent('UILabel').text
		if tonumber(task_id) ~= nil then
			LevelTask.CMSG_TASK(tonumber(task_id))
		end
	elseif obj.name == 'daily_get_btn' then
		local daily_task_id = obj.transform:Find('task_id'):GetComponent('UILabel').text
		if tonumber(daily_task_id) ~= nil then
			LevelTask.CMSG_DAILY(tonumber(daily_task_id))
		end
	elseif obj.name == 'degree_reward' then
		local degree_id = obj.transform.parent.name
		degree_id = tonumber(degree_id)
		if degree_id ~= nil then
			local t_daily_reward = Config.get_t_daily_reward(degree_id)
			local get_state = false
			for i = 1,#self.player.daily_get_id do
				if self.player.daily_get_id[i] == degree_id then
					get_state = true
					break
				end
			end
			if not get_state and self.player.daily_point >= t_daily_reward.point then
				LevelTask.CMSG_DAILY_REWARD(degree_id)
			else
				local item_id = obj.transform:GetChild(0).name
				GUIRoot.ShowGUI("DetailPanel", {item_id})
			end
		end
	end
end

function LevelTask.Fini()
	LevelTask.ClearCurrent()
end

function LevelTask.Show()
	if LevelTask.select_tab == 0 then
		level_task_lb_.text = '[E8FCFF]'..Config.get_t_script_str('LevelTask_003')..'[-]'
		daily_task_lb_.text = '[7E90A4]'..Config.get_t_script_str('LevelTask_004')..'[-]'
		LevelTask.OpenLevelTaskPage()
	elseif LevelTask.select_tab == 1 then
		level_task_lb_.text = '[7E90A4]'..Config.get_t_script_str('LevelTask_003')..'[-]'
		daily_task_lb_.text = '[E8FCFF]'..Config.get_t_script_str('LevelTask_004')..'[-]'
		LevelTask.OpenDailyTaskPage()
	end
	
	--加载 红点的变化
	local red_num = LevelTask.GetCompleteTaskNum()
	if red_num > 0 then
		level_task_redObj_:SetActive(true)
		level_task_redObj_.transform:Find('num'):GetComponent('UILabel').text = red_num
	else
		level_task_redObj_:SetActive(false)
	end
	
	red_num = LevelTask.GetDailyTaskCompleteNumber()

	if red_num > 0 then
		level_daily_redObj_:SetActive(true)
		level_daily_redObj_.transform:Find('num'):GetComponent('UILabel').text = red_num
	else
		level_daily_redObj_:SetActive(false)
	end
end

function LevelTask.OnDestroy()
	LevelTask.RemoveMessage()
	if level_task_pools_ ~= nil then
		for k,v in pairs(level_task_pools_) do
			for i = 1,#v do
				GameObject.Destroy(v[i].obj)
				LevelTask.bobjpool:remove(v[i].objid)
			end
		end
	end
	level_task_load_Pos = 0
	level_task_pools_ = nil
	LevelTask.select_tab = 0
end

function LevelTask.ClearCurrent()
	if LevelTask.select_tab == 0 then
		if level_task_pools_ ~= nil then
			for k,v in pairs(level_task_pools_) do
				for i = 1,#v do
					v[i].state = false
					v[i].obj:SetActive(false)
				end
			end
		end
		level_task_adds = {}
	elseif LevelTask.select_tab == 1 then
		for i = 1,#daily_task_pools_ do
			daily_task_pools_[i].obj:SetActive(false)
			daily_task_pools_[i].state = false	
		end
		daily_task_pools_ = {}
	end
end
-----------------------------------服务端通信---------------------------------------------
--领取等级礼包
local level_reward_id = nil
local level_task_id = nil

function LevelTask.CMSG_LEVEL_REWARD(level)
	if level == nil or level < 1 or level > Config.max_level then
		return
	end
	
	local msg = msg_hall_pb.cmsg_level_reward()
	msg.level = level
	level_reward_id = level
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_LEVEL_REWARD, data,{opcodes.SMSG_LEVEL_REWARD})
end

function LevelTask.SMSG_LEVEL_REWARD(message)
	local msg = msg_hall_pb.smsg_level_reward()
	msg:ParseFromString(message.luabuff)
	
	local rewards = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
	for i = 1, #rewards do
		self.add_reward(rewards[i])
	end

	for i = 1,#msg.roles do
		self.add_role(msg.roles[i])
	end
	
	self.player.level_reward:append(level_reward_id)
	
	if(#rewards > 0) then
		GUIRoot.ShowGUI('GainPanel', {rewards})
	end
	
	if(#msg.roles > 0) then
		if #rewards > 0 then
			GUIRoot.ShowGUI("RoleGetPanel", {msg.roles, {"LevelTask", "BackPanel",'GainPanel'}})
		else
			GUIRoot.ShowGUI("RoleGetPanel", {msg.roles, {"LevelTask", "BackPanel"}})
		end
	end
	
	LevelTask.UpdateSelectLevelIcon(level_reward_id)
	LevelTask.RefreshLevelAddDesc()
end

--等级特殊任务 领取奖励
function LevelTask.CMSG_TASK(task_id)
	local msg = msg_hall_pb.cmsg_task()
	msg.id = task_id
	level_task_id = task_id
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_TASK, data,{opcodes.SMSG_TASK})
end

--等级特殊任务 领取奖励回报
function LevelTask.SMSG_TASK()
	self.del_task_num(level_task_id)
	self.player.task_reward:append(level_task_id)
	
	local t_task = Config.get_t_task(level_task_id)
	
	local mail_rewad = {}
	for i = 1,#t_task.rewards do
		self.add_reward(t_task.rewards[i])
		table.insert(mail_rewad,t_task.rewards[i])
	end
	GUIRoot.ShowGUI('GainPanel', {mail_rewad})
	LevelTask.UpdateSelectLevelIcon(LevelTask.current_level)
	LevelTask.RefreshLevelAddDesc()
end
-----------------------------------每日任务服务端-------------------------------------------
local send_daily_task_id = nil
local send_daily_activedegree_id = nil

function LevelTask.CMSG_DAILY(daily_task_id)
	local msg = msg_hall_pb.cmsg_daily()
	msg.id = daily_task_id
	send_daily_task_id = daily_task_id
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_DAILY, data,{opcodes.SMSG_DAILY})
end

function LevelTask.SMSG_DAILY()
	self.del_daily_num(send_daily_task_id)
	self.player.daily_reward:append(send_daily_task_id)
	
	local t_daily = Config.get_t_daily(send_daily_task_id)
	
	local mail_rewad = {}
	for i = 1,#t_daily.rewards do
		self.add_reward(t_daily.rewards[i])
		table.insert(mail_rewad,t_daily.rewards[i])
	end
	GUIRoot.ShowGUI('GainPanel', {mail_rewad})
	LevelTask.RefreshDailyTask()
end

function LevelTask.CMSG_DAILY_REWARD(daily_reward_id)
	daily_reward_id = tonumber(daily_reward_id)
	local msg = msg_hall_pb.cmsg_daily_reward()
	msg.id = daily_reward_id
	send_daily_activedegree_id = daily_reward_id
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_DAILY_REWARD, data,{opcodes.SMSG_DAILY_REWARD})
end

function LevelTask.SMSG_DAILY_REWARD()
	local t_daily_reward = Config.get_t_daily_reward(send_daily_activedegree_id)
	local mail_rewad = {}
	local rd = {type = t_daily_reward.type,value1 = t_daily_reward.value1,value2 = t_daily_reward.value2,value3 = t_daily_reward.value3}
	self.add_reward(rd)
	table.insert(mail_rewad,rd)
	GUIRoot.ShowGUI('GainPanel', {mail_rewad})
	self.player.daily_get_id:append(send_daily_activedegree_id)
	LevelTask.RefreshDailyTask()
end