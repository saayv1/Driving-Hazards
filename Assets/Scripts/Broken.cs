using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broken : MonoBehaviour {
	private GameObject good_window;
	private GameObject broken_window;
	private GameObject car;
    private GameObject bumper;
	public AudioClip window;
	public AudioClip cougar;
	protected AudioSource window_sound;
	protected AudioSource cougar_sound;

	void Awake () {

		// Add an audiosource to this gameobject
		window_sound = gameObject.AddComponent<AudioSource> ();
		cougar_sound = gameObject.AddComponent<AudioSource> ();
		window_sound.playOnAwake = false;
		cougar_sound.playOnAwake = false;
		window_sound.clip = window;
		cougar_sound.clip = cougar;
	}
	// Use this for initialization
	void Start () {
		good_window = GameObject.Find ("front_window");
		broken_window = GameObject.Find ("front_window_broken");
		car = GameObject.Find ("Classic_car_1955_style1");
        bumper = GameObject.Find("Classic_car_1955_style1/bumper");
    }
	void OnCollisionEnter (Collision collision) {
		// Cougar is dead
		GameObject obj = collision.gameObject;
        if (obj == car || obj == bumper)
        {
			if (good_window.GetComponent<MeshRenderer> ().enabled) {
				window_sound.Play ();
			}
            good_window.GetComponent<MeshRenderer>().enabled = false;
            broken_window.GetComponent<MeshRenderer>().enabled = true;
			cougar_sound.Play ();
        }
	}
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == bumper)
        {
			if (good_window.GetComponent<MeshRenderer> ().enabled) {
				window_sound.Play ();
			}
            good_window.GetComponent<MeshRenderer>().enabled = false;
            broken_window.GetComponent<MeshRenderer>().enabled = true;
			cougar_sound.Play ();
        }
    }
}
