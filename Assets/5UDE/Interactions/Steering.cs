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

public class Steering : MonoBehaviour
{

    // Enumerate the states of steering
    public enum SteeringState
    {
        NotSteering,
        Accelerate,
        Decelerate,
        Brake,
        ForcedBrake
    };

    // Inspector parameters
    [Tooltip("The tracking device used to determine absolute direction for steering.")]
    public CommonTracker tracker;

    [Tooltip("The controller joystick used to determine relative direction (forward/backward) and speed.")]
    public CommonAxis joystick;

    [Tooltip("A button required to be pressed to activate steering.")]
    public CommonButton button;

    [Tooltip("A trigger button required to be pressed to accelerate.")]
    public CommonButton trigger;

    [Tooltip("A trigger button required to be pressed to brake.")]
    public CommonButton BrakeTrigger;

    [Tooltip("A grip button used to reset the car to the original position.")]
    public CommonButton gripButton;

    [Tooltip("The space that is translated by this interaction. Usually set to the physical tracking space.")]
    public CommonSpace space;

    [Tooltip("The median speed for movement expressed in meters per second.")]
    private float speed = 1.0f;

    private float speedLimit = 10.0f;

    private float acceleration_factor = 0.05f;

    private GameObject car;

	private Quaternion deltaRotation;

    public bool isHit = false;

    public Vector3 originalCarTransform;

    public Quaternion originalCarRotation;

	public Quaternion originalWheelRotation;

	public Vector3 originalWheelTransform;

    public Vector3 originalSpaceTransform;

	public Quaternion originalSpaceRotation;
    // Private interaction variables
    private SteeringState state;

    public AudioClip acc;
    public AudioClip brk;
    protected AudioSource[] m_audio;
    protected AudioSource acc_sound;
    protected AudioSource brk_sound;

    bool played = false;

    void Awake () {

		// Add an audiosource to this gameobject
        //m_audio gameobject.AddComponent
		acc_sound = gameObject.AddComponent<AudioSource> ();
		brk_sound = gameObject.AddComponent<AudioSource> ();
		acc_sound.playOnAwake = false;
		brk_sound.playOnAwake = false;
		acc_sound.clip = acc;
		brk_sound.clip = brk;
	}

public void updateOriginalTransforms()
{
    	GameObject steeringWheel = GameObject.Find ("steeringpivot");

        originalCarTransform = car.transform.position;

        originalCarRotation = car.transform.rotation;

		originalWheelRotation = steeringWheel.transform.rotation;

		originalWheelTransform = steeringWheel.transform.position;

        originalSpaceTransform = space.transform.position;

		originalSpaceRotation = space.transform.rotation;
}

    // Called at the end of the program initialization
    void Start()
    {
        // Set initial steering state to not steering
        state = SteeringState.NotSteering;

        car = GameObject.Find("Classic_car_1955_style1");

		GameObject steeringWheel = GameObject.Find ("steeringpivot");

       updateOriginalTransforms();
    }

    // FixedUpdate is not called every graphical frame but rather every physics frame
    void FixedUpdate()
    {
        if (isHit)
        {
            state = SteeringState.ForcedBrake;
        }
        // If state is not steering
        if (state == SteeringState.NotSteering)
        {

			if (trigger.GetPress()||joystick.GetAxis().x>0)
            {
                state = SteeringState.Accelerate;
            }

			if (joystick.GetAxis ().x > 0) {
				state = SteeringState.Accelerate;
			}
            // Process current not steering state
            else
            {
                // Nothing to do for not steering
            }
        }

        else if (state == SteeringState.Accelerate)
        {
			played = false;
             if (BrakeTrigger.GetPress())
            {
                state = SteeringState.Brake;
            }
			else if (!trigger.GetPress() && joystick.GetAxis().x==0)
            {
                state = SteeringState.Decelerate;
            }

            else
			{
				

				deltaRotation = GameObject.Find ("scripts/VirtualHand").GetComponent<VirtualHand> ().deltaRotation;
				speed = joystick.GetAxis ().x * acceleration_factor *	100;
				space.transform.position += space.transform.forward * Time.deltaTime  * speed ;
				space.transform.Rotate(0,deltaRotation.y*20,0);
                Debug.Log("Accelerate");
                if(acc_sound.isPlaying)
                {

                }
                else{
                    acc_sound.Play();
                    brk_sound.Stop();
                }
               
            }
        }
        else if (state == SteeringState.Decelerate)
        {
			if (BrakeTrigger.GetPress())
            {
                state = SteeringState.NotSteering;
            }
            else if (speed <= 0)
            {
                state = SteeringState.NotSteering;
            }
			else if (trigger.GetPress()||joystick.GetAxis().x>0)
            {
                state = SteeringState.Accelerate;
            }
            else
            {
				deltaRotation = GameObject.Find ("scripts/VirtualHand").GetComponent<VirtualHand> ().deltaRotation;
				space.transform.position += space.transform.forward * speed * Time.deltaTime;
				speed -= speed * acceleration_factor; 
				space.transform.Rotate (0,deltaRotation.y*20,0);
                Debug.Log("Decelerate");
                acc_sound.Stop();

            }
        }

        else if (state == SteeringState.Brake)
        {

			if (!BrakeTrigger.GetPress())
            {
                state = SteeringState.Decelerate;
            }
            else if (speed <= 0)
            {
                state = SteeringState.NotSteering;
            }
            else
            {
                //space.transform.position += space.transform.forward * speed * Time.deltaTime * acceleration_factor;
				space.transform.position += space.transform.forward * speed * Time.deltaTime ;
                speed -= speed * acceleration_factor;
                Debug.Log("Brake");
                if(brk_sound.isPlaying || played)
                {

                }
                else{
                    brk_sound.Play();
                    acc_sound.Stop();
                    played = true;
                }
            }
        }
        else if (state == SteeringState.ForcedBrake)
        {
            acc_sound.Stop();
            brk_sound.Stop();
            Debug.Log("force brake");
            if (gripButton.GetPress())
            {
				space.transform.rotation = originalSpaceRotation;
                space.transform.position = originalSpaceTransform;
				car.transform.rotation = originalCarRotation;
				car.transform.position = originalCarTransform;
                GameObject broken_window =  GameObject.Find("front_window_broken");
                GameObject good_window = GameObject.Find("front_window");

				GameObject steeringWheel = GameObject.Find ("steeringpivot");
				steeringWheel.transform.rotation = originalWheelRotation ;
				steeringWheel.transform.position = originalWheelTransform;
				speed = 0;

				deltaRotation.Set(0, 0, 0, 0);

                broken_window.GetComponent<MeshRenderer>().enabled = false;
                good_window.GetComponent<MeshRenderer>().enabled = true;
                isHit = false;
                state = SteeringState.NotSteering;
            }
            else if (speed <= 1)
            {
                state = SteeringState.NotSteering;
            }
            else
            {
                space.transform.position += space.transform.forward * speed * Time.deltaTime * -1 * acceleration_factor;
                speed -= speed * acceleration_factor;
            }
        }
    }
}