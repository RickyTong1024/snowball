using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

//GameObjectの階層構造運動制御 ver 1.3.2 20170709 (使用する場合はpublic class SmoothJoint_jの_jを削除)
public class SmoothJoint_j : MonoBehaviour {

	[Header("- Initial setup ------------------------")]
	public bool ChildAutoSetOn = false;//子階層自動登録

	[Space(10)]
	public GameObject RootObj;//階層検索を始めるroot親
	public GameObject[] JointObj;//階層GameObject

	[Space(10)]
	public GameObject Centroid;//重心

	[Header("- Collider setup ----------------------")]
	public bool ColliderAutoSetOn = false;//collider登録自動登録

	[Space(10)]
	public GameObject[] ColliderRootObj;//当たり判定を登録するrootオブジェクト

	[Header("- Collider setting --------------------")]
	public bool JointCollisionOn = true;//ジョイントの衝突処理OnOff

	[Header("- Collision Radius will be rewritten -")]
	public bool RadiusAllOn = false;//一括半径指定OnOff
	public float CollisionRadiusAll = 0.005f;//一括半径指定

	[Header("---------------------------------------")]
	public float[] CollisionRadius;//ジョイントの衝突半径

	[Header("---------------------------------------")]
	public float SpeedCRange = 0.05f;//移動の速さの処理切り替えスピード(数値が小さいほど敏感)
	public float AddRadius = 1f;//早い移動時の衝突半径に乗算何倍にするか

	[Header("- Group setup ------------------------")]
	public bool GroupAutoSetOn = false;//平均化データ自動登録

	[Space(10)]
	public GameObject[] SearchSJO;//検索先SmoothJoint適用オブジェクト
	public GameObject NexToSJ_0;//移動平均化の為の隣のSJルートオブジェクト
	public GameObject NexToSJ_1;

	[Space(10)]
	public SmoothJoint[] NexToSJData = new SmoothJoint[2];//平均化参照するSJ

	[Header("- Group setting -----------------------")]
	public bool GroupOn = false;//連携OnOff
	public bool GCollisionOn = false;//連携衝突処理
	public float AddGroup = 0.4f;//平均化加算率
	public float OffsetGroup = 1f;//位置調整

	[Header("- IK setup ----------------------------")]
	public bool FixedTargetOn = false;//末端を固定するOnOff
	public Transform[] FixedTarget;//末端を固定するときの目標オブジェクト
	public int[] FixedJointNo;//末端固定するJointObj配列番号

	[Header("- IK setting --------------------------")]
	public bool[] IKRangeOn;//IKをゴールとの距離で起動OnOff

	[Space(10)]
	public float IKRange = 0.1f;//IKを起動させる範囲
	public float AddIKSpring = 0.1f;//IK揺れ乗算何倍にするか
	public bool IKRangeDecline = true;//IK拘束力を距離で減退

	[Header("- Operation setup ---------------------")]
	public bool FixRoot = true;//rootの処理ON,OFF設定
	private bool DelayCorrection = true;//処理落補正ON,OFF設定

	[Header("- Effect setting ----------------------")]
	public float AddEffect = 1f;//エフェクト変型割合(0~1)
	public float MovingResistance = 1f;//耐える(0~1)
	public float AddRectify = 0f;//変型割合をroot近づくほどゆるくする(0~1 1で補正最大)
	public float CollisionFriction = 0f;//衝突中の摩擦(0~1 1でエフェクト値 0)

	[Header("---------------------------------------")]
	public bool TwistRestoreX = false;//ねじれ解消回転
	public bool TwistRestoreY = false;
	public bool TwistRestoreZ = false;
	public float TwistRestoreSpeed = 300f;//ねじれ解消スピード(0~)

	[Header("---------------------------------------")]
	public bool FixX = false; //固定軸指定
	public bool FixY = false;
	public bool FixZ = false;
	public Vector3 MaxRange = new Vector3(0f, 0f, 0f);//軸回転固定
	public Vector3 MiniRange = new Vector3(0f, 0f, 0f);//軸回転固定

	[Header("- Wind --------------------------------")]
	public bool WindOn = false;//風エフェクトOnOff
	public Vector3 Wind = new Vector3(0f, 0f, 0f);//風エフェクト
	public Vector3 RandWind = new Vector3(0f, 0f, 0f);//風の乱数(0~1 1で補正最大)
	public Vector3 WindPendulum = new Vector3(0.1f, 0.1f, 0.1f);//風の揺れ返し量
	public float WindSpeed = 0f;//風の速度

	[Header("- Gravity -----------------------------")]
	public bool GravityOn = true;//重力処理OnOff
	public float Gravity = -0.01635f;//1フレームあたりの重力

	[Space(10)]
	public bool InertiaOn = true;//慣性処理OnOff
	public float AddInertia = 1f;//慣性乗算何倍にするか
	public float InertiaSpeed = 0.1f;//慣性速度(0~1)
	public float InertiaFriction = 0.91f;//慣性減退(0~1)

	[Header("- Centrifugal Force -------------------")]
	public bool CentrifugalForceOn = true;//遠心力処理OnOff
	public float CentrifugalForce = 1f;//遠心力

	[Header("- Spring ------------------------------")]
	public bool SpringOn = false;//スプリング処理OnOff
	public Vector3 AddSpring = new Vector3(0f, 0f, 0f);//スプリング力乗算何倍にするか
	public float SpringHardness = 0.1f;//スプリング硬さ(0~1)
	public float SpringFriction = 0.95f;//スプリング減退(0~1)

	[Header("- Move And Scale setting -------------")]
	public bool MoveAndScaleOn = false;//移動スケール
	public Vector3 AddMove = new Vector3(0f, 0f, 0f);//移動振り幅を何倍にするか
	public Vector3 AddScale = new Vector3(0f, 0f, 0f);//スケール変型幅を何倍にするか

	[Header("- Pressure ---------------------------")]
	public bool PressureOn = false;//圧力
	public float AddPressure = 0f;//圧力乗算何倍にするか
	public float AddPressureGravity = 1f;//重力圧力乗算何倍にするか
	public float AddImpact = 0f;//衝撃力乗算何倍にするか

	[Header("- It is for data confirmation ---------")]

#if UNITY_EDITOR
	public Color colliderColor = new Color(0f, 0.7f, 1f, 1f);//コリジョン色
	public bool ColliderAllDisplay = false;//コリジョン表示OnOff
#endif

	[Space(10)]
	public List<SmoothColliderClass> CollList;//当たり判定用

	public Vector3[] HitPosition
	{
		get
		{
			this.EOperation = true;
			return this.HItFD;
		}
		set
		{
			this.HItFD = value;
		}
	}

	public SmoothColliderClass HitSCC
	{
		get
		{
			this.EOperation = true;
			return this.HitGObject;
		}
		set
		{
			this.HitGObject = value;
		}
	}

	public bool[] RangeHitIK
	{
		get {return this.IKRF;}
	}

	private Quaternion[] OldAng;
	private float[] TargeLength, IKtgl, Jvdot, JOvdot;
	private bool[] IKRF;
	private Vector3[] COPos, VIKPos, OldPos, OldScale, IneVPos, EOffP, HItFD;
	private SmoothColliderClass HitGObject;

	private Vector3 rand, rana, sptOldPos, moveData, SVPos, STPos,
	        STIKPos, SVIKPos, moveIKonPos, STHPos, SVHPos, SVData, MASVs, HSVData,
	        addrv0, addrv1;

	private List<int> fixJointNoList;//入力された配列番号確認用
	private int Oroot, hsjn;
	private bool SpeedF, IKSstart, EOperation;
	private float addfd, sysTime, fpsTime, endfd, MASfs, collAverage, load;

	public void OnValidate()
	{
		//Inspectorの値を変更すると呼ばれる
		this.SJDataReset();
	}

