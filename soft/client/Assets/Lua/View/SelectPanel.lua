SelectPanel = {}

local main_panel_

local single_panel_
local double_panel_
local label_des_
local label_sing_yes_
local label_double_yes_
local label_double_no_
local single_func_
local double_func_yes_
local double_func_no_

local bg
local off_height = 241
--启动事件--
function SelectPanel.Awake(obj)
	local lua_script = obj.transform:GetComponent('LuaUIBehaviour')
	
	main_panel_ = obj
	
	single_panel_ = obj.transform:Find('single_panel')
	double_panel_ = obj.transform:Find('double_panel')
	
	local btn_single = single_panel_:Find('single')
	local btn_double_yes = double_panel_:Find('yes')
	local btn_double_no = double_panel_:Find('no')
	
	bg = obj.transform:Find('baseboard')
	label_des_ = obj.transform:Find('des'):GetComponent('UILabel')
	label_sing_yes_ = btn_single:Find('Label'):GetComponent('UILabel')
	label_double_yes_ = btn_double_yes:Find('Label'):GetComponent('UILabel')
	label_double_no_ = btn_double_no:Find('Label'):GetComponent('UILabel')
	
	lua_script:AddButtonEvent(btn_single.gameObject, "click", SelectPanel.SingleSelect)
	lua_script:AddButtonEvent(btn_double_yes.gameObject, "click", SelectPanel.DoubleSelectYes)
	lua_script:AddButtonEvent(btn_double_no.gameObject, "click", SelectPanel.DoubleSelectNo)
	
	SelectPanel.InitUI()
	obj:SetActive(false)
end

function SelectPanel.OnDestroy()
end

function SelectPanel.OnParam(param)
	single_func_ = nil
	double_func_yes_ = nil
	double_func_no_ = nil
	if(#param <= 3) then
		SelectPanel.ShowSingleDialog(param)
	else
		SelectPanel.ShowDoubleDialog(param)
	end
	GUIRoot.UIEffectScalePos(main_panel_, true, 1)
end

function SelectPanel.ShowSingleDialog(param)
	label_des_.text = param[1]
	label_sing_yes_.text = param[2]
	single_func_ = param[3]
	SelectPanel.Refresh()
	single_panel_.gameObject:SetActive(true)
    single_panel_.parent.gameObject:SetActive(true)
end

function SelectPanel.ShowDoubleDialog(param)
	label_des_.text = param[1]
	label_double_yes_.text = param[2]
	double_func_yes_ = param[3]
	label_double_no_.text = param[4]
	double_func_no_ = param[5]
	SelectPanel.Refresh()
    double_panel_.gameObject:SetActive(true)
    double_panel_.parent.gameObject:SetActive(true)
end

function SelectPanel.SingleSelect()
	if(single_func_ ~= nil) then
		single_func_()
	end
	GUIRoot.HideGUI("SelectPanel")
end

function SelectPanel.DoubleSelectYes()
	if(double_func_yes_ ~= nil) then
		double_func_yes_()
	end
	GUIRoot.HideGUI("SelectPanel")
end

function SelectPanel.DoubleSelectNo()
	if(double_func_no_ ~= nil) then
		double_func_no_()
	end
	GUIRoot.HideGUI("SelectPanel")
end

function SelectPanel.InitUI()
	double_panel_.gameObject:SetActive(false)
	single_panel_.gameObject:SetActive(false)
end

function SelectPanel.Refresh()
	label_des_:ProcessText()
	local height = label_des_.transform:GetComponent("UIWidget").height
	if(height >= 76) then
		bg:GetComponent("UIWidget").height = height - 76 + off_height
	end
end