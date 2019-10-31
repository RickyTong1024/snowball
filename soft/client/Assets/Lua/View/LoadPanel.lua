LoadPanel = {}

local desc_
local pro_slider_
local anim_sprite_
local pro_fg_sprite_
local progress_ = 0
local autoclose_ = true
local mus_play_end_ = ""
local end_time_ = 0
local end_state_ = 0
local func_ = nil

function LoadPanel.Awake(obj)
	local lua_script = obj.transform:GetComponent('LuaUIBehaviour')
	
	desc_ = obj.transform:Find('Anchor_bottom/des'):GetComponent('UILabel')
	pro_slider_ = obj.transform:Find('Anchor_bottom/pro_slider'):GetComponent('UISlider')
	pro_fg_sprite_ = obj.transform:Find('Anchor_bottom/pro_slider'):GetComponent('UISprite')
	anim_sprite_ = obj.transform:Find('Anchor_bottom/am_sprite')
	
	obj.transform:Find('Anchor_bottom/tip'):GetComponent('UILabel').text = Config.get_t_tips().desc
	
	--pro_slider_.transform:GetComponent("UIWidget").width = GUIRoot.width - 160
	--pro_slider_.transform:Find("fg"):GetComponent("UIWidget").width = GUIRoot.width - 160
	
	progress_ = 0
	desc_.text = Config.get_t_script_str('LoadPanel_001') --"加载中..（加载过程中不消耗流量）"
	
	pro_slider_.value = progress_
	autoclose_ = true
	end_time_ = 0
	end_state_ = 0
	UpdateBeat:Add(LoadPanel.Update, LoadPanel)
end

function LoadPanel.OnParam(param)
	autoclose_ = param[1]
	mus_play_end_ = param[2]
	func_ = nil
end

function LoadPanel.Update()
	local content = string.format(Config.get_t_script_str('LoadPanel_002'),math.floor(progress_ * 100))
	desc_.text = content --"加载中.."..math.floor(progress_ * 100).."%（加载过程中不消耗流量）"
	local p = mapMgr:LoadProgress()
	if progress_ < p then
		progress_ = progress_ + Time.deltaTime / 2
	else
		progress_ = p
	end
	if(progress_ >= 1) then
		desc_.text = Config.get_t_script_str('LoadPanel_003') --"加载完成"
		progress_ = 1
		end_time_ = end_time_ + Time.deltaTime
		if autoclose_ then
			LoadPanel.End()
		end
	end
	pro_slider_.value = progress_
	anim_sprite_.localPosition = Vector3((GUIRoot.width - 160)* progress_ - GUIRoot.width / 2 + 100 - progress_ * 40, anim_sprite_.localPosition.y, 0)
end

function LoadPanel.OnDestroy()
	progress_ = 0
	mus_play_end_ = nil
	UpdateBeat:Remove(LoadPanel.Update, LoadPanel)
end

function LoadPanel.End()
	if end_state_ == 0 then
		if end_time_ > 0.1 then
			if(mus_play_end_ ~= nil) then
				soundMgr:play_mus(mus_play_end_)
			end
			if func_ ~= nil then
				func_()
			end
			end_state_ = 1
		end
	elseif end_time_ > 0.2 then
		GUIRoot.HideGUI('LoadPanel')
	end
end

function LoadPanel.close(func)
	autoclose_ = true
	func_ = func
end

function LoadPanel.csharpclose()
	autoclose_ = true
	func_ = Battle.LanGan
end