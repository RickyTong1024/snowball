using System.Collections;
using UnityEngine;

//Smooth Collider Class ver 1.3.0 20170701
public class SmoothColliderClass : MonoBehaviour {

	public string SCOName = "name";
	public int ThisNumber = 0;
	public int ThisHitNumber = -1;
	public float ThisRadius = 0.3f;
	public Transform ThisTransform;
	public Vector3 ThisHitNormal;
	public Vector3 ThisCNormal;
	public Vector3 ThisHitVec;

	public int SJCCNumber
	{
		get{return this.ThisNumber;}
		set
		{
			this.ThisNumber = value;
		}
	}

	public Transform SJCCTransform
	{
		get{return this.ThisTransform;}
		set
		{
			this.ThisTransform = value;
			this.SCOName = this.ThisTransform.name;
		}
	}

	public float SJCCRadius
	{
		get{return this.ThisRadius;}
		set
		{
			this.ThisRadius = value;
		}
	}

	public Vector3 SJCCNormal
	{
		get{return this.ThisHitNormal;}
		set
		{
			this.ThisHitNormal = value;
		}
	}

	public int SJCCHitNumber
	{
		get{return this.ThisHitNumber;}
		set
		{
			this.ThisHitNumber = value;
		}
	}

	public Vector3 SJCCCNormal
	{
		get{return this.ThisCNormal;}
		set
		{
			this.ThisCNormal = value;
		}
	}

	public Vector3 SJCCHitVec
	{
		get{return this.ThisHitVec;}
		set
		{
			this.ThisHitVec = value;
		}
	}
}
