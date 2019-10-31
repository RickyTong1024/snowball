using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private bool m_is_loading = false;
    private GameObject m_scene;
    private AssetBundle m_bundle;
    private Camera m_cam;
    private string m_load_name;
    private AsyncOperation m_async;
    private LuaFunction m_luafunc;
    private float m_shake_time = 0;
    private Vector3 m_shake_vec = Vector3.zero;
    private Vector3 m_cam_test_v;
    private Vector3 m_cam_v1;
    private Vector3 m_cam_v2;
    private Vector3 m_cur_pos;
    private Vector3 m_target_v1;
    private int m_type;
    private Vector3 m_save_cam_v1;
    private Vector3 m_save_cam_v2;
    private Vector3 m_save_cur_pos;
    private Vector3 m_save_target_v1;
    private int m_state = 0;
    private navMeshInfo m_nmi1;
    private navMeshInfo m_nmi2;

    void Start()
    {
        m_cam = Camera.main;
        if (AppFacade._instance.test)
        {
            m_cam_test_v = m_cam.transform.localPosition;
        }
    }

    public void fini()
    {
        UnloadScene();
    }

    public void LoadScene(string name, LuaFunction luafunc)
    {
        if (m_is_loading == true)
        {
            return;
        }

        UnloadScene();
        Util.ClearMemory();
        AppFacade._instance.ResourceManager.ClearEffect();

        m_load_name = name;
        m_is_loading = true;
        m_luafunc = luafunc;
        m_state = 0;

        StartCoroutine(loadScene());
    }

    IEnumerator loadScene()
    {
        m_async = SceneManager.LoadSceneAsync(m_load_name);
        m_state = 1;
        yield return m_async;
        m_is_loading = false;
        if (m_luafunc != null)
        {
            m_luafunc.Call();
        }
    }

    public bool IsLoading()
    {
        return m_is_loading;
    }

    public float LoadProgress()
    {
        if (m_state == 0)
        {
            return 0;
        }
        return m_async.progress;
    }

    public void SetScene(GameObject obj, Camera cam)
    {
        m_scene = obj;
        m_cam = cam;
    }

    public void SaveCam()
    {
        m_save_cam_v1 = m_cam_v1;
        m_save_cam_v2 = m_cam_v2;
        m_save_cur_pos = m_cur_pos;
        m_save_target_v1 = m_target_v1;
    }

    public void LoadCam()
    {
        m_cam_v1 = m_save_cam_v1;
        m_cam_v2 = m_save_cam_v2;
        m_cur_pos = m_save_cur_pos;
        m_target_v1 = m_save_target_v1;
    }

    public void SetInitCam(float x1, float y1, float z1, float x2, float y2, float z2, int type)
    {
        Vector3 v1 = new Vector3(x1, y1, z1);
        Vector3 v2 = new Vector3(x2, y2, z2);
        m_cam_v1 = v1;
        m_cam_v2 = v2;
        m_cam.transform.localPosition = v1;
        m_cam.transform.localEulerAngles = v2;
        m_shake_time = 0;
        m_shake_vec = Vector3.zero;
        m_cur_pos = Vector3.zero;
        m_target_v1 = v1;
        m_type = type;
    }

    public void SetCurCam(float x, float y, float z)
    {
        m_cur_pos = new Vector3(x, y, z);
    }

    public void SetVCam(float x, float y, float z)
    {
        m_cam_v1 = new Vector3(x, y, z);
        m_target_v1 = new Vector3(x, y, z);
        update_cam();
    }

    public void SetTargetCam(float x, float y, float z)
    {
        m_target_v1 = new Vector3(x, y, z);
    }

    public Vector3 WorldToScreenPoint(Vector3 position)
    {
        if (m_cam == null)
        {
            return position;
        }            
        Vector3 _pos = m_cam.WorldToScreenPoint(position);
        float s = AppFacade._instance.PanelManager.get_whscale();
        _pos.x /= s;
        _pos.y /= s;
        _pos.z = 0;
        return _pos;
    }

    public void UnloadScene()
    {
        if (m_scene != null)
        {
            GameObject.Destroy(m_scene);
            m_scene = null;
        }
        if (m_bundle != null)
        {
            m_bundle.Unload(true);
            m_bundle = null;
        }
    }

    public void shake_cam(float time)
    {
        m_shake_time = time;
    }

    void update_cam()
    {
        if (m_shake_time > 0)
        {
            float _addx = UnityEngine.Random.Range(-m_shake_time / 3, m_shake_time / 3);
            float _addy = UnityEngine.Random.Range(-m_shake_time / 3, m_shake_time / 3);
            float _addz = UnityEngine.Random.Range(-m_shake_time / 3, m_shake_time / 3);
            m_shake_vec = new Vector3(_addx, _addy, _addz);
            m_shake_time -= Time.deltaTime;
        }
        else
        {
            m_shake_vec = Vector3.zero;
        }
        if (m_cam != null)
        {
            if (AppFacade._instance.test)
            {
                m_cam.transform.localPosition = m_cam_test_v + m_shake_vec;
            }
            else
            {
                if (m_cam_v1 != m_target_v1)
                {
                    Vector3 v1 = m_target_v1 - m_cam_v1;
                    Vector3 v = v1 * Time.deltaTime * 2;
                    if (v1.sqrMagnitude < 0.0001f || Time.deltaTime * 2 >= 1)
                    {
                        m_target_v1 = m_cam_v1;
                    }
                    else
                    {
                        m_cam_v1 = m_cam_v1 + v;
                    }
                }
                m_cam.transform.localPosition = m_cam_v1 + m_cur_pos + m_shake_vec;
                AppFacade._instance.ResourceManager.AudioListener.transform.localPosition = m_cur_pos;
            }
        }
    }

    void LateUpdate()
    {
        if (m_type == 1)
        {
            update_cam();
        }
    }
    public int in_view(float x, float y, float z)
    {
        Vector3 v = WorldToScreenPoint(new Vector3(x, y, z));
        if (v.x < -10 || v.x > Screen.width + 10 || v.y < -10 || v.y > Screen.height + 10)
        {
            return -1;
        }
        return 0;
    }

    public void load_nmi(string name)
    {
        m_nmi1 = new navMeshInfo();
        m_nmi1.load_nav(name + "_nav");
        m_nmi2 = new navMeshInfo();
        m_nmi2.load_nav(name + "_nav1");
        NavUtil.InitNavTriInfoFromNavMesh(m_nmi1);
    }

    public void clear_nmi()
    {
        NavUtil.Clear();
        if (m_nmi1 != null)
        {
            m_nmi1.clear();
        }
        if (m_nmi2 != null)
        {
            m_nmi2.clear();
        }
    }

    public bool can_move(int x, int y)
    {
        return m_nmi1.can_move(x, y);
    }

    public bool can_move1(int x, int y)
    {
        return m_nmi2.can_move(x, y);
    }

    private void OnDestroy()
    {
        fini();
    }
}
