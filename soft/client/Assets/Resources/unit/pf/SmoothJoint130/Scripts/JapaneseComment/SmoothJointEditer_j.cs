using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//SmoothJointパラメータをまとめて編集する ver 1.3.2 20170710 (使用する場合はpublic class SmoothJointEditer_jの_jを削除)
public class SmoothJointEditer_j : MonoBehaviour {
#if UNITY_EDITOR

	[Header("- Initial setup ----------------------")]
	public bool SJAutoSetOn = false;//SmoothJointを自動で読み込む
	public bool EditerReset = false;//初期化

	[Space(10)]
	public GameObject[] SmoothJointObj;//SmoothJointを自動で読み込むOBJ
	public List<SmoothJoint> SJList;//SmoothJoint
	public List<string> SJName;//SmoothJoint名前
	public List<bool> SJEditOn;//SmoothJointのOnOff

	[Header("- To read data -----------------------")]
	public bool I_GetDataOn_I = false;
	public int GetDataListNo = 0;//データ読み込み配列番号

	[Header("- All the data are changed ----------")]
	public bool I_SetDataOn_I = false;

	[Header("- Collider setup ----------------------")]
	public bool I_ClASetOn_I = false;
	[Space(3)]
	public bool ColliderAutoSetOn = false;//collider登録自動登録

	[Space(10)]
	public bool I_ClRObjOn_I = false;
	[Space(3)]
	public GameObject[] ColliderRootObj;//当たり判定を登録するrootオブジェクト

	[Header("- Collider setting ---------------------")]
	public bool I_JCOSetOn_I = true;
	[Space(3)]
	public bool JointCollisionOn = true;//ジョイントの衝突処理OnOff

	[Header("---------------------------------------")]
	public bool I_ClRASetOn_I = true;
	[Space(3)]
	public bool RadiusAllOn = false;//一括半径指定OnOff
	public float CollisionRadiusAll = 0.005f;//ジョイントの衝突半径

	[Header("---------------------------------------")]
	public bool I_ClRSetOn_I = true;
	[Space(3)]
	public float[] CollisionRadius;//ジョイントの衝突半径リスト

	[Header("---------------------------------------")]
	public bool I_ClSAROn_I = true;
	[Space(3)]
	public float SpeedCRange = 0.05f;//移動の速さの処理切り替えスピード
	public float AddRadius = 1f;//早い移動時の衝突半径に乗算何倍にするか

	[Header("- Group setup ------------------------")]
	public bool I_GroupASetOn_I = false;
	[Space(3)]
	public bool GroupAutoSetOn = false;//連携化データ自動登録

	[Space(10)]
	public bool I_SearchSJOOn_I = false;
	[Space(3)]
	public GameObject[] SearchSJO;//検索先SmoothJoint適用オブジェクト

	[Header("- Group setting -----------------------")]
	public bool I_GroupOn_I = true;
	[Space(3)]
	public bool GroupOn = false;//連携OnOff
	public bool GCollisionOn = false;//連携衝突処理

	[Space(10)]
	public bool I_GroupDataOn_I = true;
	[Space(3)]
	public float AddGroup = 0.4f;//平均化加算率
	public float OffsetGroup = 1f;//位置調整

	[Header("- IK setting --------------------------")]
	public bool I_IKRangeSetOn_I = true;
	[Space(3)]
	public bool FixedTargetOn = false;//末端を固定するOnOff
	public bool[] IKRangeOn;//IKをゴールとの距離で起動OnOff

	[Space(10)]
	public float IKRange = 0.1f;//IKを起動させる範囲
	public float AddIKSpring = 0.1f;//IK揺れ乗算何倍にするか
	public bool IKRangeDecline = true;//IK拘束力を距離で減退

	[Header("- Operation setup ---------------------")]
	public bool I_OpSetOn_I = true;
	[Space(3)]
	public bool FixRoot = true;//rootの処理ON,OFF設定
	//public bool DelayCorrection = true;//処理落補正ON,OFF設定

	[Header("- Effect setting ----------------------")]
	public bool I_MAACSetOn_I = true;
	[Space(3)]
	public float AddEffect = 1f;//エフェクト変型割合(0~1)
	public float MovingResistance = 1f;//耐える(0~1)
	public float AddRectify = 0f;//変型割合をroot近づくほどゆるくする(0~1 1で補正最大)
	public float CollisionFriction = 0f;//衝突中の摩擦

	[Header("---------------------------------------")]
	public bool I_TRSetOn_I = true;
	[Space(3)]
	public bool TwistRestoreX = false;//ねじれ解消回転
	public bool TwistRestoreY = false;
	public bool TwistRestoreZ = false;

