using BattleDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BattleOperation
{
    private static long[] msin = { 0, 175, 349, 523, 698, 872, 1045, 1219, 1392, 1564, 1736, 1908, 2079, 2250, 2419, 2588, 2756, 2924, 3090, 3256, 3420, 3584, 3746, 3907, 4067, 4226, 4384, 4540, 4695, 4848, 5000, 5150, 5299, 5446, 5592, 5736, 5878, 6018, 6157, 6293, 6428, 6561, 6691, 6820, 6947, 7071, 7193, 7314, 7431, 7547, 7660, 7771, 7880, 7986, 8090, 8192, 8290, 8387, 8480, 8572, 8660, 8746, 8829, 8910, 8988, 9063, 9135, 9205, 9272, 9336, 9397, 9455, 9511, 9563, 9613, 9659, 9703, 9744, 9781, 9816, 9848, 9877, 9903, 9925, 9945, 9962, 9976, 9986, 9994, 9998, 10000, 9998, 9994, 9986, 9976, 9962, 9945, 9925, 9903, 9877, 9848, 9816, 9781, 9744, 9703, 9659, 9613, 9563, 9511, 9455, 9397, 9336, 9272, 9205, 9135, 9063, 8988, 8910, 8829, 8746, 8660, 8572, 8480, 8387, 8290, 8192, 8090, 7986, 7880, 7771, 7660, 7547, 7431, 7314, 7193, 7071, 6947, 6820, 6691, 6561, 6428, 6293, 6157, 6018, 5878, 5736, 5592, 5446, 5299, 5150, 5000, 4848, 4695, 4540, 4384, 4226, 4067, 3907, 3746, 3584, 3420, 3256, 3090, 2924, 2756, 2588, 2419, 2250, 2079, 1908, 1736, 1564, 1392, 1219, 1045, 872, 698, 523, 349, 175, 0, -175, -349, -523, -698, -872, -1045, -1219, -1392, -1564, -1736, -1908, -2079, -2250, -2419, -2588, -2756, -2924, -3090, -3256, -3420, -3584, -3746, -3907, -4067, -4226, -4384, -4540, -4695, -4848, -5000, -5150, -5299, -5446, -5592, -5736, -5878, -6018, -6157, -6293, -6428, -6561, -6691, -6820, -6947, -7071, -7193, -7314, -7431, -7547, -7660, -7771, -7880, -7986, -8090, -8192, -8290, -8387, -8480, -8572, -8660, -8746, -8829, -8910, -8988, -9063, -9135, -9205, -9272, -9336, -9397, -9455, -9511, -9563, -9613, -9659, -9703, -9744, -9781, -9816, -9848, -9877, -9903, -9925, -9945, -9962, -9976, -9986, -9994, -9998, -10000, -9998, -9994, -9986, -9976, -9962, -9945, -9925, -9903, -9877, -9848, -9816, -9781, -9744, -9703, -9659, -9613, -9563, -9511, -9455, -9397, -9336, -9272, -9205, -9135, -9063, -8988, -8910, -8829, -8746, -8660, -8572, -8480, -8387, -8290, -8192, -8090, -7986, -7880, -7771, -7660, -7547, -7431, -7314, -7193, -7071, -6947, -6820, -6691, -6561, -6428, -6293, -6157, -6018, -5878, -5736, -5592, -5446, -5299, -5150, -5000, -4848, -4695, -4540, -4384, -4226, -4067, -3907, -3746, -3584, -3420, -3256, -3090, -2924, -2756, -2588, -2419, -2250, -2079, -1908, -1736, -1564, -1392, -1219, -1045, -872, -698, -523, -349, -175 };
    private static long[] mcos = { 10000, 9998, 9994, 9986, 9976, 9962, 9945, 9925, 9903, 9877, 9848, 9816, 9781, 9744, 9703, 9659, 9613, 9563, 9511, 9455, 9397, 9336, 9272, 9205, 9135, 9063, 8988, 8910, 8829, 8746, 8660, 8572, 8480, 8387, 8290, 8192, 8090, 7986, 7880, 7771, 7660, 7547, 7431, 7314, 7193, 7071, 6947, 6820, 6691, 6561, 6428, 6293, 6157, 6018, 5878, 5736, 5592, 5446, 5299, 5150, 5000, 4848, 4695, 4540, 4384, 4226, 4067, 3907, 3746, 3584, 3420, 3256, 3090, 2924, 2756, 2588, 2419, 2250, 2079, 1908, 1736, 1564, 1392, 1219, 1045, 872, 698, 523, 349, 175, 0, -175, -349, -523, -698, -872, -1045, -1219, -1392, -1564, -1736, -1908, -2079, -2250, -2419, -2588, -2756, -2924, -3090, -3256, -3420, -3584, -3746, -3907, -4067, -4226, -4384, -4540, -4695, -4848, -5000, -5150, -5299, -5446, -5592, -5736, -5878, -6018, -6157, -6293, -6428, -6561, -6691, -6820, -6947, -7071, -7193, -7314, -7431, -7547, -7660, -7771, -7880, -7986, -8090, -8192, -8290, -8387, -8480, -8572, -8660, -8746, -8829, -8910, -8988, -9063, -9135, -9205, -9272, -9336, -9397, -9455, -9511, -9563, -9613, -9659, -9703, -9744, -9781, -9816, -9848, -9877, -9903, -9925, -9945, -9962, -9976, -9986, -9994, -9998, -10000, -9998, -9994, -9986, -9976, -9962, -9945, -9925, -9903, -9877, -9848, -9816, -9781, -9744, -9703, -9659, -9613, -9563, -9511, -9455, -9397, -9336, -9272, -9205, -9135, -9063, -8988, -8910, -8829, -8746, -8660, -8572, -8480, -8387, -8290, -8192, -8090, -7986, -7880, -7771, -7660, -7547, -7431, -7314, -7193, -7071, -6947, -6820, -6691, -6561, -6428, -6293, -6157, -6018, -5878, -5736, -5592, -5446, -5299, -5150, -5000, -4848, -4695, -4540, -4384, -4226, -4067, -3907, -3746, -3584, -3420, -3256, -3090, -2924, -2756, -2588, -2419, -2250, -2079, -1908, -1736, -1564, -1392, -1219, -1045, -872, -698, -523, -349, -175, 0, 175, 349, 523, 698, 872, 1045, 1219, 1392, 1564, 1736, 1908, 2079, 2250, 2419, 2588, 2756, 2924, 3090, 3256, 3420, 3584, 3746, 3907, 4067, 4226, 4384, 4540, 4695, 4848, 5000, 5150, 5299, 5446, 5592, 5736, 5878, 6018, 6157, 6293, 6428, 6561, 6691, 6820, 6947, 7071, 7193, 7314, 7431, 7547, 7660, 7771, 7880, 7986, 8090, 8192, 8290, 8387, 8480, 8572, 8660, 8746, 8829, 8910, 8988, 9063, 9135, 9205, 9272, 9336, 9397, 9455, 9511, 9563, 9613, 9659, 9703, 9744, 9781, 9816, 9848, 9877, 9903, 9925, 9945, 9962, 9976, 9986, 9994, 9998 };
    private static long[] mtan = { -572899, -286362, -190811, -143006, -114300, -95143, -81443, -71153, -63137, -56712, -51445, -47046, -43314, -40107, -37320, -34874, -32708, -30776, -29042, -27474, -26050, -24750, -23558, -22460, -21445, -20503, -19626, -18807, -18040, -17320, -16642, -16003, -15398, -14825, -14281, -13763, -13270, -12799, -12348, -11917, -11503, -11106, -10723, -10355, -10000, -9656, -9325, -9004, -8692, -8390, -8097, -7812, -7535, -7265, -7002, -6745, -6494, -6248, -6008, -5773, -5543, -5317, -5095, -4877, -4663, -4452, -4244, -4040, -3838, -3639, -3443, -3249, -3057, -2867, -2679, -2493, -2308, -2125, -1943, -1763, -1583, -1405, -1227, -1051, -874, -699, -524, -349, -174, 0, 174, 349, 524, 699, 874, 1051, 1227, 1405, 1583, 1763, 1943, 2125, 2308, 2493, 2679, 2867, 3057, 3249, 3443, 3639, 3838, 4040, 4244, 4452, 4663, 4877, 5095, 5317, 5543, 5773, 6008, 6248, 6494, 6745, 7002, 7265, 7535, 7812, 8097, 8390, 8692, 9004, 9325, 9656, 9999, 10355, 10723, 11106, 11503, 11917, 12348, 12799, 13270, 13763, 14281, 14825, 15398, 16003, 16642, 17320, 18040, 18807, 19626, 20503, 21445, 22460, 23558, 24750, 26050, 27474, 29042, 30776, 32708, 34874, 37320, 40107, 43314, 47046, 51445, 56712, 63137, 71153, 81443, 95143, 114300, 143006, 190811, 286362, 572899 };
    //浮点 0.00001
    public static int toInt(float x)
    {
        float n_num = Mathf.CeilToInt(x + 0.01f);
        float num = Mathf.CeilToInt(x);
        if (num < n_num)
        {
            if(x < 0)
                return Mathf.CeilToInt(x + 0.01f);
            else
                return Mathf.FloorToInt(x);
        }

        if (x < 0)
            return Mathf.CeilToInt(x);
        return Mathf.FloorToInt(x);
    }

    public static int toExactInt(double x)
    {
        double bj = Math.Ceiling(x + 0.001);
        double now = Math.Ceiling(x);
        if (now < bj)
            return (int)(Math.Floor(x + 0.001));
        else
            return (int)(Math.Floor(x));
    }

    public static int toInt(double x)
    {
        double bj = Math.Ceiling(x + 0.001);
        double now = Math.Ceiling(x);
        if (now < bj)
            return (int)(Math.Floor(x + 0.001));
        else
            return (int)(Math.Floor(x));
    }

    public static int checkr(double r)
    {
        int rr = toInt(r);
        rr = rr % 360;
        if (rr < 0)
            rr = rr + 360;
        return rr;
    }

    public static bool table_has<T>(List<T> t,T it)
    {
        for (int i = 0; i < t.Count; i++)
        {
            if(t[i].Equals(it))
                return true;
        }
        return false;
    }

    public static long msqrt(long x)
    {
        if (x == 1 || x == 0)
            return x;
        long low = 1,high = x;
 
        while (low < high)
        {
            long mid = (high + low)/ 2;
            if (mid  > x / mid)
                high = mid;
            else if (mid < x / mid)
                low = mid + 1;
            else
                return mid;
        }
        return low - 1;
    }

    public static bool check_distance(long x, long y, long xx, long yy, double dis)
    {
        long dx = x - xx;
        long dy = y - yy;
        return dx * dx + dy * dy > dis * dis;
    }

    public static int[] GetPenguinPatrolPoint(BattleAnimalMonster bp)
    {
        int min_x = bp.player.birth_x - bp.animal.eyeRange;
        int max_x = bp.player.birth_x + bp.animal.eyeRange;
        int min_y = bp.player.birth_y - bp.animal.eyeRange;
        int max_y = bp.player.birth_y + bp.animal.eyeRange;

        int num = 1;
        int x = BattleOperation.random(min_x, max_x);
        int y = BattleOperation.random(min_y, max_y);
        while (!BattleGrid.can_move(x, y) && !BattleOperation.check_distance(bp.animal.x, bp.animal.y, x, y, 15000))
        {
            if (num > 30)
                break;

            x = BattleOperation.random(min_x, max_x);
            y = BattleOperation.random(min_y, max_y);
            num = num + 1;
        }
        return new int[] { x, y };
    }

    public static int[] GetAccessPatrolPoint(BattleAnimalPlayer bp)
    {
        int ori_x = bp.animal.x;
        int ori_y = bp.animal.y;

        int min_x = BattleOperation.getMax(0, bp.player.birth_x - bp.animal.eyeRange);
        int max_x = BattleOperation.getMin(700000, bp.player.birth_x + bp.animal.eyeRange);

        int min_y = BattleOperation.getMax(0, bp.player.birth_y - bp.animal.eyeRange);
        int max_y = BattleOperation.getMin(700000, bp.player.birth_y + bp.animal.eyeRange);

        int x = BattleOperation.random(min_x, max_x);
        int y = BattleOperation.random(min_y, max_y);

        int num = 1;
        while (!BattleGrid.can_move(x, y) && !BattleOperation.check_distance(x, y, ori_x, ori_y, 15000))
        {
            if (num > 30)
                break;

            x = BattleOperation.random(min_x, max_x);
            y = BattleOperation.random(min_y, max_y);
            num = num + 1;
        }
        return new int[] { x, y };
    }


    public static long get_distance(long x, long y,long xx,long yy)
    {
        long dx = x - xx;
        long dy = y - yy;
        long d = dx * dx + dy * dy;
        return msqrt(d);
    }

    public static long get_distance2(long x, long y, long xx, long yy)
    {
        long dx = x - xx;
        long dy = y - yy;
        return dx * dx + dy * dy;
    }

    public static int[] add_distance(long x, long y, long r,long speed,int t)
    {
        int xx = (int)(x + mcos[r] * speed * t / 10000 / 1000);
        int yy = (int)(y + msin[r] * speed * t / 10000 / 1000);
        return new int[] { xx, yy };
    }

    public static int[] add_distance2(long x, long y, int r, double l)
    {
        int xx = toInt(x + mcos[r] * l / 10000);
        int yy = toInt(y + msin[r] * l / 10000);
        return new int[] {xx ,yy};
    }

    public static int get_r(long x, long y, long x1, long y1)
    {
        long dx = x1 - x;
        long dy = y1 - y;
        int rr = 0;
        
        if (dx > 0 && dy == 0)
            return 0;
        else if (dx < 0 && dy == 0)
            return 180;

        if (dx == 0 && dy < 0)
            rr = -90;
        else if (dx == 0 && dy > 0)
            rr = 90;
        else
        {
            long d = dy * 10000 / dx;
            int l = 0,r = 178;
            while (l != r)
            {
                int mid = (l + r) / 2;
                if (mtan[mid] > d)
                    r = mid;
                else
                    l = mid + 1;
            }
            rr = l - 90;
        }

        if (rr < 0)
            rr = rr + 180;
        if (dy < 0)
            rr = rr + 180;
        return rr;
    }

    //判断点P(x, y)与有向直线P1P2的关系.小于0表示点在直线左侧，等于0表示点在直线上，大于0表示点在直线右侧
    public static long EvaluatePointToLine(long x, long y, long x1, long y1, long x2, long y2)
    {
        long a = y2 - y1;
        long b = x1 - x2;
        long c = x2 * y1 - x1 * y2;
        return a * x + b * y + c;
    }

    //圆与线段碰撞检测
    //圆心p(x, y), 半径r, 线段两端点p1(x1, y1)和p2(x2, y2)
    public static bool IsCircleIntersectLineSeg(long x, long y,int r, long x1, long y1, long x2,long y2)
    {
        long vx1 = x - x1;
        long vy1 = y - y1;
        long vx2 = x2 - x1;
        long vy2 = y2 - y1;
        long l = msqrt(vx2 * vx2 + vy2 * vy2);
        vx2 = vx2 / l; //toInt(vx2 * 1.0 / l);
        vy2 = vy2 / l;// toInt(vy2 * 1.0 / l);
        long u = vx1 * vx2 + vy1 * vy2;
        long x0 = 0,y0 = 0;
        if (u <= 0)
        {
            x0 = x1;
            y0 = y1;
        }
        else if (u >= l)
        {
            x0 = x2;
            y0 = y2;
        }
        else
        {
            x0 = x1 + vx2 * u;
            y0 = y1 + vy2 * u;
        }
        return (x - x0) * (x - x0) + (y - y0) * (y - y0) <= r * r;
    }

    public static bool IsCircleIntersectFan(long x,long y,int r,long x1,long y1,int fx,long R,int rd)
    {
        if ((x - x1) * (x - x1) + (y - y1) * (y - y1) > (R + r) * (R + r))
            return false;

        if (rd >= 360)
            return true;

        long vx = mcos[fx] * R / 10000; //toInt(mcos[fx] * R * 1.0 / 10000);
        long vy = msin[fx] * R / 10000; //toInt(msin[fx] * R * 1.0 / 10000);

        //根据夹角 theta/2 计算出旋转矩阵，并将向量v乘该旋转矩阵得出扇形两边的端点p3,p4
        long h = rd / 2;
        long c = mcos[h];
        long s = msin[h];
        long x3 = x1 + (vx * c - vy * s) / 10000;   //toInt((vx * c - vy * s) / 10000.0);
        long y3 = y1 + (vx * s + vy * c) / 10000;   //toInt(.0);
        long x4 = x1 + (vx * c + vy * s) / 10000;   // toInt(.0);
        long y4 = y1 + (-vx * s + vy * c)/ 10000;  //toInt(.0);

        long d1 = EvaluatePointToLine(x, y, x1, y1, x3, y3);
        long d2 = EvaluatePointToLine(x, y, x4, y4, x1, y1);

        if (d1 >= 0 && d2 >= 0)
            return true;

        if (BattleOperation.IsCircleIntersectLineSeg(x, y, r, x1, y1, x3, y3))
            return true;

        if (BattleOperation.IsCircleIntersectLineSeg(x, y, r, x1, y1, x4, y4))
            return true;

        return false;
    }

    public static Vector3 LinearBezierCurve(Vector3 p0, Vector3 p1,float t)
    {
        return p0 + (p1 - p0) * t;
    }

    public static Vector3 QuadBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        var B = Vector3.zero;
   
        float t1 = (1 - t) * (1 - t);
        float t2 = t * (1 - t);
        float t3 = t * t;

        return p0 * t1 + p1 * 2 * t2 + p2 * t3;
    }

    public static Vector3 CubicBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,float t)
    {
        Vector3 B = Vector3.zero;
        float t1 = (1 - t) * (1 - t) * (1 - t);
        float t2 = (1 - t) * (1 - t) * t;
        float t3 = t * t * (1 - t);
        float t4 = t * t * t;
        B = p0 * t1 + p1 * 3 * t2 + p2 * 3 * t3 + p3 * t4;
        return B;
    }

    public static Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float ax, bx, cx;
        float ay, by, cy;
        float tSquared, tCubed;
        Vector3 result = new Vector3();

        cx = 3.0f * (p1.x - p2.x);
        bx = 3.0f * (p1.x - p2.x) - cx;
        ax = p3.x - p0.x - cx - bx;

        cy = 3.0f * (p1.y - p0.y);
        by = 3.0f * (p2.y - p1.y) - cy;
        ay = p3.y - p0.y - cy - by;

        tSquared = t * t;
        tCubed = tSquared * t;

        result.x = (ax * tCubed) + (bx * tSquared) + (cx * t) + p0.x;
        result.y = (ay * tCubed) + (by * tSquared) + (cy * t) + p0.y;
        result.z = p1.z;
        return result;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static List<double> rands = new List<double>();
    public static int rand_index = 1;

    public static void set_random_seed(int seed, int num)
    {
        rands.Clear();
        rand_index = 0;

        System.Random ra = new System.Random(seed);
        for (int i = 0; i < num; i++)
            rands.Add(ra.NextDouble());
    }

    public static int random(long a, long b)
    {
        long r = a + BattleOperation.toInt(BattleOperation.rands[BattleOperation.rand_index] * (b - a));
        if (a != b && r == b)
            r = b - 1;
        BattleOperation.rand_index = BattleOperation.rand_index + 1;
        if (BattleOperation.rand_index >= BattleOperation.rands.Count)
            BattleOperation.rand_index = 0;
        return (int)r;
    }

    public static int[] get_avilialbe_xy()
    {
        int x = BattleOperation.random(100000, 700000);
        int y = BattleOperation.random(100000, 700000);
        int num = 1;

        while (!BattleGrid.can_move(x, y))
        {
            if (num > 20)
                break;
            x = BattleOperation.random(100000, 700000);
            y = BattleOperation.random(100000, 700000);
            num = num + 1;
        }
        return new int[] {x,y};
    }

    public static int getMin(int left, int value)
    {
        if (left > value)
            return value;
        else
            return left;
    }

    public static int getMax(int right,int value)
    {
        if (value > right)
            return value;
        else
            return right;
    }

    public static int[] get_team_available_xy(int camp)
    {
        long bit_x = camp % 2;
        long bit_y = camp / 2;
        int x = BattleOperation.random(100000 + 500000 * bit_x, 150000 + 500000 * bit_x);
        int y = BattleOperation.random(100000 + 500000 * bit_y, 150000 + 500000 * bit_y);
        int num = 1;
        while (!BattleGrid.can_move(x, y))
        {
            if (num > 20)
                break;
            x = BattleOperation.random(100000 + 500000 * bit_x, 150000 + 500000 * bit_x);
            y = BattleOperation.random(100000 + 500000 * bit_y, 150000 + 500000 * bit_y);
            num = num + 1;
        }
        return new int[] { x, y };
    }

    public static int[] get_avilialbe_xy1()
    {
        int x = BattleOperation.random(100000, 700000);
        int y = BattleOperation.random(100000, 700000);

        int num = 1;
        while (!BattleGrid.can_move(x, y) || BattleGrid.get_cao(x, y) > 0)
        {
            if (num > 20)
                break;

            x = BattleOperation.random(100000, 700000);
            y = BattleOperation.random(100000, 700000);
            num = num + 1;
        }
        return new int[] { x, y };
    }

    public static int[] get_avilialbe_xy2(long x, long y, long dis)
    {
        long xx = (x + BattleOperation.random(-dis, dis));
        long yy = (y + BattleOperation.random(-dis, dis));

        int num = 1;
        while (!BattleGrid.can_move(xx, yy))
        {
            if (num > 20)
                break;
             
            xx = x + BattleOperation.random(-dis, dis);
            yy = y + BattleOperation.random(-dis, dis);
            num = num + 1;
        }
        return new int[] { (int)xx, (int)yy };
    }

    public static int calc_pre(BattleAnimal bp,int zhen)
    {
        int pre = (zhen * BattlePlayers.TICK - 500) / 15;
        int lq = 100 - bp.attr_value[93];
        if (lq <= 0)
            pre = 100;
        else
            pre = BattleOperation.toInt(pre * 100.0 / lq);
 
        if (pre > 100)
            pre = 100;
        else if (pre < 0)
            pre = 0;

        return pre;
    }

    public static float Lerp(float st, float dst, float r)
    {
        return st + (dst - st) * r;
    }

    public static int string2int(string s)
    {
        if (String.IsNullOrEmpty(s))
            return 0;

        int num = 0;
        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(s);
        for (int i = 0; i < byteArray.Length; i++)
            num = num + Convert.ToInt32(byteArray[i]);
        return num;
    }

    public static bool can_see(BattleAnimal bp, BattleAnimal bp1)
    {
        if (bp1 == null || bp1 == null)
            return true;

        if (bp1.cao > 0 && bp.cao != bp1.cao && bp.animal.camp != bp1.animal.camp)
            return false;

        if (bp1.attr.is_yinshen() && bp.animal.camp != bp1.animal.camp)
            return false;

        return true;
    }
}
