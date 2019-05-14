using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;

public class PlayerControllerDemo : MonoBehaviour 
{
	public int speed = 5;
	public float rotateSpeed = 10;

	public int inCarState = 3; //1: In Car, 2: Entering/Exiting, 3: Not In Car

	public bool enterCar = false;
	public bool nearHood = false;
	public bool nearTrunk = false;

	public bool canControl = true;
	bool fob = false;

	public Animator anim;
	Rigidbody rb;
	RaycastHit hit;
	GameObject cam;

	public Transform head;
	public Camera mainCamera;
	public CarControllerDemo vehicleScript;
	public GameObject activeVehicle;

	public Text controlsText;

	public CamFollow camScript;

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cam = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey(KeyCode.Tab)) 
		{
			controlsText.enabled = true;
		}
		else
		{
			controlsText.enabled = false;	
		}

		if (canControl) 
		{
			//Temporary fix to stop player from rotating after collision, which prevents ability to interact.
			if (anim.GetBool("isWalking") == false && anim.GetBool("walkback") == false)
			{
				rb.velocity = Vector3.zero;	
			}

			if(activeVehicle != null && Vector3.Distance(transform.position,activeVehicle.transform.localPosition) >= 15f)
			{
				activeVehicle = null;
			}

			if (Input.GetKey(KeyCode.W)) 
			{
				anim.SetBool("isWalking",true);
				transform.Translate(Vector3.forward * speed * Time.deltaTime);
			}
			else
			{
				anim.SetBool("isWalking",false);
			}

			if (Input.GetKey(KeyCode.S))
			{
				anim.SetBool("walkback",true);
				transform.Translate(-Vector3.forward * speed * Time.deltaTime);
			}
			else
			{
				anim.SetBool("walkback",false);
			}

			if (Input.GetKey(KeyCode.A))
			{
				transform.Rotate(-Vector3.up * rotateSpeed * Time.deltaTime);
			}

			if (Input.GetKey(KeyCode.D))
			{
				transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
			}

			//// PRESS 'Q' TO TOGGLE KEYFOB CONTROLS \\\\
			if (Input.GetKeyDown(KeyCode.Q))
			{
				if (!fob) 
				{
					StartCoroutine("KeyFob",false);

				}
				else
				{
					StartCoroutine("KeyFob",true);
				}
			}

			//// PRESS '1' TO WAVE AT CAMERA \\\\
			if (Input.GetKey(KeyCode.Alpha1)) 
			{
				head.LookAt(GameObject.Find("Main Camera").transform.position);
				anim.SetTrigger("Wave");
			}
		}

		//// VEHICLE INTERACTIONS \\\\
		if (Input.GetKeyDown(KeyCode.E) && activeVehicle != null)
		{
			switch (inCarState)
			{
			case 1:
				StartCoroutine("ExitCar");
				break;
			case 3:
				float distFromDoor = Vector3.Distance(transform.position,vehicleScript.exitPoint.position);
				if (nearHood) 
				{
					canControl = false;
					StartCoroutine(MoveToPosition(vehicleScript.hoodPoint,1f,1f,"Hood"));
				}
				else if (nearTrunk) 
				{
					canControl = false;
					StartCoroutine(MoveToPosition(vehicleScript.trunkPoint,1f,1f,"Trunk"));
				}
					else if (distFromDoor > .1f && distFromDoor < 10f) 
				{
					canControl = false;
					vehicleScript.anim.SetBool("Bags",false);
					StartCoroutine(MoveToPosition(vehicleScript.exitPoint,1f,1f,"Car"));
				}
					else if(distFromDoor <= .1f)
				{
					vehicleScript.anim.SetBool("Bags",false);
					vehicleScript.layinFrame = false;
					transform.position = vehicleScript.exitPoint.transform.position;
					transform.rotation = vehicleScript.exitPoint.transform.rotation;
					StartCoroutine("EnterCar");	
				}
				break;
			}
		}

		//// POP HOOD RELEASE FROM INSIDE CAR \\\\
		if (Input.GetKeyDown(KeyCode.Slash) && activeVehicle != null) 
		{
			if(inCarState == 1) 
			{
				vehicleScript.anim.SetTrigger("PopHood");
				vehicleScript.hoodPopped = true;
			}
		}

		//// OPEN TRUNK FROM INSIDE CAR \\\\
		if (Input.GetKeyDown(KeyCode.Backslash))
		{
			if (activeVehicle != null && inCarState == 1) 
			{
				vehicleScript.anim.SetTrigger("OpenTrunk");
				vehicleScript.trunkUp = true;
			}
		}

		//// CHECK FOR OBJECTS IN FRONT OF PLAYER \\\\
		if (Physics.Raycast(new Vector3(transform.position.x,transform.position.y + 1,transform.position.z),transform.TransformDirection(Vector3.forward),out hit,10f)) 
		{
			switch (hit.collider.gameObject.tag) 
			{
			case "Vehicle":
				activeVehicle = hit.collider.gameObject;
				vehicleScript = activeVehicle.GetComponent<CarControllerDemo>();
				break;
			}
		}

		//// KEYFOB CONTROLS \\\\
		if (fob) 
		{
			if (Input.GetKeyDown(KeyCode.Keypad5) && !vehicleScript.trunkUp) 
			{
				StartCoroutine("TrunkFob");
			}

			if (Input.GetKeyDown(KeyCode.Keypad7)) 
			{
				StartCoroutine("LockUnlockCar",false);
			}

			if (Input.GetKeyDown(KeyCode.Keypad9)) 
			{
				StartCoroutine("LockUnlockCar",true);
			}
		}
	}

	//// TRIGGER FUNCTIONS \\\\
	void OnTriggerEnter(Collider c)
	{
		switch (c.gameObject.tag) 
		{
		case "Hood":
			nearHood = true;
			break;
		case "Trunk":
			nearTrunk = true;
			break;
		default:
			break;
		}
	}

	void OnTriggerExit(Collider c)
	{
		switch (c.gameObject.tag) 
		{
		case "Hood":
			nearHood = false;
			break;
		case "Trunk":
			nearTrunk = false;
			break;
		default:
			break;
		}
	}

	//// OPEN/CLOSE DOOR FUNCTIONS \\\\
	public void OpenDoor(string whatDoor)
	{
		switch (whatDoor) 
		{
		case "Driver":
			vehicleScript.anim.SetTrigger("Open");
			break;
		case "Hood":
			anim.SetTrigger("HoodOpen");
			vehicleScript.OpenDoor("Hood");
			StartCoroutine("ResumeControl");
			break;
		case "Trunk":
			anim.SetTrigger("TrunkOpen");
			vehicleScript.OpenDoor("Trunk");
			StartCoroutine("ResumeControl");
			break;
		}
	}

	public void CloseDoor(string whatDoor)
	{
		switch (whatDoor) 
		{
		case "Driver":
			vehicleScript.anim.SetTrigger("Close");
			break;
		case "Hood":
			anim.SetTrigger("HoodClose");
			vehicleScript.CloseDoor("Hood");
			StartCoroutine("ResumeControl");
			break;
		case "Trunk":
			StartCoroutine("TrunkClose");
			break;
		}
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		cam = GameObject.Find("Main Camera");
		camScript = cam.GetComponent<CamFollow>();
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	IEnumerator ResumeControl()
	{
		yield return new WaitForSeconds(1.25f);
		canControl = true;
	}

	IEnumerator EnterCar()
	{
		if(!vehicleScript.locked)
		{
			vehicleScript.layinFrame = false;
			inCarState = 2;
			anim.SetTrigger("EnterCar");
			yield return new WaitForSeconds(2.4f);
			camScript.camState = 2;
			inCarState = 1;
			transform.SetParent(activeVehicle.transform);
			vehicleScript.canDrive = true;
		}
		else
		{
			inCarState = 2;
			anim.SetTrigger("CarLocked");
			yield return new WaitForSeconds(1f);
			inCarState = 3;
			canControl = true;
		}
	}

	IEnumerator MoveToPosition(Transform newPos,float time,float rotTime,string doorType)
	{
		float elapsedTime = 0;
		Vector3 startPos = transform.position;
		Quaternion startRot = transform.rotation;
		while(elapsedTime < rotTime)
		{
			Vector3 direction = (newPos.position - transform.position).normalized;
			Quaternion toRot = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Lerp(transform.rotation,toRot,rotateSpeed/4 * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		elapsedTime = 0;
		anim.SetBool("isWalking",true);
		while(elapsedTime < time)
		{
			transform.position = Vector3.Lerp(startPos,newPos.position,(elapsedTime/time));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		anim.SetBool("isWalking",false);
		elapsedTime = 0;
		while(elapsedTime < rotTime)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation,newPos.rotation,rotateSpeed/4 * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		rb.isKinematic = true;
		switch (doorType) 
		{
		case "Car":
			StartCoroutine("EnterCar");	
			break;
		case "Hood":
			if (vehicleScript.hoodPopped && vehicleScript.hoodUp) 
			{
				CloseDoor("Hood");
			}
			else if (vehicleScript.hoodPopped && !vehicleScript.hoodUp)
			{
				OpenDoor("Hood");
			}
			else
			{
				rb.isKinematic = false;
				canControl = true;
			}
			break;
		case "Trunk":
			if (vehicleScript.trunkUp == true) 
			{
				CloseDoor("Trunk");
			}
			else
			{
				rb.isKinematic = false;
				canControl = true;
			}
			break;
		}
	}

	IEnumerator ExitCar()
	{
		inCarState = 2;
		anim.SetTrigger("ExitCar");
		yield return new WaitForSeconds(2.4f);
		GetComponent<CapsuleCollider>().enabled = true;
		transform.SetParent(null);
		camScript.camState = 1;
		inCarState = 3;
		vehicleScript.canDrive = false;
		rb.isKinematic = false;
		canControl = true;
		yield return new WaitForSeconds(2.5f);
		if (vehicleScript.bags)
		{
			vehicleScript.anim.SetBool("Bags",true);
			vehicleScript.layinFrame = true;
		}		
	}

	IEnumerator KeyFob(bool isOut)
	{
		if (!isOut) 
		{
			anim.SetTrigger("Fob");
			yield return new WaitForSeconds(.5f);
			anim.ResetTrigger("Fob");
			fob = true;
		}else
		{
			anim.SetTrigger("HolsterFob");
			yield return new WaitForSeconds(.5f);
			anim.ResetTrigger("HolsterFob");
			fob = false;
		}
	}

	IEnumerator	LockUnlockCar(bool unlock)
	{
		LightsDemo lightingScript = activeVehicle.GetComponent<LightsDemo>();
		if (!unlock) 
		{
			anim.SetTrigger("UseFob");
			yield return new WaitForSeconds(1f);
			lightingScript.ChangeLightState(1);
			yield return new WaitForSeconds(.5f);
			lightingScript.ChangeLightState(0);
			vehicleScript.locked = true;
			fob = false;
		}
		else
		{
			anim.SetTrigger("UseFob");
			yield return new WaitForSeconds(1f);
			lightingScript.ChangeLightState(1);
			yield return new WaitForSeconds(.5f);
			lightingScript.ChangeLightState(0);
			vehicleScript.locked = false;
			fob = false;
		}
	}

	IEnumerator TrunkFob()
	{
		anim.SetTrigger("UseFob");
		yield return new WaitForSeconds(1f);
		vehicleScript.OpenDoor("Trunk");
		fob = false;
	}

	IEnumerator TrunkClose()
	{
		anim.SetTrigger("TrunkClose");
		yield return new WaitForSeconds(.5f);
		vehicleScript.CloseDoor("Trunk");
		yield return new WaitForSeconds(.5f);
		rb.isKinematic = false;
		canControl = true;
	}
}
