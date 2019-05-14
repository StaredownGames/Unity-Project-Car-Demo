using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsDemo : MonoBehaviour 
{
	int dir = 0;

	public Material headlights;
	public Material foglights;
	public Material taillight;
	public Material taillightL;
	public Material taillightR;
	public Material reverselights;
	public Material turnlightL;
	public Material turnlightR;

	public bool headlightsON = false;
	public bool foglightsON = false;
	public bool brakelightsON = false;
	public bool hazardlightsON = false;
	public bool reverselightsON = false;
	public bool leftlightsON = false;
	public bool rightlightsON = false;

	public List<Light> lights;

	CarControllerDemo carScript;

	// Use this for initialization
	void Start () 
	{
		carScript = GetComponentInParent<CarControllerDemo>();
		ChangeLightState(0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(carScript.canDrive)
		{
			if (carScript.reverse) 
			{
				reverselightsON = true;
			}
			else
			{
				reverselightsON = false;
			}

			if (Input.GetKey(KeyCode.S)) 
			{
				if (!hazardlightsON) 
				{
					if (!brakelightsON) 
					{
						brakelightsON = true;
					}
				}
			}
			else
			{
				brakelightsON = false;
			}


			if (Input.GetKeyDown(KeyCode.L)) 
			{
				if (!headlightsON) 
				{
					ChangeLightState(2);
					headlightsON = true;
				}
				else
				{
					ChangeLightState(0);
					headlightsON = false;
				}
			}

			if (Input.GetKeyDown(KeyCode.F)) 
			{
				if (!foglightsON) 
				{
					foglights.EnableKeyword("_EMISSION");
					foglightsON = true;
				}
				else
				{
					foglights.DisableKeyword("_EMISSION");
					foglightsON = false;
				}
			}

			if (Input.GetKeyDown(KeyCode.H)) 
			{
				if (!hazardlightsON) 
				{
					ChangeLightState(3);
				}
				else
				{
					if (headlightsON) 
					{
						ChangeLightState(2);
					}
					else
					{
						ChangeLightState(0);
					}
				}
			}

			if (Input.GetKeyDown(KeyCode.Comma)) 
			{
				if (!leftlightsON && !rightlightsON && !hazardlightsON) 
				{
					ChangeLightState(4);
				}	
			}

			if (Input.GetKeyDown(KeyCode.Period)) 
			{
				if (!rightlightsON && !leftlightsON && !hazardlightsON) 
				{
					ChangeLightState(5);
				}	
			}

			if (Input.GetKeyUp(KeyCode.A) && !hazardlightsON) 
			{
				if (headlightsON) 
				{
					ChangeLightState(2);			
				}
				else
				{
					ChangeLightState(0);
				}
			}

			if (Input.GetKeyUp(KeyCode.D)) 
			{
				if (headlightsON) 
				{
					ChangeLightState(2);	
				}
				else
				{
					ChangeLightState(0);
				}
			}

			if (!brakelightsON) 
			{
				if (!hazardlightsON) 
				{
					if (!headlightsON && !leftlightsON && !rightlightsON) 
					{
						taillightL.DisableKeyword("_EMISSION");
						taillightR.DisableKeyword("_EMISSION");
						taillight.DisableKeyword("_EMISSION");
						lights[4].intensity = 0;
						lights[5].intensity = 0;
					}
					else if (headlightsON && !leftlightsON && !rightlightsON) 
					{
						lights[2].intensity = 1.25f;
						lights[3].intensity = 1.25f;
					}
					else if (headlightsON && leftlightsON && !rightlightsON) 
					{
						lights[3].intensity = 1.25f;
					}
					else if (headlightsON && !leftlightsON && rightlightsON) 
					{
						lights[2].intensity = 1.25f;
					}
					else if (!headlightsON && !leftlightsON && rightlightsON) 
					{
						lights[2].intensity = 0f;
					}
					else if (!headlightsON && leftlightsON && !rightlightsON) 
					{
						lights[3].intensity = 0f;
					}
				}
			}
			else
			{
				if (!hazardlightsON) 
				{
					if (!leftlightsON && !rightlightsON) 
					{
						taillightL.EnableKeyword("_EMISSION");
						taillightR.EnableKeyword("_EMISSION");
						taillight.EnableKeyword("_EMISSION");
						lights[4].intensity = 3f;
						lights[5].intensity = 3f;
					}
					else if (leftlightsON) 
					{
						taillightR.EnableKeyword("_EMISSION");
						taillight.EnableKeyword("_EMISSION");
						lights[5].intensity = 3f;
					}
					else if (rightlightsON) 
					{
						taillightR.EnableKeyword("_EMISSION");
						taillight.EnableKeyword("_EMISSION");
						lights[4].intensity = 3f;
					}
				}
			}

			if (!reverselightsON) 
			{
				reverselights.DisableKeyword("_EMISSION");
			}
			else
			{
				reverselights.EnableKeyword("_EMISSION");
			}
		}
		else
		{
			if (!carScript.trunkUp) 
			{
				reverselights.DisableKeyword("_EMISSION");
			}
			else
			{
				reverselights.EnableKeyword("_EMISSION");
			}
		}
	}

	public void ChangeLightState(int lightState)
	{
		switch (lightState) 
		{
		case 0: // All lights OFF
			foglights.DisableKeyword("_EMISSION");
			headlights.DisableKeyword("_EMISSION");
			turnlightL.DisableKeyword("_EMISSION");
			turnlightR.DisableKeyword("_EMISSION");
			taillightL.DisableKeyword("_EMISSION");
			taillightR.DisableKeyword("_EMISSION");


			foreach (var light in lights) {
				light.intensity = 0;
			}
			hazardlightsON = false;
			rightlightsON = false;
			leftlightsON = false;
			headlightsON = false;
			break;
		case 1: //Parking lights ON
			turnlightL.EnableKeyword("_EMISSION");
			turnlightR.EnableKeyword("_EMISSION");
			taillightL.EnableKeyword("_EMISSION");
			taillightR.EnableKeyword("_EMISSION");
			lights[2].intensity = 1.25f;
			lights[3].intensity = 1.25f;
			lights[4].intensity = 1;
			lights[5].intensity = 1;
			hazardlightsON = false;
			rightlightsON = false;
			leftlightsON = false;
			break;
		case 2: // All lights ON
			headlights.EnableKeyword("_EMISSION");
			turnlightL.EnableKeyword("_EMISSION");
			turnlightR.EnableKeyword("_EMISSION");
			taillightL.EnableKeyword("_EMISSION");
			taillightR.EnableKeyword("_EMISSION");
			lights[0].intensity = 1;
			lights[1].intensity = 1;
			lights[2].intensity = 1.25f;
			lights[3].intensity = 1.25f;
			lights[4].intensity = 1;
			lights[5].intensity = 1;
			hazardlightsON = false;
			rightlightsON = false;
			leftlightsON = false;
			break;
		case 3: // Hazard lights ON
			dir = 1;
			hazardlightsON = true;
			rightlightsON = false;
			leftlightsON = false;
			StartCoroutine(Signal(dir));
			break;
		case 4: // Flash Left
			dir = 2;
			leftlightsON = true;
			rightlightsON = false;
			StartCoroutine(Signal(dir));
			break;
		case 5: // Flash Right
			dir = 3;
			rightlightsON = true;
			leftlightsON = false;
			StartCoroutine(Signal(dir));
			break;
		}
	}

	IEnumerator Signal(int signalDirection)
	{
		switch (signalDirection) 
		{
		case 1: // Hazards
			while (hazardlightsON) 
			{
				turnlightL.EnableKeyword("_EMISSION");
				turnlightR.EnableKeyword("_EMISSION");
				taillightL.EnableKeyword("_EMISSION");
				taillightR.EnableKeyword("_EMISSION");
				lights[2].intensity = 3f;
				lights[3].intensity = 3f;
				lights[4].intensity = 1;
				lights[5].intensity = 1;
				yield return new WaitForSeconds(.5f);
				turnlightL.DisableKeyword("_EMISSION");
				turnlightR.DisableKeyword("_EMISSION");
				taillightL.DisableKeyword("_EMISSION");
				taillightR.DisableKeyword("_EMISSION");
				lights[2].intensity = 0;
				lights[3].intensity = 0;
				lights[4].intensity = 0;
				lights[5].intensity = 0;
				yield return new WaitForSeconds(.5f);
			}
			break;
		case 2: // Left Signal
			while (leftlightsON) 
			{
				turnlightR.EnableKeyword("_EMISSION");
				taillightL.EnableKeyword("_EMISSION");
				lights[2].intensity = 3f;
				lights[5].intensity = 1;
				yield return new WaitForSeconds(.5f);
				turnlightR.DisableKeyword("_EMISSION");
				taillightL.DisableKeyword("_EMISSION");
				lights[2].intensity = 0;
				lights[5].intensity = 0;
				yield return new WaitForSeconds(.5f);
			}
			break;
		case 3: // Right Signal
			while (rightlightsON) 
			{
				turnlightL.EnableKeyword("_EMISSION");
				taillightR.EnableKeyword("_EMISSION");
				lights[4].intensity = 1;
				lights[3].intensity = 3f;
				yield return new WaitForSeconds(.5f);
				turnlightL.DisableKeyword("_EMISSION");
				taillightR.DisableKeyword("_EMISSION");
				lights[4].intensity = 0;
				lights[3].intensity = 0;
				yield return new WaitForSeconds(.5f);
			}
			break;
		}
	}
}
