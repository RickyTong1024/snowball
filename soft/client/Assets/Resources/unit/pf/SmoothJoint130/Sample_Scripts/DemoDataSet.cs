using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemoDataSet : MonoBehaviour {

	public GameObject[] DataComponent;
	public string EffectName = "";

	private List<SmoothJoint> sjo;

	public List<SmoothJoint> sjClass
	{
		get{return this.sjo;}
	}

	public void Awake()
	{
		if(this.DataComponent != null)
		{
			this.sjo = new List<SmoothJoint>();
			this.sjo.Clear();

			for (int i = 0; i < this.DataComponent.Length; i++)
			{
				if(this.DataComponent[i] != null)
				{
					SmoothJoint[] sjc = this.DataComponent[i].GetComponents<SmoothJoint>();

					if(sjc != null)
					{
						this.sjo.AddRange(sjc);
					}
				}
			}
		}
	}

	
}
