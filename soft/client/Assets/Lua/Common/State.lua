State = {}

State.state = {
	ss_null		= 0,
	ss_login	= 1,
	ss_hall		= 2,
	ss_battle	= 3,
	ss_ofbattle	= 4,
	ss_newplayerguide = 5,
}
State.cur_state = State.state.ss_null

function State.ChangeState(state, param)
	State.LeaveState(State.cur_state, param)
	State.cur_state = state
	State.EnterState(State.cur_state, param)
end

function State.EnterState(state, param)
	if state == State.state.ss_login then
		GUIRoot.ShowGUI("StartPanel")
		soundMgr:play_mus("hall")
	elseif state == State.state.ss_hall then
		mapMgr:LoadScene("sf_start", State.LoadEnd)
		GUIRoot.ShowGUI("LoadPanel", {false, "hall"})
	elseif state == State.state.ss_battle then
		mapMgr:LoadScene("sf_fight001", State.LoadEnd)
		GUIRoot.ShowGUI("LoadPanel", {false, "fight_00"..math.random(1, 2)})
	elseif state == State.state.ss_ofbattle then
		mapMgr:LoadScene("sf_fight001", State.LoadEnd)
		GUIRoot.ShowGUI("LoadPanel", {false, "fight_00"..math.random(1, 2)})
	elseif state == State.state.ss_newplayerguide then
		mapMgr:LoadScene("sf_fight001", State.LoadEnd)
		GUIRoot.ShowGUI("LoadPanel", {false, "fight_00"..math.random(1, 2)})
	end 
end

function State.LeaveState(state, param)
	if state == State.state.ss_login then
		LoginPanel.Fini()
		GUIRoot.HideGUI("StartPanel")
	elseif state == State.state.ss_hall then
		GUIRoot.HideGUI("HallPanel")
		NoticePanel.FiniInvert()
		HallScene.RemoveRole()
		HallScene.RemoveFashion()
	elseif state == State.state.ss_battle then
		Battle.OnDestroy()
	elseif state == State.state.ss_ofbattle then
		Battle.OnDestroy()
	elseif state == State.state.ss_newplayerguide then
		Battle.OnDestroy()
	end
end

function State.LoadEnd()
	--print('State.LoadEnd',State.cur_state)
	if State.cur_state == State.state.ss_hall then
		mapMgr:SetInitCam(0, 0.6, 3, -5, 180, 0, 1)
		LoadPanel.close(State.LoadEndHall)
	elseif State.cur_state == State.state.ss_battle then
		State.PreLoad()
		mapMgr:SetInitCam(0, 17, -16.2, 45, 0, 0, 2)
		State.InitSelfData()
		Battle.Awake("sf_fight001", true)
	elseif State.cur_state == State.state.ss_ofbattle then
		State.PreLoad()
		mapMgr:SetInitCam(0, 17, -16.2, 45, 0, 0, 2)
		State.InitSelfData()
		Battle.Awake("sf_fight001", false)
	elseif State.cur_state == State.state.ss_newplayerguide then
		State.PreLoad()
		mapMgr:SetInitCam(0, 17, -16.2, 45, 0, 0, 2)
		State.InitSelfData()
		Battle.Awake("sf_fight001",false,true)
		print('sf_fight true')
	end
end

function State.LoadEndHall()
	GUIRoot.ShowGUI("HallPanel")
end

function State.PreLoad()
	for i = 1, #Config.t_preload do
		for j = 1, Config.t_preload[i].num do
			local eff = resMgr:CreateEffect(Config.t_preload[i].name,false)
			resMgr:DeleteEffect(eff)
		end
	end
end

function State.Reset()
	ConnectTcp.Disconnect()
	GameTcp.Disconnect()
	self.ClearData()
	LuaHelper.restart()
end

