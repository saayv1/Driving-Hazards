using UnityEngine;
using System.Collections;
// use gravity and no "is trigger"
public class Cougar1 : MonoBehaviour {

	// Enumerate states of virtual hand interactions
	public enum CougarState {
		Run,
		Dead
	};

	public float speed;
	public Animator animator;

	// Private interaction variables
	CougarState state;
	GameObject cougar;
	GameObject car;
	static Vector3 pos;
	static Quaternion rot;

	// Called at the end of the program initialization
	void Start () {
		// Set initial state to open
		state = CougarState.Run;
		cougar = GameObject.Find ("tiger_idle1");
		car = GameObject.Find ("Classic_car_1955_style1");
		pos = cougar.transform.position;
		rot = cougar.transform.rotation;
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
				cougar.transform.position += cougar.transform.forward * speed * Time.deltaTime;
				if (Mathf.Abs (cougar.transform.position.y - pos.y) > 10 || Mathf.Abs(cougar.transform.position.x - pos.x) > 150 || Mathf.Abs(cougar.transform.position.z - pos.z) > 80) {
					cougar.transform.position = pos;
					cougar.transform.rotation = rot;
				}
			}
		} else if (state == CougarState.Dead) {
			animator.speed = 0;
		}
	}
}