	public void SJDataReset()
	{
		//データリセット

		//子階層自動登録
		this.ChildAutoSet();

		//collider半径自動登録
		if (this.RadiusAllOn == true)
		{
			this.RadiusAllOn = false;

			this.CollRadiusSet();
		}
		//手動で配列数変更されたときの処理
		if (this.CollisionRadius != null && this.JointObj.Length != this.CollisionRadius.Length)
		{
			System.Array.Resize(ref this.CollisionRadius, this.JointObj.Length);
		}
		if (this.CollisionRadius != null && this.JointObj != null)
		{
			this.collAverage = 0f;
			for (int i = 0; i < this.CollisionRadius.Length; i++)
			{
				this.collAverage += this.CollisionRadius[i];
			}
			this.collAverage /= this.JointObj.Length;//jointコリジョン平均サイズ
		}

		//IK固定設定変更処理
		if (this.FixedTarget != null && this.FixedJointNo != null
		        && this.FixedTarget.Length != this.FixedJointNo.Length)
		{
			System.Array.Resize(ref this.FixedJointNo, this.FixedTarget.Length);
		}
		if (this.FixedTarget != null && this.IKRangeOn != null
		        && this.FixedTarget.Length != this.IKRangeOn.Length)
		{
			System.Array.Resize(ref this.IKRangeOn, this.FixedTarget.Length);
		}

		//連携化データセット
		this.GroupSJSet();

		//rootの処理ON,OFF設定
		if (FixRoot == true)
		{
			this.Oroot = 1;
		}
		else
		{
			this.Oroot = 0;
		}

		//自動collider登録処理
		this.ColliderAutoSet();

		//設定値安全補正

		//衝突リスト内にnullが無いか確認
		if (this.CollList != null)
		{
			for (int i = 0; i < this.CollList.Count; i++)
			{
				if (this.CollList[i] == null)
				{
					this.CollList.RemoveAt(i);
				}
			}
		}

		if (this.CollisionFriction < 0f || this.CollisionFriction > 1f)
		{
			Debug.Log ("Collision Friction Range: 0 ~ 1");
			this.CollisionFriction = Mathf.Clamp(this.CollisionFriction, 0f, 1f);
		}

		if (this.InertiaSpeed < 0f || this.InertiaSpeed > 1f)
		{
			Debug.Log ("Inertia Speed Range: 0 ~ 1");
			this.InertiaSpeed = Mathf.Clamp(this.InertiaSpeed, 0f, 1f);
		}

		if (this.InertiaFriction < 0f || this.InertiaFriction > 1f)
		{
			Debug.Log ("Inertia Friction Range: 0 ~ 1");
			this.InertiaFriction = Mathf.Clamp(this.InertiaFriction, 0f, 1f);
		}

		if (this.InertiaFriction < 0f || this.InertiaFriction > 1f)
		{
			Debug.Log ("Inertia Friction Range: 0 ~ 1");
			this.InertiaFriction = Mathf.Clamp(this.InertiaFriction, 0f, 1f);
		}

		if (this.SpringHardness < 0f || this.SpringHardness > 1f)
		{
			Debug.Log ("SpringHardness Range: 0 ~ 1");
			this.SpringHardness = Mathf.Clamp(this.SpringHardness, 0f, 1f);
		}

		if (this.SpringFriction < 0f || this.SpringFriction > 1f)
		{
			Debug.Log ("SpringFriction Range: 0 ~ 1");
			this.SpringFriction = Mathf.Clamp(this.SpringFriction, 0f, 1f);
		}

		if (this.MovingResistance < 0f || this.MovingResistance > 1f)
		{
			Debug.Log ("MovingResistance Range: 0 ~ 1");
			this.MovingResistance = Mathf.Clamp(this.MovingResistance, 0f, 1f);
		}

		if (this.AddEffect < 0f || this.AddEffect > 1f)
		{
			Debug.Log ("AddEffect Range: 0 ~ 1");
			this.AddEffect = Mathf.Clamp(this.AddEffect, 0f, 1f);
		}

		if (this.AddRectify < 0f || this.AddRectify > 1f)
		{
			Debug.Log ("AddRectify Range: 0 ~ 1");
			this.AddRectify = Mathf.Clamp(this.AddRectify, 0f, 1f);
		}

		if (this.MaxRange.x < 0f || this.MaxRange.x > 180f)
		{
			Debug.Log ("MaxRange.x Range: + Plus~ 0 ~ 180");
			this.MaxRange.x = Mathf.Clamp(this.MaxRange.x, 0f, 180f);
		}
		if (this.MaxRange.y < 0f || this.MaxRange.y > 180f)
		{
			Debug.Log ("MaxRange.y Range: + Plus~ 0 ~ 180");
			this.MaxRange.y = Mathf.Clamp(this.MaxRange.y, 0f, 180f);
		}
		if (this.MaxRange.z < 0f || this.MaxRange.y > 180f)
		{
			Debug.Log ("MaxRange.z Range: + Plus~ 0 ~ 180");
			this.MaxRange.z = Mathf.Clamp(this.MaxRange.z, 0f, 180f);
		}

		if (this.MiniRange.x > 0f || this.MiniRange.x < -180f)
		{
			Debug.Log ("MiniRange.x Range: - Minus~ 0 ~ -180");
			this.MiniRange.x = Mathf.Clamp(this.MiniRange.x, -180f, 0f);
		}
		if (this.MiniRange.y > 0f || this.MiniRange.y < -180f)
		{
			Debug.Log ("MiniRange.y Range: - Minus~ 0 ~ -180");
			this.MiniRange.y = Mathf.Clamp(this.MiniRange.y, -180f, 0f);
		}
		if (this.MiniRange.z > 0f || this.MiniRange.z < -180f)
		{
			Debug.Log ("MiniRange.z Range: - Minus~ 0 ~ -180");
			this.MiniRange.z = Mathf.Clamp(this.MiniRange.z, -180f, 0f);
		}
		if (this.RandWind.x < 0f || this.RandWind.x > 1f)
		{
			Debug.Log ("RandWind Range: 0 ~ 1");
			this.RandWind.x = Mathf.Clamp(this.RandWind.x, 0f, 1f);
		}
		if (this.RandWind.y < 0f || this.RandWind.y > 1f)
		{
			Debug.Log ("RandWind Range: 0 ~ 1");
			this.RandWind.y = Mathf.Clamp(this.RandWind.y, 0f, 1f);
		}
		if (this.RandWind.z < 0f || this.RandWind.z > 1f)
		{
			Debug.Log ("RandWind Range: 0 ~ 1");
			this.RandWind.z = Mathf.Clamp(this.RandWind.z, 0f, 1f);
		}
		if (this.FixedTarget != null && this.fixJointNoList != null
		        && this.FixedJointNo != null  && this.FixedTargetOn == true)
		{
			if (this.FixedTarget.Length == 0
			        && this.FixedJointNo.Length == 0 && this.fixJointNoList.Count == 0)
			{
				Debug.Log ("Please set it up: FixedTarget & FixedJointNo");
			}
		}
		if (this.NexToSJData.Length != 2)
		{
			Debug.Log ("You can not change the Array size");
			System.Array.Resize(ref this.NexToSJData, 2);
		}

		if (this.TwistRestoreSpeed < 0f)
		{
			Debug.Log ("TwistRestoreSpeed Range: 0 ~");
			this.TwistRestoreSpeed = 0f;
		}
	}

