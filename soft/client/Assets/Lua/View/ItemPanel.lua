ItemPanel = {}

local lua_script_

local pack_panel_
local infor_panel_
local single_sell_panel_
local double_sell_panel_
local double_craft_panel_
local equip_panel_

local item_view_
local empty_tip_
local operate_btn_

local pre_item_ = nil
local cur_item_id = 0

local item_list_ = {}
local page_btn_ = {}

local cur_page = 0
local btn_name = {"item_btn", "ball_btn", "weapon_btn", "effect_btn"}
local page_icon = {"xdjxtb_001", "xdjxtb_003", "xdjxtb_004", "xdjxtb_002"}
local page_text = {}
local space_x_ = 100
local space_y_ = 100
local view_pos = Vector3(0, 0, 0)

local y_pos = 0

function ItemPanel.Awake(obj)
	table.insert(page_text,Config.get_t_script_str('ItemPanel_001'))
	table.insert(page_text,Config.get_t_script_str('ItemPanel_002'))
	table.insert(page_text,Config.get_t_script_str('ItemPanel_003'))
	table.insert(page_text,Config.get_t_script_str('ItemPanel_004'))
	GUIRoot.UIEffect(obj, 0)
	mapMgr:SetTargetCam(-0.533 * panelMgr:get_wh(), 0.6, 3)
	GUIRoot.ShowGUI('BackPanel', {3})
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	local btn_res = obj.transform:Find('Anchor_left/left_panel/btn_res').gameObject
	
	local fir_pos = GUIRoot.height / 2 - 160
	for i = 1, #btn_name do
		local btn_t = LuaHelper.Instantiate(btn_res)
		btn_t.transform.parent = obj.transform:Find('Anchor_left/left_panel')
		btn_t.transform.localPosition = Vector3(-20, fir_pos - (i - 1) * 80, 0)
		btn_t.transform.localScale = Vector3.one
		btn_t.name = btn_name[i]
		table.insert(page_btn_, btn_t.transform)
		btn_t:SetActive(true)
	end
	
	for i = 1, #page_btn_ do
		lua_script_:AddButtonEvent(page_btn_[i].gameObject, "click", ItemPanel.Click)
	end
	
	pack_panel_ = obj.transform:Find('Anchor_right/pack_panel')
	infor_panel_ = obj.transform:Find('Anchor_top_right/infor_panel')
	double_sell_panel_ = obj.transform:Find('Anchor_top_right/double_sell_panel')
	single_sell_panel_ = obj.transform:Find('Anchor_top_right/single_sell_panel')
	equip_panel_ = obj.transform:Find('Anchor_top_right/equip_panel')
	
	infor_panel_:Find("bg"):GetComponent("UIWidget").width = GUIRoot.width - 416
	
	item_view_ = pack_panel_:Find('item_view')
	empty_tip_ = pack_panel_:Find('empty_tip')
	
	item_view_:GetComponent("UIPanel"):SetRect(0, -20, 416, GUIRoot.height - 60)
	y_pos = (GUIRoot.height - 60) / 2 - 63
	
	view_pos = item_view_.localPosition
	
	local single_sell_btn = single_sell_panel_:Find('sell_btn')
	local double_use_btn = double_sell_panel_:Find('use_btn')
	local double_sell_btn = double_sell_panel_:Find('sell_btn')
	operate_btn_ = equip_panel_:Find('operate_btn')
	
	lua_script_:AddButtonEvent(operate_btn_.gameObject, 'click', ItemPanel.OperateEffect)
	lua_script_:AddButtonEvent(single_sell_btn.gameObject, 'click', ItemPanel.Sell)
	lua_script_:AddButtonEvent(double_sell_btn.gameObject, 'click', ItemPanel.Sell)
	lua_script_:AddButtonEvent(double_use_btn.gameObject, 'click', ItemPanel.Use)
	
	double_sell_panel_.gameObject:SetActive(false)
	equip_panel_.gameObject:SetActive(false)
	single_sell_panel_.gameObject:SetActive(false)
	infor_panel_.gameObject:SetActive(false)
	pack_panel_.gameObject:SetActive(false)
	ItemPanel.RegisterMessage()
	ItemPanel.SelectPage(1)
end