	[Space(10)]
	public bool I_TRSSetOn_I = true;
	[Space(3)]
	public float TwistRestoreSpeed = 300f;//ねじれ解消スピード(0~)

	[Header("---------------------------------------")]
	public bool I_FixSetOn_I = true;
	[Space(3)]
	public bool FixX = false; //固定軸指定
	public bool FixY = false;
	public bool FixZ = false;

	[Space(10)]
	public bool I_MMRSetOn_I = true;
	[Space(3)]
	public Vector3 MaxRange = new Vector3(0f, 0f, 0f);//軸回転固定
	public Vector3 MiniRange = new Vector3(0f, 0f, 0f);//軸回転固定

	[Header("- Wind --------------------------------")]
	public bool I_WOSetOn_I = true;
	[Space(3)]
	public bool WindOn = false;//風エフェクトOnOff

	[Space(10)]
	public bool I_WvSetOn_I = true;
	[Space(3)]
	public Vector3 Wind = new Vector3(0f, 0f, 0f);//風エフェクト
	public Vector3 RandWind = new Vector3(0f, 0f, 0f);//風の乱数(0~1 1で補正最大)
	public Vector3 WindPendulum = new Vector3(0.1f, 0.1f, 0.1f);//風の揺れ返し量
	public float WindSpeed = 0f;//風の速度

	[Header("- Gravity -----------------------------")]
	public bool I_GOSetOn_I = true;
	[Space(3)]
	public bool GravityOn = true;//重力処理OnOff

	[Space(10)]
	public bool I_GESetOn_I = true;
	[Space(3)]
	public float Gravity = -0.01f;//1フレームあたりの重力

	[Header("---------------------------------------")]
	public bool I_IOSetOn_I = true;
	[Space(3)]
	public bool InertiaOn = true;//慣性処理OnOff

	[Space(10)]
	public bool I_IESetOn_I = true;
	[Space(3)]
	public float AddInertia = 1f;//慣性加算
	public float InertiaSpeed = 0.1f;//慣性速度(0~1)
	public float InertiaFriction = 0.91f;//慣性減退(0~1)

	[Header("- Centrifugal Force ------------------")]
	public bool I_COSetOn_I = true;
	[Space(3)]
	public bool CentrifugalForceOn = true;//遠心力処理OnOff

	[Space(10)]
	public bool I_CFESetOn_I = true;
	[Space(3)]
	public float CentrifugalForce = 1f;//遠心力

	[Header("- Spring ------------------------------")]
	public bool I_SOSetOn_I = true;
	[Space(3)]
	public bool SpringOn = true;//スプリング処理OnOff

	[Space(10)]
	public bool I_SESetOn_I = true;
	[Space(3)]
	public Vector3 AddSpring = new Vector3(0f, 0f, 0f);//スプリング力加算(0~)
	public float SpringHardness = 0.1f;//スプリング硬さ(0~)
	public float SpringFriction = 0.95f;//スプリング減退(0~1)

	[Header("- Move And Scale setting -------------")]
	public bool I_MSESetOn_I = true;
	[Space(3)]
	public bool MoveAndScaleOn = false;//移動スケール
	public Vector3 AddMove = new Vector3(0f, 0f, 0f);//移動振り幅
	public Vector3 AddScale = new Vector3(0f, 0f, 0f);//スケール変型幅

	[Header("- Pressure ---------------------------")]
	public bool I_POSetOn_I = true;
	[Space(3)]
	public bool PressureOn = false;//圧力処理OnOff
	public float AddPressure = 0f;//圧力乗算何倍にするか
	public float AddPressureGravity = 1f;//重力圧力乗算何倍にするか
	public float AddImpact = 0f;//衝撃力乗算何倍にするか

	[Header("- It is for data confirmation ---------")]
	public bool I_DCSetOn_I = true;
	[Space(3)]
	public bool ColliderAllDisplay = false;//コリジョン表示OnOff

	[Header("- Editer Data -------------------------")]
	public int sjlOld = 0;//エディタ専用データ

	private string[] autoName;
	private string editName = "";
	private int listCount = 0;
	private int nameCount = 0;
	private int editOnCount = 0;
	private int sjlCount = 0;


