profiler = {}
local start_ = false
local mode_ = 0
local father_ = {}

-- start profiling
function profiler.start(mode)
	start_ = true
	mode_ = mode or 0
    -- ��ʼ������
    profiler._REPORTS           = {}
    profiler._REPORTS_BY_TITLE  = {}

    -- ��¼��ʼʱ��
    profiler._STARTIME = os.clock()
	father_ = {}
	if mode_ == 0 then
		-- ��ʼhook��ע��handler����¼call��return�¼�
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
    -- ��¼����ʱ��
    profiler._STOPTIME = os.clock()

    -- ֹͣhook
	if mode_ == 0 then
		debug.sethook()
	end

    -- ��¼�ܺ�ʱ
    local totaltime = profiler._STOPTIME - profiler._STARTIME

	if mode_ == 0 then
		-- ���򱨸�
		table.sort(profiler._REPORTS, function(a, b)
			return a.totaltime > b.totaltime
		end)

		-- ��ʽ���������
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

    -- ��ȡ��ǰ������Ӧ�ı��棬������������ʼ����
    local report = profiler._func_report(funcinfo)
    assert(report)

    -- ��¼�����������ʼ�����¼�
    report.calltime    = os.clock()

    -- �ۼ���������ĵ��ô���
    report.callcount   = report.callcount + 1

end

-- profiling return
function profiler._profiling_return(funcinfo)

    -- ��¼��������ķ���ʱ��
    local stoptime = os.clock()

    -- ��ȡ��ǰ�����ı���
    local report = profiler._func_report(funcinfo)
    assert(report)

    -- ������ۼӵ�ǰ�����ĵ�����ʱ��
    if report.calltime and report.calltime > 0 then
		report.totaltime = report.totaltime + (stoptime - report.calltime)
        report.calltime = 0
	end
end

-- the profiling handler
function profiler._profiling_handler(hooktype)

    -- ��ȡ��ǰ������Ϣ
    local funcinfo = debug.getinfo(2, 'nS')

    -- �����¼����ͣ��ֱ��� 
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
    -- ��ȡ��ǰ������Ӧ�ı��棬������������ʼ����
    local report = profiler.get_report(funcinfo)
    assert(report)

    -- ��¼�����������ʼ�����¼�
    report.calltime    = os.clock()

    -- �ۼ���������ĵ��ô���
    report.callcount   = report.callcount + 1
	table.insert(father_, funcinfo)
end

-- profiling return
function profiler.end_profiler(funcinfo)
	if not start_ then
		return
	end
    -- ��¼��������ķ���ʱ��
    local stoptime = os.clock()

	table.remove(father_, #father_)
    -- ��ȡ��ǰ�����ı���
    local report = profiler.get_report(funcinfo)
    assert(report)

    -- ������ۼӵ�ǰ�����ĵ�����ʱ��
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