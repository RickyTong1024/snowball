using System.Collections;
using System.Collections.Generic;

 #if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

//SmoothJoint for the collision object (sphere) ver 1.3.0 20170701
public class SmoothColliderObj : MonoBehaviour {

	[Header("- Initial setup -----------------------")]

	public bool SetSJCollider = false;//Additional SJ Collider

	[Header("---------------------------------------")]
	public int SJColliderNumber = 1;//The number of SJ Collider
	public GameObject ParentObject;//Collider parent object to be installed

	[Header("- Effect setting ----------------------")]

	public List<float> Radius = new List<float>(){0.3f};//radius

	[Space(10)]
	public List<Vector3> Center = new List<Vector3>(){Vector3.zero};//Offset of the center position

	[Space(10)]
	public bool CollisionAction = false;//Or rebound in the collision
	public float AddImpact = 0.5f;//Adding the impact force

	[Header("---------------------------------------")]
	#if UNITY_EDITOR
	public Color colliderColor = new Color(1f, 0.7f, 0f, 1f);//Collision color
	public bool ColliderAllDisplay = false;
	#endif

	[HideInInspector]
	public List<GameObject> CTransform;//For the control of the collider

	[HideInInspector]
	public List<SmoothColliderClass> SCCObjList;//Passing data

	private Vector3 oldPos;

	public int SJCNumber
	{
		set{
			if(this.CTransform != null)
			{
				for(int i = 0; i < this.SJColliderNumber; i++)
				{
					if(this.CTransform[i] != null)
					{
						this.CTransform[i].transform.name = this.name + "_SJCollider_" + i;

						if(this.SCCObjList != null)
						{
							if(this.SCCObjList[i] != null)
							{
								this.SCCObjList[i].SJCCTransform.name = this.CTransform[i].transform.name;
							}
						}
					}
				}
			}
		}
	}

	public List<SmoothColliderClass> SJCObject
	{
		get{
			//SmoothColliderClass data (number, position, radius)
			if(this.SCCObjList != null)
			{

				if(this.SCCObjList.Count != this.CTransform.Count)
				{
					this.SetSJCollider = true;
					this.ReSetDeta();
				}

				if(this.SCCObjList.Count == 0 && this.CTransform.Count == 0)
				{
					this.Init();
				}

				return this.SCCObjList;
			}
			else
			{
				this.Init();

				return null;
			}
		}
	}

	private void Init()
	{
		//Initialization
		if(this.SetSJCollider == true)
		{
			this.SetSJCollider = false;
		}

		//Data creation
		this.Radius = new List<float>();
		this.Radius.Clear();
		this.Center = new List<Vector3>();
		this.Center.Clear();

		this.oldPos = this.transform.position;

		for(int i = 0; i < this.SJColliderNumber; i++)
		{
			this.Radius.Add( 0.3f);
			this.Center.Add(Vector3.zero);
		}

		//Object created for the control of the collider
		if(this.CTransform == null)
		{
			this.CTransform = new List<GameObject>();
			this.CTransform.Clear();
		}

		//Class created
		if(this.SCCObjList == null)
		{
			this.SCCObjList = new List<SmoothColliderClass>();
			this.SCCObjList.Clear();

			this.colliderAddSet(false, 0);
		}
	}

	public void Update()
	{
		if(this.SCCObjList != null && this.CTransform != null)
		{
			for(int ai = 0; ai < this.SCCObjList.Count; ai++)
			{
				if(this.SCCObjList[ai] != null && this.CTransform[ai] != null)
				{
					this.SCCObjList[ai].SJCCTransform = this.CTransform[ai].transform;
				}

				this.CollisionActionSet(ai);
			}
		}

		//Movement vector
		if(this.oldPos != this.transform.position)
		{
			this.oldPos = this.transform.position;
		}
	}

	public void OnValidate()
	{
		//Verify and set the parent-child relationship
		if(this.ParentObject == null)
		{
			this.ParentObject = this.gameObject;
		}
		if(this.ParentObject != this.gameObject)
		{
			this.ReParentSet();
		}

		if (this.SJColliderNumber < 1)
		{
			Debug.Log ("SJColliderNumber Range: 1 ~");
			this.SJColliderNumber = 1;
		}

		this.ReSetDeta();
	}