function ItemPanel.RegisterMessage()
	Message.register_net_handle(opcodes.SMSG_FASHION_ON, ItemPanel.SMSG_FASHION_ON)
	Message.register_handle("back_panel_msg", ItemPanel.Back)
	Message.register_handle("back_panel_recharge", ItemPanel.Recharge)
	Message.register_handle("team_join_msg", ItemPanel.TeamJoin)
end

function ItemPanel.RemoveMessage()
	Message.remove_net_handle(opcodes.SMSG_FASHION_ON, ItemPanel.SMSG_FASHION_ON)
	Message.remove_handle("back_panel_msg", ItemPanel.Back)
	Message.remove_handle("back_panel_recharge", ItemPanel.Recharge)
	Message.remove_handle("team_join_msg", ItemPanel.TeamJoin)
end

function ItemPanel.OnDestroy()
	cur_item_id = 0
	pre_item_ = nil
	item_list_ = {}
	page_btn_ = {}
	ItemPanel.RemoveMessage()
end

function ItemPanel.Back()
	GUIRoot.HideGUI('ItemPanel')
	GUIRoot.ShowGUI('HallPanel')
	GUIRoot.HideGUI('BackPanel')
end

function ItemPanel.Recharge()
	GUIRoot.HideGUI('ItemPanel')
	GUIRoot.ShowGUI("ShopPanel", {5})
end

function ItemPanel.TeamJoin()
	GUIRoot.HideGUI('ItemPanel')
end

----------------------------刷新列表--------------------

function ItemPanel.InitListPanel(page)
	pre_item_ = nil
	item_list_ = {}
	double_sell_panel_.gameObject:SetActive(false)
	single_sell_panel_.gameObject:SetActive(false)
	equip_panel_.gameObject:SetActive(false)
	infor_panel_.gameObject:SetActive(false)
	empty_tip_.gameObject:SetActive(false)
	if(item_view_.childCount > 0) then
		for i = 0, item_view_.childCount - 1 do
            GameObject.Destroy(item_view_:GetChild(i).gameObject)
        end
	end
	local uv = item_view_:GetComponent('UIScrollView')
	if item_view_:GetComponent('SpringPanel') ~= nil then
		item_view_:GetComponent('SpringPanel').enabled = false
	end
	uv.panel.clipOffset = Vector2(0, 0)
	item_view_.localPosition = view_pos
	if(page == 1) then
		ItemPanel.InitItemList()
	elseif(page == 2) then
		ItemPanel.InitEffectList(1)
	elseif(page == 4) then
		ItemPanel.InitEffectList(2)
	elseif(page == 3) then
		ItemPanel.InitEffectList(3)
	end
end

function ItemPanel.InitItemList()
	local j = 0
	for i = 1, #Config.t_item_ids do
		local item_temp = Config.get_t_item(Config.t_item_ids[i])
		local item_num = self.get_item_num(item_temp.id)
		if(item_num > 0 and item_temp.type ~= 5001) then
			local item = IconPanel.GetIcon("item_res", {"icon", ItemPanel.SelectItem}, item_temp.icon, item_temp.color, item_num)
			item.transform.parent = item_view_
			item.transform.localPosition = Vector3(j % 4 * space_x_ - 148, -(math.floor(j / 4) * space_y_) + y_pos, 0)
			item.transform.localScale = Vector3.one
			local icon = item.transform:Find('icon')
			icon.name = item_temp.id
			icon.gameObject:SetActive(true)
			if(item_temp.id == cur_item_id) then
				pre_item_ = icon.gameObject
			elseif(pre_item_ == nil) then
				pre_item_ = icon.gameObject
			end
			item:SetActive(true)
			item_list_[item_temp.id] = item
			j = j + 1
		end
	end
	if(pre_item_ ~= nil) then
		ItemPanel.SelectItem(pre_item_)
	end
	if(j == 0) then
		empty_tip_.gameObject:SetActive(true)
	end
	pack_panel_.gameObject:SetActive(true)
end

