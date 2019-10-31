using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine.SceneManagement;

public class AppFacade : MonoBehaviour
{
    public bool test = false;
    private LuaManager LuaMgr;
    private ResourceManager ResMgr;
    private NetworkManager NetMgr;
    private SoundManager SoundMgr;
    private PanelManager PanelMgr;
    private MessageManager MessageMgr;
    private MapManager MapMgr;
    private TimerManager TimerMgr;
    private TweenManager TweenMgr;
    private ShareManager ShareMgr;
    private ToolManager ToolMgr;
    private platform_object platform_Object;
    private platform_recharge_object platform_recharge_Object;
    private UnityAdsHelper unityAdsHelper;
    private Appsflyer_management appsflyer_Management;
    private GameManager GameMgr;
    private GameObject battle_root;
    private BattleTask battleTask;

    public static AppFacade _instance;

    private GameObject tool_root;
    void Awake()
    {
        _instance = this;
        tool_root = this.transform.Find("Tool").gameObject;
        battle_root = this.transform.Find("Battle").gameObject;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = platform_config_common.GameFrameRate;
     
        PanelMgr = this.gameObject.AddComponent<PanelManager>();
        SoundMgr = this.gameObject.AddComponent<SoundManager>();
        NetMgr = this.gameObject.AddComponent<NetworkManager>();
        ResMgr = this.gameObject.AddComponent<ResourceManager>();
        MessageMgr = this.gameObject.AddComponent<MessageManager>();
        MapMgr = this.gameObject.AddComponent<MapManager>();
        TimerMgr = this.gameObject.AddComponent<TimerManager>();
        LuaMgr = this.gameObject.AddComponent<LuaManager>();
        TweenMgr = tool_root.AddComponent<TweenManager>();
        ShareMgr = tool_root.AddComponent<ShareManager>();
        ToolMgr = tool_root.AddComponent<ToolManager>();
        platform_Object = tool_root.AddComponent<platform_object>();
        platform_recharge_Object = tool_root.AddComponent<platform_recharge_object>();
        unityAdsHelper = tool_root.AddComponent<UnityAdsHelper>();
        appsflyer_Management = tool_root.AddComponent<Appsflyer_management>();
        GameMgr = this.gameObject.AddComponent<GameManager>();
        battleTask = this.battle_root.AddComponent<BattleTask>();
        BattleAchieve.RegisterMessage();
    }

    void OnDestroy()
    {
        //LuaMgr.fini();
        //MapMgr.fini();
        //ResMgr.fini();
        //ShareMgr.fini();
        BattleAchieve.RemoveMessage();
    }

    public BattleTask BattleTask
    {
        get
        {
            if (battleTask == null)
                return null;
            else
                return battleTask;
        }
    }
    public LuaManager LuaManager
    {
        get
        {
            if (LuaMgr == null)
            {
                return null;
            }
            else
            {
                return LuaMgr;
            }
        }
    }

    public ResourceManager ResourceManager
    {
        get
        {
            if (ResMgr == null)
            {
                return null;
            }
            else
            {
                return ResMgr;
            }
        }
    }

    public NetworkManager NetworkManager
    {
        get
        {
            if (NetMgr == null)
            {
                return null;
            }
            else
            {
                return NetMgr;
            }
        }
    }

    public SoundManager SoundManager
    {
        get
        {
            if (SoundMgr == null)
            {
                return null;
            }
            else
            {
                return SoundMgr;
            }
        }
    }

    public PanelManager PanelManager
    {
        get
        {
            if (PanelMgr == null)
            {
                return null;
            }
            else
            {
                return PanelMgr;
            }
        }
    }

    public MessageManager MessageManager
    {
        get
        {
            if (MessageMgr == null)
            {
                return null;
            }
            else
            {
                return MessageMgr;
            }
        }
    }

    public MapManager MapManager
    {
        get
        {
            if (MapMgr == null)
            {
                return null;
            }
            else
            {
                return MapMgr;
            }
        }
    }

    public TimerManager TimerManager
    {
        get
        {
            if (TimerMgr == null)
            {
                return null;
            }
            else
            {
                return TimerMgr;
            }
        }
    }
    public TweenManager TweenManager
    {
        get
        {
            if (TweenMgr == null)
            {
                return null;
            }
            else
            {
                return TweenMgr;
            }
        }
    }
    public ShareManager ShareManager
    {
        get
        {
            if(ShareMgr == null)
            {
                return null;
            }
            else
            {
                return ShareMgr;
            }
        }
    }

    public ToolManager ToolManager
    {
        get
        {
            if (ToolMgr == null)
            {
                return null;
            }
            else
            {
                return ToolMgr;
            }
        }
    }

    public platform_object Platform_Object
    {
        get
        {
            if (platform_Object == null)
            {
                return null;
            }
            else
            {
                return platform_Object;
            }
        }
    }

    public platform_recharge_object Platform_Recharge_Object
    {
        get
        {
            if (platform_recharge_Object == null)
            {
                return null;
            }
            else
            {
                return platform_recharge_Object;
            }
        }
    }

    public UnityAdsHelper UnityAds
    {
        get
        {
            if (unityAdsHelper == null)
            {
                return null;
            }
            else
            {
                return unityAdsHelper;
            }
        }        
    }

    public Appsflyer_management Appsflyer_Mgr
    {
        get
        {
            if (appsflyer_Management==null)
            {
                return null;
            }
            else
            {
                return appsflyer_Management;
            }
        }
    }
    public GameManager GameManager
    {
        get
        {
            if (GameMgr == null)
            {
                return null;
            }
            else
            {
                return GameMgr;
            }
        }
    }

    public void restart()
    {
        SceneManager.LoadSceneAsync("gamestart");
        GameObject.Destroy(this.gameObject);
    }
}
