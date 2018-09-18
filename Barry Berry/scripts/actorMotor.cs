using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*----------------------------------------------------------------------------/
 *E = mc2 
 * We've put thought and effort into this. Please mind the human hours when you
 * blatandly copy the code.
 * 
 * With love, Nick Elferink, Sven Schuiten, Teun Hoiting.
 * <3
 ----------------------------------------------------------------------------*/
public class actorMotor : MonoBehaviour {

    #region Variable Declaration

    // <summary>
	// How fast the actor should move without effectors. Dont use except in calculateAffectedSpeed().
    // </summary>
    public float baseMaxSpeed = 2;

    // <summary>
    // How fast the actor moves with the entire effect stack.
    // </summary>
    float affectedMaxSpeed = 100;

	// <summary>
	// How long it takes for the actor to reach max speed
	// </summary>
	public float timeToSpeed = 1;

	// <summary>
	// How long the player takes to stop.
	// </summary>
	public float stopDistance = 1;

	// <summary>
	// The transform in the world at which the character should look & aim.
	// </summary>
	public Vector3 TargetWorldPos;

	// <summary>
	// What absolute directions the player can look.
	// Note: This does not mean that the character can only look at 8 different angles! This is just for animation and representatation.
	// </summary>
	public enum LookDir {
		North,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest
	}
	public LookDir lookDir { get; private set; }

    // <summary>
    // The states the motor can be in
    // </summary>
    public enum MotorState {
        DebugFrozen,
        Idle,
        Walking
    }

    public MotorState motorState { get; private set; }
	public float XMovement{ get; set;}
	public float YMovement{ get; set;}
	private Vector2 normalizedV2Velocity;
	private Rigidbody2D _rigidbody2d;
	private float lookAngle;

	private void GetSpeedAndMaxSpeed(out float speed, out float maxSpeed, bool isX){
		if (isX) {
			speed = _rigidbody2d.velocity.x;
			maxSpeed = affectedMaxSpeed;
		} else {
			speed = _rigidbody2d.velocity.y;
			maxSpeed = affectedMaxSpeed;
		}

	}

	private float Accelerate(float speed, float acceleration, float limit)
	{
		// acceleration can be negative or positive to note acceleration in that direction.
		speed += acceleration * Time.deltaTime;

		if (acceleration > 0)
		{
			if (speed > limit)
			{
				speed = limit;
			}
		}
		else
		{
			if (speed < limit)
			{
				speed = limit;
			}
		}

		return speed;
	}

	private float Decelerate(float speed, float deceleration, float limit)
	{
		// deceleration is always positive but assumed to take the velocity backwards.
		if (speed < 0)
		{
			speed += deceleration * Time.deltaTime;

			if (speed > limit)
			{
				speed = limit;
			}
		}
		else if (speed > 0)
		{
			speed -= deceleration * Time.deltaTime;

			if (speed < limit)
			{
				speed = limit;
			}
		}

		return speed;
	}

    #endregion

	#region Motor Power

	private void Awake() {
		_rigidbody2d = GetComponent<Rigidbody2D>();
	}

	private void OnEnable(){
		if (_rigidbody2d != null) {
			_rigidbody2d.isKinematic = true;
		}
	}

	void Update() {
		ApplyMovement();
		calculateAffectedSpeed();
	}

	void FixedUpdate(){
		_rigidbody2d.MovePosition (_rigidbody2d.position + normalizedV2Velocity * Time.deltaTime);
	}

	#endregion

    #region Motor Cogs

    public void calculateAffectedSpeed() {
		//TODO: Write algorithm to calculate the affectedspeed, right now its just base=affected
		affectedMaxSpeed = baseMaxSpeed;
    }

	public void ApplyMovement() {
		
		float xSpeed = 0;
		float xMaxSpeed;
		float ySpeed = 0;
		float yMaxSpeed;



		if (Mathf.Abs (XMovement) > 0 || Mathf.Abs (YMovement) > 0) {

			GetSpeedAndMaxSpeed(out xSpeed, out xMaxSpeed, true);
			GetSpeedAndMaxSpeed(out ySpeed, out yMaxSpeed, false);

			if (timeToSpeed > 0) {
				//If we are moving faster than our xMovement * affectedMaxSpeed the decelerate rather than accelerate.
				if (xSpeed > 0 &&
				   XMovement > 0 &&
				   xSpeed > XMovement * xMaxSpeed ||
				   xSpeed < 0 &&
				   XMovement < 0 &&
				   xSpeed < XMovement * xMaxSpeed) {

					float deceleration = (xMaxSpeed * xMaxSpeed) / (2 * stopDistance);
					xSpeed = Decelerate (xSpeed,
						deceleration,
						XMovement * xMaxSpeed);
				
				} else {
				
					float acceleration = XMovement * (xMaxSpeed * timeToSpeed);
					xSpeed = Accelerate (xSpeed,
						acceleration,
						XMovement * xMaxSpeed);
				
				}

				//does the same equation for yMovement
				if (ySpeed > 0 &&
				   YMovement > 0 &&
				   ySpeed > YMovement * affectedMaxSpeed ||
				   ySpeed < 0 &&
				   YMovement < 0 &&
				   ySpeed < YMovement * affectedMaxSpeed) {

					float deceleration = (affectedMaxSpeed * affectedMaxSpeed) / (2 * stopDistance);
					ySpeed = Decelerate (ySpeed,
						deceleration,
						YMovement * affectedMaxSpeed);
				
				} else {
				
					float acceleration = YMovement * (affectedMaxSpeed * timeToSpeed);
					ySpeed = Accelerate (ySpeed,
						acceleration,
						YMovement * affectedMaxSpeed);
				
				}
			}
		}
		Vector2 moveInput = new Vector2 (xSpeed, ySpeed);
		normalizedV2Velocity = moveInput;
	}

