AchieveAnimation = {}
 
local animation_view_
local lua_script_

local normalQueue = nil
local fightQueue = nil

local normal_res_
local fight_res_

local hall_achieve_list = {}
local hall_achieve_state = false
local send_achieve_ = nil

local hall_delay_status = false


function AchieveAnimation.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	animation_view_ = obj.transform:Find("Bottom").gameObject
	
	--queues 动画队列 rtimes = 已经使用的时间 objs = 对象 now_pos = 现在的位置
	fightQueue = {Length = 0,need_s = {},rtimes = {},objs = {},now_pos = 0}

	normal_res_ = obj.transform:Find("normal_res").gameObject
	fight_res_ = obj.transform:Find('fight_res').gameObject
	hall_delay_status = false
	AchieveAnimation.RegisterMessage()
end

function AchieveAnimation.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_ACHIEVE,AchieveAnimation.SMSG_ACHIEVE)
end

---------------------------------------大厅中使用------------------------------------
function AchieveAnimation.CheckHallAchieve(check_status)
	if check_status ~= nil and hall_delay_status then
		return
	end
	
	if not hall_achieve_state and #hall_achieve_list > 0 then
		hall_achieve_state = true
		local achieve = hall_achieve_list[1]
		table.remove(hall_achieve_list,1)
		send_achieve_ = achieve
		AchieveAnimation.CMSG_ACHIEVE()
	end
end



function AchieveAnimation.SetDelayStatus(status)
	if status then
		hall_delay_status = true
	else
		hall_delay_status = false
		AchieveAnimation.CheckHallAchieve()
	end
end

function AchieveAnimation.AddHallAchieve(achieve_id,check)
	local t_achievement = Config.get_t_achievement(achieve_id)
	if t_achievement == nil then
		return
	end
	
	table.insert(hall_achieve_list,{achieve_id = achieve_id})
		
	if not check and not hall_delay_status then
		if not hall_achieve_state and #hall_achieve_list > 0 and State.state.ss_hall == State.cur_state then
			hall_achieve_state = true
			local achieve = hall_achieve_list[1]
			table.remove(hall_achieve_list,1)
			send_achieve_ = achieve
			AchieveAnimation.CMSG_ACHIEVE()
		end
	end
end

local hall_achieve_data_ = nil

function AchieveAnimation.OpenHallAchieve(hall_achieve_dt)
	local obj = LuaHelper.Instantiate(normal_res_.gameObject)
	obj.transform.parent = animation_view_.transform
	obj.transform.localScale = Vector3.one
	obj.transform.localPosition = Vector3(0,320,0)
	
	local ok_btn = obj.transform:Find('content/ok_btn').gameObject
	lua_script_:AddButtonEvent(ok_btn, "click", AchieveAnimation.OnClick)
	local shareObj = obj.transform:Find('content/can_btn').gameObject
	lua_script_:AddButtonEvent(shareObj, "click", AchieveAnimation.OnClick)
	
	hall_achieve_obj_ = obj
	hall_achieve_data_ = hall_achieve_dt
	
	timerMgr:AddRepeatTimer('achieve_level_up',AchieveAnimation.UpdateAchieveExpScroll,0.17, 0.02)
	obj:SetActive(true)
	obj:GetComponent("Animator"):Rebind()
	lua_script_:PlaySound('achievement')
end

