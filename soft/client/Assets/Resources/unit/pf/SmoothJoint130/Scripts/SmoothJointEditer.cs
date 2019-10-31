using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Edit together SmoothJoint parameters ver 1.3.2 20170710
public class SmoothJointEditer : MonoBehaviour {
#if UNITY_EDITOR

	[Header("- Initial setup ----------------------")]
	public bool SJAutoSetOn = false;//Registered in the automatic SmoothJoint
	public bool EditerReset = false;//Initialization

	[Space(10)]
	public GameObject[] SmoothJointObj;//Object to search for SmoothJoint
	public List<SmoothJoint> SJList;//SmoothJoint
	public List<string> SJName;//SmoothJoint name
	public List<bool> SJEditOn;//SmoothJoint On Off

	[Header("- To read data -----------------------")]
	public bool I_GetDataOn_I = false;
	public int GetDataListNo = 0;//Arrays number of the registered

	[Header("- All the data are changed ----------")]
	public bool I_SetDataOn_I = false;

	[Header("- Collider setup ----------------------")]
	public bool I_ClASetOn_I = false;
	[Space(3)]
	public bool ColliderAutoSetOn = false;//Automatic registration of the collider

	[Space(10)]
	public bool I_ClRObjOn_I = false;
	[Space(3)]
	public GameObject[] ColliderRootObj;//root object to register a collision detection

	[Header("- Collider setting ---------------------")]
	public bool I_JCOSetOn_I = true;
	[Space(3)]
	public bool JointCollisionOn = true;//Joint of the collision process

	[Header("---------------------------------------")]
	public bool I_ClRASetOn_I = true;
	[Space(3)]
	public bool RadiusAllOn = false;//Bulk range specification
	public float CollisionRadiusAll = 0.005f;//Radius designation

	[Header("---------------------------------------")]
	public bool I_ClRSetOn_I = true;
	[Space(3)]
	public float[] CollisionRadius;//Collision of joint radius

	[Header("---------------------------------------")]
	public bool I_ClSAROn_I = true;
	[Space(3)]
	public float SpeedCRange = 0.05f;//Speed switching between the processing of the speed of movement
	//(The lower the number, the sensitive)
	public float AddRadius = 1f;//Added to the collision radius at a fast move

	[Header("- Group setup ------------------------")]
	public bool I_GroupASetOn_I = false;
	[Space(3)]
	public bool GroupAutoSetOn = false;//Automatic registration of the averaged data

	[Space(10)]
	public bool I_SearchSJOOn_I = false;
	[Space(3)]
	public GameObject[] SearchSJO;//Object to which you want to apply the search destination SmoothJoint

	[Header("- Group setting -----------------------")]
	public bool I_GroupOn_I = true;
	[Space(3)]
	public bool GroupOn = false;//Cooperation
	public bool GCollisionOn = false;//Collaboration collision processing

	[Space(10)]
	public bool I_GroupDataOn_I = true;
	[Space(3)]
	public float AddGroup = 0.4f;//The addition rate to average
	public float OffsetGroup = 1f;//Positioning

	[Header("- IK setting --------------------------")]
	public bool I_IKRangeSetOn_I = true;
	[Space(3)]
	public bool FixedTargetOn = false;//To fix the end
	public bool[] IKRangeOn;//To start at a distance of the goal

	[Space(10)]
	public float IKRange = 0.1f;//It started to range
	public float AddIKSpring = 0.1f;//Addition of shaking
	public bool IKRangeDecline = true;//Decline the binding force in the distance

	[Header("- Operation setup ---------------------")]
	public bool I_OpSetOn_I = true;
	[Space(3)]
	public bool FixRoot = true;//Processing of root
	//public bool DelayCorrection = true;//Correction that the process is slow

	[Header("- Effect setting ----------------------")]
	public bool I_MAACSetOn_I = true;
	[Space(3)]
	public float AddEffect = 1f;//Adaptation amount of the effect (amount of deformation from the original shape(0~1)
	public float MovingResistance = 1f;//Remain in the original location(0~1)
	public float AddRectify = 0f;//To reduce the deformation rate closer root(0~1 Maximum correction in 1)
	public float CollisionFriction = 0f;//Friction during collision(0~1 Effect value is lost by 1)

	[Header("---------------------------------------")]
	public bool I_TRSetOn_I = true;
	[Space(3)]
	public bool TwistRestoreX = false;//Rotation to eliminate the twist
	public bool TwistRestoreY = false;
	public bool TwistRestoreZ = false;

