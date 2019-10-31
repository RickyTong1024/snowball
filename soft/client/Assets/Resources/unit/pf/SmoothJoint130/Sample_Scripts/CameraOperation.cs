using UnityEngine;
using System.Collections;

public class CameraOperation : MonoBehaviour {

	public Vector3 MaxPosition;
	public Vector3 MiniPosition;
	public Vector2 MouseSensibility = new Vector2(1f, 1f);

	private Vector3 oldMousePos;
	private bool cameraMode, mouseF;
	private Camera cam;


	public bool CameraModeF
	{
		get{return this.cameraMode;}
		set
		{
			this.cameraMode = value;
		}
	}

	public void Start()
	{
		this.cam = GetComponent<Camera>();
		this.oldMousePos = Vector3.zero;
		this.cameraMode = false;
		this.mouseF = false;
	}

	public void Update()
	{
		if(this.cameraMode == true)
		{
			if (Input.GetMouseButtonDown(0) == true && this.mouseF == false)
			{
				this.mouseF = true;
				this.oldMousePos = this.mousePosSet();

				//Debug.Log ("MouseButtonDown");
			}

			if( Input.GetMouseButtonUp(0) == true && this.mouseF == true)
			{
				this.mouseF = false;
				this.oldMousePos = Vector3.zero;
			}

			if(this.mouseF == true)
			{
				this.CameraOperationSet();
			}
		}
	}

	private Vector3 mousePosSet()
	{
		Vector3 mpos = Vector3.zero;

		if ( Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch (0);

			mpos.x = touch.position.x;
			mpos.y = touch.position.y;
		}
		else
		{
			mpos = Input.mousePosition;
		}

		mpos.z = 1f;

		return mpos *= 0.0001f;
	}

	private void CameraOperationSet()
	{
		Vector3 mpos = this.mousePosSet();
		
		Vector3 cpos = Vector3.zero;

		Vector3 vad = Vector3.zero;
		vad = (mpos - this.oldMousePos);

		if(mpos != this.oldMousePos)
		{

			cpos.y = this.cam.transform.position.y + (vad.y * this.MouseSensibility.y);
			cpos.z = this.cam.transform.position.z + (vad.x * this.MouseSensibility.x);

			cpos.y = Mathf.Clamp(cpos.y, this.MiniPosition.y, this.MaxPosition.y);
			cpos.z = Mathf.Clamp(cpos.z, this.MiniPosition.z, this.MaxPosition.z);


			this.cam.transform.position = cpos;

		}
	}
}
