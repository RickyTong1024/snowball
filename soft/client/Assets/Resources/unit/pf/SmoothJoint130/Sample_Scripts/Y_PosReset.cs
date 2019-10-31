using UnityEngine;
using System.Collections;

public class Y_PosReset : MonoBehaviour {

	public float ReSetY = 0.1f;

	private Vector3 basePos;
	private Rigidbody rbody;
	private Quaternion baseRot;

	public void Awake()
	{
		this.basePos = this.transform.position;
		this.baseRot = this.transform.rotation;
		this.rbody = this.GetComponent<Rigidbody>();
	}

	public void FixedUpdate()
	{
		this.FallReset();
	}

	private void FallReset()
	{
		if (this.transform.position.y < this.ReSetY)
		{
			this.transform.position = this.basePos;
			this.transform.rotation = this.baseRot;

			if(this.rbody != null)
			{
				this.rbody.velocity = Vector3.zero;
				this.rbody.angularVelocity = Vector3.zero;
				this.rbody.ResetInertiaTensor();
			}
		}
	}

}
