HallScene = {}

local role_unit_
local role_unit_weapon_
local role_id_ = 0
local r_ = -45
local role_show_unit_
local role_show_unit_weapon_

local ui_role_units = {}
local ui_role_weapons = {}

local show_fashion_ = {}

function HallScene.AddRole(role_temp)
	if(role_temp == nil) then
		local role = self.get_role(self.player.role_on)
		role_temp = Config.get_t_role(role.template_id)
	end
	if(role_id_ ~= role_temp.id) then
		HallScene.RemoveRole()
		role_unit_ = resMgr:CreateUnit(role_temp.res, true)
		HallScene.ChangeWeapon()
		role_unit_.transform.parent = resMgr.UnitRoot
		role_unit_.transform.localPosition = Vector3.zero
		role_unit_.transform.localEulerAngles = Vector3(0, r_, 0)
		role_unit_.transform.localScale = Vector3.one
		role_unit_weapon_ = role_unit_:GetComponent("unit"):get_bone("Bone001").gameObject
		role_unit_weapon_:SetActive(false)
		role_unit_:GetComponent("unit"):action("start")
		role_unit_:GetComponent("unit"):play_sound(role_temp.yy[1], role_unit_)
		role_id_ = role_temp.id
	end
end

function HallScene.ChangeWeapon()
	if(role_unit_ ~= nil) then
		local weapon_pack = "sn_shp002_show"
		if(self.player.fashion_on[3] ~= 0 and self.player.fashion_on[3] ~= nil) then
			local fashion_temp = Config.get_t_fashion(self.player.fashion_on[3])
			weapon_pack = fashion_temp.show
		end
		role_unit_:GetComponent("unit"):change_static_part("weapon", weapon_pack, "Bone001")
	end
end

function HallScene.ChangeShowWeapon()
	if role_show_unit_ ~= nil then
		local weapon_pack = "sn_shp002_show"
		if(self.player.fashion_on[3] ~= 0 and self.player.fashion_on[3] ~= nil) then
			local fashion_temp = Config.get_t_fashion(self.player.fashion_on[3])
			weapon_pack = fashion_temp.show
		end
		role_show_unit_:GetComponent("unit"):change_static_part("weapon", weapon_pack, "Bone001")
	end
end

function HallScene.AddFashion()
	HallScene.RemoveFashion()
	for i = 1, 2 do
		if(self.player.fashion_on[i] ~= 0 and self.player.fashion_on[i] ~= nil) then
			local fashion_temp = Config.get_t_fashion(self.player.fashion_on[i])
			fashion_t = resMgr:CreateEffect(fashion_temp.show,false)
			fashion_t.transform.parent = resMgr.UnitRoot
			if(fashion_temp.type == 1) then
				fashion_t.transform.localPosition = Vector3(1, 0.7, 0)
			elseif(fashion_temp.type == 2) then
				fashion_t.transform.localPosition = Vector3(0, 0, 0)
			end
			fashion_t.transform.localScale = Vector3.one
			table.insert(show_fashion_, fashion_t)
		end
	end
end

function HallScene.RemoveFashion()
	for i = 1, #show_fashion_ do
		GameObject.Destroy(show_fashion_[i])
	end
	show_fashion_ = {}
end

function HallScene.RemoveRole()
	if role_unit_ ~= nil then
		GameObject.Destroy(role_unit_)
		role_unit_ = nil
		role_unit_weapon_ = nil
		role_id_ = 0
	end
	r_ = -45
end

function HallScene.Roll(r)
	if role_unit_ ~= nil then
		r_ = r_ - r
		role_unit_.transform.localEulerAngles = Vector3(0, r_, 0)
	end
end

