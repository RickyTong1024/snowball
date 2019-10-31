BuyPanel = {}

local buy_panel_
local exchange_panel_

local num_label_
local price_label_

local buy_item_id_ = 0
local buy_item_num_ = 0
local buy_func_
local hide_panel_ = {}

local max_num = 100

function BuyPanel.Awake(obj)
	local lua_script = obj.transform:GetComponent('LuaUIBehaviour')
	
	buy_panel_ = obj.transform:Find("buy_panel")
	exchange_panel_ = obj.transform:Find("exchange_panel")
	
	num_label_ = buy_panel_:Find('buy_amount/Label'):GetComponent('UILabel')
	price_label_ = buy_panel_:Find('buy_price/Label'):GetComponent('UILabel')
	
	local add_btn = buy_panel_:Find('add_btn')
	local del_btn = buy_panel_:Find('del_btn')
	local add_ten_btn = buy_panel_:Find('add_ten_btn')
	local del_ten_btn = buy_panel_:Find('del_ten_btn')
	local ok_btn = buy_panel_:Find('ok_btn')
	local no_btn = buy_panel_:Find('no_btn')
	local close_btn = exchange_panel_:Find('close_btn')
	local exchange_btn = exchange_panel_:Find('exchange_btn')
	local item_icon = buy_panel_:Find('infor_panel/item_icon/icon')
	local effect_icon = exchange_panel_:Find("icon")
	
	lua_script:AddButtonEvent(item_icon.gameObject, "click", BuyPanel.Click)
	lua_script:AddButtonEvent(add_btn.gameObject, "click", BuyPanel.Click)
	lua_script:AddButtonEvent(del_btn.gameObject, "click", BuyPanel.Click)
	lua_script:AddButtonEvent(add_ten_btn.gameObject, "click", BuyPanel.Click)
	lua_script:AddButtonEvent(del_ten_btn.gameObject, "click", BuyPanel.Click)
	lua_script:AddButtonEvent(ok_btn.gameObject, "click", BuyPanel.Click)
	lua_script:AddButtonEvent(no_btn.gameObject, "click", BuyPanel.Click)
	lua_script:AddButtonEvent(close_btn.gameObject, "click", BuyPanel.Click)
	lua_script:AddButtonEvent(exchange_btn.gameObject, "click", BuyPanel.Click)
	lua_script:AddButtonEvent(effect_icon.gameObject, "click", BuyPanel.Click)
	
	BuyPanel.RegisterMessage()
	
	buy_panel_.gameObject:SetActive(false)
	exchange_panel_.gameObject:SetActive(false)
end

function BuyPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_ITEM_BUY, BuyPanel.SMSG_ITEM_BUY)
	Message.register_handle("team_join_msg", BuyPanel.TeamJoin)
end

function BuyPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_ITEM_BUY, BuyPanel.SMSG_ITEM_BUY)
	Message.remove_handle("team_join_msg", BuyPanel.TeamJoin)
end

function BuyPanel.OnDestroy()
	buy_item_id_ = 0
	buy_item_num_ = 1
	max_num = 100
	hide_panel_ = {}
	buy_func_ = nil
	BuyPanel.RemoveMessage()
end

function BuyPanel.OnParam(param)
	buy_item_id_ = param[1]
	hide_panel_ = param[2]
	buy_func_ = param[3]
	buy_item_num_ = 1
	local shop_temp = Config.get_t_shop(buy_item_id_)
	if(shop_temp.item_type == 7) then
		BuyPanel.InitExchangePanel()
	elseif(shop_temp.item_type == 3) then
		max_num = 1
		BuyPanel.InitBuyPanel()
	else
		BuyPanel.InitBuyPanel()
	end
end

function BuyPanel.TeamJoin()
	GUIRoot.HideGUI('BuyPanel')
end
------------------------初始化界面------------------

