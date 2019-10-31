using UnityEngine;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public struct s_part
{
	public string type;
	public GameObject obj;
    public AssetBundle bundle;
};

public class s_action
{
	public float m_time;
	public string m_type;
	public ArrayList m_floats = new ArrayList();
	public ArrayList m_ints = new ArrayList();	
	public ArrayList m_string = new ArrayList();	
};

public class s_action_set
{
	public string m_name;
	public ArrayList m_actions = new ArrayList();
};

public class unit : MonoBehaviour, IHandle
{
    private bool is_test_init = false;
    private string m_name;
    private bool m_is_show;
    private ArrayList m_parts = new ArrayList();
    private XDocument m_xml = null;
    private int m_round = 5000;
    private GameObject m_accept;
    private ArrayList m_actions = new ArrayList();
    private string[] m_action_name = {"ready", "null"};
    private ArrayList[] m_action = {null, null};
	private float[] m_action_time = {0, 0};
    private int[] m_action_pos = { 0, 0 };
    private float[] m_action_speed = { 1, 1 };

    private float m_white = 0.0f;
    private float m_alpha = 0.0f;
    private float m_target_alpha = 1.0f;
    private bool m_dalpha = true;
    private float m_base_scale = 1.0f;
    private float m_scale = 1.0f;
    private bool m_main = true;
    private List<string> m_anim_list = new List<string>();
    private string m_next_anim = "";
    private bool m_isv = true;
    private bool m_pause_anim = false;

    private static List<string> xuli_effects = new List<string>() { "Unit_chargeAttack", "Unit_chargeAttack_fire", "Unit_chargeAttack_lightning", "Unit_chargeAttack_pentagram" };
    public Vector3 get_accept_pos()
	{
		if(m_accept == null)
		{
			return transform.position;
		}

		return m_accept.transform.position;
	}

    public void set_init(string name, bool is_show)
    {
        m_name = name;
        m_is_show = is_show;
        load_xml();
    }

    public int get_round()
    {
        return m_round;
    }

    void Awake ()
    {
		alpha ();
		m_base_scale = transform.localScale.y;
        m_name = gameObject.name;
    }

    void test_init()
    {
        if (is_test_init)
        {
            return;
        }
        if (AppFacade._instance.test)
        {
            AppFacade._instance.MessageManager.RegisterHandle(this);
            is_test_init = true;
        }
    }

    void OnDestory()
    {
        for (int i = 0; i < m_parts.Count;)
        {
            s_part _part = (s_part)m_parts[i];
            Object.Destroy(_part.obj);
            _part.bundle.Unload(true);
        }
        if (AppFacade._instance.test)
        {
            AppFacade._instance.MessageManager.RemoveHandle(this);
        }
    }

    void BecameVisible()
    {
        m_isv = true;
        if (!m_pause_anim)
        {
            Animator _animator = transform.GetComponent<Animator>();
            if (_animator != null)
            {
                _animator.enabled = true;
            }
        }
        if (m_alpha > 0)
        {
            show(transform);
        }
    }

    public bool is_vis()
    {
        if (m_alpha > 0 && m_isv)
        {
            return true;
        }
        return false;
    }

    public bool CheckScreenVis()
    {
        Vector3 v = AppFacade._instance.MapManager.WorldToScreenPoint(transform.position);
        float w = AppFacade._instance.PanelManager.get_w();
        float h = AppFacade._instance.PanelManager.get_h();
        if (v.x < -50 || v.x > w + 50 || v.y < -50 || v.y > h + 100 && !m_is_show)
            return false;
        else
            return true;
    }

    void BecameInvisible()
    {
        if(m_is_show)
        {
            return;
        }
        m_isv = false;
        Animator _animator = transform.GetComponent<Animator>();
        if (_animator != null)
        {
            _animator.enabled = false;
        }
        hide(transform);
    }

