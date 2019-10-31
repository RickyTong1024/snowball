NewAchievePanel = {}

NewAchievePanel.bobjpool = nil
local global_pools = {}

local all_achieve_pool = {}
local all_achieve_reward_pool = {}
local achieve_detail_pool = {}

local achieve_res
local achieve_level_res
local dark_detail_res_
local light_detail_res_
local line_res_

local lua_script_

local get_rewardt_
local go_paget_
local go_paget_lb_
local silder_
local level_icon_
local ach_level_lb_
local wrold_rank_lb_
local next_level_lb_
local next_level_rewardt_
local exp_label_
local left_t_ --左边的边界
local top_t_ --上边界

local level_reward_panel_
local all_achieve_panel_
local achieve_detail_panel_
local all_chieve_drag_box_

local page_type = 0  --0 所有的成就页面 1=成就奖励页面

local send_reward_index_  --领取的奖励index

function NewAchievePanel.Awake(obj)
	GUIRoot.ShowGUI('BackPanel', {3})
	lua_script_ = obj:GetComponent('LuaUIBehaviour')
	achieve_res = obj.transform:Find('anchor_right/achieves_res').gameObject
	achieve_level_res = obj.transform:Find('anchor_right/leve_reward_res').gameObject
	
	get_rewardt_ = obj.transform:Find('anchor_left/get_btn')
	lua_script_:AddButtonEvent(get_rewardt_.gameObject,'click',NewAchievePanel.OnClick)
	go_paget_ = obj.transform:Find('anchor_left/back_btn')
	go_paget_lb_ = go_paget_:Find('label'):GetComponent('UILabel')
	lua_script_:AddButtonEvent(go_paget_.gameObject, "click", NewAchievePanel.OnClick)
	
	silder_ = obj.transform:Find('anchor_left/jd_sw'):GetComponent('UISlider')
	level_icon_ = obj.transform:Find('anchor_left/cur_icon'):GetComponent('UISprite')
	ach_level_lb_ = obj.transform:Find('anchor_left/ach_level_lb'):GetComponent('UILabel')
	wrold_rank_lb_ = obj.transform:Find('anchor_left/all_rank'):GetComponent('UILabel')
	next_level_lb_ = obj.transform:Find('anchor_left/next_level_lb'):GetComponent('UILabel')
	exp_label_ = obj.transform:Find('anchor_left/exp_label'):GetComponent('UILabel')
	
	local back_main_btn_ = obj.transform:Find('anchor_center/achieve_detail_panel/back_main_btn')
	lua_script_:AddButtonEvent(back_main_btn_.gameObject, "click", NewAchievePanel.OnClick)
	
	left_t_ = obj.transform:Find('anchor_left/line_2')
	top_t_ = obj.transform:Find('anchor_top/line')
	dark_detail_res_ = obj.transform:Find('anchor_center/achieve_detail_panel/detail_dark_res').gameObject
	light_detail_res_= obj.transform:Find('anchor_center/achieve_detail_panel/detail_light_res').gameObject
	line_res_ = obj.transform:Find('anchor_right/line_res').gameObject
	
	level_reward_panel_ = obj.transform:Find('anchor_right/achieve_level_panel')
	all_achieve_panel_ = obj.transform:Find('anchor_right/achieves_panel')
	achieve_detail_panel_ = obj.transform:Find('anchor_center/achieve_detail_panel')
	all_chieve_drag_box_ = obj.transform:Find('anchor_right/scview_box')
	
	next_level_rewardt_ = obj.transform:Find('anchor_left/rewards')
	
	NewAchievePanel.bobjpool = bobjpool()
	global_pools = {}
	all_achieve_pool = {}
	all_achieve_reward_pool = {}
	achieve_detail_pool = {}
	
	NewAchievePanel.RegisterMessage()
	NewAchievePanel.Show()
end

function NewAchievePanel.RegisterMessage()
	Message.register_handle("back_panel_msg", NewAchievePanel.Back)
	Message.register_handle("refresh_all_achieve", NewAchievePanel.ReLoadAllAchieves)
	Message.register_handle("back_panel_recharge", NewAchievePanel.Recharge)
	Message.register_handle("team_join_msg", NewAchievePanel.TeamJoin)
	Message.register_net_handle(opcodes.SMSG_ACHIEVE_REWARD,NewAchievePanel.SMSG_ACHIEVE_REWARD)
