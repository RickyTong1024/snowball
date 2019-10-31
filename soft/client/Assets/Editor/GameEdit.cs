using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

class GameEdit : EditorWindow
{
	static GameEdit window;
    string m_save_name = "edit.sav";
    List<string> m_action_name = new List<string>();
    string m_action_name1;
    string m_action_speed = "1";
    string m_part_name;
    string m_gm_command;
    string m_font_color;
    [MenuItem("GameEdit/Test")]
    static void Execute()
    {
        if (window == null)
			window = (GameEdit)GetWindow(typeof (GameEdit));
        window.Show();
    }

    void OnGUI()
    {
        bool _ok = false;
        read();
        GUILayout.Label("--- 行为测试 ---");
        GUILayout.BeginHorizontal();
        GUILayout.Label("速度", GUILayout.Width(60f));
        m_action_speed = EditorGUILayout.TextArea(m_action_speed, GUILayout.Width(160f));
        GUILayout.EndHorizontal();
        for (int i = 0; i < m_action_name.Count; ++i)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(m_action_name[i], GUILayout.Width(160f));
            _ok = GUILayout.Button("确定", GUILayout.Width(60f));
            bool _del = GUILayout.Button("删除", GUILayout.Width(60f));
            GUILayout.EndHorizontal();

            if (_ok)
            {
                s_message _msg = new s_message();

                _msg.name = "action";
                _msg.m_object.Add(m_action_name[i]);
                _msg.m_object.Add(m_action_speed);
                AppFacade._instance.MessageManager.AddMessage(_msg);
            }

            if (_del)
            {
                m_action_name.RemoveAt(i);
                save();
                Repaint();
            }
        }
        m_action_name1 = EditorGUILayout.TextArea(m_action_name1);
        _ok = GUILayout.Button("增加", GUILayout.Width(120f));

        if (_ok)
        {
            if (!m_action_name.Contains(m_action_name1))
            {
                m_action_name.Add(m_action_name1);
                save();
                Repaint();
            }
        }	

		GUILayout.Label("--- 换装测试 ---");
        m_part_name = EditorGUILayout.TextArea(m_part_name);
		_ok = GUILayout.Button("确定", GUILayout.Width(120f));
		
		if (_ok)
        {
            s_message _msg = new s_message();
            _msg.name = "change_part";
            _msg.m_object.Add(m_part_name);
            AppFacade._instance.MessageManager.AddMessage(_msg);
        }

        GUILayout.Label("--- 清理本地 ---");
        _ok = GUILayout.Button("清理", GUILayout.Width(120f));
        if(_ok)
        {
            PlayerPrefs.DeleteAll();
        }

        GUILayout.Label("--- 导出navmesh ---");
        _ok = GUILayout.Button("导出", GUILayout.Width(120f));

