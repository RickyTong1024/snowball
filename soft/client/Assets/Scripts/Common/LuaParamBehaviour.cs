using UnityEngine;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System;

public class LuaParamBehaviour : MonoBehaviour
{
    [SerializeField]
    private string m_name;
    public List<int> m_ints = new List<int>();
    public List<string> m_strings = new List<string>();

    protected void Awake()
    {
        Util.CallMethod(m_name, "Awake", gameObject);
    }

    protected void Start()
    {
        Util.CallMethod(m_name, "Start");
    }
}
