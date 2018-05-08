using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {


GameObject carSteering;
GameObject controller;



float angle = 0.0f;
	// Use this for initialization
	void Start () {
		
		carSteering = this.gameObject;
		controller = GameObject.Find("Vive Controller (right)");



		angle = Quaternion.Angle(controller.transform.rotation, carSteering.transform.rotation);
		
	}
	
	// Update is called once per frame
	void Update () {

		angle = Quaternion.Angle(controller.transform.rotation, carSteering.transform.rotation);
		Debug.Log(angle);
	}
}