function AchieveAnimation.UpdateAchieveExpScroll()
	if hall_achieve_data_.finish then
		return
	end
	if not hall_achieve_data_.init then
		hall_achieve_data_.sc = hall_achieve_obj_.transform:Find('content/slider'):GetComponent('UISlider')
		hall_achieve_data_.l_level_ = hall_achieve_obj_.transform:Find('content/lb/achieve_level'):GetComponent('UILabel')
		hall_achieve_data_.r_level_ = hall_achieve_obj_.transform:Find('content/next_level_lb'):GetComponent('UILabel')
		hall_achieve_data_.arrow = hall_achieve_obj_.transform:Find('content/arrow')

		local final_point = hall_achieve_data_.now_point + Config.get_t_achievement(hall_achieve_data_.achieve_id).point	
		local t_level_reward = Config.get_t_achievement_reward(hall_achieve_data_.now_level)	

		while true do
			if t_level_reward.total_point < final_point then
				t_level_reward = Config.get_t_achievement_reward(t_level_reward.id + 1)
			elseif t_level_reward.total_point == final_point then
				hall_achieve_data_.final_level = t_level_reward.id
				break
			else
				hall_achieve_data_.final_level = t_level_reward.id - 1
				break
			end
		end	
		hall_achieve_data_.current_point = hall_achieve_data_.now_point
		
		if hall_achieve_data_.final_level > hall_achieve_data_.now_level then
			hall_achieve_data_.arrow.gameObject:SetActive(true)
			hall_achieve_data_.r_level_.gameObject:SetActive(true)
			hall_achieve_data_.l_level_.text = hall_achieve_data_.now_level
			hall_achieve_data_.r_level_.text = hall_achieve_data_.now_level + 1
		else
			hall_achieve_data_.arrow.gameObject:SetActive(false)
			hall_achieve_data_.r_level_.gameObject:SetActive(false)
			hall_achieve_data_.l_level_.text = hall_achieve_data_.now_level
		end
		--加载 该成就的内容
		local t_achievement = Config.get_t_achievement(hall_achieve_data_.achieve_id)
		hall_achieve_obj_.transform:Find('content/tiaodai/label'):GetComponent('UILabel').text = t_achievement.name
		hall_achieve_obj_.transform:Find('content/desc'):GetComponent('UILabel').text = t_achievement.desc
		hall_achieve_data_.init = true
	end
	
	local sc = hall_achieve_data_.sc
	local l_level_ = hall_achieve_data_.l_level_
	local r_level_ = hall_achieve_data_.r_level_
	local arrow = hall_achieve_data_.arrow 
	local final_level = hall_achieve_data_.final_level
	local current_point = hall_achieve_data_.current_point
	
	local next_level = hall_achieve_data_.now_level + 1
	local t_next_level = Config.get_t_achievement_reward(next_level)
	if t_next_level == nil then
		return
	end
	
	local fz = current_point - Config.get_t_achievement_reward(hall_achieve_data_.now_level).total_point
	local fm = t_next_level.level_up_point
	
	local value = fz / fm
	
	if value >= 1 then
		hall_achieve_data_.now_level = next_level
		if hall_achieve_data_.now_level ~= final_level then
			l_level_.text = hall_achieve_data_.now_level
			r_level_.text = hall_achieve_data_.now_level + 1
			
			if not arrow.gameObject.activeSelf then
				arrow.gameObject:SetActive(true)
			end
			
			if not r_level_.gameObject.activeSelf then
				r_level_.gameObject:SetActive(true)
			end	
		elseif hall_achieve_data_.now_level == final_level then
			l_level_.text = hall_achieve_data_.now_level
			if arrow.gameObject.activeSelf then
				arrow.gameObject:SetActive(false)
			end
			
			if r_level_.gameObject.activeSelf then
				r_level_.gameObject:SetActive(false)
			end
			
			if value == 1 then
				value = 0
			end
		end
		lua_script_:PlaySound('levelup')
	else
		l_level_.text = hall_achieve_data_.now_level
	end
	
	if current_point < hall_achieve_data_.now_point + Config.get_t_achievement(hall_achieve_data_.achieve_id).point then  --
		hall_achieve_data_.current_point = hall_achieve_data_.current_point + 1
	else
		hall_achieve_data_.finish = true
	end
	sc.value = value	
end

--获得当前的等级
function AchieveAnimation.GetCurrentAchieveLevel()
	for i = 1,Config.max_achieve_level do
		local t_level_reward = Config.get_t_achievement_reward(i)
		if self.player.achieve_point < t_level_reward.total_point then
			return i - 1
		end
	end
	return Config.max_achieve_level
