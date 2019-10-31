using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class objyh : MonoBehaviour {

    private bool m_isv = true;
    private float m_time = 0;

    void Start()
    {
        m_time = Random.Range(0, 1000) / 1000.0f * 0.3f;
    }
    public void BecameVisible()
    {
        m_isv = true;
        show(transform);
    }

    public void BecameInvisible()
    {
        m_isv = false;
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

            Animator _animator = transform.GetComponent<Animator>();
            if (_animator != null)
            {
                _animator.enabled = true;
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
            Animator _animator = transform.GetComponent<Animator>();
            if (_animator != null)
            {
                _animator.enabled = false;
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

    void LateUpdate()
    {
        m_time += Time.deltaTime / Time.timeScale;
        if (m_time < 0.3f)
        {
            return;
        } 
        m_time = 0;
        bool vis = true;
        Vector3 v = AppFacade._instance.MapManager.WorldToScreenPoint(transform.position);
        float w = AppFacade._instance.PanelManager.get_w();
        float h = AppFacade._instance.PanelManager.get_h();
        if (v.x < -50 || v.x > w + 50 || v.y < -50 || v.y > h + 100)
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