	[Space(10)]
	public bool I_TRSSetOn_I = true;
	[Space(3)]
	public float TwistRestoreSpeed = 300f;//Speed to eliminate the twist(0~)

	[Header("---------------------------------------")]
	public bool I_FixSetOn_I = true;
	[Space(3)]
	public bool FixX = false;//Fixed axis specification
	public bool FixY = false;
	public bool FixZ = false;

	[Space(10)]
	public bool I_MMRSetOn_I = true;
	[Space(3)]
	public Vector3 MaxRange = new Vector3(0f, 0f, 0f);//Maximum angle
	public Vector3 MiniRange = new Vector3(0f, 0f, 0f);//Minimum angle

	[Header("- Wind --------------------------------")]
	public bool I_WOSetOn_I = true;
	[Space(3)]
	public bool WindOn = false;//Wind Effects

	[Space(10)]
	public bool I_WvSetOn_I = true;
	[Space(3)]
	public Vector3 Wind = new Vector3(0f, 0f, 0f);//The direction of the wind
	public Vector3 RandWind = new Vector3(0f, 0f, 0f);//Random number of wind (0~1 1 corrected maximum)
	public Vector3 WindPendulum = new Vector3(0.1f, 0.1f, 0.1f);//Amount to return the wind
	public float WindSpeed = 0f;//Speed of the wind

	[Header("- Gravity -----------------------------")]
	public bool I_GOSetOn_I = true;
	[Space(3)]
	public bool GravityOn = true;//gravity Effects

	[Space(10)]
	public bool I_GESetOn_I = true;
	[Space(3)]
	public float Gravity = -0.01f;//Gravity per frame

	[Header("---------------------------------------")]
	public bool I_IOSetOn_I = true;
	[Space(3)]
	public bool InertiaOn = true;//inertia Effects

	[Space(10)]
	public bool I_IESetOn_I = true;
	[Space(3)]
	public float AddInertia = 1f;//Addition of inertia
	public float InertiaSpeed = 0.1f;//Speed of inertia(0~1)
	public float InertiaFriction = 0.91f;//Decline of inertia(0~1)

	[Header("- Centrifugal Force ------------------")]
	public bool I_COSetOn_I = true;
	[Space(3)]
	public bool CentrifugalForceOn = true;//Centrifugal force Effects

	[Space(10)]
	public bool I_CFESetOn_I = true;
	[Space(3)]
	public float CentrifugalForce = 1f;//Centrifugal force

	[Header("- Spring ------------------------------")]
	public bool I_SOSetOn_I = true;
	[Space(3)]
	public bool SpringOn = true;//Sway Effects

	[Space(10)]
	public bool I_SESetOn_I = true;
	[Space(3)]
	public Vector3 AddSpring = new Vector3(0f, 0f, 0f);//Addition of swaying force(0~)
	public float SpringHardness = 0.1f;//The hardness of the swaying force(0~1)
	public float SpringFriction = 0.95f;//Decline of swaying force(0~1)

	[Header("- Move And Scale setting -------------")]
	public bool I_MSESetOn_I = true;
	[Space(3)]
	public bool MoveAndScaleOn = false;//Move and scale Effects
	public Vector3 AddMove = new Vector3(0f, 0f, 0f);//Move deflection width
	public Vector3 AddScale = new Vector3(0f, 0f, 0f);//Scale deformation width

	[Header("- Pressure ---------------------------")]
	public bool I_POSetOn_I = true;
	[Space(3)]
	public bool PressureOn = false;//pressure Effects
	public float AddPressure = 0f;//Calculation of pressure
	public float AddPressureGravity = 1f;//Pressure due to gravity
	public float AddImpact = 0f;//Addition by the impact force

	[Header("- It is for data confirmation ---------")]
	public bool I_DCSetOn_I = true;
	[Space(3)]
	public bool ColliderAllDisplay = false;//Display of collision

	[Header("- Editer Data -------------------------")]
	public int sjlOld = 0;//Editor-only data

	private string[] autoName;
	private string editName = "";
	private int listCount = 0;
	private int nameCount = 0;
	private int editOnCount = 0;
	private int sjlCount = 0;


