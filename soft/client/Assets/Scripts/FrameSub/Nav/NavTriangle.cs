using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class NavRect
{
    public Int64 xMin, xMax, yMin, yMax;
    public override string ToString()
    {
        return "(" +xMin+","+xMax+","+yMin+","+yMax+")";
    }
}

public class NavTriangle:IComparable<NavTriangle>
{
    public Vector2i PointA, PointB, PointC;
    public Vector2i[] vecPoints;  //顶点的 列表
    public Line2D AB,BC,CA;

    private int[] neighborList = null;
    private Int64[] gCost;
    public int ID;  //当前三角形的instanceID


    //A* 寻路 中存放 G值 和 H值
    private Int64 G,H,C;
    public NavTriangle parent; //存放 A* 寻路中的上级节点
    public bool IsOpen = false;
    private int in_Line, out_Line;  //寻路的 传入传出边
    //A*寻路之前 将 所有三角形的 数据 初始化
    public int astar_state = 0;
    public void InitAStarBefore()
    {
        this.IsOpen = false;
        this.parent = null;
        this.G = 0;
        this.H = 0;
        this.C = 0;
        this.in_Line = -1;  // 传入边  进入 这个三角形
        this.out_Line = -1;  //传出边  离开这个三角形
    }

    public int GetOutLine()
    {
        return out_Line;
    }

    public int GetInLine()
    {
        return in_Line;
    }

    public void SetOutLine(int instanceID)
    {
        int index = GetWallIndex(instanceID);
        if (index == -1)
            return;
        out_Line = index;
    }

    public void SetInLine(int instanceID)
    {
        int index = GetWallIndex(instanceID);
        if (index == -1)
            return;
        in_Line = index;
    }

    //根据 instanceID 去寻找 它是那条边
    public int GetWallIndex(int neighborID)
    {
        for (int i = 0; i < 3; i++)
        {
            if (this.neighborList[i] != -1 && this.neighborList[i] == neighborID)
                return i;
        }
        return -1;
    }

    //包围盒
    NavRect colliderRect; //包围盒
    public NavTriangle()
    {
        neighborList = new int[3] { -1, -1, -1 };
        gCost = new Int64[3] { -1, -1, -1 };
    }
    public NavTriangle(Vector2i pa, Vector2i pb, Vector2i pc)
    {
        vecPoints = new Vector2i[3];
        this.PointA = pa;
        this.PointB = pb;
        this.PointC = pc;
        this.AB = new Line2D(pa, pb);
        this.BC = new Line2D(pb, pc);
        this.CA = new Line2D(pc, pa);
        vecPoints[0] = pa;
        vecPoints[1] = pb;
        vecPoints[2] = pc;
        CalcCollider();

        neighborList = new int[3] { -1, -1, -1 };
        gCost = new Int64[3] { -1, -1, -1 };
    }

    // 三角形的中点
    public Vector2i Center;

    //计算包围盒
    private void CalcCollider()
    {
        NavRect collider = new NavRect();
        collider.xMin = collider.xMax = this.vecPoints[0].x;
        collider.yMin = collider.yMax = this.vecPoints[0].y;
        for (int i = 1; i < 3; i++)
        {
            if (this.vecPoints[i].x < collider.xMin)
                collider.xMin = this.vecPoints[i].x;
            else if (this.vecPoints[i].x > collider.xMax)
                collider.xMax = this.vecPoints[i].x;

            if (this.vecPoints[i].y < collider.yMin)
                collider.yMin = this.vecPoints[i].y;
            else if (this.vecPoints[i].y > collider.yMax)
                collider.yMax = this.vecPoints[i].y;
        }

        this.colliderRect = collider;
    }
    //判断顶点 是否在 这个三角形中
    public bool IsPointIn(Vector2i pt)
    {
        if ((this.colliderRect.xMin > pt.x || pt.x > this.colliderRect.xMax)
            || (this.colliderRect.yMin > pt.y || pt.y > this.colliderRect.yMax))
            return false;

        PointSide resultA = GetSide(0).ClassifyPoint(pt);
        PointSide resultB = GetSide(1).ClassifyPoint(pt);
        PointSide resultC = GetSide(2).ClassifyPoint(pt);

        if (resultA == PointSide.ON_LINE || resultB == PointSide.ON_LINE || resultC == PointSide.ON_LINE)
        {
            return true;
        }
        else if (resultA == PointSide.RIGHT_SIDE && resultB == PointSide.RIGHT_SIDE && resultC == PointSide.RIGHT_SIDE)
        {
            return true;
        }
        return false;
    }

    public Line2D GetSide(int sideIndex)
    {
        Line2D newSide;
        switch (sideIndex)
        {
            case 0:
                newSide = this.AB;
                break;
            case 1:
                newSide = this.BC;
                break;
            case 2:
                newSide = this.CA;
                break;
            default:
                newSide = this.AB;
                break;
        }
        return newSide;
    }

    // 计算邻居节点
    public int isNeighbor(NavTriangle triNext)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (GetSide(i).Equals(triNext.GetSide(j)))
                    return i;
            }
        }
        return -1;
    }

    public int GetNeighbor(int index)
    {
        return neighborList[index];
    }

    //设置 邻居节点 信息 进入
    public void SetNeighbor(int sideID,int instanceID)
    {
        if (sideID >= 3)
            return;
        neighborList[sideID] = instanceID;
    }

    public Int64 GetGValue()  //设置G值
    {
        return this.G;
    }

    public void SetGValue(Int64 G)  //获取 G值
    {
        this.G = G;
        this.C = this.G + this.H;
    }

    public void SetNeighborGCost(int index, Int64 value)
    {
        if (index >= 3 || index < 0)
            return;
        gCost[index] = value;
    }

    public Int64 GetNeighborGCost(int index)
    {
        if (index >= 3 || index < 0)
            return -1;
        return gCost[index];
    }

    //计算 H值
    public void CalcHeuristic(NavTriangle endCell)
    {
        Int64 xDelta = Math.Abs(this.Center.x - endCell.Center.x);
        Int64 yDelta = Math.Abs(this.Center.y - endCell.Center.y);
        this.H = xDelta + yDelta;
        this.C = this.G + this.H;
    }

    //计算 H值
    public Int64 GetHCost()
    {
        return this.H;
    }

    public Int64 GetCost()
    {
        return this.C;
    }

    public int CompareTo(NavTriangle other)
    {
        Int64 f1 = this.GetGValue() + this.GetHCost();
        Int64 f2 = other.GetGValue() + other.GetHCost();

        if (f2 > f1)
            return -1;
        else if (f1 == f2)
            return 0;
        else
            return 1;
    }
}