	public void OnValidate()
	{
		//Inspectorの値を変更すると呼ばれる

		//配列編集された
		if(this.SJList.Count == 0 && this.SJName.Count == 0 && this.SJEditOn.Count == 0)
		{
			this.sjlCount = 0;
		}

		if (this.sjlOld != this.SJList.Count)
		{
			this.SJListSet();
		}
		if (this.sjlOld != this.SJName.Count)
		{
			this.SJListSet();
		}
		if (this.sjlOld != this.SJEditOn.Count)
		{
			this.SJListSet();
		}

		//ボタン処理
		if (this.SJAutoSetOn == true)
		{
			this.SJAutoSetOn = false;

			this.SJListSet();
		}

		if (this.EditerReset == true)
		{
			this.EditerReset = false;

			this.Init();
		}

		//ロード範囲確認
		if (this.SJList.Count != 0)
		{
			if (this.SJList.Count <= this.GetDataListNo || 0 > this.GetDataListNo)
			{
				Debug.Log ("GetDataListNo Range: SJList Size");
				this.GetDataListNo = Mathf.Clamp(this.GetDataListNo, 0, this.SJList.Count - 1);
			}
		}

		//Ｇｅｔ　Ｓｅｔ
		if (this.I_GetDataOn_I == true)
		{
			this.I_GetDataOn_I = false;

			this.GetDataSet();
		}

		if (this.I_SetDataOn_I == true)
		{
			this.I_SetDataOn_I = false;

			this.SetDatSet();
		}

		//設定値安全補正

		//警告
		if(this.CollisionFriction < 0f || this.CollisionFriction > 1f)
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
		}

		if (this.SpringFriction < 0f || this.SpringFriction > 1f)
		{
			Debug.Log ("SpringFriction Range: 0 ~ 1");
		}

		if (this.MovingResistance < 0f || this.MovingResistance > 1f)
		{
			Debug.Log ("MovingResistance Range: 0 ~ 1");
		}

		if (this.AddEffect < 0f || this.AddEffect > 1f)
		{
			Debug.Log ("AddEffect Range: 0 ~ 1");
		}

		if (this.AddRectify < 0f || this.AddRectify > 1f)
		{
			Debug.Log ("AddRectify Range: 0 ~ 1");
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

		//強制補正
		this.SpringHardness = Mathf.Clamp(this.SpringHardness, 0f, 1f);
		this.SpringFriction = Mathf.Clamp(this.SpringFriction, 0f, 1f);
		this.MovingResistance = Mathf.Clamp(this.MovingResistance, 0f, 1f);
		this.AddEffect = Mathf.Clamp(this.AddEffect, 0f, 1f);

		if (this.TwistRestoreSpeed < 0f)
		{
			Debug.Log ("TwistRestoreSpeed Range: 0 ~");
			this.TwistRestoreSpeed = 0f;
		}
	}

	private void Init()
	{
		//初期化
		if (this.SmoothJointObj != null && this.SmoothJointObj.Length > 0)
		{
			if (this.autoName == null)
			{
				this.autoName = new string[this.SmoothJointObj.Length];
			}
			else
			{
				if (this.autoName.Length != this.SmoothJointObj.Length)
				{
					System.Array.Resize(ref this.autoName, this.SmoothJointObj.Length);
				}
			}

			if (this.SJList == null)
			{
				this.SJList = new List<SmoothJoint>();
			}
			this.SJList.Clear();

			if (this.SJName == null)
			{
				this.SJName = new List<string>();
			}
			this.SJName.Clear();

			if (this.SJEditOn == null)
			{
				this.SJEditOn = new List<bool>();
			}
			this.SJEditOn.Clear();

			for (int ia = 0; ia < this.SmoothJointObj.Length; ia++)
			{
				if (this.SmoothJointObj[ia] != null)
				{
					this.autoName[ia] = this.SmoothJointObj[ia].name;

					SmoothJoint[] sjc = this.SmoothJointObj[ia].GetComponents<SmoothJoint>();

					if (sjc != null)
					{
						this.SJList.AddRange(sjc);
					}
					else
					{
						Debug.Log ("In this SmoothJointObj not found: SmoothJoint");
					}
				}
				else
				{
					Debug.Log ("Please set it up: SmoothJointObj");
				}
			}

			if (this.SJList != null && this.SJList.Count > 0)
			{
				this.listCount = this.SJList.Count;

				for (int i = 0; i < this.SJList.Count; i++)
				{
					if (this.SJList[i] != null)
					{
						if (this.SJList[i].RootObj != null)
						{
							this.SJName.Add(this.SJList[i].RootObj.name);
						}
						else
						{
							if (this.SJList[i].JointObj[0] != null)
							{
								this.SJName.Add(this.SJList[i].JointObj[0].name);
							}
							else
							{
								this.SJName.Add("not found");
								Debug.Log ("Please set it up: SmoothJoint");
							}
						}
					}

					this.SJEditOn.Add(true);
				}

				this.nameCount = this.SJName.Count;
				this.editOnCount = this.SJEditOn.Count;
			}
		}
		else
		{
			Debug.Log ("Please set it up: SmoothJointObj & SJAutoSetOn");
		}
	}

