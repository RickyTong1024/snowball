using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


public class BEventListener:MonoBehaviour
{
    protected bool _cancel_UI_Click_Response = false;
    protected List<string> disable_button_list = null;
    protected string click_btn_name = "";

    public virtual void OnInit(string name, AssetBundle bundle)
    {

    }


    public virtual void OnParam(params object[] param)
    {

    }

    protected void AddButtonEvent(GameObject go, string eventname)
    {
        if (eventname == "click")
        {
            AddButtonSoundEvent(go, eventname,"");
        }
        if (eventname == "hover")
        {
            UIEventListener.Get(go).onHover = delegate (GameObject o, bool isOver)
            {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;

                OnHover(go, isOver);
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
                    OnPress(go, state);
                }
                else
                    OnPress(go, state);
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
                OnDrag(go, delta);
            };
        }
        else if (eventname == "onDragFinished")
        {
            go.GetComponent<UIScrollView>().onDragFinished = delegate () {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;
                OnDragFinished(go);
            };
        }
    }

    public void AddButtonSoundEvent(GameObject go, string eventname, String sound)
    {
        if (go == null)
            return;

        if (eventname == "click")
        {
            UIEventListener.Get(go).onClick = delegate (GameObject o)
            {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(go.name))
                    return;
                click_btn_name = go.name;
                if (sound != "")
                    PlaySound(sound);
                OnClick(go);
            };
        }
    }

    public void PlaySound(string name)
    {
        AudioClip _clip = Resources.Load<AudioClip>("ui/uisound/" + name);
        if (_clip != null)
        {
            AppFacade._instance.SoundManager.play_sound(_clip);
        }
    }

    protected virtual void OnHover(GameObject go,bool isOver)
    {

    }

    protected virtual void OnClick(GameObject go)
    {

    }

    protected virtual void OnPress(GameObject go,bool state)
    {

    }

    protected virtual void OnDrag(GameObject go, Vector2 delta)
    {

    }

    protected virtual void OnDragFinished(GameObject go)
    {

    }

    public void RegisterOnClick(GameObject go, UIEventListener.VoidDelegate d)
    {
        UIEventListener.Get(go).onClick = delegate(GameObject obj)
        {
            if (_cancel_UI_Click_Response)
                return;
            if (disable_button_list != null && disable_button_list.Contains(go.name))
                return;
            click_btn_name = go.name;
            d(obj);
        };
    }

    public void RegisterOnHover(GameObject go, UIEventListener.BoolDelegate d)
    {
        UIEventListener.Get(go).onHover = delegate(GameObject o,bool isOver)
        {
            if (_cancel_UI_Click_Response)
                return;
            if (disable_button_list != null && disable_button_list.Contains(go.name))
                return;
            d(o, isOver);
        };
    }

    public void RegisterOnPress(GameObject go, UIEventListener.BoolDelegate d)
    {
        UIEventListener.Get(go).onPress = delegate(GameObject obj,bool state)
        {
            if (state)
            {
                if (_cancel_UI_Click_Response)
                    return;
                if (disable_button_list != null && disable_button_list.Contains(obj.name))
                    return;
                click_btn_name = obj.name;
                d(obj, state);
            }
            else
                d(obj, state);
        };
    }

    public void RegisterOnDrag(GameObject go, UIEventListener.VectorDelegate d)
    {
        UIEventListener.Get(go).onDrag = delegate(GameObject o, Vector2 delta)
        {
           if (_cancel_UI_Click_Response)
               return;
           if (disable_button_list != null && disable_button_list.Contains(go.name))
               return;
           OnDrag(go, delta);
        };
    }

    public void RegisterOnDragFinished(GameObject go, UIScrollView.OnDragNotification d)
    {
        go.GetComponent<UIScrollView>().onDragFinished = delegate()
        {
            if (_cancel_UI_Click_Response)
                return;
            if (disable_button_list != null && disable_button_list.Contains(go.name))
                return;
            d();
        };
    }


    public void Cancel_UI_Click_Response()
    {
        _cancel_UI_Click_Response = true;
    }

    public void Enable_UI_Click_Response()
    {
        _cancel_UI_Click_Response = false;
    }

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

    public string current_click_name()
    {
        return click_btn_name;
    }

    public void ResetCurrentClick()
    {
        click_btn_name = "";
    }
}