end

function NewAchievePanel.RemoveMessage()
	Message.remove_handle("back_panel_msg", NewAchievePanel.Back)
	Message.remove_handle("refresh_all_achieve", NewAchievePanel.ReLoadAllAchieves)
	Message.remove_handle("back_panel_recharge", NewAchievePanel.Recharge)
	Message.remove_handle("team_join_msg", NewAchievePanel.TeamJoin)
	Message.remove_net_handle(opcodes.SMSG_ACHIEVE_REWARD,NewAchievePanel.SMSG_ACHIEVE_REWARD)
end

function NewAchievePanel.Back()
	GUIRoot.HideGUI("NewAchievePanel")
	GUIRoot.HideGUI("BackPanel")
	NewAchievePanel.OnDestroy()
	GUIRoot.ShowGUI("HallPanel")
end

function NewAchievePanel.RefreshLeftMsg()
	--上半部分 
	local now_level = NewAchievePanel.GetCurrentAchieveLevel()
	local next_level = now_level + 1

	local t_now_achieve_reward = Config.get_t_achievement_reward(now_level)
	local t_next_achieve_reward = Config.get_t_achievement_reward(next_level)

	if t_next_achieve_reward == nil then
		silder_ = 1
		exp_label_.text = 'max'
	else
		local l_value = self.player.achieve_point - t_now_achieve_reward.total_point
		local r_value = t_next_achieve_reward.level_up_point
		exp_label_.text = l_value..'/'..r_value
		silder_.value = l_value / r_value
	end
	ach_level_lb_.text = now_level
	
	--下半部分
	local rd = NewAchievePanel.GetNextReward()

	if rd == nil then
		next_level_lb_.text = string.format(Config.get_t_script_str('NewAchievePanel_001'),Config.max_achieve_level)
	else
		next_level_lb_.text = string.format(Config.get_t_script_str('NewAchievePanel_001'),rd.id) --rd.id..Config.get_t_script_str('NewAchievePanel_001')--'级奖励'
	end
	
	if rd == nil then
		get_rewardt_:GetComponent('UISprite').spriteName = 'b1_gray'
		get_rewardt_:GetComponent('BoxCollider').enabled = false
	else
		if self.player.achieve_point >= rd.total_point then
			get_rewardt_:GetComponent('UISprite').spriteName = 'b1_yellow'
			get_rewardt_:GetComponent('BoxCollider').enabled = true
		else
			get_rewardt_:GetComponent('UISprite').spriteName = 'b1_gray'
			get_rewardt_:GetComponent('BoxCollider').enabled = false
		end
		get_rewardt_:Find('achieve_index'):GetComponent('UILabel').text = rd.id
	end
	
	for i = 1,next_level_rewardt_.childCount do
		GameObject.Destroy(next_level_rewardt_:GetChild(i - 1).gameObject)
	end
	
	if rd ~= nil then
		local max_len = #rd.rewards * 85 / 2 - 42.5
		for i = 1,#rd.rewards do
			local reward = rd.rewards[i]
			local t_reward = Config.get_t_reward(reward.type,reward.value1)
			local reward_obj = nil	
			if t_reward ~= nil then
				if t_reward.color == nil then
					reward_obj = IconPanel.GetIcon("reward_res", nil,t_reward.icon, 1, reward.value2)
				else
					reward_obj = IconPanel.GetIcon("reward_res", nil,t_reward.icon,t_reward.color, reward.value2)
				end
			end
		
			reward_obj.transform:Find("icon").name = reward.type.."+"..reward.value1
			reward_obj.transform.parent = next_level_rewardt_
			reward_obj.transform.localPosition = Vector3(85 * (i - 1) - max_len,0,0)
			reward_obj.transform.localScale = Vector3.one
			reward_obj:SetActive(true)
		end
	end
end

function NewAchievePanel.Show()
	if page_type == 0 then
		if not all_achieve_panel_.gameObject.activeSelf then
			all_achieve_panel_.gameObject:SetActive(true)
		end
		
		if level_reward_panel_.gameObject.activeSelf then
			level_reward_panel_.gameObject:SetActive(false)
		end
		NewAchievePanel.LoadAllAchieves()
	elseif page_type == 1 then
		if all_achieve_panel_.gameObject.activeSelf then
			all_achieve_panel_.gameObject:SetActive(false)
		end
		
		if not level_reward_panel_.gameObject.activeSelf then
			level_reward_panel_.gameObject:SetActive(true)
		end
		NewAchievePanel.LoadAllAchieveLevel()
	end
