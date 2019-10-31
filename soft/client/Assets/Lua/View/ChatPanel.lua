ChatPanel = {}

local lua_script_

local show_msg_prefabs_ = {}
local msg_prefab_height = {}
local horn_msgs_ = {}

local main_panel
local main_bg
local horn_panel_
local express_panel_
local express_res_
local express_root_

local m_view
local chat_msg_res_
local sys_msg_res_
local horn_res_
local chat_input_
local horn_input_
local unread_btn_
local horn_num_
local input_panel_

local unread_msg = 0
local horn_time_ = 0
local move_speed_ = 300
local chat_limit_ = 20
local horn_limit_ = 30

local time_speed = 0.1

local avatar_pos = Vector3(-110, -29, 0)

local minh_ = 0
local maxh_ = 0
local viewh_ = 520
local line_ = 10
local pre_height = 0
local first_height = 0
local base_line_width = 250
local view_softness = Vector2(0, 10)
local chat_msg_pos = Vector3(0, 0, 0)
local space = '        '

local all_btn_,sys_btn_,channel_btn_
ChatPanel.wndClass = 1

function ChatPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	main_panel = obj.transform:Find("main_panel")
	main_bg = obj.transform:Find("bg")
	
	main_bg.gameObject:SetActive(true)
	GUIRoot.UIEffect(main_panel.gameObject, 3)
	
	horn_panel_ = main_panel:Find('horn_panel')
	express_panel_ = main_panel:Find('Anchor_bottom_left/express_panel')
	
	express_res_ = express_panel_:Find('express_res')
	express_root_ = express_panel_:Find('bg')
	chat_msg_res_ = main_panel:Find('Anchor_left/msg_panel/chat_view/chat_msg_res')
	sys_msg_res_ = main_panel:Find('Anchor_left/msg_panel/chat_view/sys_msg_res')
	
	m_view = main_panel:Find('Anchor_left/msg_panel/chat_view')
	horn_res_ = main_panel:Find('Anchor_left/msg_panel/horn_panel/horn_res')
	bg = main_panel:Find('Anchor_left/msg_panel/bg')
	
	m_view:GetComponent("UIScrollView").panel:SetRect(0, 0, 447, GUIRoot.height - 110)
	
	m_view:GetComponent("UIScrollView").panel.clipSoftness = view_softness
	viewh_ = GUIRoot.height - 110 - view_softness.y
	
	chat_msg_pos = Vector3(-50, viewh_ / 2 - 35, 0)
	
	input_panel_ = main_panel:Find('Anchor_bottom_left/input_panel').gameObject
	chat_input_ = main_panel:Find('Anchor_bottom_left/input_panel/chat_input'):GetComponent('UIInput')
	chat_input_.characterLimit = 60
	horn_input_ = horn_panel_:Find('horn_input'):GetComponent('UIInput')
	horn_input_.characterLimit = 60
	
	local send_btn = main_panel:Find('Anchor_bottom_left/input_panel/send_btn')
	local horn_btn = main_panel:Find('Anchor_bottom_left/input_panel/horn_btn')
	local horn_send_btn = horn_panel_:Find('horn_send_btn')
	local horn_close_btn = horn_panel_:Find('horn_close_btn')
	local close_btn = main_panel:Find('Anchor_left/msg_panel/close_btn')
	local express_btn = main_panel:Find('Anchor_bottom_left/input_panel/express_btn')
	unread_btn_ = main_panel:Find('Anchor_bottom_left/input_panel/unread_btn')
	horn_num_ = main_panel:Find('Anchor_bottom_left/input_panel/horn_btn/Label'):GetComponent('UILabel')
	
	all_btn_ = main_panel:Find('Anchor_top_left/top_left_panel/all_btn'):GetComponent('UISprite')
	sys_btn_ = main_panel:Find('Anchor_top_left/top_left_panel/sys_btn'):GetComponent('UISprite')
	channel_btn_ = main_panel:Find('Anchor_top_left/top_left_panel/channel_btn'):GetComponent('UISprite')
	ChatPanel.wndClass = 1
	
	lua_script_:AddButtonEvent(bg.gameObject, "drag", ChatPanel.OnDrag)
	lua_script_:AddButtonEvent(bg.gameObject, "press", ChatPanel.OnPress)
	lua_script_:AddButtonEvent(send_btn.gameObject, 'click', ChatPanel.Click)
	lua_script_:AddButtonEvent(horn_btn.gameObject, "click", ChatPanel.Click)
	lua_script_:AddButtonEvent(horn_send_btn.gameObject, "click", ChatPanel.Click)
	lua_script_:AddButtonEvent(horn_close_btn.gameObject, "click", ChatPanel.Click)
	lua_script_:AddButtonEvent(unread_btn_.gameObject, "click", ChatPanel.Click)
	lua_script_:AddButtonEvent(close_btn.gameObject, "click", ChatPanel.Click)
	lua_script_:AddButtonEvent(express_btn.gameObject, "click", ChatPanel.Click)
	lua_script_:AddButtonEvent(all_btn_.gameObject, "click", ChatPanel.Click)
	lua_script_:AddButtonEvent(sys_btn_.gameObject, "click", ChatPanel.Click)
	lua_script_:AddButtonEvent(channel_btn_.gameObject, "click", ChatPanel.Click)
	
	express_panel_.gameObject:SetActive(false)
	horn_panel_.gameObject:SetActive(false)
	chat_msg_res_.gameObject:SetActive(false)
	horn_res_.gameObject:SetActive(false)
	unread_btn_.gameObject:SetActive(false)
	
	timerMgr:AddRepeatTimer('ChatPanel', ChatPanel.Refresh , time_speed, time_speed)
	lua_script_:AddButtonEvent(m_view.gameObject ,'onDragFinished', ChatPanel.OnDragFinished)
	ChatPanel.RegisterMessage()
	ChatPanel.InitExpressPanel()
	ChatPanel.LoadChannel()
	ChatPanel.InitLabaNum()