function State.InitSelfData()
	local player = dhc.player_t.New()
	player.guid = self.player.guid
	player.serverid = self.player.serverid
	player.name = self.player.name
	player.region_id = self.player.region_id
	player.infomation = self.player.infomation
	player.sex = self.player.sex
	player.birth_time = self.player.birth_time
	player.last_daily_time = self.player.last_daily_time
	player.last_week_time = self.player.last_week_time
	player.last_month_time = self.player.last_month_time
	player.last_login_time = self.player.last_login_time
	player.last_check_time = self.player.last_check_time
	player.is_guide = self.player.is_guide
	
	player.gold = self.player.gold
	player.jewel = self.player.jewel
	player.level = self.player.level
	player.exp = self.player.exp
	player.cup = self.player.cup
	player.snow = self.player.snow
	player.battle_gold = self.player.battle_gold
	player.max_cup = self.player.max_cup
	
	player.box_zd_num = self.player.box_zd_num
	player.box_zd_opened = self.player.box_zd_opened
	for i = 1,#self.player.box_ids do
		player.box_ids:Add(self.player.box_ids[i])
	end
	player.box_open_slot = self.player.box_open_slot
	player.box_open_time = self.player.box_open_time
	player.sign_time = self.player.sign_time
	player.sign_index = self.player.sign_index
	player.sign_finish = self.player.sign_finish
	
	for i = 1,#self.player.role_guid do
		player.role_guid:Add(self.player.role_guid[i])
	end
	player.role_on = self.player.role_on
	for i = 1,#self.player.item_id do
		player.item_id:Add(self.player.item_id[i])
	end
	for i = 1,#self.player.item_num do
		player.item_num:Add(self.player.item_num[i])
	end
	for i = 1,#self.player.avatar do
		player.avatar:Add(self.player.avatar[i])
	end
	player.avatar_on = self.player.avatar_on
	for i = 1,#self.player.battle_his_guids do
		player.battle_his_guids:Add(self.player.battle_his_guids[i])
	end
	for i = 1,#self.player.post_guids do
		player.post_guids:Add(self.player.post_guids[i])
	end
	for i = 1,#self.player.achieve_id do
		player.achieve_id:Add(self.player.achieve_id[i])
	end
	for i = 1,#self.player.achieve_num do
		player.achieve_num:Add(self.player.achieve_num[i])
	end
	for i = 1,#self.player.achieve_reward do
		player.achieve_reward:Add(self.player.achieve_reward[i])
	end	
	for i = 1,#self.player.achieve_time do
		player.achieve_time:Add(self.player.achieve_time[i])
	end
	player.achieve_point = self.player.achieve_point
	player.achieve_index = self.player.achieve_index
	for i = 1,#self.player.toukuang do
		player.toukuang:Add(self.player.toukuang[i])
	end
	for i = 1,#self.player.toukuang_time do
		player.toukuang_time:Add(self.player.toukuang_time[i])
	end
	player.toukuang_on = self.player.toukuang_on
	for i = 1,#self.player.task_id do
		player.task_id:Add(self.player.task_id[i])
	end
	for i = 1,#self.player.task_num do
		player.task_num:Add(self.player.task_num[i])
	end
	for i = 1,#self.player.task_reward do
		player.task_reward:Add(self.player.task_reward[i])
	end
	for i = 1,#self.player.fashion_id do
		player.fashion_id:Add(self.player.fashion_id[i])
	end
	for i = 1,#self.player.fashion_on do
		player.fashion_on:Add(self.player.fashion_on[i])
	end
	for i = 1,#self.player.daily_id do
		player.daily_id:Add(self.player.daily_id[i])
	end
	for i = 1,#self.player.daily_num do
		player.daily_num:Add(self.player.daily_num[i])
	end
	for i = 1,#self.player.daily_reward do
		player.daily_reward:Add(self.player.daily_reward[i])
	end
	player.daily_point = self.player.daily_point
	for i = 1,#self.player.daily_get_id do
		player.daily_get_id:Add(self.player.daily_get_id[i])
	end
	for i = 1,#self.player.level_reward do
		player.level_reward:Add(self.player.level_reward[i])
	end
	for i = 1,#self.player.social_golds do
		player.social_golds:Add(self.player.social_golds[i])
	end
	
	player.total_recharge = self.player.total_recharge
	player.total_spend = self.player.total_spend
	player.change_name_num = self.player.change_name_num
	player.fenxiang_num = self.player.fenxiang_num
	player.fenxiang_total_num = self.player.fenxiang_total_num
	player.fenxiang_state = self.player.fenxiang_state
	for i = 1,#self.player.libao_nums do
		player.libao_nums:Add(self.player.libao_nums[i])
	end
	
	player.battle_num = self.player.battle_num
	player.offline_battle_time = self.player.offline_battle_time
	player.first_recharge = self.player.first_recharge
	player.yue_time = self.player.yue_time
	player.yue_reward = self.player.yue_reward
	player.yue_first = self.player.yue_first
	player.nian_time = self.player.nian_time
	player.nian_reward = self.player.nian_reward
	player.nian_first = self.player.nian_first
	for i = 1,#self.player.duobao_items do
		player.duobao_items:Add(self.player.duobao_items[i])
	end
	player.duobao_num = self.player.duobao_num
	--player.battle_reset_skill_num = self.player.battle_reset_skill_num
	
	local roles = System.Collections.Generic.List_dhc_role_t.New()
	for i =1 ,#self.roles do
		local role = dhc.role_t.New()
		role.guid = self.roles[i].guid
		role.player_guid = self.roles[i].player_guid
		role.template_id = self.roles[i].template_id
		role.level = self.roles[i].level
		roles:Add(role)
	end
	
	Battle.UpdateSelfData(player,self.guid,self.battle_code,roles,self.quality,self.share_url,self.eff_quality)
