GUIRoot = {}

GUIRoot.guis = {}

GUIRoot.width = 0
GUIRoot.height = 0

local panel = {}
local m_shake_time = 0
local m_shake_panel = nil
local m_shake_pos = Vector3(0, 0, 0)

function GUIRoot.Init()
	GUIRoot.guis = {}
	GUIRoot.ShowGUI("IconPanel")
	GUIRoot.ShowGUI("AvaIconPanel")
	GUIRoot.ShowGUI("NoticePanel")
end

function GUIRoot.ShowGUI(name, param)
	if(GUIRoot.guis[name] == nil) then
		GUIRoot.guis[name] = panelMgr:CreateGUI(name, nil)
		if(GUIRoot.width == 0) then
			local width = GUIRoot.guis[name].transform:GetComponent("UIPanel").width
			local height = GUIRoot.guis[name].transform:GetComponent("UIPanel").height
			local per = width / height
			if(per < 960 / 640) then
				GUIRoot.width = 960
				GUIRoot.height = math.floor(height * 960 / width)
			else
				GUIRoot.height = 640
				GUIRoot.width = math.floor(width * 640 / height)
			end
		end
	else
		GUIRoot.guis[name]:SetActive(true)
	end
	local lub = nil
	if GUIRoot.guis[name] ~= nil then
		lub = GUIRoot.guis[name].transform:GetComponent('LuaUIBehaviour')
	end
	if param ~= nil and lub ~= nil then
		lub:OnParam(param)
	end
end

function GUIRoot.HasGUI(name)
	return GUIRoot.guis[name] ~= nil
end

function GUIRoot.HideGUI(name, is_delete)
	if is_delete == nil then
		is_delete = true
	end
	if(GUIRoot.guis[name] ~= nil) then
		if is_delete then
			panelMgr:CloseGUI(name)
			GUIRoot.guis[name] = nil
		else
			GUIRoot.guis[name]:SetActive(false)
		end
	end
end

function GUIRoot.GetUIPos(t)
	local v = Vector3.zero
	while t.name ~= "UIRoot" do
		v = v + t.localPosition
		t = t.parent
	end
	return v
end

function GUIRoot.RemoveAll()
	panelMgr:RemoveAllChild(resMgr.UIRoot.gameObject)
	GUIRoot.guis = {}
end

function GUIRoot.UIEffect(obj, type_)
	if(obj.transform:GetComponent('UIPanel') ~= nil) then
		obj.transform:GetComponent('UIPanel').alpha = 0
	end
	local form =  Vector3(0, 0, 0)
	if(type_ == 0) then
		from = obj.transform.localPosition + Vector3(0, 50, 0)
	elseif(type_ == 1) then
		from = obj.transform.localPosition + Vector3(50, 0, 0)
	elseif(type_ == 2) then
		from = obj.transform.localPosition + Vector3(0, -50, 0)
	elseif(type_ == 3) then
		from = obj.transform.localPosition + Vector3(-50, 0, 0)
	end
	twnMgr:Add_Tween_Postion(obj, 0.2, from, obj.transform.localPosition)
	twnMgr:Add_Tween_Alpha(obj, 0.2, 0, 1)
end

function GUIRoot.UIEffectScalePos(obj, open, type_, from)
	if(type_ == nil) then
		type_ = 0
	end
	if(from == nil) then
		from = Vector3(0, 0, 0)
	end
	if(open) then
		if(type_ == 0) then
			twnMgr:Add_Tween_Scale(obj, 0.2, Vector3(0, 0, 0), Vector3(1, 1, 1))
			twnMgr:Add_Tween_Postion(obj, 0.2, from, Vector3(0, 0, 0))
		else
			twnMgr:Add_Tween_Scale(obj, 0.1, Vector3(1, 1, 1), Vector3(1.1, 1.1, 1))
			local param = {}
			param.obj = obj
			param.time = 0
			table.insert(panel, param)
			if(#panel == 1) then
				timerMgr:AddRepeatTimer('GUIRoot', GUIRoot.Refresh , 0.1, 0.1)
			end
		end
	else
		twnMgr:Add_Tween_Scale(obj, 0.2, Vector3(1, 1, 1), Vector3(0, 0, 0))
		twnMgr:Add_Tween_Postion(obj, 0.2, Vector3(0, 0, 0), from)
	end
end

function GUIRoot.Refresh()
	local done_panel = {}
	for i = 1, #panel do
		local param = panel[i]
		param.time = param.time + 0.1
		if(param.time >= 0.1 and param.obj.gameObject ~= nil) then
			twnMgr:Add_Tween_Scale(param.obj, 0.1, Vector3(1.1, 1.1, 1), Vector3(1, 1, 1))
			table.insert(done_panel, param)
		end
	end
	for i = 1, #done_panel do
		GUIRoot.RemovePanel(done_panel[i])
	end
	if(#panel == 0) then
		timerMgr:RemoveRepeatTimer('GUIRoot')
	end
end

function GUIRoot.Update()
	if(m_shake_panel ~= nil) then
		local m_shake_vec = Vector3(0, 0, 0)
		if(m_shake_time > 0) then
			local _addx = math.random(-m_shake_time * 10, m_shake_time * 10)
			local _addy = math.random(-m_shake_time * 10, m_shake_time * 10)
			local _addz = math.random(-m_shake_time * 10, m_shake_time * 10)
			m_shake_vec = Vector3(_addx, _addy, _addz)
			m_shake_time = m_shake_time - Time.deltaTime
			m_shake_panel.localPosition = m_shake_pos + m_shake_vec
		else
			m_shake_time = 0
			m_shake_panel.localPosition = m_shake_pos
			m_shake_panel = nil
			m_shake_pos = Vector3(0, 0, 0)
			UpdateBeat:Remove(GUIRoot.Update, GUIRoot)
		end
	end
end	

function GUIRoot.ShakePanel(time_, panel_)
	if(panel_ ~= nil and m_shake_panel == nil) then
		local seed = timerMgr:now_string()
		seed = stringTotable(seed)
		seed = table_convert(seed)
		seed = tableTostring(seed)
		math.randomseed(tonumber(seed))
		m_shake_time = time_
		m_shake_panel = panel_
		m_shake_pos = m_shake_panel.localPosition
		UpdateBeat:Add(GUIRoot.Update, GUIRoot)
	end
end

function GUIRoot.RemovePanel(param)
	for i = 1, #panel do
		if(panel[i] == param) then
			table.remove(panel, i)
			return 0
		end
	end
end