RankPanel = {}

local lua_script_
local rank_state = {}
local area_state = {}
local title_ 

local world_ranks_ = {}
local friend_ranks = {}

local rank_item_
local empty_
local rank_root_
local self_info_

local loading_num = 0

local offset_x_
local offset_y_

local to_gift_gold_obj_
local help_lb_

function RankPanel.Awake(obj)
	GUIRoot.ShowGUI('BackPanel', {3})
	
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	local jb_btn = obj.transform:Find('left_anchor/left_panel/jb_btn')
	local cj_btn = obj.transform:Find('left_anchor/left_panel/cj_btn')
	local hs_btn = obj.transform:Find('left_anchor/left_panel/hs_btn')
	local js_btn = obj.transform:Find('left_anchor/left_panel/js_btn')
	local lp_btn = obj.transform:Find('left_anchor/left_panel/lp_btn')
	
	rank_state['jb_btn'] = {state = false,objt = jb_btn,code = 0,name = 'jb_btn',sp = jb_btn:GetComponent('UISprite'),lb = jb_btn:Find('lb'):GetComponent('UILabel')}
	rank_state['cj_btn'] = {state = false,objt = cj_btn,code = 1,name = 'cj_btn',sp = cj_btn:GetComponent('UISprite'),lb = cj_btn:Find('lb'):GetComponent('UILabel')}
	rank_state['hs_btn'] = {state = false,objt = hs_btn,code = 2,name = 'hs_btn',sp = hs_btn:GetComponent('UISprite'),lb = hs_btn:Find('lb'):GetComponent('UILabel')}
	rank_state['js_btn'] = {state = false,objt = js_btn,code = 3,name = 'js_btn',sp = js_btn:GetComponent('UISprite'),lb = js_btn:Find('lb'):GetComponent('UILabel')}
	rank_state['lp_btn'] = {state = false,objt = lp_btn,code = 4,name = 'lp_btn',sp = lp_btn:GetComponent('UISprite'),lb = lp_btn:Find('lb'):GetComponent('UILabel')}
	
	local world_area_btn = obj.transform:Find('left_anchor/world')
	world_area_btn.transform:Find('bg').gameObject:SetActive(true)
	
	local firend_area_btn = obj.transform:Find('left_anchor/friend')
	
	area_state['world'] = {state = true,bg = world_area_btn.transform:Find('bg').gameObject,code = 'world',}
	area_state['friend'] = {state = false,bg = firend_area_btn.transform:Find('bg').gameObject,code = 'friend'}
	
	rank_item_ = obj.transform:Find('right_anchor/sc_item').gameObject
	empty_ = obj.transform:Find('right_anchor/empty_info').gameObject
	rank_root_ = obj.transform:Find('right_anchor/rank_content')
	title_ = obj.transform:Find('right_anchor/rank_title_area/f4_lb'):GetComponent('UILabel')
	
	lua_script_:AddButtonEvent(jb_btn.gameObject, "click", RankPanel.Click)
	lua_script_:AddButtonEvent(cj_btn.gameObject, "click", RankPanel.Click)
	lua_script_:AddButtonEvent(hs_btn.gameObject, "click", RankPanel.Click)
	lua_script_:AddButtonEvent(js_btn.gameObject, "click", RankPanel.Click)
	lua_script_:AddButtonEvent(lp_btn.gameObject, "click", RankPanel.Click)
	
	lua_script_:AddButtonEvent(world_area_btn.gameObject, "click", RankPanel.Click)
	lua_script_:AddButtonEvent(firend_area_btn.gameObject, "click", RankPanel.Click)
	
	local bg = obj.transform:Find('bg')
	lua_script_:AddButtonEvent(bg.gameObject, "click", RankPanel.Click)
	
	RankPanel.RegisterMessage()
	RankPanel.FixAnyPosition(obj)
	RankPanel.EnableRankView('jb_btn')	
end

