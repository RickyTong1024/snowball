using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.IO;

public class navVec2
{
    public long x;
    public long y;

    public navVec2()
    {
        x = 0;
        y = 0;
    }
    public navVec2(long xx, long yy)
    {
        x = xx;
        y = yy;
    }
}

public class navNode
{
    public int nodeID;
    public navVec2[] vecs;
    public long minx, miny, maxx, maxy;
}

public class navMeshInfo
{
    public long BLOCK_SIZE = 20000;
    public int BLOCK_NUM = 40;
    public int BLOCK_NUM2 = 1600;
    public List<navVec2> vecs = new List<navVec2>();//顶点
    public List<navNode> nodes = new List<navNode>();//节点
    private List<List<int>> pmap_;
    FileStream fs;
    StreamWriter sw;
    public void load_nav(string name)
    {
        pmap_ = new List<List<int>>();
        for (int i = 0; i < BLOCK_NUM2; ++i)
        {
            pmap_.Add(new List<int>());
        }
        TextAsset _txt_data = AppFacade._instance.ResourceManager.LoadNav(name);
        if (_txt_data == null)
        {
            Debug.Log("err navmesh__" + name);
            return;
        }

        List<navVec2> listVec = new List<navVec2>();
        JsonData jd = JsonMapper.ToObject(_txt_data.text);
        for (int i = 0; i < jd["v"].Count; ++i)
        {
            navVec2 v3 = new navVec2();
            v3.x = int.Parse(jd["v"][i][0].ToString());
            v3.y = int.Parse(jd["v"][i][1].ToString());
            vecs.Add(v3);
        }

        for (int i = 0; i < jd["p"].Count; ++i)
        {
            navNode node = new navNode();
            node.nodeID = i;
            List<navVec2> vs = new List<navVec2>();
            for (int j = 0; j < jd["p"][i].Count; ++j)
            {
                int poly = int.Parse(jd["p"][i][j].ToString());
                vs.Add(vecs[poly]);
            }
            node.vecs = vs.ToArray();
            CW(node.vecs);  //转成 顺时针方向
            long minx = node.vecs[0].x;
            long maxx = node.vecs[0].x;
            long miny = node.vecs[0].y;
            long maxy = node.vecs[0].y;
            for (int j = 1; j < node.vecs.Length; ++j)
            {
                long x = node.vecs[j].x;
                long y = node.vecs[j].y;
                if (x < minx)
                {
                    minx = x;
                }
                else if (x > maxx)
                {
                    maxx = x;
                }
                if (y < miny)
                {
                    miny = y;
                }
                else if (y > maxy)
                {
                    maxy = y;
                }
            }
            node.minx = minx;
            node.maxx = maxx;
            node.miny = miny;
            node.maxy = maxy;
            nodes.Add(node);

            bool flag = false;
            for (long j = minx / BLOCK_SIZE; j <= maxx / BLOCK_SIZE; ++j)
            {
                for (long k = miny / BLOCK_SIZE; k <= maxy / BLOCK_SIZE; ++k)
                {
                    int jj = (int)(j);
                    int kk = (int)(k);
                    int index = jj * BLOCK_NUM + kk;
                    if (index < 0 || index >= BLOCK_NUM2)
                    {
                        continue;
                    }
                    navNode cp = new navNode();
                    List<navVec2> poly1 = new List<navVec2>();
                    poly1.Add(new navVec2(jj * BLOCK_SIZE + BLOCK_SIZE - 1, kk * BLOCK_SIZE));
                    poly1.Add(new navVec2(jj * BLOCK_SIZE + BLOCK_SIZE - 1, kk * BLOCK_SIZE + BLOCK_SIZE - 1));
                    poly1.Add(new navVec2(jj * BLOCK_SIZE, kk * BLOCK_SIZE + BLOCK_SIZE - 1));                    
                    poly1.Add(new navVec2(jj * BLOCK_SIZE, kk * BLOCK_SIZE));
                    cp.vecs = poly1.ToArray();
                    cp.minx = jj * BLOCK_SIZE;
                    cp.maxx = jj * BLOCK_SIZE + BLOCK_SIZE - 1;
                    cp.miny = kk * BLOCK_SIZE;
                    cp.maxy = kk * BLOCK_SIZE + BLOCK_SIZE - 1;


                    if (!is_poly_in_poly(cp, node))
                    {
                        continue;
                    }
                    flag = true;
                    pmap_[index].Add(i);
                }
            }
            if (!flag)
            {
                Debug.Log("poly error");
            }
        }
    }

    public void clear()
    {
        vecs.Clear();
        nodes.Clear();
        pmap_.Clear();
    }

    bool is_poly_in_poly(navNode cp, navNode nnd) //navVec2[] poly2)
    {
        navVec2[] poly2 = nnd.vecs;
        navVec2[] poly1 = cp.vecs;

        for (int i = 0; i < poly1.Length; ++i)
        {
            if (inPoly(poly1[i], nnd))
            {
                return true;
            }
        }
        for (int i = 0; i < poly2.Length; ++i)
        {
            if (inPoly(poly2[i], cp))
            {
                return true;
            }
        }
        for (int i = 0; i < poly1.Length; ++i)
        {
            navVec2 p1 = poly1[i];
            navVec2 p2;
            if (i == poly1.Length - 1)
            {
                p2 = poly1[0];
            }
            else
            {
                p2 = poly1[i + 1];
            }
            for (int j = 0; j < poly2.Length; ++j)
            {
                navVec2 p3 = poly2[j];
                navVec2 p4;
                if (j == poly2.Length - 1)
                {
                    p4 = poly2[0];
                }
                else
                {
                    p4 = poly2[j + 1];
                }
                if (GetIntersection(p1, p2, p3, p4))
                {
                    return true;
                }
            }
        }
        return false;
    }

