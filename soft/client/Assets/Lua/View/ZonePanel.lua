ZonePanel = {}
ZonePanel.bobjpool = nil

local player = nil
local battle_results = {}
local open_mode_ = 0

local lua_script_

local home_panel_
local history_panel_
local achieve_panel_
local name_panel_
local data_panel_
local achieve_detail_
local tag_detail_

local record_res_
local record_view
local record_pos = Vector3(-45, 145, 0)
local view_softness = Vector2(0, 10)
local record_space_y = 115
local record_view_pos = Vector3.one
local record_view_offset = Vector2(0, 0)

local achieve_res_
local star_res_
local achieve_view_
local achieve_pos = Vector3(-300, 125, 0)
local achieve_view_pos = Vector3.one

local signature_input_
local country_list_
local province_list_
local city_list_
local countryItem_res

local player_sex_ = 0
local player_name_ = ''

local page_btn_ = {}
local page_text_ = {}
local rank_sprite = {"mc_one", "mc_two", "mc_three"}
local tag_sprite = {"zdjsbq_002", "zdjsbq_001", "zdjsbq_003"}
local country_iconItem_panel = nil
local country_iconItem_Pos = 0;
local country_iconItem_pool = nil
local tempFlags_BtnTable = {}

local dragbg_
local region_num = 0
local region_list = {}
local region_obj = {}
local select_region_id = 0

function ZonePanel.Awake(obj)

	table.insert(page_text_,Config.get_t_script_str('ZonePanel_001'))
	table.insert(page_text_,Config.get_t_script_str('ZonePanel_002'))
	table.insert(page_text_,Config.get_t_script_str('ZonePanel_003'))
	
	dragbg_ = obj.transform:Find('data_panel/dragbg').gameObject
	
	tempFlags_BtnTable = {};
	GUIRoot.UIEffect(obj, 0)
	GUIRoot.ShowGUI('BackPanel', {3})
	ZonePanel.bobjpool = bobjpool();
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	country_iconItem_panel = obj.transform:Find('data_panel/country_show')

	home_panel_ = obj.transform:Find('home_panel')
	history_panel_ = obj.transform:Find('history_panel')
	achieve_panel_ = obj.transform:Find('achieve_panel')
	name_panel_ = obj.transform:Find('name_panel')
	data_panel_ = obj.transform:Find('data_panel')
	achieve_detail_ = obj.transform:Find('achieve_detail')
	tag_detail_ = obj.transform:Find('tag_detail')
	
	record_res_ = history_panel_:Find('record_res')
	record_view = history_panel_:Find('record_view')
	
	achieve_res_ = achieve_panel_:Find('achieve_res').gameObject
	star_res_ = achieve_panel_:Find('star_res').gameObject
	achieve_view_ = achieve_panel_:Find('achieve_view')
	achieve_view_:GetComponent("UIPanel").clipSoftness = view_softness
	achieve_view_pos = achieve_view_.localPosition
	
	record_view:GetComponent("UIPanel").clipSoftness = view_softness
	record_view_pos = record_view.localPosition
	record_view_offset = record_view:GetComponent("UIPanel").clipOffset
	
	country_list_ = data_panel_:Find('country_menu'):GetComponent('UIPopupList')
	province_list_ = data_panel_:Find('province_menu'):GetComponent('UIPopupList')
	city_list_ = data_panel_:Find('city_menu'):GetComponent('UIPopupList')
	countryItem_res = data_panel_:Find('country_res').gameObject
	signature_input_ = home_panel_:Find('self_panel/signature'):GetComponent('UIInput')
	
	for i = 0, obj.transform:Find("left_panel").childCount - 1 do
		local page_btn = obj.transform:Find("left_panel"):GetChild(i).gameObject
		if(page_btn.name ~= "bg") then
			table.insert(page_btn_, page_btn)
			lua_script_:AddButtonEvent(page_btn, "click", ZonePanel.Click)
		end
	end
	
	local alter_avatar_btn = home_panel_:Find('self_panel/alter_avatar_btn')
	local alter_name_btn = home_panel_:Find('self_panel/alter_name_btn')
	local alter_data_btn = home_panel_:Find('self_panel/alter_data_btn')
	local achieve_detail_close = achieve_detail_:Find('bg').gameObject
	local tag_detail_close = tag_detail_:Find('tag_close').gameObject
	
	lua_script_:AddButtonEvent(alter_avatar_btn.gameObject, "click", ZonePanel.Click)
	lua_script_:AddButtonEvent(alter_name_btn.gameObject, "click", ZonePanel.Click)
	lua_script_:AddButtonEvent(alter_data_btn.gameObject, "click", ZonePanel.Click)
	lua_script_:AddButtonEvent(achieve_detail_close, "click", ZonePanel.Click)
	lua_script_:AddButtonEvent(tag_detail_close, "click", ZonePanel.Click)
	
	home_panel_.gameObject:SetActive(false)
	history_panel_.gameObject:SetActive(false)
	name_panel_.gameObject:SetActive(false)
	achieve_panel_.gameObject:SetActive(false)
	data_panel_.gameObject:SetActive(false)
	achieve_detail_.gameObject:SetActive(false)
	tag_detail_.gameObject:SetActive(false)

	ZonePanel.RegisterMessage()
end

