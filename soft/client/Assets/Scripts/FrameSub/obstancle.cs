using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class obstancle
{
	private List<List<int>> m_records = new List<List<int>>();
	private string m_name;
    private int m_xnum;
    private int m_ynum;

	public void load_obs(string name)
	{
		m_records.Clear ();
		m_name = name;
        TextAsset _txt_data = AppFacade._instance.ResourceManager.LoadObs(name);
        if (_txt_data == null)
		{
			Debug.Log("err config__" + name); 
			return;
		}

        byte[] data = _txt_data.bytes;
        m_records.Add(new List<int>());
        int l = 0;
        for (int i = 0; i < data.Length; ++i)
        {
            if ((data[i] == '\n' || data[i] == '\t'))
			{
                string _out = System.Text.Encoding.UTF8.GetString(data, l, i - l);
                m_records[m_records.Count - 1].Add(int.Parse(_out));
                l = i + 1;
			}
            if (data[i] == '\n')
            {
                m_records.Add(new List<int>());
            }
        }
        m_records.RemoveAt(m_records.Count - 1);
        m_xnum = m_records[0].Count;
        m_ynum = m_records.Count;
	}

    public void clear()
    {
        m_records.Clear();
    }

	public int get(int x, int y)
	{
        y = m_ynum - 1 - y;
        if (x < 0 || x >= m_xnum || y < 0 || y >= m_ynum)
        {
            return -1;
        }
        return m_records[y][x];
    }

	public int get_x()
	{
		return m_records[0].Count;
	}

	public int get_y()
	{
		return m_records.Count;
	}
}
