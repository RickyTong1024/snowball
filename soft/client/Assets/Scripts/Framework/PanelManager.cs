using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System;

public class PanelManager : MonoBehaviour
{
    /// <summary>
    /// ������壬������Դ������
    /// </summary>
    /// <param name="type"></param>
    private float m_whscale = 1;
    private bool m_start = false;

    public GameObject CreateGUI(string name, LuaFunction func = null)
    {
        if (!m_start)
        {
            UIRoot ur = AppFacade._instance.ResourceManager.UIRoot.transform.parent.GetComponent<UIRoot>();
            float _height = AppFacade._instance.ResourceManager.UICamera.GetComponent<Camera>().pixelHeight;
            m_whscale = _height / ur.activeHeight;
            m_start = true;
        }

        return StartCreateGUI(name, func);
    }

    public GameObject CreateGUI<T>(string name) where T:BEventListener
    {
        if (!m_start)
        {
            UIRoot ur = AppFacade._instance.ResourceManager.UIRoot.transform.parent.GetComponent<UIRoot>();
            float _height = AppFacade._instance.ResourceManager.UICamera.GetComponent<Camera>().pixelHeight;
            m_whscale = _height / ur.activeHeight;
            m_start = true;
        }

        return StartCreateGUI<T>(name);
    }

    GameObject StartCreateGUI(string name, LuaFunction func = null)
    {
        GameObject prefab = Resources.Load<GameObject>("ui/" + name);
        if (prefab == null)
        {
            return null;
        }
        GameObject go = Instantiate(prefab) as GameObject;
        go.name = name;
        go.transform.parent = AppFacade._instance.ResourceManager.UIRoot;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;

        go.AddComponent<LuaUIBehaviour>().OnInit("", null);

        if (func != null)
        {
            func.Call(go);
            func.Dispose();
        }
            
        return go;
    }

    GameObject StartCreateGUI<T>(string name) where T:BEventListener
    {
        GameObject prefab = Resources.Load<GameObject>("ui/" + name);
        if (prefab == null)
        {
            return null;
        }
        GameObject go = Instantiate(prefab) as GameObject;
        go.name = name;
        go.transform.parent = AppFacade._instance.ResourceManager.UIRoot;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;

        go.AddComponent<T>().OnInit("", null);
        return go;
    }

    /// <summary>
    /// �ر����
    /// </summary>
    /// <param name="name"></param>
    public void CloseGUI(string name) {
        var panelObj = AppFacade._instance.ResourceManager.UIRoot.Find(name);
        if (panelObj == null)
        {
            return;
        }
        Destroy(panelObj.gameObject);
    }

    public void RemoveAllChild(GameObject obj, string name)
    {
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            if (obj.transform.GetChild(i).name == name)
            {
                objs.Add(obj.transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < objs.Count; i++)
        {
            GameObject.Destroy(objs[i]);
        }
        objs.Clear();
    }

    public void RemoveAllChild(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            objs.Add(obj.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < objs.Count; i++)
        {
            GameObject.Destroy(objs[i]);
        }
        objs.Clear();
    }

    public float get_wh()
    {
        return Screen.width / (float)Screen.height;
    }

    public float get_w()
    {
        return Screen.width / m_whscale;
    }

    public float get_h()
    {
        return Screen.height / m_whscale;
    }

    public float get_whscale()
    {
        return m_whscale;
    }
}