        if (_ok)
        {
            string outfile = Application.streamingAssetsPath + "\\nav.txt";
            if (!System.IO.Directory.Exists(Application.streamingAssetsPath))
            {
                System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            GenNavMesh(outfile);
        }

        GUILayout.Label("--- 测试navmesh ---");
        _ok = GUILayout.Button("测试", GUILayout.Width(120f));

        if (_ok)
        {
            DrawMesh();
        }

        GUILayout.Label("--- gm命令 ---");
        m_gm_command = EditorGUILayout.TextArea(m_gm_command);
        _ok = GUILayout.Button("发送", GUILayout.Width(120f));

        if (_ok)
        {
            s_message mes = new s_message();
            mes.name = "edit_gm_command";
            mes.m_object.Add(m_gm_command);
            AppFacade._instance.MessageManager.AddMessage(mes);
        }
        GUILayout.Label("--- 修改字体颜色 ---");
        m_font_color = EditorGUILayout.TextArea(m_font_color);
        _ok = GUILayout.Button("修改", GUILayout.Width(120f));
        if (_ok)
        {
            UnityEngine.Object selectedObject = Selection.activeObject;
            if (selectedObject == null)
            {
                return;
            }
            string path = AssetDatabase.GetAssetPath(selectedObject);
            GameObject obj = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;
            UILabel[] us = obj.transform.GetComponentsInChildren<UILabel>();
            string[] color = m_font_color.Split(' ');
            if (color.Length < 3)
            {
                return;
            }
            float r = float.Parse(color[0]);
            float g = float.Parse(color[1]);
            float b = float.Parse(color[2]);
            for (int j = 0; j < us.Length; ++j)
            {
                us[j].color = new Color(r / 255, g / 255, b / 255, 1);
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        GUILayout.Label("--- 压缩 ---");
        _ok = GUILayout.Button("压缩", GUILayout.Width(120f));

        if (_ok)
        {
            utils.CompressDirProgress cdp = CompressDirProgress;
            utils.CompressDirFinish cdf = CompressDirFinish;
            utils.CompressDir("Assets/StreamingAssets", "Assets/res.zip", cdp, cdf);
        }

        GUILayout.Label("--- 解压 ---");
        _ok = GUILayout.Button("解压", GUILayout.Width(120f));

        if (_ok)
        {
            utils.DecompressDirProgress ddp = DecompressDirProgress;
            utils.DecompressDirFinish ddf = DecompressDirFinish;
            //utils.DecompressDir("Assets/StreamingAssets/res.zip", "d:/aaa", ddp, ddf);
        }
    }

    void CompressDirProgress(string name, float progress)
    {
        EditorUtility.DisplayProgressBar("压缩", name, progress);
    }

    void CompressDirFinish()
    {
        EditorUtility.ClearProgressBar();
    }

    void DecompressDirProgress(string name, float progress)
    {
        EditorUtility.DisplayProgressBar("解压", name, progress);
    }

    void DecompressDirFinish()
    {
        EditorUtility.ClearProgressBar();
    }

    void read()
    {
        m_action_name.Clear();
        string _file_name = Application.persistentDataPath + m_save_name;
        if (File.Exists(_file_name))
        {
            FileStream _file = new FileStream(_file_name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader _br = new BinaryReader(_file);
            int num = _br.ReadInt32();
            for (int i = 0; i < num; ++i)
            {
                string s = _br.ReadString();
                m_action_name.Add(s);
            }
            _file.Close();
        }
    }

    void save()
    {
        FileStream _file = new FileStream(Application.persistentDataPath + m_save_name, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        BinaryWriter _bw = new BinaryWriter(_file);
        BinaryReader _br = new BinaryReader(_file);

        _bw.Write(m_action_name.Count);
        for (int i = 0; i < m_action_name.Count; ++i)
        {
            _bw.Write(m_action_name[i]);
        }

        _file.Close();
    }

    void GenNavMesh(string outfile)
    {
        UnityEngine.AI.NavMeshTriangulation navtri = UnityEngine.AI.NavMesh.CalculateTriangulation();
        Dictionary<int, int> indexmap = new Dictionary<int, int>();
        List<Vector3> repos = new List<Vector3>();
        for (int i = 0; i < navtri.vertices.Length; i++)
        {
            int ito = -1;
            for (int j = 0; j < repos.Count; j++)
            {
                if (Vector3.Distance(navtri.vertices[i], repos[j]) < 0.01)
                {
                    ito = j;
                    break;
                }
            }
            if (ito < 0)
            {
                indexmap[i] = repos.Count;
                repos.Add(navtri.vertices[i]);
            }
            else
            {
                indexmap[i] = ito;
            }
        }

        //关系是 index 公用的三角形表示他们共同组成多边形
        //多边形之间的连接用顶点位置识别
        List<int> polylast = new List<int>();
        List<int[]> polys = new List<int[]>();
        for (int i = 0; i < navtri.indices.Length / 3; i++)
        {
            int i0 = navtri.indices[i * 3 + 0];
            int i1 = navtri.indices[i * 3 + 1];
            int i2 = navtri.indices[i * 3 + 2];

            if (polylast.Contains(i0) || polylast.Contains(i1) || polylast.Contains(i2))
            {
                if (polylast.Contains(i0) == false)
                    polylast.Add(i0);
                if (polylast.Contains(i1) == false)
                    polylast.Add(i1);
                if (polylast.Contains(i2) == false)
                    polylast.Add(i2);
            }
            else
            {
                if (polylast.Count > 0)
                {
                    polys.Add(polylast.ToArray());
                }
                polylast.Clear();
                polylast.Add(i0);
                polylast.Add(i1);
                polylast.Add(i2);
            }
        }
        if (polylast.Count > 0)
            polys.Add(polylast.ToArray());

        string outnav = "";
        outnav = "{\"v\":[\n";
        for (int i = 0; i < repos.Count; i++)
        {
            if (i > 0)
                outnav += ",\n";

            outnav += "[" + (int)(repos[i].x * 10000) + "," + (int)(repos[i].z * 10000) + "]";
        }
        outnav += "\n],\"p\":[\n";

        for (int i = 0; i < polys.Count; i++)
        {
            string outs = indexmap[polys[i][0]].ToString();
            for (int j = 1; j < polys[i].Length; j++)
            {
                outs += "," + indexmap[polys[i][j]];
            }

            if (i > 0)
                outnav += ",\n";

            outnav += "[" + outs + "]";
        }
        outnav += "\n]}";

        System.IO.File.WriteAllText(outfile, outnav);
    }

    void DrawMesh()
    {
        string outfile = Application.streamingAssetsPath + "\\nav.txt";
        List<Vector3> vecs = new List<Vector3>();//顶点
        List<Vector2> uvs = new List<Vector2>();//顶点
        List<int> ids = new List<int>();//节点
        string text = System.IO.File.ReadAllText(outfile);
        JsonData jd = JsonMapper.ToObject(text);

        MeshFilter meshFilter = (MeshFilter)GameObject.Find("test").GetComponent(typeof(MeshFilter));
        Mesh mesh = new Mesh();
        for (int i = 0; i < jd["v"].Count; ++i)
        {
            Vector3 v3 = new Vector3();
            v3.x = int.Parse(jd["v"][i][0].ToString()) / 10000.0f;
            v3.y = 0;
            v3.z = int.Parse(jd["v"][i][1].ToString()) / 10000.0f;
            vecs.Add(v3);
            uvs.Add(new Vector2(v3.x / 80, v3.z / 80));
        }
        mesh.vertices = vecs.ToArray();
        mesh.uv = uvs.ToArray();

        for (int i = 0; i < jd["p"].Count; ++i)
        {
            List<navVec2> vs = new List<navVec2>();
            int f = int.Parse(jd["p"][i][0].ToString());
            for (int j = 1; j < jd["p"][i].Count - 1; ++j)
            {
                int f1 = int.Parse(jd["p"][i][j].ToString());
                int f2 = int.Parse(jd["p"][i][j + 1].ToString());
                ids.Add(f);
                ids.Add(f1);
                ids.Add(f2);
            }
        }
        mesh.triangles = ids.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        meshFilter.sharedMesh = mesh;
    }
}
