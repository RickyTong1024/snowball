using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class NavPolygon
{
    public NavPolygon()
    {
        if (vertexs == null)
            vertexs = new List<Vector2i>();
        else
            vertexs.Clear();
    }

    public List<Vector2i> vertexs;  //存放 多边形的 顶点
    //返回 顶点的数量
    public int vertexsNum
    {
        get
        {
            if (vertexs == null)
                vertexs = new List<Vector2i>();
            return vertexs.Count;
        }
    }

    public List<NavTriangle> childList = new List<NavTriangle>(); //孩子的序号
    public int ID;
    public List<NavTriangle> Splice2Trianles()
    {
        List<NavTriangle> nts = new List<NavTriangle>();
        if (this.vertexsNum == 3)
        {
            NavTriangle nt = new NavTriangle(vertexs[0], vertexs[1], vertexs[2]);
            nts.Add(nt);
        }
        else
        {
            Vector2i firstPoint, secPoint, thirdPoint;
            firstPoint = vertexs[0];
            //简单 分割  012 - 023 - 034 -045 ..
            for (int i = 1; i < vertexsNum - 1; i++)
            {
                secPoint = vertexs[i];
                thirdPoint = vertexs[i + 1];
                NavTriangle nt = new NavTriangle(firstPoint,secPoint,thirdPoint);
                nts.Add(nt);
            }
        }
        return nts;
    }

    public void CW()
    {
        if (this.IsCW() == false)
        {   //如果为逆时针顺序， 反转为顺时针
            this.vertexs.Reverse(); //反转数组
        }
    }
    //clockwise 时钟的顺序 即顺时针
    public bool IsCW()  
    {
        if (vertexs == null || vertexs.Count < 0)
            return false;

        Vector2i topPt = this.vertexs[0];
        int topPtId = 0;
        for (int i = 1; i < vertexs.Count; i++)
        {
            if (topPt.y > vertexs[i].y)
            {
                topPt = vertexs[i];
                topPtId = i;
            }
            else if (topPt.y == vertexs[i].y)
            { //y相等时取x最小
                if (topPt.x > vertexs[i].x)
                {
                    topPt = vertexs[i];
                    topPtId = i;
                }
            }
        }

        //凸点的邻点
        int lastId = topPtId - 1 >= 0 ? topPtId - 1 : vertexs.Count - 1;
        int nextId = topPtId + 1 >= vertexs.Count ? 0 : topPtId + 1;
        Vector2i last = vertexs[lastId];
        Vector2i next = vertexs[nextId];
        //三点共线情况不存在，若三点共线则说明必有一点的y（斜线）或x（水平线）小于topPt
        Int64 r = multiply(last, next, topPt);
        if (r < 0)
            return true;

        return false;
    }

    //判断 当前的多边形是否是 凸多边形
    //> 0 表示 A在B的顺时针方向; <0表示A在B的逆时针方向; =0 表示则为共线向量(有可能同向，有可能
    public bool IsTpolygon()
    {
        if (vertexs == null || vertexs.Count <= 3)
            return true;

        if (!IsCW())  // 如果不是 顺时针 翻转
            CW();

        for (int i = 0; i < vertexs.Count; i++)
        {
            Vector2i firstPoint, secPoint,thirdPoint;

            firstPoint = vertexs[i % vertexsNum];
            secPoint = vertexs[(i + 1) % vertexsNum];
            thirdPoint = vertexs[(i + vertexsNum - 1) % vertexsNum];
            Int64 r = multiply(thirdPoint, secPoint, firstPoint);
            if (r > 0)
                return false;  
        }

        return true;
    }

    //last, next, topPt
    private Int64 multiply(Vector2i sp, Vector2i ep, Vector2i op)
    {
        return ((sp.x - op.x) * (ep.y - op.y) - (ep.x - op.x) * (sp.y - op.y));
    }

    public Line2D GetSide(int index)
    {
        if (index < 0 || index >= this.vertexsNum)
            return null;

        Line2D newSide;
        newSide = new Line2D(this.vertexs[index], this.vertexs[(index + 1)%this.vertexsNum]);
        return newSide;
    }

    public override string ToString()
    {
        return this.vertexs[0].ToString() + " , " + this.vertexs[1].ToString() + " , " + this.vertexs[2].ToString();
    }
}