end

function AchieveAnimation.OnClick(obj)
	if obj.name == 'ok_btn' then
		timerMgr:RemoveTimer('achieve_level_up')
		hall_achieve_data_.finish = true
		hall_achieve_state = false
		if hall_achieve_obj_ ~= nil then
			GameObject.Destroy(hall_achieve_obj_)
		end
		AchieveAnimation.CheckHallAchieve()
	elseif obj.name == 'can_btn' then
		GUIRoot.ShowGUI("SharePanel",{2, "achieve_reach"})
	end
end
----------------------------------------通知服务端该成就完成--------------------------------------------

function AchieveAnimation.CMSG_ACHIEVE()
	local msg = msg_hall_pb.cmsg_achieve()
	msg.id = send_achieve_.achieve_id
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_ACHIEVE, data,{opcodes.SMSG_ACHIEVE})
end

function AchieveAnimation.SMSG_ACHIEVE()
	self.del_achieve_num(send_achieve_.achieve_id)
	self.player.achieve_reward:append(send_achieve_.achieve_id) --增加 append
	self.player.achieve_time:append(os.time()*1000)
	
	send_achieve_.now_level = AchieveAnimation.GetCurrentAchieveLevel()
	send_achieve_.now_point = self.player.achieve_point
	
	local t_achieve = Config.get_t_achievement(send_achieve_.achieve_id)
	self.player.achieve_point = self.player.achieve_point + t_achieve.point
	
	AchieveAnimation.OpenHallAchieve(send_achieve_)
	
	local msg = s_message.New()
	msg.name = "refresh_all_achieve"
	Message.add_message(msg)
end
---------------------------------------战场中使用------------------------------------
function AchieveAnimation.UpdateFightAchieve()
	local timeSpan = Time.deltaTime / Time.timeScale
	if fightQueue.now_pos < fightQueue.Length then
		local pos = fightQueue.now_pos
		if fightQueue.rtimes[pos] <= 0 then
			fightQueue.objs[pos]:GetComponent("Animator"):Rebind()
			fightQueue.objs[pos]:SetActive(true)
			lua_script_:PlaySound('achievement')
		end
		fightQueue.rtimes[pos] = fightQueue.rtimes[pos] + timeSpan
		
		if fightQueue.rtimes[pos] >= fightQueue.need_s[pos] then
			fightQueue.rtimes[pos] = nil
			fightQueue.need_s[pos] = nil
			GameObject.Destroy(fightQueue.objs[pos])
			fightQueue.now_pos = fightQueue.now_pos + 1
		end
	end
end

function AchieveAnimation.ClearFightQueue()
	fightQueue = {Length = 0,need_s = {},rtimes = {},objs = {},now_pos = 0}
end

function AchieveAnimation.AddAnimation(data) --战场中使用
	--添加位置
	local pos = fightQueue.Length
	local clone_obj = LuaHelper.Instantiate(fight_res_.gameObject)
	
	clone_obj.transform.parent = animation_view_.transform
	clone_obj.transform.localScale = Vector3.zero
	clone_obj.transform.localPosition = Vector3(0,145.7,0)
	
	--设置成就的图像和文字
	local title_label = clone_obj.transform:Find("title"):GetComponent("UILabel")
	local desc_label = clone_obj.transform:Find("desc"):GetComponent("UILabel")
	local achieve_cup_icon = clone_obj.transform:Find("icon"):GetComponent("UISprite")
	
	title_label.text = data.name
	desc_label.text = data.desc
	achieve_cup_icon.spriteName = data.icon
	clone_obj:SetActive(false)
	
	fightQueue.objs[pos] = clone_obj
	fightQueue.need_s[pos] = 3.3
	fightQueue.rtimes[pos] = 0
	fightQueue.Length = fightQueue.Length + 1
end