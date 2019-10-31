using UnityEngine;
using System.Collections;

//Rotate the GameObject to the target direction 20170703
public class TargetLookSet : MonoBehaviour {

	public GameObject RootObj;//Basic Object of rotational position
	public Camera CameraObj;//Rendering for the camera

	[Space(10)]
	public GameObject CenterObj;//Center Object for the rotation calculation
	public GameObject CenterObjGol;//Center gol Object for the rotation calculation

	[Space(10)]
	public GameObject[] OperationObj;//object to rotate

	[Space(10)]
	public GameObject LookTargetObj;//The line of sight of the target

	[Space(10)]
	public bool TargetLook = true;//Target Look On Off

	[Space(10)]
	public float LookRotSpeed = 0.01f;//Rotation speed
	public float AddRoll = 1f;//Rotation amount correction
	public bool FreeRoll = false;//animation complements
	public bool ResetRoll = true;//Return the rotation or more rotation limit to basic
	public bool RateDecline = true;//Rotation speed decline

	[Space(10)]
	public Vector3 OffsetTarget = Vector3.zero;//Adjustment of the target position

	[Space(10)]
	public bool FixX = false; //Fixed axis specification
	public bool FixY = false;
	public bool FixZ = false;
	public Vector3 MaxRange = new Vector3(0f, 0f, 0f);//maximum
	public Vector3 MiniRange = new Vector3(0f, 0f, 0f);//minimum

	private Vector3 oldEyeTgt, oldPvd;
	private Vector3[] headRot;
	private float headrotAng, sysTime, fpsTime, outDot;
	private Quaternion[] OldHeadAng, OldHRA, trqd;
	private Quaternion OldAng, OldRA, ctrqd;
	private Vector3 ltpos;

	public void Awake()
	{
		this.Init();
	}

	public void Start()
	{
		this.sysTime = Time.fixedDeltaTime;
	}

	private void Init()
	{
		this.sysTime = Time.fixedDeltaTime;
		this.fpsTime = 1f;

		this.headrotAng = 0f;//For rotation
		this.oldPvd = Vector3.zero;//Temporarily stored in the rotation
		this.outDot = 0f;//120 Either side of the limit outside

		this.OldRA = Quaternion.identity;
		this.ctrqd = Quaternion.identity;
		this.OldAng = Quaternion.identity;
		if(this.CenterObj != null)
		{
			this.OldAng = this.CenterObj.transform.localRotation;
		}

		if (this.LookTargetObj != null)
		{
			//Control the offset value in the angle of view
			float pd = 1f;
			if(this.RootObj != null && this.CameraObj != null)
			{
				float dist = Vector3.Distance(
				this.LookTargetObj.transform.position, this.RootObj.transform.position);
				pd = Mathf.Tan(this.CameraObj.fieldOfView * Mathf.Deg2Rad) * dist;
			}

			this.ltpos = this.LookTargetObj.transform.InverseTransformDirection(this.OffsetTarget * pd);

			//1 frame before the eyes of the target position
			this.oldEyeTgt = this.LookTargetObj.transform.position + this.ltpos;
		}
		else
		{
			this.oldEyeTgt = Vector3.zero;
		}

		if (this.OperationObj != null)
		{
			this.OldHeadAng = new Quaternion[this.OperationObj.Length];
			this.headRot = new Vector3[this.OperationObj.Length];
			this.OldHRA = new Quaternion[this.OperationObj.Length];
			this.trqd = new Quaternion[this.OperationObj.Length];

			for(int i = 0; i < this.OperationObj.Length; i++)
			{
				this.OldHeadAng[i] = this.OperationObj[i].transform.localRotation;
				this.headRot[i] = Vector3.zero;
				this.OldHRA[i] = Quaternion.identity;//Angle at the start of
				this.trqd[i] = Quaternion.identity;
			}
		}
	}

	public void LateUpdate()
	{
		if (this.LookTargetObj != null && this.OperationObj != null)
		{
			if(this.sysTime != 0f && Time.deltaTime != 0f)
			{
				this.fpsTime = Time.deltaTime / this.sysTime;
			}

			//Control the offset value in the angle of view
			float pd = 1f;
			if(this.RootObj != null && this.CameraObj != null)
			{
				float dist = Vector3.Distance(
				this.LookTargetObj.transform.position, this.RootObj.transform.position);
				pd = Mathf.Tan(this.CameraObj.fieldOfView * Mathf.Deg2Rad) * dist;
			}

			this.ltpos = this.LookTargetObj.transform.InverseTransformDirection(this.OffsetTarget * pd);

			//Initialization After moving eyes target
				if (this.LookTargetObj.transform.position + this.ltpos != this.oldEyeTgt)
			{
				this.ReHeadRotationSet();
				this.oldEyeTgt = this.LookTargetObj.transform.position + this.ltpos;
			}

			this.TargetPosSet();
		}
	}