	private void SJListSet()
	{
		//自動配列読み込み
		if (this.SmoothJointObj != null)
		{
			bool sef = false;

			//SmoothJointObj変更確認
			int sjc = 0;
			for (int ic = 0; ic < this.SmoothJointObj.Length; ic++)
			{
				if(this.SmoothJointObj[ic] != null)
				{
					sjc++;
				}
			}

			if (this.sjlCount != this.SmoothJointObj.Length)
			{
				this.Init();
			}
			else
			{
				if (sjc != this.SmoothJointObj.Length)
				{
					this.Init();
				}

				//編集ＳＪ名リスト設定
				if (this.autoName == null)
				{
					this.autoName = new string[this.SmoothJointObj.Length];
				}
				else
				{
					if (this.autoName.Length != this.SmoothJointObj.Length)
					{
						this.Init();
					}
				}

				//リストが改変されてるか確認
				for (int ic = 0; ic < this.SmoothJointObj.Length; ic++)
				{
					if (this.SmoothJointObj[ic] != null)
					{
						if (this.autoName[ic] != this.SmoothJointObj[ic].name)
						{
							sef = false;
						}
						else
						{
							sef = true;
						}
					}
				}

				//ユーザーに編集されていたら初期化
				if (sef == false)
				{
					this.Init();
				}

				if (sef == true)
				{
					//どれが編集されたか確認
					if (this.SJList.Count != this.listCount && this.editName == "")
					{
						this.editName = "SJList";
					}
					if (this.SJName.Count != this.nameCount && this.editName == "")
					{
						this.editName = "SJName";
					}
					if (this.SJEditOn.Count != this.editOnCount && this.editName == "")
					{
						this.editName = "SJEditOn";
					}


					switch (this.editName)
					{
					case "SJList":

						this.SJName.Clear();
						this.SJEditOn.Clear();

						for (int i = 0; i < this.SJList.Count; i++)
						{
							if (this.SJList[i] != null)
							{
								if (this.SJList[i].RootObj != null)
								{
									this.SJName.Add(this.SJList[i].RootObj.name);
								}
								else
								{
									if (this.SJList[i].JointObj[0] != null)
									{
										this.SJName.Add(this.SJList[i].JointObj[0].name);
									}
									else
									{
										this.SJName.Add("not found");
										Debug.Log ("Please set it up: SmoothJoint");
									}
								}
							}

							this.SJEditOn.Add(true);
						}

						this.editName = "";
						this.listCount = this.SJList.Count;
						this.nameCount = this.SJName.Count;
						this.editOnCount = this.SJEditOn.Count;

						break;

					case "SJName":

						SmoothJoint[] sja = new SmoothJoint[this.SJName.Count];

						for (int i = 0; i < this.SJName.Count; i++)
						{
							for (int ia = 0; ia < this.SJList.Count; ia++)
							{
								if (this.SJList[ia] != null)
								{
									if (this.SJName[i] == this.SJList[ia].RootObj.name)
									{
										sja[i] = this.SJList[ia];
									}
								}
							}
						}


						this.SJList.Clear();
						this.SJList.AddRange(sja);

						this.SJEditOn.Clear();

						for (int ib = 0; ib < this.SJList.Count; ib++)
						{
							this.SJEditOn.Add(true);
						}

						this.editName = "";
						this.nameCount = this.SJName.Count;
						this.listCount = this.SJList.Count;
						this.editOnCount = this.SJEditOn.Count;

						break;

					case "SJEditOn":

						this.SJEditOn.Clear();

						for (int i = 0; i < this.SJList.Count; i++)
						{
							this.SJEditOn.Add(true);
						}

						this.editName = "";
						this.editOnCount = this.SJEditOn.Count;
						this.listCount = this.SJList.Count;
						this.nameCount = this.SJName.Count;

						break;
					}
				}
			}

			this.sjlCount = sjc;
			this.sjlOld = this.SJList.Count;
			Debug.Log ("SmoothJoint= " + sjc);
		}
		else
		{
			Debug.Log ("Please set it up: SmoothJointObj & SJAutoSetOn");
		}
	}

