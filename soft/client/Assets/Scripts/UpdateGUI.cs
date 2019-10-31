using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum update_state
{
    Progress = 1,
    Warn = 2,
    Select
}
public class UpdateGUI : MonoBehaviour
{
    [HideInInspector]
    private List<string> downloadFiles = new List<string>();
    // Use this for initialization
    UILabel m_pro_label;
    UISprite m_anim_sprite;
    UISlider m_pro_slider;
    GameObject m_select_panel;
    GameObject m_double_panel;
    GameObject m_single_panel;
    Transform m_single_btn;
    Transform m_double_yes_btn;
    Transform m_double_no_btn;
    UILabel m_select_desc;
    GameObject m_progress_panel;
    JsonData game_url;
    JsonData ver_inf;
    int view_width;
    bool isDownLoad = false;
    bool is_pro_show = false;
    bool is_checking = false;
    float download_pro = 0;
    WWW down_www;
    float check_time = 0;
    public delegate void CheckDelegate();
    CheckDelegate checkFail;

    void Start()
    {
        float width = this.transform.GetComponent<UIPanel>().width;
        float height = this.transform.GetComponent<UIPanel>().height;
        float per = width / height;
        if (per < 960 / 640)
        {
            view_width = 960;
        }
        else
        {
            view_width = (int)(width * 640 / height);
        }
        m_select_panel = this.transform.Find("select_panel").gameObject;
        m_select_desc = m_select_panel.transform.Find("des").GetComponent<UILabel>();
        m_double_panel = m_select_panel.transform.Find("double_panel").gameObject;
        m_single_panel = m_select_panel.transform.Find("single_panel").gameObject;
        m_single_btn = m_single_panel.transform.Find("single");
        m_double_yes_btn = m_double_panel.transform.Find("yes");
        m_double_no_btn = m_double_panel.transform.Find("no");
        m_progress_panel = this.transform.Find("Anchor_bottom/progress_panel").gameObject;
        m_pro_label = this.transform.Find("Anchor_bottom/progress_panel/des").GetComponent<UILabel>();
        m_anim_sprite = this.transform.Find("Anchor_bottom/progress_panel/am_sprite").GetComponent<UISprite>();
        m_pro_slider = this.transform.Find("Anchor_bottom/progress_panel/pro_slider").GetComponent<UISlider>();
        m_progress_panel.SetActive(false);
        m_select_panel.SetActive(false);
        m_double_panel.SetActive(false);
        m_single_panel.SetActive(false);
        StartCoroutine(CheckExtractResource());
    }

    void Update()
    {
        if (isDownLoad && down_www != null)
        {
            download_pro = download_pro + Time.deltaTime;
            if (download_pro > down_www.progress)
            {
                download_pro = down_www.progress;
                m_pro_slider.value = download_pro;
                m_pro_label.text = String.Format(Config.get_t_script_str("UpdateGUI_001"), (int)(download_pro * 100));
            }
        }
        if (is_pro_show)
        {
            download_pro = download_pro + Time.deltaTime;
            if (download_pro > 1)
            {
                is_pro_show = false;
                download_pro = 1;
            }
            m_pro_slider.value = download_pro;
        }
        if (m_progress_panel.activeInHierarchy)
        {
            float progress = m_pro_slider.value;
            m_anim_sprite.transform.localPosition = new Vector3(progress * (view_width - 160) - view_width / 2 + 100 - progress * 40, m_anim_sprite.transform.localPosition.y, 0);
        }
        if (is_checking)
        {
            check_time += Time.deltaTime;
            if (check_time >= 20)
            {
                checkFail();
            }
        }
    }
    // Update is called once per frame
    IEnumerator CheckExtractResource()
    {
        ChageState(update_state.Progress, new string[] { Config.get_t_script_str("UpdateGUI_002") });
        is_pro_show = true;
        download_pro = 0;
        is_checking = true;
        check_time = 0;
        checkFail = CheckVersionFail;
        if (platform_config_common.m_debug)
        {
            ResoureInit();
            yield break;
        }
        //加载Url
        string url = platform_config_common.m_common_url;
        string random = DateTime.Now.ToString("yyyymmddhhmmss");
        string infUrl = url + "info.json?v=" + random;
        WWW www = new WWW(infUrl);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("url error:" + infUrl);
            ChageState(update_state.Warn, new string[] { Config.get_t_script_str("UpdateGUI_003"), Config.get_t_script_str("UpdateGUI_004"), "CheckExtractResource" });
            yield break;
        }
        game_url = JsonMapper.ToObject(www.text);

