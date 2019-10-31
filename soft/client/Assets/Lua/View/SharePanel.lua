SharePanel = {}

local lua_script_

local main_panel_
local share_panel_

local chest_btn_
local url_label_ 
local share_code_
local jewel_slider_

local open_mode_ = 1
local share_type_ = "common"

function SharePanel.Awake(obj)
	
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	main_panel_ = obj.transform:Find("main_panel")
	share_panel_ = obj.transform:Find("share_panel")
	
	jewel_slider_ = main_panel_:Find('jewel_pro')
	url_label_ = main_panel_:Find('url'):GetComponent('UILabel')
	share_code_ = main_panel_:Find('z_code'):GetComponent('UITexture')
	
	local close_btn = main_panel_:Find('close_btn')
	local copy_btn = main_panel_:Find('copy_btn')
	local share_btn = main_panel_:Find('share_btn')
	local hide_btn = share_panel_:Find('hide_btn')
	chest_btn_ = main_panel_:Find("chest_btn")
	
	lua_script_:AddButtonEvent(close_btn.gameObject, "click", SharePanel.Click)
	lua_script_:AddButtonEvent(copy_btn.gameObject, "click", SharePanel.Click)
	lua_script_:AddButtonEvent(share_btn.gameObject, "click", SharePanel.Click)
	lua_script_:AddButtonEvent(hide_btn.gameObject, "click", SharePanel.Click)
	lua_script_:AddButtonEvent(chest_btn_.gameObject, "click", SharePanel.Click)
	
	local share_view = share_panel_:Find('share_view')
	
	if(share_view.childCount > 0) then
		for i = 0, share_view.childCount - 1 do
            lua_script_:AddButtonEvent(share_view:GetChild(i).gameObject, "click", SharePanel.Click)
        end
	end
	main_panel_.gameObject:SetActive(false)
	share_panel_.gameObject:SetActive(false)
	SharePanel.RegisterMessage()
end

function SharePanel.OnDestroy()
	open_mode_ = 1
	share_type_ = "common"
	lua_script_ = nil
	SharePanel.RemoveMessage()
end

function SharePanel.RegisterMessage()
	Message.register_handle("team_join_msg", SharePanel.TeamJoin)
end

function SharePanel.RemoveMessage()
	Message.remove_handle("team_join_msg", SharePanel.TeamJoin)
end

function SharePanel.OnParam(parm)
	open_mode_ = parm[1]
	share_type_ = parm[2]
	if(share_type_ == nil) then
		share_type_ = "common"
	end
	if(open_mode_ == 1) then
		GUIRoot.UIEffectScalePos(main_panel_.gameObject, true, 1)
		SharePanel.InitPanel()
	elseif(open_mode_ == 2) then
		GUIRoot.UIEffectScalePos(share_panel_.gameObject, true, 1)
		share_panel_.gameObject:SetActive(true)
	end
end

function SharePanel.TeamJoin()
	GUIRoot.HideGUI('SharePanel')
end

function SharePanel.Click(obj)
	if(obj.name == "close_btn") then
		SharePanel.ClosePanel()
	elseif(obj.name == "copy_btn") then
		toolMgr:copy(self.share_url)
		GUIRoot.ShowGUI("MessagePanel", {Config.get_t_script_str('SharePanel_001')})
	elseif(obj.name == "chest_btn") then
		SharePanel.OpenChest()
	elseif(obj.name == "wxm_btn") then
		SharePanel.Share(2, share_type_)
	elseif(obj.name == "wx_btn") then
		SharePanel.Share(1, share_type_)
	elseif(obj.name == "wb_btn") then
		SharePanel.Share(PlatformType.SinaWeibo)
	elseif(obj.name == "qq_btn") then
		SharePanel.Share(PlatformType.QQ)
	elseif(obj.name == "qqz_btn") then
		SharePanel.Share(PlatformType.QZone)
	elseif(obj.name == "share_btn") then
		GUIRoot.UIEffectScalePos(share_panel_.gameObject, true, 1)
		share_panel_.gameObject:SetActive(true)
	elseif(obj.name == "hide_btn") then
		if(open_mode_ == 1) then
			SharePanel.InitPanel()
		elseif(open_mode_ == 2) then
			SharePanel.ClosePanel()
		end
	end
end

function SharePanel.OpenChest()
	if(self.player.fenxiang_state ~= 2) then
		GUIRoot.ShowGUI("ChestPanel", {3, 301})
		GUIRoot.HideGUI("SharePanel")
	end
end

function SharePanel.Share(pt)
	shareMgr:Share(pt, self.share_text[share_type_]["title"], self.share_text[share_type_]["desc"], self.share_url, self.icon_url)
end

function SharePanel.ClosePanel()
	GUIRoot.HideGUI('SharePanel')
end

function SharePanel.InitPanel()
	if(lua_script_ ~= nil) then
		if (self.player.fenxiang_state == 2) then
			chest_btn_:GetComponent("UISprite").spriteName = "b1_gray"
			chest_btn_:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('SharePanel_002') --"已领取"
		elseif(self.player.fenxiang_state == 0) then
			chest_btn_:GetComponent("UISprite").spriteName = "b1"
			chest_btn_:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('SharePanel_003') --"查看"
		elseif(self.player.fenxiang_state == 1) then
			chest_btn_:GetComponent("UISprite").spriteName = "b1_green"
			chest_btn_:Find("Label"):GetComponent("UILabel").text = Config.get_t_script_str('SharePanel_004') --"领取"
		end
		share_panel_.gameObject:SetActive(false)
		main_panel_:Find("icon"):GetComponent("UISprite").spriteName = Config.get_t_chest(301).icon
		url_label_.text = self.share_url
		share_code_.mainTexture = self.share_code
		jewel_slider_:GetComponent("UISlider").value = self.player.fenxiang_num / 20
		jewel_slider_:Find("num"):GetComponent("UILabel").text = self.player.fenxiang_num.. "/"..tostring(20)
		main_panel_.gameObject:SetActive(true)
	end
end
