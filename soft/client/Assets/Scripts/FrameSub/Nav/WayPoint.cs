using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class WayPoint
{
    public Vector2i m_cPoint;           //位置点
    public NavTriangle m_cTriangle;    //所在三角形

    public WayPoint()
    {
        this.m_cPoint = null;
        this.m_cTriangle = null;
    }

    public WayPoint(Vector2i pos, NavTriangle tri)
    {
        this.m_cPoint = pos;
        this.m_cTriangle = tri;
    }

    // 获取路径点
    public Vector2i GetPoint()
    {
        return this.m_cPoint;
    }

    public void Clear()
    {
        this.m_cPoint = null;
        this.m_cTriangle = null;
    }
    // 获取路径三角形
    public NavTriangle GetTriangle()
    {
        return this.m_cTriangle;
    }
}