end

function NewAchievePanel.LoadAllAchieves()
	local all_achieve_types = {}
	for k,v in pairs(Config.t_achievement.achieveTrees) do
		if #v > 0 and v[1].lock ~= 1 then
			table.insert(all_achieve_types,k)
		end	
	end
	
	
	
	
	local comp = function (a,b)
		local as = 0
		local bs = 0
		for i = 1,#self.player.achieve_reward do
			if self.player.achieve_reward[i] == a then
				as = 1
			elseif self.player.achieve_reward[i] == b then
				bs = 1
			end
		end
	
		return as > bs
	end
	table.sort(all_achieve_types,comp)
	
	local uv = all_achieve_panel_:GetComponent('UIScrollView')
	if all_achieve_panel_:GetComponent('SpringPanel') ~= nil then
		all_achieve_panel_:GetComponent('SpringPanel').enabled = false
	end
	
	local offset_left = left_t_.transform.localPosition.x + left_t_:GetComponent('UIWidget').width / 2
	local panel_final_w = GUIRoot.width - offset_left
	
	local offset_top = top_t_.transform.localPosition.y - top_t_:GetComponent('UIWidget').height / 2
	local panel_final_h = GUIRoot.height - math.abs(offset_top)
	
	local y = GUIRoot.height / 2 - math.abs(offset_top) - panel_final_h / 2
	local panel = all_achieve_panel_:GetComponent('UIPanel')
	panel.transform.localPosition = Vector3(-panel_final_w/2,y + 20,0)
	panel.baseClipRegion = Vector4(0,0,panel_final_w,panel_final_h)
	panel.clipSoftness = Vector2(0,20)

	local x_num = math.floor(panel_final_w / 145)
	local x_add = math.floor((panel_final_w - 145 * x_num)/(x_num - 1))

	local x_ori = -panel_final_w / 2 + 145 / 2
	local y_ori = panel_final_h / 2 - 230 / 2

	panel.clipOffset = Vector2(0, 0)
	
	all_chieve_drag_box_.localPosition = Vector3(-panel_final_w/2,y,0)
	all_chieve_drag_box_:GetComponent('BoxCollider').size = Vector3(panel_final_w,panel_final_h)
	
	local pos_index = 0
	for i = 1,#all_achieve_types do
		local parent_id = all_achieve_types[i]
		local bobj = NewAchievePanel.GetObjByName(achieve_res)
		table.insert(all_achieve_pool,bobj)
		
		local x_offset = pos_index % x_num
		local y_offset = math.floor(pos_index / x_num)
		bobj.objt.parent = all_achieve_panel_

		NewAchievePanel.bobjpool:set_localScale(bobj.objid,1,1,1)
		NewAchievePanel.bobjpool:set_localPosition(bobj.objid,x_ori + x_offset * 145 + x_offset * x_add,y_ori - y_offset * 230,0)
		
		local icon = bobj.objt:Find('achieve_icon_bg/achieve_icon')
		lua_script_:AddButtonEvent(icon.gameObject, "click", NewAchievePanel.OnClick)
		
		local list = Config.t_achievement.achieveTrees[parent_id]
		for j = #list,1,-1 do
			--最后一个完成的 如果没有完成显示第一个
			local sign = NewAchievePanel.IsComplete(list[j].id)
				
			if sign or (not sign and j == 1) then
				bobj.objt.name = list[j].id
				NewAchievePanel.InitAchieveTypeItem(bobj.obj)
				break
			end
		end
		
		pos_index = pos_index + 1
		if i % 4 == 0 and i ~= #all_achieve_types then
			--创建一个 线条
			local bobj = NewAchievePanel.GetObjByName(line_res_)
			table.insert(all_achieve_pool,bobj)
			bobj.objt.parent = all_achieve_panel_
			NewAchievePanel.bobjpool:set_localScale(bobj.objid,1,1,1)
			NewAchievePanel.bobjpool:set_localPosition(bobj.objid,0,y_ori + 115 - (y_offset + 1) * 230,0)
			bobj.obj:SetActive(true)
		end
	end
	NewAchievePanel.RefreshLeftMsg()