    long cross(navVec2 p0, navVec2 p1, navVec2 p2)
    {
        return (p1.x - p0.x) * (p2.y - p0.y) - (p2.x - p0.x) * (p1.y - p0.y);
    }

    private bool GetIntersection(navVec2 a, navVec2 b, navVec2 c, navVec2 d)
    {
        if (Mathf.Abs(b.x - a.y) + Mathf.Abs(b.x - a.y) + Mathf.Abs(d.y - c.y) + Mathf.Abs(d.x - c.x) == 0)
        {
            if (c.x - a.x == 0)
            {
                return true;
            }
            return false;
        }

        if (Mathf.Abs(b.y - a.y) + Mathf.Abs(b.x - a.x) == 0)
        {
            return true;
        }
        if (Mathf.Abs(d.y - c.y) + Mathf.Abs(d.x - c.x) == 0)
        {
            return true;
        }

        if ((b.y - a.y) * (c.x - d.x) - (b.x - a.x) * (c.y - d.y) == 0)
        {
            return false;
        }

        navVec2 intersection = new navVec2();
        intersection.x = ((b.x - a.x) * (c.x - d.x) * (c.y - a.y) - c.x * (b.x - a.x) * (c.y - d.y) + a.x * (b.y - a.y) * (c.x - d.x)) / ((b.y - a.y) * (c.x - d.x) - (b.x - a.x) * (c.y - d.y));
        intersection.y = ((b.y - a.y) * (c.y - d.y) * (c.x - a.x) - c.y * (b.y - a.y) * (c.x - d.x) + a.y * (b.x - a.x) * (c.y - d.y)) / ((b.x - a.x) * (c.y - d.y) - (b.y - a.y) * (c.x - d.x));

        if ((intersection.x - a.x) * (intersection.x - b.x) <= 0 && (intersection.x - c.x) * (intersection.x - d.x) <= 0 && (intersection.y - a.y) * (intersection.y - b.y) <= 0 && (intersection.y - c.y) * (intersection.y - d.y) <= 0)
        {
            return true;
        }
        return false;
    }

    navVec2 n = new navVec2();
    public bool can_move(int x, int y)
    {
        int xx = x / (int)BLOCK_SIZE;
        int yy = y / (int)BLOCK_SIZE;
        xx = xx * BLOCK_NUM + yy;
        if (xx < 0 || xx >= BLOCK_NUM2)
        {
            return false;
        }
        var pmap = pmap_[xx];
        n.x = x;
        n.y = y;
        for (int p = 0; p < pmap.Count; p++)
        {
            if (inPoly(n, nodes[pmap[p]]))
            {
                return true;
            }
        }
        return false;
    }
    bool inPoly(navVec2 p, navNode nnd)
    {
        navVec2[] poly = nnd.vecs;
        if (poly.Length < 3) return false;

        if ((p.x < nnd.minx || p.x > nnd.maxx) || (p.y < nnd.miny || p.y > nnd.maxy))
            return false;

        for (int m = 0; m < poly.Length; m++)
        {
            int firstPoint = m % poly.Length;
            int secPoint = (m + 1) % poly.Length;
            if (cross(poly[firstPoint], p, poly[secPoint]) > 0)
                return false;
        }

        return true;
    }

    private void CW(navVec2[] nvs)  //将四边形是顺时针方向
    {
        if (IsCW(nvs) == false) //如果为逆时针顺序， 反转为顺时针
            Reverse(nvs);
    }

    private void Reverse(navVec2[] nvs)
    {
        for (int i = 0; i < nvs.Length / 2; i++)
        {
            navVec2 tmp = nvs[i];
            nvs[i] = nvs[nvs.Length - 1 - i];
            nvs[nvs.Length - 1 - i] = tmp;
        }
    }

    private bool IsCW(navVec2[] nvs) //clockwise 时钟的顺序 即顺时针
    {
        if (nvs == null || nvs.Length < 0)
            return false;

        navVec2 topPt = nvs[0];
        int topPtId = 0;
        for (int i = 1; i < nvs.Length; i++)
        {
            if (topPt.y > nvs[i].y)
            {
                topPt = nvs[i];
                topPtId = i;
            }
            else if (topPt.y == nvs[i].y)
            {
                if (topPt.x > nvs[i].x)
                {
                    topPt = nvs[i];
                    topPtId = i;
                }
            }
        }

        int lastId = topPtId - 1 >= 0 ? topPtId - 1 : nvs.Length - 1;
        int nextId = topPtId + 1 >= nvs.Length ? 0 : topPtId + 1;
        navVec2 last = nvs[lastId];
        navVec2 next = nvs[nextId];
        float r = multiply(last, next, topPt);
        if (r < 0)
            return true;

        return false;
    }
    private float multiply(navVec2 sp, navVec2 ep, navVec2 op)
    {
        return ((sp.x - op.x) * (ep.y - op.y) - (ep.x - op.x) * (sp.y - op.y));
    }
}
