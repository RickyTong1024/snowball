MaskPanel = {}

local desc_label_

function MaskPanel.Awake(obj)
	local lua_script = obj.transform:GetComponent('LuaUIBehaviour')
	obj.transform:GetComponent('Collider').size = Vector3(GUIRoot.width, GUIRoot.height, 0)
	desc_label_ = obj.transform:Find('desc'):GetComponent('UILabel')
	desc_label_.text = ""
end

function MaskPanel.OnDestroy()
	
end

function MaskPanel.OnParam(parm)
	local tip = parm[1]
	if(tip ~= nil) then
		desc_label_.text = tip
	else
		desc_label_.text = Config.get_t_script_str('MaskPanel_001')--'连接中'
	end
end