end

function State.reset_skill_num(num)
	 --self.player.battle_reset_skill_num = self.player.battle_reset_skill_num + num;
end

function State.set_playerguide(state)
	self.player.is_guide = state;
end

function State.FromClientToLuaPlayer_t(player)
	self.player.guid = player.guid
	self.player.serverid = player.serverid
	self.player.name = player.name
	self.player.region = player.region
	self.player.infomation = player.infomation
	self.player.sex = player.sex
	self.player.birth_time = player.birth_time
	self.player.last_daily_time = player.last_daily_time
	self.player.last_week_time = player.last_week_time
	self.player.last_month_time = player.last_month_time
	self.player.last_login_time = player.last_login_time
	self.player.last_check_time = player.last_check_time
	self.player.is_guide = player.is_guide
	
	self.player.gold = player.gold
	self.player.jewel = player.jewel
	self.player.level = player.level
	self.player.exp = player.exp
	self.player.cup = player.cup
	self.player.snow = player.snow
	self.player.battle_gold = player.battle_gold
	self.player.max_cup = player.max_cup
	
	self.player.box_zd_num = player.box_zd_num 
	self.player.box_zd_opened = player.box_zd_opened
	
	self.player.box_ids:clear()
	for i = 0,player.box_ids.Count - 1 do
		self.player.box_ids:append(player.box_ids[i])
	end
	
	self.player.box_open_slot = player.box_open_slot
	self.player.box_open_time = player.box_open_time
	self.player.sign_time = player.sign_time
	self.player.sign_index = player.sign_index
	self.player.sign_finish = player.sign_finish
	
	self.player.role_guid:clear()
	for i = 0,player.role_guid.Count - 1 do
		self.player.role_guid:append(player.role_guid[i])
	end
	self.player.role_on = player.role_on
	
	self.player.item_id:clear()
	for i = 0,player.item_id.Count - 1 do
		self.player.item_id:append(player.item_id[i])
	end
	self.player.item_num:clear()
	for i = 0,player.item_num.Count - 1 do
		self.player.item_num:append(player.item_num[i])
	end
	self.player.avatar:clear()
	for i = 0,player.avatar.Count - 1 do
		self.player.avatar:append(player.avatar[i])
	end
	self.player.avatar_on = player.avatar_on
	
	self.player.battle_his_guids:clear()
	for i = 0,player.battle_his_guids.Count - 1 do
		self.player.battle_his_guids:append(player.battle_his_guids[i])
	end
	self.player.post_guids:clear()
	for i = 0,player.post_guids.Count - 1 do
		self.player.post_guids:append(player.post_guids[i])
	end
	self.player.achieve_id:clear()
	for i = 0,player.achieve_id.Count - 1 do
		self.player.achieve_id:append(player.achieve_id[i])
	end
	self.player.achieve_num:clear()
	for i = 0,player.achieve_num.Count - 1 do
		self.player.achieve_num:append(player.achieve_num[i])
	end
	self.player.achieve_reward:clear()
	for i = 0,player.achieve_reward.Count - 1 do
		self.player.achieve_reward:append(player.achieve_reward[i])
	end
	self.player.achieve_time:clear()
	for i = 0,player.achieve_time.Count - 1 do
		self.player.achieve_time:append(player.achieve_time[i])
	end
	self.player.achieve_point = player.achieve_point
	self.player.achieve_index = player.achieve_index
	
	self.player.toukuang:clear()
	for i = 0,player.toukuang.Count - 1 do
		self.player.toukuang:append(player.toukuang[i])
	end
	self.player.toukuang_time:clear()
	for i = 0,player.toukuang_time.Count - 1 do
		self.player.toukuang_time:append(player.toukuang_time[i])
	end
	self.player.toukuang_on = player.toukuang_on
	self.player.task_id:clear()
	for i = 0,player.task_id.Count - 1 do
		self.player.task_id:append(player.task_id[i])
	end
	self.player.task_num:clear()
	for i = 0,player.task_num.Count - 1 do
		self.player.task_num:append(player.task_num[i])
	end
	self.player.task_reward:clear()
	for i = 0,player.task_reward.Count - 1 do
		self.player.task_reward:append(player.task_reward[i])
	end
	self.player.fashion_id:clear()
	for i = 0,player.fashion_id.Count - 1 do
		self.player.fashion_id:append(player.fashion_id[i])
	end
	self.player.fashion_on:clear()
	for i = 0,player.fashion_on.Count - 1 do
		self.player.fashion_on:append(player.fashion_on[i])
	end
	self.player.daily_id:clear()
	for i = 0,player.daily_id.Count - 1 do
		self.player.daily_id:append(player.daily_id[i])
	end
	self.player.daily_num:clear()
	for i = 0,player.daily_num.Count - 1 do
		self.player.daily_num:append(player.daily_num[i])
	end
	self.player.daily_reward:clear()
	for i = 0,player.daily_reward.Count - 1 do
		self.player.daily_reward:append(player.daily_reward[i])
	end
	self.player.daily_point = player.daily_point
	self.player.daily_get_id:clear()
	for i = 0,player.daily_get_id.Count - 1 do
		self.player.daily_get_id:append(player.daily_get_id[i])
	end
	self.player.level_reward:clear()
	for i = 0,player.level_reward.Count - 1 do
		self.player.level_reward:append(player.level_reward[i])
	end
	self.player.social_golds:clear()
	for i = 0,player.social_golds.Count - 1 do
		self.player.social_golds:append(player.social_golds[i])
	end

	self.player.total_recharge = player.total_recharge
	self.player.total_spend = player.total_spend
	self.player.change_name_num = player.change_name_num
	self.player.fenxiang_num = player.fenxiang_num
	self.player.fenxiang_total_num = player.fenxiang_total_num
	self.player.fenxiang_state = player.fenxiang_state
	self.player.libao_nums:clear()
	for i = 0,player.libao_nums.Count - 1 do
		self.player.libao_nums:append(player.libao_nums[i])
	end

	
	self.player.battle_num = player.battle_num
	self.player.offline_battle_time = player.offline_battle_time
	self.player.first_recharge = player.first_recharge
	self.player.yue_time = player.yue_time
	self.player.yue_reward = player.yue_reward
	self.player.yue_first = player.yue_first
	self.player.nian_time = player.nian_time
	self.player.nian_reward = player.nian_reward
	self.player.nian_first = player.nian_first
	self.player.duobao_items:clear()
	for i = 0,player.duobao_items.Count - 1 do
		self.player.duobao_items:append(player.duobao_items[i])
	end

	self.player.duobao_num = player.duobao_num
	--self.player.battle_reset_skill_num = player.battle_reset_skill_num
end