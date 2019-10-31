PlayerData = {}
PlayerData.roles = {}
PlayerData.battle_results = {}
PlayerData.chest_rewards = {}
PlayerData.gonggao = {}
PlayerData.post_num = 0
PlayerData.apply_num = 0
PlayerData.social_gold = false
PlayerData.reject = false
PlayerData.unread_msg_num = 0
PlayerData.pre_battle_num = 0

PlayerData.share_url = ""
PlayerData.share_code = nil
PlayerData.home_url = ""
PlayerData.activity_url = ""
PlayerData.help_url = ""
PlayerData.bbs_url = ""
PlayerData.icon_url = ""
PlayerData.share_text = {}

PlayerData.mus_volum = 1
PlayerData.eff_volum = 1
PlayerData.quality = 0
PlayerData.eff_quality = 0

PlayerData.is_role_show = 0
PlayerData.is_achiv_show = 0

PlayerData.login_url = ""
PlayerData.host = ""
PlayerData.port = 0
PlayerData.wh = 0
PlayerData.whtext = ""
PlayerData.review = 0
PlayerData.whitelist = {}

PlayerData.font_color = {'[3defff]', '[ff41da]', '[ffcd00]', '[f01c1c]', '[c2e5ed]', '[57FC5B]'}
PlayerData.out_attr = {}

PlayerData.old_level = 1
PlayerData.is_ios_sh = false

local load_http_url = ""
local load_sever_code = 0

local load_share_state = 0
local load_gonggao_state = 0

PlayerData.max_gonggao_id = 0

PlayerData.friend_guids = {}
PlayerData.black_guids = {}

PlayerData.ios_purchase = {} 

function PlayerData.Init()
	local random_ = PlayerData.GetNowTimeString()
	local inf_url = platform_config_common.m_common_url.."info.json?v="..random_
	local gonggao_url = platform_config_common.m_common_url.."gonggao.json?v="..random_
	local share_text_url = platform_config_common.m_common_url.."share.json?v="..random_
	PlayerData.load_weburl(inf_url, PlayerData.load_share)
	PlayerData.load_weburl(gonggao_url, PlayerData.load_gongao)
	PlayerData.load_weburl(share_text_url, PlayerData.load_share_text)
	PlayerData.load_ios_purchase()
end

function PlayerData.load_ios_purchase()
	PlayerData.ios_purchase = {}
	if platform_config_common.m_platform == "ios_yymoon" then
		if PlayerPrefs.HasKey('AppPurse') then
			local str = PlayerPrefs.GetString('AppPurse')
			local co = PlayerData.Split(str,',')
			for i = 1,#co do
				local no = PlayerData.Split(co[i],'|')
				local c = {}
				c[1] = no[2]
				c[2] = no[3]
				PlayerData.ios_purchase[no[1]] = c
			end
		end
	end
end

function PlayerData.update_ios_purchase()
	if platform_config_common.m_platform == "ios_yymoon" then
		local str = ''
		for k,v in pairs(PlayerData.ios_purchase) do
			if v ~= nil then
				local tmp = tostring(k)..'|'..tostring(v[0])..'|'..tostring(v[1])
				str = str..tmp..','
			end
		end
		str = string.sub(str,1,-2)
		if str == '' then
			if PlayerPrefs.HasKey('AppPurse') then
				PlayerPrefs.DeleteKey('AppPurse')
			end
		else
			PlayerPrefs.SetString('AppPurse',str)
		end
		PlayerPrefs.Save()
	end
end

function PlayerData.Split(szFullString, szSeparator)  
    local nFindStartIndex = 1  
    local nSplitIndex = 1  
    local nSplitArray = {}  
    while true do  
       local nFindLastIndex = string.find(szFullString, szSeparator, nFindStartIndex)  
       if not nFindLastIndex then  
			nSplitArray[nSplitIndex] = string.sub(szFullString, nFindStartIndex, string.len(szFullString))  
			break  
       end  
       nSplitArray[nSplitIndex] = string.sub(szFullString, nFindStartIndex, nFindLastIndex - 1)  
       nFindStartIndex = nFindLastIndex + string.len(szSeparator)  
       nSplitIndex = nSplitIndex + 1  
    end  
    return nSplitArray  
end

function PlayerData.InitServer(code)
	load_sever_code = code
	local random_ = PlayerData.GetNowTimeString()
	local server_url = platform_config_common.m_common_url.."server.json?v="..random_

	PlayerData.load_weburl(server_url, PlayerData.load_server)
	if(load_sever_code == 1) then
		GUIRoot.ShowGUI("MaskPanel", {Config.get_t_script_str('PlayerData_001')})
	end
end

function PlayerData.GetNowTimeString()
	local year = timerMgr:dtnow().Year
	local month = timerMgr:dtnow().Month
	local day = timerMgr:dtnow().Day
	local hour = timerMgr:dtnow().Hour
	local mins = timerMgr:dtnow().Minute
	local ms = timerMgr:dtnow().Second
	
	local ts = string.format("%04d%02d%02d%02d%02d%02d",year,month,day,hour,mins,ms)
	return ts
end