function ZonePanel.RegisterMessage()
	Message.register_handle("back_panel_msg", ZonePanel.Back)
	Message.register_net_handle(opcodes.SMSG_CHANGE_NAME, ZonePanel.SMSG_CHANGE_NAME)
	Message.register_net_handle(opcodes.SMSG_MODIFY_DATA, ZonePanel.SMSG_MODIFY_DATA)
	Message.register_net_handle(opcodes.SMSG_INFOMATION, ZonePanel.SMSG_INFOMATION)
	Message.register_handle("back_panel_recharge", ZonePanel.Recharge)
	Message.register_handle("team_join_msg", ZonePanel.TeamJoin)
end

function ZonePanel.RemoveMessage()
	Message.remove_handle("back_panel_msg", ZonePanel.Back)
	Message.remove_net_handle(opcodes.SMSG_CHANGE_NAME, ZonePanel.SMSG_CHANGE_NAME)
	Message.remove_net_handle(opcodes.SMSG_MODIFY_DATA, ZonePanel.SMSG_MODIFY_DATA)
	Message.remove_net_handle(opcodes.SMSG_INFOMATION, ZonePanel.SMSG_INFOMATION)
	Message.remove_handle("back_panel_recharge", ZonePanel.Recharge)
	Message.remove_handle("team_join_msg", ZonePanel.TeamJoin)
end

function ZonePanel.OnDestroy()
	lua_script_ = nil
	player = nil
	battle_results = {}
	page_btn_ = {}
	player_sex_ = 0
	open_mode_ = 0
	player_name_ = ''
	tempFlags_BtnTable = {}
	region_num = 0
	region_list = {}
	for k,v in pairsByKeys(region_obj) do
		GameObject.Destroy(v)
	end
	region_obj = {}
    select_region_id = 0
	ZonePanel.RemoveMessage()
end

function ZonePanel.OnParam(parm)
	player = parm[1]
	battle_results = self.rank_battle_result(parm[2])
	if (parm[3] ~= nil) then
		open_mode_ = parm[3]
	end
	ZonePanel.SelectPage(0)
end

function ZonePanel.Back()
	if(open_mode_ == 0) then
		GUIRoot.HideGUI("ZonePanel")
		GUIRoot.HideGUI("BackPanel")
		GUIRoot.ShowGUI('HallPanel')
	elseif open_mode_ == 2 then
		GUIRoot.HideGUI("ZonePanel")
		GUIRoot.ShowGUI('RankPanel')
	else
		GUIRoot.HideGUI("ZonePanel")
		GUIRoot.ShowGUI('FriendPanel')
	end
end