function RankPanel.FixAnyPosition(obj)
	local right_area_width = GUIRoot.width - 180
	local right_area_height = GUIRoot.height - 90 - 110
	
	local panel = rank_root_:GetComponent('UIPanel')
	panel.transform.localPosition = Vector3(-(right_area_width / 2) - 10,-(right_area_height / 2 + 85),0)
	panel.clipSoftness = Vector2(2,10)
	panel.baseClipRegion = Vector4(0,0,right_area_width,right_area_height)
	panel.clipOffset = Vector2(0, 0)
	
	local it_width = right_area_width - 4
	
	obj.transform:Find('right_anchor/rank_title_area').localPosition = Vector3(-(right_area_width / 2) - 10,-60,0)
	obj.transform:Find('right_anchor/rank_title_area/f1_lb').localPosition = Vector3(-it_width / 2 + 70,-14,0)
	obj.transform:Find('right_anchor/rank_title_area/f2_lb').localPosition = Vector3(-it_width / 4 + 20,-14,0)
	obj.transform:Find('right_anchor/rank_title_area/f3_lb').localPosition = Vector3(20,-14,0)
	obj.transform:Find('right_anchor/rank_title_area/f4_lb').localPosition = Vector3(150,-14,0)
	local help_btn = obj.transform:Find('right_anchor/rank_title_area/help')
	help_btn.localPosition = Vector3(it_width / 2 - 70,-14,0)
	lua_script_:AddButtonEvent(help_btn.gameObject, "click", RankPanel.Help)
	help_lb_ = obj.transform:Find('right_anchor/rank_title_area/help/help_lb'):GetComponent('UILabel')
	help_lb_.gameObject:SetActive(false)
	offset_x_ = 0
	offset_y_ = right_area_height / 2 - 50
	
	
	
	rank_item_.transform:Find('bg'):GetComponent('UISprite').width = it_width
	rank_item_.transform:Find('bg'):GetComponent('BoxCollider').size = Vector2(it_width,100)
	rank_item_.transform:Find('rank').localPosition = Vector3(-it_width / 2 + 70,-14,0)
	rank_item_.transform:Find('tx').localPosition = Vector3(-it_width / 4 + 20 - 65,-14,0)
	rank_item_.transform:Find('player_info').localPosition = Vector3(-it_width / 4 + 20,-14,0)
	rank_item_.transform:Find('area').localPosition = Vector3(20,-14,0)
	rank_item_.transform:Find('cup_btn').localPosition = Vector3(150,-14,0)
	rank_item_.transform:Find('final_lb').localPosition = Vector3(150,-14,0)
	rank_item_.transform:Find('gold').localPosition = Vector3(it_width / 2 - 70,-14,0)
	rank_item_.transform:Find('add_friend').localPosition = Vector3(it_width / 2 - 70,-14,0)
	 
	self_info_ = obj.transform:Find('bottom_anchor/self_info').gameObject
	
	self_info_.transform:Find('bg'):GetComponent('UISprite').width = it_width
	self_info_.transform:Find('rank').localPosition = Vector3(-it_width / 2 + 70,-14,0)
	self_info_.transform:Find('tx').localPosition = Vector3(-it_width / 4 + 20 - 65,-14,0)
	self_info_.transform:Find('player_info').localPosition = Vector3(-it_width / 4 + 20,-14,0)
	self_info_.transform:Find('area').localPosition = Vector3(20,-14,0)
	self_info_.transform:Find('cup_btn').localPosition = Vector3(150,-14,0)
	self_info_.transform:Find('final_lb').localPosition = Vector3(150,-14,0)
	self_info_.transform:Find('gold').localPosition = Vector3(it_width / 2 - 70,-14,0)
	self_info_.transform:Find('add_friend').localPosition = Vector3(it_width / 2 - 70,-14,0)
	
	self_info_.transform.localPosition = Vector3(-right_area_width / 2 - 10,70,0)
	self_info_.transform.localScale = Vector3.one
	self_info_:SetActive(true)
	
	obj.transform:Find('bottom_anchor/line'):GetComponent('UISprite').width = right_area_width
	obj.transform:Find('bottom_anchor/line').localPosition = Vector3(- right_area_width / 2,120,0)
	
	empty_.transform.localPosition = Vector3(-(right_area_width / 2) - 90,-(right_area_height / 2 + 110),0)
end

function RankPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_RANK, RankPanel.SMSG_RANK)
	Message.register_net_handle(opcodes.SMSG_SOCAIL_LOOK, RankPanel.SMSG_SOCAIL_LOOK)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_APPLY, RankPanel.SMSG_SOCIAL_APPLY)
	Message.register_net_handle(opcodes.SMSG_SOCIAL_GIFT, RankPanel.SMSG_SOCIAL_GIFT)
	Message.register_net_handle(opcodes.SMSG_PLAYER_LOOK, RankPanel.SMSG_PLAYER_LOOK)
	Message.register_handle("back_panel_msg", RankPanel.Back)
	Message.register_handle("back_panel_recharge", RankPanel.Recharge)
end

function RankPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_RANK, RankPanel.SMSG_RANK)
	Message.remove_net_handle(opcodes.SMSG_SOCAIL_LOOK, RankPanel.SMSG_SOCAIL_LOOK)
	Message.remove_net_handle(opcodes.SMSG_SOCIAL_APPLY, RankPanel.SMSG_SOCIAL_APPLY)
	Message.remove_net_handle(opcodes.SMSG_SOCIAL_GIFT, RankPanel.SMSG_SOCIAL_GIFT)
	Message.remove_net_handle(opcodes.SMSG_PLAYER_LOOK, RankPanel.SMSG_PLAYER_LOOK)
	Message.remove_handle("back_panel_msg", RankPanel.Back)
	Message.remove_handle("back_panel_recharge", RankPanel.Recharge)
end

function RankPanel.Help()
	if help_lb_.gameObject.activeInHierarchy then
		help_lb_.gameObject:SetActive(false)
		return
	end
	
	local code = RankPanel.GetRankCode()
	help_lb_.text = Config.get_t_rank_type(code).desc
	local len = NGUIText.CalculatePrintedWidth(help_lb_.trueTypeFont, help_lb_.fontSize,help_lb_.text)
	if len > 180 then
		len = 180
	end
	help_lb_.transform.localPosition = Vector3(-len - 50,-10,0)
	help_lb_.gameObject:SetActive(true)
end

function RankPanel.OnDestroy()
	lua_script_ = nil
	rank_state = {}
	area_state = {}
	world_ranks_ = {}
	friend_ranks = {}
	rank_item_ = nil
	empty_ = nil
	rank_root_ = nil
	self_info_ = nil
	loading_num = 0
	to_gift_gold_obj_ = nil
	RankPanel.RemoveMessage()
end

function RankPanel.GetRankName()
	for k,v in pairs(rank_state) do
		if v.state == true then
			return v.name
		end
	end
end

function RankPanel.EnableRankView(rank_name)
	local code = 0
	for k,v in pairs(rank_state) do
		if v.state == true and rank_name ~= k then
			v.state = false
			v.objt.localPosition = v.objt.localPosition - Vector3(20,0,0)
			v.sp.spriteName = 'fldb_001'
		
			v.lb.gradientTop = Color(255/ 255,255/ 255,255/ 255)
			v.lb.gradientBottom = Color(72 / 255, 126 / 255, 186 / 255)
			v.lb.effectColor = Color(13 / 255, 84 / 255, 164 / 255)
		elseif v.state == false and rank_name == k then
			v.state = true
			v.objt.localPosition = v.objt.localPosition + Vector3(20,0,0)
			code = v.code
			v.sp.spriteName = 'fldb_002'
			
			v.lb.gradientTop = Color(255/ 255,255/ 255,255/ 255)
			v.lb.gradientBottom = Color(97 / 255, 187 / 255, 237 / 255)
			v.lb.effectColor = Color(25 / 255, 147 / 255, 220 / 255)
		elseif v.state == true and rank_name == k then
			code = v.code
		end
	end
	
	if RankPanel.GetAreaCode() == 'world' then
		if world_ranks_[code] == nil then
			RankPanel.CMSG_RANK(code)
		else
			RankPanel.InitRankInfo(code,'world')
		end
	else
		RankPanel.GetFriendsInfo(code)
	end
end

function RankPanel.GetAreaCode()
	for k,v in pairs(area_state) do
		if v.state == true then
			return v.code
		end
	end
end

function RankPanel.GetRankCode()
	for k,v in pairs(rank_state) do
		if v.state == true then
			return v.code
		end
	end
end

function RankPanel.EnableRankArea(area_name)
	for k,v in pairs(area_state) do
		if v.state == true and area_name ~= k then
			v.state = false
			v.bg:SetActive(false)
		elseif v.state == false and area_name == k then
			v.state = true
			v.bg:SetActive(true)
		end
	end
end

function RankPanel.Click(obj)
	if help_lb_.gameObject.activeInHierarchy then
		help_lb_.gameObject:SetActive(false)
	end
	
	if obj.name == 'jb_btn' or obj.name == 'cj_btn' or obj.name == 'hs_btn' or obj.name == 'js_btn' or obj.name == 'lp_btn' then
		local area_code = RankPanel.GetAreaCode()
		if rank_state[obj.name].state ~= true or area_state[area_code] ~= true then
			RankPanel.EnableRankView(obj.name)
		end	
	elseif obj.name == 'world' or obj.name == 'friend' then
		local rank_name = RankPanel.GetRankName()
		if rank_state[rank_name].state ~= true or area_state[obj.name].state ~= true then
			local rank_code = RankPanel.GetRankCode()
			RankPanel.EnableRankArea(obj.name)
			if friend_ranks[rank_code] == nil then
				RankPanel.GetFriendsInfo(rank_code)
			else
				local area_code = RankPanel.GetAreaCode()
				RankPanel.InitRankInfo(rank_code,area_code)
			end
		end
	end
end