function ItemPanel.InitEffectList(type)
	local j = 0
	for i = 1, #Config.t_fashion_ids do
		local effect_temp = Config.t_fashion_ids[i]
		if(self.has_fashion(effect_temp.id) and type == effect_temp.type) then
			local effect = IconPanel.GetIcon("effect_res", {"icon", ItemPanel.SelectItem}, effect_temp.icon, effect_temp.color)
			effect.transform.parent = item_view_
			effect.transform.localPosition = Vector3(j % 4 * space_x_ - 148, -(math.floor(j / 4) * space_y_) + y_pos, 0)
			effect.transform.localScale = Vector3.one
			if(self.player.fashion_on[effect_temp.type] == effect_temp.id) then
				effect.transform:Find("equip_tip").gameObject:SetActive(true)
			end
			local icon = effect.transform:Find('icon')
			icon.name = effect_temp.id
			icon.gameObject:SetActive(true)
			if(effect_temp.id == cur_item_id) then
				pre_item_ = icon.gameObject
			elseif(pre_item_ == nil) then
				pre_item_ = icon.gameObject
			end
			effect:SetActive(true)
			j = j + 1
		end
	end
	if(pre_item_ ~= nil) then
		ItemPanel.SelectItem(pre_item_)
	end
	if(j == 0) then
		empty_tip_.gameObject:SetActive(true)
	end
	pack_panel_.gameObject:SetActive(true)
end

---------------------------------------------------------


-----------------------------ButtonEvent-----------------

function ItemPanel.Click(obj)
	if(obj.name == "item_btn") then
		ItemPanel.SelectPage(1)
	elseif(obj.name == "ball_btn") then
		ItemPanel.SelectPage(2)
	elseif(obj.name == "effect_btn") then
		ItemPanel.SelectPage(4)
	elseif(obj.name == "weapon_btn") then
		ItemPanel.SelectPage(3)
	end
end

function ItemPanel.SelectPage(page)
	local fir_pos = GUIRoot.height / 2 - 160
	for i = 1, #page_btn_ do
		page_btn_[i].localPosition = Vector3(-20, fir_pos - (i - 1) * 80, 0)
		page_btn_[i]:GetComponent("UISprite").spriteName = "fldb_001"
		page_btn_[i]:Find("icon_bg"):GetComponent("UISprite").spriteName = "fldb_001a"
		page_btn_[i]:Find("Label"):GetComponent("UILabel").text = page_text[i]
		page_btn_[i]:Find('icon'):GetComponent("UISprite").spriteName = page_icon[i].."a"
		page_btn_[i]:Find("Label"):GetComponent("UILabel").gradientTop = Color(255/ 255,255/ 255,255/ 255)
		page_btn_[i]:Find("Label"):GetComponent("UILabel").gradientBottom = Color(72 / 255, 126 / 255, 186 / 255)
		page_btn_[i]:Find("Label"):GetComponent("UILabel").effectColor = Color(13 / 255, 84 / 255, 164 / 255)
	end
	page_btn_[page].localPosition = Vector3(0, fir_pos - (page - 1) * 80, 0)
	page_btn_[page]:GetComponent("UISprite").spriteName = "fldb_002"
	page_btn_[page]:Find("icon_bg"):GetComponent("UISprite").spriteName = "fldb_002a"
	page_btn_[page]:Find('icon'):GetComponent("UISprite").spriteName = page_icon[page]
	page_btn_[page]:Find("Label"):GetComponent("UILabel").text = page_text[page]
	page_btn_[page]:Find("Label"):GetComponent("UILabel").gradientTop = Color(255/ 255,255/ 255,255/ 255)
	page_btn_[page]:Find("Label"):GetComponent("UILabel").gradientBottom = Color(97 / 255, 187 / 255, 237 / 255)
	page_btn_[page]:Find("Label"):GetComponent("UILabel").effectColor = Color(25 / 255, 147 / 255, 220 / 255)
	cur_page = page
	ItemPanel.InitListPanel(page)
end

function ItemPanel.Sell()
	GUIRoot.ShowGUI('SellPanel', {cur_item_id, ItemPanel.Refresh})
end

function ItemPanel.Use()
	GUIRoot.ShowGUI('UsePanel', {cur_item_id, ItemPanel.Refresh})
end

function ItemPanel.SelectItem(obj)
	if(pre_item_ ~= nil and pre_item_ ~= obj) then
		pre_item_.transform.parent:Find('select_icon').gameObject:SetActive(false)
	end
	obj.transform.parent.transform:Find('select_icon').gameObject:SetActive(true)
	cur_item_id = tonumber(obj.name)
	pre_item_ = obj
	ItemPanel.InitInforPanel()
end