end

function NewAchievePanel.ReLoadAllAchieves()
	if all_achieve_panel_ ~= nil and not all_achieve_panel_.gameObject.activeSelf then
		return
	end
	for i = 1,all_achieve_panel_.childCount do
		local obj = all_achieve_panel_:GetChild(i - 1).gameObject
		NewAchievePanel.InitAchieveTypeItem(obj)
	end
end

function NewAchievePanel.InitAchieveTypeItem(obj)
	local achieve_id = tonumber(obj.name)
	local t_achievement = Config.get_t_achievement(achieve_id)
	
	local is_complete = false
	local get_time = nil

	for i = 1,#self.player.achieve_reward do
		if self.player.achieve_reward[i] == achieve_id then
			is_complete = true
			get_time = self.player.achieve_time[i]
			break
		end
	end
	--初始化 星星
	local max_len = t_achievement.max_star * 28 / 2 - 14
	for i = 1,t_achievement.max_star do
		local sobjt = obj.transform:Find('stars/star_'..i)
		if i < t_achievement.star then
			sobjt:GetComponent('UISprite').spriteName = 'star_icon'
		elseif i == t_achievement.star then
			if is_complete then
				sobjt:GetComponent('UISprite').spriteName = 'star_icon'
			else
				sobjt:GetComponent('UISprite').spriteName = 'xzcj_010'
			end
		else
			sobjt:GetComponent('UISprite').spriteName = 'xzcj_010'
		end
		sobjt.localPosition = Vector3((i - 1) * 28 - max_len,0,0)
		if not sobjt.gameObject.activeSelf then
			sobjt.gameObject:SetActive(true)
		end
	end
	
	for i = t_achievement.max_star + 1,5 do
		local sobj = obj.transform:Find('stars/star_'..i).gameObject
		if sobj.activeSelf then
			sobj:SetActive(false)
		end
	end
	
	obj.transform:Find('achieve_icon_bg/achieve_icon'):GetComponent('UISprite').spriteName = t_achievement.icon
	obj.transform:Find('bottom_bg/name'):GetComponent('UILabel').text = t_achievement.name
	
	--成就图标
	if is_complete then
		obj.transform:Find('achieve_icon_bg'):GetComponent('UISprite').spriteName = 'xzcj_006'
		local time_str = os.date('%Y.%m.%d',math.floor(get_time/1000))
		obj.transform:Find('bottom_bg/time'):GetComponent('UILabel').text = time_str..' '..Config.get_t_script_str('NewAchievePanel_002')
		obj.transform:Find('achieve_icon_bg/achieve_icon'):GetComponent('UISprite').IsGray = false
	else
		obj.transform:Find('achieve_icon_bg'):GetComponent('UISprite').spriteName = 'xzcj_007'
		obj.transform:Find('bottom_bg/time'):GetComponent('UILabel').text = Config.get_t_script_str('NewAchievePanel_003')--'未获得'
		obj.transform:Find('achieve_icon_bg/achieve_icon'):GetComponent('UISprite').IsGray = true
	end
	
	obj.name = t_achievement.id
	obj:SetActive(true)
end

function NewAchievePanel.IsComplete(achieve_id)
	for i = 1,#self.player.achieve_reward do
		if self.player.achieve_reward[i] == achieve_id then
			return true
		end
	end
	return false
end