function PlayerData.calc_out_attr()
	PlayerData.out_attr = {}
	for i = 1, 100 do
		table.insert(PlayerData.out_attr, 0)
	end
	for i = 1, #self.roles do
		local role = self.roles[i]
		local t_role = Config.get_t_role(role.template_id)
		for j = 1, #t_role.gskills do
			local t_role_buff = Config.get_t_role_buff(t_role.gskills[j])
			if t_role_buff.type == 3 then
				PlayerData.out_attr[t_role_buff.param1] = PlayerData.out_attr[t_role_buff.param1] + t_role_buff.param_value(role.level)
			end
		end
	end
	local t_exp = Config.get_t_exp(self.player.level)
	for i = 1, #t_exp.level_add do
		if t_exp.level_add[i].type == 3 then
			PlayerData.out_attr[t_exp.level_add[i].param1] = PlayerData.out_attr[t_exp.level_add[i].param1] + t_exp.level_add[i].param3
		end
	end
	for i = 1, #self.player.toukuang do
		local id = self.player.toukuang[i]
		local t_toukuang = Config.get_t_toukuang(id)
		for j = 1, #t_toukuang.gskills do
			if t_toukuang.gskills[j].type == 3 then
				PlayerData.out_attr[t_toukuang.gskills[j].param1] = PlayerData.out_attr[t_toukuang.gskills[j].param1] + t_toukuang.gskills[j].param3
			end
		end
	end
end

function PlayerData.get_out_attr(id)
	local a = PlayerData.out_attr[id]
	if tonumber(timerMgr:now_string()) < tonumber(self.player.yue_time) then
		local t_vip_attr = Config.get_t_vip_attr(1)
		for i = 1, #t_vip_attr.attrs do
			if t_vip_attr.attrs[i].param1 == id then
				a = a + t_vip_attr.attrs[i].param3
			end
		end
	elseif tonumber(timerMgr:now_string()) < tonumber(self.player.nian_time) then
		local t_vip_attr = Config.get_t_vip_attr(2)
		for i = 1, #t_vip_attr.attrs do
			if t_vip_attr.attrs[i].param1 == id then
				a = a + t_vip_attr.attrs[i].param3
			end
		end
	end
	return a
end

function PlayerData.add_role(role)
	if(role ~= nil and self.get_role_id(role.template_id) == nil) then
		table.insert(PlayerData.roles, role)
		local avatar_temp = Config.get_t_avatar_id(role.template_id)
		PlayerData.add_avatar(avatar_temp.id)
		PlayerData.calc_out_attr()
		LuaAchieve.RoleAmountChange(true)
	end
end

function PlayerData.role_on(role)
	local flag = false
	if(role ~= nil) then
		for i = 1, #PlayerData.roles do
			if(PlayerData.roles[i].guid == role.guid) then
				flag = true
			end
		end
		if(flag) then
			self.player.role_on = role.guid
		end
	end
end

function PlayerData.role_upgrage(role)
	if(role ~= nil) then
		for i = 1, #PlayerData.roles do
			if(PlayerData.roles[i].guid == role.guid) then
				role.level = role.level + 1
				PlayerData.calc_out_attr()
			end
		end
	end
end

function PlayerData.get_role(guid)
	for i = 1, #PlayerData.roles do
		if(PlayerData.roles[i].guid == guid) then
			return PlayerData.roles[i]
		end
	end
	return nil
end

function PlayerData.get_role_id(id)
	for i = 1, #PlayerData.roles do
		if(PlayerData.roles[i].template_id == id) then
			return PlayerData.roles[i]
		end
	end
	return nil
end

