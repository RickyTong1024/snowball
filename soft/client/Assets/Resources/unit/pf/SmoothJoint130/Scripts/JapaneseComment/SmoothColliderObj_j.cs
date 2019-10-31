using System.Collections;
using System.Collections.Generic;

 #if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

//SmoothJoint当たりコリジョン（球 ver 1.3.0 20170701 (使用する場合はpublic class SmoothColliderObj_jの_jを削除)
public class SmoothColliderObj_j : MonoBehaviour {

	[Header("- Initial setup -----------------------")]

	public bool SetSJCollider = false;//SJ Collider追加

	[Header("---------------------------------------")]
	public int SJColliderNumber = 1;//SJ Colliderの個数
	public GameObject ParentObject;//Collider設置親オブジェクト

	[Header("- Effect setting ----------------------")]

	public List<float> Radius = new List<float>(){0.3f};//半径

	[Space(10)]
	public List<Vector3> Center = new List<Vector3>(){Vector3.zero};//中心オフセット

	[Space(10)]
	public bool CollisionAction = false;//衝突で反発するか
	public float AddImpact = 0.5f;//衝撃力加算

	[Header("---------------------------------------")]
	#if UNITY_EDITOR
	public Color colliderColor = new Color(1f, 0.7f, 0f, 1f);//コリジョン色
	public bool ColliderAllDisplay = false;
	#endif

	[HideInInspector]
	public List<GameObject> CTransform;//collider制御用

	[HideInInspector]
	public List<SmoothColliderClass> SCCObjList;//受け渡し用データ

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
			//SmoothColliderClassデータ（番号、位置、半径)
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
		//初期化
		if(this.SetSJCollider == true)
		{
			this.SetSJCollider = false;
		}

		//データ作成
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

		//collider制御Object作成
		if(this.CTransform == null)
		{
			this.CTransform = new List<GameObject>();
			this.CTransform.Clear();
		}

		//Class作成
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

		//移動ベクトル
		if(this.oldPos != this.transform.position)
		{
			this.oldPos = this.transform.position;
		}
	}

	public void OnValidate()
	{
		//Inspectorの値を変更すると呼ばれる

		//親子関係確認設定
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
		//衝突行動
		if(this.CollisionAction == true)
		{
			if(this.SCCObjList[i] != null)
			{
				if(this.SCCObjList[i].SJCCHitVec != Vector3.zero)
				{
					//移動
					this.transform.position += this.SCCObjList[i].SJCCHitVec * -1f * this.AddImpact;

					//回転
					Vector3 cmv = (this.transform.position - this.oldPos).normalized;
					if(cmv != Vector3.zero)
					{
						Quaternion rol =
						Quaternion.FromToRotation(cmv, this.SCCObjList[i].SJCCHitVec.normalized * -1f);

						this.transform.rotation *= rol;
					}

					//初期化
					this.SCCObjList[i].SJCCHitVec = Vector3.zero;
				}
			}
		}
	}

	private void ReParentSet()
	{
		//親子関係再設定

		this.ParentObject = this.gameObject;

		if(this.CTransform == null)
		{
			this.Init();
		}
		else
		{
			//collider制御Object作成
			this.CTransform = new List<GameObject>();
			this.CTransform.Clear();

			//Class作成
			this.SCCObjList = new List<SmoothColliderClass>();
			this.SCCObjList.Clear();

			this.colliderAddSet(false, 0);
		}
	}


	private void ReSetRadius()
	{
		if(this.SJColliderNumber != this.Radius.Count)
		{
			//削除数
			int del = this.Radius.Count - this.SJColliderNumber;

			if(del < 0)
			{
				for(int i = 0; i < Mathf.Abs(del); i++)
				{
					//追加
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
						//削除
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
			//削除数
			int del = this.Center.Count - this.SJColliderNumber;

			if(del < 0)
			{
				for(int i = 0; i < Mathf.Abs(del); i++)
				{
					//追加
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
						//削除
						this.Center.RemoveAt(nc);
					}
				}
			}
		}
	}

	private void ReSetDeta()
	{
		//変数確認
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
			//CTransform数が減ったら
			if(this.SJColliderNumber < this.CTransform.Count)
			{
				#if UNITY_EDITOR

				//タイミングずらして呼ぶ（警告回避
				EditorApplication.delayCall += () => this.colliderResize();

				#endif
			}

			//調整
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
			//SCCObjList数が減ったら
			if(this.SJColliderNumber < this.SCCObjList.Count)
			{
				#if UNITY_EDITOR

				//タイミングずらして呼ぶ（警告回避
				EditorApplication.delayCall += () => this.colliderResize();

				#endif
			}
		}


		//更新スイッチＯＮ
		if(this.SetSJCollider == true)
		{
			this.SetSJCollider = false;

			if(this.CTransform != null && this.CTransform.Count == 0)
			{
				this.Init();
			}

			if(this.CTransform != null)
			{
				//colliderがユーザーに削除されて無いか確認
				for(int di = 0; di < this.CTransform.Count; di++)
				{
					//nullがあれば削除
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

						//数変更
						if(this.SJColliderNumber > 1)
						{
							this.SJColliderNumber -= 1;
						}
					}
				}

				//名前変更
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

				//SCCObjListがユーザーに削除されて無いか確認
				if(this.SCCObjList != null)
				{
					for(int bi = 0; bi < this.SCCObjList.Count; bi++)
					{
						//nullがあれば削除
						if(this.SCCObjList[bi] == null)
						{
							this.SCCObjList.RemoveAt(bi);
						}
					}
				}

				//全てnullだったら
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

			//追加指定なら追加
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
		//Class削除

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
		//collider削除

		//削除数
		int del = this.CTransform.Count - this.SJColliderNumber;

		for(int i = 0; i < del; i++)
		{
			int nc = (this.CTransform.Count - 1) - i;

			if(nc > 0)
			{
				//削除
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
		//class作成
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
		//collider作成

		if(this.CTransform != null)
		{
			if(ss == false)
			{
				//初期化
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
				//追加
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

						//ユーザー操作に合わせて位置表示更新
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
					//選択object確認
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

									//ユーザー操作に合わせて位置表示更新
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
