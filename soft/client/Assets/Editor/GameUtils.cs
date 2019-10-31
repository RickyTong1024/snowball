using BattleDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GameUtils
{
    [MenuItem("HHQ Utils/GetPrefabFiles")]
    public static void ShowPrefabFiles()
    {
        UnityEngine.Object obj = Selection.activeObject;
        var path = AssetDatabase.GetAssetPath(obj);
        path = path.Substring(7);

        AssetBundle bundles = AssetBundle.LoadFromFile(Application.dataPath + "/" + path);
        foreach (var item in bundles.GetAllAssetNames())
            Debug.Log(item);
    }
    
    [MenuItem("Assets/GenLanguageFile")]
    public static void GenPrefabLanguageFile()
    {
        var obj = Selection.activeGameObject;
        var list = obj.GetComponentsInChildren<UILabel>(true);
        int num = 1;
        string directPath = Application.dataPath + "/LanguageOutput/";

        if (!Directory.Exists(directPath))
            Directory.CreateDirectory(directPath);

        string outpath = directPath + obj.name + ".txt";

        using (FileStream fs = new FileStream(outpath, FileMode.Create, FileAccess.Write))
        {
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("gbk"));
            for (int i = 0; i < list.Length; i++)
            {
                string content = list[i].text;
                if (!String.IsNullOrEmpty(content))
                {
                    sw.Write(String.Format("{0}_{1:D3}\t{2}\n", obj.name, num, content));
                    num = num + 1;
                }
            }
            sw.Flush();
            sw.Close();
        }
        Debug.LogError("File Complete!");
    }

    [MenuItem("HHQ Utils/GenLanguageDirectory")]
    public static void GenLanguageDirectory()
    {    
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.LogError("GenLanguageDirectory : " + path);
        if (!Directory.Exists(path))
            return;

        string directPath = Application.dataPath + "/LanguageOutput/";

        if (!Directory.Exists(directPath))
            Directory.CreateDirectory(directPath);

        string[] files = Directory.GetFiles(path);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].EndsWith(".prefab"))
                continue;

            string fileName = Path.GetFileNameWithoutExtension(files[i]);
            string outpath = directPath + fileName + ".txt";
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(files[i]);

            using (FileStream fs = new FileStream(outpath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("gbk"));
                var list = obj.GetComponentsInChildren<UILabel>(true);
                int num = 1;
                for (int m = 0; m < list.Length; m++)
                {
                    string content = list[m].text;
                    string p = GetComponentPath(list[m].transform);
                    if (!String.IsNullOrEmpty(content))
                    {
                        sw.Write(String.Format("{0}_{1:D3}\t{2}\t{3}\n", obj.name, num,p,content));
                        num = num + 1;
                    }
                }
                sw.Flush();
                sw.Close();
            }
        }

        Debug.LogError("文件夹 输出完毕!");
    }

    [MenuItem("HHQ Utils/SetLabelSetting")]
    public static void SetALLPreFabLabelSetting()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!Directory.Exists(path))
            return;

        string directPath = Application.dataPath + "/LanguageOutput/";

        if (!Directory.Exists(directPath))
            Directory.CreateDirectory(directPath);

        string[] files = Directory.GetFiles(path);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].EndsWith(".prefab"))
                continue;

            string fileName = Path.GetFileNameWithoutExtension(files[i]);
            string outpath = directPath + fileName + ".txt";
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(files[i]);

            var list = obj.GetComponentsInChildren<UILabel>(true);
            int num = 1;
            for (int m = 0; m < list.Length; m++)
            {
                string content = list[m].text;
                if (!String.IsNullOrEmpty(content))
                {
                    string key = String.Format("{0}_{1:D3}", obj.name, num);
                    list[m].text = key;
                    list[m].SetDefaultLanguage(true);
                    num = num + 1;
                }
            }
            EditorUtility.SetDirty(obj);
        }
        AssetDatabase.SaveAssets();
        Debug.LogError("SaveAssets 修改完毕!");
    }

    [MenuItem("HHQ Utils/SetPrefabDefaultValue(true)")]
    public static void SetPrefabDefault()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!Directory.Exists(path))
            return;

        string[] files = Directory.GetFiles(path);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].EndsWith(".prefab"))
                continue;

            string fileName = Path.GetFileNameWithoutExtension(files[i]);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(files[i]);

            var list = obj.GetComponentsInChildren<UILabel>(true);
            for (int m = 0; m < list.Length; m++)
            {
                string content = list[m].text;
                if (!String.IsNullOrEmpty(content))
                    list[m].SetDefaultLanguage(true);
            }
            EditorUtility.SetDirty(obj);
        }
        AssetDatabase.SaveAssets();
        Debug.LogError("设置完毕!");
    }

    private static string GetComponentPath(Transform trans)
    {
        List<string> paths = new List<string>();
        while (trans != null)
        {
            paths.Add(trans.name);
            trans = trans.parent;
        }

        paths.Reverse();
        string result = String.Join("/", paths.ToArray());
        return result;
    }

    [MenuItem("HHQ Utils/ExportUnRepeatLanguage")]
    public static void ExportUnRepeatLanguage()
    {
        var obj = Selection.activeGameObject;
        Debug.LogError(Application.dataPath);

        string path = AssetDatabase.GetAssetPath(obj);
        string outPath = Application.dataPath + "/script_lang/";
        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);

        Dictionary<string, t_script_str> db = parse_t_script_langs();
        List<HashSet<string>> langs = new List<HashSet<string>>();
        int max_count = 0;

        foreach (var item in db)
        {
            if (item.Value.lang.Count > langs.Count)
            {
                int dvalue = item.Value.lang.Count - langs.Count;
                for (int m = 0; m < dvalue; m++)
                    langs.Add(new HashSet<string>());
  
                if (item.Value.lang.Count > max_count)
                    max_count = item.Value.lang.Count;
            }

            for (int m = 0; m < item.Value.lang.Count; m++)
            {
                if (!langs[m].Contains(item.Value.lang[m]))
                    langs[m].Add(item.Value.lang[m]);
            }
        }

        for (int m = 0; m < langs.Count; m++)
        {
            if (langs.Count > 0)
            {
                FileStream fs = new FileStream(outPath + "/" + m + ".txt",FileMode.Create,FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach(var value in langs[m])
                        sw.Write(value + "\n");
                }
                fs.Close();
            }
        }

        Debug.LogError("Flush lang!");
    }

    private static Dictionary<string, t_script_str> parse_t_script_langs()
    {
        //var db_script_lang = new dbc();
        //db_script_lang.load_txtName("t_script_str");
        //Dictionary<string, t_script_str>  t_scripts = new Dictionary<string, t_script_str>();

        //for (int i = 0; i < db_script_lang.get_y(); i++)
        //{
        //    var tdic = new t_script_str();
        //    tdic.id = db_script_lang.get_string(0, i);
        //    tdic.lang = new List<string>();

        //    for (int j = 1; j < db_script_lang.get_x(); j++)
        //    {
        //        var lang_str = db_script_lang.get_string(j, i).Replace("##", "\n");
        //        tdic.lang.Add(lang_str);
        //    }
        //    t_scripts.Add(tdic.id, tdic);
        //}

        //return t_scripts;
        return new Dictionary<string, t_script_str>();
    }

    [MenuItem("Assets/GetInfos")]
    public static void GetInfos()
    {
        Debug.LogError("activeInstanceID : " + Selection.activeInstanceID);
        Debug.LogError("assetGUIDs : " + Selection.assetGUIDs);
        for (int i = 0; i < Selection.assetGUIDs.Length; i++)
            Debug.LogError(Selection.assetGUIDs[i]);
    }
}