function HallScene.AddUIRole(id, pos, angle)
	if(angle == nil) then
		angle = Vector3(0, -45, 0)
	end
	role_temp = Config.get_t_role(id)
	if(role_temp ~= nil) then		
		local role_unit = resMgr:CreateUnit(role_temp.res, true)
		role_unit.transform.parent = resMgr.UnitRoot
		role_unit.transform.localPosition = pos
		role_unit.transform.localEulerAngles = angle
		role_unit.transform.localScale = Vector3.one
		local weapon_pack = "sn_shp002_show"
		if(self.player.fashion_on[3] ~= 0 and self.player.fashion_on[3] ~= nil) then
			local fashion_temp = Config.get_t_fashion(self.player.fashion_on[3])
			weapon_pack = fashion_temp.show
		end
		role_unit:GetComponent("unit"):change_static_part("weapon", weapon_pack, "Bone001")
		local role_unit_weapon = role_unit:GetComponent("unit"):get_bone("Bone001").gameObject
		role_unit_weapon:SetActive(false)
		role_unit:GetComponent("unit"):action("start")
		table.insert(ui_role_units, role_unit)
		table.insert(ui_role_weapons, role_unit_weapon)
	end
end

function HallScene.RemoveUIRole()
	for i = 1, #ui_role_units do
		GameObject.Destroy(ui_role_units[i])
	end
	ui_role_units = {}
	ui_role_weapons = {}
end

function HallScene.AddShowRole(id)
	HallScene.RemoveShowRole()
	role_temp = Config.get_t_role(id)
	if(role_temp ~= nil) then		
		local eff = resMgr:CreateEffect("Unit_createCharacter",false)
		local efft = eff.transform
		efft.parent = resMgr.UnitRoot
		efft.localPosition = Vector3(50, 0, 0)
		efft.localEulerAngles = Vector3(0, 0, 0)
		efft.localScale = Vector3.one
		GameObject.Destroy(eff, 3)
	
		timerMgr:RemoveTimer('HallScene')
		timerMgr:AddTimer('HallScene', HallScene.ShowRole , 1)
	end
end

function HallScene.RemoveShowRole()
	if role_show_unit_ ~= nil then
		GameObject.Destroy(role_show_unit_)
		role_show_unit_ = nil
		role_show_unit_weapon_ = nil
	end
end

function HallScene.ShowRole()
	role_show_unit_ = resMgr:CreateUnit(role_temp.res, true)
	HallScene.ChangeShowWeapon()
	role_show_unit_.transform.parent = resMgr.UnitRoot
	role_show_unit_.transform.localPosition = Vector3(50, 0, 0)
	role_show_unit_.transform.localEulerAngles = Vector3(0, -45, 0)
	role_show_unit_.transform.localScale = Vector3.one
	role_show_unit_weapon_ = role_show_unit_:GetComponent("unit"):get_bone("Bone001").gameObject
	role_show_unit_weapon_:SetActive(false)
	role_show_unit_:GetComponent("unit"):action("start")
	role_show_unit_:GetComponent("unit"):play_sound(role_temp.yy[1], role_show_unit_)
end

function HallScene.WeaponShow()
	if role_unit_ ~= nil and role_unit_:GetComponent("unit"):can_tiox(0.2) and role_unit_weapon_ ~= nil then
		role_unit_weapon_:SetActive(true)
	end
	if(role_show_unit_ ~= nil and role_show_unit_:GetComponent("unit"):can_tiox(0.2) and role_show_unit_weapon_ ~= nil) then
		role_show_unit_weapon_:SetActive(true)
	end
	for i = 1, #ui_role_units do
		if(ui_role_units[i] ~= nil and ui_role_units[i]:GetComponent("unit"):can_tiox(0.2)) and ui_role_weapons[i] ~= nil then
			ui_role_weapons[i]:SetActive(true)
		end
	end
end

function HallScene.Update()
	if role_unit_ ~= nil and role_unit_:GetComponent("unit"):can_tiox(5) then
		if(role_unit_weapon_ ~= nil) then
			role_unit_weapon_:SetActive(false)
		end
		role_unit_:GetComponent("unit"):action("tiox")
	end
end