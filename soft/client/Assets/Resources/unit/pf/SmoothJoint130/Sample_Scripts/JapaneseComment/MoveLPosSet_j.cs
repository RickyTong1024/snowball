using UnityEngine;
using System.Collections;

//GameObjectをローカルで移動させる 20170703
public class MoveLPosSet_j : MonoBehaviour {

	public bool ChildAutoSetOn = false;//子階層自動登録
	public GameObject RootObj;//階層検索を始めるroot親

	[Space(10)]
	public GameObject[] JointObj;//移動させるObject

	[Space(10)]
	public SmoothJoint SmoothJointObj;//SmoothJoint
	public SmoothJointCatch SJCatchObj;//SmoothJointCatch

	[Space(10)]
	public bool StartMPS = false;//開始
	public bool MovePower = false;//移動速度をRoot変化させる

	[Space(10)]
	public Vector3 MoveVec;//移動方向ベクトル
	public float ShotMoveSpeed = 0.2f;//移動スピード
	public float ReturnMoveSpeed = 0.005f;//戻り移動スピード
	public float EndRange = 1f;//終了移動距離

	private Vector3[] basePos;
	private Quaternion[] baseRot;
	private float sysTime, fpsTime, vecData, powd, movRes, sjvpd, gravity;
	private bool setSJ, hitIK;

	public void OnValidate()
	{
		this.ChildAutoSet();
	}

	public void Awake()
	{
		this.Init();
	}

	private void Init()
	{
		//初期化
		if(this.JointObj != null)
		{
			this.sysTime = Time.fixedDeltaTime;//負荷測定用
			this.fpsTime = 1f;
			this.vecData = 0f;//移動データ
			this.setSJ = false;//SmoothJoint設定開始
			this.sjvpd = 0f;//加算するsjの値
			this.hitIK = false;//IK起動確認

			if(this.SmoothJointObj != null)
			{
				//SmoothJoint調整
				if(this.SmoothJointObj != null)
				{
					this.movRes = this.SmoothJointObj.MovingResistance;
					this.gravity = this.SmoothJointObj.Gravity;

					this.SmoothJointObj.MovingResistance = 0f;

					if(this.SmoothJointObj.GravityOn == true)
					{
						this.SmoothJointObj.GravityOn = false;
					}
				}
			}

			this.basePos = new Vector3[this.JointObj.Length];
			this.baseRot = new Quaternion[this.JointObj.Length];

			for (int i = 0; i < this.JointObj.Length; i++)
			{
				this.basePos[i] = this.JointObj[i].transform.localPosition;
				this.baseRot[i] = this.JointObj[i].transform.localRotation;
			}

			if(this.JointObj.Length > 0f)
			{
				this.powd = 1f / this.JointObj.Length;
			}
		}
	}

	public void Start()
	{
		this.sysTime = Time.fixedDeltaTime;
	}

	public void ChildAutoSet()
	{
		//自動階層登録処理

		if (this.ChildAutoSetOn == true)
		{
			if (this.RootObj == null)
			{
				Debug.Log ("Please set it up: RootObj");

				this.ChildAutoSetOn = false;
			}
			else
			{
				//階層の子供の合計数
				int cnd = 0;
				foreach (Transform tra in this.RootObj.GetComponentsInChildren<Transform>())
				{
					if (tra == null)
					{
						break;
					}

					cnd++;
				}

				if (cnd == 1)
				{
					System.Array.Resize(ref this.JointObj, 1);

					this.JointObj[0] = this.RootObj;

					Debug.Log ("Child not found");
				}
				else
				{
					//配列初期化
					if (this.JointObj != null)
					{
						System.Array.Resize(ref this.JointObj, cnd);
					}
					else
					{
						this.JointObj = new GameObject[cnd];
					}

					//配列に登録処理
					int tnd = 0;
					foreach (Transform tra in this.RootObj.GetComponentsInChildren<Transform>())
					{

						if (this.JointObj != null && this.JointObj.Length > tnd)
						{
							this.JointObj[tnd] = tra.gameObject;

							Debug.Log ("Please proofread. / Array No= " + tnd +
							           " / Object Name= " + this.JointObj[tnd].name);
						}

						tnd++;
					}
				}
			}

			this.ChildAutoSetOn = false;
		}
	}


	public void Update()
	{
		if(this.JointObj != null)
		{
			if(this.sysTime != 0f && Time.deltaTime != 0f)
			{
				this.fpsTime = Time.deltaTime / this.sysTime;
			}

			if(this.SmoothJointObj != null && this.SmoothJointObj.RangeHitIK != null)
			{
				bool[] sikf = this.SmoothJointObj.RangeHitIK;
				for (int i = 0; i < sikf.Length; i++)
				{
					if(sikf[i] == true)
					{
						this.hitIK = true;
					}
					else
					{
						this.hitIK = false;
					}
				}
			}

			if(this.StartMPS == true)
			{
				this.MoveObjSet(true);

				//SmoothJoint設定
				if(this.SmoothJointObj != null && this.setSJ == true)
				{
					this.SJDataSet(1);
				}
			}
			else
			{
				this.MoveObjSet(false);
			}
		}
	}