function RankPanel.InitRankInfo(rank_code,area_code)
	--更换标题
	if rank_code == 0 then
		title_.text = Config.get_t_script_str('RankPanel_001') --'奖杯'
	elseif rank_code == 1 then
		title_.text = Config.get_t_script_str('RankPanel_002') --'成就积分'
	elseif rank_code == 2 then
		title_.text = Config.get_t_script_str('RankPanel_003') --'最高得分'
	elseif rank_code == 3 then
		title_.text = Config.get_t_script_str('RankPanel_004') --'最高击杀'
	elseif rank_code == 4 then
		title_.text = Config.get_t_script_str('RankPanel_005') --'最高连杀'
	end
	
	for i = rank_root_.childCount - 1,0,-1 do
		GameObject.Destroy(rank_root_:GetChild(i).gameObject)
	end
	
	--reset
	rank_root_:GetComponent('UIScrollView'):ResetPosition()
	local panel = rank_root_:GetComponent('UIPanel')
	panel.transform.localPosition = panel.transform.localPosition + Vector3(panel.clipOffset.x,panel.clipOffset.y,0)
	panel.clipOffset = Vector2(0, 0)
	
	local infos = nil
	local is_friend = false

	if area_code == 'world' then
		infos = world_ranks_[rank_code]
	else
		infos = friend_ranks[rank_code]
		is_friend = true
	end
	
	if infos == nil or #infos <= 0 then
		empty_:SetActive(true)
		return
	else
		empty_:SetActive(false)
	end

	for i =1,#infos do
		if i > 20 then
			break;
		end
		
		local obj = nil
		if rank_code == 0 then
			obj = LuaHelper.Instantiate(rank_item_)
			RankPanel.InitJBRank(obj,infos[i],i,is_friend)
		else
			obj = LuaHelper.Instantiate(rank_item_)
			RankPanel.InitOtherRank(obj,infos[i],i,rank_code,is_friend)
		end
		obj.transform.parent = rank_root_.transform
		obj.transform.localPosition = Vector3(offset_x_,offset_y_ - 105 * (i - 1),0)
		obj.transform.localScale = Vector3.one
		obj:SetActive(true)
	end
	
	if #infos > 20 then
		loading_num = 20
	else
		loading_num = #infos
	end
	local sd = nil
	local rank_num = nil
	for i = 1,#infos do
		if infos[i].player_guid == self.player.guid then
			sd = infos[i]
			rank_num = i
			break
		end
	end
	
	if sd == nil then
		sd = {}
		sd.player_guid = self.player.guid
		sd.name = self.player.name
		sd.sex = self.player.sex
		sd.level = self.player.level
		sd.avatar = self.player.avatar_on
		sd.toukuang = self.player.toukuang_on
		
		if sd.region_id == nil then
			sd.region_id = 0
		else
			sd.region_id = self.player.region_id
		end
			
		local name_color = NoticePanel.GetVipNameColor(self.player)
		sd.name_color = name_color
		if rank_code == 0 then
			sd.value = self.player.cup
		elseif rank_code == 1 then
			sd.value = self.player.achieve_point
		elseif rank_code == 2 then
			sd.value = self.player.max_score
		elseif rank_code == 3 then
			sd.value = self.player.max_sha
		elseif rank_code == 4 then
			sd.value = self.player.max_lsha
		end
	end
	
	if rank_code == 0 then
		RankPanel.InitJBRank(self_info_,sd,rank_num,is_friend)
	else
		RankPanel.InitOtherRank(self_info_,sd,rank_num,rank_code,is_friend)
	end
end

