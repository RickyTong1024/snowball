ChestPanel = {}

local lua_script_
local open_panel_
local reward_panel_

local chest_panel_
local chest_panel_rare_
local chest_icon_
local chest_bg_
local chest_name_
local open_btn_
local lock_btn_
local unlock_btn_
local buy_btn_
local lock_tip_
local chest_slot_

local chest_id_ = 0
local open_mode_ = 0

local chest_reward_res_

local chest_silver_
local chest_gold_
local chest_epic_
local chest_leg_

local reward_pos_ = 0
local chest_rewards_ = {}
local chest_prefabs_ = {}

local effect_num_end_ = 0
local effect_num_start_ = 0
local effect_num_max_ = 0
local shake_time_ = 0
local lock_time = 0
local is_lock = false
local is_first = true

local reward_view_

local particle_color_ = {'lg', 'zg', 'jg'}
local light_color_ = {'light-blue', 'light-purple', 'light-yellow'}

function ChestPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	open_panel_ = obj.transform:Find('open_panel')
	reward_panel_ = obj.transform:Find('reward_panel')
	
	GUIRoot.UIEffectScalePos(open_panel_.gameObject, true, 1)
	
	chest_panel_ = open_panel_:Find('chest_panel')
	chest_panel_rare_ = open_panel_:Find('chest_panel_rare')
	
	reward_view_ = reward_panel_:Find('reward_view')
	
	reward_view_:GetComponent("UIPanel"):SetRect(0, 0, 420, GUIRoot.height)
	
	chest_reward_res_ = reward_panel_:Find('chest_reward_res')
	
	for i = 1, 8 do
		local chest_t = reward_panel_:Find("chest_"..i)
		if(chest_t ~= nil) then
			table.insert(chest_prefabs_, chest_t)
		end
	end
	
	chest_icon_ = open_panel_:Find('chest_icon'):GetComponent('UISprite')
	chest_bg_ = open_panel_:Find('baseboard')
	chest_name_ = open_panel_:Find('chest_icon/chest_name'):GetComponent('UILabel')
	
	open_btn_ = open_panel_:Find('open_btn')
	lock_btn_ = open_panel_:Find('lock_btn')
	unlock_btn_ = open_panel_:Find('unlock_btn')
	buy_btn_ = open_panel_:Find('buy_btn')
	lock_tip_ = open_panel_:Find('inf_bg/lock_tip'):GetComponent("UILabel")
	
	local price_icon = unlock_btn_:Find('icon'):GetComponent('UISprite')
	price_icon.spriteName = Config.get_t_resource(2).small_icon
	price_icon:MakePixelPerfect()
	
	gain_btn = obj.transform:Find('bg')
	
	local close_btn = open_panel_:Find('close_btn')
	
	lua_script_:AddButtonEvent(open_btn_.gameObject, "click", ChestPanel.Click)
	lua_script_:AddButtonEvent(lock_btn_.gameObject, "click", ChestPanel.Click)
	lua_script_:AddButtonEvent(unlock_btn_.gameObject, "click", ChestPanel.Click)
	lua_script_:AddButtonEvent(buy_btn_.gameObject, "click", ChestPanel.Click)
	lua_script_:AddButtonEvent(gain_btn.gameObject, "click", ChestPanel.ShowReward)
	lua_script_:AddButtonEvent(close_btn.gameObject, "click", ChestPanel.Click)
	ChestPanel.RegisterMessage()
	
	open_panel_.gameObject:SetActive(false)
	reward_panel_.gameObject:SetActive(false)
	buy_btn_.gameObject:SetActive(false)
	open_btn_.gameObject:SetActive(false)
	lock_btn_.gameObject:SetActive(false)
	unlock_btn_.gameObject:SetActive(false)
	lock_tip_.gameObject:SetActive(false)
end

function ChestPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_START_OPEN_BOX, ChestPanel.SMSG_START_OPEN_BOX)
	Message.register_net_handle(opcodes.SMSG_END_OPEN_BOX, ChestPanel.SMSG_END_OPEN_BOX)
	Message.register_net_handle(opcodes.SMSG_OPEN_BATTLE_BOX, ChestPanel.SMSG_OPEN_BATTLE_BOX)
	Message.register_net_handle(opcodes.SMSG_ITEM_BUY_BOX, ChestPanel.SMSG_ITEM_BUY_BOX)
	Message.register_net_handle(opcodes.SMSG_OPEN_FENXIANG_BOX, ChestPanel.SMSG_OPEN_FENXIANG_BOX)
	Message.register_handle("team_join_msg", ChestPanel.TeamJoin)
end

function ChestPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_START_OPEN_BOX, ChestPanel.SMSG_START_OPEN_BOX)
	Message.remove_net_handle(opcodes.SMSG_END_OPEN_BOX, ChestPanel.SMSG_END_OPEN_BOX)
	Message.remove_net_handle(opcodes.SMSG_ITEM_BUY_BOX, ChestPanel.SMSG_ITEM_BUY_BOX)
	Message.remove_net_handle(opcodes.SMSG_OPEN_FENXIANG_BOX, ChestPanel.SMSG_OPEN_FENXIANG_BOX)
	Message.remove_handle("team_join_msg", ChestPanel.TeamJoin)
end

function ChestPanel.OnDestroy()
	chest_slot_ = 0
	reward_pos_ = 0
	chest_id_ = 0
	effect_num_start_ = 0
	effect_num_max_ = 0
	effect_num_end_ = 0
	shake_time_ = 0
	lock_time = 0
	is_lock = false
	chest_prefabs_ = {}
	chest_rewards_ = {}
	ChestPanel.RemoveMessage()
	timerMgr:RemoveRepeatTimer('ChestPanel')
	timerMgr:RemoveRepeatTimer('Effect_num')
end

function ChestPanel.OnParam(parm)
	open_mode_ = parm[1]
	chest_slot_ = parm[2]
	if(open_mode_ == 0) then
		timerMgr:AddRepeatTimer('ChestPanel', ChestPanel.RefreshChest, 1, 1)
		ChestPanel.RefreshChest()
	elseif(open_mode_ == 1) then
		local shop_temp = Config.get_t_shop(chest_slot_)
		local chest_temp = Config.get_t_chest(shop_temp.value1)
		ChestPanel.PayOpen(chest_temp, shop_temp.past_type, shop_temp.price)
	elseif(open_mode_ == 2) then
		local chest_temp = Config.get_t_chest(chest_slot_)
		ChestPanel.BattleOpen(chest_temp)
	elseif(open_mode_ == 3) then
		local chest_temp = Config.get_t_chest(chest_slot_)
		ChestPanel.ShareOpen(chest_temp)
	end
	open_panel_.gameObject:SetActive(true)
end

function ChestPanel.TeamJoin()
	GUIRoot.HideGUI('ChestPanel')
end

-------------------------开宝箱-----------------------------