function ZonePanel.Recharge()
	GUIRoot.HideGUI('ZonePanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function ZonePanel.TeamJoin()
	GUIRoot.HideGUI('ZonePanel')
end


------------------初始化界面-----------------

function ZonePanel.InitHomePanel()
	history_panel_.gameObject:SetActive(false)
	home_panel_:Find('self_panel').gameObject:SetActive(false)
	home_panel_:Find('other_panel').gameObject:SetActive(false)
	local friend_btn = home_panel_:Find('other_panel/friend_btn').gameObject
	local black_btn = home_panel_:Find('other_panel/black_btn').gameObject
	local name_label = home_panel_:Find('data_panel/name'):GetComponent('UILabel')
	local area_sp = home_panel_:Find('data_panel/area'):GetComponent('UISprite')
	local role_num = home_panel_:Find('rank_panel/total_panel/role_num'):GetComponent('UILabel')
	local avatar_num = home_panel_:Find('rank_panel/total_panel/avatar_num'):GetComponent('UILabel')
	local toukuang_num = home_panel_:Find('rank_panel/total_panel/toukuang_num'):GetComponent('UILabel')
	local battle_num = home_panel_:Find('rank_panel/total_panel/battle_num'):GetComponent('UILabel')
	local avatar_icon = home_panel_:Find('data_panel/avatar_inf')
	local player_lv = home_panel_:Find('data_panel/lv'):GetComponent('UILabel')
	local level_add = home_panel_:Find('rank_panel/add_panel/level_add'):GetComponent('UILabel')
	local battle_gold = home_panel_:Find('rank_panel/add_panel/battle_gold'):GetComponent('UILabel')
	local last_cup_num = home_panel_:Find('rank_panel/cup_panel/last_cup_num'):GetComponent('UILabel')
	AvaIconPanel.ModifyAvatar(avatar_icon, player.avatar_on,'', player.toukuang_on, player.sex)
	player_lv.text = player.level
	local color = NoticePanel.GetVipNameColor(player)
	name_label.text = player.name
	IconPanel.InitVipLabel(name_label, color)
	last_cup_num.text = Config.get_t_cup(player.max_cup).name
	IconPanel.InitCupLabel(Config.get_t_cup(player.max_cup).dai_icon, last_cup_num)
	if(home_panel_:Find('rank_panel/cup_panel/cur_cup') ~= nil) then
		GameObject.Destroy(home_panel_:Find('rank_panel/cup_panel/cur_cup').gameObject)
	end
	local cur_cup = IconPanel.GetCup(player.cup)
	cur_cup.transform.parent = home_panel_:Find('rank_panel/cup_panel')
	cur_cup.transform.localScale = Vector3.one
	cur_cup.transform.localPosition = Vector3(330, -225, 0)
	cur_cup.name = "cur_cup"
	cur_cup:SetActive(true)
	battle_gold.text = '[c2e5ed]'..Config.get_t_script_str('ZonePanel_004')..'[-]     '..self.font_color[3]..player.battle_gold.."/"..self.get_out_attr(8)
	if(player.region_id == "") then
		area_sp.spriteName = Config.get_t_foregion(0).icon
	else
		area_sp.spriteName = Config.get_t_foregion(player.region_id).icon
	end
	role_num.text = #player.role_guid
	avatar_num.text = #player.avatar
	toukuang_num.text = #player.toukuang
	battle_num.text = player.battle_num
	local exp_temp = Config.get_t_exp(player.level)
	local level_add_text = ""
	if(#exp_temp.level_add == 0) then
		level_add_text = Config.get_t_script_str('ZonePanel_006') --"无"
	end
	for i = 1, #exp_temp.level_add do
		local t_attr = Config.get_t_acc_add(exp_temp.level_add[i].type, exp_temp.level_add[i].param1)
		if(t_attr ~= nil) then
			local att_desc = string.gsub(t_attr.desc, "{N1}", exp_temp.level_add[i].param3)
			level_add_text = level_add_text..att_desc
			if(i < #exp_temp.level_add) then
				level_add_text = level_add_text.."\n"
			end
		end
	end
	level_add.text = level_add_text
	if(player.guid == self.guid) then
		signature_input_.value = player.infomation
		lua_script_:AddButtonEvent(signature_input_.gameObject, "UIInput", ZonePanel.OnSignatureSubmit)
		home_panel_:Find('self_panel').gameObject:SetActive(true)
	else
		if(self.social_type(player.guid) == 2) then
			black_btn.transform.localPosition = friend_btn.transform.localPosition
			friend_btn:SetActive(false)
		elseif(self.social_type(player.guid) == 3) then
			black_btn:SetActive(false)
		end
		if(player.infomation == "")then
			home_panel_:Find('other_panel/signature/Label'):GetComponent("UILabel").text = Config.get_t_script_str('ZonePanel_007') --"一切尽在不言中~~"
		else
			home_panel_:Find('other_panel/signature/Label'):GetComponent("UILabel").text = player.infomation
		end
		home_panel_:Find('other_panel').gameObject:SetActive(true)
		lua_script_:AddButtonEvent(friend_btn, "click", ZonePanel.Click)
		lua_script_:AddButtonEvent(black_btn, "click", ZonePanel.Click)
	end
	home_panel_.gameObject:SetActive(true)
end

function ZonePanel.InitHistoryPanel(is_delete)
	local empty_tip = history_panel_:Find('empty_tip')
	empty_tip.gameObject:SetActive(false)
	if(is_delete) then
		home_panel_.gameObject:SetActive(false)
		if(record_view.childCount > 0) then
			for i = 0, record_view.childCount - 1 do
				GameObject.Destroy(record_view:GetChild(i).gameObject)
			end
		end
		if record_view:GetComponent('SpringPanel') ~= nil then
			record_view:GetComponent('SpringPanel').enabled = false
		end
		record_view:GetComponent("UIPanel").clipOffset = record_view_offset
		record_view.localPosition = record_view_pos
	end
	if(#battle_results == 0) then
		empty_tip.gameObject:SetActive(true)
	end
	for i = 1, #battle_results do
		local battle_result = battle_results[i]
		local record = LuaHelper.Instantiate(record_res_.gameObject)
		record.transform.parent = record_view
		record.transform.localPosition = Vector3(0, -record_space_y * (i - 1), 0) + record_pos
		record.transform.localScale = Vector3.one
		local rank = record.transform:Find('rank'):GetComponent("UILabel")
		local rank_icon = record.transform:Find('rank_icon'):GetComponent("UISprite")
		local kill = record.transform:Find('kill'):GetComponent("UILabel")
		local death = record.transform:Find('death'):GetComponent("UILabel")
		local score = record.transform:Find('score'):GetComponent("UILabel")
		local time_ = record.transform:Find('time'):GetComponent("UILabel")
		local mode = record.transform:Find('mode'):GetComponent("UILabel")
		local tag_res = record.transform:Find('tag_res').gameObject
		rank_icon.transform.gameObject:SetActive(false)
		--------------------------------------------------------
		local role_temp = Config.get_t_role(battle_result.role_id)
		local get_dou = function(num)
				local tag_list = {}
				while num >= 2 do
					table.insert(tag_list, 1, num % 2)
					num = math.floor(num / 2)
				end
				table.insert(tag_list, 1, num)
				if(#tag_list < #tag_sprite) then
					for i = 1, #tag_sprite - #tag_list do
						table.insert(tag_list, 1, 0)
					end
				end
				return tag_list
			end
		if(role_temp ~= nil) then
			if(battle_result.rank <= 3) then
				rank_icon.transform.gameObject:SetActive(true)
				rank.transform.gameObject:SetActive(false)
				rank_icon.spriteName = rank_sprite[battle_result.rank]
			end
			if(battle_result.type < 0 or battle_result.type > 2) then
				battle_result.type = 1
			end
			rank.text = battle_result.rank
			kill.text = battle_result.sha
			death.text = battle_result.die
			score.text = battle_result.score
		else
			role_temp = Config.get_t_role(1001)
			rank.text = Config.get_t_script_str('ZonePanel_008') --"丢失"
			kill.text = Config.get_t_script_str('ZonePanel_008') --"丢失"
			death.text = Config.get_t_script_str('ZonePanel_008') --"丢失"
			score.text = Config.get_t_script_str('ZonePanel_008') --"丢失"
		end
		time_.text = count_time_delta(tonumber(timerMgr:now_string()) - tonumber(battle_result.time))
		mode.text = Config.t_mode[battle_result.type + 2].name1
		local j = 0
		local tag_list = get_dou(battle_result.achieve)
		if(#tag_list >= #tag_sprite) then
			for i = 1, #tag_sprite do
				if(tag_list[#tag_sprite - i + 1] == 1) then
					local tag_t = LuaHelper.Instantiate(tag_res.gameObject)
					tag_t.transform.parent = record.transform
					tag_t.transform.localPosition = Vector3(0, -30 * j, 0) + Vector3(-223, 30, 0)
					tag_t.transform.localScale = Vector3.one
					tag_t.transform:GetComponent("UISprite").spriteName = tag_sprite[i]
					lua_script_:AddButtonEvent(tag_t, "click", ZonePanel.TagDetail)
					tag_t:SetActive(true)
					j = j + 1
				end
			end
		end
		local icon = IconPanel.GetIcon("reward_res", nil, role_temp.icon, role_temp.color, 0)
		icon.transform.parent = record.transform
		icon.transform.localPosition = Vector3(-285, 0, 0)
		icon.transform.localScale = Vector3.one
		icon.transform:Find("icon"):GetComponent("Collider").enabled = false
		icon.gameObject:SetActive(true)	
		---------------------------------------------------------
		record:SetActive(true)
	end
	history_panel_.gameObject:SetActive(true)
end

function ZonePanel.InitAchievePanel()
	local empty_tip = achieve_panel_:Find('empty_tip')
	empty_tip.gameObject:SetActive(false)
	if(achieve_view_.childCount > 0) then
		for i = 0, achieve_view_.childCount - 1 do
			GameObject.Destroy(achieve_view_:GetChild(i).gameObject)
		end
	end
	if achieve_view_:GetComponent('SpringPanel') ~= nil then
		achieve_view_:GetComponent('SpringPanel').enabled = false
	end
	achieve_view_:GetComponent("UIPanel").clipOffset = Vector2(0, 0)
	achieve_view_.localPosition = achieve_view_pos
	if(#player.achieve_reward == 0) then
		empty_tip.gameObject:SetActive(true)
	end
	local all_achieve_types = {}
	for k,v in pairs(Config.t_achievement.achieveTrees) do
		table.insert(all_achieve_types,k)
	end
	local comp = function (a,b)
		return a < b
	end
	table.sort(all_achieve_types, comp)
	local complete_achieve = {}
	for i = 1, #all_achieve_types do
		local parent_id = all_achieve_types[i]
		local list = Config.t_achievement.achieveTrees[parent_id]
		local max_achieve = nil
		for k = 1,#list do
			if ZonePanel.IsComplete(list[k].id) then
				max_achieve = list[k]
			end
		end
		if(max_achieve ~= nil) then
			table.insert(complete_achieve, max_achieve)
		end
	end
	for i = 1, #complete_achieve do
		local achieve_temp = complete_achieve[i]
		if(achieve_temp ~= nil) then
			local achieve_t = LuaHelper.Instantiate(achieve_res_)
			achieve_t.transform.parent = achieve_view_
			achieve_t.transform.localPosition = Vector3((i - 1) % 4 * 170, -math.floor((i - 1) / 4) * 245, 0) + achieve_pos
			achieve_t.transform.localScale = Vector3.one
			local first_pos = 14 - achieve_temp.max_star * 28 / 2
			for j = 1, achieve_temp.max_star do
				local star_t = LuaHelper.Instantiate(star_res_)
				star_t.transform.parent = achieve_t.transform
				star_t.transform.localPosition = Vector3(first_pos + (j - 1) * 28, -45, 0)
				star_t.transform.localScale = Vector3.one
				if(achieve_temp.star >= j) then
					star_t.transform:GetComponent("UISprite").spriteName = "star_icon"
				end
				star_t:SetActive(true)
			end
			local icon = achieve_t.transform:Find("icon"):GetComponent("UISprite")
			local name = achieve_t.transform:Find("name"):GetComponent("UILabel")
			icon.spriteName = achieve_temp.icon
			name.text = achieve_temp.name
			icon.name = achieve_temp.id
			lua_script_:AddButtonEvent(icon.gameObject, "click", ZonePanel.SelectAchieve)
			achieve_t:SetActive(true)
		end
	end
	achieve_panel_:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('ZonePanel_009')..ZonePanel.GetCurrentAchieveLevel()
	achieve_panel_.gameObject:SetActive(true)
end

function ZonePanel.GetCurrentAchieveLevel()
	local level = 1
	for i = 1, Config.max_achieve_level do
		local t_level_reward = Config.get_t_achievement_reward(i)
		if player.achieve_point >= t_level_reward.total_point then
			level = i
		end
	end
	return level
end

function ZonePanel.IsComplete(achieve_id)
	for i = 1,#player.achieve_reward do
		if player.achieve_reward[i] == achieve_id then
			return true
		end
	end
	return false
end
---------------------------------------------


------------------ButtonEvent----------------

function ZonePanel.OnDragFinished()
	local uv = record_view:GetComponent('UIScrollView')
	local constraint = uv.panel:CalculateConstrainOffset(uv.bounds.min, uv.bounds.max)
	if(constraint.y < -10) then
		if record_view:GetComponent('SpringPanel') ~= nil then
			record_view:GetComponent('SpringPanel').enabled = false
		end
		ZonePanel.InitHistoryPanel(false)
	end
end

function ZonePanel.Click(obj)
	if(obj.name == 'home_btn') then
		ZonePanel.SelectPage(0)
	elseif(obj.name == 'history_btn') then
		ZonePanel.SelectPage(2)
	elseif(obj.name == 'achiv_btn') then
		ZonePanel.SelectPage(1)
	elseif(obj.name == 'alter_avatar_btn') then
		GUIRoot.HideGUI('ZonePanel')
		GUIRoot.ShowGUI('AvatarPanel')
	elseif(obj.name == 'alter_name_btn') then
		ZonePanel.OpenNamePanel()
	elseif(obj.name == 'alter_data_btn') then
		--ZonePanel.OpenDataPanel()
		ZonePanel.OpenEditPage()
	elseif(obj.name == 'friend_btn') then
		NoticePanel.InitAddPanel(player.guid)
	elseif(obj.name == 'black_btn') then
		FriendPanel.AddPlayerBlack(player)
	elseif(obj.name == 'bg') then
		achieve_detail_.gameObject:SetActive(false)
	elseif(obj.name == 'tag_close') then
		tag_detail_.gameObject:SetActive(false)
	end
end

function ZonePanel.SelectPage(page)
	home_panel_.gameObject:SetActive(false)
	history_panel_.gameObject:SetActive(false)
	achieve_panel_.gameObject:SetActive(false)
	for i = 1, #page_btn_ do
		page_btn_[i].transform:Find("highlight").gameObject:SetActive(false)
		page_btn_[i].transform:Find("Label"):GetComponent("UILabel").text = "[7E90A4]"..page_text_[i]
	end
	page_btn_[page + 1].transform:Find("highlight").gameObject:SetActive(true)
	page_btn_[page + 1].transform:Find("Label"):GetComponent("UILabel").text = "[E8FCFF]"..page_text_[page + 1]
	if(page == 0) then
		ZonePanel.InitHomePanel()
	elseif(page == 1) then	
		ZonePanel.InitAchievePanel()
	elseif(page == 2) then
		ZonePanel.InitHistoryPanel(true)
	end
end

function ZonePanel.OpenNamePanel()
	local name_input = name_panel_:Find('name'):GetComponent('UIInput')
	name_panel_:Find('name/Label'):GetComponent('UILabel').text = Config.get_t_script_str('ZonePanel_018')
	local tip = name_panel_:Find('use_tip'):GetComponent('UILabel')
	local card_tip = name_panel_:Find('card_tip'):GetComponent('UILabel')
	if(self.player.change_name_num == 0) then
		tip.text = Config.get_t_script_str('ZonePanel_010') --"首次修改昵称免费"
	elseif(self.player.change_name_num > 0) then
		tip.text =  Config.get_t_script_str('ZonePanel_011') --"[c2e5ed]使用[ff41da]改名卡[-]来修改你的账号昵称"
	end
	name_input.value = ''
	card_tip.text =  Config.get_t_script_str('ZonePanel_012')..self.get_item_num(50010002) --"[ff41da]改名卡[-][c2e5ed]剩余数量：[-][57FC5B]"..self.get_item_num(50010002)
	local random_btn = name_panel_:Find('random_btn')
	lua_script_:AddButtonEvent(random_btn.gameObject, 'click', ZonePanel.RandomName)
	local confirm_btn = name_panel_:Find('confirm_btn')
	lua_script_:AddButtonEvent(confirm_btn.gameObject, 'click', ZonePanel.AlterName)
	local cancel_btn = name_panel_:Find('cancel_btn')
	lua_script_:AddButtonEvent(cancel_btn.gameObject, 'click', ZonePanel.CancelAlterName)
	name_panel_.gameObject:SetActive(true)
end

function ZonePanel.AlterName()
	if(self.get_item_num(50010002) >= 1 or self.player.change_name_num <= 0) then
		local name_input = name_panel_:Find('name'):GetComponent('UIInput')
		if(name_input.value ~= '') then
			player_name_ = name_input.value
			local is_ill = IsMatch(player_name_, "^[A-Za-z0-9@_\\u4e00-\\u9fa5]+$")
			if(ZonePanel.StrLen(player_name_) > 8 or ZonePanel.StrLen(player_name_) < 2) then
				GUIRoot.ShowGUI('MessagePanel', {Config.get_t_script_str('ZonePanel_013')})
			elseif(not is_ill) then
				GUIRoot.ShowGUI('MessagePanel', {Config.get_t_script_str('ZonePanel_014')})
			else
				local msg = msg_hall_pb.cmsg_player_name()
				msg.name = name_input.value
				local data = msg:SerializeToString()
				GameTcp.Send(opcodes.CMSG_CHANGE_NAME, data, {opcodes.SMSG_CHANGE_NAME})
			end
		end
	else
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_item(50010002).name..Config.get_t_script_str('ZonePanel_015')})
		GUIRoot.ShowGUI("BuyPanel", {Config.get_shop_by_item(50010002).id, {"ZonePanel"}})
	end
end

function ZonePanel.StrLen(str)
	local length = 0
	for ch in string.gmatch(str, "[\\0-\127\194-\244][\128-\191]*") do
		if(ch ~= nil) then
			length = length + 1
		end
	end
	return length
end

function ZonePanel.RandomName()
	local name_input = name_panel_:Find('name'):GetComponent('UIInput')
	name_input.value = Config.get_t_name()
end

function ZonePanel.CancelAlterName()
	name_panel_.gameObject:SetActive(false)
end

function ZonePanel.OpenDataPanel()
	lua_script_:AddButtonEvent(country_list_.gameObject, "PopupList", ZonePanel.OnCountryChange)
	lua_script_:AddButtonEvent(province_list_.gameObject, "PopupList", ZonePanel.OnProvinceChange)
	country_list_:Clear()
	local first = true
	local first_country = ''
	for k, v in pairsByKeys(Config.t_province) do
		country_list_:AddItem(k)
		if(first) then
			first_country = k
			first = false
		end
	end
	local region_tab = string_split(self.player.region, ' ')
	if(region_tab[1] ~= nil and region_tab[1] ~= '') then
		country_list_.value = region_tab[1]
	else
		country_list_.value = first_country
	end
	if(region_tab[2] ~= nil and region_tab[2] ~= '') then
		province_list_.value = region_tab[2]
	end
	if(region_tab[3] ~= nil and region_tab[3] ~= '') then
		city_list_.value = region_tab[3]
	end
	local confirm_btn = data_panel_:Find('confirm_btn')
	local cancel_btn = data_panel_:Find('cancel_btn')
	local boy_bg = data_panel_:Find('sex_inf/boy/bg')
	local girl_bg = data_panel_:Find('sex_inf/girl/bg')
	boy_bg.gameObject:SetActive(false)
	girl_bg.gameObject:SetActive(false)
	player_sex_ = player.sex
	if(player_sex_ == 0) then
		boy_bg.gameObject:SetActive(true)
	else
		girl_bg.gameObject:SetActive(true)
	end
	lua_script_:AddButtonEvent(boy_bg.parent.gameObject, 'click', ZonePanel.SelectSex)
	lua_script_:AddButtonEvent(girl_bg.parent.gameObject, 'click', ZonePanel.SelectSex)
	lua_script_:AddButtonEvent(confirm_btn.gameObject, 'click', ZonePanel.AlterData)
	lua_script_:AddButtonEvent(cancel_btn.gameObject, 'click', ZonePanel.CancelAlterData)
	data_panel_.gameObject:SetActive(true)
end

--hhq
function ZonePanel.OpenEditPage()
	data_panel_.gameObject:SetActive(true)
	region_list = {}
	for k,v in pairsByKeys(Config.t_regions) do
		table.insert(region_list,v)
	end
	
	if #region_list > 30 then
		region_num = 30
	else
		region_num = #region_list
	end
	
	if self.player.region_id == nil then
		select_region_id = 0
	else
		select_region_id = self.player.region_id
	end
	
	for i = 0,region_num - 1 do
		local row = toInt(i / 5)
		local col = i % 5
		
		local region_info = region_list[i + 1]
		local obj = LuaHelper.Instantiate(countryItem_res)
		obj.transform.parent = country_iconItem_panel
		obj.transform.localPosition = Vector3(-250 + col * 125,row * -50,0)
		obj.transform.localScale = Vector3.one
		obj.transform:Find('icon'):GetComponent('UISprite').spriteName = region_info.icon
		lua_script_:AddButtonEvent(obj, 'click', ZonePanel.ClickRegion)
		lua_script_:AddButtonEvent(obj,'drag',ZonePanel.LoadRegion)
		obj.name = region_info.id
		if region_info.id == select_region_id then
			obj.transform:Find('btn/selectBtn').gameObject:SetActive(true)
		end
		table.insert(region_obj,obj)
		obj:SetActive(true)
	end
		
	local confirm_btn = data_panel_:Find('confirm_btn')
	local cancel_btn = data_panel_:Find('cancel_btn')
	local boy_bg = data_panel_:Find('sex_inf/boy/bg')
	local girl_bg = data_panel_:Find('sex_inf/girl/bg')
	boy_bg.gameObject:SetActive(false)
	girl_bg.gameObject:SetActive(false)
	player_sex_ = player.sex
	if(player_sex_ == 0) then
		boy_bg.gameObject:SetActive(true)
	else
		girl_bg.gameObject:SetActive(true)
	end
	lua_script_:AddButtonEvent(boy_bg.parent.gameObject, 'click', ZonePanel.SelectSex)
	lua_script_:AddButtonEvent(girl_bg.parent.gameObject, 'click', ZonePanel.SelectSex)
	lua_script_:AddButtonEvent(confirm_btn.gameObject, 'click', ZonePanel.AlterData)
	lua_script_:AddButtonEvent(cancel_btn.gameObject, 'click', ZonePanel.CancelAlterData)
	lua_script_:AddButtonEvent(dragbg_,'drag',ZonePanel.LoadRegion)
end

function ZonePanel.LoadRegion()
	local is_end = NavUtil.IsScrollviewEnd(country_iconItem_panel.gameObject)
	if not is_end then
		return
	end
	
	local old_region_num = region_num
	
	if region_num == #region_list then
		return
	elseif region_num + 30 >= #region_list then
		region_num = #region_list
	else
		region_num = region_num + 30
	end
	
	for i = old_region_num,region_num - 1 do
		local row = toInt(i / 5)
		local col = i % 5
		
		local region_info = region_list[i + 1]
		local obj = LuaHelper.Instantiate(countryItem_res)
		obj.transform.parent = country_iconItem_panel.transform
		obj.transform.localPosition = Vector3(-250 + col * 125,row * -50,0)
		obj.transform.localScale = Vector3.one
		obj.transform:Find('icon'):GetComponent('UISprite').spriteName = region_info.icon
		obj.name = region_info.id
		if region_info.id == select_region_id then
			obj.transform:Find('btn/selectBtn').gameObject:SetActive(true)
		end
		lua_script_:AddButtonEvent(obj, 'click', ZonePanel.ClickRegion)
		lua_script_:AddButtonEvent(obj,'drag',ZonePanel.LoadRegion)
		table.insert(region_obj,obj)
		obj:SetActive(true)
	end
end

function ZonePanel.ClickRegion(obj)
	local sel = obj.transform:Find('btn/selectBtn').gameObject
	if sel.activeInHierarchy then
		return
	else
		for i = 1,region_num do
			local t = region_obj[i].transform:Find('btn/selectBtn').gameObject
			if t.activeInHierarchy then
				t:SetActive(false)
				break
			end
		end	
		sel:SetActive(true)
		select_region_id = obj.name
	end
end

function ZonePanel.AppendCountryItem(countryIcon_id)
	local bobj = nil
	bobj = ZonePanel.GetObjByName(countryItem_res);
	bobj.obj:SetActive(true)
	bobj.obj.name = countryIcon_id
	bobj.objt.parent = country_iconItem_panel
	local line = 0;
	local row = 0;
	for i = 0, countryIcon_id do
		line = math.floor(countryIcon_id/5);
		row = countryIcon_id%5;
	end
	local x = -246 + row * 130
	local y = 0 + line *(-60)
	ZonePanel.bobjpool:set_localScale(bobj.objid,1,1,1)
	ZonePanel.bobjpool:set_localPosition(bobj.objid,x,y,0)
	local dragArea = bobj.objt:Find('bg')
	local chooseBtn = bobj.objt:Find('btn')
	local icon = bobj.objt:Find('icon'):GetComponent('UISprite')
	local t_region = Config.get_t_foregion(countryIcon_id);
	icon.spriteName =t_region.icon
	lua_script_:AddButtonEvent(chooseBtn.gameObject, "click", ZonePanel.OnClick)
	lua_script_:AddButtonEvent(bobj.obj,'drag',ZonePanel.LoadFlagsItem)
	lua_script_:AddButtonEvent(dragArea.gameObject,'drag',ZonePanel.LoadFlagsItem)


end

function ZonePanel.LoadFlagsItem()
	local is_end = NavUtil.IsScrollviewEnd(country_iconItem_panel.gameObject)
	if not is_end then
		return
	end
	if country_iconItem_Pos < Config.max_flagCount then
		local st = country_iconItem_Pos + 1
		local ed = country_iconItem_Pos + 20
		if ed > Config.max_flagCount then
			ed = Config.max_flagCount
		end
		country_iconItem_Pos = ed
		for i = st,ed do
			ZonePanel.AppendCountryItem(i)
		end
	end
end
function ZonePanel.OnClick(obj)
	table.insert(tempFlags_BtnTable,1,obj)
	if #tempFlags_BtnTable >2 then
		for i = #tempFlags_BtnTable , 3 ,-1 do
			table.remove(tempFlags_BtnTable,i);
		end
	end
	if #tempFlags_BtnTable == 1 then
		obj.transform:Find('selectBtn').gameObject:SetActive(true);
	end
	if #tempFlags_BtnTable >1 then
		local delObj = tempFlags_BtnTable[2];
		delObj.transform:Find('selectBtn').gameObject:SetActive(false);
		obj.transform:Find('selectBtn').gameObject:SetActive(true);
	end
end
function ZonePanel.GetObjByName(ori_obj)
	local name = ori_obj.name

	if country_iconItem_pool == nil or country_iconItem_pool[name] == nil then
		--构建这个 对象
		local obj = LuaHelper.Instantiate(ori_obj)
		local objid = ZonePanel.bobjpool:add(obj)
		local objt = obj.transform
		ZonePanel.bobjpool:set_localScale(objid,1,1,1)

		if country_iconItem_pool == nil then
			country_iconItem_pool = {}
		end

		if country_iconItem_pool[name] == nil then
			country_iconItem_pool[name] = {}
		end
		local dt = {obj = obj,objt = objt,objid = objid,state = true}
		table.insert(country_iconItem_pool[name],dt)
		return dt
	elseif country_iconItem_pool[name]~= nil then
		for i = 1,#country_iconItem_pool[name] do
			if country_iconItem_pool[name][i].state == false then
				country_iconItem_pool[name][i].state = true
				return country_iconItem_pool[name][i]
			end
		end

		local obj = LuaHelper.Instantiate(ori_obj)
		local objid = ZonePanel.bobjpool:add(obj)
		local objt = obj.transform
		ZonePanel.bobjpool:set_localScale(objid,1,1,1)

		local dt = {obj = obj,objt = objt,objid = objid,state = true}
		table.insert(country_iconItem_pool[name],dt)
		return dt
	end

end


function ZonePanel.AlterData()
	local msg = msg_hall_pb.cmsg_player_data()
	msg.region_id = tonumber(select_region_id)
	msg.sex = player_sex_
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_MODIFY_DATA, data, {opcodes.SMSG_MODIFY_DATA})
end

function ZonePanel.OnCountryChange()
	province_list_:Clear()
	local province_list = Config.get_t_province(country_list_.value)
	for i = 1, #province_list do
		province_list_:AddItem(province_list[i])
	end
	province_list_.value = province_list[1]
end

function ZonePanel.OnProvinceChange()
	city_list_:Clear()
	local city_list = Config.get_t_city(province_list_.value)
	for i = 1, #city_list do
		city_list_:AddItem(city_list[i])
	end
	city_list_.value = city_list[1]
end

function ZonePanel.SelectSex(obj)
	local boy_bg = data_panel_:Find('sex_inf/boy/bg')
	local girl_bg = data_panel_:Find('sex_inf/girl/bg')
	boy_bg.gameObject:SetActive(false)
	girl_bg.gameObject:SetActive(false)
	if(obj.name == "boy") then
		player_sex_ = 0
		boy_bg.gameObject:SetActive(true)
	else
		player_sex_ = 1
		girl_bg.gameObject:SetActive(true)
	end
end

function ZonePanel.CancelAlterData()
	data_panel_.gameObject:SetActive(false)
end

function ZonePanel.OnSignatureSubmit()
	if(signature_input_.value ~= '') then
		local is_ill = IsMatch(signature_input_.value, "^[A-Za-z0-9@_ \\u4e00-\\u9fa5]+$")
		if(string.len(signature_input_.value) > 60) then
			signature_input_.value = ""
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ZonePanel_016')})
		elseif(not is_ill) then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ZonePanel_017')})
		else
			local msg = msg_hall_pb.cmsg_infomation()
			msg.info = signature_input_.value
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_INFOMATION, data, {opcodes.SMSG_INFOMATION})
		end
	end
end

function ZonePanel.AddFriend()
	if(lua_script_ ~= nil) then
		home_panel_:Find('other_panel/black_btn').transform.localPosition = Vector3(349, 82, 0)
		home_panel_:Find('other_panel/black_btn').gameObject:SetActive(true)
	end
end

function ZonePanel.AddFriendEnd(guid)
	if(lua_script_ ~= nil and guid == player.guid) then
		black_btn.transform.localPosition = friend_btn.transform.localPosition
		friend_btn:SetActive(false)
		black_btn:SetActive(true)
	end
end

function ZonePanel.SelectAchieve(obj)
	local achieve_id = tonumber(obj.name)
	local achieve_temp = Config.get_t_achievement(achieve_id)
	local main_panel = achieve_detail_:Find("main_panel")
	local icon = main_panel:Find("icon"):GetComponent("UISprite")
	local name = main_panel:Find("name"):GetComponent("UILabel")
	local desc = main_panel:Find("desc"):GetComponent("UILabel")
	icon.spriteName = achieve_temp.icon
	name.text = achieve_temp.name
	desc.text = achieve_temp.desc
	GUIRoot.UIEffectScalePos(main_panel.gameObject, true, 1)
	achieve_detail_.gameObject:SetActive(true)
end

function ZonePanel.TagDetail()
	local main_panel = tag_detail_:Find("main_panel")
	GUIRoot.UIEffectScalePos(main_panel.gameObject, true, 1)
	tag_detail_.gameObject:SetActive(true)
end
---------------------------------------------

--------------------服务器code---------------

function ZonePanel.SMSG_CHANGE_NAME()
	if(player_name_ ~= '') then
		self.player.name = player_name_
		if(self.player.change_name_num > 0) then
			self.delete_item_num(50010002, 1)
		end
		self.player.change_name_num = self.player.change_name_num + 1
		BackPanel.RefreshInf()
		TopPanel.RefreshInf()
		player_name_ = ''
	end
	ZonePanel.InitHomePanel()
	name_panel_.gameObject:SetActive(false)
end

function ZonePanel.SMSG_MODIFY_DATA()
	self.player.sex = player_sex_
	self.player.region_id = tonumber(select_region_id)
	ZonePanel.InitHomePanel()
	data_panel_.gameObject:SetActive(false)
end

function ZonePanel.SMSG_INFOMATION()
	if(signature_input_.value ~= "") then
		self.player.infomation = signature_input_.value
	end
	ZonePanel.InitHomePanel()
end

function ZonePanel.SMSG_SOCIAL_BLACK()
	if(lua_script_ ~= nil) then
		home_panel_:Find('other_panel/black_btn').gameObject:SetActive(false)
		home_panel_:Find('other_panel/friend_btn').gameObject:SetActive(true)
	end
end