function RankPanel.InitJBRank(obj,info,rank_num,is_friend)
	local bg = obj.transform:Find('bg')
	lua_script_:AddButtonEvent(bg.gameObject, "click", RankPanel.Click)		
	
	if rank_num == nil then
		obj.transform:Find('rank/icon').gameObject:SetActive(false)
		obj.transform:Find('rank/lb'):GetComponent('UILabel').text = Config.get_t_script_str('RankPanel_007') --'未上榜'
		obj.transform:Find('rank/lb').gameObject:SetActive(true)
	else
		if rank_num <= 3 then
			if rank_num == 1 then
				obj.transform:Find('rank/icon'):GetComponent('UISprite').spriteName = 'mc_one'
			elseif rank_num == 2 then
				obj.transform:Find('rank/icon'):GetComponent('UISprite').spriteName = 'mc_two'
			elseif rank_num == 3 then
				obj.transform:Find('rank/icon'):GetComponent('UISprite').spriteName = 'mc_three'
			end
			obj.transform:Find('rank/icon').gameObject:SetActive(true)
			obj.transform:Find('rank/lb').gameObject:SetActive(false)
		else
			obj.transform:Find('rank/icon').gameObject:SetActive(false)
			obj.transform:Find('rank/lb'):GetComponent('UILabel').text = rank_num
			obj.transform:Find('rank/lb').gameObject:SetActive(true)
		end
	end
	
	obj.transform:Find('player_info/icon/lb'):GetComponent('UILabel').text = info.level
	
	local name_lb = obj.transform:Find('player_info/name'):GetComponent('UILabel')
	name_lb.text = info.name
	IconPanel.InitVipLabel(name_lb, info.name_color)
	
	print('region_id',info.region_id)
	if info.region_id == nil then
		obj.transform:Find('area'):GetComponent('UISprite').spriteName = Config.get_t_foregion(0).icon
	else
		obj.transform:Find('area'):GetComponent('UISprite').spriteName = Config.get_t_foregion(info.region_id).icon
	end
	
	local tx = obj.transform:Find('tx');
	if tx.childCount > 0 then
		for i = tx.childCount - 1,0,-1 do
			GameObject.Destroy(tx:GetChild(i).gameObject)
		end
	end
    
	local avatar_t = AvaIconPanel.GetAvatarSex("social_res",RankPanel.OpenZonePanel, info.avatar,'', info.toukuang, info.sex)
	avatar_t.transform.parent = tx
	avatar_t.transform.localPosition = Vector3.zero
	avatar_t.transform.localScale = Vector3.one
	avatar_t:SetActive(true)
	local frame = avatar_t.transform:Find('frame'):GetComponent('UISprite')
	local frame_icon = avatar_t.transform:Find('avatar'):GetComponent('UISprite')
	avatar_t.transform:Find("avatar").name = info.player_guid
	frame.width = 100
	frame.height = 100
	frame_icon.width = 64
	frame_icon.height = 64
	
	
	local value = 0
	if is_friend then
		value = info.cup
	else
		value = info.value
	end
	
	local cup_btn_ = obj.transform:Find('cup_btn')
	cup_btn_.gameObject:SetActive(true)
	
	local cup_temp = Config.get_t_cup(value)
	cup_btn_:GetComponent("UISprite").atlas = IconPanel.GetAltas(cup_temp.icon)
	cup_btn_:GetComponent("UISprite").spriteName = cup_temp.icon
	local cup_name = cup_btn_:Find("name"):GetComponent("UILabel")
	local cup_num = cup_btn_:Find("num"):GetComponent("UILabel")
	local star_root = cup_btn_:Find("star_root")
	cup_name.text = cup_temp.name
	cup_num.text = ''
	IconPanel.InitCupLabel(cup_temp.dai_icon, cup_name)
	for i = 0, star_root.childCount - 1 do
		star_root:GetChild(i):GetComponent("UISprite").spriteName = "star_icon1"
        star_root:GetChild(i).gameObject:SetActive(false)
    end
	
	if(cup_temp.id == value) then
		for i = 1, cup_temp.max_star do
			star_root:Find(tostring(i)).gameObject:SetActive(true)
			if(cup_temp.star < i) then
				star_root:Find(tostring(i)):GetComponent("UISprite").spriteName = "star_bg01"
			end
		end
	else
		star_root:Find("1").gameObject:SetActive(true)
		cup_num.text = (cup_temp.star + value - cup_temp.id)
		IconPanel.InitCupLabel(cup_temp.dai_icon ,cup_num)
	end
	
	if is_friend then
		obj.transform:Find('add_friend').gameObject:SetActive(false)
		
		if self.player.guid == info.player_guid then
			obj.transform:Find('gold').gameObject:SetActive(false)
		else
			if self.send_social_golds(info.player_guid) then
				obj.transform:Find('gold').gameObject:SetActive(false)
			else
				lua_script_:AddButtonEvent(obj.transform:Find('gold').gameObject, "click", RankPanel.GiveGold)		
				obj.transform:Find('gold').gameObject:SetActive(true)
				obj.transform:Find('gold').name = info.player_guid
			end
		end
	else
		obj.transform:Find('gold').gameObject:SetActive(false)
		
		if self.player.guid == info.player_guid or self.social_type(info.player_guid) == 2 then
			obj.transform:Find('add_friend').gameObject:SetActive(false)
		else
			lua_script_:AddButtonEvent(obj.transform:Find('add_friend').gameObject, "click", RankPanel.AddFriend)
			obj.transform:Find('add_friend').gameObject:SetActive(true)
			obj.transform:Find('add_friend').gameObject.name = info.player_guid
		end
	end
	
	if obj.transform:Find('bg') ~= nil then
		lua_script_:AddButtonEvent(obj.transform:Find('bg').gameObject,'drag',RankPanel.Drag)
	end
	obj.transform:Find('final_lb').gameObject:SetActive(false)
	return obj