function BuyPanel.InitBuyPanel()
	local shop_temp = Config.get_t_shop(buy_item_id_)
	local item_temp = Config.get_t_reward(shop_temp.item_type, shop_temp.value1)
	local item_num = self.get_item_num(item_temp.id)
	local item_icon = buy_panel_:Find('infor_panel/item_icon/icon')
	local item_name = buy_panel_:Find('infor_panel/item_name'):GetComponent('UILabel')
	local item_num_text = buy_panel_:Find('infor_panel/item_num'):GetComponent('UILabel')
	local item_price = buy_panel_:Find('infor_panel/item_price'):GetComponent('UILabel')
	local past_icon_sing = buy_panel_:Find('infor_panel/item_price/icon'):GetComponent('UISprite')
	local past_icon_buy = buy_panel_:Find('buy_price'):GetComponent('UISprite')
	IconPanel.ModifyIcon(item_icon.parent, item_temp.icon, item_temp.color)
	local res_temp = Config.get_t_resource(shop_temp.past_type)
	past_icon_sing.spriteName = res_temp.small_icon
	past_icon_buy.spriteName = res_temp.small_icon
	item_name.text = item_temp.name
	IconPanel.InitQualityLabel(item_name, item_temp.color % 10)
	item_price.text = self.font_color[res_temp.color]..shop_temp.price
	item_num_text.text = item_num
	BuyPanel.Refresh()
	GUIRoot.UIEffectScalePos(buy_panel_.gameObject, true, 1)
	buy_panel_.gameObject:SetActive(true)
end

function BuyPanel.InitExchangePanel()
	local shop_temp = Config.get_t_shop(buy_item_id_)
	local effect_temp = Config.get_t_reward(shop_temp.item_type, shop_temp.value1)
	local effect_name = exchange_panel_:Find("name"):GetComponent("UILabel")
	local effect_cost = exchange_panel_:Find("cost"):GetComponent("UILabel")
	local desc = exchange_panel_:Find("desc"):GetComponent("UILabel")
	local icon = exchange_panel_:Find("icon")
	local res_temp = Config.get_t_resource(shop_temp.past_type)
	if(effect_temp.type == 1) then
		desc.text = self.font_color[5]..Config.get_t_script_str('BuyPanel_001') --"雪球皮肤"
		icon:GetComponent("UIWidget").height = 100
		icon:GetComponent("UIWidget").round = false
		icon:GetComponent("UIWidget").depth = 4
	elseif(effect_temp.type == 3) then
		desc.text = self.font_color[5]..Config.get_t_script_str('BuyPanel_002') --"雪炮皮肤"
		icon:GetComponent("UIWidget").height = 100
		icon:GetComponent("UIWidget").round = false
		icon:GetComponent("UIWidget").depth = 4
	elseif(effect_temp.type == 2) then
		desc.text = self.font_color[5]..Config.get_t_script_str('BuyPanel_003') --"蓄力特效"
		icon:GetComponent("UIWidget").height = 120
		icon:GetComponent("UIWidget").round = true
		icon:GetComponent("UIWidget").depth = 2
	end
	icon:GetComponent("UISprite").atlas = IconPanel.GetAltas(effect_temp.icon)
	icon:GetComponent("UISprite").spriteName = effect_temp.icon
	effect_name.text = effect_temp.name
	IconPanel.InitQualityLabel(effect_name, effect_temp.color % 10)
	effect_cost.text = self.font_color[1]..shop_temp.price.." "..res_temp.name
	if self.has_fashion(shop_temp.value1) then
		exchange_panel_:Find('exchange_btn'):GetComponent("UISprite").spriteName = "b1_gray"
		exchange_panel_:Find('exchange_btn/Label'):GetComponent("UILabel").text = Config.get_t_script_str('BuyPanel_004') --"已拥有"
	end
	GUIRoot.UIEffectScalePos(exchange_panel_.gameObject, true, 1)
	exchange_panel_.gameObject:SetActive(true)
end

----------------------------------------------------

------------------------ButtonEvent-----------------

function BuyPanel.Click(obj)
	if(obj.name == 'add_btn') then
		BuyPanel.AddItem(1)
	elseif(obj.name == 'del_btn') then
		BuyPanel.DelItem(1)
	elseif(obj.name == 'add_ten_btn') then
		BuyPanel.AddItem(10)
	elseif(obj.name == 'del_ten_btn') then
		BuyPanel.DelItem(10)
	elseif(obj.name == 'ok_btn') then
		BuyPanel.Buy()
	elseif(obj.name == 'no_btn') then
		BuyPanel.ClosePanel()
	elseif(obj.name == 'icon') then
		local shop_temp = Config.get_t_shop(buy_item_id_)
		GUIRoot.ShowGUI('DetailPanel', {shop_temp.item_type.."+"..shop_temp.value1})
	elseif(obj.name == 'close_btn') then
		BuyPanel.ClosePanel()
	elseif(obj.name == 'exchange_btn') then
		BuyPanel.Buy()
	end
end

function BuyPanel.AddItem(num)
	local shop_temp = Config.get_t_shop(buy_item_id_)
	if(buy_item_num_ + num >= max_num) then
		buy_item_num_ = max_num
	else
		buy_item_num_ = buy_item_num_ + num
	end
	BuyPanel.Refresh()
