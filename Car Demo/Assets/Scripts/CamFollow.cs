using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour 
{
	public int camState = 1;
	public Vector3 curPos;
	public Transform close;
	public Transform chase;
	public Transform follow;
	public Quaternion curRot;
	public Quaternion newRot;

	// Use this for initialization
	void Start () 
	{
		close = GameObject.Find("Cam_Close").transform;
		chase = GameObject.Find("Cam_Chase").transform;
		follow = GameObject.Find("Cam_Follow").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		curPos = transform.position;
		curRot = transform.rotation;
		switch (camState) 
		{
		case 1: //Out of Car
			transform.position = Vector3.Lerp(curPos,follow.position,2*Time.deltaTime);
			transform.rotation = Quaternion.Lerp(curRot,follow.rotation,2*Time.deltaTime);
			break;
		case 2: //In Car
			transform.position = Vector3.Lerp(curPos,chase.position,Time.deltaTime);
			transform.rotation = Quaternion.Lerp(curRot,chase.rotation,Time.deltaTime);
			break;
		case 3: //Close
			transform.position = Vector3.Lerp(curPos,close.position,2*Time.deltaTime);
			transform.rotation = Quaternion.Lerp(curRot,close.rotation,2*Time.deltaTime);
			break;
		default:
			break;
		}
	}
}
