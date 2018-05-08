using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCar : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.name == "Box02" ||col.gameObject.name == "bridgeBottom" || col.gameObject.name == "LakeTerrain")
		{
			GameObject steeringScript = GameObject.Find("scripts/Steering");
			steeringScript.GetComponent<Steering>().isHit = true;
		}
	}
}