	private void ReHeadRotationSet()
	{
		//The target moves during completion
		if (this.OperationObj != null)
		{
			if (this.FreeRoll == true)
			{
				this.headrotAng = 1f;
			}
			else
			{
				this.headrotAng = 0f;
			}

			this.OldRA = this.ctrqd;
		}
	}

	private void TargetPosSet()
	{
		//Calculating the target angle only once
		if(this.CenterObj != null && this.CenterObjGol != null)
		{
			//Limits specified
			Vector3 lhp = this.CenterObj.transform.InverseTransformPoint(
			                  this.LookTargetObj.transform.position + this.ltpos);
			Vector3 ltp = this.CenterObj.transform.InverseTransformPoint
					(this.CenterObjGol.transform.position);

			Vector3 twpd = this.LookTargetObj.transform.position + this.ltpos;//target
			float thang = Vector3.Angle(lhp, ltp);
			Vector3 qvd = Vector3.zero;

			Vector3 rlv = this.RootObj.transform.right;
			Vector3 rtv = ((this.LookTargetObj.transform.position + this.ltpos)
				- this.RootObj.transform.position).normalized;

			float lad = Vector3.Dot(rlv, rtv);
			bool remf = false;
			if(thang >= 120f)
			{
				if(this.outDot == 0f)//Interior angle when it entered out of range
				{
					this.outDot = lad;
				}

				if(this.ResetRoll == false)
				{
					if(this.outDot < 0f && lad > 0f)
					{
						this.ReHeadRotationSet();
						remf = true;
					}
					if(this.outDot > 0f && lad < 0f)
					{
						this.ReHeadRotationSet();
						remf = true;
					}
				}
				else
				{
					this.ReHeadRotationSet();
				}

			}

			if (thang < 120f)
			{
				qvd = this.PosRotSet(this.CenterObj.transform.position,
				                              twpd,
				                              this.CenterObjGol.transform.position,
				                              this.CenterObj.transform);

				this.oldPvd = qvd;
				this.outDot = 0f;
			}
			else
			{
				if(this.ResetRoll == false)
				{
					if(remf == true)
					{
						qvd = Vector3.zero;
					}
					else
					{
						qvd = this.oldPvd;
					}
				}
				else
				{
					qvd = Vector3.zero;
				}
			}

			if(this.TargetLook == false)
			{
				this.ReHeadRotationSet();
				qvd = Vector3.zero;
			}

			Quaternion headqd = Quaternion.Euler(qvd * this.AddRoll);

			//Rotate over time
			if (this.headrotAng >= 0f && this.headrotAng < 1f)
			{
				float vd = 1f;
				if(this.RateDecline == true)
				{
					//Interior angle to the target
					Vector3 ogv = this.CenterObjGol.transform.position
								 - this.CenterObj.transform.position;

					Vector3 otv = twpd - this.CenterObj.transform.position;

					vd = 1f - Mathf.Abs(Vector3.Dot(ogv.normalized, otv.normalized));
				}

				float adds = (this.LookRotSpeed * vd) * 0.3f;
				this.headrotAng += ((this.LookRotSpeed * 0.7f) + adds) * this.fpsTime;
			}

			this.headrotAng = Mathf.Clamp(this.headrotAng, 0f, 1f);

			if (this.FreeRoll == true)
			{
				this.headrotAng = 1f;
			}

			Quaternion adq = Quaternion.Lerp(this.OldRA, headqd, this.headrotAng);//Complement

			this.ctrqd = adq;

			adq = this.CenterObj.transform.localRotation * adq;//Basic data

			Quaternion fqd = this.fixingSet(adq, this.OldAng);//Range specification

			this.CenterObj.transform.localRotation = fqd;

			for(int i = 0; i < this.OperationObj.Length; i++)
			{
				this.OperationObj[i].transform.localRotation = fqd;
			}
		}
	}