function ChestPanel.ShowReward()
	if(is_lock) then
		return 0
	end
	if(reward_panel_:Find('reward') ~= nil) then
		local anim = reward_panel_:Find('reward'):GetComponent('Animator')
		if(anim:GetCurrentAnimatorStateInfo(0).normalizedTime <= 1) then
			anim.speed = 10
			return 0
		else 
			GameObject.DestroyImmediate(reward_panel_:Find('reward').gameObject)
		end
	end
	if(reward_pos_ < #chest_rewards_) then
		local anim_chest = chest_prefabs_[chest_id_]:GetComponent('Animator')
		if(reward_pos_ == 0) then
			anim_chest:Play('chest_silver_open')
		elseif(reward_pos_ < #chest_rewards_ - 1) then
			anim_chest:Play('chest_silver_opening')
			anim_chest:Play('chest_silver_copen')
		else
			anim_chest:Play('chest_silver_lopen')
		end
		reward_pos_ = reward_pos_ + 1
		chest_prefabs_[chest_id_]:Find('num/Label'):GetComponent('UILabel').text = #chest_rewards_ - reward_pos_
		local reward = chest_rewards_[reward_pos_]
		local reward_temp = Config.get_t_reward(reward.type, reward.value1)
		local type_text = ""
		if(reward.type == 1) then
			type_text = Config.get_t_script_str('ChestPanel_001')--'资源'
		elseif(reward.type == 2) then
			if(reward_temp.type == 1001) then
				type_text = Config.get_t_script_str('ChestPanel_002') --"角色"
			else
				type_text = Config.get_t_script_str('ChestPanel_003') --'道具'
			end
		end
		local reward_t = LuaHelper.Instantiate(chest_reward_res_.gameObject)
		reward_t.name = 'reward'
		reward_t.transform.parent = reward_panel_
		reward_t.transform.localPosition = Vector3(170, 0, 0)
		reward_t.transform.localScale = Vector3.one
		local reward_icon = IconPanel.GetIcon("reward_res", nil, reward_temp.icon, reward_temp.color, 1)
		reward_icon.transform.parent = reward_t.transform
		reward_icon.transform.localPosition = Vector3(0, 0, 0)
		reward_icon.transform.localScale = Vector3.one
		reward_icon.transform:Find("icon").name = reward.type.."+"..reward_temp.id
		reward_icon:SetActive(true)
		if reward_temp.color == 1 then
			lua_script_:PlaySound("get_chest2")
		elseif reward_temp.color == 2 then
			lua_script_:PlaySound("get_chest3")
		else
			lua_script_:PlaySound("get_chest4")
		end
		local name = reward_t.transform:Find('name'):GetComponent('UILabel')
		local num = reward_t.transform:Find('num'):GetComponent('UILabel')
		local type_ = reward_t.transform:Find('type'):GetComponent('UILabel')
		local light = reward_t.transform:Find('light/effect/effect'):GetComponent('UISprite')
		local flash = reward_t.transform:Find('star'):GetComponent('Particle2D')
		type_.text = type_text
		name.text = reward_temp.name
		IconPanel.InitQualityLabel(name, reward_temp.color % 10)
		num.text = reward.value2
		if(reward.value2 == 1) then
			num.text = ""
		end
		local chest_particle = chest_prefabs_[chest_id_]:Find('star'):GetComponent('Particle2D')
		chest_particle.sprites:RemoveAt(0)
		chest_particle.sprites:Add(particle_color_[reward_temp.color % 10])
		light.spriteName = light_color_[reward_temp.color % 10]
		flash.sprites:RemoveAt(0)
		flash.sprites:Add(particle_color_[reward_temp.color % 10])
		reward_t:SetActive(true)
		ChestPanel.AddNum(reward_t)
	elseif(reward_pos_ >= #chest_rewards_ and #chest_rewards_ > 0) then
		if(reward_pos_ == #chest_rewards_) then
			if(reward_panel_:Find('reward') ~= nil) then
				GameObject.Destroy(reward_panel_:Find('reward').gameObject)
			end
			if(#chest_prefabs_ > 0) then
				chest_prefabs_[chest_id_]:GetComponent('Animator'):Play('chest_silver_opend')
			end
			ChestPanel.RankReward()
			reward_pos_ = reward_pos_ + 1
		elseif(reward_pos_ > #chest_rewards_ and not is_lock) then
			GUIRoot.HideGUI("ChestPanel")
		end
	end
end


function ChestPanel.AddNum(reward_t)
	if(reward_panel_:Find('reward') ~= nil) then
		local reward = chest_rewards_[reward_pos_]
		local tip = reward_t.transform:Find('tip'):GetComponent('UILabel')
		local num_slider = reward_t.transform:Find('num_slider')
		local num_label = reward_t.transform:Find('num_slider/Label'):GetComponent('UILabel')
		local icon = reward_t.transform:Find('num_slider/icon'):GetComponent('UISprite')
		tip.text = ' '
		if(reward.type == 1) then
			icon.gameObject:SetActive(true)
			num_slider:GetComponent('UISlider').value = 0
			num_slider:Find('arr').gameObject:SetActive(false)
			effect_num_max_ = 0
			local res_temp = Config.get_t_resource(reward.value1)
			icon.spriteName = res_temp.mid_icon
			if(reward.value1 == 1) then
				effect_num_start_ = self.player.gold - reward.value2
				effect_num_end_ = self.player.gold
			elseif(reward.value1 == 2) then
				effect_num_start_ = self.player.jewel - reward.value2
				effect_num_end_ = self.player.jewel
			end
			num_label.text = effect_num_start_
		elseif reward.type == 2 then
			local item_temp = Config.get_t_item(reward.value1)
			local item_num = self.get_item_num(reward.value1)
			effect_num_start_ = item_num - reward.value2
			effect_num_end_ = item_num
			if item_temp.type == 1001 then
				local role = Config.get_t_role(item_temp.def1)
				local role_t = self.get_role_id(role.id)
				if role_t == nil then
					if(role.get_type == 0) then
						tip.text = Config.get_t_script_str('ChestPanel_004')--'碎片合成'
						effect_num_max_ = role.suipian_cost
						num_slider:GetComponent('UISlider').value = effect_num_start_ / role.suipian_cost
						num_label.text = effect_num_start_.."/"..effect_num_max_
						if item_num >= role.suipian_cost then
							tip.text = Config.get_t_script_str('ChestPanel_005')--'可以合成'
							num_slider:Find('arr'):GetComponent('UISprite').spriteName = 'jiantou-g'
							num_slider:Find('fg_y'):GetComponent('UISprite').spriteName = 'jdt-30-green'
						end
					else
						if(role.get_type == 1) then
							tip.text = Config.get_t_script_str('ChestPanel_006')--'等级解锁'
						elseif(role.get_type == 2) then
							tip.text = Config.get_t_script_str('ChestPanel_007')--'商城购买'
						end
						num_label.text = effect_num_start_
						num_slider:GetComponent('UISlider').value = 1
						effect_num_max_ = 0
					end
				else
					local role_level = Config.get_t_role_level(role_t.level + 1)
					if(role_level ~= nil) then
						num_slider:GetComponent('UISlider').value = effect_num_start_ / role_level.suipian_cost
						effect_num_max_ = role_level.suipian_cost
						num_label.text = effect_num_start_.."/"..effect_num_max_
						if(item_num >= role_level.suipian_cost) then
							tip.text = Config.get_t_script_str('ChestPanel_008')--'可以升星'
							num_slider:Find('arr'):GetComponent('UISprite').spriteName = 'jiantou-g'
							num_slider:Find('fg_y'):GetComponent('UISprite').spriteName = 'jdt-30-green'
						end
					else
						tip.text = Config.get_t_script_str('ChestPanel_009') --'已满星'
						num_label.text = effect_num_start_
						num_slider:GetComponent('UISlider').value = 1
						effect_num_max_ = 0
					end
				end
			else
				if(item_num == 0) then
					tip.text = Config.get_t_script_str('ChestPanel_010') --'新的道具'
				end
				num_slider:GetComponent('UISlider').value = 0
				num_label.text = effect_num_start_
				num_slider:Find('arr').gameObject:SetActive(false)
				effect_num_max_ = 0
			end
		end
		shake_time_ = 0
		is_first = true
	end
end


function ChestPanel.Effect_num()
	if(reward_panel_:Find('reward') ~= nil) then
		local reward_t = reward_panel_:Find('reward')
		local num_slider = reward_t:Find('num_slider')
		local num_label = num_slider:Find('Label'):GetComponent('UILabel')
		if(reward_t:GetComponent('Animator'):GetCurrentAnimatorStateInfo(0).normalizedTime > 1) then
			if(not is_lock and is_first) then
				is_lock = true
				is_first = false
				lock_time = 0
			end
			if(is_lock) then
				lock_time = lock_time + 1
				if(lock_time >= 3) then
					is_lock = false
				end
			end
			if(effect_num_start_ < effect_num_end_) then
				shake_time_ = shake_time_ + 1
				if(shake_time_ < 3) then
					return 0
				end
				num_slider:GetComponent('Animator'):Play('slider_shake')
				local d_value = effect_num_end_ - effect_num_start_
				if d_value >= 1000 then
					effect_num_start_ = effect_num_start_ + 200
				elseif d_value >= 100 then
					effect_num_start_ = effect_num_start_ + 50
				elseif d_value >= 10 then
					effect_num_start_ = effect_num_start_ + 5
				else
					effect_num_start_ = effect_num_start_ + 1
				end
				if(effect_num_max_ == 0) then
					num_label.text = effect_num_start_
				elseif(effect_num_max_ > 0) then
					num_slider:GetComponent('UISlider').value = effect_num_start_ / effect_num_max_
					num_label.text = tostring(effect_num_start_)..'/'..tostring(effect_num_max_)
				else
				end
			else
				if(effect_num_end_ >= effect_num_max_ and effect_num_max_ > 0) then
					reward_t:GetComponent('Animator').speed = 1
					reward_t:GetComponent('Animator'):Play('reward_up')
				end
				if(effect_num_end_ < effect_num_max_ and effect_num_max_ > 0 or effect_num_max_ == 0) then
					reward_t:GetComponent('Animator').speed = 1
					reward_t:GetComponent('Animator'):Play('reward_normal')
				end
			end
		end
	end
	if(reward_pos_ > #chest_rewards_ and is_lock) then
		lock_time = lock_time + 1
		if(lock_time >= 30) then
			is_lock = false
		end
	end
end


function ChestPanel.RankReward()
	local line = math.floor(#chest_rewards_ / 4)
	if(#chest_rewards_ % 4 > 0) then
		line = line + 1
	end
	refer_pos_y = (line - 1) / 2 * 100
	for i = 1, #chest_rewards_ do
		local reward = chest_rewards_[i]
		local reward_temp = Config.get_t_reward(reward.type, reward.value1)
		local reward_t = IconPanel.GetIcon("reward_res", nil, reward_temp.icon, reward_temp.color, reward.value2)
		reward_t.transform.parent = reward_view_
		reward_t.transform.localPosition = Vector3((i - 1) % 4 * 100 + -150, -(math.floor((i - 1) / 4) * 100) + refer_pos_y, 0)
		reward_t.transform.localScale = Vector3.one
		reward_t.transform:Find("icon").name = reward.type.."+"..reward_temp.id
		local from = reward_t.transform.localPosition + Vector3(-10, 0, 0)
		twnMgr:Add_Tween_Postion(reward_t, 0.4, from, reward_t.transform.localPosition)
		twnMgr:Add_Tween_Alpha(reward_t, 0.4, 0, 1)
		reward_t:SetActive(true)
	end
	is_lock = true
	lock_time = 0
	lua_script_:PlaySound("show_item")
end
-------------------------------------------------------


-----------------------------刷新界面------------------------

function ChestPanel.InitOpenPanel(chest_temp, panel)
	chest_panel_.gameObject:SetActive(false)
	chest_panel_rare_.gameObject:SetActive(false)
	open_btn_.gameObject:SetActive(false)
	lock_btn_.gameObject:SetActive(false)
	unlock_btn_.gameObject:SetActive(false)
	local gold_num = panel:Find('gold_num/Label'):GetComponent('UILabel')
	local card_num = panel:Find('card_num/Label'):GetComponent('UILabel')
	gold_num.text = tostring(chest_temp.get_gold_min())..'-'..tostring(chest_temp.get_gold_max())
	card_num.text = "x"..tostring(chest_temp.item_num)
	chest_icon_.atlas = IconPanel.GetAltas(chest_temp.icon)
	chest_icon_.spriteName = chest_temp.icon
	chest_name_.text = chest_temp.name
	ChestShowPanel.InitNameLabel(chest_temp.id, chest_name_)
	if(chest_temp.treasure_num > 0) then
		local least_label = panel:Find('least_gain/Label'):GetComponent('UILabel')
		least_label.text = 'x'..tostring(chest_temp.treasure_num)
	end
end

function ChestPanel.RefreshChest()
	chest_panel_.gameObject:SetActive(false)
	chest_panel_rare_.gameObject:SetActive(false)
	local chest_id = self.player.box_ids[chest_slot_]
	local chest_temp = Config.get_t_chest(chest_id)
	chest_icon_.spriteName = chest_temp.icon
	chest_name_.text = chest_temp.name
	local panel = nil
	if(chest_temp.treasure_num > 0) then
		panel = chest_panel_rare_
	else
		panel = chest_panel_
	end
	if(self.player.box_open_slot == 0 or self.player.box_open_slot ~= chest_slot_) then
		ChestPanel.CanOpen(chest_temp)
	else
		if(tonumber(timerMgr:now_string()) >= tonumber(self.player.box_open_time)) then
			ChestPanel.ModifyPanel(panel, 1)
			ChestPanel.CanOpen(chest_temp)
			open_btn_.gameObject:SetActive(true)
			lock_btn_.gameObject:SetActive(false)
			unlock_btn_.gameObject:SetActive(false)
		else
			ChestPanel.InitOpenPanel(chest_temp, panel)
			ChestPanel.ModifyPanel(panel)
			local time_label = unlock_btn_:Find('bg/time'):GetComponent('UILabel')
			local open_cost_label = unlock_btn_:Find('icon/cost'):GetComponent('UILabel')
			local time_ = tonumber(self.player.box_open_time) - tonumber(timerMgr:now_string())
			local jewel = math.floor((time_ + 179999) / 180000)
			if(self.player.jewel >= jewel) then
				open_cost_label.text = self.font_color[2]..jewel
			else
				open_cost_label.text = self.font_color[4]..jewel
			end
			time_label.text = count_time(time_)
			unlock_btn_.gameObject:SetActive(true)
			open_btn_.gameObject:SetActive(false)
			lock_btn_.gameObject:SetActive(false)
		end
	end
	panel.gameObject:SetActive(true)
end

function ChestPanel.CanOpen(chest_temp)
	local panel = nil
	if(chest_temp.treasure_num > 0) then
		panel = chest_panel_rare_
	else
		panel = chest_panel_
	end
	ChestPanel.InitOpenPanel(chest_temp, panel)
	if(self.player.box_open_slot == 0) then
		ChestPanel.ModifyPanel(panel, 1)
		local lock_time = lock_btn_:Find("lock/time"):GetComponent("UILabel")
		lock_time.text = count_time(chest_temp.get_time() * 1000)
		lock_btn_.gameObject:SetActive(true)
	else
		timerMgr:RemoveRepeatTimer('ChestPanel')
		if(self.player.box_open_slot ~= chest_slot_) then
			ChestPanel.ModifyPanel(panel, 2)
			lock_tip_.text = Config.get_t_script_str('ChestPanel_011') --"另一个宝箱正在解锁"
			lock_tip_.gameObject:SetActive(true)
		end
	end
	panel.gameObject:SetActive(true)
end

function ChestPanel.PayOpen(chest_temp, past_type, cost)
	local panel = nil
	if(chest_temp.treasure_num > 0) then
		panel = chest_panel_rare_
	else
		panel = chest_panel_
	end
	ChestPanel.ModifyPanel(panel, 1)
	local cost_label = buy_btn_:Find("icon/cost"):GetComponent('UILabel')
	local price_icon = buy_btn_:Find("icon"):GetComponent("UISprite")
	local res_temp = Config.get_t_resource(past_type)
	price_icon.spriteName = res_temp.small_icon
	ChestPanel.InitOpenPanel(chest_temp, panel)
	local res_temp = Config.get_t_resource(past_type)
	if(self.get_resource(past_type) >= cost) then
		cost_label.text = self.font_color[res_temp.color]..cost
	else
		cost_label.text = self.font_color[4]..cost
	end
	buy_btn_.gameObject:SetActive(true)
	panel.gameObject:SetActive(true)
end

function ChestPanel.BattleOpen(chest_temp)
	local panel = nil
	if(chest_temp.treasure_num > 0) then
		panel = chest_panel_rare_
	else
		panel = chest_panel_
	end
	ChestPanel.InitOpenPanel(chest_temp, panel)
	if(self.player.box_zd_num >= chest_temp.time) then
		open_btn_.gameObject:SetActive(true)
		ChestPanel.ModifyPanel(panel, 1)
	else
		ChestPanel.ModifyPanel(panel, 2)
		lock_tip_.text = Config.get_t_script_str('ChestPanel_012')--"未完成足够的战斗场数"
		lock_tip_.gameObject:SetActive(true)
	end
	panel.gameObject:SetActive(true)
end

function ChestPanel.ShareOpen(chest_temp)
	local panel = nil
	if(chest_temp.treasure_num > 0) then
		panel = chest_panel_rare_
	else
		panel = chest_panel_
	end
	ChestPanel.InitOpenPanel(chest_temp, panel)
	if(self.player.fenxiang_state == 1) then
		open_btn_.gameObject:SetActive(true)
		ChestPanel.ModifyPanel(panel, 1)
	else
		ChestPanel.ModifyPanel(panel, 2)
		lock_tip_.text = Config.get_t_script_str('ChestPanel_013') --"完成一次分享操作即可领取哦,快去召唤小伙伴吧"
		lock_tip_.gameObject:SetActive(true)
	end
	panel.gameObject:SetActive(true)
end

function ChestPanel.ModifyPanel(panel, state)
	local gold_num = panel:Find('gold_num')
	local card_num = panel:Find('card_num')
	if(state == 2) then
		chest_bg_:GetComponent('UIWidget').height = chest_bg_:GetComponent('UIWidget').height - 80
		open_panel_:Find("line").gameObject:SetActive(false)
		chest_bg_.localPosition = Vector3(0, chest_bg_.localPosition.y - 40, 0)
	end
	if(panel == chest_panel_rare_) then
		local least_label = chest_panel_rare_:Find('least_gain')
		local gold_pos = Vector3(-17, 72, 0)
		local card_pos = Vector3(172, 73, 0)
		local rare_pos = Vector3(24, 1, 0)
		if(state == 1) then
			gold_pos = gold_pos - Vector3(0, 22, 0)
			card_pos = card_pos - Vector3(0, 22, 0)
			rare_pos = rare_pos - Vector3(0, 38, 0)
		elseif(state == 2) then
			gold_pos = gold_pos - Vector3(0, 40, 0)
			card_pos = card_pos - Vector3(0, 40, 0)
			rare_pos = rare_pos - Vector3(0, 40, 0)
		end
		gold_num.localPosition = gold_pos
		card_num.localPosition = card_pos
		least_label.localPosition = rare_pos
	elseif(panel == chest_panel_) then
		local gold_pos = Vector3(-17, 35, 0)
		local card_pos = Vector3(172, 36, 0)
		if(state == 1) then
			gold_pos = gold_pos - Vector3(0, 26, 0)
			card_pos = card_pos - Vector3(0, 26, 0)
		elseif(state == 2) then
			gold_pos = gold_pos - Vector3(0, 40, 0)
			card_pos = card_pos - Vector3(0, 40, 0)
		end
		gold_num.localPosition = gold_pos
		card_num.localPosition = card_pos
	end
end
-------------------------------------------------------------


--------------------------ButtonEvent---------------------------

function ChestPanel.Click(obj)
	if(obj.name == 'open_btn') then
		if(open_mode_ == 0) then
			ChestPanel.OpenChest()
		elseif(open_mode_ == 2) then
			ChestPanel.BattleChest()
		elseif(open_mode_ == 3) then
			ChestPanel.ShareChest()
		end
	elseif(obj.name == 'lock_btn') then
		ChestPanel.OpenChest()
	elseif(obj.name == 'unlock_btn') then
		ChestPanel.OpenChest()
	elseif(obj.name == 'buy_btn') then
		ChestPanel.PayChest()
	elseif(obj.name == 'close_btn') then
		ChestPanel.ClosePanel()
	end
end

function ChestPanel.PayChest()
	local shop_temp = Config.get_t_shop(chest_slot_)
	local msg = msg_hall_pb.cmsg_item_buy()
	local res_temp = Config.get_t_resource(shop_temp.past_type)
	msg.id = chest_slot_
	msg.num = 1
	if(self.get_resource(shop_temp.past_type) >= shop_temp.price) then
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_ITEM_BUY, data, {opcodes.SMSG_ITEM_BUY_BOX})
	else
		local cancel_func = function() GUIRoot.ShowGUI("ChestPanel") end
		if(shop_temp.past_type == 2) then
			GUIRoot.HideGUI("ChestPanel", false)
			local content = string.format(Config.get_t_script_str('ChestPanel_014'),self.font_color[2],Config.get_t_resource(2).name);
			GUIRoot.ShowGUI("SelectPanel", {content,Config.get_t_script_str('ChestPanel_015'), ChestPanel.Recharge,Config.get_t_script_str('ChestPanel_016'), cancel_func})
		else
			GUIRoot.ShowGUI("MessagePanel", {self.font_color[res_temp.color]..res_temp.name.."[-] not enough"})
		end
	end
end

function ChestPanel.Recharge()
	if(open_mode_ == 0) then
		GUIRoot.HideGUI("ChestPanel")
		GUIRoot.HideGUI("ChestShowPanel")
		GUIRoot.ShowGUI("ShopPanel", {5})
	elseif(open_mode_ == 1) then
		GUIRoot.HideGUI("ChestPanel")
		ShopPanel.SelectPage(5)
	end
end

function ChestPanel.OpenChest()
	local msg = msg_hall_pb.cmsg_start_open_box()
	msg.slot = chest_slot_
	if(self.player.box_open_slot == 0) then
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_START_OPEN_BOX, data, {opcodes.SMSG_START_OPEN_BOX})
	else
		if(chest_slot_ == self.player.box_open_slot) then
			msg = msg_hall_pb.cmsg_end_open_box()
			msg.slot = chest_slot_
			if(tonumber(timerMgr:now_string()) >= tonumber(self.player.box_open_time)) then
				msg.is_jewel = 0
				local data = msg:SerializeToString()
				GameTcp.Send(opcodes.CMSG_END_OPEN_BOX, data, {opcodes.SMSG_END_OPEN_BOX})
			else
				local time_ = tonumber(self.player.box_open_time) - tonumber(timerMgr:now_string())
				local jewel = math.floor((time_ + 179999) / 180000)
				msg.is_jewel = 1
				if(self.player.jewel >= jewel) then
					local data = msg:SerializeToString()
					GameTcp.Send(opcodes.CMSG_END_OPEN_BOX, data, {opcodes.SMSG_END_OPEN_BOX})
				else
					local cancel_func = function() GUIRoot.ShowGUI("ChestPanel") end
					GUIRoot.HideGUI("ChestPanel", false)
					local content = string.format(Config.get_t_script_str('ChestPanel_014'),self.font_color[2],Config.get_t_resource(2).name);
					GUIRoot.ShowGUI("SelectPanel", {content, Config.get_t_script_str('ChestPanel_015'), ChestPanel.Recharge, Config.get_t_script_str('ChestPanel_016'), cancel_func})
				end
			end
		else
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ChestPanel_011')})
		end
	end
end

function ChestPanel.BattleChest()
	if(self.player.box_zd_opened == 0) then
		GameTcp.Send(opcodes.CMSG_OPEN_BATTLE_BOX, nil, {opcodes.SMSG_OPEN_BATTLE_BOX})
	end
end

function ChestPanel.ShareChest()
	if(self.player.fenxiang_state == 1) then
		GameTcp.Send(opcodes.CMSG_OPEN_FENXIANG_BOX, nil, {opcodes.SMSG_OPEN_FENXIANG_BOX})
	end
end
----------------------------------------------------------------


function ChestPanel.CountReward(msg)
	chest_rewards_ = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
	for i = 1, #chest_rewards_ do
		self.add_reward(chest_rewards_[i])
	end
end


function ChestPanel.ClosePanel()
	GUIRoot.HideGUI('ChestPanel')
	if(open_mode_ == 3) then
		GUIRoot.ShowGUI("SharePanel", {1})
	end
end

---------------------------服务器code---------------------------

function ChestPanel.SMSG_START_OPEN_BOX()
	local chest_id = self.player.box_ids[chest_slot_]
	local chest_temp = Config.get_t_chest(chest_id)
	if(chest_slot_ <= #self.player.box_ids and self.player.box_ids[chest_slot_] ~= 0) then
		self.player.box_open_slot = chest_slot_
		self.player.box_open_time = tonumber(timerMgr:now_string()) + chest_temp.get_time() * 1000
		ChestShowPanel.RefreshChestPanel(chest_slot_)
		GUIRoot.HideGUI('ChestPanel')
	end
end

function ChestPanel.SMSG_END_OPEN_BOX(message)
	local msg = msg_hall_pb.smsg_end_open_box()
	msg:ParseFromString(message.luabuff)
	local chest_id = self.player.box_ids[chest_slot_]
	local chest_temp = Config.get_t_chest(chest_id)
	if(self.player.box_open_slot == chest_slot_) then
		timerMgr:RemoveRepeatTimer('ChestPanel')
		chest_id_ = self.player.box_ids[chest_slot_]
		self.player.box_open_slot = 0
		self.player.box_ids[chest_slot_] = 0
		if(msg.jewel > 0) then
			self.add_resource(2, -msg.jewel)
		end
		ChestPanel.CountReward(msg)
		ChestShowPanel.RefreshChestPanel(chest_slot_)
		open_panel_.gameObject:SetActive(false)
		for i = 1, #chest_prefabs_ do
			chest_prefabs_[i].gameObject:SetActive(false)
		end
		if(#chest_prefabs_ > 0) then
			chest_prefabs_[chest_id_].gameObject:SetActive(true)
		end
		reward_panel_.gameObject:SetActive(true)
		timerMgr:AddTimer('ChestPanel', ChestPanel.ShowReward, 0.5)
		timerMgr:AddRepeatTimer('Effect_num', ChestPanel.Effect_num, 0.02, 0.02)
		GUIRoot.ShowGUI("ChestPanel")
	end
end

function ChestPanel.SMSG_OPEN_BATTLE_BOX(message)
	local msg = msg_hall_pb.smsg_end_open_box()
	msg:ParseFromString(message.luabuff)
	if(self.player.box_zd_opened == 0) then
		chest_id_ = 7
		self.player.box_zd_opened = self.player.box_zd_opened + 1
		ChestShowPanel.InitBattleChest()
		ChestPanel.CountReward(msg)
		open_panel_.gameObject:SetActive(false)
		for i = 1, #chest_prefabs_ do
			chest_prefabs_[i].gameObject:SetActive(false)
		end
		if(#chest_prefabs_ > 0) then
			chest_prefabs_[chest_id_].gameObject:SetActive(true)
		end
		reward_panel_.gameObject:SetActive(true)
		timerMgr:AddTimer('ChestPanel', ChestPanel.ShowReward, 0.5)
		timerMgr:AddRepeatTimer('Effect_num', ChestPanel.Effect_num, 0.02, 0.02)
	end
end

function ChestPanel.SMSG_ITEM_BUY_BOX(message)
	local msg = msg_hall_pb.smsg_end_open_box()
	msg:ParseFromString(message.luabuff)
	local shop_temp = Config.get_t_shop(chest_slot_)
	local chest_temp = Config.get_t_chest(msg.id)
	if(chest_temp ~= nil) then
		if(shop_temp.past_type == 2) then
			self.add_resource(shop_temp.past_type, -shop_temp.price)
		else
			self.add_resource(shop_temp.past_type, -shop_temp.price)
		end
		chest_id_ = chest_temp.id % 100
		ChestPanel.CountReward(msg)
		open_panel_.gameObject:SetActive(false)
		for i = 1, #chest_prefabs_ do
			chest_prefabs_[i].gameObject:SetActive(false)
		end
		if(#chest_prefabs_ > 0) then
			chest_prefabs_[chest_id_].gameObject:SetActive(true)
		end
		reward_panel_.gameObject:SetActive(true)
		timerMgr:AddTimer('ChestPanel', ChestPanel.ShowReward, 0.5)
		timerMgr:AddRepeatTimer('Effect_num', ChestPanel.Effect_num, 0.02, 0.02)
		GUIRoot.ShowGUI("ChestPanel")
	end
end

function ChestPanel.SMSG_OPEN_FENXIANG_BOX(message)
	local msg = msg_hall_pb.smsg_end_open_box()
	msg:ParseFromString(message.luabuff)
	if(self.player.fenxiang_state == 1) then
		self.player.fenxiang_state = 2
		chest_id_ = 8
		ChestPanel.CountReward(msg)
		open_panel_.gameObject:SetActive(false)
		for i = 1, #chest_prefabs_ do
			chest_prefabs_[i].gameObject:SetActive(false)
		end
		if(#chest_prefabs_ > 0) then
			chest_prefabs_[chest_id_].gameObject:SetActive(true)
		end
		reward_panel_.gameObject:SetActive(true)
		timerMgr:AddTimer('ChestPanel', ChestPanel.ShowReward, 0.5)
		timerMgr:AddRepeatTimer('Effect_num', ChestPanel.Effect_num, 0.02, 0.02)
		SharePanel.InitPanel()
	end
end