function NewAchievePanel.LoadAllAchieveLevel()
	local achieve_rewards = {}
	for k,v in pairs(Config.t_achievement_reward) do
		table.insert(achieve_rewards,v)
	end
	local comp = function(a,b)
		return a.id < b.id
	end
	table.sort(achieve_rewards,comp)
	
	local uv = level_reward_panel_:GetComponent('UIScrollView')
	if level_reward_panel_:GetComponent('SpringPanel') ~= nil then
		level_reward_panel_:GetComponent('SpringPanel').enabled = false
	end
	
	local offset_left = left_t_.transform.localPosition.x + left_t_:GetComponent('UIWidget').width / 2
	local panel_final_w = GUIRoot.width - offset_left
	
	local offset_top = top_t_.transform.localPosition.y - top_t_:GetComponent('UIWidget').height / 2
	local panel_final_h = GUIRoot.height - math.abs(offset_top)
	
	local y = GUIRoot.height / 2 - math.abs(offset_top) - panel_final_h / 2
	local panel = level_reward_panel_:GetComponent('UIPanel')
	panel.transform.localPosition = Vector3(-panel_final_w/2,y,0)
	panel.baseClipRegion = Vector4(0,0,panel_final_w,panel_final_h + 40)
	panel.clipSoftness = Vector2(0,20)
	panel.clipOffset = Vector2.zero
	
	local top = y + panel_final_h / 2
	
	for i = 1,#achieve_rewards do
		local data = achieve_rewards[i]
		local bobj = NewAchievePanel.GetObjByName(achieve_level_res)
		bobj.objt:GetComponent('UIWidget').width = panel_final_w - 12
		table.insert(all_achieve_reward_pool,bobj)
		
		local y = top - (i - 1) * 119
		bobj.objt.parent = level_reward_panel_
		NewAchievePanel.bobjpool:set_localScale(bobj.objid,1,1,1)
		NewAchievePanel.bobjpool:set_localPosition(bobj.objid,0,y,0)
		bobj.objt.name = achieve_rewards[i].id
		NewAchievePanel.UpdateAchieveReward(bobj.objt)
	end
	NewAchievePanel.RefreshLeftMsg()
end

function NewAchievePanel.ReLoadAchieveLevelReward()
	if not level_reward_panel_.gameObject.activeSelf then
		return
	end
	for i = 1,level_reward_panel_.childCount do
		local objt = level_reward_panel_:GetChild(i - 1)
		NewAchievePanel.UpdateAchieveReward(objt)
	end
end

function NewAchievePanel.UpdateAchieveReward(objt)
	local reward_id = tonumber(objt.name)
	local t_achievement_reward = Config.get_t_achievement_reward(reward_id)
	objt:Find('title'):GetComponent('UILabel').text = string.format(Config.get_t_script_str('NewAchievePanel_001'),t_achievement_reward.id)
	
	local sign = false

	if t_achievement_reward.id <= self.player.achieve_index + 1 then
		sign = true
	end
	
	if sign then
		objt:Find('ly_label'):GetComponent('UILabel').text = '[54FD5A]'..Config.get_t_script_str('NewAchievePanel_004')..'[-]'
	else
		if self.player.achieve_point >= t_achievement_reward.total_point then
			objt:Find('ly_label'):GetComponent('UILabel').text = '[6DFFFF]'..Config.get_t_script_str('NewAchievePanel_005')..'[-]'
		else
			objt:Find('ly_label'):GetComponent('UILabel').text = '[F73F3F]'..Config.get_t_script_str('NewAchievePanel_006')..'[-]'
		end
	end
	
	local rewardt = objt:Find('rewards')
	for i = 1,rewardt.childCount do
		GameObject.Destroy(rewardt:GetChild(i - 1).gameObject)
	end
	
	for i = 1,#t_achievement_reward.rewards do
		local rd = t_achievement_reward.rewards[i]

		local t_reward = Config.get_t_reward(rd.type,rd.value1)
		local reward_obj = nil	
		if t_reward ~= nil then
			if t_reward.color == nil then
				reward_obj = IconPanel.GetIcon("reward_res", nil,t_reward.icon, 1, rd.value2)
			else
				reward_obj = IconPanel.GetIcon("reward_res", nil,t_reward.icon,t_reward.color, rd.value2)
			end
		end
		
		reward_obj.transform:Find("icon").name = rd.type.."+"..rd.value1
		reward_obj.transform.parent = rewardt
		reward_obj.transform.localPosition = Vector3(50 + -85 * (i - 1),0,0)
		reward_obj.transform.localScale = Vector3.one
		reward_obj:SetActive(true)
	end
	
	objt.gameObject:SetActive(true)
end

function NewAchievePanel.ClearCurrentView(page_id)
	if page_id == 0 then
		for i = 1,#all_achieve_pool do
			all_achieve_pool[i].state = false
			all_achieve_pool[i].obj:SetActive(false)
		end
	elseif page_id == 1 then
		for i = 1,#all_achieve_reward_pool do
			all_achieve_reward_pool[i].state = false
			all_achieve_reward_pool[i].obj:SetActive(false)
		end
	end
end

