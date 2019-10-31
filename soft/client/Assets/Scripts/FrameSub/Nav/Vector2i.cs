using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Vector2i
{
    public Int64 x;

    public Int64 y;

    public Vector2i(Int64 x, Int64 y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2i()
    {
        x = 0;
        y = 0;
    }

    public static Vector2i zero { get { return new Vector2i(0, 0); } }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override string ToString()
    {
        return "("+x+","+y+")";
    }

    public static Vector2i operator+(Vector2i a, Vector2i b)
    {
        Vector2i v = new Vector2i();
        v.x = a.x + b.x;
        v.y = a.y + b.y;
        return v;
    }

    public static Vector2i operator -(Vector2i a, Vector2i b)
    {
        Vector2i v = new Vector2i();
        v.x = a.x - b.x;
        v.y = a.y - b.y;
        return v;
    }

    public static Vector2i operator *(Vector2i a, Vector2i b)
    {
        Vector2i v = new Vector2i();
        v.x = a.x * b.x;
        v.y = a.y * b.y;
        return v;
    }

    public static Vector2i operator /(Vector2i a, Vector2i b)
    {
        Vector2i v = new Vector2i();
        v.x = a.x / b.x;
        v.y = a.y / b.y;
        return v;
    }

    public static Vector2i operator /(Vector2i a, int b)
    {
        Vector2i v = new Vector2i();
        v.x = a.x / b;
        v.y = a.y / b;
        return v;
    }

    public static bool operator !=(Vector2i a, Vector2i b)
    {
        return !((a.x == b.x) && (a.y == b.y));
    }

    public static bool operator ==(Vector2i a, Vector2i b)
    {
        return (a.x == b.x) && (a.y == b.y);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

