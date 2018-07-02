using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CustomKnob : MonoBehaviour {
	// EMS - related
	public Ems_Handler ems_handler;
	// Button Animation related
	public float returnSpeed = 10.0f;
	public float activationDistance = 0.5f;
	private bool pressed = false;
	private bool released = false;
	private Vector3 startPosition;
	private Quaternion startRot;



	private Material mat;

	private Color prevColor;


	private bool istriggering;

	public GameObject controller;

  String basis = "OutputLED_";

    // Use this for initialization
    void Start () {
		startPosition = transform.localPosition; // Reset position for Button Animation
		mat = GetComponent<Renderer>().material;
    prevColor = mat.color;
		startRot = transform.rotation;

    }

	void Update () {

		//handleEMS();
		transform.rotation=Quaternion.Lerp (transform.rotation, startRot, 15.0f*Time.deltaTime);
		//AnimatedKnobUpdate();
	}

    public void checkKnob(String direction)		// direction must be A or B in the case of the standard knob
    {
		if (PersistentManager.Instance.markIfValidStep(name + "_" + direction))
        {
            mat.color = Color.green;

			Debug.Log("Pressed Correct Button");

			int lastProcedureStep = PersistentManager.Instance.isProcedureDone(false);
            if(lastProcedureStep == 1)
            {
                feedbackForSucces(true);
            }else
            if(lastProcedureStep == 2)
            {
                feedbackForSucces(false);
            }
        }
        else
		{
			StartCoroutine(resetColor());
            mat.color = Color.red;
            if(PersistentManager.Instance.isProcedureFailed()){
                feedbackForFailure();
            }
            Debug.Log("Pressed wrong button");
        }
    }

	IEnumerator waitOneSecond()
	{
		yield return new WaitForSeconds(1);
	}

    IEnumerator resetColor()
    {
        yield return new WaitForSeconds(1);
		GetComponent<Renderer>().material.color = prevColor;
    }

    void handleEMS(){
    	if (PersistentManager.Instance.isStepValid(name)){
				ems_handler.CheckEMS_rightButton(transform.position);
			}
			else{
				ems_handler.CheckEMS_wrongButton(transform.position);
			}
    }

		void AnimatedKnobUpdate(){
			released = false;
			float distance;

			Vector3 localPos = transform.localPosition;
			localPos.x = startPosition.x;
			localPos.z = startPosition.z;
			//localPos.y = Mathf.Clamp(localPos.y, -0.4f, 0f);

			transform.localPosition = localPos;

			// Move back to startPosition if not obstructed
			transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * returnSpeed);

			// trigger distance calculation
			Vector3 xyzdistance = transform.localPosition - startPosition;
			distance = Math.Abs(xyzdistance.y);


			float pressComplete = Mathf.Clamp(1 / activationDistance * distance, 0f, 1f);

			//Activate when pressed
			if (pressComplete >= 0.025f && !pressed && !released)
			{
					// TODO: Insert Triggered events here
					pressed = true;
					Debug.Log("pressed " + localPos.y);

			}
			//Deactivate when released
			else if (pressComplete <= 0.01f && pressed)
			{
					pressed = false;
					released = true;
					Debug.Log("released " + localPos.y);
			}
		}

    //false entspricht alles geschafft
    //true entspricht Zwischenschritt
    public void feedbackForSucces(bool zwischenschritt)
    {
        if(zwischenschritt)
        {
            mat.color = Color.white;
            StartCoroutine(resetColor());
            //Debug.Log("Zwischenschritt!");
        }
				else
				{
            mat.color = Color.blue;
            StartCoroutine(resetColor());
            //Debug.Log("Done!");

			// Disable Light Alarm
			LightAlarm light1 = (LightAlarm)GameObject.Find("AlarmLight1").GetComponent(typeof(LightAlarm));
			light1.StopAlarm();

			LightAlarm light2 = (LightAlarm)GameObject.Find("AlarmLight3").GetComponent(typeof(LightAlarm));
			light2.StopAlarm();

			LightAlarm light3 = (LightAlarm)GameObject.Find("AlarmLight2").GetComponent(typeof(LightAlarm));
			light3.StopAlarm();

			// Disable Audio Alarm
			GameObject.Find("LeftSpeaker").GetComponent<AudioSource>().Stop();
			GameObject.Find("RightSpeaker").GetComponent<AudioSource>().Stop();

        }
    }

    public void feedbackForFailure(){
		Application.Quit();
        mat.color = Color.black;
        // SceneManager.UnloadSceneAsync("Scene");
        //Debug.Log(basis + (PersistentManager.Instance.getStrikes() + 1));
        // Destroy(GameObject.Find("[CameraRig]"));

        StartCoroutine(resetColor());
        Debug.Log("Failed!");
    }
}
