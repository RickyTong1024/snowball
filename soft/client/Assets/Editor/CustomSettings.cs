using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using LuaInterface;
using UnityEditor;

using BindType = ToLuaMenu.BindType;
using System.Reflection;

public static class CustomSettings
{
    public static string FrameworkPath = platform_config_common.FrameworkRoot;
    public static string saveDir = FrameworkPath + "/ToLua/Source/Generate/";
    public static string luaDir = FrameworkPath + "/Lua/";
    public static string toluaBaseType = FrameworkPath + "/ToLua/BaseType/";
	public static string baseLuaDir = FrameworkPath + "/ToLua/Lua";
	public static string injectionFilesPath = Application.dataPath + "/ToLua/Injection/";

    //导出时强制做为静态类的类型(注意customTypeList 还要添加这个类型才能导出)
    //unity 有些类作为sealed class, 其实完全等价于静态类
    public static List<Type> staticClassTypes = new List<Type>
    {        
        typeof(UnityEngine.Application),
        typeof(Util),
        typeof(UnityEngine.Time),
        typeof(UnityEngine.Screen),
        typeof(UnityEngine.SleepTimeout),
        typeof(UnityEngine.Input),
		typeof(UnityEngine.PlayerPrefs),
        typeof(UnityEngine.Resources),
        typeof(UnityEngine.Physics),
        typeof(UnityEngine.RenderSettings),
        typeof(UnityEngine.QualitySettings),
        typeof(UnityEngine.GL),
        typeof(UnityEngine.Graphics),
    };

    //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
    public static DelegateType[] customDelegateList = 
    {        
        _DT(typeof(Action)),                
        _DT(typeof(UnityEngine.Events.UnityAction)),
        _DT(typeof(System.Predicate<int>)),
        _DT(typeof(System.Action<int>)),
        _DT(typeof(System.Comparison<int>)),
        _DT(typeof(System.Func<int, int>)),
    };

    //在这里添加你要导出注册到lua的类型列表
    public static BindType[] customTypeList =
    {
        _GT(typeof(Debugger)).SetNameSpace(null),   
        _GT(typeof(Component)),
        _GT(typeof(Transform)),
        _GT(typeof(Material)),
        _GT(typeof(Rigidbody)),
        _GT(typeof(Camera)),
        _GT(typeof(AudioSource)),
        _GT(typeof(Behaviour)),
        _GT(typeof(MonoBehaviour)),        
        _GT(typeof(GameObject)),
        _GT(typeof(TrackedReference)),
        _GT(typeof(Application)),
        _GT(typeof(platform_config_common)),
        _GT(typeof(platform_recharge)),
        _GT(typeof(platform)),
        _GT(typeof(UnityAdsHelper)),
        _GT(typeof(Physics)),
        _GT(typeof(Collider)),
        _GT(typeof(Time)),        
        _GT(typeof(Texture)),
        _GT(typeof(Texture2D)),
        _GT(typeof(Shader)),        
        _GT(typeof(Renderer)),
        _GT(typeof(WWW)),
        _GT(typeof(WWWForm)),
        _GT(typeof(Screen)),        
        _GT(typeof(CameraClearFlags)),
        _GT(typeof(AudioClip)),        
        _GT(typeof(AssetBundle)),
        _GT(typeof(ParticleSystem)),
        _GT(typeof(System.DateTime)),
        _GT(typeof(AsyncOperation)).SetBaseType(typeof(System.Object)),        
        _GT(typeof(LightType)),
        _GT(typeof(SleepTimeout)),
        _GT(typeof(Animator)),
        _GT(typeof(Input)),
		_GT(typeof(PlayerPrefs)),
        _GT(typeof(KeyCode)),
        _GT(typeof(SkinnedMeshRenderer)),
        _GT(typeof(Space)),      
       

        _GT(typeof(MeshRenderer)),
        _GT(typeof(BoxCollider)),
        _GT(typeof(MeshCollider)),
        _GT(typeof(SphereCollider)),        
        _GT(typeof(CharacterController)),
        _GT(typeof(CapsuleCollider)),
        
        _GT(typeof(Animation)),        
        _GT(typeof(AnimationClip)).SetBaseType(typeof(UnityEngine.Object)),        
        _GT(typeof(AnimationState)),
        _GT(typeof(AnimationBlendMode)),
        _GT(typeof(QueueMode)),  
        _GT(typeof(PlayMode)),
        _GT(typeof(WrapMode)),

        _GT(typeof(QualitySettings)),
        _GT(typeof(RenderSettings)),                                                   
        _GT(typeof(BlendWeights)),           
        _GT(typeof(RenderTexture)),  
		_GT(typeof(Resources)),  
          
        //for LuaFramework
        _GT(typeof(NGUIText)),
        _GT(typeof(UIPanel)),
        _GT(typeof(UILabel)),
        _GT(typeof(UILabel.Effect)),
        _GT(typeof(UITexture)),
        _GT(typeof(AnimatorStateInfo)),
        _GT(typeof(UISprite)),
        _GT(typeof(UIAtlas)),
        _GT(typeof(UISlider)),
		_GT(typeof(UIInput)),
        _GT(typeof(UIPopupList)),
        _GT(typeof(UIScrollView)),
        _GT(typeof(Particle2D)),
        _GT(typeof(TweenPosition)),
        _GT(typeof(TweenAlpha)),
        _GT(typeof(Util)),
        _GT(typeof(WrapGrid)),
        _GT(typeof(LuaHelper)),
        _GT(typeof(LuaUIBehaviour)),
        _GT(typeof(LuaParamBehaviour)),

        _GT(typeof(GameManager)),
        _GT(typeof(LuaManager)),
        _GT(typeof(PanelManager)),
        _GT(typeof(SoundManager)),
        _GT(typeof(NetworkManager)),
        _GT(typeof(ResourceManager)),
        _GT(typeof(MessageManager)),
        _GT(typeof(MapManager)),
        _GT(typeof(TimerManager)),
        _GT(typeof(TweenManager)),
        _GT(typeof(ShareManager)),
        _GT(typeof(ToolManager)),
        _GT(typeof(s_message)),
        _GT(typeof(s_net_message)),
        _GT(typeof(dbc)),
        _GT(typeof(DFA)),
        _GT(typeof(ArrayList)),
        _GT(typeof(unit)),
        _GT(typeof(Joystick)),
        _GT(typeof(obstancle)),
        _GT(typeof(navMeshInfo)),
        _GT(typeof(NavUtil)),
        _GT(typeof(bobjpool)),
        _GT(typeof(Battle)),
        _GT(typeof(BattleAchieve)),
        _GT(typeof(dhc.player_t)),
        _GT(typeof(dhc.role_t)),
        _GT(typeof(List<dhc.role_t>)),
    };