        string verUrl = url + "version/ver"+platform_config_common.version+".json?v=" + random;
        www = new WWW(verUrl);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("url error:" + verUrl);
            ChageState(update_state.Warn, new string[] { Config.get_t_script_str("UpdateGUI_003"), Config.get_t_script_str("UpdateGUI_004"), "CheckExtractResource" });
            yield break;
        }
        ver_inf = JsonMapper.ToObject(www.text);
        if(check_time >= 20)
        {
            yield break;
        }
        is_checking = false;
        //检查版本
        if (ToolManager.CountVerInf(ver_inf["ver_min"].ToString()) > ToolManager.CountVerInf(platform_config_common.version))
        {
            if (Application.platform == RuntimePlatform.Android && ver_inf["ver_store"].Keys.Contains(platform_config_common.m_platform))
            {
                ChageState(update_state.Select, new string[] { Config.get_t_script_str("UpdateGUI_005"), Config.get_t_script_str("UpdateGUI_006"), "DownLoad", Config.get_t_script_str("UpdateGUI_007"), "Quit" });
            }
            else
            {
                if (!ver_inf["ver_store"].Keys.Contains(platform_config_common.m_platform))
                {
                    ChageState(update_state.Warn, new string[] { Config.get_t_script_str("UpdateGUI_005"), Config.get_t_script_str("UpdateGUI_008"), "Quit" });
                }
                else
                {
                    ChageState(update_state.Select, new string[] { Config.get_t_script_str("UpdateGUI_005"), Config.get_t_script_str("UpdateGUI_009"), "WebPage", Config.get_t_script_str("UpdateGUI_007"), "Quit" });
                }
            }
            yield break;
        }
        string dataPath = Util.DataPath;
        string verfile = dataPath + "ver.txt";
        if (!File.Exists(verfile))
        {
            Debug.Log("新包 解压");
            StartCoroutine(OnExtractResource());
            yield break;
        }
        else
        {
            int cur_ver = 0;
            int pre_ver = 0;
            string ver_infile = Util.AppContentPath() + "ver.txt";
            StreamReader sr = new StreamReader(verfile);
            pre_ver = ToolManager.CountVerInf(sr.ReadLine());
            sr.Close();
            if (Application.platform == RuntimePlatform.Android)
            {
                www = new WWW(ver_infile);
                yield return www;
                if (www.isDone)
                {
                    cur_ver = ToolManager.CountVerInf(www.text);
                }
            }
            else
            {
                sr = new StreamReader(ver_infile);
                cur_ver = ToolManager.CountVerInf(sr.ReadLine());
                sr.Close();
            }
            Debug.Log("new ver:" + cur_ver);
            Debug.Log("old ver:" + pre_ver);
            if (cur_ver > pre_ver)
            {
                Debug.Log("新包 解压 覆盖");
                if (Directory.Exists(Util.DataPath))
                {
                    DeleteDirectory(Util.DataPath);
                }
                StartCoroutine(OnExtractResource());
                yield break;
            }
        }
        ResoureInit();
    }

    void CheckVersionFail()
    {
        StopCoroutine(CheckExtractResource());
        ChageState(update_state.Warn, new string[] { Config.get_t_script_str("UpdateGUI_003"), Config.get_t_script_str("UpdateGUI_004"), "CheckExtractResource" });
    }

    IEnumerator OnExtractResource()
    {
        string resPath = Util.AppContentPath() + "res.zip";//游戏包资源目录 
        string copy_path = Util.DataPath + "res.zip";
        string directoryName = Path.GetDirectoryName(copy_path);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        utils.DecompressDirProgress depPro = new utils.DecompressDirProgress(ShowDePressPro);
        utils.DecompressDirFinish depFin = new utils.DecompressDirFinish(ShowFinish);
        ChageState(update_state.Progress, new string[] { "Unzip files" });
        string message = Config.get_t_script_str("UpdateGUI_010") + ":>";
        m_pro_label.text = message;
        if (File.Exists(copy_path))
        {
            File.Delete(copy_path);
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(resPath);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(copy_path, www.bytes);
                StartCoroutine(utils.DecompressDir(copy_path, Util.DataPath, depPro, depFin));
            }
            yield return 0;
        }
        else
        {
            File.Copy(resPath, copy_path);
            StartCoroutine(utils.DecompressDir(copy_path, Util.DataPath, depPro, depFin));
        }
    }

    void ShowDePressPro(string name, float progress)
    {
        string message = Config.get_t_script_str("UpdateGUI_010") + ":>" + name;
        m_pro_label.text = message;
        m_pro_slider.value = progress;
    }

    void ShowFinish()
    {
        StartCoroutine(DePressFinish());
    }

    IEnumerator DePressFinish()
    {
        string dataPath = Util.DataPath;  //数据目录
        string resPath = Util.AppContentPath(); //游戏包资源目录
        string ver_infile = resPath + "ver.txt";
        string ver_outfile = dataPath + "ver.txt";
        string copy_path = Util.DataPath + "res.zip";
        if (File.Exists(copy_path))
        {
            File.Delete(copy_path);
        }
        if (File.Exists(ver_outfile))
        {
            File.Delete(ver_outfile);
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(ver_infile);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(ver_outfile, www.bytes);
            }
            yield return 0;
        }
        else
        {
            File.Copy(ver_infile, ver_outfile, true);
        }
        yield return new WaitForEndOfFrame();
        m_pro_label.text = Config.get_t_script_str("UpdateGUI_011");// "解包完成!!!";
        yield return new WaitForSeconds(0.1f);
        m_progress_panel.SetActive(false);
        ResoureInit();
    }

    void ResoureInit()
    {
        m_progress_panel.SetActive(false);
        if (AppFacade._instance != null)
        {
            AppFacade._instance.GetComponent<GameManager>().UpdateComplete();
        }
        Destroy(this.gameObject);
    }

    void ChageState(update_state state, string[] arg)
    {
        m_select_panel.SetActive(false);
        m_double_panel.SetActive(false);
        m_single_panel.SetActive(false);
        if (state == update_state.Progress)
        {
            is_pro_show = false;
            m_pro_label.text = arg[0];
            m_pro_slider.value = 0;
            m_anim_sprite.transform.localPosition = new Vector3(100 - view_width / 2, m_anim_sprite.transform.localPosition.y, 0);
            m_progress_panel.SetActive(true);
        }
        else if (state == update_state.Warn)
        {
            is_checking = false;
            m_select_desc.text = arg[0];
            m_single_btn.Find("Label").GetComponent<UILabel>().text = arg[1];
            m_single_btn.name = arg[2];
            m_double_panel.SetActive(false);
            m_single_panel.SetActive(true);
            m_select_panel.SetActive(true);
        }
        else if (state == update_state.Select)
        {
            m_select_desc.text = arg[0];
            m_double_yes_btn.Find("Label").GetComponent<UILabel>().text = arg[1];
            m_double_yes_btn.name = arg[2];
            m_double_no_btn.Find("Label").GetComponent<UILabel>().text = arg[3];
            m_double_no_btn.name = arg[4];
            m_double_panel.SetActive(true);
            m_single_panel.SetActive(false);
            m_select_panel.SetActive(true);
        }
    }

    void click(GameObject obj)
    {
        if (obj.name == "CheckExtractResource")
        {
            StartCoroutine(CheckExtractResource());
        }
        else if (obj.name == "DownLoad")
        {
            StartCoroutine(DownLoadApk());
        }
        else if (obj.name == "WebPage")
        {
            Application.OpenURL(ver_inf["ver_store"][platform_config_common.m_platform].ToString());
            Application.Quit();
        }
        else if (obj.name == "Quit")
        {
            Destroy(this.gameObject);
            Application.Quit();
        }
    }

    IEnumerator DownLoadApk()
    {
        ChageState(update_state.Progress, new string[] { Config.get_t_script_str("UpdateGUI_019") });
        down_www = new WWW(ver_inf["ver_store"][platform_config_common.m_platform].ToString());
        isDownLoad = true;
        download_pro = 0;
        string directoryName = Path.GetDirectoryName(Util.DataPath);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        yield return down_www;
        if (down_www.error != null)
        {
            isDownLoad = false;
            down_www.Dispose();
            ChageState(update_state.Warn, new string[] { Config.get_t_script_str("UpdateGUI_017"), Config.get_t_script_str("UpdateGUI_004"), "CheckExtractResource" });
            yield break;
        }
        if (down_www.isDone)
        {
            isDownLoad = false;
            string save_path = Util.DataPath + "snowball.apk";
            try
            {
                if (File.Exists(save_path))
                {
                    File.Delete(save_path);
                }
                File.WriteAllBytes(save_path, down_www.bytes);
                down_www.Dispose();
                platform._instance.game_install(save_path);
                Application.Quit();
            }
            catch (IOException e)
            {
                Debug.Log(e.Message);
            }
        }
    }
    public static void DeleteDirectory(string target_dir)
    {
        string[] files = Directory.GetFiles(target_dir);
        string[] dirs = Directory.GetDirectories(target_dir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(target_dir, false);
    }
}
