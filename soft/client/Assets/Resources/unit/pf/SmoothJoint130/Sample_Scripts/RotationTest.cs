using UnityEngine;
using System.Collections;

public class RotationTest : MonoBehaviour {

	public Transform TargetObj;
	public GameObject TestObj;
	public float RotSpeed = 1f;

	private Camera cam;
	private bool RotationTestMode, mouseF;
	private Vector3 oldmousePos, mousePos;

	public bool RotationTestF
	{
		get{return this.RotationTestMode;}
		set
		{
			this.RotationTestMode = value;
		}
	}

	public void Start()
	{
		this.RotationTestMode = true;
		this.cam = GetComponent<Camera>();
		this.mouseF = false;
		this.oldmousePos = Vector3.zero;
		this.mousePos = Vector3.zero;

		this.RotationCameraSet();
	}

	public void Update()
	{
		if(this.RotationTestMode == true)
		{
			if (Input.GetMouseButtonDown(0) == true && this.mouseF == false)
			{
				this.mouseF = true;
				this.oldmousePos = this.mousePosSet();
			}

			if( Input.GetMouseButtonUp(0) == true && this.mouseF == true)
			{
				this.mouseF = false;
				this.oldmousePos = Vector3.zero;
			}

			if(this.mouseF == true)
			{
				this.RotationCameraSet();
			}
		}
	}

	private Vector3 mousePosSet()
	{
		Vector3 mposd = Vector3.zero;
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

		if (this.cam != null)
		{
			mposd = this.cam.ScreenToWorldPoint(mpos);
		}

		return mposd;
	}

	private Vector3 PosRotSet(Vector3 od, Vector3 md, Vector3 td, Transform tra)
	{

		Vector3 roth = Vector3.zero;

		Vector3 tod = Vector3.zero;
		Vector3 mod = Vector3.zero;

		Quaternion ro = Quaternion.identity;

		roth = ro.eulerAngles;

		tod += (tra.InverseTransformDirection(td - od).normalized);
		mod += (tra.InverseTransformDirection(md - od).normalized);


		if (tod != Vector3.zero && mod != Vector3.zero)
		{
			ro = Quaternion.FromToRotation(tod, mod);

			ro.x = (Mathf.Abs(ro.x) < 0.001f) ? 0f : ro.x;
			ro.y = (Mathf.Abs(ro.y) < 0.001f) ? 0f : ro.y;
			ro.z = (Mathf.Abs(ro.z) < 0.001f) ? 0f : ro.z;

			roth = ro.eulerAngles;
		}

		return roth;
	}

	private void RotationCameraSet()
	{

		this.mousePos = this.mousePosSet();

		Vector3 vad = Vector3.zero;
		vad = this.mousePos - this.oldmousePos;

		vad *= this.RotSpeed;

		//RotationSet------------------

		Vector3 rotv = Vector3.zero;
		rotv = this.PosRotSet(TargetObj.position
		                      , this.transform.position + vad
		                      , this.transform.position
		                      , TargetObj);

		rotv.x = 0f;
		rotv.z = 0f;

		rotv.y %= 360f;

		if(TestObj != null)
		{
			this.TestObj.transform.localRotation *= Quaternion.Euler(rotv);
		}
	}
}