	private void GetDataSet()
	{
		//データ取得

		if (this.SJList.Count > this.GetDataListNo && 0 <= this.GetDataListNo)
		{

			if (this.I_GroupOn_I == true)
			{
				this.GroupOn = this.SJList[this.GetDataListNo].GroupOn;//連携OnOff
				this.GCollisionOn = this.SJList[this.GetDataListNo].GCollisionOn;//連携衝突処理OnOff
			}

			if (this.I_GroupDataOn_I == true)
			{
				this.AddGroup = this.SJList[this.GetDataListNo].AddGroup;//平均化加算率
				this.OffsetGroup = this.SJList[this.GetDataListNo].OffsetGroup;//位置調整
			}

			if (this.I_SearchSJOOn_I == true)
			{
				//連携検索先SmoothJoint適用オブジェクト
				this.SearchSJO = null;

				this.SearchSJO = this.SJList[this.GetDataListNo].SearchSJO;
			}

			if (this.I_IKRangeSetOn_I == true)
			{
				this.FixedTargetOn = this.SJList[this.GetDataListNo].FixedTargetOn;//末端を固定するOnOff

				this.IKRangeOn = null;//IKをゴールとの距離で起動OnOff
				this.IKRangeOn = this.SJList[this.GetDataListNo].IKRangeOn;
				this.IKRange = this.SJList[this.GetDataListNo].IKRange;//IKを起動させる範囲
				this.AddIKSpring = this.SJList[this.GetDataListNo].AddIKSpring;//IK揺れ加算
				this.IKRangeDecline = this.SJList[this.GetDataListNo].IKRangeDecline;//IK拘束力を距離で減退
			}

			if (this.I_ClRObjOn_I == true)
			{
				//ColliderRootObjの受け渡し
				this.ColliderRootObj = null;

				this.ColliderRootObj = this.SJList[this.GetDataListNo].ColliderRootObj;
			}

			if (this.I_ClRSetOn_I == true)
			{
				//ジョイントの衝突半径リストの受け渡し
				this.CollisionRadius = null;

				this.CollisionRadius = this.SJList[this.GetDataListNo].CollisionRadius;
			}

			if (this.I_JCOSetOn_I == true)
			{
				this.JointCollisionOn = this.SJList[this.GetDataListNo].JointCollisionOn;//ジョイントの衝突処理OnOff
			}

			if (this.I_ClRASetOn_I == true)
			{
				this.CollisionRadiusAll = this.SJList[this.GetDataListNo].CollisionRadiusAll;//ジョイントの衝突半径
			}

			if (this.I_ClSAROn_I == true)
			{
				this.SpeedCRange = this.SJList[this.GetDataListNo].SpeedCRange;//移動の速さの処理切り替えスピード
				this.AddRadius = this.SJList[this.GetDataListNo].AddRadius;//速い移動時の衝突半径に乗算何倍にするか
			}

			if (this.I_OpSetOn_I == true)
			{
				this.FixRoot = this.SJList[this.GetDataListNo].FixRoot;//rootの処理ON,OFF設定
				//this.DelayCorrection = this.SJList[this.GetDataListNo].DelayCorrection;//処理落補正ON,OFF設定
			}

			if (this.I_MAACSetOn_I == true)
			{
				this.AddEffect = this.SJList[this.GetDataListNo].AddEffect;//エフェクト変型割合(0~1)
				this.MovingResistance = this.SJList[this.GetDataListNo].MovingResistance;//耐える(0~1)
				this.AddRectify = this.SJList[this.GetDataListNo].AddRectify;//変型割合調整(0~1)
				this.CollisionFriction = this.SJList[this.GetDataListNo].CollisionFriction;//衝突中の摩擦(0~1)
			}

			if (this.I_TRSetOn_I == true)
			{
				this.TwistRestoreX = this.SJList[this.GetDataListNo].TwistRestoreX;//ねじれ解消回転
				this.TwistRestoreY = this.SJList[this.GetDataListNo].TwistRestoreY;
				this.TwistRestoreZ = this.SJList[this.GetDataListNo].TwistRestoreZ;
			}

			if (this.I_TRSSetOn_I == true)
			{
				this.TwistRestoreSpeed = this.SJList[this.GetDataListNo].TwistRestoreSpeed;//ねじれ解消スピード(0~)
			}

			if (this.I_FixSetOn_I == true)
			{
				this.FixX = this.SJList[this.GetDataListNo].FixX; //固定軸指定
				this.FixY = this.SJList[this.GetDataListNo].FixY;
				this.FixZ = this.SJList[this.GetDataListNo].FixZ;
			}

			if (this.I_MMRSetOn_I == true)
			{
				this.MaxRange = this.SJList[this.GetDataListNo].MaxRange;//軸回転固定
				this.MiniRange = this.SJList[this.GetDataListNo].MiniRange;//軸回転固定
			}

			if (this.I_WOSetOn_I == true)
			{
				this.WindOn = this.SJList[this.GetDataListNo].WindOn;//風エフェクトOnOff
			}

			if (this.I_WvSetOn_I == true)
			{
				this.Wind = this.SJList[this.GetDataListNo].Wind;//風エフェクト
				this.RandWind = this.SJList[this.GetDataListNo].RandWind;//風の乱数
				this.WindPendulum = this.SJList[this.GetDataListNo].WindPendulum;//風の揺れ返し量
				this.WindSpeed = this.SJList[this.GetDataListNo].WindSpeed;//風の速度
			}

			if (this.I_GOSetOn_I == true)
			{
				this.GravityOn = this.SJList[this.GetDataListNo].GravityOn;//重力処理OnOff
			}

			if (this.I_GESetOn_I == true)
			{
				this.Gravity = this.SJList[this.GetDataListNo].Gravity;//1フレームあたりの重力
			}

			if (this.I_IOSetOn_I == true)
			{
				this.InertiaOn = this.SJList[this.GetDataListNo].InertiaOn;//慣性処理OnOff
			}

			if(this.I_IESetOn_I == true)
			{
				this.AddInertia = this.SJList[this.GetDataListNo].AddInertia;//慣性加算
				this.InertiaSpeed = this.SJList[this.GetDataListNo].InertiaSpeed;//慣性速度(0~1)
				this.InertiaFriction = this.SJList[this.GetDataListNo].InertiaFriction;//慣性減退(0~1)
			}

			if (this.I_COSetOn_I == true)
			{
				this.CentrifugalForceOn = this.SJList[this.GetDataListNo].CentrifugalForceOn;//遠心力処理OnOff
			}

			if (this.I_CFESetOn_I == true)
			{
				this.CentrifugalForce = this.SJList[this.GetDataListNo].CentrifugalForce;//遠心力
			}

			if (this.I_SOSetOn_I == true)
			{
				this.SpringOn = this.SJList[this.GetDataListNo].SpringOn;//スプリング処理OnOff
			}

			if (this.I_SESetOn_I == true)
			{
				this.AddSpring = this.SJList[this.GetDataListNo].AddSpring;//スプリング力加算(0~)
				this.SpringHardness = this.SJList[this.GetDataListNo].SpringHardness;//スプリング硬さ(0~)
				this.SpringFriction = this.SJList[this.GetDataListNo].SpringFriction;//スプリング減退(0~1)
			}

			if (this.I_MSESetOn_I == true)
			{
				this.MoveAndScaleOn = this.SJList[this.GetDataListNo].MoveAndScaleOn;//移動スケールOnOff
				this.AddMove = this.SJList[this.GetDataListNo].AddMove;//移動振り幅
				this.AddScale = this.SJList[this.GetDataListNo].AddScale;//スケール変型幅
			}

			if (this.I_POSetOn_I == true)
			{
				this.PressureOn = this.SJList[this.GetDataListNo].PressureOn;//圧力処理OnOff
				this.AddPressure = this.SJList[this.GetDataListNo].AddPressure;//圧力乗算何倍にするか
				this.AddPressureGravity = this.SJList[this.GetDataListNo].AddPressureGravity;//重力圧力乗算何倍にするか
				this.AddImpact = this.SJList[this.GetDataListNo].AddImpact;//衝撃力乗算何倍にするか
			}

			if (this.I_DCSetOn_I == true)
			{
				this.ColliderAllDisplay = this.SJList[this.GetDataListNo].ColliderAllDisplay;//コリジョン表示OnOff
			}
		}
	}


