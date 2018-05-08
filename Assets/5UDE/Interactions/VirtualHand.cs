/*
Copyright ©2017. The University of Texas at Dallas. All Rights Reserved.
Permission to use, copy, modify, and distribute this software and its documentation for 
educational, research, and not-for-profit purposes, without fee and without a signed 
licensing agreement, is hereby granted, provided that the above copyright notice, this 
paragraph and the following two paragraphs appear in all copies, modifications, and 
distributions.
Contact The Office of Technology Commercialization, The University of Texas at Dallas, 
800 W. Campbell Road (AD15), Richardson, Texas 75080-3021, (972) 883-4558, 
otc@utdallas.edu, https://research.utdallas.edu/otc for commercial licensing opportunities.
IN NO EVENT SHALL THE UNIVERSITY OF TEXAS AT DALLAS BE LIABLE TO ANY PARTY FOR DIRECT, 
INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES, INCLUDING LOST PROFITS, ARISING 
OUT OF THE USE OF THIS SOFTWARE AND ITS DOCUMENTATION, EVEN IF THE UNIVERSITY OF TEXAS AT 
DALLAS HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
THE UNIVERSITY OF TEXAS AT DALLAS SPECIFICALLY DISCLAIMS ANY WARRANTIES, INCLUDING, BUT 
NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
PURPOSE. THE SOFTWARE AND ACCOMPANYING DOCUMENTATION, IF ANY, PROVIDED HEREUNDER IS 
PROVIDED "AS IS". THE UNIVERSITY OF TEXAS AT DALLAS HAS NO OBLIGATION TO PROVIDE 
MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
*/
using UnityEngine;
using System.Collections;
public class VirtualHand : MonoBehaviour {

	// Enumerate states of virtual hand interactions
	public enum VirtualHandState {
		Open,
		Touching,
		Grabbing
	};
	// Inspector parameters
	[Tooltip("The tracking device used for tracking the real hand.")]
	public CommonTracker tracker;
	[Tooltip("The interactive used to represent the virtual hand.")]
	public Affect hand;
	[Tooltip("The button required to be pressed to grab objects.")]
	public CommonButton button;
	[Tooltip("The tracking device used for tracking the real hand.")]
	public CommonTracker right_tracker;
	[Tooltip("The interactive used to represent the virtual hand.")]
	public Affect right_hand;
	[Tooltip("The button required to be pressed to grab objects.")]
	public CommonButton right_button;

	[Tooltip("The speed amplifier for thrown objects. One unit is physically realistic.")]
	public float speed = 1.0f;
	// Private interaction variables
	VirtualHandState state;
	FixedJoint grasp;
	Collider target;

	public GameObject steeringWheel;
	public Vector3 contact;
	Vector3 initialVector;
	Vector3 currentVector;
	public Vector3 steeringWheelNormal;
	Quaternion origSteeringWheelRotation;
	public Quaternion deltaRotation;

	private bool isHit;

	private bool set;

	// Called at the end of the program initialization
	void Start () {
		// Set initial state to open
		state = VirtualHandState.Open;
		// Ensure hand interactive is properly configured
		hand.type = AffectType.Virtual;
		GameObject car = GameObject.Find("Classic_car_1955_style1");
		contact = Vector3.zero;
		steeringWheelNormal = steeringWheel.transform.forward;
		deltaRotation = new Quaternion();
		set = false;
	}
	// FixedUpdate is not called every graphical frame but rather every physics frame
	void FixedUpdate ()
	{
		// If state is open
		if (state == VirtualHandState.Open) {
			isHit = GameObject.Find ("scripts/Steering").GetComponent<Steering> ().isHit;

			// If the hand is touching something
			if (hand.triggerOngoing) {
				// Change state to touching
				state = VirtualHandState.Touching;
			}
			// Process current open state
			else {
				// Nothing to do for open
			}
		}
		// If state is touching
		else if (state == VirtualHandState.Touching) {
			// If the hand is not touching something
			if (!hand.triggerOngoing || !right_hand.triggerOngoing) {
				// Change state to open
				state = VirtualHandState.Open;
			}
			// If the hand is touching something and the button is pressed
			else if (hand.triggerOngoing && button.GetPress () && right_hand.triggerOngoing && right_button.GetPress()) {
				// Fetch touched target
				target = hand.ongoingTriggers [0];
				//initialVector = hand.gameObject.transform.position- steeringWheel.transform.position;

				initialVector = (hand.gameObject.transform.position - steeringWheel.transform.position) + (right_hand.gameObject.transform.position - steeringWheel.transform.position);

				initialVector = Vector3.ProjectOnPlane(initialVector, steeringWheelNormal);
				origSteeringWheelRotation = steeringWheel.transform.rotation;

				// Change state to LeftTurn
				state = VirtualHandState.Grabbing;
			}
			// Process current touching state
			else {
				//initialPosition = hand.gameObject.transform.position;

				// Nothing to do for touching
			}
		}
		// If state is LeftTurn
		else if (state == VirtualHandState.Grabbing) {


			// If button has been released and grasp still exists
			if (!button.GetPress () || !right_button.GetPress()) {
				state = VirtualHandState.Open;
			}

			if (isHit) {
				//deltaRotation.Equals (0);
				state = VirtualHandState.Open;
				initialVector.Set (0, 0, 0);
				currentVector.Set (0, 0, 0);
				deltaRotation.Set (0, 0, 0, 0);

			}
			// Process current LeftTurn state
			else {
				
				steeringWheelNormal = steeringWheel.transform.forward;

				origSteeringWheelRotation = steeringWheel.transform.rotation;

				currentVector = (hand.gameObject.transform.position - steeringWheel.transform.position) + (right_hand.gameObject.transform.position - steeringWheel.transform.position);

				initialVector = Vector3.ProjectOnPlane (initialVector, steeringWheelNormal);

				currentVector = Vector3.ProjectOnPlane(currentVector, steeringWheelNormal);

				deltaRotation.SetFromToRotation (initialVector, currentVector);

				GameObject car = GameObject.Find ("Classic_car_1955_style1");

				steeringWheel.transform.rotation = deltaRotation * origSteeringWheelRotation;

				isHit = GameObject.Find ("scripts/Steering").GetComponent<Steering> ().isHit;

				initialVector = currentVector;

			}
		}
	}
}