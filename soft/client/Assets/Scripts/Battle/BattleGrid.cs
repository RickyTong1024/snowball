using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum grid_type
{
    et_player = 0,
    et_effect = 1,
    et_item = 2
}

public class BattleGrid
{
    public const int GRID_SIZE = 5000;
    public const int BLOCK_SIZE = 20000;
    public static int width_ = 0;
    public static int height_ = 0;
    public static int[,] obss_ = null;
    public static Dictionary<string,int> locs_str = null;
    public static Dictionary<int, int> locs_int = null;

    public static Dictionary<grid_type, HashSet<string>>[] objs_ = null;
    public static void Init(string name)
    {
        var obstancle = new obstancle();
        obstancle.load_obs(name);

        obss_ = new int[obstancle.get_x(),obstancle.get_y()];
        for (int i = 0; i < obstancle.get_x(); i++)
        {
            for (int j = 0; j < obstancle.get_y(); j++)
            {
                int a = obstancle.get(i, j);
                if (a >= 2)
                    obss_[i,j] = a - 1;
                else
                    obss_[i,j] = 0;
            }
        }
        
        width_  = obstancle.get_x() * GRID_SIZE  / BLOCK_SIZE;
        height_ = obstancle.get_y() * GRID_SIZE  / BLOCK_SIZE;

        obstancle.clear();
        AppFacade._instance.MapManager.load_nmi(name);
        //Util.CallMethod("BattlePlayerAI", "Init");

        //加载数据
        locs_str = new Dictionary<string, int>();
        locs_int = new Dictionary<int, int>();
        objs_ = new Dictionary<grid_type, HashSet<string>>[width_ * height_ + 1];

        for (int i = 1; i <= width_ * height_; i++)
            objs_[i] = new Dictionary<grid_type, HashSet<string>>();
    }

    public static void Fini()
    {
        //Util.CallMethod("BattlePlayerAI", "Fini");
        LuaHelper.GetMapManager().clear_nmi();
    }

    public static bool add(grid_type t, string guid, int x, int y)
    {
        int xx = x / BLOCK_SIZE;
        int yy = y / BLOCK_SIZE;
        if (xx < 0 || xx >= width_ || yy < 0 || yy >= height_)
            return false;
        int index = yy * width_ + xx + 1;

        if (locs_str.ContainsKey(guid) && index == locs_str[guid])
            return false;

        BattleGrid.del(t, guid);
        if (!objs_[index].ContainsKey(t))
            objs_[index].Add(t,new HashSet<string>());

        if(!objs_[index][t].Contains(guid))
            objs_[index][t].Add(guid);

        if (locs_str.ContainsKey(guid))
            locs_str[guid] = index;
        else
            locs_str.Add(guid, index);

        return true;
    }

    public static bool add(grid_type t, int guid, int x, int y)
    {
        int xx = x  / BLOCK_SIZE;
        int yy = y  / BLOCK_SIZE;
        if (xx < 0 || xx >= width_ || yy < 0 || yy >= height_)
            return false;
        int index = yy * width_ + xx + 1;

        if (locs_int.ContainsKey(guid) && index == locs_int[guid])
            return false;

        BattleGrid.del(t, guid);
        if (!objs_[index].ContainsKey(t))
            objs_[index].Add(t,new HashSet<string>());

        if (!objs_[index][t].Contains(guid.ToString()))
        {
            if (objs_[index][t].Contains(guid.ToString()))
                UnityEngine.Debug.Log("同一个数据!"+guid);
            objs_[index][t].Add(guid.ToString());
        }
            

        if (locs_int.ContainsKey(guid))
            locs_int[guid] = index;
        else
            locs_int.Add(guid, index);
        return true;
    }

    public static void del(grid_type t, string guid)
    {
        if (!locs_str.ContainsKey(guid))
        {
            //StringBuilder sb = new StringBuilder();

            //foreach (var item in locs_str)
            //    sb.Append(item + ",");

            //UnityEngine.Debug.Log("del guid null" + guid + "," + t + sb.ToString());
            return;
        }

        bool sign = false;
        if (objs_[locs_str[guid]].ContainsKey(t))
        {
            objs_[locs_str[guid]][t].Remove(guid);
            sign = true;
        }
        locs_str.Remove(guid);
        if (!sign)
            UnityEngine.Debug.Log("dont delete success!");
    }

    public static void del(grid_type t, int guid)
    {
        if (!locs_int.ContainsKey(guid))
            return;
        if (objs_[locs_int[guid]].ContainsKey(t))
            objs_[locs_int[guid]][t].Remove(guid.ToString());
        locs_int.Remove(guid);
    }

    public static void get(grid_type t, int x, int y, out string[] guids, int sz = 1)
    {
        if (sz != 1)
            sz = sz / BLOCK_SIZE + 1;

        int xx = x / BLOCK_SIZE;
        int yy = y / BLOCK_SIZE;
        List<string> ct = new List<string>();

        for (int i = xx - sz; i <= xx + sz; i++)
        {
            for (int j = yy - sz; j <= yy + sz; j++)
            {
                if (i >= 0 && i < width_ && j >= 0 && j < height_)
                {
                    int index = j * width_ + i + 1;
                    if (objs_[index].ContainsKey(t))
                    {
                        foreach (var item in objs_[index][t])
                            ct.Add(item);
                    }
                }
            }
        }
        //if (t == grid_type.et_player)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(BattlePlayers.zhen +"x"+x+",y"+y+ "\r\n");
        //    for (int i = 0; i < ct.Count; i++)
        //        sb.Append(ct[i] + ",");
        //    UnityEngine.Debug.Log(sb.ToString());
        //}
        ct.Sort();
        guids = ct.ToArray();
    }

    public static int get_cao(int x, int y)
    {
        int xx = x / GRID_SIZE;
        int yy = y / GRID_SIZE;

        if (xx >= obss_.GetLength(0) || xx < 0 || yy >= obss_.GetLength(1) || yy < 0)
            return 0;
        return obss_[xx, yy];
    }

    public static bool can_move_type(int t)
    {
        if (t == -1 || t == 1)
            return false;

        return true;
    }

    public static bool can_move(long xx, long yy)
    {
        return AppFacade._instance.MapManager.can_move((int)xx, (int)yy);
    }

    public static bool can_move1(int xx, int yy)
    {
        return AppFacade._instance.MapManager.can_move1(xx, yy);
    }

    public static int[] get_move_point(int x1, int y1, int xx, int yy)
    {
        if (BattleGrid.can_move(xx, yy))
            return new int[]{ xx, yy };

        int x2 = xx;
        int y2 = yy;
        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);

        while(dx > 1000 || dy > 1000)
        {
            int mx = (x1 + x2) / 2;
            int my = (y1 + y2) / 2;
            if (BattleGrid.can_move(mx, my))
            {
                x1 = mx;
                y1 = my;
            }
            else
            {
                x2 = mx;
                y2 = my;
            }
            dx = Math.Abs(x2 - x1);
            dy = Math.Abs(y2 - y1);
        }
        return new int[] { x1, y1 };
    }

    public static bool can_effect_move(int xx,int yy)
    {
        return LuaHelper.GetMapManager().can_move1(xx, yy);
    }
}

