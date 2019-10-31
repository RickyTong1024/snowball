using UnityEngine;
using System.Collections;

//GameObjectをターゲット方向へ回転させる 20170703
public class TargetLookSet_j : MonoBehaviour {

	public GameObject RootObj;//位置回転基本Object
	public Camera CameraObj;//レンダリング用カメラ

	[Space(10)]
	public GameObject CenterObj;//回転計算用中央Object
	public GameObject CenterObjGol;//回転計算用中央GolObject

	[Space(10)]
	public GameObject[] OperationObj;//回転させるobject

	[Space(10)]
	public GameObject LookTargetObj;//視線ターゲット

	[Space(10)]
	public bool TargetLook = true;//Target Look On Off

	[Space(10)]
	public float LookRotSpeed = 0.01f;//回転スピード
	public float AddRoll = 1f;//回転量補正
	public bool FreeRoll = false;//リアルタイムにターゲットを見るか？自動アニメ補完するか
	public bool ResetRoll = true;//回転限界以上で回転を基本に戻すか？
	public bool RateDecline = true;//回転速度減退

	[Space(10)]
	public Vector3 OffsetTarget = Vector3.zero;//ターゲット位置の調整

	[Space(10)]
	public bool FixX = false;//固定軸指定
	public bool FixY = false;
	public bool FixZ = false;
	public Vector3 MaxRange = new Vector3(0f, 0f, 0f);//maximum軸回転固定
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
		this.sysTime = Time.fixedDeltaTime;//負荷測定用
	}

	private void Init()
	{
		this.sysTime = Time.fixedDeltaTime;
		this.fpsTime = 1f;

		this.headrotAng = 0f;//回転用
		this.oldPvd = Vector3.zero;//回転オイラー一時保管
		this.outDot = 0f;//120制限外のどちら側か

		this.OldRA = Quaternion.identity;
		this.ctrqd = Quaternion.identity;
		this.OldAng = Quaternion.identity;
		if(this.CenterObj != null)
		{
			this.OldAng = this.CenterObj.transform.localRotation;
		}

		if (this.LookTargetObj != null)
		{
			//画角でオフセット値を制御
			float pd = 1f;
			if(this.RootObj != null && this.CameraObj != null)
			{
				float dist = Vector3.Distance(
				this.LookTargetObj.transform.position, this.RootObj.transform.position);
				pd = Mathf.Tan(this.CameraObj.fieldOfView * Mathf.Deg2Rad) * dist;
			}

			this.ltpos = this.LookTargetObj.transform.InverseTransformDirection(this.OffsetTarget * pd);

			//1フレーム前の目線ターゲット位置
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
				this.OldHRA[i] = Quaternion.identity;//開始時の角度
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

			//画角でオフセット値を制御
			float pd = 1f;
			if(this.RootObj != null && this.CameraObj != null)
			{
				float dist = Vector3.Distance(
				this.LookTargetObj.transform.position, this.RootObj.transform.position);
				pd = Mathf.Tan(this.CameraObj.fieldOfView * Mathf.Deg2Rad) * dist;
			}

			this.ltpos = this.LookTargetObj.transform.InverseTransformDirection(this.OffsetTarget * pd);

			//目線ターゲットが動いたら初期化
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
		//補完中にターゲットが動いた
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
		//ターゲット角度を一度だけ算出
		if(this.CenterObj != null && this.CenterObjGol != null)
		{
			//180度限界範囲指定
			Vector3 lhp = this.CenterObj.transform.InverseTransformPoint(
			                  this.LookTargetObj.transform.position + this.ltpos);
			Vector3 ltp = this.CenterObj.transform.InverseTransformPoint
					(this.CenterObjGol.transform.position);

			Vector3 twpd = this.LookTargetObj.transform.position + this.ltpos;//ターゲット
			float thang = Vector3.Angle(lhp, ltp);
			Vector3 qvd = Vector3.zero;

			Vector3 rlv = this.RootObj.transform.right;
			Vector3 rtv = ((this.LookTargetObj.transform.position + this.ltpos)
				- this.RootObj.transform.position).normalized;

			float lad = Vector3.Dot(rlv, rtv);
			bool remf = false;
			if(thang >= 120f)
			{
				if(this.outDot == 0f)//範囲外に入った時の内角
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

			//時間をかけて回転させる
			if (this.headrotAng >= 0f && this.headrotAng < 1f)
			{
				float vd = 1f;
				if(this.RateDecline == true)
				{
					//ターゲットまでの内角
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

			Quaternion adq = Quaternion.Lerp(this.OldRA, headqd, this.headrotAng);//アニメ補完

			this.ctrqd = adq;

			adq = this.CenterObj.transform.localRotation * adq;//基本データ

			Quaternion fqd = this.fixingSet(adq, this.OldAng);//範囲指定

			this.CenterObj.transform.localRotation = fqd;

			for(int i = 0; i < this.OperationObj.Length; i++)
			{
				this.OperationObj[i].transform.localRotation = fqd;
			}
		}
	}

	private Quaternion fixingSet(Quaternion qd, Quaternion oqd)
	{
		//回転抑制
		Quaternion rotDate = Quaternion.identity;

		if (this.FixX == true || this.FixY == true || this.FixZ == true)
		{
			//固定基礎ローカル角度
			Vector3 oqav = oqd.eulerAngles;
			//入力角度
			Vector3 rdv = Vector3.zero;
			rdv = qd.eulerAngles;

			rdv.x %= 360f;
			rdv.y %= 360f;
			rdv.z %= 360f;

			//範囲指定
			Vector3 cpad  = Vector3.zero;
			Vector3 nca = Vector3.zero;
			Vector3 psd = Vector3.zero;

			//現在角度を範囲基本0にオフセット
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

			//固定指定確認
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
		//回転オイラー角度を返す
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