	public void ConfigureMotorLookDir(Vector3 targetPos) {
		//Set the TargetWorldTransform so the script knows where it is looking.
		TargetWorldPos = targetPos;

		//Calculates the angle at which to look.
		Vector3 dir = targetPos - this.transform.position;
		lookAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg + 180; //the +180 is to combat the fact that angles are never higher than 180.

		//Sets the Enum Lookdir to it's appropiate direction.
		if (lookAngle < 22.5 || lookAngle > 337.5 && lookAngle < 360) {
			lookDir = LookDir.West;
		} else if (lookAngle < 337.5 && lookAngle > 292.5) {
			lookDir = LookDir.NorthWest;
		} else if (lookAngle < 292.5 && lookAngle > 247.5) {
			lookDir = LookDir.North;
		} else if (lookAngle < 247.5 && lookAngle > 202.5) {
			lookDir = LookDir.NorthEast;
		} else if (lookAngle < 202.5 && lookAngle > 157.5) {
			lookDir = LookDir.East;
		} else if (lookAngle < 157.5 && lookAngle > 112.5) {
			lookDir = LookDir.SouthEast;
		} else if (lookAngle < 112.5 && lookAngle > 67.5) {
			lookDir = LookDir.South;
		} else if (lookAngle < 67.5 && lookAngle > 22.5) {
			lookDir = LookDir.SouthWest;
		} else {
			//This is unpossible. Log an error!
			Debug.LogError("Somehow you spinned outside the 360 degrees given. Welcome in this new dimension!");
		}
	}

    #endregion

    #region List of tools for debugging
    //These are to show neccesary information visually!
    void OnDrawGizmosSelected(){
		Bounds box = GetComponent<Collider2D> ().bounds;
		Vector2 min;
		Vector2 max;

		//Check box
		min = box.min;
		max = box.max;

		//Show the distance the motor requires to stop
		Vector2 stopfrom = new Vector2 (box.center.x, box.center.y);
		Gizmos.color = Color.green;
		Gizmos.DrawLine (box.center, stopfrom + Vector2.right * stopDistance);

		//Shows what way the player is looking exactly in blue and what rough direction in red.
		Gizmos.color = Color.blue;
		//Vector3 endLineLocation = new Vector3(Mathf.Clamp(TargetWorldPos.x, box.center.x - 0.5f, box.center.x +0.5f), Mathf.Clamp(TargetWorldPos.y, box.center.y - 0.5f, box.center.y +0.5f), 0);
		Gizmos.DrawLine (box.center, TargetWorldPos);

		Gizmos.color = Color.red;
		switch (lookDir) {
		case LookDir.North:
			Gizmos.DrawLine (box.center + new Vector3 (0, 0.5f), box.center + new Vector3 (0, 0.75f));
			break;
		case LookDir.NorthEast:
			Gizmos.DrawLine (box.center + new Vector3 (0.5f, 0.5f), box.center + new Vector3 (0.75f, 0.75f));
			break;
		case LookDir.East:
			Gizmos.DrawLine (box.center + new Vector3 (0.5f, 0), box.center + new Vector3 (0.7f, 0));
			break;
		case LookDir.SouthEast:
			Gizmos.DrawLine (box.center + new Vector3 (0.5f, -0.5f), box.center + new Vector3 (0.75f, -0.75f));
			break;
		case LookDir.South:
			Gizmos.DrawLine (box.center + new Vector3 (0, -0.5f), box.center + new Vector3 (0, -0.75f));
			break;
		case LookDir.SouthWest:
			Gizmos.DrawLine (box.center + new Vector3 (-0.5f, -0.5f), box.center + new Vector3 (-0.75f, -0.75f));
			break;
		case LookDir.West:
			Gizmos.DrawLine (box.center + new Vector3 (-0.5f, 0), box.center + new Vector3 (-0.75f, 0));
			break;
		case LookDir.NorthWest:
			Gizmos.DrawLine (box.center + new Vector3 (-0.5f, 0.5f), box.center + new Vector3 (-0.75f, 0.75f));
			break;
		}
    }
    #endregion
}
