SellPanel = {}

local lua_script_

local main_panel_

local num_label_
local price_label_
local item_icon_
local item_name_
local item_price_
local item_num_
local price_icon_

local sell_item_id_ = 0
local sell_item_num_ = 1
local sell_func_

function SellPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	main_panel_ = obj.transform:Find("main_panel")
	
	GUIRoot.UIEffectScalePos(main_panel_.gameObject, 1, true)
	
	num_label_ = main_panel_:Find('sell_amount/Label'):GetComponent('UILabel')
	price_label_ = main_panel_:Find('sell_price/Label'):GetComponent('UILabel')
	sell_icon_ = main_panel_:Find('sell_price'):GetComponent('UISprite')
	item_icon_ = main_panel_:Find('infor_panel/item_icon')
	price_icon_ = main_panel_:Find("infor_panel/item_price/icon"):GetComponent("UISprite")
	item_name_ = main_panel_:Find('infor_panel/item_name'):GetComponent('UILabel')
	item_num_ = main_panel_:Find('infor_panel/item_num'):GetComponent('UILabel')
	item_price_ = main_panel_:Find('infor_panel/item_price'):GetComponent('UILabel')
	
	local add_btn = main_panel_:Find('add_btn')
	local del_btn = main_panel_:Find('del_btn')
	local add_ten_btn = main_panel_:Find('add_ten_btn')
	local del_ten_btn = main_panel_:Find('del_ten_btn')
	local ok_btn = main_panel_:Find('ok_btn')
	local no_btn = main_panel_:Find('no_btn')
	
	lua_script_:AddButtonEvent(add_btn.gameObject, "click", SellPanel.Click)
	lua_script_:AddButtonEvent(del_btn.gameObject, "click", SellPanel.Click)
	lua_script_:AddButtonEvent(add_ten_btn.gameObject, "click", SellPanel.Click)
	lua_script_:AddButtonEvent(del_ten_btn.gameObject, "click", SellPanel.Click)
	lua_script_:AddButtonEvent(ok_btn.gameObject, "click", SellPanel.Click)
	lua_script_:AddButtonEvent(no_btn.gameObject, "click", SellPanel.Click)
	SellPanel.RegisterMessage()
end

function SellPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_ITEM_SELL, SellPanel.SMSG_ITEM_SELL)
	Message.register_handle("team_join_msg", SellPanel.TeamJoin)
end

function SellPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_ITEM_SELL, SellPanel.SMSG_ITEM_SELL)
	Message.remove_handle("team_join_msg", SellPanel.TeamJoin)
end

function SellPanel.OnParam(param)
	sell_item_id_ = param[1]
	sell_func_ = param[2]
	sell_item_num_ = 1
	local item_temp = Config.get_t_item(sell_item_id_)
	local res_temp = Config.get_t_resource(item_temp.sell_type)
	price_icon_.spriteName = res_temp.small_icon
	sell_icon_.spriteName = res_temp.small_icon
	local item_num = self.get_item_num(sell_item_id_)
	IconPanel.ModifyIcon(item_icon_, item_temp.icon, item_temp.color)
	item_name_.text = item_temp.name
	IconPanel.InitQualityLabel(item_name_, item_temp.color % 10)
	item_price_.text = self.font_color[res_temp.color]..item_temp.sell
	item_num_.text = item_num
	SellPanel.Refresh()
end

function SellPanel.OnDestroy()
	sell_item_id_ = 0
	sell_item_num_ = 1
	sell_func_ = nil
	SellPanel.RemoveMessage()
end

function SellPanel.TeamJoin()
	GUIRoot.HideGUI('SellPanel')
end
---------------------ButtonEvent--------------

function SellPanel.Click(obj)
	if(obj.name == 'add_btn') then
		SellPanel.AddItem(1)
	elseif(obj.name == 'del_btn') then
		SellPanel.DelItem(1)
	elseif(obj.name == 'add_ten_btn') then
		SellPanel.AddItem(10)
	elseif(obj.name == 'del_ten_btn') then
		SellPanel.DelItem(10)
	elseif(obj.name == 'ok_btn') then
		SellPanel.Sell()
	elseif(obj.name == 'no_btn') then
		SellPanel.ClosePanel()
	end
end

function SellPanel.AddItem(num)
	local item_num = self.get_item_num(sell_item_id_)
	if(sell_item_num_ + num >= item_num) then
		sell_item_num_ = item_num
	else
		sell_item_num_ = sell_item_num_ + num
	end
	SellPanel.Refresh()
end

function SellPanel.DelItem(num)
	if(sell_item_num_ <= num) then
		sell_item_num_ = 1
	else
		sell_item_num_ = sell_item_num_ - num
	end
	SellPanel.Refresh()
end

function SellPanel.Sell()
	local item_num = self.get_item_num(sell_item_id_)
	if(item_num >= sell_item_num_) then
		local msg = msg_hall_pb.cmsg_item_sell()
		msg.id = sell_item_id_
		msg.num = sell_item_num_
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_ITEM_SELL, data, {opcodes.SMSG_ITEM_SELL})
	end
end

----------------------------------------------

function SellPanel.ClosePanel()
	GUIRoot.HideGUI("SellPanel")
end

function SellPanel.Refresh()
	local item_num = self.get_item_num(sell_item_id_)
	local item_temp = Config.get_t_item(sell_item_id_)
	local res_temp = Config.get_t_resource(item_temp.sell_type)
	num_label_.text = tostring(sell_item_num_)..'/'..tostring(item_num)
	price_label_.text = self.font_color[res_temp.color]..sell_item_num_ * item_temp.sell
end

function SellPanel.SMSG_ITEM_SELL()
	local item_temp = Config.get_t_item(sell_item_id_)
	local item_sell = sell_item_num_ * item_temp.sell
	local item_num = self.get_item_num(sell_item_id_)
	if(item_num >= sell_item_num_) then
		self.delete_item_num(sell_item_id_, sell_item_num_)
		local reward = {}
		reward.type = 1
		reward.value1 = item_temp.sell_type
		reward.value2 = item_sell
		reward.value3 = 0
		self.add_reward(reward)
		if item_temp.type == 1001 then
			self.add_all_type_num(240, 1)
		end
		GUIRoot.ShowGUI('GainPanel', {{reward}})
		GUIRoot.HideGUI('SellPanel')
		if(sell_func_ ~= nil) then
			sell_func_()
		end
	end
end
