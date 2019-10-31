
using System;
using UnityEngine;
using System.Collections;
using LuaInterface;
#if UNITYADS
using UnityEngine.Advertisements;
#endif

public class UnityAdsHelper : MonoBehaviour
{
    public string iosGameID = "2999897";
    public string androidGameID = "2999896";

    public bool enableTestMode = true;
    public bool showInfoLogs;
    public bool showDebugLogs;
    public bool showWarningLogs = true;
    public bool showErrorLogs = true;

    private static Action _handleFinished;
    private static Action _handleSkipped;
    private static Action _handleFailed;
    private static Action _onContinue;
    static LuaFunction donefunc;
    static LuaFunction skipfunc;

    public static LuaFunction Donefunc
    {
        get
        {
            return donefunc;
        }

        set
        {
            donefunc = value;
        }
    }

    public static LuaFunction Skipfunc
    {
        get
        {
            return skipfunc;
        }

        set
        {
            skipfunc = value;
        }
    }
#if UNITYADS
    void Start()
    {
        Debug.Log("Running precheck for Unity Ads initialization...");

        string gameID = null;

#if UNITY_IPHONE
		gameID = iosGameID;
#elif UNITY_ANDROID
        gameID = androidGameID;
#endif
        if (!Advertisement.isSupported)
        {
            Debug.Log("the platform is not supported");
        }
        else if (Advertisement.isInitialized)
        {
            Debug.Log("Unity Ads is already initialized.");
        }
        else if (string.IsNullOrEmpty(gameID))
        {
            Debug.LogError("gameID is ERROR");
        }
        else
        {
            Advertisement.Initialize(gameID, false);
            StartCoroutine(LogWhenUnityAdsIsInitialized());
        }
    }
	
	public void init ()
	{
		
	}

	private IEnumerator LogWhenUnityAdsIsInitialized ()
	{
		float initStartTime = Time.time;

		do yield return new WaitForSeconds(0.1f);
		while (!Advertisement.isInitialized);

        Debug.Log(string.Format("AD loading {0:F1} seconds.", Time.time - initStartTime));
		yield break;
	}
	
	//--- Static Helper Methods

	public static bool isShowing { get { return Advertisement.isShowing; }}
	public static bool isSupported { get { return Advertisement.isSupported; }}
	public static bool isInitialized { get { return Advertisement.isInitialized; }}
	
	public static bool IsReady () 
	{ 
		return IsReady(null); 
	}
	public static bool IsReady (string zoneID) 
	{
		if (string.IsNullOrEmpty(zoneID)) zoneID = null;
		
		return Advertisement.IsReady(zoneID);
	}

	public static void ShowAd () 
	{
		ShowAd(null,null,null,null,null);
	}
	public static void ShowAd (string zoneID) 
	{
		ShowAd(zoneID,null,null,null,null);
	}
	public static void ShowAd (string zoneID, Action handleFinished) 
	{
		ShowAd(zoneID,handleFinished,null,null,null);
	}
	public static void ShowAd (string zoneID, Action handleFinished, Action handleSkipped) 
	{
		ShowAd(zoneID,handleFinished,handleSkipped,null,null);
	}
	public static void ShowAd (string zoneID, Action handleFinished, Action handleSkipped, Action handleFailed) 
	{
		ShowAd(zoneID,handleFinished,handleSkipped,handleFailed,null);
	}

	public static void ShowAd (string zoneID, Action handleFinished, Action handleSkipped, Action handleFailed, Action onContinue)
	{
		if (string.IsNullOrEmpty(zoneID)) zoneID = null;
		_handleFinished = handleFinished_c;
		_handleSkipped = handleSkipped_c;
		_handleFailed = handleFinished_c;
		_onContinue = onContinue;

		if (Advertisement.IsReady(zoneID))

		{
			Debug.Log("Start Show Ad");
			
			ShowOptions options = new ShowOptions();
			options.resultCallback = HandleShowResult;

			Advertisement.Show(zoneID,options);
		}
		else 
		{
			Debug.LogWarning("Ad is no Ready!!");
		}
	}

    public static void handleFinished_c()
    {
        if (donefunc != null)
        {
            donefunc.Call();
        }
    }

    public static void handleSkipped_c()
    {
        if (skipfunc != null)
        {
            skipfunc.Call();
        }
    }

    private static void HandleShowResult (ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("AD show end ,issue reward !!");
			if (!object.ReferenceEquals(_handleFinished,null)) _handleFinished();
			break;
		case ShowResult.Skipped:
			Debug.LogWarning("The ad was skipped before reaching the end.");
			if (!object.ReferenceEquals(_handleSkipped,null)) _handleSkipped();
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			if (!object.ReferenceEquals(_handleFailed,null)) _handleFailed();
			break;
		}

		if (!object.ReferenceEquals(_onContinue,null)) _onContinue();
	}
#else 
    void Start()
    {
        Debug.LogWarning("Unity Ads is not supported under the current build platform.");
    }

    public static bool isShowing { get { return false; } }
    public static bool isSupported { get { return false; } }
    public static bool isInitialized { get { return false; } }

    public static bool IsReady() { return false; }
    public static bool IsReady(string zoneID) { return false; }

    public static void ShowAd()
    {        
        Debug.LogError("Failed to show ad. Unity Ads is not supported under the current build platform.");
    }
    public static void ShowAd(string zoneID) { ShowAd(); }
    public static void ShowAd(string zoneID, Action handleFinished) { ShowAd(); }
    public static void ShowAd(string zoneID, Action handleFinished, Action handleSkipped) { ShowAd(); }
    public static void ShowAd(string zoneID, Action handleFinished, Action handleSkipped, Action handleFailed) { ShowAd(); }
    public static void ShowAd(string zoneID, Action handleFinished, Action handleSkipped, Action handleFailed, Action onContinue) { ShowAd(); }
#endif
}

