using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LuaInterface;
using System.IO;
using System;

public class ToolManager : MonoBehaviour
{
#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void KeyChainSave(string text);
    [DllImport("__Internal")]
    private static extern void KeyChainLoad();
    [DllImport("__Internal")]
    private static extern void _save_photo(string readaddr);
#endif
    // Use this for initialization
    void Start()
    {
        cancel_notify();
        load_kc();
    }

    public void create_notify(string title, string text, int hour, int minute)
    {
        if (Application.isEditor)
        {
            return;
        }

        int _second = AppFacade._instance.TimerManager.dtnow().Second;
        int _minute = AppFacade._instance.TimerManager.dtnow().Minute;
        int _hour = AppFacade._instance.TimerManager.dtnow().Hour;

        if (_hour < hour || (_hour == hour && _minute < minute))
        {
            int time = (hour - _hour) * 60 + minute - _minute;
            time = time * 60 - _second;
            create_notify(title, text, text, time);
        }
        {
            int time = (hour + 24 - _hour) * 60 + minute - _minute;
            time = time * 60 - _second;
            create_notify(title, text, text, time);
        }
    }

    public void create_notify(string title, string text, string ticker, int secondsFromNow)
    {
        if (Application.isEditor)
        {
            return;
        }
        platform._instance.create_notify(title, text, ticker, secondsFromNow);

    }

    public void cancel_notify()
    {
        if (Application.isEditor)
        {
            return;
        }
        platform._instance.cancel_notify();       
    }

    public void copy(string text)
    {
        if (Application.isEditor)
        {
            return;
        }
        platform._instance.copy(text);        

    }

    public void save_kc(string text)
    {
#if UNITY_IPHONE
		KeyChainSave(text);
#endif
    }

    public void load_kc()
    {
#if UNITY_IPHONE
		KeyChainLoad();
#endif
    }

    static string kc_code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    void load_kc_callback(string s)
    {
        if (s == "")
        {
            for (int i = 0; i < 32; ++i)
            {
                int index = UnityEngine.Random.Range(0, kc_code.Length);
                s += kc_code[index];
            }
            save_kc(s);
        }
        s_message msg = new s_message();
        msg.name = "load_kc_callback";
        msg.m_object.Add(s);
        AppFacade._instance.MessageManager.AddMessage(msg);
    }

    IEnumerator SavePhoto(string name, string message)
    {
        ScreenCapture.CaptureScreenshot(name);
        yield return new WaitForSeconds(0.1f);

        if (message != "")
        {
            s_message msg = new s_message();
            msg.name = message;
            AppFacade._instance.MessageManager.AddMessage(msg);
        }
    }

    IEnumerator SavePhotoIOS(string name, string message)
    {

        ScreenCapture.CaptureScreenshot(name);
        yield return new WaitForSeconds(1.2f);
#if UNITY_IPHONE
        string path_read = Application.persistentDataPath + "/" + name;
        _save_photo(path_read);

        if (message != "")
        {
            s_message msg = new s_message();
            msg.name = message;
            AppFacade._instance.MessageManager.AddMessage(msg);
        }
#endif
    }