	private void SJDataSet(int sef)
	{
		//SmoothJoint設定

		float ikg = 1f;

		switch (sef)
		{
		case 0:
			//初期化
			if(this.SmoothJointObj.GravityOn == true)
			{
				this.SmoothJointObj.GravityOn = false;
			}
			if(this.setSJ == true)
			{
				this.setSJ = false;
			}

			if(this.SJCatchObj != null)
			{
				this.SJCatchObj.CatchAllClear();
			}

			this.sjvpd = 0f;
			break;

		case 1:
			//開始
			if(this.SmoothJointObj.GravityOn == false)
			{
				this.SmoothJointObj.GravityOn = true;
			}

			if(this.hitIK == false)
			{
				if(this.sjvpd < 1f)
				{
					this.sjvpd += this.ShotMoveSpeed * this.fpsTime;

					this.sjvpd = Mathf.Clamp(this.sjvpd, 0f, 1f);
				}
			}
			else
			{
				this.sjvpd = 1f;

				ikg = 0.3f;
			}
			break;

		case 2:
			//終了
			if(this.sjvpd > 0f)
			{
				this.sjvpd -= (this.ShotMoveSpeed * 0.5f) * this.fpsTime;

				this.sjvpd = Mathf.Clamp(this.sjvpd, 0f, 1f);

				ikg *= this.sjvpd;
			}
			break;
		}

		this.SmoothJointObj.MovingResistance = this.movRes * this.sjvpd;
		this.SmoothJointObj.Gravity = (this.gravity * ikg) * this.sjvpd;
	}

	private void MoveObjSet(bool sef)
	{
		//移動処理

		if(sef == true)
		{
			//開始
			if(this.hitIK == false)
			{
				if(this.EndRange > this.vecData)
				{
					this.vecData += this.ShotMoveSpeed * this.fpsTime;
				}
				else
				{
					if(this.setSJ == false)
					{
						this.setSJ = true;
					}
				}
			}
			else
			{
				if(this.setSJ == false)
				{
					this.setSJ = true;
				}
			}

		}
		else
		{
			//終了
			if(this.vecData > 0f)
			{
				this.vecData -= this.ReturnMoveSpeed * this.fpsTime;

				if(this.SmoothJointObj != null)
				{
					this.SJDataSet(2);
				}
			}
		}

		if(this.vecData != 0f && this.vecData < 0.0003f)
		{
			this.vecData = 0f;

			if(this.setSJ == true)
			{
				this.setSJ = false;
			}

			if(this.SmoothJointObj != null)
			{
				this.SJDataSet(0);
			}
		}

		this.vecData = Mathf.Clamp(this.vecData, 0f, this.EndRange);

		for (int i = 0; i < this.JointObj.Length; i++)
		{
			int ai = (this.JointObj.Length - i) - 1;

			Vector3 sjhd = Vector3.zero;
			if(this.SmoothJointObj != null && this.SmoothJointObj.HitPosition != null
				&& this.SmoothJointObj.HitPosition.Length > i)
			{
				sjhd = this.SmoothJointObj.HitPosition[i];
			}

			if(ai < this.JointObj.Length && ai >= 0)
			{
				//移動
				if(this.MovePower == true)
				{
					//移動距離調整
					if(ai > 0 && sjhd == Vector3.zero)
					{
						float pd = this.powd * ai;

						this.JointObj[ai].transform.localPosition = this.basePos[ai] +
							((this.MoveVec * this.vecData) * pd);
					}
				}
				else
				{
					//等間隔移動
					if(ai > 0 && sjhd == Vector3.zero)
					{
						this.JointObj[ai].transform.localPosition = this.basePos[ai] +
							(this.MoveVec * this.vecData);
					}
				}

				//回転
				if(sef == false && this.vecData > 0f && this.EndRange != 0f)
				{
					float red = Mathf.Abs(this.vecData / this.EndRange);
					if(red < 0.001f)
					{
						red = 0f;
					}
					red = Mathf.Clamp( red, 0f, 1f);

					if(red < 0.2f && sjhd == Vector3.zero)
					{
						if(this.SmoothJointObj != null && this.SmoothJointObj.GravityOn == true)
						{
							this.SmoothJointObj.GravityOn = false;
						}

						Quaternion rotd = Quaternion.identity;

						red *= 5f;

						rotd = Quaternion.Lerp(this.baseRot[ai],
						 this.JointObj[ai].transform.localRotation, red);

						this.JointObj[ai].transform.localRotation = rotd;
					}
				}

				//初期化
				if(this.vecData < 0.0003f)
				{
					this.JointObj[ai].transform.localPosition = this.basePos[ai];
					this.JointObj[ai].transform.localRotation = this.baseRot[ai];
				}
			}
		}
	}
}