	private void CollisionActionSet(int i)
	{
		//Collision behavior
		if(this.CollisionAction == true)
		{
			if(this.SCCObjList[i] != null)
			{
				if(this.SCCObjList[i].SJCCHitVec != Vector3.zero)
				{
					//Move
					this.transform.position += this.SCCObjList[i].SJCCHitVec * -1f * this.AddImpact;

					//rotation
					Vector3 cmv = (this.transform.position - this.oldPos).normalized;
					if(cmv != Vector3.zero)
					{
						Quaternion rol =
						Quaternion.FromToRotation(cmv, this.SCCObjList[i].SJCCHitVec.normalized * -1f);

						this.transform.rotation *= rol;
					}

					//Initialization
					this.SCCObjList[i].SJCCHitVec = Vector3.zero;
				}
			}
		}
	}

	private void ReParentSet()
	{
		//Re-setting of the parent-child relationship

		this.ParentObject = this.gameObject;

		if(this.CTransform == null)
		{
			this.Init();
		}
		else
		{
			//collider creation of the control object
			this.CTransform = new List<GameObject>();
			this.CTransform.Clear();

			//Class created
			this.SCCObjList = new List<SmoothColliderClass>();
			this.SCCObjList.Clear();

			this.colliderAddSet(false, 0);
		}
	}


	private void ReSetRadius()
	{
		if(this.SJColliderNumber != this.Radius.Count)
		{
			//Number Delete
			int del = this.Radius.Count - this.SJColliderNumber;

			if(del < 0)
			{
				for(int i = 0; i < Mathf.Abs(del); i++)
				{
					//add to
					this.Radius.Add(0.3f);
				}
			}
			else
			{
				for(int i = 0; i < del; i++)
				{
					int nc = (this.Radius.Count - 1) - i;

					if(nc > 0)
					{
						//Delete
						this.Radius.RemoveAt(nc);
					}
				}
			}
		}
	}

	private void ReSetCenter()
	{
		if(this.SJColliderNumber != this.Center.Count)
		{
			//Number Delete
			int del = this.Center.Count - this.SJColliderNumber;

			if(del < 0)
			{
				for(int i = 0; i < Mathf.Abs(del); i++)
				{
					//add to
					this.Center.Add(Vector3.zero);
				}
			}
			else
			{
				for(int i = 0; i < del; i++)
				{
					int nc = (this.Center.Count - 1) - i;

					if(nc > 0)
					{
						//Delete
						this.Center.RemoveAt(nc);
					}
				}
			}
		}
	}

	private void ReSetDeta()
	{
		//Variable confirmation
		if(this.Radius != null)
		{
			this.ReSetRadius();
		}

		if(this.Center != null)
		{
			this.ReSetCenter();
		}

		if(this.CTransform != null)
		{
			//CTransform the number has decreased
			if(this.SJColliderNumber < this.CTransform.Count)
			{
				#if UNITY_EDITOR

				EditorApplication.delayCall += () => this.colliderResize();

				#endif
			}

			//Adjustment
			if(this.SetSJCollider == false && this.SJColliderNumber == this.CTransform.Count)
			{
				for(int i = 0; i < this.CTransform.Count; i++)
				{
					if(this.CTransform[i] != null && this.Center != null)
					{
						this.CTransform[i].transform.localPosition = this.Center[i];
					}

					if(this.SCCObjList != null && this.CTransform != null && this.Radius != null)
					{
						if(this.SCCObjList[i] != null && this.CTransform[i] != null)
						{
							this.SCCObjList[i].SJCCTransform = this.CTransform[i].transform;
							this.SCCObjList[i].SJCCRadius = this.Radius[i];
						}
					}
				}
			}
		}

		if(this.SCCObjList != null)
		{
			//SCCObjList number has decreased
			if(this.SJColliderNumber < this.SCCObjList.Count)
			{
				#if UNITY_EDITOR

				EditorApplication.delayCall += () => this.colliderResize();

				#endif
			}
		}

		//Update ON
		if(this.SetSJCollider == true)
		{
			this.SetSJCollider = false;

			if(this.CTransform != null && this.CTransform.Count == 0)
			{
				this.Init();
			}

			if(this.CTransform != null)
			{
				//Sure collider has not been deleted to the user
				for(int di = 0; di < this.CTransform.Count; di++)
				{
					//Delete if there is a null
					if(this.CTransform[di] == null)
					{
						this.CTransform.RemoveAt(di);
						this.Radius.RemoveAt(di);
						this.Center.RemoveAt(di);

						if(this.SCCObjList[di] != null)
						{
							#if UNITY_EDITOR

							EditorApplication.delayCall += () => this.classDestroy(di - 1);

							#endif
						}

						//Number change
						if(this.SJColliderNumber > 1)
						{
							this.SJColliderNumber -= 1;
						}
					}
				}

				//Change name
				for(int ni = 0; ni < this.CTransform.Count; ni++)
				{
					if(this.CTransform[ni] != null)
					{
						this.CTransform[ni].transform.name =
						 "Please click a ColliderAutoSetOn (SmoothJoint Component)";

						if(this.SCCObjList != null)
						{
							if(this.SCCObjList.Count > ni && this.SCCObjList[ni] != null)
							{
								this.SCCObjList[ni].SJCCTransform.name = this.CTransform[ni].transform.name;
							}
						}

						Debug.Log ("Please click a ColliderAutoSetOn after setting (SmoothJoint Component)");
					}
				}

				//SCCObjList to make sure that your not been removed to the user
				if(this.SCCObjList != null)
				{
					for(int bi = 0; bi < this.SCCObjList.Count; bi++)
					{
						//Delete if there is a null
						if(this.SCCObjList[bi] == null)
						{
							this.SCCObjList.RemoveAt(bi);
						}
					}
				}

				//All was null
				if(this.CTransform.Count == 1 && this.CTransform[0] == null)
				{
					this.CTransform.Clear();
					this.SCCObjList.Clear();
					this.colliderAddSet(false, 0);

					this.Radius[0] = 0.3f;
					this.Center[0] = Vector3.zero;

					Debug.Log ("SmoothCollider Null All");
				}
			}

			//Add If specify additional
			if(this.CTransform.Count != this.SJColliderNumber && this.SJColliderNumber > this.CTransform.Count)
			{
				this.colliderAddSet(true, this.CTransform.Count);
			}

			if(this.SCCObjList != null &&
				this.SCCObjList.Count != this.SJColliderNumber && this.SJColliderNumber > this.SCCObjList.Count)
			{
				this.classAddSet();
			}
		}
	}

