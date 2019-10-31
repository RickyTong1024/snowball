AvaIconPanel = {}

local lua_script_

local avatar_list_ = {}

local www_avatar_ = {}

local url_avatar_ = {}

local sex_icon_ = {"boy_icon", "girl_icon"}

function AvaIconPanel.Awake(obj)
	lua_script_ = obj.transform:GetComponent('LuaUIBehaviour')
	
	if(obj.transform.childCount > 0) then
		for i = 0, obj.transform.childCount - 1 do
			avatar_list_[obj.transform:GetChild(i).name] = obj.transform:GetChild(i).gameObject
		end
	end
	
end

function AvaIconPanel.OnDestroy()
	avatar_list_ = {}
end

function AvaIconPanel.GetAvatar(avatar_name, click_fun, avatar_, avatar_url, frame)
	if(avatar_list_[avatar_name] ~= nil) then
		local avatar_res = LuaHelper.Instantiate(avatar_list_[avatar_name].gameObject)
		local avatar_icon = avatar_res.transform:Find('avatar'):GetComponent('UISprite')
		local frame_temp = Config.get_t_toukuang(frame)
		local avatar_temp = AvaIconPanel.GetIcon(avatar_icon, avatar_, avatar_url)
		if(avatar_temp ~= nil) then
			avatar_icon.atlas = IconPanel.GetAltas(avatar_temp.icon)
			avatar_icon.spriteName = avatar_temp.icon
		end
		if(click_fun ~= nil) then
			local click_obj = avatar_res.transform:Find("avatar")
			lua_script_:AddButtonEvent(click_obj.gameObject, "click", click_fun)
		end
		if(frame ~= nil) then
			local frame_avatar = avatar_res.transform:Find('frame'):GetComponent('UISprite')
			frame_avatar.atlas = IconPanel.GetAltas(frame_temp.big_icon)
			frame_avatar.spriteName = frame_temp.big_icon
			if(avatar_name ~= "social_res") then
				frame_avatar:MakePixelPerfect()
			end
		end
		local sex_icon = avatar_res.transform:Find('sex')
		if(sex_icon ~= nil) then
			sex_icon.gameObject:SetActive(false)
		end
		return avatar_res
	else
		return nil
	end
end

function AvaIconPanel.GetAvatarSex(avatar_name, click_fun, avatar_, avatar_url, frame, sex)
	if(avatar_list_[avatar_name] ~= nil) then
		local avatar_res = LuaHelper.Instantiate(avatar_list_[avatar_name].gameObject)
		local avatar_icon = avatar_res.transform:Find('avatar'):GetComponent('UISprite')
		local frame_temp = Config.get_t_toukuang(frame)
		local avatar_temp = AvaIconPanel.GetIcon(avatar_icon, avatar_, avatar_url)
		if(avatar_temp ~= nil) then
			avatar_icon.atlas = IconPanel.GetAltas(avatar_temp.icon)
			avatar_icon.spriteName = avatar_temp.icon
		end
		if(click_fun ~= nil) then
			local click_obj = avatar_res.transform:Find("avatar")
			lua_script_:AddButtonEvent(click_obj.gameObject, "click", click_fun)
		end
		if(frame ~= nil) then
			local frame_avatar = avatar_res.transform:Find('frame'):GetComponent('UISprite')
			frame_avatar.atlas = IconPanel.GetAltas(frame_temp.big_icon)
			frame_avatar.spriteName = frame_temp.big_icon
			if(avatar_name ~= "social_res") then
				frame_avatar:MakePixelPerfect()
			end
		end
		if(sex == nil) then
			local sex_icon = avatar_res.transform:Find('sex')
			if(sex_icon ~= nil) then
				sex_icon.gameObject:SetActive(false)
			end
		else
			local sex_icon = avatar_res.transform:Find('sex'):GetComponent('UISprite')
			sex_icon.spriteName = sex_icon_[sex + 1]
		end
		return avatar_res
	else
		return nil
	end
end

function AvaIconPanel.ModifyAvatar(avatar_t, avatar_, avatar_url, frame, sex)
	if(avatar_t ~= nil) then
		local avatar_icon = avatar_t:Find('avatar'):GetComponent('UISprite')
		local frame_temp = Config.get_t_toukuang(frame)
		local avatar_temp = AvaIconPanel.GetIcon(avatar_icon, avatar_, avatar_url)
		if(avatar_temp ~= nil) then
			avatar_icon.atlas = IconPanel.GetAltas(avatar_temp.icon)
			avatar_icon.spriteName = avatar_temp.icon
		end
		if(frame ~= nil) then
			local frame_avatar = avatar_t:Find('frame'):GetComponent('UISprite')
			frame_avatar.atlas = IconPanel.GetAltas(frame_temp.big_icon)
			frame_avatar.spriteName = frame_temp.big_icon
			frame_avatar:MakePixelPerfect()
		end
		if(sex ~= nil) then
			local sex_icon = avatar_t:Find('sex'):GetComponent('UISprite')
			sex_icon.spriteName = sex_icon_[sex + 1]
		end
	end
end

function AvaIconPanel.GetIcon(avatar_t, avatar, avatar_url)
	return Config.get_t_avatar(avatar)
end

function AvaIconPanel.GetAvatarIconUrl(www)
	if(www_avatar_[www.url] ~= nil) then
		www_avatar_[www.url].mainTexture = www.texture
		www_avatar_[www.url] = nil
		local url_param = {}
		url_param.time = 0
		url_param.texture = www.texture
		url_avatar_[www.url] = url_param
	end
	local i = 0
	for k, v in pairsByKeys(www_avatar_) do
		if(v ~= nil) then
			i = i + 1
		end
	end
	if(i == 0) then
		www_avatar_ = {}
	end
end

function AvaIconPanel.Refresh()
	
end