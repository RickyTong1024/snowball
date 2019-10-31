AvatarPanel = {}

local lua_script_

local avatar_state = {
	lock = 1,
	have = 2,
	equip = 3,
}

local avatars = {}

local detail_panel_
local avatar_panel_
local frame_panel_
local title_label_

local avatar_btn_
local frame_btn_

local avatar_view

local panel_state_ = 0
local select_avatar_id = 0
local pre_select_ava_

local avatar_pos = Vector3(-270, 130, 0)
local view_softness = Vector2(0, 10)
local view_pos = Vector3.one
local view_offset = Vector2(0, 0)
local space_x_ = 110
local space_y_ = 110

function AvatarPanel.Awake(obj)
	GUIRoot.UIEffect(obj, 0)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	avatar_panel_ = obj.transform:Find('avatar_panel')
	frame_panel_ = obj.transform:Find('frame_panel')
	detail_panel_ = obj.transform:Find('desc_panel')
	
	
	avatar_view = obj.transform:Find('avatar_view')
	title_label_ = obj.transform:Find('baseboard/Label'):GetComponent("UILabel")
	
	view_pos = avatar_view.localPosition
	view_offset = avatar_view:GetComponent("UIPanel").clipOffset
	avatar_view:GetComponent("UIPanel").clipSoftness = view_softness
	
	avatar_btn_ = obj.transform:Find('left_panel/avatar_btn')
	frame_btn_ = obj.transform:Find('left_panel/frame_btn')
	local equip_avatar_btn = detail_panel_:Find('equip_btn')
	
	lua_script_:AddButtonEvent(avatar_btn_.gameObject, "click", AvatarPanel.Click)
	lua_script_:AddButtonEvent(frame_btn_.gameObject, "click", AvatarPanel.Click)
	lua_script_:AddButtonEvent(equip_avatar_btn.gameObject, "click", AvatarPanel.EquipAvatar)
	
	AvatarPanel.RegisterMessage()
	AvatarPanel.SelectPage(0)
end

function AvatarPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_AVATAR_ON, AvatarPanel.SMSG_AVATAR_ON)
	Message.register_net_handle(opcodes.SMSG_TOUKUANG_ON, AvatarPanel.SMSG_TOUKUANG_ON)
	Message.register_handle("back_panel_msg", AvatarPanel.Back)
	Message.register_handle("back_panel_recharge", AvatarPanel.Recharge)
	Message.register_handle("team_join_msg", AvatarPanel.TeamJoin)
end

function AvatarPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_AVATAR_ON, AvatarPanel.SMSG_AVATAR_ON)
	Message.remove_net_handle(opcodes.SMSG_TOUKUANG_ON, AvatarPanel.SMSG_TOUKUANG_ON)
	Message.remove_handle("back_panel_msg", AvatarPanel.Back)
	Message.remove_handle("back_panel_recharge", AvatarPanel.Recharge)
	Message.remove_handle("team_join_msg", AvatarPanel.TeamJoin)
end

function AvatarPanel.OnDestroy()
	panel_state_ = 0
	select_avatar_id = 0
	pre_select_ava_ = nil
	avatars = {}
	AvatarPanel.RemoveMessage()
end

function AvatarPanel.Back()
	GUIRoot.HideGUI('AvatarPanel')
	GUIRoot.ShowGUI("ZonePanel", {self.player, self.battle_results})
end

