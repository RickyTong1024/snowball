using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class UsedBundle
{
    public AssetBundle bundle;
    public int num;
}

public class EffectTime
{
    public float time;
    public float total_time;
    public GameObject obj;
    public int state = 0;
}

public class ResourceManager : MonoBehaviour
{
    private Transform m_unit_root;
    public Transform UnitRoot
    {
        get
        {
            if (m_unit_root == null)
            {
                GameObject go = GameObject.FindWithTag("UnitRoot");
                if (go != null)
                {
                    m_unit_root = go.transform;
                }
            }
            return m_unit_root;
        }
    }

    private Transform m_pool_root;
    public Transform PoolRoot
    {
        get
        {
            if (m_pool_root == null)
            {
                GameObject go = GameObject.FindWithTag("PoolRoot");
                if (go != null)
                {
                    m_pool_root = go.transform;
                }
            }
            return m_pool_root;
        }
    }

    private Transform m_ui_root;
    public Transform UIRoot
    {
        get
        {
            if (m_ui_root == null)
            {
                GameObject go = GameObject.FindWithTag("UIRoot");
                if (go != null)
                {
                    m_ui_root = go.transform;
                }
            }
            return m_ui_root;
        }
    }

    private Transform m_ui_camera;
    public Transform UICamera
    {
        get
        {
            if (m_ui_camera == null)
            {
                GameObject go = GameObject.FindWithTag("UICamera");
                if (go != null)
                {
                    m_ui_camera = go.transform;
                }
            }
            return m_ui_camera;
        }
    }

    private Transform m_audio_listener;
    public Transform AudioListener
    {
        get
        {
            if (m_audio_listener == null)
            {
                GameObject go = GameObject.FindWithTag("AudioListener");
                if (go != null)
                {
                    m_audio_listener = go.transform;
                }
            }
            return m_audio_listener;
        }
    }

    public TextAsset LoadTxt(string name)
    {
        return Resources.Load<TextAsset>("config/" + name);
    }

    public TextAsset LoadObs(string name)
    {
        return Resources.Load<TextAsset>("config/mission/" + name);
    }

    public TextAsset LoadNav(string name)
    {
        return Resources.Load<TextAsset>("config/mission/" + name);
    }

    public GameObject CreateUnit(string name, bool is_show = false)
    {
        GameObject obj = null;
        GameObject res = null;
        if (is_show)
        {
            res = Resources.Load<GameObject>("unit/" + name + "/" + name + "_show");
        }
        else
        {
            res = Resources.Load<GameObject>("unit/" + name + "/" + name);
        }
        obj = Instantiate(res);
        if (obj.GetComponent<unit>() != null)
        {
            obj.GetComponent<unit>().set_init(name, is_show);
        }
        obj.name = name;
        SkinnedMeshRenderer[] ms = obj.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < ms.Length; ++i)
        {
            ms[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            ms[i].receiveShadows = false;
        }
        return obj;
    }

    Dictionary<String, HashSet<GameObject>> eff_objs_ = new Dictionary<string,HashSet<GameObject>>();
    HashSet<EffectTime> eff_times_ = new HashSet<EffectTime>();

    public void ClearEffect()
    {
        foreach (HashSet<GameObject> s in eff_objs_.Values)
        {
            foreach (GameObject obj in s)
            {
                GameObject.Destroy(obj);
            }
        }
        eff_objs_.Clear();
        foreach (EffectTime et in eff_times_)
        {
            GameObject.Destroy(et.obj);
        }
        eff_times_.Clear();
    }

    public GameObject CreateEffect(string name,bool is_low = false)
    {
        HashSet<GameObject> s = null;
        if (eff_objs_.ContainsKey(name))
        {
            s = eff_objs_[name];
        }
        if (s == null || s.Count == 0)
        {
            return CreateEffectEX(name,is_low);
        }
        GameObject o = null;
        foreach (GameObject obj in s)
        {
            o = obj;
            break;
        }
        s.Remove(o);
        if (name != "rongqi")
        {
            if(!o.activeSelf)
                o.SetActive(true);
            objyh oh = o.GetComponent<objyh>();
            if (oh != null)
            {
                oh.enabled = true;
                oh.BecameVisible();
            }
            if (o.GetComponent<AudioSource>() != null)
            {
                if (!LuaHelper.GetSoundManager().Is_play_eff)
                {
                    o.GetComponent<AudioSource>().enabled = false;
                }
                else
                {
                    o.GetComponent<AudioSource>().enabled = true;
                }
            }
        }
        return o;
    }
    public GameObject CreateEffectEX(string name,bool is_low = false)
    {
        GameObject obj = null;
        GameObject res = null;
        string name_low = name + "_low";
        if (is_low)
        {
            res = Resources.Load<GameObject>("effect/effect_low/" + name_low);
            if (res == null)
            {
                res = Resources.Load<GameObject>("effect/" + name);
            }
        }
        else
        {
            res = Resources.Load<GameObject>("effect/" + name);
        }
        obj = Instantiate(res);
        if (!LuaHelper.GetSoundManager().Is_play_eff && obj.GetComponent<AudioSource>() != null)
        {
            obj.GetComponent<AudioSource>().enabled = false;
        }
        obj.name = name;
        MeshRenderer[] ms = obj.GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < ms.Length; ++i)
        {
            ms[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            ms[i].receiveShadows = false;
        }
        if (name != "rongqi")
        {
            obj.AddComponent<objyh>();
        }
        return obj;
    }

    public void DeleteEffect(GameObject obj)
    {
        DeleteEffect(obj, 0.0f);
    }
    public void DeleteEffect(GameObject obj, float time)
    {
        EffectTime et = new EffectTime();
        et.obj = obj;
        et.total_time = time;
        et.time = 0;
        et.state = 0;
        eff_times_.Add(et);
    }

    void DeleteEffect1(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        string name = obj.name;
        if (name == "rongqi")
        {
            Transform t = obj.transform.Find("sub");
            if (t.childCount > 0)
            {
                t = t.GetChild(0);
                DeleteEffect1(t.gameObject);
            }
        }
        HashSet<GameObject> s = null;
        if (eff_objs_.ContainsKey(name))
        {
            s = eff_objs_[name];
        }
        else
        {
            s = new HashSet<GameObject>();
            eff_objs_.Add(name, s);
        }
        obj.transform.parent = PoolRoot;
        obj.transform.localPosition = new Vector3(10000, -10000, 10000);
        s.Add(obj);
    }

    private List<EffectTime> m_ets = new List<EffectTime>();
    void Update()
    {
        foreach (EffectTime et in eff_times_)
        {
            et.time += Time.deltaTime;
            if (et.time >= et.total_time)
            {
                if (et.obj == null)
                {
                    m_ets.Add(et);
                }
                else if (et.state == 0)
                {
                    if (et.obj.name != "rongqi")
                    {
                        et.obj.SetActive(false);
                    }
                    else
                    {
                        Transform t = et.obj.transform.Find("sub");
                        if (t.childCount > 0)
                        {
                            t = t.GetChild(0);
                            t.gameObject.SetActive(false);
                        }
                    }
                    et.state = 1;
                    et.time = 0;
                }
                else if (et.time > 1)
                {
                    m_ets.Add(et);
                }
            }
        }
        for (int i = 0; i < m_ets.Count; ++i)
        {
            DeleteEffect1(m_ets[i].obj);
            eff_times_.Remove(m_ets[i]);
        }
        m_ets.Clear();
    }
}