	public void OnValidate()
	{
		//Edited Arrays
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

		//Button processing
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

		//Load range confirmation
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

		//Safety correction of the set value

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

		//Force correction
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
		//Initialization
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
		//Registration of automatic array
		if (this.SmoothJointObj != null)
		{
			bool sef = false;

			//Change confirmation of SmoothJointObj
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

				//Edit SJ name list set
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

				//List to make sure that your have been modified
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

				//Initialization After being edited to the user
				if (sef == false)
				{
					this.Init();
				}

				if (sef == true)
				{
					//Sure which it has been edited
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
		//Data acquisition

		if (this.SJList.Count > this.GetDataListNo && 0 <= this.GetDataListNo)
		{
			if (this.I_GroupOn_I == true)
			{
				this.GroupOn = this.SJList[this.GetDataListNo].GroupOn;
				this.GCollisionOn = this.SJList[this.GetDataListNo].GCollisionOn;
			}

			if (this.I_GroupDataOn_I == true)
			{
				this.AddGroup = this.SJList[this.GetDataListNo].AddGroup;
				this.OffsetGroup = this.SJList[this.GetDataListNo].OffsetGroup;
			}

			if (this.I_SearchSJOOn_I == true)
			{
				this.SearchSJO = null;

				this.SearchSJO = this.SJList[this.GetDataListNo].SearchSJO;
			}

			if (this.I_IKRangeSetOn_I == true)
			{
				this.FixedTargetOn = this.SJList[this.GetDataListNo].FixedTargetOn;

				this.IKRangeOn = null;
				this.IKRangeOn = this.SJList[this.GetDataListNo].IKRangeOn;
				this.IKRange = this.SJList[this.GetDataListNo].IKRange;
				this.AddIKSpring = this.SJList[this.GetDataListNo].AddIKSpring;
				this.IKRangeDecline = this.SJList[this.GetDataListNo].IKRangeDecline;
			}

			if (this.I_ClRObjOn_I == true)
			{
				this.ColliderRootObj = null;

				this.ColliderRootObj = this.SJList[this.GetDataListNo].ColliderRootObj;
			}

			if (this.I_ClRSetOn_I == true)
			{
				this.CollisionRadius = null;

				this.CollisionRadius = this.SJList[this.GetDataListNo].CollisionRadius;
			}

			if (this.I_JCOSetOn_I == true)
			{
				this.JointCollisionOn = this.SJList[this.GetDataListNo].JointCollisionOn;
			}

			if (this.I_ClRASetOn_I == true)
			{
				this.CollisionRadiusAll = this.SJList[this.GetDataListNo].CollisionRadiusAll;
			}

			if (this.I_ClSAROn_I == true)
			{
				this.SpeedCRange = this.SJList[this.GetDataListNo].SpeedCRange;
				this.AddRadius = this.SJList[this.GetDataListNo].AddRadius;
			}

			if (this.I_OpSetOn_I == true)
			{
				this.FixRoot = this.SJList[this.GetDataListNo].FixRoot;
				//this.DelayCorrection = this.SJList[this.GetDataListNo].DelayCorrection;
			}

			if (this.I_MAACSetOn_I == true)
			{
				this.AddEffect = this.SJList[this.GetDataListNo].AddEffect;
				this.MovingResistance = this.SJList[this.GetDataListNo].MovingResistance;
				this.AddRectify = this.SJList[this.GetDataListNo].AddRectify;
				this.CollisionFriction = this.SJList[this.GetDataListNo].CollisionFriction;
			}

			if (this.I_TRSetOn_I == true)
			{
				this.TwistRestoreX = this.SJList[this.GetDataListNo].TwistRestoreX;
				this.TwistRestoreY = this.SJList[this.GetDataListNo].TwistRestoreY;
				this.TwistRestoreZ = this.SJList[this.GetDataListNo].TwistRestoreZ;
			}

			if (this.I_TRSSetOn_I == true)
			{
				this.TwistRestoreSpeed = this.SJList[this.GetDataListNo].TwistRestoreSpeed;
			}

			if (this.I_FixSetOn_I == true)
			{
				this.FixX = this.SJList[this.GetDataListNo].FixX;
				this.FixY = this.SJList[this.GetDataListNo].FixY;
				this.FixZ = this.SJList[this.GetDataListNo].FixZ;
			}

			if (this.I_MMRSetOn_I == true)
			{
				this.MaxRange = this.SJList[this.GetDataListNo].MaxRange;
				this.MiniRange = this.SJList[this.GetDataListNo].MiniRange;
			}

			if (this.I_WOSetOn_I == true)
			{
				this.WindOn = this.SJList[this.GetDataListNo].WindOn;
			}

			if (this.I_WvSetOn_I == true)
			{
				this.Wind = this.SJList[this.GetDataListNo].Wind;
				this.RandWind = this.SJList[this.GetDataListNo].RandWind;
				this.WindPendulum = this.SJList[this.GetDataListNo].WindPendulum;
				this.WindSpeed = this.SJList[this.GetDataListNo].WindSpeed;
			}

			if (this.I_GOSetOn_I == true)
			{
				this.GravityOn = this.SJList[this.GetDataListNo].GravityOn;
			}

			if (this.I_GESetOn_I == true)
			{
				this.Gravity = this.SJList[this.GetDataListNo].Gravity;
			}

			if (this.I_IOSetOn_I == true)
			{
				this.InertiaOn = this.SJList[this.GetDataListNo].InertiaOn;
			}

			if(this.I_IESetOn_I == true)
			{
				this.AddInertia = this.SJList[this.GetDataListNo].AddInertia;
				this.InertiaSpeed = this.SJList[this.GetDataListNo].InertiaSpeed;
				this.InertiaFriction = this.SJList[this.GetDataListNo].InertiaFriction;
			}

			if (this.I_COSetOn_I == true)
			{
				this.CentrifugalForceOn = this.SJList[this.GetDataListNo].CentrifugalForceOn;
			}

			if (this.I_CFESetOn_I == true)
			{
				this.CentrifugalForce = this.SJList[this.GetDataListNo].CentrifugalForce;
			}

			if (this.I_SOSetOn_I == true)
			{
				this.SpringOn = this.SJList[this.GetDataListNo].SpringOn;
			}

			if (this.I_SESetOn_I == true)
			{
				this.AddSpring = this.SJList[this.GetDataListNo].AddSpring;
				this.SpringHardness = this.SJList[this.GetDataListNo].SpringHardness;
				this.SpringFriction = this.SJList[this.GetDataListNo].SpringFriction;
			}

			if (this.I_MSESetOn_I == true)
			{
				this.MoveAndScaleOn = this.SJList[this.GetDataListNo].MoveAndScaleOn;
				this.AddMove = this.SJList[this.GetDataListNo].AddMove;
				this.AddScale = this.SJList[this.GetDataListNo].AddScale;
			}

			if (this.I_POSetOn_I == true)
			{
				this.PressureOn = this.SJList[this.GetDataListNo].PressureOn;
				this.AddPressure = this.SJList[this.GetDataListNo].AddPressure;
				this.AddPressureGravity = this.SJList[this.GetDataListNo].AddPressureGravity;
				this.AddImpact = this.SJList[this.GetDataListNo].AddImpact;
			}

			if (this.I_DCSetOn_I == true)
			{
				this.ColliderAllDisplay = this.SJList[this.GetDataListNo].ColliderAllDisplay;
			}
		}
	}


	private void SetDatSet()
	{
		//Data rewriting

		for (int i = 0; i < this.SJList.Count; i++)
		{
			if (SJEditOn[i] == true)
			{
				if (I_GroupASetOn_I == true)
				{
					this.SJList[i].GroupAutoSetOn = this.GroupAutoSetOn;

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
					this.SJList[i].GroupOn = this.GroupOn;
					this.SJList[i].GCollisionOn = this.GCollisionOn;
				}

				if (this.I_GroupDataOn_I == true)
				{
					this.SJList[i].AddGroup = this.AddGroup;
					this.SJList[i].OffsetGroup = this.OffsetGroup;
				}

				if (this.I_SearchSJOOn_I == true)
				{
					this.SJList[i].SearchSJO = null;

					this.SJList[i].SearchSJO = this.SearchSJO;
				}

				if (this.I_IKRangeSetOn_I == true)
				{

					this.SJList[i].FixedTargetOn = this.FixedTargetOn;

					if(this.SJList[i].IKRangeOn.Length == this.IKRangeOn.Length)
					{
						this.SJList[i].IKRangeOn = null;
						this.SJList[i].IKRangeOn = this.IKRangeOn;
					}
					else
					{
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

					this.SJList[i].IKRange = this.IKRange;
					this.SJList[i].AddIKSpring = this.AddIKSpring;
					this.SJList[i].IKRangeDecline = this.IKRangeDecline;
				}

				if (this.I_JCOSetOn_I == true)
				{
					this.SJList[i].JointCollisionOn = this.JointCollisionOn;
				}

				if (this.I_ClRObjOn_I == true)
				{
					if (this.ColliderRootObj != null)
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
					this.SJList[i].ColliderAutoSetOn = this.ColliderAutoSetOn;

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
					this.SJList[i].CollisionRadiusAll = this.CollisionRadiusAll;

					this.SJList[i].RadiusAllOn = this.RadiusAllOn;

					if (this.RadiusAllOn == true)
					{
						this.SJList[i].CollRadiusSet();
					}
				}

				if (this.I_ClRSetOn_I == true)
				{
					if(this.SJList[i].CollisionRadius.Length == this.CollisionRadius.Length)
					{
						this.SJList[i].CollisionRadius = null;

						this.SJList[i].CollisionRadius = this.CollisionRadius;
					}
					else
					{
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
					this.SJList[i].SpeedCRange = this.SpeedCRange;
					this.SJList[i].AddRadius = this.AddRadius;
				}

				if (this.I_OpSetOn_I == true)
				{
					this.SJList[i].FixRoot = this.FixRoot;
					//this.SJList[i].DelayCorrection = this.DelayCorrection;
				}

				if (this.I_MAACSetOn_I == true)
				{
					this.SJList[i].AddEffect = this.AddEffect;
					this.SJList[i].MovingResistance = this.MovingResistance;
					this.SJList[i].AddRectify = this.AddRectify;
					this.SJList[i].CollisionFriction = this.CollisionFriction;
				}

				if (this.I_TRSetOn_I == true)
				{
					this.SJList[i].TwistRestoreX = this.TwistRestoreX;
					this.SJList[i].TwistRestoreY = this.TwistRestoreY;
					this.SJList[i].TwistRestoreZ = this.TwistRestoreZ;
				}

				if (this.I_TRSSetOn_I == true)
				{
					this.SJList[i].TwistRestoreSpeed = this.TwistRestoreSpeed;
				}

				if (this.I_FixSetOn_I == true)
				{
					this.SJList[i].FixX = this.FixX;
					this.SJList[i].FixY = this.FixY;
					this.SJList[i].FixZ = this.FixZ;
				}

				if (this.I_MMRSetOn_I == true)
				{
					this.SJList[i].MaxRange = this.MaxRange;
					this.SJList[i].MiniRange = this.MiniRange;
				}

				if (this.I_WOSetOn_I == true)
				{
					this.SJList[i].WindOn = this.WindOn;
				}

				if (this.I_WvSetOn_I == true)
				{
					this.SJList[i].Wind = this.Wind;
					this.SJList[i].RandWind = this.RandWind;
					this.SJList[i].WindPendulum = this.WindPendulum;
					this.SJList[i].WindSpeed = this.WindSpeed;
				}

				if (this.I_GOSetOn_I == true)
				{
					this.SJList[i].GravityOn = this.GravityOn;
				}

				if (this.I_GESetOn_I == true)
				{
					this.SJList[i].Gravity = this.Gravity;
				}

				if (this.I_IOSetOn_I == true)
				{
					this.SJList[i].InertiaOn = this.InertiaOn;
				}

				if (this.I_IESetOn_I == true)
				{
					this.SJList[i].AddInertia = this.AddInertia;
					this.SJList[i].InertiaSpeed = this.InertiaSpeed;
					this.SJList[i].InertiaFriction = this.InertiaFriction;
				}

				if (this.I_COSetOn_I == true)
				{
					this.SJList[i].CentrifugalForceOn = this.CentrifugalForceOn;
				}

				if (this.I_CFESetOn_I == true)
				{
					this.SJList[i].CentrifugalForce = this.CentrifugalForce;
				}

				if (this.I_SOSetOn_I == true)
				{
					this.SJList[i].SpringOn = this.SpringOn;
				}

				if (this.I_SESetOn_I == true)
				{
					this.SJList[i].AddSpring = this.AddSpring;
					this.SJList[i].SpringHardness = this.SpringHardness;
					this.SJList[i].SpringFriction = this.SpringFriction;
				}

				if (this.I_MSESetOn_I == true)
				{
					this.SJList[i].MoveAndScaleOn = this.MoveAndScaleOn;
					this.SJList[i].AddMove = this.AddMove;
					this.SJList[i].AddScale = this.AddScale;
				}

				if (this.I_POSetOn_I == true)
				{
					this.SJList[i].PressureOn = this.PressureOn;
					this.SJList[i].AddPressure = this.AddPressure;
					this.SJList[i].AddPressureGravity = this.AddPressureGravity;
					this.SJList[i].AddImpact = this.AddImpact;
				}

				if (this.I_DCSetOn_I == true)
				{
					this.SJList[i].ColliderAllDisplay = this.ColliderAllDisplay;
				}

				//Data update instruction
				this.SJList[i].SJDataReset();
			}

			//Button initialization
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