function ItemPanel.OperateEffect()
	local msg = msg_hall_pb.cmsg_fashion_on()
	local effect_temp = Config.get_t_fashion(cur_item_id)
	if(self.player.fashion_on[effect_temp.type] == effect_temp.id) then
		msg.id = 0
		msg.type = effect_temp.type
	else
		msg.id = effect_temp.id
		msg.type = effect_temp.type
	end
	local data = msg:SerializeToString()
	GameTcp.Send(opcodes.CMSG_FASHION_ON, data, {opcodes.SMSG_FASHION_ON})
end

function ItemPanel.InitInforPanel()
	double_sell_panel_.gameObject:SetActive(false)
	single_sell_panel_.gameObject:SetActive(false)
	equip_panel_.gameObject:SetActive(false)
	infor_panel_:Find('limit_time').gameObject:SetActive(false)
	infor_panel_:Find('sell_value').gameObject:SetActive(false)
	infor_panel_:Find('item_icon/equip_tip').gameObject:SetActive(false)
	local num_label = infor_panel_:Find('item_amount'):GetComponent('UILabel')
	local item_temp = nil
	if(cur_page == 1) then
		item_temp = Config.get_t_item(cur_item_id)
		local item_num = PlayerData.get_item_num(item_temp.id)
		num_label.text = Config.get_t_script_str('ItemPanel_005').." "..item_num
	elseif(cur_page == 2 or cur_page == 3 or cur_page == 4) then
		item_temp = Config.get_t_fashion(cur_item_id)
		if(self.player.fashion_on[item_temp.type] == item_temp.id) then
			infor_panel_:Find('item_icon/equip_tip').gameObject:SetActive(true)
			operate_btn_:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('ItemPanel_006')--"卸下"
			operate_btn_:GetComponent("UISprite").spriteName = "b1_red"
		else
			operate_btn_:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('ItemPanel_007') --"装备"
			operate_btn_:GetComponent("UISprite").spriteName = "b1"
		end
		if(item_temp.type == 1) then
			num_label.text = Config.get_t_script_str('ItemPanel_008') --"雪球皮肤"
		elseif(item_temp.type == 2) then
			num_label.text = Config.get_t_script_str('ItemPanel_009') --"蓄力特效"
		elseif(item_temp.type == 3) then
			num_label.text = Config.get_t_script_str('ItemPanel_010') --"雪炮皮肤"
		end
		equip_panel_.gameObject:SetActive(true)
	end
	local name_label = infor_panel_:Find('item_name'):GetComponent('UILabel')
	IconPanel.ModifyIcon(infor_panel_:Find('item_icon'), item_temp.icon, item_temp.color)
	name_label.text = item_temp.name
	IconPanel.InitQualityLabel(name_label, item_temp.color % 10)
	infor_panel_:Find('desc'):GetComponent('UILabel').text = item_temp.desc
	if(cur_page == 1 and item_temp.sell > 0) then
		local res_temp = Config.get_t_resource(item_temp.sell_type)
		local sell_icon = infor_panel_:Find('sell_value/icon'):GetComponent("UISprite")
		sell_icon.spriteName = res_temp.small_icon
		infor_panel_:Find('sell_value'):GetComponent('UILabel').text = self.font_color[res_temp.color]..item_temp.sell
		infor_panel_:Find('sell_value').gameObject:SetActive(true)
		if(item_temp.type == 3001) then
			double_sell_panel_.gameObject:SetActive(true)
		else
			single_sell_panel_.gameObject:SetActive(true)
		end
	end
	infor_panel_.gameObject:SetActive(true)
end

---------------------------------------------------------


function ItemPanel.Refresh()
	ItemPanel.SelectPage(1)
end

function ItemPanel.SMSG_FASHION_ON()
	local effect_temp = Config.get_t_fashion(cur_item_id)
	if(self.player.fashion_on[effect_temp.type] == effect_temp.id) then
		self.player.fashion_on[effect_temp.type] = 0
	else
		self.player.fashion_on[effect_temp.type] = effect_temp.id
	end
	if(effect_temp.type == 1) then
		ItemPanel.SelectPage(2)
	elseif(effect_temp.type == 2) then
		ItemPanel.SelectPage(4)
	elseif(effect_temp.type == 3) then
		ItemPanel.SelectPage(3)
		HallScene.ChangeWeapon()
	end
end