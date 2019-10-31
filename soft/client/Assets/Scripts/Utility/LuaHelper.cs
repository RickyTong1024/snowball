using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using LuaInterface;
using System;

public static class LuaHelper {

    /// <summary>
    /// getType
    /// </summary>
    /// <param name="classname"></param>
    /// <returns></returns>
    public static System.Type GetType(string classname) {
        Assembly assb = Assembly.GetExecutingAssembly();  //.GetExecutingAssembly();
        System.Type t = null;
        t = assb.GetType(classname); ;
        if (t == null) {
            t = assb.GetType(classname);
        }
        return t;
    }

    /// <summary>
    /// 面板管理器
    /// </summary>
    public static PanelManager GetPanelManager() {
        return AppFacade._instance.PanelManager;
    }

    /// <summary>
    /// 资源管理器
    /// </summary>
    public static ResourceManager GetResManager() {
        return AppFacade._instance.ResourceManager;
    }

    /// <summary>
    /// 网络管理器
    /// </summary>
    public static NetworkManager GetNetManager() {
        return AppFacade._instance.NetworkManager;
    }

    /// <summary>
    /// 音乐管理器
    /// </summary>
    public static SoundManager GetSoundManager() {
        return AppFacade._instance.SoundManager;
    }

    /// <summary>
    /// 消息管理器
    /// </summary>
    public static MessageManager GetMessageManager()
    {
        return AppFacade._instance.MessageManager;
    }

    public static MapManager GetMapManager()
    {
        return AppFacade._instance.MapManager;
    }

    public static TimerManager GetTimerManager()
    {
        return AppFacade._instance.TimerManager;
    }

    public static TweenManager GetTweenManager()
    {
        return AppFacade._instance.TweenManager;
    }

	public static ShareManager GetShareManager()
    {
        return AppFacade._instance.ShareManager;
    }

    public static ToolManager GetToolManager()
    {
        return AppFacade._instance.ToolManager;
    }

    public static GameManager GetGameManager()
    {
        return AppFacade._instance.GameManager;
    }
    public static void restart()
    {
        AppFacade._instance.restart();
    }

    public static Action Action(LuaFunction func) {
        Action action = () => {
            func.Call();
        };
        return action;
    }

    public static UIEventListener.VoidDelegate VoidDelegate(LuaFunction func) {
        UIEventListener.VoidDelegate action = (go) => {
            func.Call(go);
        };
        return action;
    }

    /// <summary>
    /// pbc/pblua函数回调
    /// </summary>
    /// <param name="func"></param>
    public static void OnCallLuaFunc(LuaByteBuffer data, LuaFunction func) {
        if (func != null) func.Call(data);
        Debug.LogWarning("OnCallLuaFunc length:>>" + data.buffer.Length);
    }

    /// <summary>
    /// cjson函数回调
    /// </summary>
    /// <param name="data"></param>
    /// <param name="func"></param>
    public static void OnJsonCallFunc(string data, LuaFunction func) {
        Debug.LogWarning("OnJsonCallback data:>>" + data + " lenght:>>" + data.Length);
        if (func != null) func.Call(data);
    }

    public static GameObject Instantiate(GameObject obj)
    {
        return GameObject.Instantiate(obj) as GameObject;
    }
}
