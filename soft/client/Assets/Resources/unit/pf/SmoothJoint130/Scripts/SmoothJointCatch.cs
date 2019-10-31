using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Installing the IK gol the collision point of SmoothJoint ver 1.3.0 201701
public class SmoothJointCatch : MonoBehaviour {

	[Header("- Initial setup ----------------------")]
	public bool SJAutoSetOn = false;//Registered in the automatic SmoothJoint

	[Space(10)]
	public GameObject[] SmoothJointObj;//Objects to search for SmoothJoint

	[Header("- Catch setup ------------------------")]
	public bool SmoothCatchOn = true;//SmoothCatch On Off
	public bool AvoidanceOn = true;//Avoid gol embedment
	public bool RangeOutOn = false;//Or joint to disable when it is drawn out of the range of IK

	[Space(10)]
	public bool WorldPos = false;//To world coordinates
	public float Friction = 0.99f;//Frictional resistance

	[Space(10)]
	public bool AddRandom = false;//Adding a variable to the collision coordinate
	public Vector3 AddRand = new Vector3(1f, 1f, 1f);//Addition of random numbers

	[Header("- Catch Data -------------------------")]
	public bool[][] Catch;//Constrain (for user operation)
	public List<SmoothJoint> SJList;//SmoothJoint
	public Vector3[][] basePos, baseScale;
	public Quaternion[][] baseRot;

	private Vector3[][] oldPos;
	private bool[][] rndF, hitF;

	public bool[][] CatchSet
	{
		get{return this.Catch;}
		set
		{
			this.Catch = value;
		}
	}

	public void CatchAllClear()
	{
		//Initialization of all of Catch flag
		if (this.SJList != null && this.SJList.Count > 0)
		{
			for (int i = 0; i < this.SJList.Count; i++)
			{
				SmoothJoint sjc = this.SJList[i];

				//IK sure there
				if (sjc != null && this.Catch != null && this.Catch[i] != null)
				{
					for (int ai = 0; ai < this.Catch[i].Length; ai++)
					{
						this.Catch[i][ai] = false;
					}
				}
			}
		}
	}

	public void OnValidate()
	{
		//Button processing
		if (this.SJAutoSetOn == true)
		{
			this.SJAutoSetOn = false;

			this.Init();
		}
	}

	public void Awake()
	{
		this.BasePosSet();
	}