    public static List<Type> dynamicList = new List<Type>()
    {
        typeof(MeshRenderer),
#if !UNITY_5_4_OR_NEWER
        typeof(ParticleEmitter),
        typeof(ParticleRenderer),
        typeof(ParticleAnimator),
#endif

        typeof(BoxCollider),
        typeof(MeshCollider),
        typeof(SphereCollider),
        typeof(CharacterController),
        typeof(CapsuleCollider),

        typeof(Animation),
        typeof(AnimationClip),
        typeof(AnimationState),

        typeof(BlendWeights),
        typeof(RenderTexture),
        typeof(Rigidbody),
    };

    //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
    //使用方法参见例子14
    public static List<Type> outList = new List<Type>()
    {
        
    };
        
    //ngui优化，下面的类没有派生类，可以作为sealed class
    public static List<Type> sealedList = new List<Type>()
    {
        /*typeof(Transform),
        typeof(UIRoot),
        typeof(UICamera),
        typeof(UIViewport),
        typeof(UIPanel),
        typeof(UILabel),
        typeof(UIAnchor),
        typeof(UIAtlas),
        typeof(UIFont),
        typeof(UITexture),
        typeof(UISprite),
        typeof(UIGrid),
        typeof(UITable),
        typeof(UIWrapGrid),
        typeof(UIInput),
        typeof(UIScrollView),
        typeof(UIEventListener),
        typeof(UIScrollBar),
        typeof(UICenterOnChild),
        typeof(UIScrollView),        
        typeof(UIButton),
        typeof(UITextList),
        typeof(UIPlayTween),
        typeof(UIDragScrollView),
        typeof(UISpriteAnimation),
        typeof(UIWrapContent),
        typeof(TweenWidth),
        typeof(TweenAlpha),
        typeof(TweenColor),
        typeof(TweenRotation),
        typeof(TweenPosition),
        typeof(TweenScale),
        typeof(TweenHeight),
        typeof(TypewriterEffect),
        typeof(UIToggle),
        typeof(Localization),*/
    };

    public static BindType _GT(Type t)
    {
        return new BindType(t);
    }

    public static DelegateType _DT(Type t)
    {
        return new DelegateType(t);
    }
}