	private void classDestroy(int ci)
	{
		//Class Delete

		if(this.SCCObjList.Count > ci)
		{
			DestroyImmediate(this.SCCObjList[ci]);

			this.SCCObjList.RemoveAt(ci);
		}

		if(this.SCCObjList != null)
		{
			for(int i = 0; i < this.SCCObjList.Count; i++)
			{
				if(this.SCCObjList[i] != null && this.CTransform[i] != null)
				{
					this.SCCObjList[i].SJCCTransform = this.CTransform[i].transform;
				}
			}
		}
	}

	private void colliderResize()
	{
		//collider Delete

		//Number Delete
		int del = this.CTransform.Count - this.SJColliderNumber;

		for(int i = 0; i < del; i++)
		{
			int nc = (this.CTransform.Count - 1) - i;

			if(nc > 0)
			{
				//Delete
				if(this.Radius.Count > nc)
				{
					this.Radius.RemoveAt(nc);
				}
				if(this.Center.Count > nc)
				{
					this.Center.RemoveAt(nc);
				}

				DestroyImmediate(this.CTransform[nc]);

				if(this.CTransform.Count > nc)
				{
					this.CTransform.RemoveAt(nc);
				}

				this.classDestroy(nc);
			}
		}

		Debug.Log ("deletion a SmoothCollider: remains " + this.CTransform.Count);
	}

	private void classAddSet()
	{
		//class created
		if(this.SCCObjList != null)
		{
			int an = this.CTransform.Count - this.SCCObjList.Count;

			if(an > 0)
			{
				for(int i = 0; i < an; i++)
				{
					SmoothColliderClass scc = gameObject.AddComponent<SmoothColliderClass>();

					#if UNITY_EDITOR
					Undo.RegisterCreatedObjectUndo(scc, "Created SJ Class");
					#endif

					if(scc != null)
					{
						scc.SJCCNumber = i;

						if(this.CTransform[i] != null)
						{
							scc.SJCCTransform = this.CTransform[i].transform;
							scc.SJCCTransform.name = this.CTransform[i].transform.name;
						}

						if(this.Radius != null)
						{
							scc.SJCCRadius = this.Radius[i];
						}

						this.SCCObjList.Add(scc);
					}
				}
			}
		}
	}

