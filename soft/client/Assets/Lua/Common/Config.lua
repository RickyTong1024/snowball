Config = {}

------------------ t_name ------------------
Config.t_name = {}

function Config.parse_t_name()
	local dbc_name = dbc.New()
	dbc_name:load_txt("t_name")
	local firname = {}
	local lastname = {}
	local engName = {}
	for i = 0, dbc_name:get_y() - 1 do
		local fir = dbc_name:get_string(0, i)
		local last = dbc_name:get_string(1, i)
		local third = dbc_name:get_string(2, i)
		if(fir ~= '') then
			table.insert(firname, fir)
		end
		if(last ~= '') then
			table.insert(lastname, last)
		end
		if third ~= '' then
			table.insert(engName, third)
		end
	end
	table.insert(Config.t_name, firname)
	table.insert(Config.t_name, lastname)
	table.insert(Config.t_name, engName)
end

function Config.get_t_name()
	local fir = Config.t_name[1][math.random(#Config.t_name[1])]
	local last = Config.t_name[2][math.random(#Config.t_name[2])]
	return table.concat({fir, last})
end

----------------------------------------------

---------------------t_role_skill----------------------------
Config.t_role_skill = {}

function Config.parse_t_role_skill()
	local dbc_role_skill = dbc.New()
	dbc_role_skill:load_txt("t_role_skill")
	for i = 0, dbc_role_skill:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_role_skill:get_int(0, i)
		tdic.name = dbc_role_skill:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.desc = dbc_role_skill:get_string(4, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		tdic.attrs = {}
		for j = 0, 1 do
			local attr = {}
			attr.type = dbc_role_skill:get_int(5 + j * 5, i)
			attr.param1 = dbc_role_skill:get_int(6 + j * 5, i)
			attr.param2 = dbc_role_skill:get_int(7 + j * 5, i)
			attr.param3 = dbc_role_skill:get_int(8 + j * 5, i)
			attr.param4 = dbc_role_skill:get_int(9 + j * 5, i)
			attr.param_value = function(level)
				return attr.param3 + attr.param4 * (level - 1)
			end
			if attr.type > 0 then
				table.insert(tdic.attrs, attr)
			end
		end
		Config.t_role_skill[tdic.id] = tdic
	end
end

function Config.get_t_role_skill(id)
	if Config.t_role_skill[id] == nil then
		return nil
	else
		return Config.t_role_skill[id]
	end
end
-------------------------------------------------------------

---------------------t_role_buff-----------------------------
Config.t_role_buff = {}

function Config.parse_t_role_buff()
	local dbc_role_buff = dbc.New()
	dbc_role_buff:load_txt("t_role_buff")
	for i = 0, dbc_role_buff:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_role_buff:get_int(0, i)
		tdic.name = dbc_role_buff:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.desc = dbc_role_buff:get_string(4, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		tdic.type = dbc_role_buff:get_int(5, i)
		tdic.param1 = dbc_role_buff:get_int(6, i)
		tdic.param2 = dbc_role_buff:get_int(7, i)
		tdic.param3 = dbc_role_buff:get_int(8, i)
		tdic.param4 = dbc_role_buff:get_int(9, i)
		Config.t_role_buff[tdic.id] = tdic
		
		tdic.param_value = function(level)
			return tdic.param3 + tdic.param4 * (level - 1)
		end
	end
end

function Config.get_t_role_buff(id)
	if Config.t_role_buff[id] == nil then
		return nil
	else
		return Config.t_role_buff[id]
	end
end

-------------------------------------------------------------


---------------------t_role_level----------------------------
Config.t_role_level = {}
Config.max_role_lev = 0
function Config.parse_t_role_level()
	local dbc_role_level = dbc.New()
	dbc_role_level:load_txt("t_role_level")
	for i = 0, dbc_role_level:get_y() - 1 do
		local tdic = {}
		tdic.level = dbc_role_level:get_int(0, i)
		tdic.suipian_cost = dbc_role_level:get_int(1, i)
		tdic.gold_cost = {}
		tdic.exp = {}
		for j = 1, 3 do
			table.insert(tdic.gold_cost, dbc_role_level:get_int(j + 1, i))
		end
		tdic.get_gold_cost = function(color)
			local gold = tdic.gold_cost[color]
			if color == 2 then
				gold = toInt(gold * (100 - self.get_out_attr(3)) / 100)
			elseif color == 3 then
				gold = toInt(gold * (100 - self.get_out_attr(4)) / 100)
			end
			return gold
		end
		
		Config.t_role_level[tdic.level] = tdic
		Config.max_role_lev = Config.max_role_lev + 1
		
	end
end

function Config.get_t_role_level(level)
	if Config.t_role_level[level] == nil then
		return nil
	else
		return Config.t_role_level[level]
	end
end
-------------------------------------------------------------

------------------ t_item --------------------
Config.t_item = {}
Config.t_item_ids = {}

function Config.parse_t_item()
	local dbc_item = dbc.New()
	dbc_item:load_txt("t_item")
	for i = 0, dbc_item:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_item:get_int(0, i)
		tdic.name = dbc_item:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.color = dbc_item:get_int(3, i)
		tdic.type = dbc_item:get_int(4, i)
		if(tdic.type == 1001) then
			tdic.color = tdic.color + 10
		end
		tdic.level = dbc_item:get_int(5, i)
		tdic.desc = dbc_item:get_string(7, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		tdic.icon = dbc_item:get_string(8, i)
		tdic.price = dbc_item:get_int(9, i)
		tdic.sell_type = dbc_item:get_int(10, i)
		tdic.sell = dbc_item:get_int(11, i)
		tdic.def1 = dbc_item:get_int(12, i)
		tdic.def2 = dbc_item:get_int(13, i)
		tdic.def3 = dbc_item:get_int(14, i)
		tdic.def4 = dbc_item:get_int(15, i)
		Config.t_item[tdic.id] = tdic
		table.insert(Config.t_item_ids, tdic.id)
	end
end

function Config.get_t_item(id)
	if Config.t_item[id] == nil then
		return nil
	else
		return Config.t_item[id]
	end
end
----------------------------------------------

--------------------t_item_box----------------
Config.t_item_box = {}

function Config.parse_t_item_box()
	local dbc_item_box = dbc.New()
	dbc_item_box:load_txt("t_itembox")
	for i = 0, dbc_item_box:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_item_box:get_int(0, i)
		tdic.index = dbc_item_box:get_int(2, i)
		tdic.rewards = {}
		for j = 1, (dbc_item_box:get_x() - 3) / 5 do
			if dbc_item_box:get_int(3 + (j - 1) * 5, i) ~= 0 then
				local reward = {}
				reward.type = dbc_item_box:get_int(3 + (j - 1) * 5, i)
				reward.value1 = dbc_item_box:get_int(4 + (j - 1) * 5, i)
				reward.value2 = dbc_item_box:get_int(5 + (j - 1) * 5, i)
				reward.value3 = dbc_item_box:get_int(6 + (j - 1) * 5, i)
				table.insert(tdic.rewards, reward)
			end
		end
		Config.t_item_box[tdic.id] = tdic
	end
end

function Config.get_t_item_box(id)
	if Config.t_item_box[id] == nil then
		return nil
	else
		return Config.t_item_box[id]
	end
end

-----------------------------------------------

----------------------t_resource---------------
Config.t_resource = {}

function Config.parse_t_resource()
	local dbc_resource = dbc.New()
	dbc_resource:load_txt("t_resource")
	for i = 0, dbc_resource:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_resource:get_int(0, i)
		tdic.name = dbc_resource:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.color = dbc_resource:get_int(3, i)
		tdic.icon = dbc_resource:get_string(5, i)
		tdic.small_icon = dbc_resource:get_string(7, i)
		tdic.mid_icon = dbc_resource:get_string(6, i)
		tdic.desc = dbc_resource:get_string(9, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		Config.t_resource[tdic.id] = tdic
	end
end

function Config.get_t_resource(id)
	if Config.t_resource[id] == nil then
		return nil
	else
		return Config.t_resource[id]
	end
end
-----------------------------------------------


------------------ t_role --------------------
Config.t_role = {}
Config.t_role_blue_ids = {}

function Config.parse_t_role()
	local dbc_role = dbc.New()
	dbc_role:load_txt("t_role")
	for i = 0, dbc_role:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_role:get_int(0, i)
		tdic.name = dbc_role:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.color = dbc_role:get_int(3, i)
		tdic.sex = dbc_role:get_int(4, i)
		tdic.icon = dbc_role:get_string(5, i)
		tdic.res = dbc_role:get_string(6, i)
		tdic.desc = dbc_role:get_string(7, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		tdic.yy = {}
		for j = 0, 3 do
			local yy = dbc_role:get_string(8 + j, i)
			table.insert(tdic.yy, yy)
		end
		tdic.hp = dbc_role:get_int(12, i)
		tdic.hp_add = dbc_role:get_int(13, i)
		tdic.atk = dbc_role:get_int(14, i)
		tdic.atk_add = dbc_role:get_int(15, i)
		tdic.def = dbc_role:get_int(16, i)
		tdic.def_add = dbc_role:get_int(17, i)
		tdic.range = dbc_role:get_int(18, i)
		tdic.aspeed = dbc_role:get_int(19, i)
		tdic.speed = dbc_role:get_int(20, i)
		tdic.gskills = {}
		for k = 0, 2 do
			local bs = dbc_role:get_int(21 + k, i)
			if bs > 0 then
				table.insert(tdic.gskills, bs)
			end
		end
		tdic.bskills = {}
		for k = 0, 2 do
			local bs = dbc_role:get_int(24 + k, i)
			if bs > 0 then
				table.insert(tdic.bskills, bs)
			end
		end
		tdic.suipian_id = dbc_role:get_int(27, i)
		tdic.suipian_cost = dbc_role:get_int(28, i)
		tdic.type = dbc_role:get_int(29, i)
		tdic.get_type = dbc_role:get_int(30, i)
		Config.t_role[tdic.id] = tdic
		if tdic.color == 1 then
			table.insert(Config.t_role_blue_ids, tdic.id)
		end
	end
end

function Config.get_t_role(id)
	if Config.t_role[id] == nil then
		return nil
	else
		return Config.t_role[id]
	end
end

function Config.get_random_role()
	if Config.t_role[id] == nil then
		return nil
	else
		return Config.t_role[id]
	end
end
----------------------------------------------

------------------ t_avatar --------------------
Config.t_avatar = {}

Config.t_avatar_ids = {}

function Config.parse_t_avatar()
	local dbc_avatar = dbc.New()
	dbc_avatar:load_txt("t_avatar")
	for i = 0, dbc_avatar:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_avatar:get_int(0, i)
		tdic.name = dbc_avatar:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.role_id = dbc_avatar:get_int(3, i)
		tdic.icon = dbc_avatar:get_string(4, i)
		tdic.desc1 = dbc_avatar:get_string(5, i)
		tdic.desc1 = Config.get_t_lang(tdic.desc1)
		tdic.desc2 = dbc_avatar:get_string(6, i)
		tdic.desc2 = Config.get_t_lang(tdic.desc2)
		tdic.color = dbc_avatar:get_int(7, i)
		tdic.lock = dbc_avatar:get_int(8, i)
		Config.t_avatar[tdic.id] = tdic
		table.insert(Config.t_avatar_ids, tdic.id)
	end
end

function Config.get_t_avatar(id)
	if Config.t_avatar[id] == nil then
		return nil
	else
		return Config.t_avatar[id]
	end
end

function Config.get_t_avatar_id(role_id)
	for k, v in pairsByKeys(Config.t_avatar) do
		if(role_id == v.role_id) then
			return v
		end
	end
	return nil
end
----------------------------------------------


------------------ t_toukuang --------------------
Config.t_toukuang = {}
Config.t_toukuang_ids = {}

function Config.parse_t_toukuang()
	local dbc_toukuang = dbc.New()
	dbc_toukuang:load_txt("t_toukuang")
	for i = 0, dbc_toukuang:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_toukuang:get_int(0, i)
		tdic.name = dbc_toukuang:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.big_icon = dbc_toukuang:get_string(3, i)
		tdic.color = dbc_toukuang:get_int(4, i)
		tdic.desc1 = dbc_toukuang:get_string(5, i)
		tdic.desc1 = Config.get_t_lang(tdic.desc1)
		tdic.desc2 = dbc_toukuang:get_string(6, i)
		tdic.desc2 = Config.get_t_lang(tdic.desc2)
		tdic.icon = dbc_toukuang:get_string(8, i)
		tdic.gskills = {}
		for j = 1, 3 do
			if dbc_toukuang:get_int(9 + (j - 1) * 4, i) ~= 0 then
				local gskill = {}
				gskill.type = dbc_toukuang:get_int(9 + (j - 1) * 4, i)
				gskill.param1 = dbc_toukuang:get_int(10 + (j - 1) * 4, i)
				gskill.param2 = dbc_toukuang:get_int(11 + (j - 1) * 4, i)
				gskill.param3 = dbc_toukuang:get_int(12 + (j - 1) * 4, i)
				table.insert(tdic.gskills, gskill)
			end
		end
		tdic.desc = tdic.desc1.."\n"..tdic.desc2
		tdic.lock = dbc_toukuang:get_int(21, i)
		Config.t_toukuang[tdic.id] = tdic
		table.insert(Config.t_toukuang_ids, tdic.id)
	end
end

function Config.get_t_toukuang(id)
	if Config.t_toukuang[id] == nil then
		return nil
	else
		return Config.t_toukuang[id]
	end
end

----------------------------------------------


-------------------t_lang-----------------------
Config.t_lang = {}

function Config.parse_t_lang()
	local dbc_lang = dbc.New()
	dbc_lang:load_txt("t_lang")
	for i = 0, dbc_lang:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_lang:get_string(0, i)
		tdic.lang = {}
		
		for j = 1, dbc_lang:get_x() - 1 do
			local lang = string.gsub(dbc_lang:get_string(j, i), "##", "\n")
			table.insert(tdic.lang, lang)
		end
		Config.t_lang[tdic.id] = tdic
	end
end

function Config.get_t_lang(id)
	if Config.t_lang[id] == nil then
		return ""
	else
		if #Config.t_lang[id].lang > platform_config_common.languageType then
			return Config.t_lang[id].lang[platform_config_common.languageType + 1]
		else
			return Config.t_lang[id].lang[1]
		end	
	end
end

------------------------------------------------

----------------------t_biaoqing--------------------------
Config.t_biaoqing = {}

function Config.parse_t_biaoqing()
	local dbc_biaoqing = dbc.New()
	dbc_biaoqing:load_txt('t_biaoqing')
	for i = 0, dbc_biaoqing:get_y() - 1 do
		local tdic = {}
		tdic.id  = dbc_biaoqing:get_int(0, i)
		tdic.desc = dbc_biaoqing:get_string(1, i)
		tdic.icon = dbc_biaoqing:get_string(2, i)
		Config.t_biaoqing[tdic.id] = tdic
	end
end

function Config.get_t_biaoqing(id)
	if Config.t_biaoqing[id] == nil then
		return nil
	else
		return Config.t_biaoqing[id]
	end
end
----------------------------------------------

-----------------------t_region----------------------------

Config.t_country = {}
Config.t_province = {}
Config.t_city = {}

function Config.parse_t_region()
	local dbc_region = dbc.New()
	dbc_region:load_txt('t_address')
	for i = 0, dbc_region:get_y() - 1 do
		local tdic = {}
		tdic.country = dbc_region:get_string(0, i)
		tdic.province = dbc_region:get_string(1, i)
		tdic.city = dbc_region:get_string(2, i)
		if(Config.t_province[tdic.country] == nil) then
			Config.t_province[tdic.country] = {}
			table.insert(Config.t_country, tdic.country)
		end
		if(Config.t_city[tdic.province] == nil) then
			Config.t_city[tdic.province] = {}
		end
		if(not table_contain(Config.t_province[tdic.country], tdic.province)) then
			table.insert(Config.t_province[tdic.country], tdic.province)
		end
		table.insert(Config.t_city[tdic.province], tdic.city)
	end
end

function Config.get_t_province(country)
	if Config.t_province[country] == nil then
		return nil
	else
		return Config.t_province[country]
	end
end

function Config.get_random_province()
	local c = Config.t_country[random(1, #Config.t_country + 1)]
	return Config.t_province[c][random(1, #Config.t_province[c] + 1)]
end

function Config.get_t_city(province)
	if Config.t_city[province] == nil then
		return nil
	else
		return Config.t_city[province]
	end
end
-----------------------------------------------------------

-----------------------t_exp----------------------------------

Config.t_exp = {}
Config.max_level = 1

function Config.parse_t_exp()
	local dbc_exp = dbc.New()
	dbc_exp:load_txt('t_exp')
	for i = 0, dbc_exp:get_y() - 1 do
		local tdic = {}
		tdic.level = dbc_exp:get_int(0, i)
		tdic.exp = dbc_exp:get_int(1, i)
		tdic.rewards = {}
		for j = 1,3 do
			local reward = {}
			reward.type = dbc_exp:get_int(2 + (j - 1) * 4, i)
			if reward.type > 0 then
				reward.value1 = dbc_exp:get_int(3 + (j - 1) * 4, i)
				reward.value2 = dbc_exp:get_int(4 + (j - 1) * 4, i)
				reward.value3 = dbc_exp:get_int(5 + (j- 1) * 4, i)
				table.insert(tdic.rewards,reward)
			end			
		end

		tdic.tasks = {}
		for j = 1,3 do
			local task_id = dbc_exp:get_int(14 + j - 1, i)
			if task_id > 0 then
				table.insert(tdic.tasks,task_id)
			end
		end
		
		tdic.level_add = {}
		for j = 0, 6 do
			local add = {}
			add.type = dbc_exp:get_int(j * 4 + 17 , i)
			if(add.type ~= 0) then
				add.param1 = dbc_exp:get_int(j * 4 + 18, i)
				add.param2 = dbc_exp:get_int(j * 4 + 19, i)
				add.param3 = dbc_exp:get_int(j * 4 + 20, i)
				table.insert(tdic.level_add, add)
			end
		end
		Config.t_exp[tdic.level] = tdic
		if tdic.level > Config.max_level then
			Config.max_level = tdic.level
		end
	end
end

function Config.get_t_exp(level)
	if Config.t_exp[level] == nil then
		return nil
	else
		return Config.t_exp[level]
	end
end

function Config.get_t_acc_add(type, id)
	if(type == 1) then
		return nil
	elseif(type == 2) then
		return nil
	elseif(type == 3) then
		return Config.get_t_attr(id)
	end
end

---------------------------------------------------------------

-------------------------t_cup----------------------------------

Config.t_cup = {}

function Config.parse_t_cup()
	local dbc_cup = dbc.New()
	dbc_cup:load_txt('t_cup')
	for i = 0, dbc_cup:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_cup:get_int(0, i)
		tdic.name = dbc_cup:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.star = dbc_cup:get_int(3, i)
		tdic.max_star = dbc_cup:get_int(4, i)
		tdic.icon = dbc_cup:get_string(5, i)
		tdic.small_icon = dbc_cup:get_string(6, i)
		tdic.dai_icon = dbc_cup:get_string(7, i)
		tdic.duan = dbc_cup:get_int(8, i)
		tdic.down = dbc_cup:get_int(9, i)
		tdic.sb = dbc_cup:get_int(10, i)
		tdic.jb = dbc_cup:get_int(11, i)
		tdic.tsb = dbc_cup:get_int(12, i)
		tdic.tjb = dbc_cup:get_int(13, i)
		tdic.tsbnum= dbc_cup:get_int(14, i)
		tdic.rewards = {}
		for j = 0, 2 do
			local reward = {}
			reward.type = dbc_cup:get_int(15 + j * 4, i)
			reward.value1 = dbc_cup:get_int(16 + j * 4, i)
			reward.value2 = dbc_cup:get_int(17 + j * 4, i)
			reward.value3 = dbc_cup:get_int(18 + j * 4, i)
			if reward.type ~= 0 then
				table.insert(tdic.rewards, reward)
			end
		end
		table.insert(Config.t_cup, tdic)
	end
end

function Config.get_t_cup(id)
	if id < 0 then
		return nil
	elseif id >= #Config.t_cup then
		id = #Config.t_cup - 1
	end
	return Config.t_cup[id + 1]
end

function Config.get_t_cup_lv(lv)
	for i = 1, #Config.t_cup do
		if(Config.t_cup[i].duan == lv and Config.t_cup[i].star == 0) then
			return Config.t_cup[i]
		end
	end
	return nil
end

----------------------------------------------------------------


--------------------------t_shop--------------------------------

Config.t_shop = {}

function Config.parse_t_shop()
	local dbc_shop = dbc.New()
	dbc_shop:load_txt('t_shop')
	for i = 0, dbc_shop:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_shop:get_int(0, i)
		tdic.type = dbc_shop:get_int(1, i)
		tdic.item_type = dbc_shop:get_int(2, i)
		tdic.value1 = dbc_shop:get_int(3, i)
		tdic.value2 = dbc_shop:get_int(4, i)
		tdic.value3 = dbc_shop:get_int(5, i)
		tdic.name = dbc_shop:get_string(6, i)
		tdic.past_type = dbc_shop:get_int(7, i)
		tdic.price = dbc_shop:get_int(8, i)
		tdic.icon = dbc_shop:get_string(9, i)
		tdic.color = dbc_shop:get_int(10, i)
		tdic.pre_item = dbc_shop:get_int(11, i)
		Config.t_shop[tdic.id] = tdic
	end
end

function Config.get_t_shop(id)
	if Config.t_shop[id] == nil then
		return nil
	else
		return Config.t_shop[id]
	end
end

function Config.get_shop_by_item(id)
	for k, v in pairsByKeys(Config.t_shop) do
		if(v.value1 == id) then
			return v
		end
	end
	return nil
end

----------------------------------------------------------------

--------------------------t_error--------------------------------

Config.t_error = {}

function Config.parse_t_error()
	local dbc_error = dbc.New()
	dbc_error:load_txt('t_error')
	for i = 0, dbc_error:get_y() - 1 do
		local id = dbc_error:get_int(0, i)
		local text = dbc_error:get_string(2, i)
		text = Config.get_t_lang(text)
		Config.t_error[id] = text
	end
end

function Config.get_t_error(id)
	if Config.t_error[id] == nil then
		return ""
	else
		return Config.t_error[id]
	end
end
----------------------------------------------------------------


-------------------------t_chest--------------------------------

Config.t_chest = {}

function Config.parse_t_chest()
	local dbc_chest = dbc.New()
	dbc_chest:load_txt('t_chest')
	for i = 0, dbc_chest:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_chest:get_int(0, i)
		tdic.name = dbc_chest:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.color = dbc_chest:get_int(3, i)
		tdic.icon = dbc_chest:get_string(4, i)
		tdic.time = dbc_chest:get_int(6, i)
		tdic.gold_min = dbc_chest:get_int(7, i)
		tdic.gold_max = dbc_chest:get_int(8, i)
		tdic.item_num = dbc_chest:get_int(9, i)
		tdic.treasure_num = dbc_chest:get_int(10, i)
		
		tdic.get_time = function()
			return toInt(tdic.time * (100 - self.get_out_attr(6)) / 100)
		end
		tdic.get_gold_min = function()
			return tdic.gold_min + self.get_out_attr(5)
		end
		tdic.get_gold_max = function()
			return tdic.gold_max + self.get_out_attr(5)
		end
		
		Config.t_chest[tdic.id] = tdic
	end
end

function Config.get_t_chest(id)
	if Config.t_chest[id] == nil then
		return nil
	else
		return Config.t_chest[id]
	end
end

--------------------------------------------------------------------


-------------------------t_attr-----------------------------------

Config.t_attr = {}

function Config.parse_t_attr()
	local dbc_attr = dbc.New()
	dbc_attr:load_txt('t_attr')
	for i = 0, dbc_attr:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_attr:get_int(0, i)
		tdic.desc = dbc_attr:get_string(3, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		Config.t_attr[tdic.id] = tdic
	end
end

function Config.get_t_attr(id)
	if Config.t_attr[id] == nil then
		return nil
	else
		return Config.t_attr[id]
	end
end

--------------------------------------------------------------------

---------------------------t_achievement----------------------------

Config.t_achievement = {}
Config.t_achievement_type = {} --同一种类型

function Config.parse_t_achievement()
	local dbc_achieve = dbc.New()
	dbc_achieve:load_txt('t_achievement')
	local types = {} --类型分类
	local achieveTrees = {}  --成就树
	Config.t_achievement.achievesTB = {}
	
	for i = 0, dbc_achieve:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_achieve:get_int(0, i)
		tdic.name = dbc_achieve:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.desc = dbc_achieve:get_string(4, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		tdic.dtype = dbc_achieve:get_int(5, i)
		tdic.pre = dbc_achieve:get_int(6, i)
		tdic.target_num = dbc_achieve:get_int(7, i)
		tdic.type_id = dbc_achieve:get_int(8, i)
		tdic.param1 = dbc_achieve:get_int(9,i)
		tdic.param2 = dbc_achieve:get_int(10,i)
		tdic.param3 = dbc_achieve:get_int(11,i)
		tdic.param4 = dbc_achieve:get_int(12,i)
		tdic.point = dbc_achieve:get_int(13,i)
		tdic.max_star = dbc_achieve:get_int(14,i)
		tdic.star = dbc_achieve:get_int(15,i)
		tdic.icon = dbc_achieve:get_string(16,i)
		tdic.lock = dbc_achieve:get_int(17,i)
		if types[tdic.type_id] == nil then
			types[tdic.type_id] = {}
		end
		
		table.insert(types[tdic.type_id], tdic)
		
		Config.t_achievement.achievesTB[tdic.id] = tdic
	end
	
	local searchState = {}
	for k,v in pairs(Config.t_achievement.achievesTB) do
		if not searchState[k] then
			local list = {}
			local tmp = v
			while (tmp.pre > 0) do
				table.insert(list,1,tmp)
				searchState[tmp.id] = true
				tmp = Config.t_achievement.achievesTB[tmp.pre]
			end
			table.insert(list,1,tmp)
			
			if achieveTrees[tmp.id] ~= nil then
				if #achieveTrees[tmp.id] < #list then
					achieveTrees[tmp.id] = list
				end
			else
				achieveTrees[tmp.id] = list
			end
		end
	end
	
	Config.t_achievement.types = types	
	Config.t_achievement.achieveTrees = achieveTrees
	
	Config.t_achievement_type = {} 
	for k,v in pairs(achieveTrees) do
		local t_achievement = Config.get_t_achievement(k)
		if Config.t_achievement_type[t_achievement.type_id] == nil then
			Config.t_achievement_type[t_achievement.type_id] = {}
		end
		table.insert(Config.t_achievement_type[t_achievement.type_id],v)
	end
	
	--每个技能的后续技能
	for k,v in pairs(Config.t_achievement.achieveTrees) do
		if #v == 0 then
			Config.get_t_achievement(k).next_achieve_id = nil 
		else
			Config.get_t_achievement(k).next_achieve_id = v[1].id
		end
		
		for i = 1,#v do
			if i ~= #v then
				v[i].next_achieve_id = v[i + 1].id
			else
				v[i].next_achieve_id = nil
			end
		end	
	end
end

function Config.get_t_achievement(id)
   if Config.t_achievement.achievesTB[id] == nil then
	 return nil
   else
	 return Config.t_achievement.achievesTB[id]
   end
end

--------------------------------------------------------------------

------------------------------t_talent------------------------------
Config.t_talent = {}

function Config.parse_t_talent()
	local dbc_talent = dbc.New()
	dbc_talent:load_txt('t_talent')
	
	for i = 0, dbc_talent:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_talent:get_int(0, i)
		tdic.desc1 = dbc_talent:get_string(2, i)
		tdic.desc1 = Config.get_t_lang(tdic.desc1)
		tdic.desc2 = dbc_talent:get_string(4, i)
		tdic.desc2 = Config.get_t_lang(tdic.desc2)
		tdic.max_level = dbc_talent:get_int(5,i)
		tdic.icon = dbc_talent:get_string(6, i)
		tdic.type = dbc_talent:get_int(7,i)
		tdic.param1 = dbc_talent:get_int(8,i)
		tdic.param2 = dbc_talent:get_int(9,i)
		tdic.param3 = dbc_talent:get_int(10,i)
		Config.t_talent[tdic.id] = tdic
	end
end

function Config.get_t_talent(id)
   if Config.t_talent[id] == nil then
	 return nil
   else
	 return Config.t_talent[id]
   end
end

--------------------------t_recharge----------------------------------

Config.t_recharge = {}

function Config.parse_t_recharge()
	local dbc_recharge = dbc.New()
	dbc_recharge:load_txt("t_recharge")
	for i = 0, dbc_recharge:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_recharge:get_int(0, i)
		tdic.name = dbc_recharge:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.type = dbc_recharge:get_int(3, i)
		tdic.check = dbc_recharge:get_int(4, i)
		tdic.price = dbc_recharge:get_int(5, i)
		tdic.icon = dbc_recharge:get_string(6, i)
		tdic.value = dbc_recharge:get_int(7, i)
		tdic.code = dbc_recharge:get_string(8, i)
		tdic.desc = dbc_recharge:get_string(9, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		Config.t_recharge[tdic.id] = tdic
	end
end

function Config.get_t_recharge(id)
	if Config.t_recharge[id] == nil then
		return nil
	else
		return Config.t_recharge[id]
	end
end

-----------------------------------------------------------------------


-------------------------t_sign----------------------------------------

Config.t_sign = {}

function Config.parse_t_sign()
	local dbc_sign = dbc.New()
	dbc_sign:load_txt("t_sign")
	for i = 0, dbc_sign:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_sign:get_int(0, i)
		tdic.day = dbc_sign:get_string(2, i)
		tdic.day = Config.get_t_lang(tdic.day)
		tdic.type = dbc_sign:get_int(3, i)
		tdic.value1 = dbc_sign:get_int(4, i)
		tdic.value2 = dbc_sign:get_int(5, i)
		tdic.value3 = dbc_sign:get_int(6, i)
		Config.t_sign[tdic.id] = tdic
	end
end

function Config.get_t_sign(id)
	if Config.t_sign[id] == nil then
	 return nil
   else
	 return Config.t_sign[id]
   end
end

function Config.get_t_reward(type, id)
	local reward_temp = nil
	if(type == 1) then
		reward_temp = Config.get_t_resource(id)
	elseif(type == 2) then
		reward_temp = Config.get_t_item(id)
	elseif(type == 3) then
		reward_temp = Config.get_t_role(id)
	elseif(type == 4) then
		reward_temp = Config.get_t_avatar(id)
	elseif(type == 5) then
		reward_temp = Config.get_t_chest(id)
	elseif(type == 6) then
		reward_temp = Config.get_t_toukuang(id)
	elseif(type == 7) then
		reward_temp = Config.get_t_fashion(id)
	end
	return reward_temp
end

------------------------------------------------------------------------

-----------------------------t_achievement_reward-----------------------
Config.t_achievement_reward = {}
Config.max_achieve_level = 0
function Config.parse_t_achievement_reward()
	local dbc_achieve_reward = dbc.New()
	dbc_achieve_reward:load_txt("t_achievement_reward")
	for i = 0, dbc_achieve_reward:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_achieve_reward:get_int(0, i)
		tdic.level_up_point = dbc_achieve_reward:get_int(1, i)
		tdic.rewards  = {}
		for j = 1,3 do
			local reward = {}
			reward.type = dbc_achieve_reward:get_int(2 + (j - 1)*4,i)
			if reward.type > 0 then
				reward.value1 = dbc_achieve_reward:get_int(3 + (j - 1)*4,i)
				reward.value2 = dbc_achieve_reward:get_int(4 + (j - 1)*4,i)
				reward.value3 = dbc_achieve_reward:get_int(5 + (j - 1)*4,i)
				table.insert(tdic.rewards,reward)
			end
		end
		
		Config.t_achievement_reward[tdic.id] = tdic
	end
	
	local ids = {}
	for k,v in pairs(Config.t_achievement_reward) do
		table.insert(ids,k)
	end
	local comp = function(a,b)
		return a < b
	end
	table.sort(ids,comp)
	local total_point = 0
	for i = 1,#ids do
		total_point = total_point + Config.t_achievement_reward[ids[i]].level_up_point
		Config.t_achievement_reward[ids[i]].total_point = total_point
		if Config.t_achievement_reward[ids[i]].id > Config.max_achieve_level then
			Config.max_achieve_level = Config.t_achievement_reward[ids[i]].id
		end
	end
end 

function Config.get_t_achievement_reward(id)
	if Config.t_achievement_reward[id] == nil then
		return nil
	else
		return Config.t_achievement_reward[id]
	end
end

------------------------------------------------------------------------

----------------------------- t_preload -----------------------
Config.t_preload = {}

function Config.parse_t_preload()
	local dbc_preload = dbc.New()
	dbc_preload:load_txt("t_preload")
	for i = 0, dbc_preload:get_y() - 1 do
		local tdic = {}
		tdic.name = dbc_preload:get_string(0, i)
		tdic.num = dbc_preload:get_int(1, i)
		table.insert(Config.t_preload, tdic)
	end
end

------------------------------------------------------------------------

-------------------------------t_mode---------------------------------

Config.t_mode = {}

function Config.parse_t_mode()
	local dbc_mode = dbc.New()
	dbc_mode:load_txt("t_mode")
	for i = 0, dbc_mode:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_mode:get_int(0, i)
		tdic.name1 = dbc_mode:get_string(2, i)
		tdic.name1 = Config.get_t_lang(tdic.name1)
		tdic.name2 = dbc_mode:get_string(3, i)
		tdic.name2 = Config.get_t_lang(tdic.name2)
		tdic.exp = dbc_mode:get_int(4, i)
		tdic.icon = dbc_mode:get_string(5, i)
		tdic.desc = dbc_mode:get_string(7, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		tdic.level = dbc_mode:get_int(8, i)
		table.insert(Config.t_mode, tdic)
	end
end
----------------------------------------------------------------------


------------------------------t_fashion----------------------------------

Config.t_fashion = {}
Config.t_fashion_ids = {}

function Config.parse_t_fashion()
	local dbc_fashion = dbc.New()
	dbc_fashion:load_txt("t_fashion")
	for i = 0, dbc_fashion:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_fashion:get_int(0, i)
		tdic.name = dbc_fashion:get_string(2, i)
		tdic.type = dbc_fashion:get_int(3, i)
		tdic.icon = dbc_fashion:get_string(4, i)
		tdic.color = dbc_fashion:get_int(5, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.model = dbc_fashion:get_string(6, i)
		tdic.hit_effect = dbc_fashion:get_string(7, i)
		tdic.down_effect = dbc_fashion:get_string(8, i)
		tdic.show = dbc_fashion:get_string(9, i)
		tdic.desc1 = dbc_fashion:get_string(11, i)
		tdic.desc1 = Config.get_t_lang(tdic.desc1)
		tdic.desc2 = dbc_fashion:get_string(13, i)
		tdic.desc2 = Config.get_t_lang(tdic.desc2)
		tdic.desc = tdic.desc1.."\n"..tdic.desc2
		tdic.buchang = dbc_fashion:get_int(14, i)
		tdic.param_type = dbc_fashion:get_int(15, i)
		tdic.param1 = dbc_fashion:get_int(16, i)
		tdic.param2 = dbc_fashion:get_int(17, i)
		tdic.param3 = dbc_fashion:get_int(18, i)
		Config.t_fashion[tdic.id] = tdic
		table.insert(Config.t_fashion_ids, tdic)
	end
end

function Config.get_t_fashion(id)
	if Config.t_fashion[id] == nil then
		return nil
	else
		return Config.t_fashion[id]
	end
end

----------------------------------------------------------------------

------------------------------t_teamid----------------------------------

Config.t_teamid = {}

function Config.parse_t_teamid()
	local dbc_teamid = dbc.New()
	dbc_teamid:load_txt("t_teamid")
	for i = 0, dbc_teamid:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_teamid:get_int(0, i)
		tdic.name = dbc_teamid:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		Config.t_teamid[tdic.id] = tdic
	end
end

function Config.get_t_teamid(id)
	if Config.t_teamid[id] == nil then
		return nil
	else
		return Config.t_teamid[id]
	end
end

----------------------------------------------------------------------------

--------------------------------t_task--------------------------------------
Config.t_task = {}
Config.t_task_types = {} --任务表的达成类型
function Config.parse_t_task()
	local dbc_task = dbc.New()
	dbc_task:load_txt('t_task')
	for i = 0,dbc_task:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_task:get_int(0, i)	
		tdic.desc = dbc_task:get_string(2, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		tdic.level = dbc_task:get_int(3, i)
		tdic.dtype = dbc_task:get_int(4,i)
		tdic.target_num = dbc_task:get_int(5,i)
		tdic.type = dbc_task:get_int(6,i)
		tdic.param1 = dbc_task:get_int(7,i)
		tdic.param2 = dbc_task:get_int(8,i)
		tdic.param3 = dbc_task:get_int(9,i)
		tdic.param4 = dbc_task:get_int(10,i)
		tdic.rewards = {}
		for j =1,2 do 
			local reward = {}
			reward.type = dbc_task:get_int(11 + (j - 1) * 4,i)
			if reward.type ~= 0 then
				reward.value1 = dbc_task:get_int(12 + (j - 1) * 4,i)
				reward.value2 = dbc_task:get_int(13 + (j - 1) * 4,i)
				reward.value3 = dbc_task:get_int(14 + (j - 1) * 4,i)
				table.insert(tdic.rewards,reward)
			end
		end
		
		if Config.t_task_types[tdic.type] == nil then
			Config.t_task_types[tdic.type] = {}
		end
		
		table.insert(Config.t_task_types[tdic.type],tdic)
		Config.t_task[tdic.id] = tdic
	end
end

function Config.get_t_task(id)
	if Config.t_task[id] == nil then
		return nil
	else
		return Config.t_task[id]
	end
end
----------------------------------------------------------------------
----------------------------------t_daily-----------------------------
Config.t_daily = {}
Config.t_daily_types = {}

function Config.parse_t_daily()
	local dbc_daily = dbc.New()
	dbc_daily:load_txt('t_daily')
	for i = 0,dbc_daily:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_daily:get_int(0, i)	
		tdic.name = dbc_daily:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.desc = dbc_daily:get_string(4,i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		tdic.dtype = dbc_daily:get_int(5,i)
		tdic.target_num = dbc_daily:get_int(6,i)
		tdic.type = dbc_daily:get_int(7,i)
		tdic.param1 = dbc_daily:get_int(8,i)
		tdic.param2 = dbc_daily:get_int(9,i)
		tdic.param3 = dbc_daily:get_int(10,i)
		tdic.param4 = dbc_daily:get_int(11,i)
		tdic.rewards = {}
		for j = 1,3 do
			local reward = {}
			reward.type = dbc_daily:get_int(12 + (j - 1) * 4,i)
			if reward.type ~= 0 then
				reward.value1 = dbc_daily:get_int(13 + (j - 1) * 4,i)
				reward.value2 = dbc_daily:get_int(14 + (j - 1) * 4,i)
				reward.value3 = dbc_daily:get_int(15 + (j - 1) * 4,i)
				table.insert(tdic.rewards,reward)
			end
		end
		Config.t_daily[tdic.id] = tdic
		if Config.t_daily_types[tdic.type] == nil then
			Config.t_daily_types[tdic.type] = {}
		end
		table.insert(Config.t_daily_types[tdic.type],tdic)
	end
end

function Config.get_t_daily(id)
	if Config.t_daily[id] == nil then
		return nil
	else
		return Config.t_daily[id]
	end
end
----------------------------------------------------------------------
----------------------------------t_daily_reward----------------------
Config.t_daily_reward = {}
function Config.parse_t_daily_reward()
	local dbc_daily_reward = dbc.New()
	dbc_daily_reward:load_txt('t_daily_reward')
	for i = 0,dbc_daily_reward:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_daily_reward:get_int(0, i)	
		tdic.point = dbc_daily_reward:get_int(1,i)
		tdic.type = dbc_daily_reward:get_int(2,i)
		tdic.value1 = dbc_daily_reward:get_int(3,i)
		tdic.value2 = dbc_daily_reward:get_int(4,i)
		tdic.value3 = dbc_daily_reward:get_int(5,i)
		
		Config.t_daily_reward[tdic.id] = tdic
	end
end

function Config.get_t_daily_reward(id)
	if Config.t_daily_reward[id] == nil then
		return nil
	else
		return Config.t_daily_reward[id]
	end
end
----------------------------------------------------------------------

----------------------------------t_tips------------------------------

Config.t_tips = {}

function Config.parse_t_tips()
	local dbc_tips = dbc.New()
	dbc_tips:load_txt("t_tips")
	for i = 0, dbc_tips:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_tips:get_int(0, i)
		tdic.desc = dbc_tips:get_string(2, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		table.insert(Config.t_tips, tdic)
	end
end

function Config.get_t_tips()
	return Config.t_tips[math.random(#Config.t_tips)]
end
----------------------------------------------------------------------
----------------------------------t_bodysize--------------------------
Config.t_bodysize = {}
Config.max_score_level = 1

function Config.parse_t_bodysize()
	local dbc_bodysize = dbc.New()
	dbc_bodysize:load_txt("t_bodysize")
	for i = 0, dbc_bodysize:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_bodysize:get_int(0, i)
		tdic.score = dbc_bodysize:get_int(1, i)
		tdic.scale = dbc_bodysize:get_int(2,i)
		tdic.dis = dbc_bodysize:get_int(3,i)
		tdic.snowsize = dbc_bodysize:get_int(4,i)
		Config.t_bodysize[tdic.id] = tdic
		if tdic.id > Config.max_score_level then
			Config.max_score_level = tdic.id
		end
	end
end

function Config.get_t_bodysize(id)
	if Config.t_bodysize[id] == nil then
		return nil
	else
		return Config.t_bodysize[id]
	end
end
----------------------------t_ai_setting------------------------------
Config.t_ai_setting = {}

function Config.parse_t_ai_setting()
	local dbc_aisetting = dbc.New()
	dbc_aisetting:load_txt("t_ai_setting")
	for i = 0, dbc_aisetting:get_y() - 1 do
		local tdic = {}
		tdic.level = dbc_aisetting:get_int(0, i)
		tdic.low_ai_amount = dbc_aisetting:get_int(1, i)
		tdic.mid_ai_amount = dbc_aisetting:get_int(2,i)
		tdic.high_ai_amount = dbc_aisetting:get_int(3,i)
		Config.t_ai_setting[tdic.level] = tdic
	end
end

function Config.get_t_ai_setting(level)
	if Config.t_ai_setting[level] == nil then
		return nil
	else
		return Config.t_ai_setting[level]
	end
end

---------------------------------t_ai_attr------------------------------------
Config.t_ai_attr = {}

function Config.parse_t_ai_attr()
	local dbc_ai_attr = dbc.New()
	dbc_ai_attr:load_txt("t_ai_attr")
	for i = 0, dbc_ai_attr:get_y() - 1 do
		local tdic = {}
		tdic.ai_type = dbc_ai_attr:get_int(0, i)
		tdic.attack_p = dbc_ai_attr:get_int(2, i)
		tdic.attack_t = dbc_ai_attr:get_int(3,i)
		tdic.damage_p = dbc_ai_attr:get_int(4,i)
		tdic.snow_p = dbc_ai_attr:get_int(5,i)
		tdic.fire_p = dbc_ai_attr:get_int(6,i)
		tdic.exp_t = dbc_ai_attr:get_int(7,i)
		tdic.talent_p = dbc_ai_attr:get_int(8,i)
		tdic.stand_p = dbc_ai_attr:get_int(9,i)
		tdic.stand_min = dbc_ai_attr:get_int(10,i)
		tdic.stand_max = dbc_ai_attr:get_int(11,i)
		tdic.charge_attack_p = dbc_ai_attr:get_int(12,i)
		tdic.attack_player_p = dbc_ai_attr:get_int(13,i)
		Config.t_ai_attr[tdic.ai_type] = tdic
	end
end

function Config.get_t_ai_attr(ai_type)
	if Config.t_ai_attr[ai_type] == nil then
		return nil
	else
		return Config.t_ai_attr[ai_type]
	end
end
----------------------------------------------------------------------


--------------------------------t_vip_attr----------------------------

Config.t_vip_attr = {}

function Config.parse_t_vip_attr()
	local dbc_vip_attr = dbc.New()
	dbc_vip_attr:load_txt("t_vip_attr")
	for i = 0, dbc_vip_attr:get_y() - 1 do
		local tdic = {}
		tdic.id = dbc_vip_attr:get_int(0, i)
		tdic.color = dbc_vip_attr:get_string(2, i)
		tdic.first_id = dbc_vip_attr:get_int(5, i)
		tdic.day_id = dbc_vip_attr:get_int(6, i)
		tdic.attrs = {}
		for j = 0, 4 do
			local t_attr = {}
			t_attr.type = dbc_vip_attr:get_int(7 + j * 4, i)
			if(t_attr.type > 0) then
				t_attr.param1 = dbc_vip_attr:get_int(8 + j * 4, i)
				t_attr.param2 = dbc_vip_attr:get_int(9 + j * 4, i)
				t_attr.param3 = dbc_vip_attr:get_int(10 + j * 4, i)
				table.insert(tdic.attrs, t_attr)
			end
		end
		Config.t_vip_attr[tdic.id] = tdic
	end
end

function Config.get_t_vip_attr(id)
	if Config.t_vip_attr[id] == nil then
		return nil
	else
		return Config.t_vip_attr[id]
	end
end

-------------------------------------------------------------------------------------
Config.t_boss_attr = {}

function Config.parse_t_boss_attr()
	local db_boss_attr = dbc.New()
	db_boss_attr:load_txt("t_boss_attr")
	for i = 0, db_boss_attr:get_y() - 1 do
		local tdic = {}
		tdic.id = db_boss_attr:get_int(0, i)
		tdic.name = db_boss_attr:get_string(2, i)
		tdic.name = Config.get_t_lang(tdic.name)
		tdic.role_id = db_boss_attr:get_int(3, i)
		tdic.refresh_t = db_boss_attr:get_int(4, i)
		tdic.life_t = db_boss_attr:get_int(5, i)
		tdic.body_scale = db_boss_attr:get_int(6, i)
		tdic.birth_x = db_boss_attr:get_int(7, i)
		tdic.birth_y = db_boss_attr:get_int(8, i)
		tdic.def = db_boss_attr:get_int(9, i)
		tdic.skill1 = db_boss_attr:get_int(10, i)
		tdic.skill2 = db_boss_attr:get_int(11, i)
		tdic.patk_score = db_boss_attr:get_int(12, i)
		tdic.xl_score = db_boss_attr:get_int(13, i)
		tdic.bexp = db_boss_attr:get_int(14, i)
		tdic.bscore = db_boss_attr:get_int(15, i)
		tdic.maxb = db_boss_attr:get_int(16, i)
		tdic.max_dis = db_boss_attr:get_int(17, i)
		tdic.toukuang_id = db_boss_attr:get_int(18, i)
		Config.t_boss_attr[tdic.id] = tdic
	end
end

function Config.get_t_boss_attr(id)
	if Config.t_boss_attr[id] == nil then
		return nil
	else
		return Config.t_boss_attr[id]
	end
end
-------------------------------------------------------------------------------------
Config.t_rank_type = {}

function Config.parse_t_rank_type()
	local db_rank_type = dbc.New()
	db_rank_type:load_txt("t_rank_type")
	for i = 0, db_rank_type:get_y() - 1 do
		local tdic = {}
		tdic.id = db_rank_type:get_int(0, i)
		tdic.name = db_rank_type:get_string(1, i)
		tdic.num = db_rank_type:get_int(2, i)
		tdic.desc = db_rank_type:get_string(4, i)
		tdic.desc = Config.get_t_lang(tdic.desc)
		
		Config.t_rank_type[tdic.id] = tdic
	end
end

function Config.get_t_rank_type(id)
	if Config.t_rank_type[id] == nil then
		return nil
	else
		return Config.t_rank_type[id]
	end
end
-------------------------------------------------------------------------------------
Config.t_script_str = {}

function Config.parse_t_script_str()
	local db_script = dbc.New()
	db_script:load_txt("t_script_str")
	for i = 0, db_script:get_y() - 1 do
		local tdic = {}
		tdic.id = db_script:get_string(0, i)
		tdic.lang = {}
		
		for j = 1, db_script:get_x() - 1 do
			local lang = string.gsub(db_script:get_string(j, i), "##", "\n")
			table.insert(tdic.lang, lang)
		end
		Config.t_script_str[tdic.id] = tdic
	end
end

function Config.get_t_script_str(key)
	if Config.t_script_str[key] ~= nil then
		if #Config.t_script_str[key].lang > platform_config_common.languageType then
			return Config.t_script_str[key].lang[platform_config_common.languageType + 1]
		else
			return Config.t_script_str[key].lang[1]
		end	
	else
		return ""
	end
end
-------------------------------------------------------------------------------------
Config.t_regions = {}

function Config.parse_t_foregion()
	local db_region = dbc.New()
	db_region:load_txt("t_region")
	for i = 0, db_region:get_y() - 1 do
		local tdic = {}
		tdic.id = db_region:get_int(0, i)
		tdic.icon = db_region:get_string(1, i)
		
		Config.t_regions[tdic.id] = tdic
	end
end

function Config.get_t_foregion(key)
	if Config.t_regions[key] ~= nil then
		return Config.t_regions[key]
	else
		return nil
	end
end
-------------------------------------------------------------------------------------
function Config.Init()
	Config.parse_t_lang()
	Config.parse_t_name()
	Config.parse_t_item()
	Config.parse_t_role()
	Config.parse_t_avatar()
	Config.parse_t_role_skill()
	Config.parse_t_role_level()
	Config.parse_t_biaoqing()
	Config.parse_t_region()
	Config.parse_t_exp()
	Config.parse_t_cup()
	Config.parse_t_resource()
	Config.parse_t_shop()
	Config.parse_t_error()
	Config.parse_t_chest()
	Config.parse_t_attr()
	Config.parse_t_achievement()
	Config.parse_t_recharge()
	Config.parse_t_talent()
	Config.parse_t_role_buff()
	Config.parse_t_sign()
	Config.parse_t_toukuang()
	Config.parse_t_item_box()
	Config.parse_t_achievement_reward()
	Config.parse_t_preload()
	Config.parse_t_mode()
	Config.parse_t_fashion()
	Config.parse_t_teamid()
	Config.parse_t_task()
	Config.parse_t_daily()
	Config.parse_t_daily_reward()
	Config.parse_t_tips()
	Config.parse_t_bodysize()
	Config.parse_t_ai_setting()
	Config.parse_t_ai_attr()
	Config.parse_t_vip_attr()
	Config.parse_t_boss_attr()
	Config.parse_t_rank_type()
	Config.parse_t_script_str()
	Config.parse_t_foregion()
end
