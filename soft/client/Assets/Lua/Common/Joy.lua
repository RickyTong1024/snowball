Joy = {}
local joy_map_ = {}

function Joy.OnInit(name, joy)
	joy_map_[name] = joy
end

function Joy.Get(name)
	if joy_map_[name] ~= nil then
		local joy = joy_map_[name]
		return joy
	end
	return nil
end