	private void SetDatSet()
	{
		//データ書き換え

		for (int i = 0; i < this.SJList.Count; i++)
		{
			if (SJEditOn[i] == true)
			{
				if (I_GroupASetOn_I == true)
				{
					this.SJList[i].GroupAutoSetOn = this.GroupAutoSetOn;//連携化データ自動登録

					if (this.SJList[i].NexToSJ_0 == null && this.SJList[i].NexToSJ_1 == null)
					{
						Debug.Log ("NexToSJ_0 or NexToSJ_1 is required at SmoothJoint");
					}
					else
					{
						if (this.SJList[i].NexToSJ_0 != null || this.SJList[i].NexToSJ_1 != null)
						{
							this.SJList[i].GroupSJSet();
						}
					}
				}

				if (this.I_GroupOn_I == true)
				{
					this.SJList[i].GroupOn = this.GroupOn;//連携OnOff
					this.SJList[i].GCollisionOn = this.GCollisionOn;//連携衝突処理OnOff
				}

				if (this.I_GroupDataOn_I == true)
				{
					this.SJList[i].AddGroup = this.AddGroup;//平均化加算率
					this.SJList[i].OffsetGroup = this.OffsetGroup;//位置調整
				}

				if (this.I_SearchSJOOn_I == true)
				{
					//連携検索先SmoothJoint適用オブジェクト
					this.SJList[i].SearchSJO = null;

					this.SJList[i].SearchSJO = this.SearchSJO;
				}

				if (this.I_IKRangeSetOn_I == true)
				{

					this.SJList[i].FixedTargetOn = this.FixedTargetOn;//末端を固定するOnOff

					//同じ配列数なら
					if(this.SJList[i].IKRangeOn.Length == this.IKRangeOn.Length)
					{
						this.SJList[i].IKRangeOn = null;//IKをゴールとの距離で起動OnOff
						this.SJList[i].IKRangeOn = this.IKRangeOn;
					}
					else
					{
						//配列の長さが違うのでSJに合わせる
						for (int il = 0; il < this.SJList[i].IKRangeOn.Length; il++)
						{
							if(this.IKRangeOn.Length > il)
							{
								this.SJList[i].IKRangeOn[il] = this.IKRangeOn[il];
							}
						}

						Debug.Log ("The length of the list is different:"
							 + this.SJList[i].IKRangeOn.Length + " / " + this.IKRangeOn.Length);
					}

					this.SJList[i].IKRange = this.IKRange;//IKを起動させる範囲
					this.SJList[i].AddIKSpring = this.AddIKSpring;//IK揺れ加算
					this.SJList[i].IKRangeDecline = this.IKRangeDecline;//IK拘束力を距離で減退
				}

				if (this.I_JCOSetOn_I == true)
				{
					this.SJList[i].JointCollisionOn = this.JointCollisionOn;//ジョイントの衝突処理OnOff
				}

				if (this.I_ClRObjOn_I == true)
				{
					if (this.ColliderRootObj != null) //ColliderRootObjの受け渡し
					{
						this.SJList[i].ColliderRootObj = null;

						this.SJList[i].ColliderRootObj = this.ColliderRootObj;
					}
					else
					{
						Debug.Log ("Please set it up: Collider Root Obj");
					}
				}

				if (this.I_ClASetOn_I == true)
				{
					this.SJList[i].ColliderAutoSetOn = this.ColliderAutoSetOn;//collider登録自動登録

					if (this.ColliderAutoSetOn == true)
					{
						if (this.SJList[i].ColliderRootObj != null)
						{
							this.SJList[i].ChildAutoSet();
						}
						else
						{
							Debug.Log ("In this SmoothJoint not found: ColliderRootObj");
						}
					}
				}

				if (this.I_ClRASetOn_I == true)
				{
					this.SJList[i].CollisionRadiusAll = this.CollisionRadiusAll;//jointの衝突半径

					this.SJList[i].RadiusAllOn = this.RadiusAllOn;//一括半径指定OnOff

					if (this.RadiusAllOn == true)
					{
						this.SJList[i].CollRadiusSet();
					}
				}

				if (this.I_ClRSetOn_I == true)
				{
					//ジョイントの衝突半径リストの受け渡し

					//同じ配列数なら
					if(this.SJList[i].CollisionRadius.Length == this.CollisionRadius.Length)
					{
						this.SJList[i].CollisionRadius = null;

						this.SJList[i].CollisionRadius = this.CollisionRadius;
					}
					else
					{
						//配列の長さが違うのでSJに合わせる
						for (int il = 0; il < this.SJList[i].CollisionRadius.Length; il++)
						{
							if(this.CollisionRadius.Length > il)
							{
								this.SJList[i].CollisionRadius[il] = this.CollisionRadius[il];
							}
						}

						Debug.Log ("The length of the list is different:"
							 + this.SJList[i].CollisionRadius.Length + " / " + this.CollisionRadius.Length);
					}
				}

				if (this.I_ClSAROn_I == true)
				{
					this.SJList[i].SpeedCRange = this.SpeedCRange;//移動の速さの処理切り替えスピード
					this.SJList[i].AddRadius = this.AddRadius;//速い移動時の衝突半径に乗算何倍にするか
				}

				if (this.I_OpSetOn_I == true)
				{
					this.SJList[i].FixRoot = this.FixRoot;//rootの処理ON,OFF設定
					//this.SJList[i].DelayCorrection = this.DelayCorrection;//処理落補正ON,OFF設定
				}

				if (this.I_MAACSetOn_I == true)
				{
					this.SJList[i].AddEffect = this.AddEffect;//エフェクト変型割合(0~1)
					this.SJList[i].MovingResistance = this.MovingResistance;//耐える(0~1)
					this.SJList[i].AddRectify = this.AddRectify;//変型割合調整(0~1)
					this.SJList[i].CollisionFriction = this.CollisionFriction;//衝突中の摩擦(0~1)
				}

				if (this.I_TRSetOn_I == true)
				{
					this.SJList[i].TwistRestoreX = this.TwistRestoreX;//ねじれ解消回転
					this.SJList[i].TwistRestoreY = this.TwistRestoreY;
					this.SJList[i].TwistRestoreZ = this.TwistRestoreZ;
				}

				if (this.I_TRSSetOn_I == true)
				{
					this.SJList[i].TwistRestoreSpeed = this.TwistRestoreSpeed;//ねじれ解消スピード(0~)
				}

				if (this.I_FixSetOn_I == true)
				{
					this.SJList[i].FixX = this.FixX; //固定軸指定
					this.SJList[i].FixY = this.FixY;
					this.SJList[i].FixZ = this.FixZ;
				}

				if (this.I_MMRSetOn_I == true)
				{
					this.SJList[i].MaxRange = this.MaxRange;//軸回転固定
					this.SJList[i].MiniRange = this.MiniRange;//軸回転固定
				}

				if (this.I_WOSetOn_I == true)
				{
					this.SJList[i].WindOn = this.WindOn;//風エフェクトOnOff
				}

				if (this.I_WvSetOn_I == true)
				{
					this.SJList[i].Wind = this.Wind;//風エフェクト
					this.SJList[i].RandWind = this.RandWind;//風の乱数
					this.SJList[i].WindPendulum = this.WindPendulum;//風の揺れ返し量
					this.SJList[i].WindSpeed = this.WindSpeed;//風の速度
				}

				if (this.I_GOSetOn_I == true)
				{
					this.SJList[i].GravityOn = this.GravityOn;//重力処理OnOff
				}

				if (this.I_GESetOn_I == true)
				{
					this.SJList[i].Gravity = this.Gravity;//1フレームあたりの重力
				}

				if (this.I_IOSetOn_I == true)
				{
					this.SJList[i].InertiaOn = this.InertiaOn;//慣性処理OnOff
				}

				if (this.I_IESetOn_I == true)
				{
					this.SJList[i].AddInertia = this.AddInertia;//慣性加算
					this.SJList[i].InertiaSpeed = this.InertiaSpeed;//慣性速度(0~1)
					this.SJList[i].InertiaFriction = this.InertiaFriction;//慣性減退(0~1)
				}

				if (this.I_COSetOn_I == true)
				{
					this.SJList[i].CentrifugalForceOn = this.CentrifugalForceOn;//遠心力処理OnOff
				}

				if (this.I_CFESetOn_I == true)
				{
					this.SJList[i].CentrifugalForce = this.CentrifugalForce;//遠心力
				}

				if (this.I_SOSetOn_I == true)
				{
					this.SJList[i].SpringOn = this.SpringOn;//スプリング処理OnOff
				}

				if (this.I_SESetOn_I == true)
				{
					this.SJList[i].AddSpring = this.AddSpring;//スプリング力加算(0~)
					this.SJList[i].SpringHardness = this.SpringHardness;//スプリング硬さ(0~)
					this.SJList[i].SpringFriction = this.SpringFriction;//スプリング減退(0~1)
				}

				if (this.I_MSESetOn_I == true)
				{
					this.SJList[i].MoveAndScaleOn = this.MoveAndScaleOn;//移動スケールOnOff
					this.SJList[i].AddMove = this.AddMove;//移動振り幅
					this.SJList[i].AddScale = this.AddScale;//スケール変型幅
				}

				if (this.I_POSetOn_I == true)
				{
					this.SJList[i].PressureOn = this.PressureOn;//圧力処理OnOff
					this.SJList[i].AddPressure = this.AddPressure;//圧力乗算何倍にするか
					this.SJList[i].AddPressureGravity = this.AddPressureGravity;//重力圧力乗算何倍にするか
					this.SJList[i].AddImpact = this.AddImpact;//衝撃力乗算何倍にするか
				}

				if (this.I_DCSetOn_I == true)
				{
					this.SJList[i].ColliderAllDisplay = this.ColliderAllDisplay;//コリジョン表示OnOff
				}

				//データ更新指示
				this.SJList[i].SJDataReset();
			}

			//ボタン初期化
			if (i == this.SJList.Count - 1)
			{
				if (this.I_ClRObjOn_I == true)
				{
					this.I_ClRObjOn_I = false;
				}

				if (this.I_ClASetOn_I == true)
				{
					this.ColliderAutoSetOn = false;
					this.I_ClASetOn_I = false;
				}

				if (this.I_SearchSJOOn_I == true)
				{
					this.I_SearchSJOOn_I = false;
				}

				if (I_GroupASetOn_I == true)
				{
					this.GroupAutoSetOn = false;
					this.I_GroupASetOn_I = false;
				}

				if (this.I_ClRASetOn_I == true)
				{
					this.RadiusAllOn = false;
					this.I_ClRASetOn_I = false;
				}

				if (this.I_ClRSetOn_I == true)
				{
					this.I_ClRSetOn_I = false;
				}
			}
		}

		Debug.Log ("Data updated");
	}

#endif
}
