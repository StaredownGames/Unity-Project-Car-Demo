using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class CarControllerDemo : MonoBehaviour 
{
	
	//// ACCELERATION \\\\
	public float currentSpeed;
	public float maxSpeed;

	//// STEERING \\\\
	public float angle;
	float curAngle;
	public float stWhlAngle;
	public float engineRPM;

	public WheelCollider Wheel_FL;
	public WheelCollider Wheel_FR;
	public WheelCollider Wheel_RL;
	public WheelCollider Wheel_RR;
	float timeCountL = 0.0f;
	float timeCountR = 0.0f;

	public bool locked = false;
	public bool reverse = false;
	public bool canDrive = false;
	public bool hoodPopped = false;
	public bool hoodUp = false;
	public bool trunkUp = false;
	public bool slipping = false;
	bool roof = false;
	bool windowsF = false;
	bool windowsR = false;
	public bool layinFrame = false;

	public Transform exitPoint;
	public Transform exitPointDR;
	public Transform hoodPoint;
	public Transform trunkPoint;
	public GameObject steeringWheel;
	public Animator anim;

	public bool bags = false;

	protected Rigidbody rb;

	public Vector3 CenterOfMass;

	Vector3 localVelocity;



	public WheelInfo[] Wheels;



	public float MotorPower = 5000f;

	public float SteerAngle = 35f;



	[Range(0, 1)]

	public float KeepGrip = 1f;

	public float Grip = 5f;

	void Awake () {

		rb = GetComponent<Rigidbody>();

		rb.centerOfMass = CenterOfMass;

		OnValidate();

	}
	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float HInput = Input.GetAxis("Horizontal");
		float steering = HInput * angle;
		currentSpeed = rb.velocity.magnitude * 2.237f; //convert to mph; 3.6 for kph

		Wheel_FL.steerAngle = steering;
		Wheel_FR.steerAngle = steering;
		curAngle = Wheel_FR.steerAngle/steering;
		stWhlAngle = 180f * curAngle * HInput;
	
		if (canDrive) 
		{
			///CHECK IF WE ARE BACKING UP (FOR LIGHT SCRIPT)\\\
			localVelocity = transform.InverseTransformDirection(rb.velocity); 
			if (localVelocity.z < -.5f) 
			{
				reverse = true;	
			}
			else
			{
				reverse = false;
			}

			///ENABLE AIR BAG SUSPENSION\\\
			if (Input.GetKeyDown(KeyCode.B)) 
			{
				bags = true;
			}
			///DISABLE AIR BAG SUSPENSION\\\
			if (Input.GetKeyDown(KeyCode.N)) 
			{
				bags = false;	
			}

			////WINDOW CONTROLS\\\\
			if (Input.GetKeyDown(KeyCode.Keypad5)) 
			{
				if (!roof) 
				{
					anim.SetTrigger("OpenRoof");
					roof = true;
				}
				else
				{
					anim.SetTrigger("CloseRoof");
					roof = false;
				}
			}

			if (Input.GetKeyDown(KeyCode.Keypad4)) 
			{
				if (!windowsF) 
				{
					anim.SetTrigger("OpenWindow");
					windowsF = true;
				}
				else
				{
					anim.SetTrigger("CloseWindow");
					windowsF = false;
				}
			}

			if (Input.GetKeyDown(KeyCode.Keypad6)) 
			{
				if (!windowsR) 
				{
					windowsR = true;
				}
				else
				{
					windowsR = false;
				}
			}

			for(int i = 0; i < Wheels.Length; i++)

			{

				if (Wheels[i].Motor)
				{

					Wheels[i].WheelCollider.motorTorque = Input.GetAxisRaw("Vertical") * MotorPower;
				}
		    }

			rb.AddForceAtPosition(transform.up * rb.velocity.magnitude * -0.1f * Grip, transform.position + transform.rotation * CenterOfMass);

			steeringWheel.transform.localRotation = Quaternion.Euler(6,stWhlAngle,0);
		}
		else
		{
			rb.velocity = Vector3.zero;
		}

		MoveWheels(Wheel_FL);
		MoveWheels(Wheel_FR);
		MoveWheels(Wheel_RL);
		MoveWheels(Wheel_RR);
	}

	public void MoveWheels(WheelCollider wc)
	{
		if (GetComponent<Collider>().transform.childCount == 0) {
			return;
		}

		Transform wheelMesh = wc.transform.GetChild(0);

		Vector3 position;
		Quaternion rotation;
		wc.GetWorldPose(out position,out rotation);
		wheelMesh.transform.position = position;
		if (!layinFrame && canDrive) 
		{
			wheelMesh.transform.rotation = rotation;
		}
		else if(layinFrame && !canDrive)
		{
			if (wc.name == "FR") 
			{
				
				Quaternion oldRot = wheelMesh.transform.localRotation;
				wheelMesh.transform.localRotation = Quaternion.Slerp(oldRot,Quaternion.Euler(0,0,6f),timeCountR/12);
				timeCountR += Time.deltaTime;
			}
			else if (wc.name == "FL") 
			{
				Quaternion oldRot = wheelMesh.transform.localRotation;
				wheelMesh.transform.localRotation = Quaternion.Slerp(oldRot,Quaternion.Euler(0,0,-6f),timeCountL/12);
				timeCountL += Time.deltaTime;
			}
		}
		else if(!layinFrame && !canDrive)
		{
			if (wc.name == "FR") 
			{

				Quaternion oldRot = wheelMesh.transform.localRotation;
				wheelMesh.transform.localRotation = Quaternion.Slerp(oldRot,Quaternion.Euler(0,0,0),timeCountR/12);
				timeCountR += Time.deltaTime;
			}
			else if (wc.name == "FL") 
			{
				Quaternion oldRot = wheelMesh.transform.localRotation;
				wheelMesh.transform.localRotation = Quaternion.Slerp(oldRot,Quaternion.Euler(0,0,0),timeCountL/12);
				timeCountL += Time.deltaTime;
			}
		}
	}

	public void OpenDoor(string whatDoor)
	{
		switch (whatDoor) 
		{
		case "Driver":
			anim.SetTrigger("Open");
			break;
		case "Passenger":
			break;
		case "DriverR":
			break;
		case "PassengerR":
			break;
		case "Hood":
			anim.SetTrigger("OpenHood");
			hoodUp = true;
			break;
		case "Trunk":
			anim.SetTrigger("OpenTrunk");
			trunkUp = true;
			break;
		}
	}
    
	public void CloseDoor(string whatDoor)
	{
		switch (whatDoor) 
		{
		case "Driver":
			anim.SetTrigger("Close");
			break;
		case "Passenger":
			break;
		case "DriverR":
			break;
		case "PassengerR":
			break;
		case "Hood":
			anim.SetTrigger("CloseHood");
			hoodPopped = false;
			hoodUp = false;
			break;
		case "Trunk":
			anim.SetTrigger("CloseTrunk");
			trunkUp = false;
			break;
		}
	}

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag == "Ground") 
		{
			layinFrame = true;	
		}
	}

	void OnValidate()

	{

		Debug.Log("Validate");

		for (int i = 0; i < Wheels.Length; i++)

		{

			//settings

			var ffriction = Wheels[i].WheelCollider.forwardFriction;

			var sfriction = Wheels[i].WheelCollider.sidewaysFriction;

			ffriction.asymptoteValue = Wheels[i].WheelCollider.forwardFriction.extremumValue * KeepGrip * 0.998f + 0.002f;

			sfriction.extremumValue = 1f;

			ffriction.extremumSlip = 1f;

			ffriction.asymptoteSlip = 2f;

			ffriction.stiffness = Grip;

			sfriction.extremumValue = 1f;

			sfriction.asymptoteValue = Wheels[i].WheelCollider.sidewaysFriction.extremumValue * KeepGrip * 0.998f + 0.002f;

			sfriction.extremumSlip = 0.5f;

			sfriction.asymptoteSlip = 1f;

			sfriction.stiffness = Grip;

			Wheels[i].WheelCollider.forwardFriction = ffriction;

			Wheels[i].WheelCollider.sidewaysFriction = sfriction;

		}

	}



	[System.Serializable]

	public struct WheelInfo

	{

		public WheelCollider WheelCollider;

		public Transform MeshRenderer;

		public bool Steer;

		public bool Motor;

		[HideInInspector]

		public float Rotation;

	}
}