end

function RankPanel.OpenZonePanel(obj)
	local guid = obj.name
	if(self.player.guid == guid) then
		GUIRoot.HideGUI("RankPanel",false)
		GUIRoot.ShowGUI("ZonePanel", {self.player, self.battle_results,2})
	else
		local msg = msg_hall_pb.cmsg_player_look()
		msg.target_guid = guid
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_PLAYER_LOOK, data, {opcodes.SMSG_PLAYER_LOOK})
	end
end

function RankPanel.Drag()
	if help_lb_.gameObject.activeInHierarchy then
		help_lb_.gameObject:SetActive(false)
	end
	local is_end = NavUtil.IsScrollviewEnd(rank_root_.gameObject)
	if not is_end then
		return
	end
	
	local area_code = RankPanel.GetAreaCode()
	local rank_code = RankPanel.GetRankCode()
	
	local infos = nil
	local is_friend = false
	if area_code == 'world' then
		infos = world_ranks_[rank_code]
	else
		infos = friend_ranks[rank_code]
		is_friend = true
	end
	
	if loading_num >= #infos then
		return
	end
	
	local count = 0
	for i = loading_num + 1,#infos do
		if count > 20 then
			break
		end
		
		local obj = LuaHelper.Instantiate(rank_item_)
		if rank_code == 0 then
			RankPanel.InitJBRank(obj,infos[i],i,is_friend)
		else
			RankPanel.InitOtherRank(obj,infos[i],i,rank_code,is_friend)
		end
		obj.transform.parent = rank_root_.transform
		obj.transform.localPosition = Vector3(offset_x_,offset_y_ - 105 * (i - 1),0)
		obj.transform.localScale = Vector3.one
		obj:SetActive(true)
		count = count + 1
	end
	loading_num = loading_num + count
end

function RankPanel.InitOtherRank(obj,info,rank_num,rank_code,is_friend)
	local bg = obj.transform:Find('bg')
	lua_script_:AddButtonEvent(bg.gameObject, "click", RankPanel.Click)	
	
	if rank_num == nil then
		obj.transform:Find('rank/icon').gameObject:SetActive(false)
		obj.transform:Find('rank/lb'):GetComponent('UILabel').text = Config.get_t_script_str('RankPanel_007') --'未上榜'
		obj.transform:Find('rank/lb').gameObject:SetActive(true)
	else
		if rank_num <= 3 then
			if rank_num == 1 then
				obj.transform:Find('rank/icon'):GetComponent('UISprite').spriteName = 'mc_one'
			elseif rank_num == 2 then
				obj.transform:Find('rank/icon'):GetComponent('UISprite').spriteName = 'mc_two'
			elseif rank_num == 3 then
				obj.transform:Find('rank/icon'):GetComponent('UISprite').spriteName = 'mc_three'
			end
			obj.transform:Find('rank/icon').gameObject:SetActive(true)
			obj.transform:Find('rank/lb').gameObject:SetActive(false)
		else
			obj.transform:Find('rank/icon').gameObject:SetActive(false)
			obj.transform:Find('rank/lb'):GetComponent('UILabel').text = rank_num
			obj.transform:Find('rank/lb').gameObject:SetActive(true)
		end
	end
	
	obj.transform:Find('player_info/icon/lb'):GetComponent('UILabel').text = info.level
	
	local name_lb = obj.transform:Find('player_info/name'):GetComponent('UILabel')
	name_lb.text = info.name
	IconPanel.InitVipLabel(name_lb, info.name_color)
	
	print('region_id',info.region_id)
	if info.region_id == nil then
		obj.transform:Find('area'):GetComponent('UISprite').spriteName = Config.get_t_foregion(0).icon
	else
		obj.transform:Find('area'):GetComponent('UISprite').spriteName = Config.get_t_foregion(info.region_id).icon
	end
	
	local tx = obj.transform:Find('tx');
	if tx.childCount > 0 then
		for i = tx.childCount - 1,0,-1 do
			GameObject.Destroy(tx:GetChild(i).gameObject)
		end
	end

    local avatar_t = AvaIconPanel.GetAvatarSex("social_res",RankPanel.OpenZonePanel, info.avatar,'', info.toukuang, info.sex)
	avatar_t.transform.parent = tx
	avatar_t.transform.localPosition = Vector3.zero
	avatar_t.transform.localScale = Vector3.one
	avatar_t:SetActive(true)
	local frame = avatar_t.transform:Find('frame'):GetComponent('UISprite')
	local frame_icon = avatar_t.transform:Find('avatar'):GetComponent('UISprite')
	avatar_t.transform:Find("avatar").name = info.player_guid
	frame.width = 100
	frame.height = 100
	frame_icon.width = 64
	frame_icon.height = 64
	
	
	obj.transform:Find('cup_btn').gameObject:SetActive(false)
	
	if is_friend then
		obj.transform:Find('add_friend').gameObject:SetActive(false)	
		if self.player.guid == info.player_guid then
			obj.transform:Find('gold').gameObject:SetActive(false)
		else
			if self.send_social_golds(info.player_guid) then
				obj.transform:Find('gold').gameObject:SetActive(false)
			else
				lua_script_:AddButtonEvent(obj.transform:Find('gold').gameObject, "click", RankPanel.GiveGold)		
				obj.transform:Find('gold').gameObject:SetActive(true)
				obj.transform:Find('gold').name = info.player_guid
			end
		end
	else
		obj.transform:Find('gold').gameObject:SetActive(false)		
		if self.player.guid == info.player_guid or self.social_type(info.player_guid) == 2 then
			obj.transform:Find('add_friend').gameObject:SetActive(false)
		else
			lua_script_:AddButtonEvent(obj.transform:Find('add_friend').gameObject, "click", RankPanel.AddFriend)
			obj.transform:Find('add_friend').gameObject:SetActive(true)
			obj.transform:Find('add_friend').gameObject.name = info.player_guid
		end
	end
	
	local value = 0
	if is_friend then
		if rank_code == 0 then
			value = info.cup
		elseif rank_code == 1 then
			value = info.achieve_point
		elseif rank_code == 2 then
			value = info.max_score
		elseif rank_code == 3 then
			value = info.max_sha
		elseif rank_code == 4 then
			value = info.max_lsha
		end
	else
		value = info.value
	end
	
	if obj.transform:Find('bg') ~= nil then
		lua_script_:AddButtonEvent(obj.transform:Find('bg').gameObject,'drag',RankPanel.Drag)
	end
	
	obj.transform:Find('final_lb'):GetComponent('UILabel').text = value
	obj.transform:Find('final_lb').gameObject:SetActive(true)
	return obj