	private void colliderAddSet(bool ss, int sn)
	{
		//collider created

		if(this.CTransform != null)
		{
			if(ss == false)
			{
				//Initialization
				for(int i = 0; i < this.SJColliderNumber; i++)
				{
					GameObject go = new GameObject("Please click a ColliderAutoSetOn (SmoothJoint Component)");

					#if UNITY_EDITOR
					Undo.RegisterCreatedObjectUndo (go, "Created SJ Collider");
					#endif

					if(go != null)
					{
						this.CTransform.Add(go);

						this.CTransform[i].transform.parent = this.ParentObject.transform;

						this.CTransform[i].transform.localPosition = this.Center[i];
						this.CTransform[i].transform.localRotation = Quaternion.identity;
						this.CTransform[i].transform.localScale = new Vector3(1f, 1f, 1f);

						if(this.SCCObjList != null)
						{
							SmoothColliderClass scc = gameObject.AddComponent<SmoothColliderClass>();

							#if UNITY_EDITOR
							Undo.RegisterCreatedObjectUndo(scc, "Created SJ Class");
							#endif

							if(scc != null)
							{
								scc.SJCCNumber = i;
								scc.SJCCTransform = this.CTransform[i].transform;
								scc.SJCCTransform.name = this.CTransform[i].transform.name;

								if(this.Radius != null)
								{
									scc.SJCCRadius = this.Radius[i];
								}

								this.SCCObjList.Add(scc);
							}
						}
					}
				}
			}
			else
			{
				//add to
				for(int si = sn; si < this.SJColliderNumber; si++)
				{
					GameObject go = new GameObject("Please click a ColliderAutoSetOn (SmoothJoint Component)");

					#if UNITY_EDITOR
					Undo.RegisterCreatedObjectUndo (go, "Created SJ Collider");
					#endif

					if(go != null)
					{
						this.CTransform.Add(go);

						this.CTransform[si].transform.parent = this.ParentObject.transform;

						this.ReSetRadius();
						this.ReSetCenter();

						this.CTransform[si].transform.localPosition = this.Center[si];
						this.CTransform[si].transform.localRotation = Quaternion.identity;
						this.CTransform[si].transform.localScale = new Vector3(1f, 1f, 1f);

						if(this.SCCObjList != null)
						{
							SmoothColliderClass scc = gameObject.AddComponent<SmoothColliderClass>();

							#if UNITY_EDITOR
							Undo.RegisterCreatedObjectUndo(scc, "Created SJ Class");
							#endif

							if(scc != null)
							{
								scc.SJCCNumber = si - sn;
								scc.SJCCTransform = this.CTransform[si].transform;
								scc.SJCCTransform.name = this.CTransform[si].transform.name;

								if(this.Radius != null)
								{
									scc.SJCCRadius = this.Radius[si];
								}

								this.SCCObjList.Add(scc);
							}
						}
					}
				}
			}
		}

		Debug.Log ("Add a SmoothCollider: Total " + this.CTransform.Count);
	}

	private void CenterVectorSet(int i)
	{
		if(this.CTransform[i].transform.localPosition != this.Center[i])
		{
			this.Center[i] = this.CTransform[i].transform.localPosition;
		}
	}

	#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if(this.CTransform != null && this.Radius != null && this.Center != null)
		{
			if(this.ColliderAllDisplay == true)
			{
				for(int ia = 0; ia < this.CTransform.Count; ia++)
				{
					if(this.CTransform[ia] != null
					 && this.CTransform.Count == this.Radius.Count)
					{
						Gizmos.color = this.colliderColor;
						Gizmos.DrawWireSphere(this.CTransform[ia].transform.position, this.Radius[ia]);

						//Position display update in accordance with the user operation
						this.CenterVectorSet(ia);
					}
				}
			}
			else
			{
				if(this.CTransform.Count == this.SJColliderNumber
				&& this.Radius.Count == this.SJColliderNumber
					&& this.Center.Count == this.SJColliderNumber)
				{
					//Select object confirmation
					if(Selection.activeGameObject != null)
					{
						bool rf = false;
						rf = (this.name == Selection.activeGameObject.name) ? true : false;
						if(rf == false)
						{
							for(int i = 0; i < this.CTransform.Count; i++)
							{
								if(this.CTransform[i] != null)
								{
									rf = (this.CTransform[i].name == Selection.activeGameObject.name) ? true : false;
								}
							}
						}

						if(rf == true)
						{
							for(int i = 0; i < this.CTransform.Count; i++)
							{
								if(this.CTransform[i] != null)
								{
									Gizmos.color = this.colliderColor;
									Gizmos.DrawWireSphere(this.CTransform[i].transform.position, this.Radius[i]);

									//Position display update in accordance with the user operation
									this.CenterVectorSet(i);
								}
							}
						}
					}
				}
			}
		}
	}
	#endif

}
