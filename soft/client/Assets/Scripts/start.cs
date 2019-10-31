using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour {

	// Use this for initialization
    void Start()
    {
        platform_config_common.init();
        platform_config.init();
        float bl = Screen.width / (float)Screen.height;
        if (bl > 1.5)
        {
            Screen.SetResolution(Screen.width * 720 / Screen.height, 720, true);
        }
        else
        {
            Screen.SetResolution(1280, Screen.height * 1280 / Screen.width, true);
        }
        SceneManager.LoadSceneAsync("main");
    }
}
