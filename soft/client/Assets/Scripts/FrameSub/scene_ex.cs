using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scene_ex : MonoBehaviour {

    public Camera m_cam;
    public GameObject[] m_ms;

    void Awake ()
    {
        if (AppFacade._instance == null)
        {
            return;
        }
		DontDestroyOnLoad(this.gameObject);
		AppFacade._instance.MapManager.SetScene(this.gameObject, m_cam);
        MeshRenderer[] ms = GetComponentsInChildren<MeshRenderer>(true);
        for (int i = 0; i < ms.Length; ++i)
        {
            ms[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            ms[i].receiveShadows = false;
        }
        for (int i = 0; i < m_ms.Length; ++i)
        {
            ms = m_ms[i].GetComponentsInChildren<MeshRenderer>(true);
            for (int j = 0; j < ms.Length; ++j)
            {
                ms[j].receiveShadows = true;
            }
        }
    }
}
