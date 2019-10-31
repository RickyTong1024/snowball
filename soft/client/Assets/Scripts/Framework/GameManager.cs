using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.Reflection;
using System.IO;

public class GameManager : MonoBehaviour
{
    protected static bool initialize = false;
    private List<string> downloadFiles = new List<string>();

    /// <summary>
    /// 初始化游戏管理器
    /// </summary>
    void Awake() {
        Init();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void Init() {
        InitGui();
        DontDestroyOnLoad(gameObject);  //防止销毁自己
    }

    void InitGui()
    {
        Config.Init();
        platform_recharge._instance.init();
        GameObject obj = Resources.Load("ui/UpdatePanel") as GameObject;
        GameObject ins = Instantiate(obj) as GameObject;
        ins.transform.parent = AppFacade._instance.ResourceManager.UIRoot;
        ins.transform.localPosition = Vector3.zero;
        ins.transform.localScale = Vector3.one;
    }
        
    public void UpdateComplete()
    {
        GC.Collect();
        InitLogic();
        OnResourceInited();
    }

    void InitLogic()
    {
        DFA._instance.init();
    }
    /// <summary>
    /// 资源初始化结束
    /// </summary>
    public void OnResourceInited() {
        if (!AppFacade._instance.test)
        {
            AppFacade._instance.LuaManager.InitStart();
            initialize = true;                          //初始化完
        }
        else
        {
            initialize = true;
        }
    }

    ///////////////////////////////////////////////////////////////

    void Update()
    {
        UpdateTick();
    }

    private long TickToMilliSec(long tick)
    {
        return tick / (10 * 1000);
    }

    private long mFrameCount = 0;
    private long mLastFrameTime = 0;
    static long mLastFps = 0;
    private void UpdateTick()
    {
        if (true)
        {
            mFrameCount++;
            long nCurTime = TickToMilliSec(System.DateTime.Now.Ticks);
            if (mLastFrameTime == 0)
            {
                mLastFrameTime = TickToMilliSec(System.DateTime.Now.Ticks);
            }

            if ((nCurTime - mLastFrameTime) >= 1000)
            {
                long fps = (long)(mFrameCount * 1.0f / ((nCurTime - mLastFrameTime) / 1000.0f));

                mLastFps = fps;

                mFrameCount = 0;

                mLastFrameTime = nCurTime;
            }
        }
    }

    public int GetFPS()
    {
        return (int)mLastFps;
    }
}