    IEnumerator SavePhotoAndroid(string name, string message)
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        byte[] bytes = texture.EncodeToJPG();
        if (Application.platform == RuntimePlatform.Android)
        {
            platform._instance.do_save_photo_platform(name, bytes);
        }
    }
    public void save_photo(string message)
    {
        System.DateTime now = System.DateTime.Now;
        string name = string.Format("image{0}{1}{2}{3}{4}{5}.png", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        if (Application.isEditor)
        {
            StartCoroutine(SavePhoto(name, message));
        }
        else
        {
#if UNITY_ANDROID
            StartCoroutine(SavePhotoAndroid(name, message));
#elif UNITY_IPHONE
			StartCoroutine (SavePhotoIOS(name, message));
#endif
        }
    }

    public string load_file(string path)
    {
        string str = "";
        StreamReader sr = new StreamReader(path);
        string read_data = sr.ReadLine();
        while (read_data != null)
        {
            str += read_data;
            read_data = sr.ReadLine();
        }
        sr.Close();
        return str;
    }

    private Dictionary<string, url_inf> url_tab = new Dictionary<string, url_inf>();
    private List<string> url_list = new List<string>();
    private bool url_is_loading = false;
    private int url_count = 0;
    public void load_url(string url, LuaFunction luafunc)
    {
        string key = url_count.ToString();
        url_inf url_f = new url_inf(url, luafunc);
        url_tab[key] = url_f;
        url_list.Add(key);
        url_count += 1;
        if (!url_is_loading)
        {
            StartCoroutine(LoadUrl(url, luafunc));
            url_is_loading = true;
        }
    }

    public void load_url(string url, Action<WWW> func)
    {
        string key = url_count.ToString();
        url_inf url_f = new url_inf(url, func);
        url_tab[key] = url_f;
        url_list.Add(key);
        url_count += 1;
        if (!url_is_loading)
        {
            StartCoroutine(LoadUrl(url, func));
            url_is_loading = true;
        }
    }
    IEnumerator LoadUrl(string url, Action<WWW> func)
    {
        WWW www = new WWW(url);
        yield return www;
        if (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        else
        {
            func(www);
            url_tab.Remove(url_list[0]);
            url_list.RemoveAt(0);
            if (url_list.Count > 0)
            {
                StartCoroutine(LoadUrl(url_tab[url_list[0]].url, url_tab[url_list[0]].func));
                yield break;
            }
            else
            {
                url_is_loading = false;
                url_count = 0;
            }
        }
    }

    IEnumerator LoadUrl(string url, LuaFunction luafunc)
    {
        WWW www = new WWW(url);
        yield return www;
        if (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        else
        {
            luafunc.Call(www);
            url_tab.Remove(url_list[0]);
            url_list.RemoveAt(0);
            if (url_list.Count > 0)
            {
                StartCoroutine(LoadUrl(url_tab[url_list[0]].url, url_tab[url_list[0]].luafunc));
                yield break;
            }
            else
            {
                url_is_loading = false;
                url_count = 0;
            }
        }
    }
    private Dictionary<string, WWW> http_list = new Dictionary<string, WWW>();
    public void load_http(string url, WWWForm wwwf, LuaFunction luafunc, LuaFunction failfunc)
    {
        StartCoroutine(LoadHttp(url, wwwf, luafunc, failfunc));
    }

    public bool unload_http(string url)
    {
        if (http_list.ContainsKey(url))
        {
            http_list[url].Dispose();
            http_list.Remove(url);
            return true;
        }
        return false;
    }

    IEnumerator LoadHttp(string url, WWWForm wwwf, LuaFunction luafunc, LuaFunction failfunc)
    {
        WWW www_http = new WWW(url, wwwf);
        if (http_list.ContainsKey(url))
        {
            http_list[url].Dispose();
        }
        http_list[url] = www_http;
        yield return www_http;
        if(!http_list.ContainsKey(url))
        {
            yield break;
        }
        if (www_http.error != null)
        {
            failfunc.Call();
            http_list.Remove(url);
            yield break;
        }
        if (!www_http.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        else
        {
            luafunc.Call(www_http);
            http_list.Remove(url);
        }
    }

    public static int CountVerInf(string ver)
    {
        ver = ver.Replace("\r", "");
        ver = ver.Replace("\n", "");
        string[] ver_str = ver.Split('.');
        int a = int.Parse(ver_str[0]) * 10000;
        int b = int.Parse(ver_str[1]);
        for (int i = ver_str[1].Length; i < 4; ++i)
        {
            b = b * 10;
        }
        a = a + b;
        return a;
    }
}

public class url_inf
{
    public string url;
    public LuaFunction luafunc;
    public Action<WWW> func;
    public url_inf(string url_, LuaFunction luafunc_)
    {
        this.url = url_;
        this.luafunc = luafunc_;
    }

    public url_inf(string url_, Action<WWW> func)
    {
        this.url = url_;
        this.func = func;
    }
}