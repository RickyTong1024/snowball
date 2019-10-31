using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public delegate void UpdateBeat();
public delegate void LateUpdateBeat();
public delegate void FixUpdateBeat();

public class BattleTask:MonoBehaviour
{
    private Dictionary<string, UpdateBeat> update_list = new Dictionary<string, UpdateBeat>();
    private Dictionary<string, LateUpdateBeat> lateupdate_list = new Dictionary<string, LateUpdateBeat>();
    private Dictionary<string, FixUpdateBeat> fixupdate_list = new Dictionary<string, FixUpdateBeat>();

    private void Awake()
    {
        update_list.Clear();
        lateupdate_list.Clear();
        fixupdate_list.Clear();
    }

    private void Start()
    {

    }
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        if (update_list != null && update_list.Count > 0)
        {
            foreach (var item in update_list.Values)
                item();
        }
    }

    private void LateUpdate()
    {
        if (lateupdate_list != null && lateupdate_list.Count > 0)
        {
            foreach (var item in lateupdate_list.Values)
                item();
        }
    }

    private void FixedUpdate()
    {
        if (fixupdate_list != null && fixupdate_list.Count > 0)
        {
            foreach (var item in fixupdate_list.Values)
                item();
        }
    }

    public void AddUpdate(string name, UpdateBeat e)
    {
        if (!update_list.ContainsKey(name))
        {
            update_list.Add(name, e);
        }
    }

    public void RemoveUpdate(string name)
    {
        if (update_list.ContainsKey(name))
            update_list.Remove(name);
    }

    public void AddLateUpdate(string name, LateUpdateBeat e)
    {
        if (!lateupdate_list.ContainsKey(name))
        {
            lateupdate_list.Add(name, e);
        }
    }

    public void RemoveLateUpdate(string name)
    {
        if (lateupdate_list.ContainsKey(name))
            lateupdate_list.Remove(name);
    }

    public void AddFixUpdate(string name, FixUpdateBeat e)
    {
        if (!fixupdate_list.ContainsKey(name))
        {
            fixupdate_list.Add(name, e);
        }
    }

    public void RemoveFixUpdate(string name)
    {
        if (fixupdate_list.ContainsKey(name))
            fixupdate_list.Remove(name);
    }

    private void OnDestroy()
    {
        if (update_list != null)
            update_list.Clear();
        if (lateupdate_list != null)
            lateupdate_list.Clear();
        if (fixupdate_list != null)
            fixupdate_list.Clear();
    }
}

