using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DemoButtonSet : MonoBehaviour {

	public GameObject AnimatorObject;
	public GameObject LookTarget;
	public TargetLookSet TLSObject;
	public Vector3 WindEffectVec;
	public Vector3 WindPendulum;
	public float WindEffectSp;

	private bool LookMode, windOnOff;
	private Animator animator;
	private string animMode = "";
	private CameraOperation CamOpe;
	private RotationTest RotTes;
	private Text infoText;
	private float expression, timeData;
	private List<SmoothJoint> sjco;
	private List<SmoothJoint> sjWco;
	private DemoDataSet[] ddSst;
	private Vector3 oldLookPos;
	private Transform camTra;

	private TargetLookSet targetlS;


	public void Start ()
	{
		this.LookMode = false;
		this.windOnOff = false;
		this.timeData = Time.realtimeSinceStartup;

		this.expression = 0f;

		this.oldLookPos = Vector3.zero;
		if(this.LookTarget != null)
		{
			this.oldLookPos = this.LookTarget.transform.position;
		}

		GameObject CameraObj = GameObject.FindWithTag("MainCamera");
		GameObject InfoObj = GameObject.Find("CanvasInfo/info");

		if (InfoObj != null)
		{
			Text infoTextObj = InfoObj.GetComponent<Text>();

			if (infoTextObj != null)
			{
				this.infoText = infoTextObj;
			}
		}
		else
		{
			Debug.Log ("InfoObj Null!");
		}

		if(this.AnimatorObject != null)
		{
			this.animator = this.AnimatorObject.GetComponent<Animator>();
		}

		if (CameraObj != null)
		{
			this.CamOpe = CameraObj.GetComponent<CameraOperation>();
			this.RotTes = CameraObj.GetComponent<RotationTest>();

			this.camTra = CameraObj.transform;
		}
		else
		{
			Debug.Log ("CameraObj Null!");
		}

		if (this.CamOpe == null)
		{
			Debug.Log ("CameraOperation Null!");
		}

		if (this.RotTes == null)
		{
			Debug.Log ("RotationTest Null!");
		}

		if(this.TLSObject != null)
		{
			TargetLookSet[] tlSa = this.TLSObject.GetComponents<TargetLookSet>();
			if(tlSa != null)
			{
				for (int ti = 0; ti < tlSa.Length; ti++)
				{
					for (int oi = 0; oi <tlSa[ti].OperationObj.Length; oi++)
					{
						if(tlSa[ti].OperationObj[oi].transform.name == "Head")
						{
							this.targetlS = tlSa[ti];
						}
					}
				}
			}
		}
		else
		{
			Debug.Log ("TLSObject Null!");
		}


		GameObject DollEventObj = GameObject.Find("DollEventSystem");
		if (DollEventObj != null)
		{
			this.ddSst = DollEventObj.GetComponents<DemoDataSet>();

			if (this.ddSst != null)
			{
				for (int di = 0; di < this.ddSst.Length; di++)
				{
					if(this.ddSst[di].EffectName == "SHEffect")
					{
						this.sjWco = this.ddSst[di].sjClass;
					}
				}
			}
		}
	}

	public void Update ()
	{
		float tsd = Time.realtimeSinceStartup - this.timeData;
		if (tsd > 4f)
		{
			this.expression = Mathf.Round(Random.value * 10f);
			this.timeData = Time.realtimeSinceStartup;

			this.EyesAnimatorSet();
		}
	}

	private void WindEffectSet(bool wef)
	{
		if(this.sjWco != null)
		{
			for (int i = 0; i < this.sjWco.Count; i++)
			{
				if(wef == false)
				{
					this.sjWco[i].WindOn = false;
					this.sjWco[i].SJDataReset();
				}
				else
				{
					this.sjWco[i].WindOn = true;
					this.sjWco[i].SJDataReset();
				}
			}
		}
	}

	private void LookEventSet(bool look)
	{
		if(this.LookTarget != null && this.camTra != null && this.AnimatorObject != null)
		{
			if(look == false)
			{
				this.LookTarget.transform.position = this.oldLookPos;
			}
			else
			{
				Vector3 tpos = (this.AnimatorObject.transform.InverseTransformPoint(this.camTra.position)
						 - this.AnimatorObject.transform.position).normalized * 3f;
				tpos.y += 0.8f;

				this.LookTarget.transform.position = tpos;
			}
		}
	}

	private void StopLE(bool slef)
	{
		if(this.targetlS != null)
		{
			if(slef == false)
			{
				if(this.targetlS.TargetLook == true)
				{
					this.targetlS.TargetLook = false;
				}
			}
			else
			{
				if(this.targetlS.TargetLook == false)
				{
					this.targetlS.TargetLook = true;
				}
			}
		}
	}

	private void EyesAnimatorSet()
	{
		if (this.animMode == "BaseWait" || this.animMode == "Jump" || this.animMode == "HipHop"
			|| this.animMode == "Walk" || this.animMode == "Run")
		{
			if (this.animMode == "" && this.LookMode == false)
			{
				this.LookEventSet(false);
			}

			if (this.expression > 1)
			{
				this.LookEventSet(true);
				this.LookMode = true;
			}

			if (this.expression <= 1 && this.LookMode == true)
			{
				this.LookEventSet(false);
				this.LookMode = false;
			}
		}

		if (this.animMode != "BaseWait" && this.animMode != "Jump" && this.animMode != "HipHop"
			&& this.animMode != "Walk" && this.animMode != "Run")
		{
			if (this.LookMode == true)
			{
				this.LookEventSet(false);
				this.LookMode = false;
			}
		}
	}

	public void AnimatorSet(string hit)
	{
		if(hit == "WindEffect")
		{
			this.windOnOff = !this.windOnOff;

			this.WindEffectSet(this.windOnOff);
		}


		if (this.CamOpe != null && this.RotTes != null)
		{
			switch (hit)
			{
			case "CamOperation":

				this.CamOpe.CameraModeF = true;
				this.RotTes.RotationTestF = false;

				this.infoText.text = "Camera move by a click and drag in the scene";

				break;

			case "ObjOperation":

				this.CamOpe.CameraModeF = false;
				this.RotTes.RotationTestF = true;

				this.infoText.text = "Object rotation by a click and drag in the scene";

				break;

			default:

				break;
			}
		}

		if (this.animator != null)
		{
			switch (hit)
			{
			case "BaseWait":
				if (this.animMode == "RotationF-B")
				{
					this.animator.CrossFade("RotationB-FToBaseWait", 0.5f);
				}
				else
				{
					this.animator.CrossFade("BaseWait", 0.5f);
				}

				this.StopLE(true);

				this.animMode = "BaseWait";
				break;

			case "Walk":
				if (this.animMode == "RotationF-B")
				{
					this.animator.CrossFade("RotationB-FToWalk", 0.5f);
				}
				else
				{
					this.animator.CrossFade("Walk", 0.5f);
				}

				this.StopLE(true);

				this.animMode = "Walk";
				break;

			case "Run":
				if (this.animMode == "RotationF-B")
				{
					this.animator.CrossFade("RotationB-FToRun", 0.5f);
				}
				else
				{
					this.animator.CrossFade("Run", 0.5f);
				}

				this.StopLE(true);

				this.animMode = "Run";
				break;

			case "ToStrokeUpward":
				if (this.animMode == "RotationF-B")
				{
					this.animator.CrossFade("RotationB-FToStrokeUpward", 0.5f);
				}
				else
				{
					this.animator.CrossFade("ToStrokeUpward", 0.5f);
				}

				this.StopLE(true);

				this.animMode = "ToStrokeUpward";
				break;

			case "LookRightAndLeft":
				if (this.animMode == "RotationF-B")
				{
					this.animator.CrossFade("RotationB-FToLookRightAndLeft", 0.5f);
				}
				else
				{
					this.animator.CrossFade("LookRightAndLeft", 0.5f);
				}

				this.StopLE(false);

				this.animMode = "LookRightAndLeft";
				break;

			case "Jump":
				if (this.animMode == "RotationF-B")
				{
					this.animator.CrossFade("RotationB-FToJump", 0.5f);
				}
				else
				{
					this.animator.CrossFade("Jump", 0.5f);
				}

				this.StopLE(true);

				this.animMode = "Jump";
				break;

			case "WaggleHead":
				if (this.animMode == "RotationF-B")
				{
					this.animator.CrossFade("RotationB-FToWaggleHead", 0.5f);
				}
				else
				{
					this.animator.CrossFade("WaggleHead", 0.5f);
				}

				this.StopLE(false);

				this.animMode = "WaggleHead";
				break;

			case "HipHop":
				if (this.animMode == "RotationF-B")
				{
					this.animator.CrossFade("RotationB-FToHipHop", 0.5f);
				}
				else
				{
					this.animator.CrossFade("HipHop", 0.5f);
				}

				this.StopLE(true);

				this.animMode = "HipHop";
				break;

			case "RotationF-B":
				if (this.animMode == "RotationF-B")
				{
					this.animator.CrossFade("RotationB-FToRotationF-B", 0.5f);
				}
				else
				{
					this.animator.CrossFade("RotationF-B", 0.5f);
				}

				this.StopLE(false);

				this.animMode = "RotationF-B";
				break;

			default:

				break;
			}
		}
	}
}
