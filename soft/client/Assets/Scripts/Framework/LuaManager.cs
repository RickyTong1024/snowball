using UnityEngine;
using System.Collections;
using LuaInterface;
using System;

public class LuaManager : MonoBehaviour
{
    private LuaState lua;
    private LuaLoader loader;
    private LuaLooper loop = null;
    private bool m_is_start = false;
    private ulong m_time;
    private float m_uptime;

    // Use this for initialization
    void Awake() {
        loader = new LuaLoader();
        lua = new LuaState();
        this.OpenLibs();
        lua.LuaSetTop(0);

        LuaBinder.Bind(lua);
        LuaCoroutine.Register(lua, this);
    }

    public void fini()
    {
        if (loop != null)
        {
            loop.Destroy();
            loop = null;
        }

        if (lua != null)
        {
            lua.Dispose();
            lua = null;
        }
        loader = null;
    }

    public void InitStart() {
        InitLuaPath();
        InitLuaBundle();
        this.lua.Start();    //启动LUAVM
        this.StartMain();
        this.StartLooper();
        m_is_start = true;
        m_time = AppFacade._instance.TimerManager.now();
    }

    void StartLooper() {
        loop = gameObject.AddComponent<LuaLooper>();
        loop.luaState = lua;
    }

    //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
    protected void OpenCJson() {
        lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
        lua.OpenLibs(LuaDLL.luaopen_cjson);
        lua.LuaSetField(-2, "cjson");

        lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
        lua.LuaSetField(-2, "cjson.safe");
    }

    void StartMain() {
        lua.DoFile("Main.lua");

        LuaFunction main = lua.GetFunction("Main");
        main.Call();
        main.Dispose();
        main = null;    
    }
        
    /// <summary>
    /// 初始化加载第三方库
    /// </summary>
    void OpenLibs() {
        lua.OpenLibs(LuaDLL.luaopen_pb);      
        lua.OpenLibs(LuaDLL.luaopen_sproto_core);
        lua.OpenLibs(LuaDLL.luaopen_protobuf_c);
        lua.OpenLibs(LuaDLL.luaopen_lpeg);
        lua.OpenLibs(LuaDLL.luaopen_bit);
        lua.OpenLibs(LuaDLL.luaopen_socket_core);

        this.OpenCJson();
    }

    /// <summary>
    /// 初始化Lua代码加载路径
    /// </summary>
    void InitLuaPath() {
        if (platform_config_common.m_debug) {
            string rootPath = platform_config_common.FrameworkRoot;
            lua.AddSearchPath(rootPath + "/Lua");
            lua.AddSearchPath(rootPath + "/ToLua/Lua");
        } else {
            lua.AddSearchPath(Util.DataPath + "lua");
        }
    }

    /// <summary>
    /// 初始化LuaBundle
    /// </summary>
    void InitLuaBundle() {
        if (!platform_config_common.m_debug)
        {
            loader.AddBundle("lua/lua.unity3d");
            loader.AddBundle("lua/lua_battle.unity3d");
            loader.AddBundle("lua/lua_cjson.unity3d");
            loader.AddBundle("lua/lua_common.unity3d");
            loader.AddBundle("lua/lua_lpeg.unity3d");
            loader.AddBundle("lua/lua_misc.unity3d");
            loader.AddBundle("lua/lua_net.unity3d");
            loader.AddBundle("lua/lua_protobuf.unity3d");
            loader.AddBundle("lua/lua_socket.unity3d");
            loader.AddBundle("lua/lua_system.unity3d");
            loader.AddBundle("lua/lua_system_reflection.unity3d");
            loader.AddBundle("lua/lua_unityengine.unity3d");
            loader.AddBundle("lua/lua_view.unity3d");
        }
    }

    public void DoFile(string filename) {
        lua.DoFile(filename);
    }

    // Update is called once per frame
    public object[] CallFunction(string funcName, params object[] args) {
        if (lua == null)
        {
            return null;
        }
        LuaFunction func = lua.GetFunction(funcName);
        if (func != null) {
            return func.LazyCall(args);
        }
        return null;
    }

    public void LuaGC() {
        lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
    }

    void Update()
    {
        if (!m_is_start)
        {
            return;
        }
        m_uptime += Time.deltaTime / Time.timeScale;
        if (m_uptime < 1)
        {
            return;
        }
        m_uptime = 0;
        if (AppFacade._instance.TimerManager.trigger_time(m_time, 5, 0))
        {
            Util.CallMethod("PlayerData","day_refresh");
        }
        if (AppFacade._instance.TimerManager.trigger_week_time(m_time))
        {
            Util.CallMethod("PlayerData", "week_refresh");
        }
        if (AppFacade._instance.TimerManager.trigger_month_time(m_time))
        {
            Util.CallMethod("PlayerData", "month_refresh");
        }
        m_time = AppFacade._instance.TimerManager.now();
    }

    public LuaTable GetLuaTable(string s, string name)
    {
        LuaTable ff = lua.GetTable(name);
        if (ff == null)
            lua.DoString(s);
        else
        {
            while (ff.IsAlive)
                ff.Dispose();       
            lua.DoString(s);    
        }
        return lua.GetTable(name);
    }

    private void OnDestroy()
    {
        fini();
    }
}