	public void GroupSJSet()
	{
		//連携化の為の初期化
		if (this.GroupAutoSetOn == true)
		{
			//初期化
			if (this.NexToSJData != null)
			{
				for (int ic = 0; ic < this.NexToSJData.Length; ic++)
				{
					this.NexToSJData[ic] = null;
				}
			}

			if (this.SearchSJO == null)
			{
				Debug.Log ("Please set it up: Search SJO");

				if (this.NexToSJ_0 == null && this.NexToSJ_1 == null)
				{
					Debug.Log ("One of Root Obj is needed: NexToSJ_0 or NexToSJ_1");
				}
			}
			else
			{
				string nexToName_0 = "none";
				string nexToName_1 = "none";
				int nexc = 0;

				if (this.NexToSJ_0 != null)
				{
					nexToName_0 = this.NexToSJ_0.name;
					nexc++;
				}
				if (this.NexToSJ_1 != null)
				{
					nexToName_1 = this.NexToSJ_1.name;
					nexc++;
				}

				if (nexc == 0)
				{
					Debug.Log ("One of Root Obj is needed: NexToSJ_0 or NexToSJ_1");
				}

				Debug.Log ("Please proofread. / Group Target number: " + nexc);

				//ＳＪコンポーネント取得
				if (this.NexToSJData != null && nexc > 0)
				{
					for (int ia = 0; ia < this.SearchSJO.Length; ia++)
					{
						if (this.SearchSJO[ia] != null)
						{
							SmoothJoint[] sjc = this.SearchSJO[ia].GetComponents<SmoothJoint>();

							if (sjc != null)
							{
								for (int i = 0; i < sjc.Length; i++)
								{
									if (sjc[i].JointObj != null && sjc[i].JointObj[0] != null)
									{
										if (sjc[i].JointObj[0].name == nexToName_0)
										{
											this.NexToSJData[0] = sjc[i];

											Debug.Log ("0 / Get SmoothJoint.JointObj.name = " + nexToName_0);
										}
										else
										{
											if (sjc[i].JointObj[0].name == nexToName_1)
											{
												if (nexc == 1)
												{
													if (nexToName_0 == "none")
													{
														this.NexToSJData[0] = sjc[i];

														Debug.Log ("0 / Get SmoothJoint.JointObj.name = " + nexToName_1);
													}
												}
												if (nexc == 2)
												{
													if (nexToName_0 == "none")
													{
														this.NexToSJData[0] = sjc[i];

														Debug.Log ("0 / Get SmoothJoint.JointObj.name = " + nexToName_1);
													}
													if (nexToName_0 != "none")
													{
														this.NexToSJData[1] = sjc[i];

														Debug.Log ("1 / Get SmoothJoint.JointObj.name = " + nexToName_1);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}

			this.GroupAutoSetOn = false;
		}
	}

	public void CollRadiusSet()
	{
		//collider半径自動登録

		//配列数確認
		if (this.JointObj != null)
		{
			if (this.CollisionRadius == null)
			{
				this.CollisionRadius = new float[this.JointObj.Length];
			}
			else
			{
				if (this.JointObj.Length != this.CollisionRadius.Length)
				{
					System.Array.Resize(ref this.CollisionRadius, this.JointObj.Length);
				}
			}

			if (this.CollisionRadius != null)
			{
				for (int i = 0; i < this.CollisionRadius.Length; i++)
				{
					this.CollisionRadius[i] = this.CollisionRadiusAll;
				}

				this.collAverage = this.CollisionRadiusAll;
			}
		}
	}

	public void ChildAutoSet()
	{
		//自動階層登録処理

		if (this.ChildAutoSetOn == true)
		{
			if (this.RootObj == null)
			{
				Debug.Log ("Please set it up: RootObj");
				Debug.Log ("or manual setting: JointObj");

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

				//重心オブジェクトの登録
				this.Centroid = this.RootObj.transform.root.gameObject;
			}

			//初期化
			this.Init();

			this.CollRadiusSet();

			this.ChildAutoSetOn = false;
		}
	}

	private void ColliderAutoSet()
	{
		//自動collider登録処理

		if (this.ColliderAutoSetOn == true)
		{
			if (this.ColliderRootObj != null)
			{
				int sccn = 0;

				//当たり判定用
				this.CollList = new List<SmoothColliderClass>();
				this.CollList.Clear();

				for (int ic = 0; ic < this.ColliderRootObj.Length; ic++)
				{
					foreach (SmoothColliderObj sccl in
					         this.ColliderRootObj[ic].GetComponentsInChildren<SmoothColliderObj>())
					{
						if (sccl.SJCObject != null)
						{
							for (int ci = 0; ci < sccl.SJCObject.Count; ci++)
							{
								if (sccl.SJCObject[ci] != null)
								{
									//SmoothColliderObj番号更新
									sccl.SJCNumber = ic;

									Debug.Log ("SJColliderNumber= " + ic + " / SJCObject.Count= " + ci);

									//Listに登録
									this.CollList.Add(sccl.SJCObject[ci]);

									sccn++;
								}
							}
						}
						else
						{
							Debug.Log ("In this ColliderRootObj not found: SmoothColliderClass");
						}
					}
				}

				if (sccn == 0)
				{
					Debug.Log ("In this ColliderRootObj not found: SmoothColliderObj");
				}
				else
				{
					Debug.Log ("In this ColliderRootObj SmoothColliderObj number: " + sccn + " object");
				}
			}

			if (this.CollList != null)
			{
				//すでに登録済みか確認
				for (int li = 0; li < this.CollList.Count - 1; li++)
				{
					for (int rli = 1 + li; rli < this.CollList.Count; rli++)
					{
						if (this.CollList[li].SJCCNumber == this.CollList[rli].SJCCNumber)
						{
							if (this.CollList[li].SJCCTransform.position
							        == this.CollList[rli].SJCCTransform.position)
							{
								if (this.CollList[li].SJCCTransform.name
								        == this.CollList[rli].SJCCTransform.name)
								{
									//重複削除
									if (this.CollList.Count > rli)
									{
										Debug.Log ("Duplicate Delete: " +
										           this.CollList[rli].SJCCTransform.name);
										this.CollList.RemoveAt(rli);
									}
								}
							}
						}
					}
				}

				Debug.Log ("The number of the total. Coll List number: " + this.CollList.Count + " object");
			}
			this.ColliderAutoSetOn = false;
		}
	}

	private void Init()
	{
		//初期化
		if (this.JointObj != null)
		{
			this.sysTime = Time.fixedDeltaTime;//負荷測定用
			this.fpsTime = 1f;
			this.load = 1f;

			this.EOperation = false;//外部操作

			this.TargeLength = new float[this.JointObj.Length];//親子の長さ
			this.OldAng = new Quaternion[this.JointObj.Length];//基本角度
			this.Jvdot = new float[this.JointObj.Length];//変形角度
			this.JOvdot = new float[this.JointObj.Length];

			this.HitGObject = null;//衝突相手

			this.IKSstart = false;

			this.SVData = Vector3.zero;
			this.endfd = 0f;

			this.MASVs = Vector3.zero;
			this.MASfs = 0f;

			this.HSVData = Vector3.zero;

			this.hsjn = -1;

			if (this.AddSpring != Vector3.zero)
			{
				this.SVPos = Vector3.zero;//スプリングベクトル
				this.STPos = Vector3.zero;//スプリングターゲット
				this.moveData = Vector3.zero;//スプリングエフェクト計算元データ

				this.STIKPos = Vector3.zero;//スプリングIKターゲット
				this.SVIKPos = Vector3.zero;//スプリングIKベクトル
				this.moveIKonPos = Vector3.zero;//IK接触開始座標

				this.STHPos = Vector3.zero;//スプリング衝突揺れターゲット
				this.SVHPos = Vector3.zero;//スプリング衝突揺れベクトル

				if (this.JointObj.Length > 0)
				{
					this.sptOldPos = this.JointObj[0].transform.position;
				}
			}

			this.COPos = new Vector3[this.JointObj.Length];//前フレーム位置座標

			if (this.FixedTarget != null && this.FixedTarget.Length > 0)
			{
				this.VIKPos = new Vector3[this.JointObj.Length];//ベクトルIKの目標座標
			}

			this.HItFD = new Vector3[this.JointObj.Length];//衝突中フラグ
			this.IneVPos = new Vector3[this.JointObj.Length];//慣性計算用
			this.OldPos = new Vector3[this.JointObj.Length];//基本ローカル初期座標

			if (this.AddScale != Vector3.zero)
			{
				this.OldScale = new Vector3[this.JointObj.Length];//基本ローカル初期スケール
			}

			if (this.GroupOn == true)
			{
				this.EOffP = new Vector3[this.JointObj.Length];//平均化オフセット基本位置
				for (int ie = 0; ie < this.EOffP.Length; ie++)
				{
					this.GroupSet(ie, Vector3.zero, true);
				}
			}

			this.rand = new Vector3(1f, 1f, 1f);
			this.rana = Vector3.zero;

			this.SpeedF = false;//移動スピードで警告

			if (this.Centroid == null)
			{
				this.Centroid = this.RootObj.transform.root.gameObject;
			}

			for (int i = 0; i < this.JointObj.Length; i++)
			{
				if (this.FixedTarget != null && this.FixedTarget.Length > 0)
				{
					this.VIKPos[i] = Vector3.zero;
				}

				this.IneVPos[i] = Vector3.zero;
				this.HItFD[i] = Vector3.zero;

				if (this.JointObj[i] != null)
				{
					this.OldAng[i] = this.JointObj[i].transform.localRotation;
					this.COPos[i] = this.JointObj[i].transform.position;
					this.OldPos[i] = this.JointObj[i].transform.localPosition;

					if (this.AddScale != Vector3.zero)
					{
						this.OldScale[i] = this.JointObj[i].transform.localScale;
					}
				}

				if (this.JointObj[i] == null )
				{
					Debug.Log ("Joint Obj not found: " + i);
				}

				if (i == 0)
				{
					this.TargeLength[i] = 0f;//保険初期化
				}

				if (this.JointObj.Length > i + 1)
				{
					if (this.JointObj[i] != null && this.JointObj[i + 1] != null)
					{
						//親子の長さを記憶
						this.TargeLength[i] = Vector3.Distance(
						                          this.JointObj[i].transform.position,
						                          this.JointObj[i + 1].transform.position);

						//重力用
						this.JOvdot[i] = Vector3.Dot((this.JointObj[i + 1].transform.position
						 - this.JointObj[i].transform.position).normalized , Vector3.down);
						this.Jvdot[i] = this.JOvdot[i];
					}
				}
			}

			if (this.JointObj.Length != 0)
			{
				this.addfd = 1f / this.JointObj.Length;
			}
			else
			{
				this.addfd = 1f;
			}

			//IKユーザー指定配列番号確認用
			this.fixJointNoList = new List<int>();
			this.fixJointNoList.Clear();

			if (this.FixedJointNo != null)
			{
				this.IKtgl = new float[this.FixedJointNo.Length];
				this.IKRF = new bool[this.FixedJointNo.Length];

				for (int il = 0; il < this.FixedJointNo.Length; il++)
				{
					this.fixJointNoList.Add(this.FixedJointNo[il]);

					this.IKtgl[il] = 0f;
					this.IKRF[il] = false;
				}

				this.fixJointNoList.Sort();//小さい順に並び替え

				//最大の要素を取得
				int max = 0;

				foreach (int e in this.fixJointNoList)
				{
					if (max < e) max = e;
				}

				//最小の要素を取得
				int min = int.MaxValue;

				foreach (int e in this.fixJointNoList)
				{
					if (e < min) min = e;
				}

				if (this.JointObj != null)
				{
					if (this.JointObj.Length < max || 0 > min)
					{
						//警告メッセージ
						Debug.Log ("FixedJointNo Range outside");
					}
				}
			}
		}
	}

	public void Awake()
	{
		if (FixRoot == true)
		{
			this.Oroot = 1;
		}
		else
		{
			this.Oroot = 0;
		}

		this.Init();
	}

	public void Start()
	{
		this.sysTime = Time.fixedDeltaTime;//負荷測定用
	}

	public void Update()
	{
		if (this.JointObj != null)
		{
			//スプリング座標初期化
			if (this.AddSpring != Vector3.zero)
			{
				if (this.JointObj.Length > 0)
				{
					//IK有効なら
					if (this.FixedTarget != null && this.FixedTarget.Length > 0 && this.FixedTargetOn == true)
					{
						for (int i = 0; i < this.IKRF.Length; i++)
						{
							if (this.IKRF[i] == true)
							{
								if (this.IKSstart == false)
								{
									this.moveIKonPos = this.IKAveragePosSet(true);
									this.STIKPos = Vector3.zero;
									this.SVIKPos = Vector3.zero;
									this.IKSstart = true;
								}
							}
							else
							{
								if (this.IKSstart == true)
								{
									Vector3 mld0 = (this.moveIKonPos - this.IKAveragePosSet(false)).normalized
									               * this.IKRange * 0.5f;

									Vector3 mld1 = this.moveIKonPos - this.IKAveragePosSet(false);

									this.STIKPos = (mld0 * 0.5f) + (mld1 * 0.5f);
									this.IKSstart = false;
								}
							}
						}
					}

					//揺れ終了初期化
					this.moveData = this.JointObj[0].transform.position - this.sptOldPos;
				}
			}

			//ベクトル IK
			if (this.FixedTargetOn == true)
			{
				this.VectorIKSet();
			}
		}
	}

	public void LateUpdate()
	{
		if (this.JointObj != null)
		{
			if (this.DelayCorrection == true)
			{
				if (this.sysTime != 0f && Time.deltaTime != 0f)
				{
					this.fpsTime = Time.deltaTime / this.sysTime;
				}
			}

			this.load = Mathf.Clamp(1f / this.fpsTime, 0f, 1f);

			for (int i = 0; i < this.JointObj.Length; i++)
			{
				if (this.JointObj.Length > i + 1)
				{
					if (this.JointObj[i] != null && this.JointObj[i + 1] != null)
					{
						//親子の長さを記憶
						this.TargeLength[i] = Vector3.Distance(
						                          this.JointObj[i].transform.position,
						                          this.JointObj[i + 1].transform.position);
					}
				}

				this.EDataSet(i);

				if (this.TwistRestoreX == true || this.TwistRestoreY == true || this.TwistRestoreZ == true)
				{
					this.TwistRestoreSet(i);//ねじれ補正
				}

				//オールド座標初期化
				if (this.JointObj[i].transform.position != this.COPos[i])
				{
					this.COPos[i] = this.JointObj[i].transform.position;
				}
			}
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (this.JointObj != null)
		{
			if (this.ColliderAllDisplay == true)
			{
				for (int i = 0; i < this.JointObj.Length; i++)
				{
					if (this.JointObj[i] != null && this.CollisionRadius != null)
					{
						Gizmos.color = this.colliderColor;
						Gizmos.DrawWireSphere(this.JointObj[i].transform.position, this.CollisionRadius[i]);

						if (i > 0 && this.JointObj[i - 1] != null)
						{
							Gizmos.DrawLine(this.JointObj[i - 1].transform.position,
							                this.JointObj[i].transform.position);
						}
					}
				}
			}
		}
	}
#endif

	private void EDataSet(int i)
	{
		//データ入力処理

		if (this.JointObj != null && this.JointObj.Length > i)
		{
			//エフェクト
			Vector3 ddv = Vector3.zero;

			//移動延滞(オールド座標にどのぐらい残るか)
			Vector3 ds = (this.COPos[i] - this.JointObj[i].transform.position);

			ddv.x = ddv.x + ((ds.x - ddv.x) * this.MovingResistance) * this.load;
			ddv.y = ddv.y + ((ds.y - ddv.y) * this.MovingResistance) * this.load;
			ddv.z = ddv.z + ((ds.z - ddv.z) * this.MovingResistance) * this.load;

			ddv = this.WindSet(i, ddv);//風

			ddv = this.CentrifugalSet(i, ddv);//遠心力

			ddv = this.SpringSet(i, ddv);//スプリング

			ddv = this.GravitySet(i, ddv);//重力

			if(this.HItFD.Length > i)
			{
				if (this.GroupOn == true && this.HItFD[i] == Vector3.zero)
				{
					ddv = this.GroupSet(i, ddv, false);//連携化
				}

				if (this.HItFD[i] != Vector3.zero)
				{
					ddv *= 1 - this.CollisionFriction;//衝突中エフェクト減算
				}
			}

			if (this.DelayCorrection == true && this.FixedTargetOn == false)
			{
				ddv *= this.fpsTime;//処理負荷でエフェクトスピードを調整
			}

			this.LineEvasiveAction(i, ddv);
		}
	}

	private Vector3 IKAveragePosSet(bool wlf)
	{
		//IKgol指定平均座標

		Vector3 sgp = Vector3.zero;

		//IK有効なら
		if (this.FixedTarget != null && this.FixedTarget.Length > 0 && this.FixedTargetOn == true)
		{
			for (int i = 0; i < FixedJointNo.Length; i++)
			{
				int fga0 = this.FixedJointNo[i] - 1;
				int fga1 = this.FixedJointNo[i];

				if (fga0 >= 0 && this.JointObj.Length > fga0)
				{
					if (wlf == false)
					{
						sgp += this.JointObj[fga1].transform.position;
					}
					else
					{
						Vector3 ltow = Vector3.zero;
						if (this.JointObj.Length > fga1)
						{
							ltow = this.JointObj[fga0].transform.
							       TransformPoint(this.JointObj[fga1].transform.localPosition);
						}
						sgp += ltow;
					}
				}
			}

			float dfs0 = 1f / this.FixedJointNo.Length;

			sgp *= dfs0;
		}

		return sgp;
	}

	private void LineEvasiveAction(int i, Vector3 ddv)
	{
		//回避行動
		Vector3 evd1 = Vector3.zero;//目標ワールド座標

		if (this.JointObj.Length > i + 1)
		{
			Vector3 mpd1 = this.JointObj[i + 1].transform.position + ddv;
			Vector3 vd1 = (mpd1 - this.JointObj[i].transform.position).normalized;
			evd1 = (vd1 * this.TargeLength[i]) + this.JointObj[i].transform.position;//エフェクトのワールド座標
		}

		//変形割合
		if(this.AddEffect < 1f && this.JointObj.Length > i + 1)
		{
			Vector3 rotv = Vector3.zero;
			rotv = this.PosRotSet(this.JointObj[i].transform.position
			                      , evd1
			                      , this.JointObj[i + 1].transform.position
			                      , this.JointObj[i].transform);

			//基礎回転データ
			Quaternion aqd = Quaternion.identity;
			aqd = this.fixingSet(
				this.JointObj[i].transform.localRotation * Quaternion.Euler(rotv), i);

			aqd = Quaternion.Lerp(this.OldAng[i], aqd, this.AddEffect);

			//回転適用
			Transform tra = this.JointObj[i].transform;
			tra.localRotation = aqd;

			if(this.OldPos != null && this.OldPos.Length > i + 1)
			{
				Vector3 erp = tra.TransformDirection(this.OldPos[i + 1]);
				evd1 = this.JointObj[i].transform.position + erp;
			}
		}

		if (this.JointCollisionOn == true || this.GCollisionOn == true)
		{
			//IK指定確認
			bool ikj = false;

			if (this.FixedTarget != null && this.FixedJointNo != null && this.FixedTargetOn == true)
			{
				for (int ki = 0; ki < this.FixedJointNo.Length; ki++)
				{
					if (i + 1 == this.FixedJointNo[ki])
					{
						ikj = true;
					}
				}
			}

			bool jhitf = false;
			for (int ci = 0; ci < this.CollList.Count; ci++)
			{
				if (this.CollList[ci] != null)
				{
					//衝突確認
					Vector3 Hitd1 = Vector3.zero;
					Vector3 gcvd1 = Vector3.zero;

					if (i + 1 < this.JointObj.Length)
					{
						//衝突判定
						Vector3 ccn = Vector3.zero;
						Vector3 hvec = Vector3.zero;

						if(this.JointCollisionOn == true)
						{
							Hitd1 = this.LinePosHItTestSet(this.JointObj[i].transform.position
													, ref evd1
													, ref ccn
													, ref hvec
													, this.CollList[ci].SJCCTransform.position
													, this.CollList[ci].SJCCRadius
													, i);

							if(Hitd1[0] != 0f && hvec != Vector3.zero)
							{
								this.CollList[ci].SJCCHitVec = hvec;
							}

							this.CollList[ci].SJCCCNormal = ccn;
						}

						//連携衝突確認
						if (this.GCollisionOn == true)
						{
							gcvd1 = this.GroupCollisionset(this.JointObj[i].transform.position
													, evd1
													, this.CollList[ci].SJCCTransform.position
													, ccn
													, this.CollList[ci].SJCCRadius
													, this.TargeLength[i]
													, i);
						}
					}

					//このジョイントは衝突中
					if(Hitd1[0] != 0f)
					{
						jhitf = true;
					}

					if(this.JointCollisionOn == true)
					{
						//IK指定衝突相手なら衝突SmoothColliderClass情報
						if (ikj == true)
						{
							if (Hitd1[0] != 0f)
							{
								if (this.CollList[ci] != null)
								{
									this.HitGObject = this.CollList[ci];
								}
								else
								{
									this.HitGObject = null;
								}
							}
						}
					}

					//連携衝突移動
					if (this.GCollisionOn == true)
					{
						if(gcvd1 != Vector3.zero)
						{
							evd1 += gcvd1;
						}
					}

					//衝突変形関連処理
					float cRange = 0f;
					float jtl = 0f;
					Vector3 hitpd = Vector3.zero;

					if (Hitd1[0] != 0f && i + 1 < this.JointObj.Length)
					{
						if (this.PressureOn == true)
						{
							Vector3 jvd = this.JointObj[i + 1].transform.position
							              - this.JointObj[i].transform.position;//jointベクトル

							hitpd = this.JointObj[i].transform.position
							        + (jvd.normalized * Hitd1[1]);//ef衝突座標

							cRange = Hitd1[2] + this.CollList[ci].SJCCRadius;
							jtl = Vector3.Distance(this.CollList[ci].SJCCTransform.position, hitpd);
						}
					}

					if (this.PressureOn == true)
					{
						//衝突揺れ終了
						if (Hitd1[0] == 0f && this.CollList[ci].SJCCHitNumber == i)
						{
							this.CollList[ci].SJCCNormal = Vector3.zero;
							this.CollList[ci].SJCCHitNumber = -1;
							this.MASVs = Vector3.zero;
							this.MASfs = 0f;
							this.hsjn = -1;
						}

						//衝突揺れ開始
						if (i + 1 < this.JointObj.Length && Hitd1[0] != 0f)
						{
							Vector3 hvd = (hitpd -
								this.CollList[ci].SJCCTransform.position).normalized;

							//衝突joint指定初期化
							if (this.CollList[ci].SJCCNormal == Vector3.zero
									 && this.CollList[ci].SJCCHitNumber < 0 && this.hsjn != i)//初回か確認
							{
								this.hsjn = i;//joint番号記憶
								this.CollList[ci].SJCCHitNumber = i;

								//衝突ベクトル力
								float hitp = cRange - jtl;
								hitp = Mathf.Clamp(hitp, 0f, Hitd1[2] * 0.5f);

								float vpd = 1f;
								if (cRange != 0f && jtl != 0f)
								{
									vpd = jtl / cRange;

									if (vpd > 1f)
									{
										vpd = 1f - (0.2f * this.AddImpact);
									}
								}

								Vector3 hitv0 = hvd * (hitp * vpd);
								this.CollList[ci].SJCCNormal = hitv0;//コライダ情報渡し

								if (this.SVHPos.sqrMagnitude > hitp * hitp)
								{
									hitv0 *= 0f;
								}

								if(this.AddImpact != 0f)
								{
									this.SVHPos = Vector3.zero;
									this.STHPos = hitv0;
								}
							}

							//衝突揺れ中処理
							if (this.hsjn == i
							 && this.CollList[ci].SJCCNormal != Vector3.zero && this.AddImpact != 0f)
							{
								float mvf = Vector3.Dot(this.STHPos.normalized,
								                        this.CollList[ci].SJCCNormal.normalized);

								if (mvf < 0f)
								{
									this.HSVData = this.STHPos;
								}
								else
								{
									this.HSVData = Vector3.zero;
								}
							}
						}

						//圧力
						if (this.AddPressure != 0f)
						{
							if (this.CollList[ci].SJCCHitNumber == i && Hitd1[0] != 0f && this.hsjn == i)
							{
								float tcl = cRange - jtl;
								tcl = Mathf.Clamp(tcl, 0f, Hitd1[2] * 0.8f);

								float ppd = tcl * this.AddPressure;
								this.MASVs = this.CollList[ci].SJCCNormal.normalized;
								this.MASfs = ppd;
							}
						}

						if (this.AddImpact != 0f
							 && this.hsjn != i && this.CollList[ci].SJCCNormal == Vector3.zero)
						{
							if (this.HSVData != Vector3.zero)
							{
								this.HSVData = this.STHPos;
							}
						}
					}
				}
			}

			//衝突データ初期化
			if (this.EOperation == false && jhitf == false && this.HItFD.Length > i)
			{
				this.HItFD[i] = Vector3.zero;
				this.HitGObject = null;
			}
		}

		//移動スケール処理
		this.MoveAndScaleSet(i);

		//回転処理
		this.RotMoveSet(i, evd1);
	}

	private Vector3 GroupCollisionset(
		Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 ccn,
		float Radius, float Length, int i)
	{
		//pos1=親joint座標 pos2=エフェクトのワールド座標
			//pos3=CollList座標 Radius=衝突確認オブジェ半径 i= joint処理中番号
		//めり込み量を返す

		//左右連携先を確認して連携衝突処理
		Vector3 ghv0 = Vector3.zero;
		Vector3 ghv1 = Vector3.zero;
		Vector3 ghv2 = Vector3.zero;

		if (this.NexToSJData[0] != null && this.NexToSJData[0].JointObj.Length > i + 1)
		{
			//隣 0 とのベクトルに衝突地点を検索
			if (this.NexToSJData[0].JointObj[i] != null
					&& this.NexToSJData[0].JointObj[i + 1] != null
						&& this.JointObj[i + 1] != null)
			{

				ghv1 = this.GroupCollisionDataSet(this.NexToSJData[0].JointObj[i].transform.position,
						 this.NexToSJData[0].JointObj[i + 1].transform.position,
						 this.NexToSJData[0].CollisionRadius[i + 1],
							pos1, pos2, pos3, ccn, Radius, i);
			}
		}

		if (this.NexToSJData[1] != null && this.NexToSJData[1].JointObj.Length > i + 1)
		{
			//隣 1 とのベクトルに衝突地点を検索
			if (this.NexToSJData[1].JointObj[i] != null
					&& this.NexToSJData[1].JointObj[i + 1] != null
						&& this.JointObj[i + 1] != null)
			{

				ghv2 = this.GroupCollisionDataSet(this.NexToSJData[1].JointObj[i].transform.position,
						 this.NexToSJData[1].JointObj[i + 1].transform.position,
						 this.NexToSJData[1].CollisionRadius[i + 1],
							pos1, pos2, pos3, ccn, Radius, i);
			}
		}

		if(ghv1 != Vector3.zero && ghv2 == Vector3.zero)
		{
			ghv0 = ghv1;
		}
		if(ghv1 == Vector3.zero && ghv2 != Vector3.zero)
		{
			ghv0 = ghv2;
		}
		if(ghv1 != Vector3.zero && ghv2 != Vector3.zero)
		{
			ghv0 = (ghv1 * 0.5f) + (ghv2 * 0.5f);
		}

		return ghv0;
	}

	private Vector3 GroupCollisionDataSet(Vector3 ntsjo, Vector3 ntsjt, float ntsjc,
		Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 ccn, float Radius, int i)
	{
		//pos1=親joint座標 pos2=エフェクトのワールド座標
			//pos3=CollList座標 Radius=衝突確認オブジェ半径 i= joint処理中番号

		//連携ジョイントベクトル上衝突処理
		Vector3 gcds = Vector3.zero;

		//隣とのベクトルに衝突地点を検索
		if (i > 0 && this.JointObj[i] != null)
		{
			Vector3 v1 = ntsjt - pos2;//横ジョイントベクトル
			Vector3 v1n = v1.normalized;
			Vector3 v2 = pos3 - pos2;//中心から衝突オブジェ
			float vrd = Vector3.Dot(v2.normalized, v1n);
			float v2m = v2.magnitude;
			float vrds = v2m * Mathf.Sin(vrd);//コリジョンから最接近点までの長さ
			float v1sq = v1.sqrMagnitude;

			if(vrds > 0f && v1sq >= (v2m - Radius) * (v2m - Radius))
			{
				//基点と衝突点の長さと角度で基点-エフェクト点ベクトル上の衝突原点を出す

				Vector3 lp  = pos2 + (v1n * vrds);//最接近点ワールド座標

				//最接近点とコリジョンとの長さ
				Vector3 lp3v = lp - pos3;
				float lp3l = lp3v.magnitude;

				float oel = (v2m * vrd) / v2m;//0~1割合
				oel = Mathf.Clamp(oel, 0f, 1f);

				float colrd = this.CollisionRadiusSet(
					this.CollisionRadius[i + 1], ntsjc, oel);

				float rall = colrd + Radius;//衝突範囲

				if(lp3l <= rall)
				{
					//衝突中

					//操作するジョイントと横衝突位置で割合変更
					float ratioL = 1f;
					float l2sq = (lp -pos2).sqrMagnitude;
					if(l2sq != 0f && v1sq != 0f)
					{
						ratioL = 1f - (l2sq / v1sq);
					}
					ratioL = Mathf.Clamp(ratioL, 0f, 1f);

					Vector3 nvp0 = ((lp - pos3) + ccn) + pos2;//回避 w
					Vector3 onvp0 = ((nvp0 - pos1).normalized * this.TargeLength[i]) + pos1;//回転範囲補正
					Vector3 cnv0 = onvp0 - pos3;//衝突回避ベクトル

					gcds = cnv0.normalized * ((rall - lp3l) * ratioL);
				}
			}
		}

		return gcds;
	}

	private float CollisionRadiusSet(float cro, float crt, float ratio)
	{
		//jointのコリジョンサイズを調整して返す
		//cro=中心コリジョンサイズ, crt=ターゲットコリジョンサイズ, ratio=割合

		ratio = Mathf.Clamp(ratio, 0f, 1f);

		float colrd = cro + ((crt - cro) * ratio);

		if(cro < crt)
		{
			colrd = Mathf.Clamp(colrd, cro, crt);
		}
		if(cro > crt)
		{
			colrd = Mathf.Clamp(colrd, crt, cro);
		}
		if(cro == crt)
		{
			colrd = cro;
		}

		return colrd;
	}

	private Vector3 LinePosHItTestSet(Vector3 pos1,
		ref Vector3 pos2, ref Vector3 cnv, ref Vector3 hvec,
		Vector3 pos3, float Radius, int i)
	{
		//pos1=中心座標 pos2=エフェクト座標 pos3=衝突確認オブジェ座標 Radius=衝突確認オブジェ半径 i= joint処理中番号

		//線と点の当たり判定(0で当たっていない、０でなければめり込み量を返す)
		Vector3 hf = Vector3.zero;

		//衝突候補確認
		bool htf = false;

		//jointコリジョンサイズ調整
		float colrd = this.CollisionRadius[i + 1];

		Vector3 v1 = pos2 - pos1;//中心からエフェクト
		Vector3 v2 = pos3 - pos1;//中心から衝突オブジェ
		Vector3 v3 = pos2 - pos3;//子から衝突オブジェ
		Vector3 v1n = v1.normalized;
		float v2l = v2.magnitude;
		float v3sm = v3.sqrMagnitude;
		float vrd = Vector3.Dot(v2.normalized, v1n);

		float oel = (v2l * vrd) / v2l;//0~1割合
				oel = Mathf.Clamp(oel, 0f, 1f);

		if(this.TargeLength[i] > v2l - Radius && vrd > 0f)
		{
			htf = true;
		}

		//コリジョン範囲設定
		if (this.CollisionRadius.Length > i + 1)
		{
			colrd = this.CollisionRadiusSet(
				this.CollisionRadius[i], this.CollisionRadius[i + 1], oel);
		}

		if(v3sm <= (Radius + colrd) * (Radius + colrd))
		//if(v2l <= Radius + colrd)
		{
			htf = true;
		}

		//衝突可能性あり
		if(htf == true)
		{
			//移動スピード確認
			if (this.AddRadius != 1f)
			{
				float dist0 = (this.JointObj[i].transform.position - this.COPos[i]).sqrMagnitude;

				if (dist0 > this.SpeedCRange * this.SpeedCRange)
				{
					this.SpeedF = true;
				}
				else
				{
					this.SpeedF = false;
				}
			}
			else
			{
				if (this.SpeedF == true)
				{
					this.SpeedF = false;
				}
			}

			//基点と衝突点の長さと角度で基点-エフェクト点ベクトル上の衝突原点を出す
			float vrds = Mathf.Sin(vrd);
			float cnl = v2l * vrds;//コリジョンから最接近点までの長さ

			cnl = Mathf.Clamp(cnl, this.CollisionRadius[i] * 1.1f, this.TargeLength[i]);
			Vector3 lp = pos1 + (v1n * cnl);//最接近点ワールド座標

			Vector3 p3cv = lp - pos3;//コリジョンから最接近点へ向かうベクトル
			float vld0 = p3cv.magnitude;//距離

			//衝突確認
			float ral = 0f;
			float addrd = (this.AddRadius - 1f) * this.addfd;

			if (this.SpeedF == false)
			{
				ral = Radius + colrd * 1.1f;//範囲
			}
			else
			{
				//速い移動のときの処理
				ral = (Radius * (1f + (addrd * i))) + colrd; //範囲
			}

			//衝突中
			if (ral >= vld0)
			{
				//前回の法線
				if(cnv == Vector3.zero)
				{
					cnv = p3cv;
				}

				float pnf = 1f;
				if(cnv != Vector3.zero)
				{
					pnf = Vector3.Dot(p3cv.normalized, cnv.normalized);
				}

				if(pnf < 0)
				{
					hf[0] = ral + vld0;//反対側めり込み量
				}
				else
				{
					hf[0] = ral - vld0;//めり込み量
				}

				if (this.SpeedF == true)
				{
					hf[0] *= 1f + (addrd * i);
				}

				hf[1] = Vector3.Distance(pos1, lp);//軸長さ

				//衝突座標渡し
				if (this.HItFD.Length > i)
				{
					this.HItFD[i] = lp + ((pos3 - lp).normalized * colrd);
				}

				hf[2] = colrd;//jointコリジョンサイズ

				//衝突回避
				if(this.JointObj.Length > i + 1)
				{
					Vector3 cvd = this.JointObj[i + 1].transform.position - pos2;
					Vector3 nvp0 = ((lp - pos3) + cvd) + pos2;//回避 w
					Vector3 onvp0 = ((nvp0 - pos1).normalized * this.TargeLength[i]) + pos1;//回転範囲補正
					Vector3 cnv0 = onvp0 - pos3;//衝突回避ベクトル

					hvec = cnv0.normalized * hf[0];
					pos2 += hvec;
				}
			}
			else
			{
				if(cnv != Vector3.zero)
				{
					cnv = Vector3.zero;
				}
			}
		}
		else
		{
			if(cnv != Vector3.zero)
			{
				cnv = Vector3.zero;
			}
		}

		return hf;
	}

	private void MoveAndScaleSet(int i)
	{
		//移動とサイズエフェクト
		if (this.JointObj != null && this.JointObj.Length > i && this.JointObj[i] != null)
		{
			if (this.MoveAndScaleOn == true)
			{
				//重力
				Vector3 maslg = Vector3.zero;
				if (i > 0)
				{
					float md = 1f;
					if (this.CollisionRadius[i] != 0f && this.collAverage != 0f)
					{
						md = this.CollisionRadius[i] / this.collAverage;
						if (md > 1f)
						{
							md *= 1.1f;
						}
						else
						{
							md *= 0.9f;
						}
					}
					Vector3 mgd = new Vector3(0f, this.Gravity, 0f);
					Vector3 gfd = (mgd * 0.9f) + ((mgd * md * 0.1f * this.addfd) * i);

					if(this.PressureOn == true)
					{
						gfd *= this.AddPressureGravity;
					}

					maslg = this.JointObj[i].transform.InverseTransformDirection(gfd);
				}

				//つぶれエフェクト
				Vector3 scaleData = Vector3.zero;

				if (this.OldScale != null && this.OldScale.Length > i && this.AddScale != Vector3.zero)
				{
					//スケール初期化
					scaleData = Vector3.zero;
					this.JointObj[i].transform.localScale = this.OldScale[i];

					if (this.JointObj.Length > i + 1 && this.OldScale.Length > i + 1)
					{
						this.JointObj[i + 1].transform.localScale = this.OldScale[i + 1];
					}

					//圧力
					Vector3 ppsd0 = Vector3.zero;
					if (this.hsjn == i && this.PressureOn == true && this.MASVs != Vector3.zero)
					{
						//条件設定

						//ローカル変換
						Vector3 lond =
						    this.JointObj[i].transform.InverseTransformDirection(this.MASVs);

						Vector3 ppsd1 = Vector3.zero;
						ppsd1.x = Mathf.Abs(lond.x);
						ppsd1.y = Mathf.Abs(lond.y);
						ppsd1.z = Mathf.Abs(lond.z);

						float[] vfda = new float[3];
						vfda[0] = ppsd1.x;
						vfda[1] = ppsd1.y;
						vfda[2] = ppsd1.z;
						float maxvd =  Mathf.Max(vfda);
						if (vfda[0] == maxvd)
						{
							ppsd1.y += (ppsd1.x * 0.05f) * this.AddPressure;
							ppsd1.z += (ppsd1.x * 0.05f) * this.AddPressure;
							ppsd1.x *= -1f * this.AddPressure;
						}
						if (vfda[1] == maxvd)
						{
							ppsd1.x += (ppsd1.y * 0.05f) * this.AddPressure;
							ppsd1.z += (ppsd1.y * 0.05f) * this.AddPressure;
							ppsd1.y *= -1f * this.AddPressure;
						}
						if (vfda[2] == maxvd)
						{
							ppsd1.x += (ppsd1.z * 0.05f) * this.AddPressure;
							ppsd1.y += (ppsd1.z * 0.05f) * this.AddPressure;
							ppsd1.z *= -1f * this.AddPressure;
						}

						ppsd0 = ppsd1 * this.MASfs;
					}

					//揺れスケール変型値（ワールド）
					Vector3 wvd = this.SVData + this.STIKPos + (this.STHPos * this.AddImpact);

					//条件設定
					wvd.x = Mathf.Abs(wvd.x);
					wvd.z = Mathf.Abs(wvd.z);

					//ローカルに変換
					Vector3 lscvd = this.JointObj[i].transform.InverseTransformDirection(wvd);
					lscvd *= this.addfd;
					lscvd *= i;

					//重力調整
					Vector3 gsd = maslg;
					if(this.PressureOn == true && this.AddPressureGravity != 0)
					{
						float vlf = 0f;
						if (this.JointObj.Length > i + 1)
						{
							//割合値
							Vector3 jld = this.JointObj[i + 1].transform.position
							              - this.JointObj[i].transform.position;
							vlf = Vector3.Dot(jld.normalized, Vector3.down) * -2f;
						}
						Vector3 ladd = this.JointObj[i].transform.InverseTransformDirection(new Vector3(1f, 0f, 1f));
						Vector3 addg = Vector3.Scale(gsd, ladd) * vlf;//ワールドYを抜いた加算（ローカルデータ)

						addg.x = Mathf.Abs(addg.x);
						addg.y = Mathf.Abs(addg.y);
						addg.z = Mathf.Abs(addg.z);

						gsd += addg;

						gsd.x = Mathf.Abs(gsd.x);
						gsd.y = Mathf.Abs(gsd.y);
						gsd.z = Mathf.Abs(gsd.z);

						gsd *= -1f;
						gsd *= this.AddPressureGravity;
					}
					else
					{
						gsd = Vector3.zero;
					}

					//スケール変更
					if (this.hsjn == i)
					{
						scaleData = ppsd0 + lscvd + gsd;
					}
					else
					{
						scaleData = lscvd + gsd;
					}

					scaleData *= this.load;
					scaleData = Vector3.Scale(scaleData, this.AddScale);

					//NaNエラー回避処理
					if (float.IsNaN(scaleData.x) == true)
					{
						scaleData.x = 0f;
					}
					if (float.IsNaN(scaleData.y) == true)
					{
						scaleData.y = 0f;
					}
					if (float.IsNaN(scaleData.z) == true)
					{
						scaleData.z = 0f;
					}

					this.JointObj[i].transform.localScale = this.OldScale[i] + scaleData;

					//子のスケールを元に戻す
					if (this.hsjn == i && this.JointObj.Length > i + 1 && this.OldScale.Length > i + 1)
					{
						this.JointObj[i + 1].transform.localScale =
						    this.OldScale[i + 1] - scaleData;
					}
				}

				//移動
				if (this.OldPos != null && this.OldPos.Length > i + 1 && this.AddMove != Vector3.zero)
				{
					if (this.hsjn != i)
					{
						this.JointObj[i + 1].transform.localPosition = this.OldPos[i + 1];
					}

					//揺れ移動データをローカルデータに
					Vector3 moveVec = this.JointObj[i].transform.InverseTransformDirection(
							this.SVData + (this.STIKPos * this.AddIKSpring)) + maslg;

					Vector3 shmove = this.JointObj[i].transform.InverseTransformDirection(this.HSVData);
					moveVec += shmove;

					moveVec*= this.load;
					moveVec = Vector3.Scale(moveVec, this.AddMove);

					//NaNエラー回避処理
					if (float.IsNaN(moveVec.x) == true)
					{
						moveVec.x = 0f;
					}
					if (float.IsNaN(moveVec.y) == true)
					{
						moveVec.y = 0f;
					}
					if (float.IsNaN(moveVec.z) == true)
					{
						moveVec.z = 0f;
					}

					//揺れ移動適用
					this.JointObj[i + 1].transform.localPosition = this.OldPos[i + 1] + moveVec;
				}
			}
		}
	}

	private Quaternion fixingSet(Quaternion qd, int i)
	{
		//回転抑制
		Quaternion rotDate = Quaternion.identity;

		if (this.FixX == true || this.FixY == true || this.FixZ == true)
		{
			//固定基礎ローカル角度
			Vector3 oqav = this.OldAng[i].eulerAngles;
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

			//x
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

			//値入力
			psd.x = nca.x + oqav.x;
			cpad.x = (psd.x < 0f) ? psd.x + 360f : psd.x;//0~360に戻す
			cpad.x %= 360f;

			//y
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

			//値入力
			psd.y = nca.y + oqav.y;
			cpad.y = (psd.y < 0f) ? psd.y + 360f : psd.y;//0~360に戻す
			cpad.y %= 360f;

			//z
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

			//値入力
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

		tod += (tra.InverseTransformDirection(td - od).normalized);//ターゲットと基点の角度出し用
		mod += (tra.InverseTransformDirection(md - od).normalized);//現在位置出し用

		if (tod != Vector3.zero && mod != Vector3.zero)
		{
			ro = Quaternion.FromToRotation(tod, mod);

			//0.001以下エラーログ回避
			ro.x = (Mathf.Abs(ro.x) < 0.001f) ? 0f : ro.x;
			ro.y = (Mathf.Abs(ro.y) < 0.001f) ? 0f : ro.y;
			ro.z = (Mathf.Abs(ro.z) < 0.001f) ? 0f : ro.z;

			roth = ro.eulerAngles;
		}

		return roth;
	}

	private Vector3 GroupSet(int i, Vector3 pos, bool initf)
	{
		//連携化
		Vector3 epd = pos;
		i += 1;

		if (this.GroupOn == true && this.EOffP != null && this.JointObj.Length > i)
		{
			if (this.NexToSJData != null && this.NexToSJData.Length > 0)
			{
				//連携jointが片側のみ
				if (this.NexToSJData[0] == null || this.NexToSJData[1] == null)
				{
					Vector3 jpos2 = Vector3.zero;

					if (this.NexToSJData[0] != null)
					{
						if (this.NexToSJData[0].JointObj.Length > i)
						{
							if (this.NexToSJData[0].JointObj[i] != null)
							{
								jpos2 = this.NexToSJData[0].JointObj[i].transform.position;
							}
						}
					}

					if (initf == false)
					{
						if (jpos2 != Vector3.zero)
						{
							//ローカルオフセット
							Vector3 tvd5 = jpos2 - this.JointObj[i].transform.position;
							Vector3 tvd6 = (tvd5 * 0.5f) + (tvd5.normalized * this.EOffP[i].x);

							epd += ((tvd6 * this.addfd) * i) * this.AddGroup;
						}
					}
					else
					{
						if (this.EOffP.Length > i && jpos2 != Vector3.zero)
						{
							this.EOffP[i] = Vector3.zero;
							this.EOffP[i].x = Vector3.Distance(this.JointObj[i].transform.position, jpos2) * -0.5f;
						}
					}
				}
				else
				{
					//両サイドと連携
					if (this.NexToSJData[0] != null && this.NexToSJData[1] != null)
					{
						//ジョイント数内で処理
						if (i < this.NexToSJData[0].JointObj.Length && i < this.NexToSJData[1].JointObj.Length)
						{
							//連携先取得
							Vector3 jpos0 = Vector3.zero;
							Vector3 jpos1 = Vector3.zero;
							if (this.NexToSJData[0].JointObj.Length > i)
							{
								if (this.NexToSJData[0].JointObj[i] != null)
								{
									jpos0 = this.NexToSJData[0].JointObj[i].transform.position;
								}
							}
							if (this.NexToSJData[1].JointObj.Length > i)
							{
								if (this.NexToSJData[1].JointObj[i] != null)
								{
									jpos1 = this.NexToSJData[1].JointObj[i].transform.position;
								}
							}

							//自ジョイントと連携ジョイントの直線平均ワールド座標
							Vector3 tvd0 = this.JointObj[i].transform.position
							               + ((jpos0 - this.JointObj[i].transform.position) * 0.5f);

							Vector3 tvd1 = this.JointObj[i].transform.position
							               + ((jpos1 - this.JointObj[i].transform.position) * 0.5f);

							//平均ワールド座標
							Vector3 tvd2 = tvd0 + (tvd1 - tvd0) * 0.5f;

							Vector3 tvd3 = Vector3.zero;

							float dist = Vector3.Distance(this.JointObj[i].transform.position, tvd2);

							//平均処理
							if (initf == false)
							{
								if (jpos0 != Vector3.zero && jpos1 != Vector3.zero)
								{
									if (this.EOffP != null)
									{
										tvd3 = (this.EOffP[i] * this.OffsetGroup) * dist;
									}

									Vector3 tvd4 = (tvd2 + tvd3) - this.JointObj[i].transform.position;

									//平均データ
									epd += ((tvd4 * this.addfd) * i) * this.AddGroup;
								}
							}
							else
							{
								//初期化処理
								if (this.SearchSJO != null)
								{
									if (this.NexToSJ_0 != null || this.NexToSJ_1 != null)
									{
										this.EOffP[i] = (this.JointObj[i].transform.position - tvd2).normalized;
									}
								}
							}
						}
					}
				}
			}
		}

		return epd;
	}

	private Vector3 GravitySet(int i, Vector3 pos)
	{
		//重力処理
		Vector3 gpd = pos;
		Vector3 gvd = Vector3.zero;

		if (this.GravityOn == true)
		{
			int ino = this.JointObj.Length - i - 1;
			float gv = (((this.Gravity * this.addfd) * ino) * 0.5f) + (this.Gravity * 0.5f);
			Vector3 gvv3 = Vector3.up * this.Gravity;

			if (this.InertiaOn == true && this.fpsTime < 6f)
			{
				//ベクトルの慣性
				Vector3 frv = new Vector3(0f, gv, 0f) - pos;

				this.IneVPos[i] += frv * this.InertiaSpeed;
				this.IneVPos[i] *= this.InertiaFriction;

				this.IneVPos[i].x = (
					Mathf.Abs(this.IneVPos[i].x) < 0.0001f * this.fpsTime) ? 0f : this.IneVPos[i].x;
				this.IneVPos[i].y = (
					Mathf.Abs(this.IneVPos[i].y) < 0.0001f * this.fpsTime) ? 0f : this.IneVPos[i].y;
				this.IneVPos[i].z = (
					Mathf.Abs(this.IneVPos[i].z) < 0.0001f * this.fpsTime) ? 0f : this.IneVPos[i].z;

				Vector3 giv = this.IneVPos[i] - (gvv3 * this.InertiaFriction);

				//風設定中は慣性制御
				if (this.WindOn == true && this.Wind != Vector3.zero)
				{
					giv = Vector3.Scale(giv, this.WindPendulum);
				}

				gvd = gvv3 + (giv * this.AddInertia);
			}
			else
			{
				gvd = gvv3;
			}

			gpd = pos + gvd;
		}

		return gpd;
	}

	private Vector3 springVecSet(Vector3 opv, Vector3 tpv, ref Vector3 vad)
	{
		//ベクトルスプリング処理
		Vector3 spl0 = tpv - opv;
		Vector3 spl1 = spl0 * this.SpringHardness;

		vad += spl1;
		vad *= this.SpringFriction;

		opv += vad;

		return opv;
	}

	private Vector3 SpringSet(int i, Vector3 pos)
	{
		//スプリング挙動をさせる
		Vector3 spd = pos;

		if (this.SpringOn == true && this.AddSpring != Vector3.zero)
		{
			//揺れ計算
			this.moveData.x = (
				Mathf.Abs(this.moveData.x) < 0.00001f) ? 0f : this.moveData.x;
			this.moveData.y = (
				Mathf.Abs(this.moveData.y) < 0.00001f) ? 0f : this.moveData.y;
			this.moveData.z = (
				Mathf.Abs(this.moveData.z) < 0.00001f) ? 0f : this.moveData.z;

			this.STPos += this.moveData;
			this.STPos = this.springVecSet(this.STPos, Vector3.zero, ref this.SVPos);
			this.SVData = this.STPos;
			this.SVData *= -1f;

			//IK揺れ
			Vector3 iks = Vector3.zero;
			if (this.FixedTarget != null && this.FixedTarget.Length > 0 && this.FixedTargetOn == true)
			{
				this.STIKPos = this.springVecSet(this.STIKPos, Vector3.zero, ref this.SVIKPos);

				float gtl0 = 0f;//golとTarget距離
				for (int ai = 0; this.FixedJointNo.Length > ai; ai++)
				{
					if (this.FixedJointNo[ai] == i + 1)
					{
						if (this.IKtgl.Length > ai)
						{
							gtl0 = this.IKtgl[ai];
						}
					}
				}

				iks = this.STIKPos;

				//範囲内に減退
				float pow = gtl0 - this.IKRange;
				if (pow < 0f)
				{
					iks = Vector3.zero;
				}
				else
				{
					float asd = (this.AddSpring.x + this.AddSpring.y + this.AddSpring.z) * 0.33f;
					if (asd != 0f)
					{
						float rate0 = 1f / asd;
						float rate1 = 1f - rate0;

						float adp = rate0 + (rate1 * pow);

						iks *= adp;
					}
					else
					{
						iks = Vector3.zero;
					}
				}

				if (iks != Vector3.zero)
				{
					iks /= this.JointObj.Length;
				}
			}

			//衝突揺れ
			if (this.STHPos != Vector3.zero)
			{
				this.STHPos = this.springVecSet(this.STHPos, Vector3.zero, ref this.SVHPos);
			}

			//揺れデータ(ワールド)
			Vector3 allsd = this.SVData + iks + this.STHPos;

			Vector3 locpd = Vector3.Scale(
						this.JointObj[i].transform.InverseTransformDirection(allsd), //ローカル変換
						 this.AddSpring);//ユーザー加算

			//揺れ調整
			this.endfd = locpd.sqrMagnitude;

			if (this.endfd > 0.000003f)
			{
				//回転
				spd = pos +
				(this.JointObj[i].transform.TransformDirection(locpd) * this.fpsTime);
			}
			else
			{
				this.moveData = Vector3.zero;
				this.STPos = Vector3.zero;
				this.SVPos = Vector3.zero;
				this.SVData = Vector3.zero;

				spd = pos;
			}

			if(this.sptOldPos != this.JointObj[0].transform.position)
			{
				this.sptOldPos = this.JointObj[0].transform.position;
			}
		}

		return spd;
	}

	private Vector3 CentrifugalSet(int i, Vector3 pos)
	{
		//遠心力セット
		Vector3 cpd = pos;

		if (this.CentrifugalForceOn == true)
		{
			if (this.JointObj.Length > i + 1)
			{
				if (i >= this.Oroot)
				{
					Vector3 cvd = Vector3.zero;
					Transform cent;

					if (this.Centroid != null)
					{
						cent = this.Centroid.transform;
					}
					else
					{
						cent = this.JointObj[i].transform.root.gameObject.transform;
					}

					//中心から外側に向くベクトル
					cvd = ((this.JointObj[i].transform.position - cent.position)
							 + (this.JointObj[i + 1].transform.position -
								 this.JointObj[i].transform.position)).normalized;

					//遠心力データ作成
					Vector3 rotDV = Vector3.zero;
					rotDV = cvd * Centrifugal(i, this.JointObj[i + 1].transform.position,
								 this.COPos[i + 1]);

					cpd = pos + (rotDV * i);
				}
			}
		}

		return cpd;
	}

	private float Centrifugal(int i, Vector3 td, Vector3 md)
	{
		//遠心力
		float cff = 0f;

		//角度
		float angd = Vector3.Angle(td, md) * Mathf.Deg2Rad;

		//遠心力（質量　*　半径　*　角速度2乗
		float cfd = this.CentrifugalForce * this.TargeLength[i] * Mathf.Pow(angd, 2f);

		cff = cfd;

		return cff;
	}

	private Vector3 WindSet(int i, Vector3 pos)
	{
		//風エフェクト
		Vector3 wpd = pos;

		Vector3 wed = this.Wind * this.addfd;

		if (this.WindOn == true && this.Wind != Vector3.zero)
		{
			Vector3 wdd = Vector3.zero;

			if(this.RandWind != Vector3.zero && i == 0 && Random.value < 0.1f)
			{
				this.rand = new Vector3(
				(1f - this.RandWind.x) + (this.RandWind.x * Random.value),
				(1f - this.RandWind.y) + (this.RandWind.y * Random.value),
				(1f - this.RandWind.z) + (this.RandWind.z * Random.value));
			}

			this.rana.x += this.WindSpeed;
			this.rana.x %= 360f;

			float rdX = Mathf.Sin(this.rana.x * Mathf.Deg2Rad);
			if (rdX >= 0f)
			{
				wdd.x = (wed.x * this.rand.x * i) * rdX;
			}
			else
			{
				wdd.x = ((wed.x * this.rand.x * i) * this.WindPendulum.x) * rdX;
			}

			this.rana.y += this.WindSpeed;
			this.rana.y %= 360f;

			float rdY = Mathf.Sin(this.rana.y * Mathf.Deg2Rad);
			if (rdY >= 0f)
			{
				wdd.y = (wed.y * this.rand.y * i) * rdY;
			}
			else
			{
				wdd.y = ((wed.y * this.rand.y * i) * this.WindPendulum.y) * rdY;
			}

			this.rana.z += this.WindSpeed;
			this.rana.z %= 360f;

			float rdZ = Mathf.Sin(this.rana.z * Mathf.Deg2Rad);
			if (rdZ >= 0f)
			{
				wdd.z = (wed.z * this.rand.z * i) * rdZ;
			}
			else
			{
				wdd.z = ((wed.z * this.rand.z * i) * this.WindPendulum.z) * rdZ;
			}

			wpd = pos + wdd;
		}

		return wpd;
	}

	private void TwistRestoreSet(int i)
	{
		//ねじれを解消する
		float step = 0f;
		step = this.TwistRestoreSpeed * Time.deltaTime;

		Vector3 orjD = Vector3.zero;
		orjD = this.JointObj[i].transform.localRotation.eulerAngles;

		Vector3 rotDV = Vector3.zero;
		rotDV = Quaternion.RotateTowards(
		            this.JointObj[i].transform.localRotation,
		            this.OldAng[i]
		            , step).eulerAngles;

		if (this.TwistRestoreX == true)
		{
			orjD.x = rotDV.x;
		}
		if (this.TwistRestoreY == true)
		{
			orjD.y = rotDV.y;
		}
		if (this.TwistRestoreZ == true)
		{
			orjD.z = rotDV.z;
		}

		this.JointObj[i].transform.localRotation = Quaternion.Euler(orjD);
	}

	private void VectorIKSet()
	{
		//ベクトルIK補正
		int lnd = this.JointObj.Length * 1;

		if (this.FixedTarget != null && this.fixJointNoList != null && this.VIKPos != null
		        && this.FixedJointNo != null && this.IKRangeOn != null && this.FixedTargetOn == true)
		{
			if (this.FixedTarget.Length > 0 && this.FixedJointNo.Length > 0 && this.fixJointNoList.Count > 0)
			{
				for (int bi = this.FixedTarget.Length; bi > 0 ; bi--)
				{
					int ei = bi - 1;

					//移動目標座標算出
					for (int ci = lnd; ci > 0; ci--)
					{
						//最先端から逆カウント
						int ai = ci - 1;
						ai %= this.JointObj.Length;

						//IK指定範囲内か？
						if (ai > 0 && this.fixJointNoList[this.fixJointNoList.Count - 1] >= ai)
						{
							Vector3 tpos = Vector3.zero;

							//指定目標オブジェクトか？確認
							if (this.FixedTarget.Length > ei && this.JointObj.Length > ai)
							{
								if (this.FixedJointNo.Length > ei && this.FixedJointNo[ei] == ai)
								{
									//目標ゴールがある (最後のジョイント用)

									if (this.IKRangeOn[ei] == true) //範囲ＩＫ起動
									{
										float hrang = Vector3.Distance(this.FixedTarget[ei].position,
										                               this.JointObj[ai].transform.position);

										//golとTarget距離
										if (this.IKtgl.Length > ei)
										{
											this.IKtgl[ei] = hrang;
										}

										if (hrang < this.IKRange)
										{
											//距離で減退
											float jtl = 0f;
											if (this.IKRangeDecline == true
											        && this.IKRange != 0f && hrang != 0f)
											{
												jtl = hrang / this.IKRange;
											}

											Vector3 jtpos = Vector3.Lerp(
														this.FixedTarget[ei].position,
														 this.JointObj[ai].transform.position, jtl);

											this.VIKPos[ai] = jtpos;
											this.IKRF[ei] = true;
										}
										else
										{
											this.VIKPos[ai] = Vector3.zero;
											this.IKRF[ei] = false;
										}
									}
									else
									{
										this.VIKPos[ai] = this.FixedTarget[ei].position;
									}
								}

								//回転目標算出
								if (this.VIKPos.Length > ai + 1 && this.TargeLength.Length > ai)
								{
									//目標算出
									tpos = this.JointObj[ai].transform.position - this.VIKPos[ai + 1];
									tpos = tpos.normalized * this.TargeLength[ai];

									//リスト内に無しを確認
									if (this.fixJointNoList.BinarySearch(ai) < 0)
									{
										if (ai > 0 && this.VIKPos[ai + 1] != Vector3.zero)
										{
											this.VIKPos[ai] = this.VIKPos[ai + 1] + tpos;
										}
										else
										{
											this.VIKPos[ai] = Vector3.zero;
										}
									}
									else
									{
										//目標ゴールがある (最後以外のジョイント用)
										if (this.FixedJointNo.Length > ei && this.FixedJointNo[ei] == ai)
										{
											if (this.IKRangeOn[ei] == true) //範囲ＩＫ起動
											{
												//golとTarget距離
												float hrang = Vector3.Distance(this.FixedTarget[ei].position,
															 this.JointObj[ai].transform.position);

												if (this.IKtgl.Length > ei)
												{
													this.IKtgl[ei] = hrang;
												}

												if (hrang < this.IKRange)
												{
													//距離で減退
													float jtl = 0f;
													if (this.IKRangeDecline == true
													        && this.IKRange != 0f && hrang != 0f)
													{
														jtl = hrang / this.IKRange;
													}
													Vector3 jtpos = Vector3.Lerp(
															this.FixedTarget[ei].position,
															 this.JointObj[ai].transform.position, jtl);

													this.VIKPos[ai] = jtpos;

													this.IKRF[ei] = true;
												}
												else
												{
													//範囲外IKをOff(親がIK起動中で無ければ初期化)
													if (ai > 0 && this.VIKPos[ai + 1] != Vector3.zero)
													{
														this.VIKPos[ai] = this.VIKPos[ai + 1] + tpos;
													}
													else
													{
														this.VIKPos[ai] = Vector3.zero;
													}

													this.IKRF[ei] = false;
												}
											}
											else
											{
												this.VIKPos[ai] = this.FixedTarget[ei].position;
											}
										}
									}
								}
							}
						}
						else
						{
							this.VIKPos[ai] = Vector3.zero;
						}
					}
				}
			}
		}
	}

	private Quaternion RootDiminution(Vector3 qdata, int i)
	{
		//元の形状との変型割合調整
		Quaternion AddRotDate = Quaternion.identity;

		Quaternion rott = Quaternion.identity;
		rott = Quaternion.Euler(qdata);

		//基礎回転データ
		Quaternion aqd = Quaternion.identity;
		aqd = this.JointObj[i].transform.localRotation * rott;

		//回転範囲処理
		aqd = this.fixingSet(aqd, i);

		if (this.OldAng.Length > i)
		{
			AddRotDate = Quaternion.Lerp(this.OldAng[i], aqd, this.AddEffect);
		}
		else
		{
			AddRotDate = aqd;
		}

		return AddRotDate;
	}

	private void RotMoveSet(int i, Vector3 pos)
	{
		//回転移動処理
		if (i >= this.Oroot)
		{
			//２つめより処理
			Vector3 pdw = Vector3.zero;

			//固定目標あり
			if (this.FixedTarget != null && this.fixJointNoList != null
					 && this.VIKPos != null && this.FixedTargetOn == true)
			{
				if (this.VIKPos.Length > i + 1 && this.JointObj.Length > i + 1)
				{
					if (this.VIKPos[i + 1] != Vector3.zero)
					{
						Vector3 vpd0 = this.VIKPos[i + 1] - this.JointObj[i].transform.position;
						Vector3 vpd1 = pos - this.JointObj[i].transform.position;

						Vector3 vpd = vpd0 + vpd1;

						pdw = vpd.normalized + this.JointObj[i].transform.position;
					}
					else
					{
						pdw = pos;
					}
				}
			}
			else
			{
				pdw = pos;
			}

			if (this.JointObj.Length > i + 1)
			{
				Vector3 rotv = Vector3.zero;
				rotv = this.PosRotSet(this.JointObj[i].transform.position
										 , pdw
										 , this.JointObj[i + 1].transform.position
										 , this.JointObj[i].transform);

				//基礎回転データ
				Quaternion aqd = Quaternion.identity;
				aqd = this.JointObj[i].transform.localRotation * Quaternion.Euler(rotv);

				//rootに向かってエフェクト処理を減退させる
				float adr = this.addfd * (i + 1);
				Quaternion reEvd = Quaternion.Lerp(this.OldAng[i], aqd, adr);
				aqd = Quaternion.Lerp(aqd, reEvd, this.AddRectify);

				//回転範囲処理
				Quaternion rotationDate = Quaternion.identity;
				rotationDate = this.fixingSet(aqd, i);

				//回転適用
				this.JointObj[i].transform.localRotation = rotationDate;
			}
		}
	}

}