function NewAchievePanel.OnClick(obj)
	if obj.name == 'back_btn' then
		if page_type == 0 then
			page_type = 1
			NewAchievePanel.Show()
			go_paget_lb_.text = Config.get_t_script_str('NewAchievePanel_007')--'查看成就'
			NewAchievePanel.ClearCurrentView(0)
		elseif page_type == 1 then
			page_type = 0
			NewAchievePanel.Show()
			go_paget_lb_.text = Config.get_t_script_str('NewAchievePanel_008') --'查看所有奖励'
			NewAchievePanel.ClearCurrentView(1)
		end
	elseif obj.name == 'achieve_icon' then
		local parent_id = tonumber(obj.transform.parent.parent.name)
		local t_achievement = Config.get_t_achievement(parent_id)
		
		while t_achievement.pre > 0 do
			t_achievement = Config.get_t_achievement(t_achievement.pre)
		end
		NewAchievePanel.OpenAchieveDetail(t_achievement.id)
	elseif obj.name == 'back_main_btn' then
		for i = 1,#achieve_detail_pool do
			achieve_detail_pool[i].state = false
			achieve_detail_pool[i].obj:SetActive(false)
		end
		if achieve_detail_panel_.gameObject.activeSelf then
			achieve_detail_panel_.gameObject:SetActive(false)
		end
	elseif obj.name == 'get_btn' then
		local index = tonumber(obj.transform:Find('achieve_index'):GetComponent('UILabel').text)
		NewAchievePanel.CMSG_ACHIEVE_REWARD()
	end
end