end

function ChatPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_CHAT_LABA, ChatPanel.SMSG_CHAT_LABA)
	Message.register_net_handle(opcodes.SMSG_PLAYER_LOOK, ChatPanel.SMSG_PLAYER_LOOK)
	Message.register_handle("team_join_msg", ChatPanel.TeamJoin)
end

function ChatPanel.Remove_Message()
	Message.remove_net_handle(opcodes.SMSG_CHAT_LABA, ChatPanel.SMSG_CHAT_LABA)
	Message.remove_net_handle(opcodes.SMSG_PLAYER_LOOK, ChatPanel.SMSG_PLAYER_LOOK)
	Message.remove_handle("team_join_msg", ChatPanel.TeamJoin)
end

function ChatPanel.ClearInit()
	if show_msg_prefabs_ ~= nil then
		for i = 1,#show_msg_prefabs_ do
			GameObject.Destroy(show_msg_prefabs_[i])
		end
	end
	show_msg_prefabs_ = {}
	msg_prefab_height = {}
	pre_height = 0
	first_height = 0
	minh_ = 0
	maxh_ = 0
	unread_msg = 0
	
	if m_view:GetComponent('SpringPanel') ~= nil then
		m_view:GetComponent('SpringPanel').enabled = false
	end
	m_view:GetComponent("UIScrollView").panel:SetRect(0, 0, 447, GUIRoot.height - 110)
	m_view.localPosition = Vector3(223,0,0)
	m_view:GetComponent("UIScrollView").panel.clipOffset = Vector2.zero
	m_view:GetComponent("UIScrollView").panel.clipSoftness = view_softness
	viewh_ = GUIRoot.height - 110 - view_softness.y
end

function ChatPanel.OnDestroy()
	show_msg_prefabs_ = {}
	msg_prefab_height = {}
	horn_msgs_ = {}
	pre_height = 0
	first_height = 0
	lua_script_ = nil
	minh_ = 0
	maxh_ = 0
	unread_msg = 0
	horn_time_ = 0
	show_msg_prefabs_ = {}
	timerMgr:RemoveRepeatTimer('ChatPanel')
	ChatPanel.Remove_Message()
	ChatPanel.wndClass = 1