    private void show(Transform trans)
    {
        List<Transform> l = get_all_child(trans);
        for (int i = 0; i < l.Count; i++)
        {
            Transform t = l[i];
            Renderer[] render = t.GetComponents<Renderer>();
            for (int j = 0; j < render.Length; ++j)
            {
                if (render[j].enabled == false)
                {
                    render[j].enabled = true;
                }
            }
            t.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    private void hide(Transform trans)
    {
        List<Transform> l = get_all_child(trans);
        for (int i = 0; i < l.Count; i++)
        {
            Transform t = l[i];
            Renderer[] render = t.GetComponents<Renderer>();
            for (int j = 0; j < render.Length; ++j)
            {
                if (render[j].enabled == true)
                {
                    render[j].enabled = false;
                }
            }
            t.gameObject.layer = LayerMask.NameToLayer("UnitHide");
        }
    }

    List<Transform> get_all_child(Transform t)
    {
        List<Transform> l = new List<Transform>();
        l.Add(t);
        for (int i = 0; i < t.childCount; i++)
        {
            Transform tt = t.GetChild(i);
            l.AddRange(get_all_child(tt));
        }
        return l;
    }

	public void load_xml()
	{
		if(m_xml != null)
		{
			return;
		}

		m_parts.Clear ();
		m_actions.Clear ();

		m_xml = new XDocument();
        TextAsset text_asset = Resources.Load<TextAsset>("unit/" + m_name + "/" + m_name);
        StringReader reader = new StringReader(text_asset.text);
		m_xml = XDocument.Load(reader);
        reader.Close();

        XElement unit_node = m_xml.Element("unit");
        if (unit_node.Attribute("round") != null)
        {
            m_round = int.Parse(unit_node.Attribute("round").Value);
        }
        /// 受击点
        Transform _bone = get_bone("Bip01 Spine1");
        if (_bone != null)
        {
            m_accept = _bone.gameObject;
        }
        else
        {
            m_accept = this.gameObject;
        }
        /// 行为
        IEnumerable<XElement>  nodeList = unit_node.Elements("actions");
		foreach (XElement xe in nodeList)
		{
			string name = xe.Attribute("name").Value;
			s_action_set _actions = new s_action_set();
			_actions.m_name = name;
			IEnumerable<XElement> _sub_xml = xe.Elements("atcion");
			foreach (XElement _sub_xe in _sub_xml)
			{
				s_action _action = new s_action();

				_action.m_type = _sub_xe.Attribute("type").Value;
				_action.m_time = float.Parse(_sub_xe.Attribute("time").Value);
				if(_action.m_type == "anim")
				{
					_action.m_string.Add(_sub_xe.Attribute("name").Value);
				}
                if (_action.m_type == "anim1")
                {
                    _action.m_string.Add(_sub_xe.Attribute("name").Value);
                }
				if(_action.m_type == "action")
				{
					_action.m_string.Add(_sub_xe.Attribute("name").Value);
				}
				if(_action.m_type == "entity" || _action.m_type == "target_entity" || _action.m_type == "copy_effect_object"  || _action.m_type == "copy_effect_object_ex")
				{
					_action.m_string.Add(_sub_xe.Attribute("name").Value);
					_action.m_floats.Add(float.Parse(_sub_xe.Attribute("remove_time").Value));
                    if (_sub_xe.Attribute("common") != null)
                    {
                        _action.m_ints.Add(int.Parse(_sub_xe.Attribute("common").Value));
                    }
                    else
                    {
                        _action.m_ints.Add((int)0);
                    }
				}

				if(_action.m_type == "entity_bone" 
				   || _action.m_type == "target_entity_bone" 
				   || _action.m_type == "target_copy_effect_object")
				{
					_action.m_string.Add(_sub_xe.Attribute("name").Value);
					_action.m_string.Add(_sub_xe.Attribute("bone").Value);
			
					if(_sub_xe.Attribute("follow") != null)
					{
						_action.m_ints.Add(int.Parse(_sub_xe.Attribute("follow").Value));
					}
					else
					{
						_action.m_ints.Add((int)1);
					}
                    if (_sub_xe.Attribute("common") != null)
                    {
                        _action.m_ints.Add(int.Parse(_sub_xe.Attribute("common").Value));
                    }
                    else
                    {
                        _action.m_ints.Add((int)0);
                    }

					_action.m_floats.Add(float.Parse(_sub_xe.Attribute("remove_time").Value));
				}
				if(_action.m_type == "entity_link")
				{
					_action.m_string.Add(_sub_xe.Attribute("name").Value);
					_action.m_string.Add(_sub_xe.Attribute("bone").Value);
					
					_action.m_floats.Add(float.Parse(_sub_xe.Attribute("remove_time").Value));
				}
				if(_action.m_type == "entity_cast")
				{
					_action.m_string.Add(_sub_xe.Attribute("name").Value);
					_action.m_string.Add(_sub_xe.Attribute("bone").Value);
					_action.m_string.Add(_sub_xe.Attribute("effect").Value);
	
					_action.m_floats.Add(float.Parse(_sub_xe.Attribute("speed").Value));
					_action.m_floats.Add(float.Parse(_sub_xe.Attribute("cast_remove_time").Value));
					_action.m_floats.Add(float.Parse(_sub_xe.Attribute("remove_time").Value));
				}
				if(_action.m_type == "sound")
				{
					_action.m_string.Add(_sub_xe.Attribute("sound").Value);
					if(_sub_xe.Attribute("loop") != null)
					{
						_action.m_floats.Add(float.Parse(_sub_xe.Attribute("loop").Value));
					}
				}
                if (_action.m_type == "shake_cam")
                {
                    _action.m_floats.Add(float.Parse(_sub_xe.Attribute("shake").Value));
                }
                _actions.m_actions.Add(_action);
			}

			m_actions.Add(_actions);
		}
	}

    public void set_main(bool main)
    {
        m_main = main;
    }
    public void set_alpha(float a)
    {
        m_target_alpha = a;
        m_dalpha = true;
    }
    
	void alpha(Transform transform)
	{
		for(int i = 0;i < transform.childCount;i ++)
		{
			Transform _unit = transform.GetChild(i);
            Renderer[] render = _unit.GetComponents<Renderer>();
            for (int t = 0; t < render.Length; ++t)
            {
                Material[] ms = render[t].materials;
                for (int tt = 0; tt < ms.Length; ++tt)
                {
                    ms[tt].SetFloat("_alpha",m_alpha);
                }
			}

			alpha(_unit);
		}
	}

	void alpha()
	{
        if (!m_dalpha)
        {
            return;
        }
		float _delta_time = Time.deltaTime * 5;
        if (Mathf.Abs(m_alpha - m_target_alpha) > _delta_time)
        {
            if (m_alpha > m_target_alpha)
            {
                m_alpha -= _delta_time;
            }
            else if (m_alpha < m_target_alpha)
            {
                m_alpha += _delta_time;
            }
        }
        else
        {
            m_alpha = m_target_alpha;
            m_dalpha = false;
        }
        alpha(transform);
		if(m_alpha <= 0)
		{
            hide(transform);
		}
		else if (m_isv)
		{
            show(transform);
		}
	}

    public void white()
    {
        m_white = 0.1f;
        do_white();
    }

	void do_white()
	{
		if(m_white > 0.0f)
		{
			int w = 1;            m_white -= Time.deltaTime;
            if (m_white < 0.0f)
            {
                m_white = 0.0f;
				w = 0;
            }

			for(int i = 0;i < transform.childCount;i ++)
			{
				Transform _unit = transform.GetChild(i);
                Renderer[] render = _unit.GetComponents<Renderer>();
                for (int t = 0; t < render.Length; ++t )
                {
                    Material[] ms = render[t].materials;
                    for (int tt = 0; tt < ms.Length; ++tt)
                    {
                        ms[tt].SetFloat("_white", w);
                    }
                }
			}
		}
	}

	public Transform get_bone(string bone_name)
	{
        if (bone_name == "")
        {
            return transform;
        }
		else if(bone_name == "accept" )
		{
			if (m_accept == null)
			{
				Debug.Log("accept 不存在");
				return transform;
			}
			return m_accept.transform;
		}

		Transform[] hips = transform.GetComponentsInChildren<Transform>();
		foreach(Transform hip in hips)
		{		
			if(hip.name == bone_name)
			{
				return hip;
			}
		}

		return null;
	}

    public void remove_part(string type)
    {
        Transform[] _hips = transform.GetComponentsInChildren<Transform>();

        foreach (Transform hip in _hips)
        {
            if (hip.name.IndexOf(type) != -1)
            {
                GameObject.Destroy(hip.gameObject);
            }
        }

        for (int i = 0; i < m_parts.Count;)
        {
            s_part _part = (s_part)m_parts[i];
            if (_part.type == type)
            {
                Object.Destroy(_part.obj);
                m_parts.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }

	public void change_static_part(string type, string pack, string bone_name)
	{
		remove_part (type);
        s_part _new_part = new s_part();
        _new_part.type = type;

        Transform _bone = get_bone (bone_name);
		if(_bone == null)
		{
			Debug.Log("找到不到骨骼" + bone_name);
			return;
		}
        
        GameObject res = Resources.Load<GameObject>("unit/avatar/" + pack);
        GameObject _copy = Instantiate(res);
        _copy.transform.parent = _bone;
        _copy.transform.localPosition = new Vector3 (0, 0, 0);
        _copy.transform.localEulerAngles = new Vector3 (0, 0, 0);
		
		_new_part.obj = _copy;

		m_parts.Add (_new_part);
	}

	public void change_skin_part(string type, string pack, string part)
	{
		remove_part (type);
		s_part _new_part = new s_part ();
		_new_part.type = type;
        
        GameObject res = Resources.Load<GameObject>("unit/" + pack + "/" + pack);
        GameObject _copy = Instantiate(res);
        GameObject _obj = null;
        if (part == "")
        {
            _obj = _copy;
        }
        else
        {
            _obj = _copy.transform.Find(part).gameObject;
        }
        if (_obj == null)
        {
            return;
        }
        SkinnedMeshRenderer smr = _obj.transform.GetComponent<SkinnedMeshRenderer>();
		Transform[] _hips = transform.GetComponentsInChildren<Transform>();
		Transform[] _copy_hips = _copy.GetComponentsInChildren<Transform>();
		List<Transform> bones = new List<Transform>();
		foreach (Transform bone in smr.bones)
		{
			foreach (Transform hip in _hips)
			{
                if (hip.name != bone.name)
                {
                    continue;
                }
				bones.Add(hip);	
				foreach (Transform copy_hip in _copy_hips)
				{
                    if (copy_hip.name != hip.name)
                    {
                        continue;
                    }
					for (int i = 0; i < copy_hip.childCount; ++i)
					{
						GameObject _copy_obj = copy_hip.GetChild(i).gameObject;
						for(int c = 0;c < hip.childCount;c ++)
						{
							if (copy_hip.name == hip.name)
							{
								_copy_obj = null;
								break;
							}
						}
						if (_copy_obj)
						{
							GameObject _new_obj = (GameObject)Object.Instantiate(_copy_obj);
							_new_obj.name = type;
							_new_obj.transform.parent = hip;
							_new_obj.transform.localPosition = _copy_obj.transform.localPosition;
							_new_obj.transform.localRotation = _copy_obj.transform.localRotation;
							_new_obj.transform.localScale = _copy_obj.transform.localScale;
						}
					}
				}
				break;	
			}	
		}
		
		_new_part.obj = new GameObject();
		_new_part.obj.name = type;
		_new_part.obj.transform.parent = transform;

		SkinnedMeshRenderer _target = _new_part.obj.AddComponent<SkinnedMeshRenderer>();
		_target.sharedMesh = smr.sharedMesh;
		_target.bones = bones.ToArray();
		_target.materials = smr.materials;

		GameObject.Destroy (_copy);

        Material[] ms = _target.materials;
        for (int tt = 0; tt < ms.Length; ++tt)
        {
            ms[tt].SetFloat("_alpha", m_alpha);
        }
	}

	public void change_part(string name)
	{
		if (name == null)
		{
			return;
		}
		if (this.gameObject.activeInHierarchy == false)
		{
			return;
		}
		if (m_xml == null)
		{
			load_xml();
		}
		XElement unit_node = m_xml.Element("unit");		
		if (unit_node == null)
		{
			Debug.Log ("无角色脚本");
		}		
		IEnumerable<XElement> nodeList = unit_node.Elements("part");
		foreach (XElement xe in nodeList)
		{
			if(xe.Attribute("name").Value == name)
			{
				IEnumerable<XElement> _sub_xml = xe.Elements("mesh");
				foreach (XElement _sub_xe in _sub_xml)
				{
					if(_sub_xe.Attribute("bone").Value == "")
					{
						change_skin_part(_sub_xe.Attribute("type").Value, _sub_xe.Attribute("pack").Value, _sub_xe.Attribute("part").Value);
					}
					else
					{
						change_static_part(_sub_xe.Attribute("type").Value, _sub_xe.Attribute("pack").Value, _sub_xe.Attribute("bone").Value);
					}
				}
			}
		}
	}

	public void remove_part_name(string name)
	{
		if(m_xml == null)
		{
			load_xml();
		}

		XElement unit_node = m_xml.Element("unit");		
		if(unit_node == null)
		{
			Debug.Log ("无角色脚本");
		}
        
		IEnumerable<XElement> nodeList = unit_node.Elements("part");
		foreach (XElement xe in nodeList)
		{
			if(xe.Attribute("name").Value == name)
			{
				IEnumerable<XElement> _sub_xml = xe.Elements("mesh");
				foreach (XElement _sub_xe in _sub_xml)
				{
					remove_part(_sub_xe.Attribute("type").Value);
				}
			}
		}
	}

    public GameObject create_effect(string name)
    {
        GameObject _ins = Resources.Load<GameObject>("unit/" + m_name + "/" + name);
        return Instantiate(_ins);
    }

    public void play_sound(string name, GameObject target)
    {
        AudioClip clip = Resources.Load<AudioClip>("unit/" + m_name + "/sound/" + name);
        if (clip != null)
        {
            AppFacade._instance.SoundManager.play_3dsound(clip, target);
        }
    }

    public void play_monster_sound(string name, GameObject target)
    {
        AudioClip clip = Resources.Load<AudioClip>("unit/penguin_sound/" + name);
        if (clip != null)
        {
            AppFacade._instance.SoundManager.play_3dsound(clip, target);
        }
    }

    public bool has_atcion(string name)
	{
		for(int i = 0;i < m_actions.Count;i ++ )
		{
			s_action_set _set = (s_action_set)m_actions[i]; 
			
			if(_set.m_name == name)
			{
				return true;
			}
		}

		return false;
	}

    public void pause_action()
    {
        m_pause_anim = true;
        if (!m_isv)
        {
            return;
        }
        Animator _animator = transform.GetComponent<Animator>();
        if (_animator != null)
        {
            _animator.enabled = false;
        }
    }

    public void continue_action()
    {
        m_pause_anim = false;
        if (!m_isv)
        {
            return;
        }
        Animator _animator = transform.GetComponent<Animator>();
        if (_animator != null)
        {
            _animator.enabled = true;
        }
    }

    public float action(string name)
    {
        return action(name, 1.0f);
    }

    public float action(string name, float speed)
    {
        return action(0, name, speed);
    }

    public float action1(string name)
    {
        return action1(name, 1.0f);
    }

    public float action1(string name, float speed)
    {
        return action(1, name, speed);
    }

	public float action(int index, string name, float speed)
	{
        load_xml();
        if (name == null || name.Length == 0)
        {
            return 0.0f;
        }
        if (index == 0 && m_action_name[index] == name)
        {
            Animator _animator = transform.GetComponent<Animator>();
            if (_animator != null)
            {
                _animator.speed = speed;
            }
            return 0.0f;
        }
		m_action_name[index] = name;
		List<int> _aspects = new List<int>();
		for(int i = 0; i < m_actions.Count; ++i)
		{
			s_action_set _set = (s_action_set)m_actions[i]; 
			if(_set.m_name == name)
			{
				_aspects.Add(i);
			}
		}

		if(_aspects.Count == 0)
		{
			Debug.Log("单位行为无效" + name);
			return 0.0f;
		}

		int _index = Random.Range (0, _aspects.Count);
        m_action[index] = ((s_action_set)m_actions[(int)_aspects[_index]]).m_actions;
        m_action_pos[index] = 0;
        m_action_time[index] = 0.0f;
        m_action_speed[index] = speed;

        float _lenght = 0.0f;
        for (int i = 0; i < m_action[index].Count; i++)
		{
            s_action _act = (s_action)m_action[index][i];

			_lenght = _act.m_time;
		}

		return _lenght;
	}

	void action_update()
    {
        for (int i = 0; i < 2; ++i)
        {
            m_action_time[i] += Time.deltaTime * m_action_speed[i];
            if (m_action[i] == null)
            {
                continue;
            }

            while (m_action[i].Count > 0 && m_action_pos[i] < m_action[i].Count)
            {
                s_action _action = (s_action)m_action[i][m_action_pos[i]];

                if (m_action_time[i] >= _action.m_time)
                {
                    m_action_pos[i]++;
                    if (_action.m_type == "anim")
                    {
                        Animator _animator = transform.GetComponent<Animator>();
                        string _anim_name = (string)_action.m_string[0];
                        if (_animator != null)
                        {
                            add_anim_list(_anim_name);
                            _animator.speed = m_action_speed[i];
                            _animator.SetFloat("speed1", 1.0f / _animator.speed);
                        }
                    }
                    if (_action.m_type == "anim1")
                    {
                        Animator _animator = transform.GetComponent<Animator>();
                        string _anim_name = (string)_action.m_string[0];
                        if (_animator != null)
                        {
                            _animator.Play(_anim_name);
                        }
                    }
                    if (_action.m_type == "action")
                    {
                        action((string)_action.m_string[0]);
                        return;
                    }
                    if (_action.m_type == "sound")
                    {
                        play_sound((string)_action.m_string[0], gameObject);
                    }
                    if (_action.m_type == "entity")
                    {
                        int _common = (int)_action.m_ints[0];
                        GameObject _ins = null;
                        if (_common == 0)
                        {
                            _ins = create_effect((string)_action.m_string[0]);
                        }
                        else
                        {
                            _ins = AppFacade._instance.ResourceManager.CreateEffect((string)_action.m_string[0]);
                        }
                        _ins.transform.position = transform.transform.position;
                        _ins.transform.localEulerAngles = transform.transform.eulerAngles;
                        _ins.transform.localScale = new Vector3(m_scale, m_scale, m_scale);

                        GameObject.Destroy(_ins, (float)_action.m_floats[0]);
                    }

                    if (_action.m_type == "entity_bone")
                    {
                        int _follow = (int)_action.m_ints[0];
                        int _common = (int)_action.m_ints[1];
                        GameObject _ins = null;
                        if (_common == 0)
                        {
                            _ins = create_effect((string)_action.m_string[0]);
                        }
                        else
                        {
                            _ins = AppFacade._instance.ResourceManager.CreateEffect((string)_action.m_string[0]);
                        }
                        Transform _bone = get_bone((string)_action.m_string[1]);

                        if (_bone)
                        {
                            if (_follow == 1)
                            {
                                _ins.transform.parent = _bone;
                                _ins.transform.localPosition = new Vector3(0, 0, 0);
                                _ins.transform.localEulerAngles = new Vector3(0, 0, 0);
                                _ins.transform.localScale = new Vector3(1, 1, 1);
                            }
                            else
                            {
                                _ins.transform.position = _bone.position;
                                _ins.transform.localEulerAngles = _bone.eulerAngles;
                                _ins.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
                            }
                        }

                        GameObject.Destroy(_ins, (float)_action.m_floats[0]);
                    }
                    if (_action.m_type == "shake_cam")
                    {
                        if (m_main)
                        {
                            AppFacade._instance.MapManager.shake_cam((float)_action.m_floats[0]);
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            if (m_action_pos[i] >= m_action[i].Count)
            {
                m_action[i] = null;
                continue;
            }
        }
	}

    void add_anim_list(string anim_name)
    {
        if (m_next_anim == anim_name)
        {
            m_anim_list.Clear();
            return;
        }

        int index = -1;
        for (int i = 0; i < m_anim_list.Count; ++i)
        {
            if (m_anim_list[i] == anim_name)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            int num = m_anim_list.Count - index - 1;
            m_anim_list.RemoveRange(index + 1, num);
        }
        else
        {
            m_anim_list.Add(anim_name);
            check_anim_list();
        }
    }

    void check_anim_list()
    {
        Animator _animator = transform.GetComponent<Animator>();
        if (!_animator.enabled)
        {
            return;
        }
        if (m_next_anim == "" || _animator.GetCurrentAnimatorStateInfo(0).IsName(m_next_anim))
        {
            if (m_anim_list.Count != 0)
            {
                m_next_anim = m_anim_list[0];
                m_anim_list.RemoveAt(0);
                _animator.CrossFade(m_next_anim, 0.1f);
            }
        }
    }

    public GameObject Attach(string bone, string name, bool yj,float? speed = null)
    {
        bool f = !m_isv || m_alpha <= 0;
        if (!yj && f)
        {
            return null;
        }
        if (bone == "")
        {
		    bone = "accept";
        }
	    Transform t = get_bone(bone);
	    GameObject eff = AppFacade._instance.ResourceManager.CreateEffect(name);
	    eff.transform.parent = t;
        if (name == "Unit_buff_IceWing")
        {
            eff.transform.localPosition = new Vector3(0.65f, -0.1f, -0.05f);
            eff.transform.localEulerAngles = new Vector3(55.87f, 133.48f, -133.92f);
        }
        else if (name == "Unit_DebuffLoop_SpeedUp")
        {
            eff.transform.localPosition = Vector3.zero;
            eff.transform.localEulerAngles = Vector3.zero;
        }
        else if (xuli_effects.Contains(name))
        {
            if (speed == null)
                speed = 1.0f;

            ParticleSystem[] particles = eff.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particles.Length; i++)
                particles[i].playbackSpeed = speed.GetValueOrDefault();

            eff.transform.localPosition = Vector3.zero;
            eff.transform.eulerAngles = Vector3.zero;
        }
        else
        {
            eff.transform.localPosition = Vector3.zero;
            eff.transform.eulerAngles = Vector3.zero;
        }
        eff.transform.localScale = Vector3.one;
        objyh sc = eff.GetComponent<objyh>();
        if (sc != null)
        {
            if (f)
            {
                sc.BecameInvisible();
            }
            sc.enabled = false;
        }
	    if (!yj)
        {
		    AppFacade._instance.ResourceManager.DeleteEffect(eff, 2);
        }
	    return eff;
    }

    public GameObject Attach1(string bone, string name)
    {
        bool f = !m_isv || m_alpha <= 0;
        if (f)
        {
            return null;
        }
        if (bone == "")
        {
		    bone = "accept";
        }
	    Transform t = get_bone(bone);
	    GameObject eff = AppFacade._instance.ResourceManager.CreateEffect(name);
        eff.transform.parent = AppFacade._instance.ResourceManager.UnitRoot.transform;
	    eff.transform.localPosition = t.position;
        eff.transform.localEulerAngles = Vector3.zero;
        eff.transform.localScale = Vector3.one;
        AppFacade._instance.ResourceManager.DeleteEffect(eff, 2);
	    return eff;
    }

    void IHandle.net_message(s_net_message message)
    {

    }

    void IHandle.message(s_message message)
    {
        if (!AppFacade._instance.test)
        {
            return;
        }
        if (message.name == "change_part")
        {
            change_part((string)message.m_object[0]);
        }
        else if(message.name == "action")
        {
            float speed = float.Parse((string)message.m_object[1]);
            action((string)message.m_object[0], speed);
        }
    }

    public bool can_tiox(float action_tiox)
    {
        if (m_action_name[0] == "ready" && m_action_time[0] > action_tiox)
        {
            return true;
        }
        return false;
    }

    void Update ()
    {
        test_init();
        load_xml();
        if (m_isv)
        {
            alpha();
            do_white();
        }
        action_update();
        check_anim_list();
        bool vis = true;
        Vector3 v = AppFacade._instance.MapManager.WorldToScreenPoint(transform.position);
        float w = AppFacade._instance.PanelManager.get_w();
        float h = AppFacade._instance.PanelManager.get_h();
        if (v.x < -50 || v.x > w + 50 || v.y < -50 || v.y > h + 100 && !m_is_show)
        {
            vis = false;
        }
        if (m_isv && !vis)
        {
            BecameInvisible();
        }
        else if (!m_isv && vis)
        {
            BecameVisible();
        }
    }
}