function NewAchievePanel.OpenAchieveDetail(parent_id)
	local new_list = Config.t_achievement.achieveTrees[parent_id]
	local comp = function (a,b)
		return a.id < b.id
	end
	table.sort(new_list,comp)
	achieve_detail_panel_:Find('title'):GetComponent('UILabel').text = new_list[1].name
	achieve_detail_pool = {}
	local sachieve_panel = achieve_detail_panel_:Find('sachieve_panel')
	
	for i = 1,#new_list do
		local isComplete = false
		local timeStamp = nil
		for j = 1,#self.player.achieve_reward do
			if self.player.achieve_reward[j] == new_list[i].id then
				isComplete = true
				timeStamp = self.player.achieve_time[j]
				break
			end
		end
		local bobj = nil
		if isComplete then
			bobj = NewAchievePanel.GetObjByName(light_detail_res_)
		else
			bobj = NewAchievePanel.GetObjByName(dark_detail_res_)
		end
		bobj.objt.parent = sachieve_panel
		local x = 185 * (i - 1) - (185 * #new_list / 2 - 92.5)
		NewAchievePanel.bobjpool:set_localScale(bobj.objid,1,1,1)
		NewAchievePanel.bobjpool:set_localPosition(bobj.objid,x,0,0)
		table.insert(achieve_detail_pool,bobj)
		local obj = bobj.obj
		
		local max_len = new_list[i].max_star * 28 / 2 - 14
		for j = 1,new_list[i].max_star do
			local sobjt = obj.transform:Find('stars/star_'..j)
			if j <= new_list[i].star then
				sobjt:GetComponent('UISprite').spriteName = 'star_icon'
			else
				sobjt:GetComponent('UISprite').spriteName = 'xzcj_010'
			end
			sobjt.localPosition = Vector3((j - 1) * 28 - max_len,0,0)
			if not sobjt.gameObject.activeSelf then
				sobjt.gameObject:SetActive(true)
			end
		end
	
		for j = new_list[i].max_star + 1,5 do
			local sobj = obj.transform:Find('stars/star_'..j).gameObject
			if sobj.activeSelf then
				sobj:SetActive(false)
			end
		end
		
		obj.transform:Find('icon_bg/icon'):GetComponent('UISprite').spriteName = new_list[i].icon
		obj.transform:Find('score'):GetComponent('UILabel').text = '+'..new_list[i].point
		obj.transform:Find('achieve_name'):GetComponent('UILabel').text = new_list[i].desc
		
		if isComplete then
			obj.transform:Find('icon_bg'):GetComponent('UISprite').spriteName = 'xzcj_006'
			local time_str = os.date('%Y.%m.%d',timeStamp / 1000)
			obj.transform:Find('jindu'):GetComponent('UILabel').text = time_str..' '..Config.get_t_script_str('NewAchievePanel_002')
		else
			obj.transform:Find('icon_bg'):GetComponent('UISprite').spriteName = 'xzcj_007'
			obj.transform:Find('jindu'):GetComponent('UILabel').text = self.get_achieve_num(new_list[i].id)..'/'..new_list[i].target_num
		end
		obj:SetActive(true)
	end
	
	achieve_detail_panel_.gameObject:SetActive(true)
end

function NewAchievePanel.GetObjByName(ori_obj)
	local name = ori_obj.name

	if global_pools[name] == nil then
		--构建这个 对象
		local obj = LuaHelper.Instantiate(ori_obj)
		local objid = NewAchievePanel.bobjpool:add(obj)
		local objt = obj.transform
		NewAchievePanel.bobjpool:set_localScale(objid,1,1,1)	
		
		if global_pools == nil then
			global_pools = {}
		end
		
		if global_pools[name] == nil then
			global_pools[name] = {}
		end
		local dt = {obj = obj,objt = objt,objid = objid,state = true}
		table.insert(global_pools[name],dt)
		return dt
	elseif global_pools[name]~= nil then
		for i = 1,#global_pools[name] do
			if global_pools[name][i].state == false then
				global_pools[name][i].state = true
				return global_pools[name][i]
			end
		end
		
		local obj = LuaHelper.Instantiate(ori_obj)
		local objid = NewAchievePanel.bobjpool:add(obj)
		local objt = obj.transform
		NewAchievePanel.bobjpool:set_localScale(objid,1,1,1)
		
		local dt = {obj = obj,objt = objt,objid = objid,state = true}
		table.insert(global_pools[name],dt)
		return dt
	end
end

function NewAchievePanel.OnDestroy()
	NewAchievePanel.RemoveMessage()
	for k,v in pairs(global_pools) do
		for i = 1,#v do
			NewAchievePanel.bobjpool:remove(v[i].objid)
			GameObject.Destroy(v[i].obj)
		end
	end
	global_pools = {}
	all_achieve_pool = {}
	all_achieve_reward_pool = {}
	achieve_detail_pool = {}
	
	NewAchievePanel.RemoveMessage()
end

function NewAchievePanel.Recharge()
	GUIRoot.HideGUI('NewAchievePanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function NewAchievePanel.TeamJoin()
	GUIRoot.HideGUI('NewAchievePanel')
end

--获得当前的等级
function NewAchievePanel.GetCurrentAchieveLevel()
	for i = 1,Config.max_achieve_level do
		local t_level_reward = Config.get_t_achievement_reward(i)
		if self.player.achieve_point < t_level_reward.total_point then
			return i - 1
		end
	end
	return Config.max_achieve_level
end

-------------------------------------服务端----------------------------------
function NewAchievePanel.CMSG_ACHIEVE_REWARD() --领取等级奖励
	GameTcp.Send(opcodes.CMSG_ACHIEVE_REWARD, nil,{opcodes.SMSG_ACHIEVE_REWARD})
end

function NewAchievePanel.GetNextReward()
	for i = self.player.achieve_index + 2,Config.max_achieve_level do
		local t_achieve_reward = Config.get_t_achievement_reward(i)
		if #t_achieve_reward.rewards > 0 then
			return t_achieve_reward
		end
	end
end

function NewAchievePanel.SMSG_ACHIEVE_REWARD()
	local t_achieve_reward_ = NewAchievePanel.GetNextReward()
	local msg = {types = {},value1s = {},value2s = {},value3s = {}}
	
	for i = 1,#t_achieve_reward_.rewards do
		table.insert(msg.types,t_achieve_reward_.rewards[i].type)
		table.insert(msg.value1s,t_achieve_reward_.rewards[i].value1)
		table.insert(msg.value2s,t_achieve_reward_.rewards[i].value2)
		table.insert(msg.value3s,t_achieve_reward_.rewards[i].value3)
	end
	
	self.player.achieve_index = t_achieve_reward_.id - 1
	--弹出奖励获得
	local mail_rewad = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
	
	for i = 1, #mail_rewad do
		self.add_reward(mail_rewad[i])
	end
	if(#mail_rewad > 0) then
		GUIRoot.ShowGUI('GainPanel', {mail_rewad})
	end
	
	NewAchievePanel.RefreshLeftMsg()
	NewAchievePanel.ReLoadAchieveLevelReward()
end