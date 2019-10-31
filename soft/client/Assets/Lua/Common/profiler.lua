profiler = {}
local start_ = false
local mode_ = 0
local father_ = {}

-- start profiling
function profiler.start(mode)
	start_ = true
	mode_ = mode or 0
    -- 初始化报告
    profiler._REPORTS           = {}
    profiler._REPORTS_BY_TITLE  = {}

    -- 记录开始时间
    profiler._STARTIME = os.clock()
	father_ = {}
	if mode_ == 0 then
		-- 开始hook，注册handler，记录call和return事件
		debug.sethook(profiler._profiling_handler, 'cr')
	end
end

function profiler.pause()
	if not start_ then
		return
	end
	if mode_ == 0 then
		debug.sethook()
	end
end

function profiler.continue()
	if not start_ then
		return
	end
	if mode_ == 0 then
		debug.sethook(profiler._profiling_handler, 'cr')
	end
end

-- stop profiling
function profiler.stop()
	if not start_ then
		return
	end
	start_ = false
    -- 记录结束时间
    profiler._STOPTIME = os.clock()

    -- 停止hook
	if mode_ == 0 then
		debug.sethook()
	end

    -- 记录总耗时
    local totaltime = profiler._STOPTIME - profiler._STARTIME

	if mode_ == 0 then
		-- 排序报告
		table.sort(profiler._REPORTS, function(a, b)
			return a.totaltime > b.totaltime
		end)

		-- 格式化报告输出
		local num = 0
		for _, report in ipairs(profiler._REPORTS) do
			
			-- calculate percent
			local percent = (report.totaltime / totaltime) * 100
			-- trace
			log(string.format("%6.3f, %6.2f%%, %7d, %s", report.totaltime, percent, report.callcount, report.title))
			num = num + 1
			if num > 20 then
				break
			end
		end
	else
		for _, report in pairs(profiler._REPORTS) do
			profiler.print(report, totaltime, 0)
		end
	end
end

-- profiling call
function profiler._profiling_call(funcinfo)

    -- 获取当前函数对应的报告，如果不存在则初始化下
    local report = profiler._func_report(funcinfo)
    assert(report)

    -- 记录这个函数的起始调用事件
    report.calltime    = os.clock()

    -- 累加这个函数的调用次数
    report.callcount   = report.callcount + 1

end

-- profiling return
function profiler._profiling_return(funcinfo)

    -- 记录这个函数的返回时间
    local stoptime = os.clock()

    -- 获取当前函数的报告
    local report = profiler._func_report(funcinfo)
    assert(report)

    -- 计算和累加当前函数的调用总时间
    if report.calltime and report.calltime > 0 then
		report.totaltime = report.totaltime + (stoptime - report.calltime)
        report.calltime = 0
	end
end

-- the profiling handler
function profiler._profiling_handler(hooktype)

    -- 获取当前函数信息
    local funcinfo = debug.getinfo(2, 'nS')

    -- 根据事件类型，分别处理 
    if hooktype == "call" then
        profiler._profiling_call(funcinfo)
    elseif hooktype == "return" then
        profiler._profiling_return(funcinfo)
    end
end

-- get the function title
function profiler._func_title(funcinfo)

    -- check
    assert(funcinfo)

    -- the function name
    local name = funcinfo.name or 'anonymous'

    -- the function line
    local line = string.format("%d", funcinfo.linedefined or 0)

    -- the function source
    local source = funcinfo.short_src or 'C_FUNC'
    --if os.isfile(source) then
        --source = path.relative(source, xmake._PROGRAM_DIR)
    --end

    -- make title
    return string.format("%-30s. %s. %s", name, source, line)
end

-- get the function report
function profiler._func_report(funcinfo)

    -- get the function title
    local title = profiler._func_title(funcinfo)

    -- get the function report
    local report = profiler._REPORTS_BY_TITLE[title]
    if not report then
        
        -- init report
        report = 
        {
            title       = profiler._func_title(funcinfo)
        ,   callcount   = 0
        ,   totaltime   = 0
        }

        -- save it
        profiler._REPORTS_BY_TITLE[title] = report
        table.insert(profiler._REPORTS, report)
    end

    -- ok?
    return report
end

--------------------------------------------------------

function profiler.get_report(funcinfo)
	local r = nil
	for i = 1, #father_ do
		if r == nil then
			r = profiler._REPORTS[father_[i]]
			assert(r)
		else
			r = r.subr[father_[i]]
			assert(r)
		end
	end
	if r == nil then
		if profiler._REPORTS[funcinfo] ~= nil then
			return profiler._REPORTS[funcinfo]
		end
		report = 
        {
            title       = funcinfo
        ,   callcount   = 0
        ,   totaltime   = 0
		,	subr = {}
        }
        profiler._REPORTS[funcinfo] = report
		return report
	else
		if r.subr[funcinfo] ~= nil then
			return r.subr[funcinfo]
		end
		report = 
        {
            title       = funcinfo
        ,   callcount   = 0
        ,   totaltime   = 0
		,	subr = {}
        }
        r.subr[funcinfo] = report
		return report
	end
end

function profiler.begin_profiler(funcinfo)
	if not start_ then
		return
	end
    -- 获取当前函数对应的报告，如果不存在则初始化下
    local report = profiler.get_report(funcinfo)
    assert(report)

    -- 记录这个函数的起始调用事件
    report.calltime    = os.clock()

    -- 累加这个函数的调用次数
    report.callcount   = report.callcount + 1
	table.insert(father_, funcinfo)
end

-- profiling return
function profiler.end_profiler(funcinfo)
	if not start_ then
		return
	end
    -- 记录这个函数的返回时间
    local stoptime = os.clock()

	table.remove(father_, #father_)
    -- 获取当前函数的报告
    local report = profiler.get_report(funcinfo)
    assert(report)

    -- 计算和累加当前函数的调用总时间
    if report.calltime and report.calltime > 0 then
		report.totaltime = report.totaltime + (stoptime - report.calltime)
        report.calltime = 0
	end
end

function profiler.print(report, totaltime, t)
	local s = ">"
	for i = 1, t do
		s = "    "..s
	end
	local percent = (report.totaltime / totaltime) * 100
	local ttt = report.totaltime
	for _, r in pairs(report.subr) do
		ttt = ttt - r.totaltime
	end
	ttt = (ttt / totaltime) * 100
	local ss = string.format("%s, %6.2f%%, %6.2f%%, %7d, %6.3f", s..report.title, ttt, percent, report.callcount, report.totaltime)
	log(ss)
	for _, r in pairs(report.subr) do
		profiler.print(r, totaltime, t + 1)
	end
end