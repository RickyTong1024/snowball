using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IKSampleEventSet : MonoBehaviour {

	public GameObject MLPaObj;
	public GameObject MLPbObj;

	public GameObject OpaObj;
	public GameObject OpbObj;

	public Slider sliderA;
	public Slider sliderB;

	public GameObject OpcObj;

	private MoveLPosSet mlpa, mlpb;
	private bool mplaf, mplbf;
	private Quaternion oldqa, oldqb;
	private Slider slia, slib;
	private Rigidbody rig;

	public void Awake()
	{
		this.Init();
	}

	public void Update ()
	{
		this.OpRotSet();
	}

	private void Init()
	{
		this.mplaf = false;
		this.mplbf = false;

		if(this.MLPaObj != null)
		{
			this.mlpa = this.MLPaObj.GetComponent<MoveLPosSet>();
		}
		if(this.MLPbObj != null)
		{
			this.mlpb = this.MLPbObj.GetComponent<MoveLPosSet>();
		}

		if(this.OpaObj != null)
		{
			this.oldqa = this.OpaObj.transform.rotation;
		}
		if(this.OpbObj != null)
		{
			this.oldqb = this.OpbObj.transform.rotation;
		}

		if(this.sliderA != null)
		{
			this.slia = this.sliderA.GetComponent<Slider>();
		}
		if(this.sliderB != null)
		{
			this.slib = this.sliderB.GetComponent<Slider>();
		}

		if(this.OpcObj != null)
		{
			this.rig = this.OpcObj.GetComponent<Rigidbody>();
		}
	}

	private void OpRotSet()
	{
		if(this.OpaObj != null && this.slia != null)
		{
			float vafa = this.slia.value - 0.5f;

			Vector3 vaa = new Vector3(0f, 40f, 0f) * vafa;

			Quaternion qda = Quaternion.Euler(vaa);

			this.OpaObj.transform.rotation = this.oldqa * qda;
		}

		if(this.OpbObj != null && this.slib != null)
		{
			float vafb = this.slib.value - 0.5f;

			Vector3 vab = new Vector3(0f, 40f, 0f) * vafb;

			Quaternion qdb = Quaternion.Euler(vab);

			this.OpbObj.transform.rotation = this.oldqb * qdb;
		}
	}

	public void StopRigidbody()
	{
		if(this.rig != null)
		{
			switch (this.rig.useGravity)
			{
			case false:

				this.rig.useGravity = true;
				break;

			case true:

				this.rig.useGravity = false;
				break;
			}
		}
	}

	public void StartMLPSet(bool bf)
	{
		if(bf == false)
		{
			if(this.mlpa != null)
			{
				switch (this.mplaf)
				{
				case false:

					this.mplaf = true;
					this.mlpa.StartMPS = true;
					break;

				case true:

					this.mplaf = false;
					this.mlpa.StartMPS = false;
					break;
				}
			}
		}
		else
		{
			if(this.mlpb != null)
			{
				switch (this.mplbf)
				{
				case false:

					this.mplbf = true;
					this.mlpb.StartMPS = true;
					break;

				case true:

					this.mplbf = false;
					this.mlpb.StartMPS = false;
					break;
				}
			}
		}
	}
}