function PlayerData.add_battle_result(battle_result)
	if(battle_result ~= nil) then
		table.insert(PlayerData.battle_results, battle_result)
		PlayerData.battle_results = PlayerData.rank_battle_result(PlayerData.battle_results)
		if #PlayerData.battle_results > 10 then
			table.remove(PlayerData.battle_results, #PlayerData.battle_results)
		end
	end
end

function PlayerData.rank_battle_result(battle_results)
	for i = 1, #battle_results do
		for j = 1, #battle_results - i do
			if(tonumber(battle_results[j].time) < tonumber(battle_results[j + 1].time)) then
				local battle_result = battle_results[j + 1]
				battle_results[j + 1] = battle_results[j]
				battle_results[j] = battle_result
			end
		end
	end
	return battle_results
end 

function PlayerData.add_item(id, num)
	local flag = false
	for i = 1, #self.player.item_id do
		if(self.player.item_id[i] == id) then
			self.player.item_num[i] = self.player.item_num[i] + num
			flag = true
		end
	end
	if(not flag and num > 0) then
		self.player.item_id:append(id)
		self.player.item_num:append(num)
	end
end

function PlayerData.get_item_num(id)
	for i = 1, #self.player.item_id do
		if(self.player.item_id[i] == id) then
			return	self.player.item_num[i]
		end
	end
	return 0
end

function PlayerData.delete_item_num(id, num)
	for i = 1, #self.player.item_id do
		if(self.player.item_id[i] == id) then
			self.player.item_num[i] = self.player.item_num[i] - num
			if(self.player.item_num[i] <= 0) then
				self.player.item_id[i] = self.player.item_id[#self.player.item_id]
				self.player.item_num[i] = self.player.item_num[#self.player.item_num]
				table.remove(self.player.item_id, #self.player.item_id)
				table.remove(self.player.item_num, #self.player.item_num)
			end
		end
	end
end

function PlayerData.has_avatar(id)
	for i = 1, #self.player.avatar do
		if(self.player.avatar[i] == id) then
			return true
		end
	end
	return false
end

function PlayerData.add_avatar(id)
	local flag = false
	for i = 1, #self.player.avatar do
		if(self.player.avatar[i] == id) then
			flag = true
		end
	end
	if not flag then
		table.insert(self.player.avatar, id)
	end
end

function PlayerData.has_toukuang(id)
	for i = 1, #self.player.toukuang do
		if(self.player.toukuang[i] == id) then
			return true
		end
	end
	return false
end

function PlayerData.add_toukuang(id, time)
	flag = self.has_toukuang(id)
	if not flag then
		self.player.toukuang:append(id)
		PlayerData.calc_out_attr()
	end
end

function PlayerData.is_deadline(id)
	local index = 0
	for i = 1, #self.player.toukuang do
		if(self.player.toukuang[i] == id) then
			index = i
			break
		end
	end
	if(index ~= 0 and self.player.toukuang_time[index] ~= nil) then
		if(self.player.toukuang_time[index] == 0) then
			return false
		elseif(tonumber(self.player.toukuang_time[index]) > tonumber(timerMgr:now_string())) then
			return false
		else
			return true
		end
	else
		return true
	end
end

function PlayerData.add_fashion(id)
	local flag = false
	for i = 1, #self.player.fashion_id do
		if(self.has_fashion(id)) then
			flag = true
		end
	end
	if not flag then
		self.player.fashion_id:append(id)
		PlayerData.calc_out_attr()
	end
end

function PlayerData.has_fashion(id)
	for i = 1, #self.player.fashion_id do
		if(self.player.fashion_id[i] == id) then
			return true
		end
	end
	return false
end

function PlayerData.set_cup(cup)
	self.player.cup = cup
	if cup > self.player.max_cup then
		self.player.max_cup = cup
	end
	TopPanel.RefreshMoney()
	BackPanel.RefreshMoney()
end

function PlayerData.add_effect_resource(t, value2, num, from, speed)
	local eff_num = 0
	local v = toInt(value2 / num)
	if v < 1 then
		v = 1
	end
	while value2 > 0 do
		local add_num = v
		if(add_num > value2) then
			add_num = value2
		end
		value2 = value2 - add_num
		eff_num = eff_num + 1
		IconPanel.CreateEffect(t, from, speed, PlayerData.add_end_resource, {t, add_num})
	end
end

function PlayerData.add_end_resource(param)
	PlayerData.add_resource(param[1], param[2])
end

function PlayerData.add_resource(value1, value2, is_show)
	local tmp_jewel = self.player.jewel
	if(value1 == 1) then
		self.player.gold = self.player.gold + value2
		if(self.player.gold < 0) then
			self.player.gold = 0
		end
	elseif(value1 == 2) then
		self.player.jewel = self.player.jewel + value2
		if(self.player.jewel < 0) then
			self.player.jewel = 0
		end
	elseif(value1 == 3) then
		self.player.exp = self.player.exp + value2
		local exp_lv = Config.get_t_exp(self.player.level + 1)
		PlayerData.old_level = self.player.level
		while exp_lv ~= nil and self.player.exp >= exp_lv.exp do
			self.player.exp = self.player.exp - exp_lv.exp
			self.player.level = self.player.level + 1
			PlayerData.calc_out_attr()
			exp_lv = Config.get_t_exp(self.player.level + 1)
			--
			LuaAchieve.AccountLevelUp()
			TopPanel.LevelUp()
			BackPanel.LevelUp()
			platform._instance:on_game_user_upgrade(self.player.level)
		end

		if(exp_lv == nil) then
			self.player.exp = 0
		end
	elseif(value1 == 4) then
		self.player.snow = self.player.snow + value2
		if(self.player.snow < 0) then
			self.player.jewel = self.player.jewel + self.player.snow
			if(self.player.jewel < 0) then
				self.player.jewel = 0
			end
			self.player.snow = 0
		end
	elseif(value1 == 6) then
		self.set_cup(self.player.cup + value2)
	elseif(value1 == 7) then
		self.player.achieve_point = self.player.achieve_point + value2
		if(self.player.achieve_point < 0) then
			self.player.achieve_point = 0
		end
	elseif(value1 == 8) then
		self.player.daily_point = self.player.daily_point + value2
		if(self.player.daily_point < 0) then
			self.player.daily_point = 0
		end
	end
	if self.player.jewel < tmp_jewel then
		self.player.total_spend = self.player.total_spend + tmp_jewel - self.player.jewel
	end
	if(value2 > 0 and value1 <= 4 and value1 > 0) then
		TopPanel.AddResource(value1)
		BackPanel.AddResource(value1)
	end
	if(is_show or is_show == nil) then
		BackPanel.RefreshInf()
		BackPanel.RefreshMoney()
		TopPanel.RefreshInf()
		TopPanel.RefreshMoney()
	end
	HallPanel.ShowTip()
end

function PlayerData.get_resource(type)
	if(type == 1) then
		return self.player.gold
	elseif(type == 2) then
		return self.player.jewel
	elseif(type == 3) then
		return self.player.exp
	elseif(type == 4) then
		return self.player.snow
	elseif(type == 6) then
		return self.player.cup
	elseif(type == 7) then
		return self.player.achieve_point
	end
end

function PlayerData.count_reward(types, value1s, value2s, value3s)
	local rewards = {}
	for i = 1, #types do
		if(types[i] ~= 3) then
			local reward = {}
			reward.type = types[i]
			reward.value1 = value1s[i]
			reward.value2 = value2s[i]
			reward.value3 = value3s[i]
			table.insert(rewards, reward)
		end
	end
	return rewards
end

function PlayerData.add_reward(reward, is_show)
	if(reward.type == 1) then
		PlayerData.add_resource(reward.value1, reward.value2, is_show)
	elseif(reward.type == 2) then
		PlayerData.add_item(reward.value1, reward.value2)
	elseif(reward.type == 4) then
		PlayerData.add_avatar(reward.value1)
	elseif(reward.type == 5) then	
		PlayerData.add_chest(reward.value1)
	elseif(reward.type == 6) then
		PlayerData.add_toukuang(reward.value1)
	elseif(reward.type == 7) then
		PlayerData.add_fashion(reward.value1)
	end
	HallPanel.ShowTip()
end

function PlayerData.add_chest(value1)
	local pos = -1
	for i = 1, #self.player.box_ids do
		if(self.player.box_ids[i] == 0 and pos == -1) then
			pos = i
		end
	end
	if(pos ~= -1) then
		self.player.box_ids[pos] = value1
		HallPanel.GetChest(pos)
	end
end

function PlayerData.add_social_guid(type, guid)
	if(type == 2) then
		for i = 1, #self.friend_guids do
			if(self.friend_guids[i] == guid) then
				return 0
			end
		end
		table.insert(self.friend_guids, guid)
	elseif(type == 3) then
		for i = 1, #self.black_guids do
			if(self.black_guids[i] == guid) then
				return 0
			end
		end
		table.insert(self.black_guids, guid)
	end
end

function PlayerData.del_social_guid(type, guid)
	if(type == 2) then
		for i = 1, #self.friend_guids do
			if(self.friend_guids[i] == guid) then
				table.remove(self.friend_guids, i)
				break
			end
		end
	elseif(type == 3) then
		for i = 1, #self.black_guids do
			if(self.black_guids[i] == guid) then
				table.remove(self.black_guids, i)
				break
			end
		end
	end
end

function PlayerData.social_type(guid)
	for i = 1, #self.friend_guids do
		if(self.friend_guids[i] == guid) then
			return 2
		end
	end
	for i = 1, #self.black_guids do
		if(self.black_guids[i] == guid) then
			return 3
		end
	end
	return 0
end

function PlayerData.send_social_golds(guid)
	for i = 1, #self.player.social_golds do
		if(self.player.social_golds[i] == guid) then
			return true
		end
	end
	return false
end

function PlayerData.check_role()
	PlayerData.is_role_show = 0
	for k, v in pairs(Config.t_role) do
		local role_temp = v
		if(role_temp.type == 0) then
			local suipian_num = PlayerData.get_item_num(role_temp.suipian_id)
			local role_t = PlayerData.get_role_id(role_temp.id)
			if(role_t == nil) then
				if(role_temp.get_type == 0) then
					if(suipian_num >= role_temp.suipian_cost) then
						PlayerData.is_role_show = PlayerData.is_role_show + 1
					end
				elseif(role_temp.get_type == 1) then
					if(self.player.level >= role_temp.suipian_cost) then
						PlayerData.is_role_show = PlayerData.is_role_show + 1
					end
				end
			else
				local role_level = Config.get_t_role_level(role_t.level + 1)
				if(role_level ~= nil) then
					local suipian_cost = role_level.suipian_cost
					local gold_cost_num = role_level.get_gold_cost(role_temp.color)
					if(suipian_num >= suipian_cost and self.player.gold >= gold_cost_num) then
						PlayerData.is_role_show = PlayerData.is_role_show + 1
					end
				end
			end
		end
	end 
end

function PlayerData.check_task_daily_num()
	local task_num = LevelTask.GetCompleteTaskNum()
	local daily_num = LevelTask.GetDailyTaskCompleteNumber()
	return (task_num + daily_num)
end

function PlayerData.check_achieve()
	PlayerData.is_achiv_show = 0
	for i = self.player.achieve_index + 2,Config.max_achieve_level do
		if self.player.achieve_point >= Config.get_t_achievement_reward(i).total_point then
			if #Config.get_t_achievement_reward(i).rewards > 0 then
				PlayerData.is_achiv_show = PlayerData.is_achiv_show + 1
			end
		end
	end
end

function PlayerData.add_all_type_num(t, num)
	PlayerData.add_achieve_type_num(t, num)
	PlayerData.add_task_type_num(t, num)
	PlayerData.add_daily_type_num(t, num)
end

function PlayerData.add_achieve_type_num(t, num)
	local ids = Config.t_achievement.types[t]
	if ids == nil then
		return
	end
	for i = 1,  #ids do
		PlayerData.add_achieve_num(ids[i].id, num, false)
	end
end

function PlayerData.add_achieve_num(id, num, check)
	local t_achievement = Config.get_t_achievement(id)
	if t_achievement == nil then
		return
	end
	if check and t_achievement.dtype ~= 2 then
		return
	end
	for i = 1, #self.player.achieve_reward do
		if self.player.achieve_reward[i] == id then
			return
		end
	end
	for i = 1, #self.player.achieve_id do
		if self.player.achieve_id[i] == id then
			num = self.player.achieve_num[i] + num
			if num > t_achievement.target_num then
				num = t_achievement.target_num
			end
			self.player.achieve_num[i] = num
			return
		end
	end
	if num > t_achievement.target_num then
		num = t_achievement.target_num
	end
	self.player.achieve_id:append(id)
	self.player.achieve_num:append(num)
end

function PlayerData.get_achieve_num(id)
	t_achievement = Config.get_t_achievement(id)
	if t_achievement == nil then
		return 0
	end
	if t_achievement.type_id == 1 then
		num = 0
		for i = 1, #self.roles do
			flag = true
			role = self.roles[i]
			t_role = Config.get_t_role(role.template_id)
			if t_achievement.param1 ~= 2 and t_achievement.param1 ~= t_role.sex then
				flag = false
			end
			if t_achievement.param2 ~= 0 and t_achievement.param2 ~= t_role.color then
				flag = false
			end
			if flag then
				num = num + 1
			end
		end
		return num
	elseif t_achievement.type_id == 2 then
		return self.player.level
	elseif t_achievement.type_id == 3 then
		num = 0
		for i = 1, #self.roles do
			role = self.roles[i]
			if role.template_id == t_achievement.param1 then
				return role.level
			end
		end
		return num
	elseif t_achievement.type_id == 35 then
		if self.player.max_cup == nil then
			return 0
		else
			if self.player.max_cup >= t_achievement.param1 then
				return 1
			else
				return 0
			end
		end
	else
		for i = 1, #self.player.achieve_id do
			if self.player.achieve_id[i] == id then
				return self.player.achieve_num[i]
			end
		end
	end
	return 0
end

function PlayerData.del_achieve_num(id)
	for i = 1, #self.player.achieve_id do
		if self.player.achieve_id[i] == id then
			self.player.achieve_id[i] = self.player.achieve_id[#self.player.achieve_id]
			self.player.achieve_num[i] = self.player.achieve_num[#self.player.achieve_num]
			table.remove(self.player.achieve_id, #self.player.achieve_id)
			table.remove(self.player.achieve_num, #self.player.achieve_num)
			return
		end
	end
end

function PlayerData.add_task_type_num(t, num)
	local ids = Config.t_task_types[t]
	if ids == nil then
		return
	end
	for i = 1,#ids do
		PlayerData.add_task_num(ids[i].id, num, false);
	end
end

function PlayerData.add_task_num(id,num,check)
	local t_task = Config.get_t_task(id)
	if t_task == nil then
		return
	end
	
	if check and t_task.dtype ~= 2 then
		return
	end

	if self.player.level < t_task.level then
		return
	end
	
	for i = 1,#self.player.task_reward do
		if self.player.task_reward[i] == id then
			return
		end
	end
	
	for i = 1,#self.player.task_id do
		if self.player.task_id[i] == id then
			num = self.player.task_num[i] + num
			if num > t_task.target_num then
				num = t_task.target_num
			end
			self.player.task_num[i] = num
			return
		end
	end
	
	if num > t_task.target_num then
		num = t_task.target_num
	end
	
	self.player.task_id:append(id)
	self.player.task_num:append(num)
end

function PlayerData.get_task_num(id)
	local t_task = Config.get_t_task(id)
	if t_task == nil then
		return 0
	end
	if t_task.type == 1 then
		for i = 1, #self.roles do
			flag = true
			role = self.roles[i]
			t_role = Config.get_t_role(role.template_id)
			if t_task.param1 ~= 2 and t_task.param1 ~= t_role.sex then
				flag = false
			end
			if t_task.param2 ~= 0 and t_task.param2 ~= t_role.color then
				flag = false
			end
			if flag then
				num = num + 1
			end
		end
		return num
	elseif t_task.type == 2 then
		return self.player.level
	elseif t_task.type == 3 then
		num = 0
		for i = 1, #self.roles do
			role = self.roles[i]
			if role.level >= 5 then
				num = num + 1
			end
		end
		return num
	elseif t_task.type == 35 then
		if self.player.max_cup == nil then
			return 0
		else
			if self.player.max_cup >= t_task.param1 then
				return 1
			else
				return 0
			end
		end
	else
		for i = 1,#self.player.task_id do
			if self.player.task_id[i] == id then
				return self.player.task_num[i]
			end
		end
	end

	return 0
end

function PlayerData.del_task_num(id)
	for i = 1,#self.player.task_id do
		if self.player.task_id[i] == id then
			self.player.task_id[i] = self.player.task_id[#self.player.task_id]
			self.player.task_num[i] = self.player.task_num[#self.player.task_num]
			table.remove(self.player.task_id, #self.player.task_id)
			table.remove(self.player.task_num, #self.player.task_num)
		end
	end
end

function PlayerData.add_daily_type_num(type,num)
	local ids = Config.t_daily_types[type]
	if ids == nil then
		return
	end
	for i = 1,#ids do
		PlayerData.add_daily_num(ids[i].id,num,false)
	end
end

function PlayerData.add_daily_num(id,num,check)
	local t_daily = Config.get_t_daily(id)
	if t_daily == nil then
		return
	end
	
	if check and t_daily.dtype ~= 2 then
		return
	end
	
	for i = 1,#self.player.daily_reward do
		if self.player.daily_reward[i] == id then
			return
		end
	end
	
	for i = 1,#self.player.daily_id do
		if self.player.daily_id[i] == id then
			num = self.player.daily_num[i] + num
			if num > t_daily.target_num then
				num = t_daily.target_num
			end
			self.player.daily_num[i] = num
			return
		end
	end
	
	if num > t_daily.target_num then
		num = t_daily.target_num
	end
	self.player.daily_id:append(id)
	self.player.daily_num:append(num)
end

function PlayerData.get_daily_num(id)
	for i = 1,#self.player.daily_id do
		if self.player.daily_id[i] == id then
			return self.player.daily_num[i]
		end
	end
	return 0
end

function PlayerData.del_daily_num(id)
	for i = 1,#self.player.daily_id do
		if self.player.daily_id[i] == id then
			self.player.daily_id[i] = self.player.daily_id[#self.player.daily_id]
			self.player.daily_num[i] = self.player.daily_num[#self.player.daily_num]
			table.remove(self.player.daily_id,#self.player.daily_id)
			table.remove(self.player.daily_num,#self.player.daily_num)
			return
		end
	end
end

function PlayerData.get_name_color()
	if tonumber(self.player.nian_time) > tonumber(timerMgr:now_string()) then
		return 2
	elseif tonumber(self.player.yue_time) > tonumber(timerMgr:now_string()) then
		return 1
	end
	return 0
end

function PlayerData.load_server(www)
	if(load_sever_code == 1) then
		GUIRoot.HideGUI("MaskPanel")
	end
	if(www.error ~= nil) then
		GUIRoot.HideGUI("MaskPanel")
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('PlayerData_002'), Config.get_t_script_str('PlayerData_003'), State.Reset})
		return 0
	end
	local data = cjson_safe.decode(www.text)
	if(data == nil) then
		log("server error")
		return 1
	end
	local server_inf = nil
	if(data[platform_config_common.version] ~= nil) then
		PlayerData.is_ios_sh = true
		server_inf = data[platform_config_common.version]
	else
		PlayerData.is_ios_sh = false
		server_inf = data["default"]
	end

	PlayerData.review = tonumber(server_inf["review"])
	PlayerData.login_url = server_inf["account"]
	PlayerData.host = server_inf["host"]
	PlayerData.port = tonumber(server_inf["port"])

	PlayerData.wh = tonumber(server_inf["wh"])
	PlayerData.whtext = server_inf["whtext"]
	if(server_inf["whitelist"] ~= nil) then
		PlayerData.whitelist = server_inf["whitelist"]
	end
	if(load_sever_code == 1) then
		StartPanel.InitPanel()
	elseif(load_sever_code == 2) then
		PlayerData.Init()
	end
end

function PlayerData.load_share(www)
	local data = cjson_safe.decode(www.text)
	if(data == nil) then
		log("share error")
		return 1
	end
	load_share_state = 1
	PlayerData.home_url = data["home_url"]
	PlayerData.activity_url = data["activity_url"]
	PlayerData.help_url = data["help_url"]
	PlayerData.bbs_url = data["bbs_url"]
	PlayerData.icon_url = data["icon_url"]
	local url = data["share_url"]
	local url_ran = url[math.random(1, #url)]
	local share_id = shareMgr:GetShareID(self.guid)
	PlayerData.share_url = url_ran..share_id
	PlayerData.share_code = shareMgr:ShareZcode(PlayerData.share_url, 512, 512)
end

function PlayerData.load_gongao(www)
	local data = cjson_safe.decode(www.text)
	if(data == nil) then
		log("gonggao error")
		return 1
	end
	load_gonggao_state = 1
	if(PlayerPrefs.HasKey("MaxGongGao")) then
		PlayerData.max_gonggao_id = tonumber(PlayerPrefs.GetString("MaxGongGao"))
	end
	for i = 1, #data do
		table.insert(PlayerData.gonggao, data[i])
	end
	PlayerData.gonggao = PlayerData.rank_gonggao(PlayerData.gonggao)
end

function PlayerData.rank_gonggao(gonggao_tab)
	for i = 1, #gonggao_tab do
		for j = 1, #gonggao_tab - i do
			if(tonumber(gonggao_tab[j]["id"]) < tonumber(gonggao_tab[j + 1]["id"])) then
				local gonggao_temp = gonggao_tab[j + 1]
				gonggao_tab[j + 1] = gonggao_tab[j]
				gonggao_tab[j] = gonggao_temp
			end
		end
	end
	return gonggao_tab
end

function PlayerData.load_share_text(www)
	local data = cjson_safe.decode(www.text)
	if(data == nil or www.error ~= nil) then
		log("share text error")
		GUIRoot.HideGUI("MaskPanel")
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('PlayerData_002'), Config.get_t_script_str('PlayerData_003'), State.Reset})
		return 1
	end
	PlayerData.share_text = data
	if(load_gonggao_state == 0 or load_share_state == 0) then
		GUIRoot.HideGUI("MaskPanel")
		GUIRoot.ShowGUI("SelectPanel", {Config.get_t_script_str('PlayerData_002'), Config.get_t_script_str('PlayerData_003'), State.Reset})
		return 0
	end
	LoginPanel.Init()
end

function PlayerData.is_whitelist(user_name)
	for i = 1, #PlayerData.whitelist do
		if(PlayerData.whitelist[i] == user_name) then
			return true
		end
	end
	return false
end

function PlayerData.get_gonggao(id)
	for i = 1, #PlayerData.gonggao do
		if(PlayerData.gonggao[i]["id"] == id) then
			return PlayerData.gonggao[i]
		end
	end
	return nil
end

function PlayerData.read_gonggao(id)
	local pos = 0
	for i = 1, #PlayerData.gonggao do
		if(PlayerData.gonggao[i]["id"] == id and pos == 0) then		
			pos = i
		end
	end
	if(pos ~= 0) then
		if(tonumber(id) > PlayerData.max_gonggao_id) then
			PlayerData.max_gonggao_id = tonumber(id)
			PlayerPrefs.SetString("MaxGongGao", tostring(PlayerData.max_gonggao_id))
			PlayerPrefs.Save()
		end
	end
end

function PlayerData.load_weburl(url, luafunc)
	toolMgr:load_url(url, luafunc)
end

function PlayerData.send_http(url, wwwform, back_func, text, time_)
	if(text == nil) then
		text = ""
	end
	if(time_ == nil) then
		time_ = 10
	end
	load_http_url = url
	GUIRoot.ShowGUI("MaskPanel", {text})
	toolMgr:load_http(url, wwwform, back_func, PlayerData.fail_http)
	timerMgr:AddTimer("PlayerData", PlayerData.over_time_http, time_)
end

function PlayerData.over_time_http()
	if(toolMgr:unload_http(load_http_url)) then
		GUIRoot.HideGUI("MaskPanel")
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('PlayerData_004')})
	end
end

function PlayerData.fail_http()
	GUIRoot.HideGUI("MaskPanel")
	GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('PlayerData_005')})
end

function PlayerData.load_set()
	if(PlayerPrefs.HasKey('mus_volum')) then
		PlayerData.mus_volum = tonumber(PlayerPrefs.GetString('mus_volum'))
	end
	if(PlayerPrefs.HasKey('quality')) then
		PlayerData.quality = tonumber(PlayerPrefs.GetString('quality'))
	end
	
	if(PlayerPrefs.HasKey('eff_quality')) then
		PlayerData.eff_quality = tonumber(PlayerPrefs.GetString('eff_quality'))
	end
	
	if(PlayerPrefs.HasKey('eff_volum')) then
		PlayerData.eff_volum = tonumber(PlayerPrefs.GetString('eff_volum'))
	end
	if(PlayerPrefs.HasKey(self.player.guid..'_total_unread_num')) then
		PlayerData.unread_msg_num = tonumber(PlayerPrefs.GetString(self.player.guid..'_total_unread_num'))
	end
	if(PlayerData.mus_volum == 0) then
		soundMgr.Is_play_mus = false
	end
	if(PlayerData.eff_volum == 0) then
		soundMgr.Is_play_eff = false
	end
	QualitySettings.SetQualityLevel(PlayerData.quality, true)
end

function PlayerData.save_set()
	PlayerPrefs.SetString('mus_volum', PlayerData.mus_volum)
	PlayerPrefs.SetString('eff_volum', PlayerData.eff_volum)
	PlayerPrefs.SetString('quality', PlayerData.quality)
	PlayerPrefs.SetString('eff_quality', PlayerData.eff_quality)
	PlayerPrefs.Save()
end

function PlayerData.ClearData()
	PlayerData.roles = {}
	PlayerData.battle_results = {}
	PlayerData.chest_rewards = {}
	FriendPanel.ClearUnreadList()
	load_gonggao_state = 0
	load_share_state = 0
end

function PlayerData.day_refresh()
	if self.player == nil then
		return
	end
	self.player.box_zd_num = 0
	self.player.box_zd_opened = 0
	self.player.fenxiang_state = 0
	self.player.fenxiang_num = 0
	self.player.sign_finish = 0
	if(self.player.sign_index >= 6)	then
		self.player.sign_index = 0
	else
		self.player.sign_index = self.player.sign_index + 1
	end
	self.player.daily_id:clear()
	self.player.daily_num:clear()
	self.player.daily_reward:clear()
	self.player.daily_point = 0
	self.player.daily_get_id:clear()
	self.player.social_golds:clear()
	self.player.yue_reward = 0
	self.player.nian_reward = 0
	self.player.duobao_num = 0
end

function PlayerData.week_refresh()
	if self.player == nil then
		return
	end
	local cup_temp = Config.get_t_cup(self.player.cup)
	PlayerData.set_cup(cup_temp.down)
	self.player.battle_gold = 0
end

function PlayerData.month_refresh()
	if self.player == nil then
		return
	end
end

function PlayerData.adjust_data(player, roles)
	PlayerData.adjust_single_data(player, self.player, "name")
	PlayerData.adjust_single_data(player, self.player, "gold")
	PlayerData.adjust_single_data(player, self.player, "jewel")
	PlayerData.adjust_single_data(player, self.player, "level")
	PlayerData.adjust_single_data(player, self.player, "exp")
	PlayerData.adjust_single_data(player, self.player, "is_guide")
	PlayerData.adjust_single_data(player, self.player, "cup")
	PlayerData.adjust_single_data(player, self.player, "snow")
	PlayerData.adjust_single_data(player, self.player, "max_cup")
	PlayerData.adjust_single_data(player, self.player, "battle_gold")
	PlayerData.adjust_single_data(player, self.player, "box_zd_num")
	PlayerData.adjust_single_data(player, self.player, "box_zd_opened")
	PlayerData.adjust_repeated_data(player, self.player, "box_ids")
	PlayerData.adjust_single_data(player, self.player, "box_open_slot")
	PlayerData.adjust_single_data(player, self.player, "sign_index")
	PlayerData.adjust_single_data(player, self.player, "sign_finish")
	PlayerData.adjust_single_data(player, self.player, "role_on")
	PlayerData.adjust_repeated_data(player, self.player, "item_id")
	PlayerData.adjust_repeated_data(player, self.player, "item_num")
	PlayerData.adjust_repeated_data(player, self.player, "avatar")
	PlayerData.adjust_single_data(player, self.player, "avatar_on")
	PlayerData.adjust_repeated_data(player, self.player, "achieve_id")
	PlayerData.adjust_repeated_data(player, self.player, "achieve_num")
	PlayerData.adjust_repeated_data(player, self.player, "achieve_reward")
	PlayerData.adjust_single_data(player, self.player, "achieve_point")
	PlayerData.adjust_single_data(player, self.player, "achieve_index")
	PlayerData.adjust_repeated_data(player, self.player, "toukuang")
	PlayerData.adjust_single_data(player, self.player, "toukuang_on")
	PlayerData.adjust_repeated_data(player, self.player, "task_id")
	PlayerData.adjust_repeated_data(player, self.player, "task_num")
	PlayerData.adjust_repeated_data(player, self.player, "task_reward")
	PlayerData.adjust_repeated_data(player, self.player, "fashion_id")
	PlayerData.adjust_repeated_data(player, self.player, "fashion_on")
	PlayerData.adjust_repeated_data(player, self.player, "daily_id")
	PlayerData.adjust_repeated_data(player, self.player, "daily_num")
	PlayerData.adjust_repeated_data(player, self.player, "daily_reward")
	PlayerData.adjust_single_data(player, self.player, "daily_point")
	PlayerData.adjust_repeated_data(player, self.player, "daily_get_id")
	PlayerData.adjust_repeated_data(player, self.player, "level_reward")
	PlayerData.adjust_single_data(player, self.player, "change_name_num")
	PlayerData.adjust_single_data(player, self.player, "fenxiang_num")
	PlayerData.adjust_single_data(player, self.player, "fenxiang_total_num")
	PlayerData.adjust_single_data(player, self.player, "fenxiang_state")
	PlayerData.adjust_single_data(player, self.player, "battle_num")
	if(#roles ~= #self.roles) then
		log("role_num diff".." server:"..#roles.." client"..#self.roles)
	end
	for i = 1, #roles do
		local role = self.get_role(roles[i].guid)
		if(role == nil) then
			log("role diff".." server:"..roles[i].guid.." client:null")
		else
			PlayerData.adjust_single_data(roles[i], role, "template_id")
			PlayerData.adjust_single_data(roles[i], role, "level")
		end
	end
end

function PlayerData.adjust_single_data(data1, data2, name)
	if(data1[name] ~= data2[name]) then
		log(name.." diff server:"..data1[name].." client"..data2[name])
	end
end

function PlayerData.adjust_repeated_data(data1, data2, name)
	if #data1[name] ~= #data2[name] then
		log(name.." diff server:"..#data1[name].." client"..#data2[name])
		return
	end
	for i = 1, #data1[name] do
		if data1[name][i] ~= data2[name][i] then
			log(name.." diff index:"..tostring(i).." server:"..data1[name][i].." client:"..data2[name][i])
		end
	end
end
