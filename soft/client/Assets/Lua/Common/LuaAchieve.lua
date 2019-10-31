LuaAchieve = {}

LuaAchieve.completeAchieve = nil

function LuaAchieve.OnInit()
	LuaAchieve.completeAchieve = {}
	LuaAchieve.PushCompleteInfoToClient()
end

function LuaAchieve.IsFirstComplete(id)
	for i = 1,#LuaAchieve.completeAchieve do
		if LuaAchieve.completeAchieve[i] == id then
			return false
		end
	end
	
	for i =1,#self.player.achieve_reward do
		if self.player.achieve_reward[i] == id then
			return false
		end
	end
	
	return true
end


function LuaAchieve.TTypeChange()
	if self == nil or self.player == nil then
		return
	end
	
	for i = 1,#Config.t_achievement.types[30] do
		local t_achievement = Config.t_achievement.types[30][i]	
		if self.get_achieve_num(t_achievement.id) >= t_achievement.target_num then
			if LuaAchieve.IsFirstComplete(t_achievement.id) then
				AchieveAnimation.AddHallAchieve(t_achievement.id,true)
				table.insert(LuaAchieve.completeAchieve,t_achievement.id)
			end
		end
	end
	
	for i = 1,#Config.t_achievement.types[130] do
		local t_achievement = Config.t_achievement.types[130][i]	
		if self.get_achieve_num(t_achievement.id) >= t_achievement.target_num then
			if LuaAchieve.IsFirstComplete(t_achievement.id) then
				AchieveAnimation.AddHallAchieve(t_achievement.id,true)
				table.insert(LuaAchieve.completeAchieve,t_achievement.id)
			end
		end
	end
end

function LuaAchieve.RoleAmountChange(delay)
	if self == nil or self.player == nil then
		return
	end

	for i = 1,#Config.t_achievement.types[1] do
		local t_achievement = Config.t_achievement.types[1][i]	
		local num = 0
		for i = 1, #self.roles do
			local flag = true
			local role = self.roles[i]
			local t_role = Config.get_t_role(role.template_id)
			if t_achievement.param1 ~= 2 and t_achievement.param1 ~= t_role.sex then
				flag = false
			end
			if t_achievement.param2 ~= 0 and t_achievement.param2 ~= t_role.color then
				flag = false
			end
			if flag then
				num = num + 1
			end
		end
		
		if num >= t_achievement.target_num then
			if LuaAchieve.IsFirstComplete(t_achievement.id) then
				AchieveAnimation.AddHallAchieve(t_achievement.id,delay)
				table.insert(LuaAchieve.completeAchieve,t_achievement.id)
			end
		end
	end
end

function LuaAchieve.AccountLevelUp()
	if self == nil or self.player == nil then
		return
	end
	
	for i = 1,#Config.t_achievement.types[2] do
		local data = Config.t_achievement.types[2][i]
		if self.player.level >= data.target_num then
			if LuaAchieve.IsFirstComplete(data.id) then
				AchieveAnimation.AddHallAchieve(data.id)
				table.insert(LuaAchieve.completeAchieve,data.id)
			end	
		end
	end
end

function LuaAchieve.RoleStarLevelUp()  
	if self == nil or self.roles == nil then
		return
	end
	
	for i = 1,#Config.t_achievement.types[3] do
		local data = Config.t_achievement.types[3][i]
		local role_id = data.param1

		for j = 1,#self.roles do
			if role_id == self.roles[j].template_id and data.target_num <= self.roles[j].level then
				if LuaAchieve.IsFirstComplete(data.id) then
					AchieveAnimation.AddHallAchieve(data.id)
					table.insert(LuaAchieve.completeAchieve,data.id)
					break
				end
			end
		end
	end
end

function LuaAchieve.PushCompleteInfoToClient()
	local list = System.Collections.ArrayList.New()
	
	for i = 1,#LuaAchieve.completeAchieve do
		list:Add(LuaAchieve.completeAchieve[i])
	end
	--BattleAchieve.Load(list)
end

function LuaAchieve.LoadFromClient(arrList)
	local dict = {}
	for i = 1,#LuaAchieve.completeAchieve do
		dict[LuaAchieve.completeAchieve[i]] = true
	end
	
	for i = 0,arrList.Count - 1 do
		if dict[arrList[i]] == nil then
			table.insert(LuaAchieve.completeAchieve,arrList[i])
		end
	end
end
