using UnityEngine;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class LuaUIBehaviour : MonoBehaviour {
    private AssetBundle m_bundle = null;
    private string m_bundle_name;
    private HashSet<string> events = new HashSet<string>();
    private Dictionary<string, HashSet<LuaFunction>> buttonevents = new Dictionary<string, HashSet<LuaFunction>>();
    void Awake() {
        Util.CallMethod(name, "Awake", gameObject);
    }

    void OnPress(bool isPressed)
    {
        if (events.Contains("OnPress"))
        {
            Util.CallMethod(name, "OnPress", isPressed);
        }
    }

    void Start()
    {
        if (events.Contains("OnStart"))
        {
            Util.CallMethod(name, "OnStart");
        }
    }

    void OnEnable()
    {
        if (events.Contains("OnEnable"))
        {
            Util.CallMethod(name, "OnEnable");
        }
    }

    void OnDisable()
    {
        if (events.Contains("OnDisable"))
        {
            Util.CallMethod(name, "OnDisable");
        }
    }

    void OnDrag(Vector2 delta)
    {
        if (events.Contains("OnDrag"))
        {
            Util.CallMethod(name, "OnDrag", delta);
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (events.Contains("OnApplicationPause"))
        {
            Util.CallMethod(name, "OnApplicationPause", paused);
        }
    }

    public void OnInit(string name, AssetBundle bundle) {
        m_bundle_name = name;
        m_bundle = bundle; //初始化
        Debug.LogWarning("OnInit---->>>" + name);
    }

    public void OnParam(LuaTable t)
    {
        Util.CallMethod(name, "OnParam", t);
    }
    
    public void PlaySound(string name)
    {
        AudioClip _clip = Resources.Load<AudioClip>("ui/uisound/" + name);
        if (_clip != null)
        {
            AppFacade._instance.SoundManager.play_sound(_clip);
        }
    }

    public void PlaySoundEX(string name)
    {
        AudioClip _clip = Resources.Load<AudioClip>("ui/uisound/" + name);
        if (_clip != null)
        {
            AppFacade._instance.SoundManager.play_sound_ex(_clip);
        }
    }

    public void AddButtonSoundEvent(GameObject go, string eventname, LuaFunction luafunc, String sound)
    {
        if (go == null || luafunc == null) return;
        if (eventname == "click")
        {
            UIEventListener.Get(go).onClick = delegate(GameObject o)
            {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;
                click_btn_name = go.name;
                if (sound != "")
                    PlaySound(sound);
                luafunc.Call(go);
            };
        }
    }

    private bool _cancel_UI_Click_Response = false;

    public void Cancel_UI_Click_Response()
    {
        _cancel_UI_Click_Response = true;
    }

    public void Enable_UI_Click_Response()
    {
        _cancel_UI_Click_Response = false;
    }

    private List<string> disable_button_list = null;


    public void EnableButtonClickLimit(string name)
    {
        if (String.IsNullOrEmpty(name))
        {
            disable_button_list = null;
            return;
        }

        string[] st = name.Split(new char[] { ',' });
        disable_button_list = new List<string>();
        for (int i = 0; i < st.Length; i++)
            disable_button_list.Add(st[i]);
    }

    public void DisableButtonClickLimit()
    {
        disable_button_list = null;
    }

    private string click_btn_name = "";
    public string current_click_name()
    {
        return click_btn_name;
    }

    public void ResetCurrentClick()
    {
        click_btn_name = "";
    }

    public void AddButtonEvent(GameObject go, string eventname, LuaFunction luafunc) {
        if (go == null || luafunc == null) return;
        if (eventname == "click")
        {
            AddButtonSoundEvent(go, eventname, luafunc, "");
        }
        if (eventname == "hover")
        {
            UIEventListener.Get(go).onHover = delegate (GameObject o, bool isOver)
            {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;
                luafunc.Call(go, isOver);
            };
        }
        else if (eventname == "press")
        {
            UIEventListener.Get(go).onPress = delegate (GameObject o, bool state)
            {
                if (state)
                {
                    if (_cancel_UI_Click_Response)
                        return;
                    if (disable_button_list != null && disable_button_list.Contains(go.name))
                        return;
                    click_btn_name = go.name;
                    luafunc.Call(go, state); 
                }
                else
                    luafunc.Call(go, state);
            };
        }
        else if (eventname == "drag")
        {
            UIEventListener.Get(go).onDrag = delegate (GameObject o, Vector2 delta)
            {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;
                luafunc.Call(go, delta);  
            };
        }
        else if (eventname == "onDragFinished")
        {
            go.GetComponent<UIScrollView>().onDragFinished = delegate () {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;
                luafunc.Call(go);
            };
        }
        else if (eventname == "PopupList")
        {
            UIPopupList up = go.GetComponent<UIPopupList>();
            EventDelegate.Add(up.onChange, delegate ()
            {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;
                luafunc.Call(go);
            });
        }
        else if (eventname == "UIInput")
        {
            UIInput up = go.GetComponent<UIInput>();
            EventDelegate.Add(up.onSubmit, delegate ()
            {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;
                luafunc.Call(go);
            });
        }
        else if (eventname == "UISlider")
        {
            UISlider us = go.GetComponent<UISlider>();
            EventDelegate.Add(us.onChange, delegate ()
            {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;
                luafunc.Call(go);
            });
        }
    }

    public void AddEvent(string e)
    {
        events.Add(e);
    }

    //-----------------------------------------------------------------
    void OnDestroy() {
        Util.CallMethod(name, "OnDestroy");
    }

}