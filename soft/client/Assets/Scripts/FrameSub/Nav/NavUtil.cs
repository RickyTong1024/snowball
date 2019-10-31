using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class NavUtil
{
    public static List<NavTriangle> navList = null;  //保存 这张地图的 三角形数据
    public static List<List<NavPolygon>> meshPolys = new List<List<NavPolygon>>();
    public static void Clear()
    {
        if (navList != null)
            navList.Clear();
        navList = null;
        meshPolys.Clear();
    }
    public static List<NavPolygon> LoadNav(navMeshInfo navData)
    {
        List<Vector2i> currentVertexList = new List<Vector2i>();
        List<NavPolygon> polys = new List<NavPolygon>();
        for (int i = 0; i < 1600; ++i)
            meshPolys.Add(new List<NavPolygon>());

        for (int i = 0; i < navData.nodes.Count; i++)
        {
            NavPolygon np = new NavPolygon();
            navNode nd = navData.nodes[i];
            for (int j = 0; j < nd.vecs.Length; j++)
            {
                navVec2 nav = nd.vecs[j];
                Vector2i v2i = new Vector2i(nav.x, nav.y);
                np.vertexs.Add(v2i);  
            }

            long minx = nd.vecs[0].x;
            long maxx = nd.vecs[0].x;
            long miny = nd.vecs[0].y;
            long maxy = nd.vecs[0].y;
            for (int j = 1; j < nd.vecs.Length; ++j)
            {
                long x = nd.vecs[j].x;
                long y = nd.vecs[j].y;
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
            np.ID = i;
            for (long j = minx / navData.BLOCK_SIZE; j <= Mathf.CeilToInt(maxx*1.0f / 20000); ++j)
            {
                for (long k = miny / navData.BLOCK_SIZE; k <= Mathf.CeilToInt(maxy * 1.0f / 20000); ++k)
                {
                    int jj = (int)(j);
                    int kk = (int)(k);
                    int index = jj * navData.BLOCK_NUM + kk;
                    if (index < 0 || index >= navData.BLOCK_NUM2)
                        continue;
                    meshPolys[index].Add(np);
                }
            }
            polys.Add(np);  //将 数据 放到 polys中
        }
        return polys;
    }

    public static void InitNavTriInfoFromNavMesh(navMeshInfo navData)
    {
        List<NavPolygon> polys = LoadNav(navData);
        List<NavTriangle> triList = GetNavTriangleBaseInfo(polys,navData);
        navList = triList;
    }

    //A* 寻路 算法 实现  467163  185051
    public static List<int> GetPathFromNavMesh(int x1,int y1,int x2,int y2)
    {
        Vector2i startPoint = new Vector2i(x1, y1);
        Vector2i endPoint = new Vector2i(x2, y2);
        NavTriangle startCell = GetClosestTriangle(navList, startPoint);
        NavTriangle endCell = GetClosestTriangle(navList, endPoint);

        //没有路径
        if (startCell == null || endCell == null)
            return null;

        List<NavTriangle> path = new List<NavTriangle>();
        if (startCell == endCell)
            path.Add(startCell);
        else
            path = BuildPath(startCell, endCell);

        List<Vector2i> finalPath = GetTargetWayPoints(path, startPoint, endPoint);

        List<int> resultList = new List<int>();
        for(int i = 0;i < finalPath.Count;i++)
        {
            resultList.Add((int)finalPath[i].x);
            resultList.Add((int)finalPath[i].y);
        }
        return resultList;
    }

    public static Int64 CrossProduct(Vector2i p1, Vector2i p2)
    {
        return (p1.x * p2.y - p1.y * p2.x);
    }
    public static Int64 CrossProduct(Vector2i p1, Vector2i p2,Vector2i p3)
    {
        return (p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x);
    }

    public static bool IsEqualZero(Vector2i data)
    {
        return data == Vector2i.zero;
    }

    public static bool IsEqualZero(Int64 data)
    {
        return data == 0;
    }
    public static bool CheckCross(Vector2i sp1, Vector2i ep1, Vector2i sp2, Vector2i ep2)
    {
        if (Math.Max(sp1.x, ep1.x) < Math.Min(sp2.x, ep2.x))
        {
            return false;
        }
        if (Math.Min(sp1.x, ep1.x) > Math.Max(sp2.x, ep2.x))
        {
            return false;
        }
        if (Math.Max(sp1.y, ep1.y) < Math.Min(sp2.y, ep2.y))
        {
            return false;
        }
        if (Math.Min(sp1.y, ep1.y) > Math.Max(sp2.y, ep2.y))
        {
            return false;
        }

        Int64 temp1 = CrossProduct((sp1 - sp2), (ep2 - sp2)) * CrossProduct((ep2 - sp2), (ep1 - sp2));
        Int64 temp2 = CrossProduct((sp2 - sp1), (ep1 - sp1)) * CrossProduct((ep1 - sp1), (ep2 - sp1));

        if ((temp1 >= 0) && (temp2 >= 0))
        {
            return true; 
        }

        return false;
    }
    //将 多边形 全部转化成 三角形 并且 初始化 三角形的 ID 邻居节点  计算 到邻居点的 cost
    public static List<NavTriangle> GetNavTriangleBaseInfo(List<NavPolygon> polys,navMeshInfo navData)
    {
        List<NavTriangle> ntList = new List<NavTriangle>();
        int instanceID = 0;
        for (int i = 0; i < polys.Count; i++)
        {
            List<NavTriangle> tmpLT = polys[i].Splice2Trianles();
            polys[i].childList.Clear();
            for (int j = 0; j < tmpLT.Count; j++)
            {
                tmpLT[j].ID = instanceID++;  //赋值 唯一的 instanceID
                polys[i].childList.Add(tmpLT[j]);
                ntList.Add(tmpLT[j]);
            }
        }
        
        //将三角形的Center建立起来 
        for (int i = 0; i < ntList.Count; i++)
            ntList[i].Center = (((ntList[i].PointA + ntList[i].PointB) / 2 + (ntList[i].PointB + ntList[i].PointC) / 2) / 2 + (ntList[i].PointC + ntList[i].PointA) / 2) / 2;

        //依据三角形的边 建立字典 value 是 三角形的 List instanceID  key值是Line的Hash码
        Dictionary<int, List<int>> lineDics = new Dictionary<int, List<int>>();

        for (int i = 0; i < ntList.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Line2D line_key = ntList[i].GetSide(j);
                int key = line_key.ToString().GetHashCode();
                if (!lineDics.ContainsKey(key))
                {
                    lineDics.Add(key, new List<int>());
                    lineDics[key].Add(ntList[i].ID);
                }
                else
                {
                    if (!lineDics[key].Contains(ntList[i].ID))
                        lineDics[key].Add(ntList[i].ID);
                }
            }
        }

        foreach (int key in lineDics.Keys)
        {
            if (lineDics[key].Count <= 1)  //和其他的三角形没有公变
                continue;

            for (int i = 0; i < lineDics[key].Count; i++)
            {
                NavTriangle tri = ntList[lineDics[key][i]];
                for (int j = 0; j < lineDics[key].Count; j++)
                {
                    NavTriangle triNext = ntList[lineDics[key][j]];

                    if (tri.ID == triNext.ID)
                        continue;

                    int result = tri.isNeighbor(triNext);   // 9次

                    if (result != -1)
                        tri.SetNeighbor(result, triNext.ID);
                }
            }
        }

        for (int i = 0; i < ntList.Count; i++)  //设置 和 邻居三角形的 cost
        {
            for (int j = 0; j < 3; j++)
            {
                int neighborID = ntList[i].GetNeighbor(j);
                if (neighborID <= 0)
                    continue;
                //计算 gCost
                Int64 result = Sqrt((ntList[i].Center.x - ntList[neighborID].Center.x) * (ntList[i].Center.x - ntList[neighborID].Center.x)
            + (ntList[i].Center.y - ntList[neighborID].Center.y) * (ntList[i].Center.y - ntList[neighborID].Center.y));
                ntList[i].SetNeighborGCost(j, result);
            }
        }

        return ntList;
    }

    public static NavTriangle GetHashMinValue(Dictionary<int,NavTriangle> set)
    {
        if (set.Count <= 0)
            return null;
        NavTriangle nt = null;
        Dictionary<int, NavTriangle>.KeyCollection.Enumerator it = set.Keys.GetEnumerator();
        while (it.MoveNext())
        {
            if (nt == null)
                nt = set[it.Current];
            else
            {
                Int64 targetValue = nt.GetGValue() + nt.GetHCost();
                Int64 currValue = set[it.Current].GetGValue() + set[it.Current].GetHCost();
                if (currValue < targetValue)
                    nt = set[it.Current];
            }
        }
        return nt;
    }


    static List<int> modlist_ = new List<int>();
    static List<NavTriangle> pathList = new List<NavTriangle>();
    static Heap openHeap_ = new Heap();
    static int maxSearchStep = 30;
    static int currentStep = 0;
    static int lastid_ = -1;
    
    public static List<NavTriangle> BuildPath(NavTriangle startCell, NavTriangle endCell)
    {
        modlist_.Clear();
        openHeap_.Clear();
        startCell.InitAStarBefore();
        startCell.astar_state = 1;

        modlist_.Add(startCell.ID);
        openHeap_.Add(startCell);

        NavTriangle currentCell = null;
        currentStep = 0;
        lastid_ = -1;
        while (openHeap_.current > 0)
        {
            if (currentStep >= maxSearchStep)
            {
                break;
            }
            // 1. 把当前节点从开放列表删除, 加入到封闭列表
            currentCell = openHeap_.RemoveHeap(0); //GetHashMinValue(openDics);//  //; 
            currentCell.astar_state = 2;
            lastid_ = currentCell.ID;

            if (currentCell == endCell) //已经找到目的地
                break;

            for (int i = 0; i < 3; i++)
            {
                int neighborID = currentCell.GetNeighbor(i);  //邻居节点的三角形实例ID
                if (neighborID < 0)
                {
                    continue;
                }
                else
                {
                    NavTriangle neighborTri = navList[neighborID];
                    if (neighborTri.astar_state == 2)  //邻居已经在CLOSE表中了，那么不需要考虑了~
                    {
                        continue;
                    }
                    else if (neighborTri.astar_state == 0)  //邻居不在OPEN表中，那么将邻居加入OPEN 并将次邻居的父节点赋值为当前节点
                    {
                        neighborTri.InitAStarBefore();
                        neighborTri.astar_state = 1;
                        neighborTri.parent = currentCell;
                        neighborTri.CalcHeuristic(endCell);// 计算启发值h
                        // 计算三角形花费g
                        neighborTri.SetGValue(currentCell.GetGValue() + currentCell.GetNeighborGCost(i));
                        modlist_.Add(neighborTri.ID);
                        openHeap_.Add(neighborTri);
                    }
                    else
                    {
                        //邻居在OPEN中，那么需要看看此邻居点的G值与当前点的(G值 + 当前点到邻居点的距
                        Int64 updateValue = currentCell.GetGValue() + currentCell.GetNeighborGCost(i);
                        if (neighborTri.GetGValue() > updateValue)
                        {
                            neighborTri.parent = currentCell;
                            neighborTri.SetGValue(updateValue);
                            openHeap_.SortAll();
                        }//if
                    }//else
                }
            }
            currentStep += 1;
        }//while

        for (int i = 0; i < modlist_.Count; ++i)
        {
            navList[modlist_[i]].astar_state = 0;
        }

        var pathList = new List<NavTriangle>();
        if (lastid_ != -1)
        {
            NavTriangle path = navList[lastid_];
            pathList.Add(path);
            while (path.parent != null)
            {
                pathList.Add(path.parent);
                path = path.parent;
            }
            pathList.Reverse();
        }
        return pathList;
    }

    static List<Vector2i> finalPath = new List<Vector2i>();
    static WayPoint way = new WayPoint();
    public static List<Vector2i> GetTargetWayPoints(List<NavTriangle> list,Vector2i starPoint, Vector2i endPoint)
    {
        //没有一个路径 不用检查
        if (list ==null || list.Count < 1)
            return null;

        finalPath.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            NavTriangle tri = list[i];
            if (i != list.Count - 1)
            {
                NavTriangle nextTri = list[i + 1];
                tri.SetOutLine(nextTri.ID);
            }
        }

        finalPath.Add(starPoint);
        //起点与终点在同一三角形中
        if (list.Count == 1)
        {
            finalPath.Add(endPoint);
            return finalPath;
        }

        way.m_cPoint = starPoint;
        way.m_cTriangle = list[0];

        if (!list[list.Count - 1].IsPointIn(endPoint))
            endPoint = list[list.Count - 1].Center;

        while (way.GetPoint() != endPoint)
        {
            way = GetFurthestWayPoint(way, list, endPoint);
            if (way == null)
                return null;
            finalPath.Add(way.GetPoint());
            break;  //获得第一个拐点就行了
        }

        return finalPath;
    }
    static Line2D lastLineA = new Line2D();
    static Line2D lastLineB = new Line2D();
    static Vector2i testPntA;
    static Vector2i testPntB;
    public static WayPoint GetFurthestWayPoint(WayPoint way, List<NavTriangle> list, Vector2i endPoint)
    {
        WayPoint nextWay = null;
        Vector2i currPnt = way.GetPoint();
        NavTriangle currTri = way.GetTriangle();

        NavTriangle lastTriA = currTri;
        NavTriangle lastTriB = currTri;
        int startIndex = list.IndexOf(currTri);     //开始路点所在的网格索引

        Line2D outSide = currTri.GetSide(currTri.GetOutLine());//路径线在网格中的穿出边?
        Vector2i lastPntA = outSide.startPoint;
        Vector2i lastPntB = outSide.endPoint;

        lastLineA.startPoint = currPnt;
        lastLineA.endPoint = lastPntA;

        lastLineB.startPoint = currPnt;
        lastLineB.endPoint = lastPntB;


        for (int i = startIndex + 1; i < list.Count; i++)
        {
            currTri = list[i];
            outSide = currTri.GetSide(currTri.GetOutLine());  //
            if (i == list.Count - 1)
            {
                testPntA = endPoint;
                testPntB = endPoint;
            }
            else
            {
                testPntA = outSide.startPoint;
                testPntB = outSide.endPoint;
            }

            if (lastPntA != testPntA)
            {
                if (lastLineB.ClassifyPoint(testPntA) == PointSide.RIGHT_SIDE)
                {
                    nextWay = new WayPoint(lastPntB, lastTriB);
                    return nextWay;
                }
                else if (lastLineA.ClassifyPoint(testPntA) != PointSide.LEFT_SIDE)
                {
                    lastPntA = testPntA;
                    lastTriA = currTri;
                    //重设直线
                    lastLineA.endPoint = lastPntA;
                }
            }

            if (lastPntB != testPntB)
            {
                if (lastLineA.ClassifyPoint(testPntB) == PointSide.LEFT_SIDE)
                {
                    nextWay = new WayPoint(lastPntA, lastTriA);
                    return nextWay;
                }
                else if (lastLineB.ClassifyPoint(testPntB) != PointSide.RIGHT_SIDE)
                {
                    lastPntB = testPntB;
                    lastTriB = currTri;
                    //重设直线
                    lastLineB = new Line2D(lastLineB.startPoint, lastPntB);
                }
            }
        }
        nextWay = new WayPoint(endPoint, list[list.Count - 1]);
        return nextWay;
    }

    //获取 当前点所在的 三角形中
    public static NavTriangle GetClosestTriangle(List<NavTriangle> triList, Vector2i point)
    {
        int xx = (int)(point.x / 20000);
        int yy = (int)(point.y / 20000);
        xx = xx * 40 + yy;

        if (xx < 0 || xx >= 1600)
            return null;

        var pmap = meshPolys[xx];
        for (int i = 0; i < pmap.Count; i++)
        {
            for (int j = 0; j < pmap[i].childList.Count; j++)
            {
                if (pmap[i].childList[j].IsPointIn(point))
                {
                    return pmap[i].childList[j];
                }
            }
        }
        return null;
    }

    public static Int64 Sqrt(Int64 x)
    {
        if (x == 1 || x == 0)
            return x;
        Int64 low = 1;
        Int64 high = x;
        while (low < high)
        {
            Int64 mid = (high + low) / 2;
            if (mid * mid > x)
                high = mid;
            else if (mid * mid < x)
                low = mid + 1;
            else
                return mid;
        }
        return low - 1;
    }
    public static Vector3 WorldToScreenPoint(Vector3 position)
    {
        if (UICamera.mainCamera == null)
            return position;

        return UICamera.currentCamera.WorldToScreenPoint(position);
    }

    public static Vector3 worldpos_to_nguipos(Vector3 pos,GameObject parentObj)
    {
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 0f;
        Vector3 pos2 = UICamera.currentCamera.ScreenToWorldPoint(pos);
        return pos2;
    }

    public static Vector3 worldpos_to_localpos(Vector3 pos, GameObject parentObj)
    {
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 0f;
        Vector3 pos2 = UICamera.currentCamera.ScreenToWorldPoint(pos);
        Vector3 now_pos = parentObj.transform.parent.InverseTransformPoint(pos2);
        return now_pos;
    }

    public static AudioSource PlaySound(AudioSource source, string name)
    {
        AudioClip _clip = Resources.Load<AudioClip>("ui/uisound/" + name);
        if (_clip != null)
        { 
            if (source.isPlaying)
                source.Stop();
            source.clip = _clip;
            source.Play();
        }
        return source;
    }

    public static void AdjustScrollview(GameObject obj,float left_limit,float right_limit,float offset_x,int sign)
    {
        UIScrollView sw = obj.GetComponent<UIScrollView>();
        Vector2 pv = NGUIMath.GetPivotOffset(sw.contentPivot);
        float x = pv.x;
        float y = 1f - pv.y;

        UIPanel mPanel = obj.GetComponent<UIPanel>();
        Bounds b = sw.bounds;

        Vector4 clip = mPanel.finalClipRegion;

        float hx = clip.z * 0.5f;
        float hy = clip.w * 0.5f;
        float left = b.min.x + hx;
        float right = b.max.x - hx;
        float bottom = b.min.y + hy;
        float top = b.max.y - hy;

        if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
        {
            left -= mPanel.clipSoftness.x;
            right += mPanel.clipSoftness.x;
            bottom -= mPanel.clipSoftness.y;
            top += mPanel.clipSoftness.y;
        }

        // Calculate the offset based on the scroll value
        float ox = Mathf.Lerp(left, right, x);
        float oy = Mathf.Lerp(top, bottom, y);


        Vector3 pos = obj.transform.localPosition;

        if (sw.canMoveHorizontally) pos.x += clip.x - ox;
        if (sw.canMoveVertically) pos.y += clip.y - oy;
        if (sign == 0)
            offset_x = left_limit - offset_x;
        else if (sign == 1)
            offset_x = right_limit - offset_x;

        pos = pos + new Vector3(offset_x,0,0);

        SpringPanel.Begin(mPanel.gameObject, pos, 8f);
    }

    public static bool IsScrollviewEnd(GameObject obj)
    {
        UIScrollView sw = obj.GetComponent<UIScrollView>();
        UIPanel mPanel = obj.GetComponent<UIPanel>();

        if (sw == null || mPanel == null)
            return false;
        
        Bounds b = sw.bounds;
        Vector2 bmin = b.min;
        Vector2 bmax = b.max;
        if (sw.movement == 0)
        {
            Vector4 clip = mPanel.finalClipRegion;
            int intViewSize = Mathf.RoundToInt(clip.z);
            if ((intViewSize & 1) != 0) intViewSize -= 1;
            float halfViewSize = intViewSize * 0.5f;
            halfViewSize = Mathf.Round(halfViewSize);

            if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
                halfViewSize -= mPanel.clipSoftness.x;

            float contentSize = bmax.x - bmin.x;
            float viewSize = halfViewSize * 2f;
            float contentMin = bmin.x;
            float contentMax = bmax.x;
            float viewMin = clip.x - halfViewSize;
            float viewMax = clip.x + halfViewSize;

            contentMin = viewMin - contentMin;
            contentMax = contentMax - viewMax;
            if (contentMax <= 0)
                return true;
            else
                return false;
        }
        else
        {
            Vector4 clip = mPanel.finalClipRegion;
            int intViewSize = Mathf.RoundToInt(clip.w);
            if ((intViewSize & 1) != 0) intViewSize -= 1;
            float halfViewSize = intViewSize * 0.5f;
            halfViewSize = Mathf.Round(halfViewSize);

            if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
                halfViewSize -= mPanel.clipSoftness.y;

            float contentSize = bmax.y - bmin.y;
            float viewSize = halfViewSize * 2f;
            float contentMin = bmin.y;
            float contentMax = bmax.y;
            float viewMin = clip.y - halfViewSize;
            float viewMax = clip.y + halfViewSize;

            contentMin = viewMin - contentMin;
            contentMax = contentMax - viewMax;
            if (contentMin <= 0)
                return true;
            else
                return false;
        }
    }
}