end

function RankPanel.Back()
	if(not GUIRoot.HasGUI("ZonePanel")) then
		GUIRoot.HideGUI('RankPanel')
		GUIRoot.HideGUI('BackPanel')
		GUIRoot.ShowGUI('HallPanel')
	end
end
-----------------------------------------------------服务端信息---------------------------------------------------------
function RankPanel.SMSG_PLAYER_LOOK(message)
	local msg = msg_hall_pb.smsg_player_look()
	msg:ParseFromString(message.luabuff)	
	GUIRoot.ShowGUI("ZonePanel", {msg.player, msg.battle_his, 2})
	GUIRoot.HideGUI("RankPanel", false)
end

function RankPanel.Recharge()
	GUIRoot.HideGUI('RankPanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function RankPanel.GiveGold(obj)
	if(#self.player.social_golds >= 10) then
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('RankPanel_008')})
	else
		if self.send_social_golds(obj.name) then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('RankPanel_009')})
			return
		end
		to_gift_gold_obj_ = obj
		
		local msg = msg_social_pb.cmsg_social_gift()
		msg.target_guid = obj.name	
		msg.gold = 5
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_SOCIAL_GIFT, data, {opcodes.SMSG_SOCIAL_GIFT})
	end
end

function RankPanel.SMSG_SOCIAL_GIFT()
	if(to_gift_gold_obj_ ~= nil) then
		self.player.social_golds:append(to_gift_gold_obj_.name)
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('RankPanel_010')})
		to_gift_gold_obj_:SetActive(false)
	end
end

function RankPanel.AddFriend(obj)
	local msg = msg_social_pb.cmsg_social_apply()
	msg.target_guid = obj.name
	msg.verify = ''
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_SOCIAL_APPLY, data, {opcodes.SMSG_SOCIAL_APPLY})
end

function RankPanel.SMSG_SOCIAL_APPLY()
	GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('RankPanel_011')})
	self.del_social_guid(3, add_guid_)
end

local world_rank_code_
function RankPanel.CMSG_RANK(code)
	local msg = msg_rank_pb.cmsg_rank()
	msg.id = code
	world_rank_code_ = code
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_RANK,data,{opcodes.SMSG_RANK})
end

function RankPanel.SMSG_RANK(message)
	local msg = msg_rank_pb.smsg_rank()
	msg:ParseFromString(message.luabuff)

	local d = RankPanel.ParseRank(msg.rank)
	world_ranks_[world_rank_code_] = d
	
	local area_code = RankPanel.GetAreaCode()
	local rank_code = RankPanel.GetRankCode()
	RankPanel.InitRankInfo(rank_code,area_code)
