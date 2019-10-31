using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bobjpool_sub
{
    public int tid;
    public GameObject obj;
    public int type;
    public int tchid;
    public float ypy;
    public float ysc;
    public Vector3 v;
}

public class bobjpool {

    private int tid_ = 0;
    private Dictionary<int, bobjpool_sub> objs_ = new Dictionary<int, bobjpool_sub>();
    private HashSet<int> update_ids_ = new HashSet<int>();

    public int add(GameObject obj)
    {
        bobjpool_sub bs = new bobjpool_sub();
        bs.tid = tid_;
        bs.obj = obj;
        bs.type = 0;
        objs_[tid_] = bs;
        tid_++;
        return tid_ - 1;
    }

    public void set_type1(int id, int tid, float ypy, float ysc)
    {
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        if (!objs_.ContainsKey(tid))
        {
            return;
        }
        objs_[id].type = 1;
        objs_[id].tchid = tid;
        objs_[id].ypy = ypy;
        objs_[id].ysc = ysc;
        update_ids_.Add(id);
    }

    public void set_type2(int id, float x, float y, float z)
    {
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        objs_[id].type = 2;
        objs_[id].v = new Vector3(x, y, z);
        update_ids_.Add(id);
    }

    public void remove(int id)
    {
        objs_.Remove(id);
        update_ids_.Remove(id);
    }

    public void clear()
    {
        objs_.Clear();
        tid_ = 0;
    }

    public void update()
    {
        foreach (int tid in update_ids_)
        {
            if (objs_.ContainsKey(tid))
            {
                bobjpool_sub bs = objs_[tid];
                if (bs.type == 1 && bs.obj.activeSelf)
                {
                    if (objs_.ContainsKey(bs.tchid))
                    {
                        bobjpool_sub tbs = objs_[bs.tchid];
                        Vector3 v = tbs.obj.transform.position;
                        v.y = v.y + bs.ypy + bs.ysc * tbs.obj.transform.parent.localScale.y;
                        bs.obj.transform.localPosition = AppFacade._instance.MapManager.WorldToScreenPoint(v);
                    }
                }
                else if (bs.type == 2)
                {
                    bs.obj.transform.localPosition = AppFacade._instance.MapManager.WorldToScreenPoint(bs.v);
                }
            }
        }
    }

	public void get_localPosition(int id, out float x, out float y, out float z)
    {
        x = 0;
        y = 0;
        z = 0;
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        Vector3 v = objs_[id].obj.transform.localPosition;
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public void set_localPosition(int id, float x, float y, float z)
    {
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        objs_[id].obj.transform.localPosition = new Vector3(x, y, z);
    }

    public void get_position(int id, out float x, out float y, out float z)
    {
        x = 0;
        y = 0;
        z = 0;
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        Vector3 v = objs_[id].obj.transform.position;
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public void set_position(int id, float x, float y, float z)
    {
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        objs_[id].obj.transform.position = new Vector3(x, y, z);
    }

    public void get_localEulerAngles(int id, out float x, out float y, out float z)
    {
        x = 0;
        y = 0;
        z = 0;
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        Vector3 v = objs_[id].obj.transform.localEulerAngles;
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public void set_localEulerAngles(int id, float x, float y, float z)
    {
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        objs_[id].obj.transform.localEulerAngles = new Vector3(x, y, z);
    }

    public void get_eulerAngles(int id, out float x, out float y, out float z)
    {
        x = 0;
        y = 0;
        z = 0;
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        Vector3 v = objs_[id].obj.transform.eulerAngles;
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public void set_eulerAngles(int id, float x, float y, float z)
    {
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        objs_[id].obj.transform.eulerAngles = new Vector3(x, y, z);
    }

    public void get_localScale(int id, out float x, out float y, out float z)
    {
        x = 0;
        y = 0;
        z = 0;
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        Vector3 v = objs_[id].obj.transform.localScale;
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public void set_localScale(int id, float x, float y, float z)
    {
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        objs_[id].obj.transform.localScale = new Vector3(x, y, z);
    }

    public void LookAt(int id, float x, float y, float z)
    {
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        objs_[id].obj.transform.LookAt(new Vector3(x, y, z));
    }

    public void set_WorldToScreenPoint(int id, float x, float y, float z)
    {
        if (!objs_.ContainsKey(id))
        {
            return;
        }
        objs_[id].obj.transform.localPosition = AppFacade._instance.MapManager.WorldToScreenPoint(new Vector3(x, y, z));
    }
}
