UsePanel = {}

local lua_script_

local main_panel_

local num_label_
local item_icon_
local item_name_
local item_num_

local use_item_id_ = 0
local use_item_num_ = 1
local use_func_

function UsePanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	main_panel_ = obj.transform:Find("main_panel")
	
	GUIRoot.UIEffectScalePos(main_panel_.gameObject, 1, true)
	
	num_label_ = main_panel_:Find('sell_amount/Label'):GetComponent('UILabel')
	item_icon_ = main_panel_:Find('infor_panel/item_icon')
	item_name_ = main_panel_:Find('infor_panel/item_name'):GetComponent('UILabel')
	item_num_ = main_panel_:Find('infor_panel/item_num'):GetComponent('UILabel')
	
	local add_btn = main_panel_:Find('add_btn')
	local del_btn = main_panel_:Find('del_btn')
	local add_ten_btn = main_panel_:Find('add_ten_btn')
	local del_ten_btn = main_panel_:Find('del_ten_btn')
	local ok_btn = main_panel_:Find('ok_btn')
	local no_btn = main_panel_:Find('no_btn')
	
	lua_script_:AddButtonEvent(add_btn.gameObject, "click", UsePanel.Click)
	lua_script_:AddButtonEvent(del_btn.gameObject, "click", UsePanel.Click)
	lua_script_:AddButtonEvent(add_ten_btn.gameObject, "click", UsePanel.Click)
	lua_script_:AddButtonEvent(del_ten_btn.gameObject, "click", UsePanel.Click)
	lua_script_:AddButtonEvent(ok_btn.gameObject, "click", UsePanel.Click)
	lua_script_:AddButtonEvent(no_btn.gameObject, "click", UsePanel.Click)
	UsePanel.RegisterMessage()
end

function UsePanel.RegisterMessage()
	Message.register_handle("team_join_msg", UsePanel.TeamJoin)
	Message.register_net_handle(opcodes.SMSG_ITEM_APPLY, UsePanel.SMSG_ITEM_APPLY)
end

function UsePanel.RemoveMessage()
	Message.remove_handle("team_join_msg", UsePanel.TeamJoin)
	Message.remove_net_handle(opcodes.SMSG_ITEM_APPLY, UsePanel.SMSG_ITEM_APPLY)
end

function UsePanel.OnParam(param)
	use_item_id_ = param[1]
	use_func_ = param[2]
	use_item_num_ = 1
	local item_temp = Config.get_t_item(use_item_id_)
	local item_num = self.get_item_num(use_item_id_)
	IconPanel.ModifyIcon(item_icon_, item_temp.icon, item_temp.color)
	item_name_.text = item_temp.name
	IconPanel.InitQualityLabel(item_name_, item_temp.color % 10)
	item_num_.text = item_num
	UsePanel.Refresh()
end

function UsePanel.OnDestroy()
	use_item_id_ = 0
	use_item_num_ = 1
	use_func_ = nil
	UsePanel.RemoveMessage()
end

function UsePanel.TeamJoin()
	GUIRoot.HideGUI('UsePanel')
end

---------------------ButtonEvent--------------

function UsePanel.Click(obj)
	if(obj.name == 'add_btn') then
		UsePanel.AddItem(1)
	elseif(obj.name == 'del_btn') then
		UsePanel.DelItem(1)
	elseif(obj.name == 'add_ten_btn') then
		UsePanel.AddItem(10)
	elseif(obj.name == 'del_ten_btn') then
		UsePanel.DelItem(10)
	elseif(obj.name == 'ok_btn') then
		UsePanel.Use()
	elseif(obj.name == 'no_btn') then
		UsePanel.ClosePanel()
	end
end

function UsePanel.AddItem(num)
	local item_num = self.get_item_num(use_item_id_)
	if(use_item_num_ + num >= item_num) then
		use_item_num_ = item_num
	else
		use_item_num_ = use_item_num_ + num
	end
	UsePanel.Refresh()
end

function UsePanel.DelItem(num)
	if(use_item_num_ <= num) then
		use_item_num_ = 1
	else
		use_item_num_ = use_item_num_ - num
	end
	UsePanel.Refresh()
end

function UsePanel.Use()
	local item_num = self.get_item_num(use_item_id_)
	local item_temp = Config.get_t_item(use_item_id_)
	if(item_num >= use_item_num_ and self.player.level >= item_temp.level and item_temp.type == 3001) then
		local msg = msg_hall_pb.cmsg_item_apply()
		msg.item_id = use_item_id_
		msg.num = use_item_num_
		msg.item_index = Config.get_t_item_box(item_temp.def1).index
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_ITEM_APPLY, data, {opcodes.SMSG_ITEM_APPLY})
	end
end

----------------------------------------------

function UsePanel.ClosePanel()
	GUIRoot.HideGUI("UsePanel")
end

function UsePanel.Refresh()
	local item_num = self.get_item_num(use_item_id_)
	local item_temp = Config.get_t_item(use_item_id_)
	num_label_.text = tostring(use_item_num_)..'/'..tostring(item_num)
end

function UsePanel.SMSG_ITEM_APPLY(message)
	local msg = msg_hall_pb.smsg_item_apply()
	msg:ParseFromString(message.luabuff)
	local item_temp = Config.get_t_item(use_item_id_)
	local item_num = self.get_item_num(use_item_id_)
	if(item_num >= use_item_num_) then
		local use_rewad = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
		for i = 1, #use_rewad do
			self.add_reward(use_rewad[i])
		end
		self.delete_item_num(use_item_id_, use_item_num_)
		if(#use_rewad > 0) then
			GUIRoot.ShowGUI('GainPanel', {use_rewad})
		end
		GUIRoot.HideGUI('UsePanel')
		if(use_func_ ~= nil) then
			use_func_()
		end
	end
end
