using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*----------------------------------------------------------------------------/
 * Is a watermelon a cucumber? Another dill pickle.
 * We've put thought and effort into this. Please mind the human hours when you
 * blatandly copy the code.
 * 
 * With love, Nick Elferink, Sven Schuiten, Teun Hoiting.
 * <3
 ----------------------------------------------------------------------------*/

[RequireComponent(typeof(actorMotor))]
public class playerController : MonoBehaviour {

	#region Initialization

	actorMotor _motor;

	void Start(){
		_motor = GetComponent<actorMotor> ();
	}

	#endregion

	#region Manual Control Protocol

	//The MOVETHRESHOLD is the value axis must have before any effect takes place. Good for porting to consoles.
	public float MOVETHRESHOLD = 0.0f;

	void Update () {

		//Retrieve the InputAxis
		float xInput = Input.GetAxis ("Horizontal");
		float yInput = Input.GetAxis ("Vertical");

		//Declare the outcome variable holders first


		//Check if the input is stronger than the threshold, then submit the movement
		if (xInput >= MOVETHRESHOLD || xInput <= -MOVETHRESHOLD ) {
			_motor.XMovement = xInput;
		} else
			_motor.XMovement = 0;

		if (yInput >= MOVETHRESHOLD || yInput <= -MOVETHRESHOLD ) {
			_motor.YMovement = yInput;
		} else
			_motor.YMovement = 0;

		_motor.ConfigureMotorLookDir (Camera.main.ScreenToWorldPoint (Input.mousePosition));

	}
		
	#endregion
}
