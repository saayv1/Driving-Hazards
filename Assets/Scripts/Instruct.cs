using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruct : MonoBehaviour {
	public AudioClip voice;
	protected AudioSource sound;
	GameObject car;
	bool played;
	private CommonSpace s;
	GameObject steeringWheel;
	void Awake () {
		// Add an audiosource to this gameobject
		sound = gameObject.AddComponent<AudioSource> ();
		sound.playOnAwake = false;
		sound.clip = voice;
		played = false;
		
	}
	// Use this for initialization
	void Start () {
		car = GameObject.Find ("Classic_car_1955_style1");
		s = GameObject.Find("scripts/Steering").GetComponent<Steering>().space;
		steeringWheel = GameObject.Find("steeringpivot");
		Update ();
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log(Vector3.Distance(car.transform.position, transform.position).ToString());
		if (!played && Vector3.Distance(car.transform.position, transform.position) < 20f) {
			sound.Play ();
			played = true;
			GameObject.Find("scripts/Steering").GetComponent<Steering>().updateOriginalTransforms();
			

		}
	}
}