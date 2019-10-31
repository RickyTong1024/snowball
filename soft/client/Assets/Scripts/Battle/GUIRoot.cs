using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GUIRoot
{
    public static Dictionary<string, GameObject> guis = new Dictionary<string, GameObject>();
    public static void ShowGUI<T>(string name,params object[] param) where T:BEventListener
    {
        if (!guis.ContainsKey(name))
        {
            var obj = LuaHelper.GetPanelManager().CreateGUI<T>(name);
            guis.Add(name, obj);
        }
        else
            guis[name].SetActive(true);

        if (param != null)
        {
            var lub = guis[name].transform.GetComponent<T>();
            lub.OnParam(param);
        }
    }

    public static void HideGUI(string name,bool isDelete = true)
    {
        if (guis.ContainsKey(name))
        { 
            if (isDelete)
            {
                LuaHelper.GetPanelManager().CloseGUI(name);
                guis.Remove(name);
            }
            else
                guis[name].SetActive(false);
        }      
    }

    public static GameObject GetPanel(string name)
    {
        if (guis.ContainsKey(name))
            return guis[name];
        else
            return null;
    }
}

