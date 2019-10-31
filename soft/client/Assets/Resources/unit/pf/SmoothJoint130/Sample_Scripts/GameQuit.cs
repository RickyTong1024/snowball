using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//To end at Esc key
public class GameQuit : MonoBehaviour {

	public Button btnObj;

	private RectTransform rectT;

	public void Awake()
	{
		this.Init();
	}

	private void Init()
	{
		//Initialization
		if(this.btnObj != null)
		{
			this.rectT = this.btnObj.GetComponent<RectTransform>();
		}
	}

	public void Update ()
	{
		//Game over
		if (Input.GetKey("escape"))
		{
			Application.Quit();
		}

		//Layout
		if(this.rectT != null)
		{
			Vector2 pwsd = new Vector2((float)Screen.width, (float)Screen.height);
			Vector2 bpos = Vector2.zero;
			bpos.x = (pwsd.x * -0.5f) + 80f;
			bpos.y = (pwsd.y * -0.5f) + 12f;

			this.rectT.anchoredPosition = bpos;
		}
	}

	public void GameQuitSet()
	{
		//Exit
		Application.Quit();
	}
}
