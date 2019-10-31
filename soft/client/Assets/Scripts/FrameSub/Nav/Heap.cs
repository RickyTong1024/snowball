using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Heap
{
    public NavTriangle[] list = null;
    public int maxLength;
    public int current;
    public Heap()
    {
        maxLength = 1000;
        current = 0;
        list = new NavTriangle[maxLength];
    }
    public void Clear()
    {
        current = 0;
    }
    private void Swap(int firstIndex, int secIndex)
    {
        NavTriangle temp;
        temp = list[firstIndex];
        list[firstIndex] = list[secIndex];
        list[secIndex] = temp;
    }

    public void Add(NavTriangle nt) //调整节点
    {
        if (current >= maxLength)
            return;

        list[current] = nt;
        AdjustTop(current);
        current++;
    }

    public void AdjustTop(int pos)
    {
        int parent_pos = (pos - 1) / 2;
        while (true)
        {
            //和父节点比较 如果比父节点小交换数值 如果比父节点大直接退出
            //当父节点是根节点的时候退出比较
            if (list[parent_pos].GetCost() > list[pos].GetCost())
                Swap(parent_pos, pos);
            else
                break;

            pos = parent_pos;
            parent_pos = (pos - 1) / 2;

            if (pos == 0)
                break;
        }
    }

    public void AdjustDown(int index)
    {
        int left_child = (index * 2) + 1;
        int right_chid = (index + 1) * 2;
        while (index < current)
        {
            if (left_child >= current)
                break;
            if (right_chid >= current)
            {
                if (list[index].GetCost() > list[left_child].GetCost())
                {
                    Swap(index, left_child);
                    index = left_child;
                }
                else
                    break;
            }
            else
            {
                if (list[left_child].GetCost() > list[right_chid].GetCost())
                {
                    if (list[index].GetCost() > list[right_chid].GetCost())
                    {
                        Swap(index, right_chid);
                        index = right_chid;
                    }
                    else
                        break;
                }
                else
                {
                    if (list[index].GetCost() > list[left_child].GetCost())
                    {
                        Swap(index, left_child);
                        index = left_child;
                    }
                    else
                        break;
                }
            }
            left_child = (index * 2) + 1;
            right_chid = (index + 1) * 2;
        }
    }

    public NavTriangle RemoveHeap(int index)  //删除 堆中的某个Index上的数据
    {
        NavTriangle dt = list[index];
        int current_end_pos = current - 1;
        current--;
        Swap(index, current_end_pos);
        AdjustDown(index);
        return dt;
    }

    public int GetIndex(NavTriangle t)
    {
        for (int i = 0; i < current; i++)
            if (t == list[i])
                return i;
        return -1;
    }
    public int SortCount = 0;
    public void SortAll()
    {
        for (int i = (current - 1) / 2; i >= 0; i--)
            AdjustDown(i);
    }
}