end

function ChatPanel.TeamJoin()
	GUIRoot.HideGUI("ChatPanel")
end

function ChatPanel.LoadChannel()
	ChatPanel.ClearInit()
	if ChatPanel.wndClass == 1 then
		all_btn_.spriteName =''
		channel_btn_.spriteName ='lt_haoyoudi'
		sys_btn_.spriteName ='lt_haoyoudi'
		ChatPanel.InitAllMsg()
	elseif ChatPanel.wndClass == 2 then
		all_btn_.spriteName ='lt_haoyoudi'
		channel_btn_.spriteName =''
		sys_btn_.spriteName ='lt_haoyoudi'
		ChatPanel.InitPdMsg()
	elseif ChatPanel.wndClass == 3 then
		all_btn_.spriteName ='lt_haoyoudi'
		channel_btn_.spriteName ='lt_haoyoudi'
		sys_btn_.spriteName =''
		ChatPanel.InitSysMsg()
	end
end

-------------------------聊天信息列表-------------------

function ChatPanel.InitAllMsg()
	if not input_panel_.activeInHierarchy then
		input_panel_:SetActive(true)
	end
	
	local chat_msg = NoticePanel.GetMsg()
	if(#chat_msg >= line_) then
		for i = 1, line_ do
			local msg = chat_msg[#chat_msg - line_ + i]
			if msg.type_id == 1 then
				ChatPanel.NewMesssage(msg.data)
			elseif msg.type_id == 2 then
				ChatPanel.NewSysMessage(msg.data)
			end
		end
	else
		for i = 1, #chat_msg do
			local msg = chat_msg[i]
			if msg.type_id == 1 then
				ChatPanel.NewMesssage(msg.data)
			elseif msg.type_id == 2 then
				ChatPanel.NewSysMessage(msg.data)
			end
		end
	end
end

function ChatPanel.InitSysMsg()
	local chat_msg = NoticePanel.GetMsgType(2)
	if express_panel_.gameObject.activeInHierarchy then
		express_panel_.gameObject:SetActive(false)
	end
	
	if input_panel_.activeInHierarchy then
		input_panel_:SetActive(false)
	end
	
	if(#chat_msg >= line_) then
		for i = 1, line_ do
			local msg = chat_msg[#chat_msg - line_ + i]
			ChatPanel.NewSysMessage(msg.data)
		end
	else
		for i = 1, #chat_msg do
			local msg = chat_msg[i]
			ChatPanel.NewSysMessage(msg.data)
		end
	end
end

function ChatPanel.InitPdMsg()
	if not input_panel_.activeInHierarchy then
		input_panel_:SetActive(true)
	end
	
	local chat_msg = NoticePanel.GetMsgType(1)
	if(#chat_msg >= line_) then
		for i = 1, line_ do
			local msg = chat_msg[#chat_msg - line_ + i]
			ChatPanel.NewMesssage(msg.data)
		end
	else
		for i = 1, #chat_msg do
			local msg = chat_msg[i]
			ChatPanel.NewMesssage(msg.data)
		end
	end
end

function ChatPanel.CheckLatestMsg()
	if m_view:GetComponent('SpringPanel') ~= nil then
		m_view:GetComponent('SpringPanel').enabled = false
	end	
	local uv = m_view:GetComponent('UIScrollView')
	local offy = uv.panel.clipOffset.y
	local tmaxh = maxh_ + offy
	if(tmaxh > viewh_) then
		uv.panel.clipOffset = Vector2(0, viewh_ - maxh_ - pre_height + first_height - 17)
		m_view.localPosition = Vector3(223, maxh_ - viewh_ + pre_height - first_height + 17, 0)
	end
	unread_msg = 0
	unread_btn_.gameObject:SetActive(false)
end


function ChatPanel.NewMesssage(msg)
	local chat_prefab = nil
	chat_prefab = LuaHelper.Instantiate(chat_msg_res_.gameObject)
	chat_prefab.transform.parent = m_view
	chat_prefab.transform.localScale = Vector3.one
	local avatar_res = AvaIconPanel.GetAvatarSex("avatar_res", ChatPanel.OpenZone, msg.avatar,'', msg.toukuang, msg.sex)
	avatar_res.transform.parent = chat_prefab.transform
	avatar_res.transform.localScale = Vector3.one
	avatar_res.transform.localPosition = avatar_pos
	avatar_res.transform:Find("avatar").name = msg.player_guid
	avatar_res:SetActive(true)
	chat_prefab.transform:Find('lv'):GetComponent('UILabel').text = msg.level
	chat_prefab.transform:Find('name'):GetComponent('UILabel').text = msg.player_name
	chat_prefab.transform:Find('region'):GetComponent('UISprite').spriteName = Config.get_t_foregion(msg.region_id).icon
	IconPanel.InitVipLabel(chat_prefab.transform:Find('name'):GetComponent('UILabel'), msg.name_color)
	local laba_tip = chat_prefab.transform:Find("tip")
	local text_label = chat_prefab.transform:Find('text'):GetComponent('UILabel')
	local last_start_index = 1
	local row = 0
	local text = {}
	local str = msg.text
	base_line_width = 250
	if(msg.type == 1) then
		text_label.transform.localPosition = text_label.transform.localPosition + Vector3(28, 0, 0)
		base_line_width = base_line_width - 28
		laba_tip.gameObject:SetActive(true)
	end
	text_label.overflowWidth = base_line_width
	text = stringTotable(msg.text)
	local count = #text
	local ex_num = 0
	for i = 1, count do
		if(text[i] == '[' and i + 7 <= #text and text[i + 1] == '#') then
			local ex_name = string.sub(str, i + 2, i + 6)
			local ex_id = tonumber(ex_name)
			local ex_pos = Vector3.zero
			local fx = 0
			for  j = 0, 7 do 
				table.remove(text, i)
			end
			table.insert(text, i, space)
			str = tableTostring(text)
			text = stringTotable(str)
			local text_width = NGUIText.CalculatePrintedWidth(text_label.trueTypeFont, text_label.fontSize, string.sub(str, last_start_index, i + 7))
			if(text_width  > base_line_width - 4) then
				ex_num = ex_num + 1
				fx = 16
				local space_num  = (base_line_width - text_width) / 4
				for  j = 1, space_num do 
					table.insert(text, i, " ")
				end
				count = count + space_num
				last_start_index = i + space_num
				row = row + 1
				ex_pos.x = fx
				ex_pos.y = -row * 28 - 7
				ex_pos.z = 0
			else 
				fx = text_width - 16
				if(ex_num > 0) then
					ex_pos.x = fx - row * 8
				else
					ex_pos.x = fx
				end
				ex_pos.y = -row * 28 - 7
				ex_pos.z = 0
			end
			local express = Config.get_t_biaoqing(ex_id)
			if(express ~= nil) then
				local express_t = LuaHelper.Instantiate(express_res_.gameObject)
				express_t.transform:GetComponent("Collider").enabled = false
				express_t.transform.parent = text_label.transform
				express_t.transform.localPosition = ex_pos
				express_t.transform.localScale = Vector3.one
				express_t.transform:GetComponent('UISprite').spriteName = express.icon
				express_t:SetActive(true)
			end
		else
			local cur_width = NGUIText.CalculatePrintedWidth(text_label.trueTypeFont, text_label.fontSize, string.sub(str, last_start_index, i))
			if(cur_width > base_line_width) then
				last_start_index = i
				row = row + 1
			end
		end
	end
	text_label.text = str
	text_label:ProcessText()
	local height = text_label.transform:GetComponent('UIWidget').height
	local width = text_label.transform:GetComponent('UIWidget').width
	if(msg.type == 1) then
		width = text_label.transform:GetComponent('UIWidget').width + 28
		laba_tip.localPosition = Vector3(-16, -16 - (height + 25) / 2, 0)
	end
	chat_prefab.transform:Find('bg'):GetComponent('UIWidget').height = height + 25
	chat_prefab.transform:Find('bg'):GetComponent('UIWidget').width = width + 40
	height = height + 56
	chat_prefab.transform:GetComponent('Collider').size = Vector3(447, height + 40, 0)
	chat_prefab.transform:GetComponent('Collider').center = Vector3(60, -40 - (height - 90) / 2, 0)
	if(pre_height == 0) then
		pre_height = height
		first_height = height
	end
	height = height + 30
	msg_prefab_height[chat_prefab] = pre_height
	-----------
	local uv = m_view:GetComponent('UIScrollView')
	local offy = uv.panel.clipOffset.y
	local pmaxh = maxh_
	maxh_ = maxh_ + pre_height
	local tpmaxh = pmaxh + offy
	local tmaxh = maxh_ + offy
	chat_prefab.transform.localPosition = Vector3(0, -(maxh_ - first_height), 0) + chat_msg_pos
	if tpmaxh <= viewh_ + 30 and tmaxh > viewh_ then
		--翻页
		if m_view:GetComponent('SpringPanel') ~= nil then
			m_view:GetComponent('SpringPanel').enabled = false
		end
		uv.panel.clipOffset = Vector2(0, viewh_ - maxh_- height + first_height - 17)
		m_view.localPosition = Vector3(223, maxh_ - viewh_ + height - first_height + 17, 0)
	end
	pre_height = height
	table.insert(show_msg_prefabs_, 1, chat_prefab)
	chat_prefab:SetActive(true)
	-----------
	if(#show_msg_prefabs_ > line_) then
		msg_prefab = show_msg_prefabs_[#show_msg_prefabs_]
		table.remove(show_msg_prefabs_, #show_msg_prefabs_)
		if(msg_prefab ~= nil) then
			GameObject.Destroy(msg_prefab)
		end
		minh_ = minh_ + msg_prefab_height[msg_prefab]
		local tminh = minh_ + offy
		if tminh >= 0 then
			if m_view:GetComponent('SpringPanel') ~= nil then
				m_view:GetComponent('SpringPanel').enabled = false
			end
			uv.panel.clipOffset = Vector2(0, -minh_)
			m_view.localPosition = Vector3(223, minh_, 0)
		end
	end
	-----------
end

function ChatPanel.NewSysMessage(message)
	local sys_prefab = nil
	sys_prefab = LuaHelper.Instantiate(sys_msg_res_.gameObject)
	sys_prefab.transform.parent = m_view
	sys_prefab.transform.localScale = Vector3.one
	sys_prefab.transform:GetComponent('UILabel').text = message.text
	
	local uiwight = sys_prefab.transform:GetComponent('UIWidget')
	sys_prefab.transform:Find('bg').localPosition = Vector3(0,0,0)
	sys_prefab.transform:Find('bg'):GetComponent('UIWidget').height = uiwight.height + 20

	local height = uiwight.height + 20
	if(pre_height == 0) then
		pre_height = height
		first_height = height
	end
	
	local uv = m_view:GetComponent('UIScrollView')
	local offy = uv.panel.clipOffset.y
	local pmaxh = maxh_
	maxh_ = maxh_ + pre_height
	local tpmaxh = pmaxh + offy
	local tmaxh = maxh_ + offy
	

	pre_height = height + 20
	sys_prefab.transform.localPosition= Vector3(0, -(maxh_ - first_height) + (viewh_ / 2 - 35), 0)
	msg_prefab_height[sys_prefab] = height
	table.insert(show_msg_prefabs_, 1, sys_prefab)
	sys_prefab:SetActive(true)
	
	sys_prefab.transform:Find('bg'):GetComponent('Collider').size = Vector3(447,height,0)
	sys_prefab.transform:Find('bg'):GetComponent('Collider').center = Vector3.zero
	
	if tpmaxh <= viewh_ + height and tmaxh > viewh_ then
		--翻页
		if m_view:GetComponent('SpringPanel') ~= nil then
			m_view:GetComponent('SpringPanel').enabled = false
		end
		uv.panel.clipOffset = Vector2(0, viewh_ - maxh_- height + first_height - 20)
		m_view.localPosition = Vector3(223, maxh_ - viewh_ + height - first_height + 20, 0)
	end

	if(#show_msg_prefabs_ > line_) then
		sys_prefab = show_msg_prefabs_[#show_msg_prefabs_]
		table.remove(show_msg_prefabs_, #show_msg_prefabs_)
		if(sys_prefab ~= nil) then
			GameObject.Destroy(sys_prefab)
		end
		minh_ = minh_ + msg_prefab_height[sys_prefab]
		local tminh = minh_ + offy
		if tminh >= 0 then
			if m_view:GetComponent('SpringPanel') ~= nil then
				m_view:GetComponent('SpringPanel').enabled = false
			end
			uv.panel.clipOffset = Vector2(0, -minh_)
			m_view.localPosition = Vector3(223, minh_, 0)
		end
	end
end

function ChatPanel.InitExpressPanel()
	local express_num = 0
	for k, v in pairsByKeys(Config.t_biaoqing) do
		express_num = express_num + 1
	end
	local express_width = express_res_:GetComponent('UIWidget').width
	if(express_num > 0) then
		local row = math.floor((express_num - 1) / 6 + 1)
		local height = (row - 1) * 50 + express_width + 18
		local width = 5 * 50 + express_width + 18
		express_root_:GetComponent('UIWidget').height = height
		express_root_:GetComponent('UIWidget').width = width
		local fir_pos_y = height - 9 - express_width / 2
		local fir_pos_x = express_width / 2 + 9
		local i = 0
		for k, v in pairsByKeys(Config.t_biaoqing) do
			local express = v
			local express_t = LuaHelper.Instantiate(express_res_.gameObject)
			express_t.transform.parent = express_root_
			express_t.transform.localPosition = Vector3(i % 6 * 50 + fir_pos_x, -(math.floor(i / 6) * 50) + fir_pos_y, 0)
			express_t.transform.localScale = Vector3.one
			express_t.transform:GetComponent('UISprite').spriteName = express.icon
			express_t.name = express.id
			lua_script_:AddButtonEvent(express_t, "click", ChatPanel.SelectExpress)
			express_t:SetActive(true)
			i = i + 1
		end
	end
end

function ChatPanel.InitLabaNum()
	if(lua_script_ ~= nil) then
		horn_num_.text = self.get_item_num(50010001)
		horn_panel_:Find("horn_num/Label"):GetComponent("UILabel").text = self.get_item_num(50010001)
	end
end
---------------------------------------------------------


---------------------------ButtonEvent-------------------

function ChatPanel.OnDragFinished()
	local uv = m_view:GetComponent('UIScrollView')
	local constraint = uv.panel:CalculateConstrainOffset(uv.bounds.min, uv.bounds.max)
	local offy = uv.panel.clipOffset.y
	if(offy < viewh_ - maxh_ + 50) then
		unread_msg = 0
		unread_btn_.gameObject:SetActive(false)
	end
end

function ChatPanel.Click(obj)
	if(obj.name == 'send_btn') then
		ChatPanel.SendMessage()
	elseif(obj.name == 'horn_btn') then
		ChatPanel.OpenHornPanel()
	elseif(obj.name == 'express_btn') then
		if(express_panel_.gameObject.activeInHierarchy) then
			express_panel_.gameObject:SetActive(false)
		else
			express_panel_.gameObject:SetActive(true)
		end
	elseif(obj.name == 'horn_send_btn') then
		ChatPanel.SendHornMessage()
	elseif(obj.name == 'horn_close_btn') then
		horn_panel_.gameObject:SetActive(false)
		horn_input_.value = ''
	elseif(obj.name == 'unread_btn') then
		ChatPanel.CheckLatestMsg()
	elseif(obj.name == 'close_btn') then
		GUIRoot.HideGUI("ChatPanel")
	elseif obj.name == 'all_btn' then
		ChatPanel.wndClass = 1
		ChatPanel.LoadChannel()
	elseif obj.name == 'channel_btn' then
		ChatPanel.wndClass = 2
		ChatPanel.LoadChannel()
	elseif obj.name == 'sys_btn' then
		ChatPanel.wndClass = 3
		ChatPanel.LoadChannel()
	end
end

function ChatPanel.SelectExpress(obj)
	local express_id = tonumber(obj.name)
	local text_label = chat_input_.transform:Find("Label"):GetComponent("UILabel")
	local text_width = NGUIText.CalculatePrintedWidth(text_label.trueTypeFont, text_label.fontSize, string.sub(chat_input_.value, 1, -1))
	if(text_width <= chat_limit_ * 16 - 32) then
		chat_input_.value = chat_input_.value..'[#'..obj.name..']'
	end
	express_panel_.gameObject:SetActive(false)
end

function ChatPanel.OpenHornPanel()
	horn_input_.value = ''
	horn_input_.transform:Find('Label'):GetComponent('UILabel').text = Config.get_t_script_str('ChatPanel_006')
	horn_panel_.gameObject:SetActive(true)
end

function ChatPanel.SendMessage()
	if(chat_input_.value ~= '') then
		if(string.find(chat_input_.value, "\n")) then
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ChatPanel_001')})
		else
			local msg = msg_hall_pb.cmsg_chat()
			msg.text = chat_input_.value
			if(string.len(chat_input_.value) > 60) then
				local str = str_sub(msg.text, 60)
				local str_tab = stringTotable(str)
				local pos = 0
				for i = #str_tab - 6, #str_tab do
					if(str_tab[i] == "[") then
						if(i == #str_tab) then
							table.remove(str_tab, i)
						elseif(str_tab[i + 1] == "#") then
							for k = 1, #str_tab - i + 1 do
								table.remove(str_tab, i)
							end
						end
					end
				end
				msg.text = tableTostring(str_tab)
			end
			msg.type = 0
			local data = msg:SerializeToString()
			GameTcp.Send(opcodes.CMSG_CHAT_CHANNEL, data)
		end
		chat_input_.value = ''
	else
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ChatPanel_002')})
	end
end

function ChatPanel.SendHornMessage()
	if(horn_input_.value ~= '') then
		if(self.get_item_num(50010001) > 0) then
			if(string.find(horn_input_.value, "\n")) then
				GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ChatPanel_001')})
			elseif(string.len(horn_input_.value) > 90) then
				GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ChatPanel_003')})
			else
				local msg = msg_hall_pb.cmsg_chat()
				msg.text = horn_input_.value
				if(string.len(horn_input_.value) > 90) then
					msg.text = str_sub(msg.text, 90)
				end
				msg.type = 1
				local data = msg:SerializeToString()
				GameTcp.Send(opcodes.CMSG_CHAT_CHANNEL, data)
				horn_input_.value = ''
				horn_panel_.gameObject:SetActive(false)
			end
		else
			GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ChatPanel_004')})
			GUIRoot.ShowGUI("BuyPanel", {Config.get_shop_by_item(50010001).id, {"ChatPanel", "HallPanel"}, ChatPanel.InitLabaNum})
		end
	else
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('ChatPanel_002')})
	end
end


function ChatPanel.OpenZone(obj)
	local guid = tostring(obj.name)
	if(guid == self.guid) then
		if(self.is_guest == 1) then
			GUIRoot.ShowGUI("AccountPanel")
		else
			GUIRoot.HideGUI("ChatPanel")
			GUIRoot.HideGUI("HallPanel")
			GUIRoot.ShowGUI("ZonePanel", {self.player, self.battle_results})
		end
	else
		local msg = msg_hall_pb.cmsg_player_look()
		msg.target_guid = guid
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_PLAYER_LOOK, data, {opcodes.SMSG_PLAYER_LOOK})
	end
end

-----------------------------------------------------------
function ChatPanel.Refresh()
	ChatPanel.ShowHornMsg()
end

function ChatPanel.ShowHornMsg()
	if(horn_res_.gameObject.activeInHierarchy) then
		horn_time_ = horn_time_ + time_speed
		if(horn_time_ >= 5) then
			table.remove(horn_msgs_, 1)
			horn_time_ = 0
			horn_res_.gameObject:SetActive(false)
		end
	else
		if(#(horn_msgs_) > 0) then
			local msg = horn_msgs_[1]
			--horn_res_:Find('lv'):GetComponent('UILabel').text = 'Lv.'..msg.level
			horn_res_:Find('name'):GetComponent('UILabel').text = msg.player_name
			horn_res_:Find('region'):GetComponent('UISprite').spriteName = Config.get_t_foregion(msg.region_id).icon
			IconPanel.InitVipLabel(horn_res_:Find('name'):GetComponent('UILabel'), msg.name_color)
			horn_res_:Find('text'):GetComponent('UILabel').text = msg.text
			local avatar_ = horn_res_:Find('avatar_inf')
			AvaIconPanel.ModifyAvatar(avatar_, msg.avatar,'', msg.toukuang, msg.sex)
			horn_res_.gameObject:SetActive(true)
		end
	end
end

function ChatPanel.BuyLaba()
	GUIRoot.HideGUI("ChatPanel")
	GUIRoot.HideGUI("HallPanel")
	GUIRoot.ShowGUI("ShopPanel")
end
----------------------------服务器code---------------------

function ChatPanel.SMSG_CHAT(msg)
	if(lua_script_ == nil) then
		return 0
	end
	if (ChatPanel.wndClass == 1 or ChatPanel.wndClass == 2) and  (msg.type == 0 or msg.type == 1) then
		ChatPanel.NewMesssage(msg)
		if(msg.player_guid == self.player.guid) then
			ChatPanel.CheckLatestMsg()
		else
			local uv = m_view:GetComponent('UIScrollView')
			local offy = uv.panel.clipOffset.y
			if(offy >= (viewh_ - maxh_) - pre_height + 100 + pre_height / 3) then
				unread_msg = unread_msg + 1
				local content = string.format(Config.get_t_script_str('ChatPanel_005'),tostring(unread_msg));
				unread_btn_:Find('Label'):GetComponent('UILabel').text = content
				unread_btn_.gameObject:SetActive(true)
			end
		end
		if(msg.type == 1) then
			table.insert(horn_msgs_, msg)
		end
	end
end

function ChatPanel.SMSG_SYS_INFO(msg)
	if(lua_script_ == nil) then
		return 0
	end
	if ChatPanel.wndClass == 3 and ChatPanel.wndClass == 1 then
		ChatPanel.NewSysMessage(msg)
		local uv = m_view:GetComponent('UIScrollView')
		local offy = uv.panel.clipOffset.y
		if(offy >= (viewh_ - maxh_) - pre_height + 100 + pre_height / 3) then
			unread_msg = unread_msg + 1
			local content = string.format(Config.get_t_script_str('ChatPanel_005'),tostring(unread_msg));
			unread_btn_:Find('Label'):GetComponent('UILabel').text = content
			unread_btn_.gameObject:SetActive(true)
		end
	end
end

function ChatPanel.SMSG_CHAT_LABA()
	if(self.get_item_num(50010001) > 0) then
		self.delete_item_num(50010001, 1)
		self.add_all_type_num(140, 1)
		ChatPanel.InitLabaNum()
	end
end

function ChatPanel.SMSG_PLAYER_LOOK(message)
	local msg = msg_hall_pb.smsg_player_look()
	msg:ParseFromString(message.luabuff)
	GUIRoot.ShowGUI("ZonePanel", {msg.player, msg.battle_his})
	GUIRoot.HideGUI("ChatPanel")
	GUIRoot.HideGUI("HallPanel")
end