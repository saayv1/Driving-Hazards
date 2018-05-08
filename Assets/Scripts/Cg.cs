using UnityEngine;
using System.Collections;
// use gravity and no "is trigger"
public class Cg : MonoBehaviour {

	// Enumerate states of virtual hand interactions
	public enum CougarState {
		Run,
		Dead
	};

	public float speed;
	public Animator animator;
	public AudioClip voice;
	protected AudioSource sound;

	// Private interaction variables
	CougarState state;
	GameObject car;
	Vector3 pos;
	Quaternion rot;

	void Awake () {
		// Add an audiosource to this gameobject
		sound = gameObject.AddComponent<AudioSource> ();
		sound.playOnAwake = false;
		sound.clip = voice;
	}

	// Called at the end of the program initialization
	void Start () {
		// Set initial state to open
		state = CougarState.Run;
		car = GameObject.Find ("Classic_car_1955_style1");
		pos = transform.position;
		rot = transform.rotation;
	}


	void OnCollisionEnter (Collision collision) {
		// Cougar is dead
		GameObject obj = collision.gameObject;
		if (obj != car)
			return;
		state = CougarState.Dead;
		animator.speed = 0;
	}


	// FixedUpdate is not called every graphical frame but rather every physics frame
	void FixedUpdate ()
	{
        // If state is Run
        if (state == CougarState.Run) {
			if (animator.speed != 0) {
				if (Mathf.Abs (transform.rotation.z - rot.z) > 0.4 || Mathf.Abs (transform.position.y - pos.y) > 10 || Mathf.Abs(transform.position.x - pos.x) > 150 || Mathf.Abs(transform.position.z - pos.z) > 150) {
					transform.rotation = rot;
					if (Vector3.Distance(car.transform.position, transform.position) > 75f) {
						transform.position = pos;
					}
				}
                if (Mathf.Abs(transform.rotation.x - rot.x) > 0.1)
                {
                    transform.position += transform.forward * 4f * Time.deltaTime;
                }
                else
                {
                    transform.position += transform.forward * speed * Time.deltaTime;
                }
				sound.Play ();
			}
		} else if (state == CougarState.Dead) {
			animator.speed = 0;
		}
	}
}