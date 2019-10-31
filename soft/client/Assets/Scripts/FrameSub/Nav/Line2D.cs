using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public enum PointSide
{
    ON_LINE = 0,    //在线段上
    LEFT_SIDE = 1,  //在线段左边
    RIGHT_SIDE = 2, //在线段右边
};

/// <summary>
/// 两线段交叉状态
/// </summary>
public enum LineCrossState
{
    COLINE = 0, //外线口
    PARALLEL,   //平行线
    CROSS,      //相交
    NOT_CROSS   //无相交
}

public class Line2D
{
    public Vector2i startPoint;
    public Vector2i endPoint;

    public Line2D()
    {
        startPoint = Vector2i.zero;
        endPoint = Vector2i.zero;
    }

    public Line2D(Vector2i startPoint, Vector2i endPoint)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }

    /// <summary>
    /// 判断点与直线的关系，假设你站在a点朝向b点， 
    /// 则输入点与直线的关系分为：Left, Right or Centered on the line
    /// </summary>
    /// <param name="point">判断点</param>
    /// <returns>判断结果</returns>
    public PointSide ClassifyPoint(Vector2i point)
    {
        if (point == this.startPoint || point == this.endPoint)
            return PointSide.ON_LINE;

        Int64 crossResult = NavUtil.CrossProduct(this.startPoint, this.endPoint, point);//NavUtil.CrossProduct(vectorA, vectorB);

        if (crossResult == 0)
            return PointSide.ON_LINE;
        else if (crossResult < 0)
            return PointSide.RIGHT_SIDE;
        else
            return PointSide.LEFT_SIDE;
    }

    // 获得直线方向
    public Vector2i GetDirection()
    {
        Vector2i dir = this.endPoint - this.startPoint;
        return dir;
    }

    // 两条线段是否相等
    public bool Equals(Line2D line)
    {
        //只是一个点
        if (NavUtil.IsEqualZero(line.startPoint - line.endPoint) ||
            NavUtil.IsEqualZero(startPoint - endPoint))
            return false;

        //whatever the direction
        bool bEquals = NavUtil.IsEqualZero(startPoint - line.startPoint) ? true : NavUtil.IsEqualZero(startPoint - line.endPoint);
        if (bEquals)
        { 
            bEquals = NavUtil.IsEqualZero(endPoint - line.startPoint) ? true : NavUtil.IsEqualZero(endPoint - line.endPoint);
        }
        return bEquals;
    }

    public override string ToString()
    {
        if (startPoint.x > endPoint.x)
            return "[" + endPoint.ToString() + "," + startPoint.ToString() + "]";
        else if (startPoint.x == endPoint.x)
        {
            if (startPoint.y < endPoint.y)
                return "[" + endPoint.ToString() + "," + startPoint.ToString() + "]";
            else
                return "[" + startPoint.ToString() + "," + endPoint.ToString() + "]";
        }
        else
            return "[" + startPoint.ToString() + "," + endPoint.ToString() + "]";
    }
}