function AvatarPanel.Recharge()
	GUIRoot.HideGUI('AvatarPanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function AvatarPanel.TeamJoin()
	GUIRoot.HideGUI('AvatarPanel')
end

---------------------列表刷新-------------------

function AvatarPanel.ClearView()
	if(avatar_view.childCount > 0) then
		for i = 0 , avatar_view.childCount - 1 do
			GameObject.Destroy (avatar_view:GetChild(i).gameObject)
		end
		if avatar_view:GetComponent('SpringPanel') ~= nil then
			avatar_view:GetComponent('SpringPanel').enabled = false
		end
		avatar_view:GetComponent("UIPanel").clipOffset = view_offset
		avatar_view.localPosition = view_pos
		pre_select_ava_ = nil
	end
end

function AvatarPanel.InitAvatarList()
	local avatar_res = avatar_panel_:Find('avatar_res')
	AvatarPanel.ClearView()
	
	local avatars = {}
	for i = 1, #Config.t_avatar_ids do
		local avatar_temp = Config.get_t_avatar(Config.t_avatar_ids[i])
		if avatar_temp.role_id >= 1000 and avatar_temp.lock ~= 1 then
			table.insert(avatars,avatar_temp)
		end
	end	
	
	for i = 1, #avatars do
		local avatar_temp = avatars[i]
		local avatar = AvaIconPanel.GetAvatar("avatar_res_mask", AvatarPanel.SelectAvatar, avatar_temp.id, "", 1)
		if(self.has_avatar(avatar_temp.id)) then
			avatar.transform:Find('mask').gameObject:SetActive(false)
		end
		if(pre_select_ava_ == nil) then
			pre_select_ava_ = avatar.transform:Find("avatar").gameObject
		end
		if(select_avatar_id == avatar_temp.id) then
			pre_select_ava_ = avatar.transform:Find("avatar").gameObject
		end
		if(self.player.avatar_on == avatar_temp.id) then
			avatar.transform:Find("tip").gameObject:SetActive(true)
		end
		avatar.transform:Find("avatar").name = avatar_temp.id
		avatar.transform.parent = avatar_view
		avatar.transform.localPosition = Vector3((i - 1) % 4 * space_x_, -(math.floor((i - 1) / 4) * space_y_), 0) + avatar_pos
		avatar.transform.localScale = Vector3.one
		avatar:SetActive(true)
	end
	AvatarPanel.SelectAvatar(pre_select_ava_)
	avatar_panel_.gameObject:SetActive(true)
end

function AvatarPanel.InitFrameList()
	local frame_res = frame_panel_:Find('frame_res')
	AvatarPanel.ClearView()
	local show = {}
	for i = 1,#Config.t_toukuang_ids do
		local frame_temp = Config.get_t_toukuang(Config.t_toukuang_ids[i])
		if frame_temp.lock ~= 1 then
			table.insert(show,frame_temp)
		end
	end
	
	for i = 1, #show do
		local frame_temp = show[i]
		if frame_temp.lock ~= 1 then
			local frame = LuaHelper.Instantiate(frame_res.gameObject)
			if(self.has_toukuang(frame_temp.id)) then
				frame.transform:Find('lock').gameObject:SetActive(false)
			end
			if(pre_select_ava_ == nil) then
				pre_select_ava_ = frame
			end
			if(select_avatar_id == frame_temp.id) then
				pre_select_ava_ = frame
			end
			if(self.player.toukuang_on == frame_temp.id) then
				frame.transform:Find("tip").gameObject:SetActive(true)
			end
			frame.name = frame_temp.id
			frame.transform:GetComponent('UISprite').spriteName = frame_temp.icon
			frame.transform:GetComponent('UISprite').atlas = IconPanel.GetAltas(frame_temp.icon)
			frame.transform:GetComponent('UISprite'):MakePixelPerfect()
			frame.transform.parent = avatar_view
			frame.transform.localPosition = Vector3((i - 1) % 4 * space_x_, -(math.floor((i - 1) / 4) * space_y_), 0) + avatar_pos
			frame.transform.localScale = Vector3.one
			lua_script_:AddButtonEvent(frame, "click", AvatarPanel.SelectFrame)
			frame:SetActive(true)
		end
	end
	AvatarPanel.SelectFrame(pre_select_ava_)
	frame_panel_.gameObject:SetActive(true)
end

function AvatarPanel.InitDetailPanel(avatar_temp, type)
	local avatar_icon = detail_panel_:Find('avatar_icon')
	local frame_icon = detail_panel_:Find('frame_icon')
	local equip_btn = detail_panel_:Find('equip_btn'):GetComponent("UISprite")
	local btn_text = detail_panel_:Find('equip_btn/Label'):GetComponent('UILabel')
	local name = detail_panel_:Find('name'):GetComponent('UILabel')
	avatar_icon.gameObject:SetActive(false)
	frame_icon.gameObject:SetActive(false)
	local icon = nil
	local desc = detail_panel_:Find('desc'):GetComponent('UILabel')
	local gain_desc = detail_panel_:Find('gain_desc'):GetComponent('UILabel')
	local state = 0
	if(type == 1) then
		icon = avatar_icon:GetComponent('UISprite')
		avatar_icon.gameObject:SetActive(true)
		if(self.has_avatar(avatar_temp.id)) then
			if(avatar_temp.id == self.player.avatar_on) then
				state = avatar_state.equip
			else
				state = avatar_state.have
			end
		else
			state = avatar_state.lock
		end
	else
		icon = frame_icon:GetComponent('UISprite')
		frame_icon.gameObject:SetActive(true)
		if(self.has_toukuang(avatar_temp.id)) then
			if(avatar_temp.id == self.player.toukuang_on) then
				state = avatar_state.equip
			else
				state = avatar_state.have
			end
		else
			state = avatar_state.lock
		end
	end
	equip_btn.spriteName = "b1_gray"
	gain_desc.text = ""
	if(state == avatar_state.equip) then
		btn_text.text = Config.get_t_script_str('AvatarPanel_001') --"使用中"
	elseif(state == avatar_state.lock) then
		btn_text.text = Config.get_t_script_str('AvatarPanel_002') --"未解锁"
		gain_desc.text = avatar_temp.desc2
	elseif(state == avatar_state.have) then
		btn_text.text = Config.get_t_script_str('AvatarPanel_003') --"使用"
		equip_btn.spriteName = "b1"
	end
	if(avatar_temp.id == 201 and tonumber(self.player.yue_time) > tonumber(timerMgr:now_string())) then
		local time = tonumber(self.player.yue_time) - tonumber(timerMgr:now_string())
		gain_desc.text = Config.get_t_script_str('AvatarPanel_004')..count_time_day(time, 1)  --"[e4ac01]剩余时间："
	end
	if((avatar_temp.id == 202 or avatar_temp.id == 201) and tonumber(self.player.nian_time) > tonumber(timerMgr:now_string())) then
		local time = tonumber(self.player.nian_time) - tonumber(timerMgr:now_string())
		gain_desc.text = Config.get_t_script_str('AvatarPanel_004')..count_time_day(time, 1)  --"[e4ac01]剩余时间："
	end
	desc.text = avatar_temp.desc1
	name.text = avatar_temp.name
	IconPanel.InitQualityLabel(name, avatar_temp.color)
	icon.atlas = IconPanel.GetAltas(avatar_temp.icon)
	icon.spriteName = avatar_temp.icon
	if(type == 2) then
		icon:MakePixelPerfect()
	end
	detail_panel_.gameObject:SetActive(true)
end

-------------------------------------------------


--------------------ButtonEvent-------------------

function AvatarPanel.Click(obj)
	if(obj.name == 'avatar_btn') then
		AvatarPanel.SelectPage(0)
	elseif(obj.name == 'frame_btn') then
		AvatarPanel.SelectPage(1) 
	end
end

function AvatarPanel.SelectPage(page)
	avatar_panel_.gameObject:SetActive(false)
	frame_panel_.gameObject:SetActive(false)
	detail_panel_.gameObject:SetActive(false)
	avatar_btn_:Find("highlight").gameObject:SetActive(false)
	frame_btn_:Find("highlight").gameObject:SetActive(false)
	avatar_btn_:Find("Label"):GetComponent("UILabel").text = "[7E90A4]"..Config.get_t_script_str('AvatarPanel_005')
	frame_btn_:Find("Label"):GetComponent("UILabel").text = "[7E90A4]"..Config.get_t_script_str('AvatarPanel_006')
	pre_select_ava_ = nil
	select_avatar_id = 0
	if(page == 0) then
		AvatarPanel.InitAvatarList()
		title_label_.text = Config.get_t_script_str('AvatarPanel_005') --"头像"
		avatar_btn_:Find("highlight").gameObject:SetActive(true)
		avatar_btn_:Find("Label"):GetComponent("UILabel").text = "[E8FCFF]"..Config.get_t_script_str('AvatarPanel_005')
	elseif(page == 1) then
		AvatarPanel.InitFrameList()
		title_label_.text = Config.get_t_script_str('AvatarPanel_006') --"头像框"
		frame_btn_:Find("highlight").gameObject:SetActive(true)
		frame_btn_:Find("Label"):GetComponent("UILabel").text = "[E8FCFF]"..Config.get_t_script_str('AvatarPanel_006')
	end
end

function AvatarPanel.SelectAvatar(obj)
	if(pre_select_ava_ ~= nil) then
		pre_select_ava_.transform.parent:Find("select").gameObject:SetActive(false)
	end
	pre_select_ava_ = obj
	obj.transform.parent:Find("select").gameObject:SetActive(true)
	panel_state_ = 1
	select_avatar_id = tonumber(obj.name)
	local avatar_temp = Config.get_t_avatar(select_avatar_id)
	AvatarPanel.InitDetailPanel(avatar_temp, 1)
end

function AvatarPanel.SelectFrame(obj)
	if(pre_select_ava_ ~= nil) then
		pre_select_ava_.transform:Find("select").gameObject:SetActive(false)
	end
	pre_select_ava_ = obj
	obj.transform:Find("select").gameObject:SetActive(true)
	panel_state_ = 2
	select_avatar_id = tonumber(obj.name)
	local avatar_temp = Config.get_t_toukuang(select_avatar_id)
	AvatarPanel.InitDetailPanel(avatar_temp, 2)
end

function AvatarPanel.EquipAvatar()
	if(panel_state_ == 1) then
		if(self.has_avatar(select_avatar_id)) then
			if(self.player.avatar_on == select_avatar_id) then
				GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('AvatarPanel_006')})
			else
				local msg = msg_hall_pb.cmsg_avatar_on()
				msg.id = select_avatar_id
				local data = msg:SerializeToString()
				GameTcp.Send(opcodes.CMSG_AVATAR_ON, data, {opcodes.SMSG_AVATAR_ON})
			end
		else
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('AvatarPanel_007')})
		end
	elseif(panel_state_ == 2) then
		if(self.has_toukuang(select_avatar_id)) then
			if(self.player.toukuang_on == select_avatar_id) then
				GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('AvatarPanel_008')})
			else
				local msg = msg_hall_pb.cmsg_toukuang_on()
				msg.id = select_avatar_id
				local data = msg:SerializeToString()
				GameTcp.Send(opcodes.CMSG_TOUKUANG_ON, data, {opcodes.SMSG_TOUKUANG_ON})
			end
		else
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('AvatarPanel_009')})
		end
	end
end

---------------------------------------------------

function AvatarPanel.SMSG_AVATAR_ON()
	if(panel_state_ == 1) then
		if(self.has_avatar(select_avatar_id)) then
			self.player.avatar_on = select_avatar_id
			AvatarPanel.InitAvatarList()
			local avatar_temp = Config.get_t_avatar(select_avatar_id)
			AvatarPanel.InitDetailPanel(avatar_temp, 1)
		end
	end
end

function AvatarPanel.SMSG_TOUKUANG_ON()
	if(panel_state_ == 2) then
		if(self.has_toukuang(select_avatar_id)) then
			self.player.toukuang_on = select_avatar_id
			AvatarPanel.InitFrameList()
			local frame_temp = Config.get_t_toukuang(select_avatar_id)
			AvatarPanel.InitDetailPanel(frame_temp, 2)
		end
	end
end