	private void Init()
	{
		//Initialization

		//Safety correction settings
		if (this.Friction < 0f || this.Friction > 1f)
		{
			Debug.Log ("Friction Range: 0 ~ 1");
			this.Friction = Mathf.Clamp(this.Friction, 0f, 1f);
		}

		if (this.SmoothJointObj != null)
		{
			if (this.SJList == null)
			{
				this.SJList = new List<SmoothJoint>();
			}
			this.SJList.Clear();

			for (int ia = 0; ia < this.SmoothJointObj.Length; ia++)
			{
				if (this.SmoothJointObj[ia] != null)
				{
					SmoothJoint[] sjcs = this.SmoothJointObj[ia].GetComponents<SmoothJoint>();

					if (sjcs != null)
					{
						//Confirm the presence of IK
						for (int i = 0; i < sjcs.Length; i++)
						{
							if (sjcs[i].FixedTarget != null && sjcs[i].FixedTarget.Length > 0)
							{
								this.SJList.AddRange(sjcs);
							}
						}
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

			if (this.SJList != null)
			{
				Debug.Log ("It has been registered: " + this.SJList.Count);
			}

			this.BasePosSet();
		}
	}

	private void BasePosSet()
	{
		//Goal initial position storage
		if (this.SJList != null && this.SJList.Count > 0)
		{
			if (this.baseRot == null)
			{
				this.baseRot = new Quaternion[this.SJList.Count][];
			}
			else
			{
				if (this.baseRot.Length != this.SJList.Count)
				{
					System.Array.Resize(ref this.baseRot, this.SJList.Count);

					for (int ri = 0; ri < this.baseRot.Length; ri++)
					{
						this.baseRot[ri] = null;
					}
				}
			}

			if(this.Catch == null)
			{
				this.Catch = new bool[this.SJList.Count][];
			}
			else
			{
				if (this.Catch.Length != this.SJList.Count)
				{
					System.Array.Resize(ref this.Catch, this.SJList.Count);

					for (int ri = 0; ri < this.Catch.Length; ri++)
					{
						this.Catch[ri] = null;
					}
				}
			}

			if (this.basePos == null)
			{
				this.basePos = new Vector3[this.SJList.Count][];
			}
			else
			{
				if (this.basePos.Length != this.SJList.Count)
				{
					System.Array.Resize(ref this.basePos, this.SJList.Count);

					for (int ri = 0; ri < this.basePos.Length; ri++)
					{
						this.basePos[ri] = null;
					}
				}
			}

			if (this.baseScale == null)
			{
				this.baseScale = new Vector3[this.SJList.Count][];
			}
			else
			{
				if (this.baseScale.Length != this.SJList.Count)
				{
					System.Array.Resize(ref this.baseScale, this.SJList.Count);

					for (int ri = 0; ri < this.baseScale.Length; ri++)
					{
						this.baseScale[ri] = null;
					}
				}
			}

			if(this.hitF == null)
			{
				this.hitF = new bool[this.SJList.Count][];
			}
			else
			{
				if (this.hitF.Length != this.SJList.Count)
				{
					System.Array.Resize(ref this.hitF, this.SJList.Count);

					for (int ri = 0; ri < this.hitF.Length; ri++)
					{
						this.hitF[ri] = null;
					}
				}
			}

			//1 frame before the collision coordinate
			if (this.oldPos == null)
			{
				this.oldPos = new Vector3[this.SJList.Count][];
			}
			else
			{
				if (this.oldPos.Length != this.SJList.Count)
				{
					System.Array.Resize(ref this.oldPos, this.SJList.Count);

					for (int ri = 0; ri < this.oldPos.Length; ri++)
					{
						this.oldPos[ri] = null;
					}
				}
			}

			//Acquired confirmation of random coordinates
			if (this.AddRandom == true)
			{
				if (this.rndF == null)
				{
					this.rndF = new bool[this.SJList.Count][];
				}
				else
				{
					if (this.rndF.Length != this.SJList.Count)
					{
						System.Array.Resize(ref this.rndF, this.SJList.Count);

						for (int ri = 0; ri < this.rndF.Length; ri++)
						{
							this.rndF[ri] = null;
						}
					}
				}
			}


			for (int i = 0; i < this.SJList.Count; i++)
			{
				//Confirm the presence of IK
				if (this.SJList[i] != null)
				{
					if (this.SJList[i].FixedJointNo != null && this.SJList[i].FixedTarget != null)
					{
						this.Catch[i] = new bool[this.SJList[i].FixedTarget.Length];
						this.baseRot[i] = new Quaternion[this.SJList[i].FixedTarget.Length];
						this.basePos[i] = new Vector3[this.SJList[i].FixedTarget.Length];
						this.baseScale[i] = new Vector3[this.SJList[i].FixedTarget.Length];
						this.oldPos[i] = new Vector3[this.SJList[i].FixedTarget.Length];

						this.hitF[i] = new bool[this.SJList[i].FixedTarget.Length];

						if (this.AddRandom == true && this.rndF != null && this.rndF.Length > i)
						{
							this.rndF[i] = new bool[this.SJList[i].FixedTarget.Length];
						}

						//Storing an initial position
						for (int ai = 0; ai < this.SJList[i].FixedTarget.Length; ai++)
						{
							this.Catch[i][ai] = false;
							this.baseRot[i][ai] = this.SJList[i].FixedTarget[ai].transform.rotation;
							this.basePos[i][ai] = this.SJList[i].FixedTarget[ai].transform.position;
							this.baseScale[i][ai] = this.SJList[i].FixedTarget[ai].transform.localScale;
							this.oldPos[i][ai] = Vector3.zero;

							this.hitF[i][ai] = false;

							if (this.AddRandom == true && this.rndF != null
								 && this.rndF.Length > i && this.rndF[i] != null && this.rndF[i].Length > ai)
							{
								this.rndF[i][ai] = false;
							}
						}
					}
				}
			}
		}
	}

	public void Update()
	{
		if (this.SmoothCatchOn == true)
		{
			this.confirmation();
		}
	}

	private void confirmation()
	{
		//Confirmation
		if (this.SJList != null && this.SJList.Count > 0 && this.Catch != null)
		{
			for (int i = 0; i < this.SJList.Count; i++)
			{
				SmoothJoint sjc = this.SJList[i];

				//Confirm the presence of IK
				if (sjc != null && sjc.FixedJointNo != null && sjc.FixedTarget != null && this.Catch[i] != null)
				{
					for (int ai = 0; ai < sjc.FixedJointNo.Length; ai++)
					{
						int jn = sjc.FixedJointNo[ai] - 1;

						//Check the running of IK
						bool ikrf = false;
						if (sjc.RangeHitIK != null && sjc.RangeHitIK.Length > ai)
						{
							ikrf = sjc.RangeHitIK[ai];
						}
						if(this.RangeOutOn == true && ikrf == false)//Outside the range of IK
						{
							this.Catch[i][ai] = false;
						}

						//Collision confirmation
						if (sjc.FixedTarget.Length > ai && sjc.HitPosition != null && sjc.HitPosition.Length > jn)
						{
							Vector3 hitpos = sjc.HitPosition[jn];

							if (this.basePos != null && this.basePos.Length > i)
							{
								bool hpf = false;
								if(this.RangeOutOn == true && hitpos != Vector3.zero)
								{
									hpf = true;
								}
								if(this.RangeOutOn == false && hitpos != Vector3.zero
									&& this.Catch[i][ai] == false)
								{
									hpf = true;
								}

								//In collision
								if (hpf == true)
								{
									this.Catch[i][ai] = true;
									sjc.IKRangeOn[ai] = false;//Pause range IK

									if (this.hitF != null && this.hitF.Length > i
										 && this.hitF[i] != null && this.hitF[i].Length > ai)
									{
										this.hitF[i][ai] = true;
									}

									GameObject hitobj = null;
									if (sjc.HitSCC != null &&
										 sjc.HitSCC.SJCCTransform.parent.gameObject != null)
									{
										hitobj = sjc.HitSCC.SJCCTransform.parent.gameObject;
									}

									//random number
									Vector3 rndd0 = Vector3.zero;
									if (this.AddRandom == true && sjc.HitSCC != null && this.rndF != null
									 && this.rndF.Length > i && this.rndF[i] != null && this.rndF[i].Length > ai)
									{
										if (this.rndF[i][ai] == false)
										{
											Vector3 rndd1 = (hitpos +
											Vector3.Scale(new Vector3(Random.value, Random.value, Random.value),
											this.AddRand)
											 - sjc.HitSCC.SJCCTransform.position).normalized *
											(sjc.HitSCC.SJCCRadius * 0.96f);

											rndd0 = sjc.HitSCC.SJCCTransform.position + rndd1;

											this.rndF[i][ai] = true;
										}
									}
									else
									{
										rndd0 = hitpos;
									}

									//Previous coordinates moving
									Vector3 hpd1 = Vector3.zero;

									//Confirmation of collision
									if (this.AvoidanceOn == true &&
										 sjc.HitSCC != null && sjc.CollisionRadius.Length > jn + 1)
									{
										hpd1 = this.HitTest(sjc.HitSCC.SJCCTransform.position,
											sjc.JointObj[jn + 1].transform.position,
												sjc.HitSCC.SJCCRadius, sjc.CollisionRadius[jn + 1]);
									}
									else
									{
										hpd1 = rndd0;
									}

									//Resistance of movement
									if (this.oldPos != null && this.oldPos[i] != null &&
											 this.oldPos[i][ai] != Vector3.zero)
									{
										hpd1 = Vector3.Lerp(hpd1, this.oldPos[i][ai], this.Friction);
									}

									if (this.WorldPos == false && hitobj != null)
									{
										//To child
										sjc.FixedTarget[ai].transform.parent = hitobj.transform;

										//local
										Vector3 hitLpos =
											 hitobj.transform.InverseTransformPoint(hpd1);
										sjc.FixedTarget[ai].transform.localPosition = hitLpos;
									}
									else
									{
										//world
										sjc.FixedTarget[ai].transform.position = hpd1;
									}

									//Storage of coordinates
									if (this.oldPos[i][ai] != hpd1)
									{
										this.oldPos[i][ai] = hpd1;
									}
								}

								//No collision
								if (this.hitF != null && this.hitF.Length > i
								 && this.hitF[i] != null && this.hitF[i].Length > ai)
								{
									int hcn = 0;

									if(sjc != null && sjc.HitSCC != null)
									{
										hcn++;
									}

									//Check out of range (for after collision)
									if(this.hitF[i][ai] == true &&
										sjc != null && sjc.HitSCC != null)
									{
										float cjl = -1f;

										if(sjc.JointObj[jn] != null)
										{
											cjl = (sjc.HitSCC.SJCCTransform.position
											- sjc.JointObj[jn].transform.position).magnitude;
										}

										if(sjc.CollisionRadius.Length > jn)
										{
											float rld = sjc.HitSCC.SJCCRadius + sjc.CollisionRadius[jn];

											if(cjl != -1f && cjl > rld * 1.5f)
											{
												if(this.RangeOutOn == true && ikrf == false)
												{
													hcn = 0;
												}
												if(this.RangeOutOn == false)
												{
													hcn = 0;
												}
											}
										}
									}

									if(hcn == 0)
									{
										if(sjc.IKRangeOn[ai] == false)
										{
											sjc.IKRangeOn[ai] = true;//Start range IK
										}

										this.BaseReturnSet(sjc, i, ai);//It moved to the base position
										this.hitF[i][ai] = false;

										//SJ side external operation (initialization
										sjc.HitSCC = null;

										if(sjc.HitPosition.Length > jn)
										{
											sjc.HitPosition[jn] = Vector3.zero;
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
	private Vector3 HitTest(Vector3 pos0, Vector3 pos1, float Radius0, float Radius1)
	{
		//Return the avoidance coordinate to check the collision
		Vector3 repd = pos1;

		float ld = Vector3.Distance(pos0, pos1);
		float rd = Radius0 + Radius1;

		if (rd > ld)
		{
			//In collision
			float dent = rd - ld;
			Vector3 vd = (pos1 - pos0).normalized;

			Vector3 rvd = vd * dent;//Vector length sinking
			Vector3 hpd = pos0 + (vd * Radius0);//Collision coordinate

			repd = hpd + rvd;
		}

		return repd;
	}

	private void BaseReturnSet(SmoothJoint sjc, int i, int ai)
	{
		//Return to the initial value
		if (this.baseRot != null && this.baseRot.Length > i)
		{
			if (this.baseRot[i].Length > ai)
			{
				//Back to the base rotation
				sjc.FixedTarget[ai].transform.rotation = this.baseRot[i][ai];
			}
		}

		if (this.basePos != null && this.basePos.Length > i)
		{
			if (this.basePos[i].Length > ai)
			{
				if (this.WorldPos == false)
				{
					//Release the child
					sjc.FixedTarget[ai].transform.parent = null;

					sjc.FixedTarget[ai].transform.position = this.basePos[i][ai];
				}
				else
				{
					sjc.FixedTarget[ai].transform.position = this.basePos[i][ai];
				}
			}
		}

		if (this.baseScale != null && this.baseScale.Length > i)
		{
			if (this.baseScale[i].Length > ai)
			{
				//Back to the base scale
				sjc.FixedTarget[ai].transform.localScale = this.baseScale[i][ai];
			}
		}

		if (this.oldPos != null && this.oldPos.Length > i)
		{
			if (this.oldPos[i].Length > ai)
			{
				this.oldPos[i][ai] = Vector3.zero;
			}
		}

		if (this.AddRandom == true && this.rndF != null
				 && this.rndF.Length > i && this.rndF[i] != null
				 && this.rndF[i].Length > ai)
		{
			if (this.rndF[i][ai] == true)
			{
				this.rndF[i][ai] = false;
			}
		}
	}

}