end

function BuyPanel.DelItem(num)
	if(buy_item_num_ <= num) then
		buy_item_num_ = 1
	else
		buy_item_num_ = buy_item_num_ - num
	end
	BuyPanel.Refresh()
end

function BuyPanel.Buy()
	local shop_temp = Config.get_t_shop(buy_item_id_)
	if shop_temp.item_type == 7 and self.has_fashion(shop_temp.value1) then
		local str = string.format(Config.get_t_script_str('BuyPanel_004'),self.font_color[Config.get_t_fashion(shop_temp.value1).color],Config.get_t_fashion(shop_temp.value1).name)
		GUIRoot.ShowGUI("MessagePanel", {str})
		return 0
	end
	local past = buy_item_num_ * shop_temp.price
	local can_buy = false
	local player_res = self.get_resource(shop_temp.past_type)
	if(player_res >= past) then
		can_buy = true
	else
		local cancel_func = function() GUIRoot.ShowGUI("BuyPanel") end
		if(shop_temp.past_type == 2) then
			if(platform_config_common.m_platform == "android_yymoon") then
				local res_temp = Config.get_t_resource(shop_temp.past_type)
				local content = string.format(Config.get_t_script_str('BuyPanel_008'),self.font_color[res_temp.color],res_temp.name)	
				GUIRoot.ShowGUI("MessagePanel", {content})
			else
				GUIRoot.HideGUI("BuyPanel", false)
				local content = string.format(Config.get_t_script_str('BuyPanel_005'),self.font_color[2],Config.get_t_resource(2).name)
				GUIRoot.ShowGUI("SelectPanel", {content, Config.get_t_script_str('BuyPanel_006'), BuyPanel.Recharge, Config.get_t_script_str('BuyPanel_007'), cancel_func})
			end			
		else
			local res_temp = Config.get_t_resource(shop_temp.past_type)
			local content = string.format(Config.get_t_script_str('BuyPanel_008'),self.font_color[res_temp.color],res_temp.name)	
			GUIRoot.ShowGUI("MessagePanel", {content})
		end
	end
	if(can_buy) then
		local msg = msg_hall_pb.cmsg_item_buy()
		msg.id = buy_item_id_
		msg.num = buy_item_num_
		local data = msg:SerializeToString()
		GameTcp.Send(opcodes.CMSG_ITEM_BUY, data, {opcodes.SMSG_ITEM_BUY})
	end
end

-----------------------------------------------------

function BuyPanel.Recharge()
	GUIRoot.HideGUI("BuyPanel")
	if(not GUIRoot.HasGUI("ShopPanel")) then
		if(hide_panel_ ~= nil) then
			for i = 1, #hide_panel_ do
				GUIRoot.HideGUI(hide_panel_[i])
			end
		end
		GUIRoot.ShowGUI("ShopPanel", {5})
	else
		ShopPanel.SelectPage(5)
	end
end

function BuyPanel.ClosePanel()
	GUIRoot.HideGUI("BuyPanel")
end

function BuyPanel.Refresh()
	local shop_temp = Config.get_t_shop(buy_item_id_)
	local res_temp = Config.get_t_resource(shop_temp.past_type)
	num_label_.text = tostring(buy_item_num_)..'/'..tostring(max_num)
	price_label_.text = self.font_color[res_temp.color]..buy_item_num_ * shop_temp.price
end

function BuyPanel.SMSG_ITEM_BUY(message)
	local msg = msg_hall_pb.smsg_item_buy()
	msg:ParseFromString(message.luabuff)
	local shop_temp = Config.get_t_shop(buy_item_id_)
	local past = buy_item_num_ * shop_temp.price
	local reward = self.count_reward(msg.types, msg.value1s, msg.value2s, msg.value3s)
	local can_buy = false
	if(self.get_resource(shop_temp.past_type) >= past) then
		can_buy = true
		self.add_resource(shop_temp.past_type, -past)
	end
	if(can_buy) then
		if shop_temp.type == 1 then
			self.add_all_type_num(40, 1)
		end
		if(#reward > 0) then
			for i = 1, #reward do
				self.add_reward(reward[i])
			end
			GUIRoot.ShowGUI("GainPanel", {reward})
		end
		for i = 1, #msg.roles do
			self.add_role(msg.roles[i])
		end
		GUIRoot.HideGUI('BuyPanel')
		if(#msg.roles > 0) then
			GUIRoot.ShowGUI("RoleGetPanel", {msg.roles, hide_panel_})
		end
		if(buy_func_ ~= nil) then
			buy_func_()
		end
	end
end