end

function RankPanel.IsContainChinese(str)
	for i = 1,#str do
		if string.byte(str,i) > 127 then
			return true
		end
	end
	return false
end

function RankPanel.ParseRank(msg)
	local list = {}	
	for i = 1,#msg.name do
		local item = {}
		item.player_guid = msg.player_guid[i]
		item.name = msg.name[i]
		item.sex = msg.sex[i]
		item.level = msg.level[i]
		item.avatar = msg.avatar[i]
		item.toukuang = msg.toukuang[i]
		
		if msg.region_id[i] == nil or msg.region_id[i] == 0 then
			item.region_id = 0 --'未知'
		else
			item.region_id = msg.region_id[i]
		end
		
		item.name_color = msg.name_color[i]
		item.value = msg.value[i]
		table.insert(list,item)
	end
	return list
end

function RankPanel.ParseFriendInfo(msg)
	local list = {}
	for i = 1,#msg do
		local item = {}
		item.player_guid = msg[i].target_guid
		item.name = msg[i].name
		item.sex = msg[i].sex
		item.level = msg[i].level
		item.avatar = msg[i].avatar
		item.toukuang = msg[i].toukuang
		
		if msg[i].region_id == nil or msg[i].region_id == 0 then
			item.region_id = 0 --'未知'
		else
			item.region_id = msg[i].region_id
		end
	
		item.name_color = msg[i].name_color
		item.cup = msg[i].cup
		item.achieve_point = msg[i].achieve_point
		item.max_score = msg[i].max_score
		item.max_sha = msg[i].max_sha
		item.max_lsha = msg[i].max_lsha
		table.insert(list,item)
	end
	--add self
	local item = {}
	item.player_guid = self.player.guid
	item.name = self.player.name
	item.sex = self.player.sex
	item.level = self.player.level
	item.avatar = self.player.avatar_on
	item.toukuang = self.player.toukuang_on
	
	if self.player.region_id == nil or self.player.region_id == 0 then
		item.region_id = 0 --'未知'
	else
		item.region_id = self.player.region_id
	end
	
	local name_color = NoticePanel.GetVipNameColor(self.player)
	item.name_color = name_color	
	item.cup = self.player.cup
	item.achieve_point = self.player.achieve_point
	item.max_score = self.player.max_score
	item.max_sha = self.player.max_sha
	item.max_lsha = self.player.max_lsha
	table.insert(list,item)
	return list
end

function RankPanel.GetFriendsInfo(code)
	local msg = msg_social_pb.cmsg_social_look()
	msg.type = 2
	world_rank_code_ = code
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_SOCIAL_LOOK, data)
end

function RankPanel.SMSG_SOCAIL_LOOK(message)
	local msg = msg_social_pb.smsg_social_look()
	msg:ParseFromString(message.luabuff)
	
	local d = RankPanel.ParseFriendInfo(msg.socials)
	RankPanel.SortFriendRank(d)
	
	local area_code = RankPanel.GetAreaCode()
	local rank_code = RankPanel.GetRankCode()
	RankPanel.InitRankInfo(rank_code,area_code)
end

function RankPanel.SortFriendRank(msg)
	--奖杯榜
	local jb_list = {}
	for i = 1,#msg do
		table.insert(jb_list,msg[i])
	end
	local comp = function(a,b)
		return a.cup > b.cup
	end
	table.sort(jb_list,comp)
	friend_ranks[0] = jb_list
	
	--成就榜
	local cj_list = {}
	for i = 1,#msg do
		table.insert(cj_list,msg[i])
	end
	local comp = function(a,b)
		return a.achieve_point > b.achieve_point
	end
	table.sort(cj_list,comp)
	friend_ranks[1] = cj_list
	
	--高分榜
	local gc_list = {}
	for i = 1,#msg do
		table.insert(gc_list,msg[i])
	end
	local comp = function(a,b)
		return a.max_score > b.max_score
	end
	table.sort(gc_list,comp)
	friend_ranks[2] = gc_list
	
	--破敌榜
	local pd_list = {}
	for i = 1,#msg do
		table.insert(pd_list,msg[i])
	end
	local comp = function(a,b)
		return a.max_sha > b.max_sha
	end
	table.sort(pd_list,comp)
	friend_ranks[3] = pd_list
	
	--连破榜
	local lp_list = {}
	for i = 1,#msg do
		table.insert(lp_list,msg[i])
	end
	local comp = function(a,b)
		return a.max_lsha > b.max_lsha
	end
	table.sort(lp_list,comp)
	friend_ranks[4] = lp_list
end