using UnityEngine;
using System.Collections;

//Moving local GameObject 20170703
public class MoveLPosSet : MonoBehaviour {

	public bool ChildAutoSetOn = false;//Child hierarchy automatic registration
	public GameObject RootObj;//root parent to start a hierarchical search

	[Space(10)]
	public GameObject[] JointObj;//Object to moved

	[Space(10)]
	public SmoothJoint SmoothJointObj;//SmoothJoint
	public SmoothJointCatch SJCatchObj;//SmoothJointCatch

	[Space(10)]
	public bool StartMPS = false;//start
	public bool MovePower = false;//To the moving speed is root change

	[Space(10)]
	public Vector3 MoveVec;//Movement direction vector
	public float ShotMoveSpeed = 0.2f;//Movement speed
	public float ReturnMoveSpeed = 0.005f;//Return movement speed
	public float EndRange = 1f;//Distance to finish the move

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
		//Initialization
		if(this.JointObj != null)
		{
			this.sysTime = Time.fixedDeltaTime;//For load measurement
			this.fpsTime = 1f;
			this.vecData = 0f;//Move data
			this.setSJ = false;//SmoothJoint setting start
			this.sjvpd = 0f;//Value of the addition to SmoothJoint
			this.hitIK = false;//IK Start confirmation

			if(this.SmoothJointObj != null)
			{
				//SmoothJoint adjustment
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
		//Child hierarchy automatic registration

		if (this.ChildAutoSetOn == true)
		{
			if (this.RootObj == null)
			{
				Debug.Log ("Please set it up: RootObj");

				this.ChildAutoSetOn = false;
			}
			else
			{
				//The total number of the hierarchy of children
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
					//Array initialization
					if (this.JointObj != null)
					{
						System.Array.Resize(ref this.JointObj, cnd);
					}
					else
					{
						this.JointObj = new GameObject[cnd];
					}

					//Registration process in the array
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

				//SmoothJoint setting
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
		//SmoothJoint setting

		float ikg = 1f;

		switch (sef)
		{
		case 0:
			//Initialization
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
			//start
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
			//end
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
		//Moving process

		if(sef == true)
		{
			//start

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
			//End
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
				//Move
				if(this.MovePower == true)
				{
					//Moving distance adjustment
					if(ai > 0 && sjhd == Vector3.zero)
					{
						float pd = this.powd * ai;

						this.JointObj[ai].transform.localPosition = this.basePos[ai] +
							((this.MoveVec * this.vecData) * pd);
					}
				}
				else
				{
					//At regular intervals move
					if(ai > 0 && sjhd == Vector3.zero)
					{
						this.JointObj[ai].transform.localPosition = this.basePos[ai] +
							(this.MoveVec * this.vecData);
					}
				}

				//rotation
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

				//Initialization
				if(this.vecData < 0.0003f)
				{
					this.JointObj[ai].transform.localPosition = this.basePos[ai];
					this.JointObj[ai].transform.localRotation = this.baseRot[ai];
				}
			}
		}
	}
}