	private Quaternion fixingSet(Quaternion qd, Quaternion oqd)
	{
		//Rotation suppressing
		Quaternion rotDate = Quaternion.identity;

		if (this.FixX == true || this.FixY == true || this.FixZ == true)
		{
			//Fixed base local angle
			Vector3 oqav = oqd.eulerAngles;
			//Input angle
			Vector3 rdv = Vector3.zero;
			rdv = qd.eulerAngles;

			rdv.x %= 360f;
			rdv.y %= 360f;
			rdv.z %= 360f;

			//Range specification
			Vector3 cpad  = Vector3.zero;
			Vector3 nca = Vector3.zero;
			Vector3 psd = Vector3.zero;

			//Current offset angle in the range basic 0
			Vector3 nowa = rdv - oqav;

			nowa.x %= 360f;
			nowa.y %= 360f;
			nowa.z %= 360f;

			if (nowa.x > 180f)
			{
				nowa.x -= 360f;
			}
			if (nowa.y > 180f)
			{
				nowa.y -= 360f;
			}
			if (nowa.z > 180f)
			{
				nowa.z -= 360f;
			}

			if (nowa.x < -180f)
			{
				nowa.x += 360f;
			}
			if (nowa.y < -180f)
			{
				nowa.y += 360f;
			}
			if (nowa.z < -180f)
			{
				nowa.z += 360f;
			}

			//x-----------------------------
			float xd0 = nowa.x;
			if(nowa.y == 180f && nowa.z == 180f)
			{
				if(nowa.x < 0)
				{
					xd0 = -90f + (-90f - nowa.x);
				}
				else
				{
					xd0 = 90f + (90f - nowa.x);
				}
			}

			if(nowa.x != 0f && nowa.x != 180f && nowa.x != -180f)
			{
				nca.x = Mathf.Clamp(xd0, this.MiniRange.x, this.MaxRange.x);

				float xd1 = 0f;
				if(nowa.y == 180f && nowa.z == 180f)
				{
					if(nowa.x < 0)
					{
						xd1 = -90 + (-90f - nca.x);
					}
					else
					{
						xd1 = 90f + (90f - nca.x);
					}

					nca.x = xd1;
				}
			}
			else
			{
				nca.x = nowa.x;
			}

			psd.x = nca.x + oqav.x;
			cpad.x = (psd.x < 0f) ? psd.x + 360f : psd.x;//0~360に戻す
			cpad.x %= 360f;

			//y---------------------------
			float yd0 = nowa.y;
			if(nowa.x == 180f && nowa.z == 180f)
			{
				if(nowa.y < 0)
				{
					yd0 = -90f + (-90f - nowa.y);
				}
				else
				{
					yd0 = 90f + (90f - nowa.y);
				}
			}

			if(nowa.y != 0f && nowa.y != 180f && nowa.y != -180f)
			{
				nca.y = Mathf.Clamp(yd0, this.MiniRange.y, this.MaxRange.y);

				float yd1 = 0f;
				if(nowa.x == 180f && nowa.z == 180f)
				{
					if(nowa.y < 0)
					{
						yd1 = -90 + (-90f - nca.y);
					}
					else
					{
						yd1 = 90f + (90f - nca.y);
					}

					nca.y = yd1;
				}
			}
			else
			{
				nca.y = nowa.y;
			}

			psd.y = nca.y + oqav.y;
			cpad.y = (psd.y < 0f) ? psd.y + 360f : psd.y;//0~360に戻す
			cpad.y %= 360f;

			//z----------------------------
			float zd0 = nowa.z;
			if(nowa.x == 180f && nowa.y == 180f)
			{
				if(nowa.z < 0)
				{
					zd0 = -90f + (-90f - nowa.z);
				}
				else
				{
					zd0 = 90f + (90f - nowa.z);
				}
			}

			if(nowa.z != 0f && nowa.z != 180f && nowa.z != -180f)
			{
				nca.z = Mathf.Clamp(zd0, this.MiniRange.z, this.MaxRange.z);

				float zd1 = 0f;
				if(nowa.x == 180f && nowa.y == 180f)
				{
					if(nowa.z < 0)
					{
						zd1 = -90 + (-90f - nca.z);
					}
					else
					{
						zd1 = 90f + (90f - nca.z);
					}

					nca.z = zd1;
				}
			}
			else
			{
				nca.z = nowa.z;
			}

			psd.z = nca.z + oqav.z;
			cpad.z = (psd.z < 0f) ? psd.z + 360f : psd.z;//0~360に戻す
			cpad.z %= 360f;

			//Fixed specified confirmation
			if (this.FixX == false)
			{
				cpad.x = rdv.x;
			}
			if (this.FixY == false)
			{
				cpad.y = rdv.y;
			}
			if (this.FixZ == false)
			{
				cpad.z = rdv.z;
			}

			rotDate = Quaternion.Euler(cpad);
		}
		else
		{
			rotDate = qd;
		}

		return rotDate;
	}

	private Vector3 PosRotSet(Vector3 od, Vector3 md, Vector3 td, Transform tra)
	{
		//It returns the rotation Euler angle
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
}
