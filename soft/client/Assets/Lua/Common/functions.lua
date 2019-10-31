
--输出日志--
function log(str)
    Util.Log(tostring(str));
end

--错误日志--
function logError(str) 
	Util.LogError(tostring(str));
end

--警告日志--
function logWarn(str) 
	Util.LogWarning(tostring(str));
end

--查找对象--
function find(str)
	return GameObject.Find(str);
end

function newObject(prefab)
	return GameObject.Instantiate(prefab);
end

--创建面板--
function createGUI(name)
	panelMgr:CreateGUI(name);
end

function child(str)
	return transform:Find(str);
end

function subGet(childNode, typeName)		
	return child(childNode):GetComponent(typeName);
end

function findPanel(str) 
	local obj = find(str);
	if obj == nil then
		error(str.." is null");
		return nil;
	end
	return obj:GetComponent("BaseLua");
end

function removeAllChild(t)
	for i = 1,t.childCount do
		GameObject.Destroy(t:GetChild(i - 1).gameObject)
	end
end

function pairsByKeys(t)  
    local a = {}  
    for n in pairs(t) do  
        a[#a+1] = n  
    end  
    table.sort(a)  
    local i = 0  
    return function()  
        i = i + 1  
        return a[i], t[a[i]]  
    end  
end

function tableTostring(tab)
	local str = ''
	for k, v in pairsByKeys(tab) do
		str = str..tostring(v)
	end
	return str
end

function stringTotable(str)
	local tab = {}
	for i = 1, string.len(str) do
		local str_ = string.sub(str, i, i)
		table.insert(tab, str_)
	end
	return tab
end

function string_split(str, split_char)
    local sub_str_tab = {};
    while (true) do
        local pos = string.find(str, split_char);
        if (not pos) then
            sub_str_tab[#sub_str_tab + 1] = str;
            break;
        end
        local sub_str = string.sub(str, 1, pos - 1);
        sub_str_tab[#sub_str_tab + 1] = sub_str;
        str = string.sub(str, pos + 1, #str);
    end
    return sub_str_tab;
end

function table_convert(tab)
	for i = 1, #tab do
		local tab_top = tab[i]
		local tab_bot = tab[#tab - i + 1]
		tab[i] = tab_bot
		tab[#tab - i + 1] = tab_top
	end
	return tab
end

function table_contain(tab, elemt)
	local flag = false
	for k, v in pairsByKeys(tab) do 
		if(v == elemt) then
			flag = true
		end
	end
	return flag
end

function string_replace(string_t, pattern, re_str)
	string_t = stringTotable(string_t)
	pattern = stringTotable(pattern)
	re_str = stringTotable(re_str)
	for i = 1, #string_t do
		local pos = 0
		for j = 1, #pattern do
			if(string_t[i + j - 1] ~= pattern[j]) then
				pos = j
			end
		end
		if(pos == 0) then
			for j = 1, #re_str do
				if( j > #pattern) then
					table.insert(string_t, i + j - 1, re_str[j])
				else
					string_t[i + j - 1] = re_str[j]
				end
			end
		end
	end
	return tableTostring(string_t)
end

function count_time(time_)
	local time_t = time_ / 1000
	local hour = math.floor(time_t / 3600)
	local minte = math.floor(time_t % 3600 / 60)
	local second = math.floor(time_t % 60)
	if(minte < 10) then
		minte = stringTotable(tostring(minte))
		table.insert(minte, 1, 0)
		minte = tableTostring(minte)
	end
	if(hour < 10) then
		hour = stringTotable(tostring(hour))
		table.insert(hour, 1, 0)
		hour = tableTostring(hour)
	end
	if(second < 10) then
		second = stringTotable(tostring(second))
		table.insert(second, 1, 0)
		second = tableTostring(second)
	end
	return hour..':'..minte..":"..second
end

function count_time_day(time_, type)
	local time_t = time_ / 1000
	local day = math.floor(time_t / 86400)
	local hour = math.floor(time_t % 86400 / 3600)
	local minte = math.floor(time_t % 3600 / 60)
	if(minte == 0 and minte % 60 > 0) then
		minte = minte + 1
	end
	local time_inf = ""
	if day > 0 and type >= 1 then
		time_inf = time_inf..tostring(day)..Config.get_t_script_str('function_001')
	end
	if hour > 0 and type >= 2 then
		time_inf = time_inf..tostring(hour)..Config.get_t_script_str('function_002')
	end
	if minte > 0 and type >= 3 then
		time_inf = time_inf..tostring(minte)..Config.get_t_script_str('function_003')
	end
	return time_inf
end

function count_time_delta(time_)
	local time_t = time_ / 1000
	local month = math.floor(time_t / (86400 * 30))
	local day = math.floor(time_t % (86400 * 30) / 86400)
	local hour = math.floor(time_t % 86400 / 3600)
	local minte = math.floor(time_t % 3600 / 60)
	if month > 0 then
		return month..Config.get_t_script_str('function_004')
	end
	if day > 0 then
		return day..Config.get_t_script_str('function_005')
	end
	if hour > 0 then
		return hour..Config.get_t_script_str('function_006')
	end
	if minte > 0 then
		return minte..Config.get_t_script_str('function_007')
	else
		return Config.get_t_script_str('function_008')
	end
end

function str_sub(str, length)
	while(string.len(str) > length) do
		str = string.sub(str, 1, -2)
	end
	return str
end

function get_time_show(time_, type)
	local time_det = timerMgr:get_time_show(time_)
	local hour = time_det.Hour
	local minute = time_det.Minute
	if(hour < 10) then
		hour = stringTotable(tostring(hour))
		table.insert(hour, 1, 0)
		hour = tableTostring(hour)
	end
	if(minute < 10) then
		minute = stringTotable(tostring(minute))
		table.insert(minute, 1, 0)
		minute = tableTostring(minute)
	end
	if(type == 1) then
		return time_det.Year.."/"..time_det.Month.."/"..time_det.Day.."  "..hour..":"..minute
	else
		return time_det.Month..Config.get_t_script_str('function_009')..time_det.Day..Config.get_t_script_str('function_010')..hour..":"..minute
	end
end

function IsMatch(input, pattern)
	local res = System.Text.RegularExpressions.Regex.IsMatch(input, pattern)
	return res
end

function split(str, delimiter)
	if str==nil or str=='' or delimiter==nil then
		return nil
	end
	
    local result = {}
    for match in (str..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, match)
    end
    return result
end

function toInt(x)
	if x < 0 then
		return math.ceil(x)
	end
	return math.floor(x)
end

function LinearBezierCurve(p0, p1, t)
	return p0 + (p1 - p0) * t; 
end

function QuadBezierCurve(p0, p1, p2, t)
	local B = Vector3.zero
	local t1 = (1 - t) * (1 - t)  
	local t2 = t * (1 - t)
	local t3 = t * t
	B = p0*t1 + p1*2*t2 + p2*t3
	return B
end 

function CubicBezierCurve(p0, p1, p2, p3, t)
	local B = Vector3.zero
	local t1 = (1 - t)*(1 - t)*(1 - t) 
	local t2 = (1 - t)*(1 - t)*t
	local t3 = t*t*(1 - t)
	local t4 = t*t*t
	B = p0*t1 + p1*3*t2 + p2*3*t3 + p3*t4
	return B
end

function BezierCurve(p0,p1,p2,p3,t)
    local ax, bx, cx;
    local ay, by, cy;
    local tSquared, tCubed;
	local result = {}
	
    cx = 3.0 * (p1.x - p2.x);
    bx = 3.0 * (p1.x - p2.x) - cx;
    ax = p3.x - p0.x - cx - bx;
 
    cy = 3.0 * (p1.y - p0.y);
    by = 3.0 * (p2.y - p1.y) - cy;
    ay = p3.y - p0.y - cy - by;
 
    tSquared = t * t;
    tCubed = tSquared * t;
	
	
    result.x = (ax * tCubed) + (bx * tSquared) + (cx * t) + p0.x;
    result.y = (ay * tCubed) + (by * tSquared) + (cy * t) + p0.y;
	result.z = p1.z
    return result;
end

function random(a, b)
	local r = a + toInt(math.random() * (b - a))
	return r
end