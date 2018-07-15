using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CustomButton : MonoBehaviour
{
	// EMS - related
	public Ems_Handler ems_handler;
	// Button Animation related
	public float returnSpeed = 10.0f;
	public float activationDistance = 0.5f;
	private bool pressed = false;
	private bool released = false;
	private Vector3 startPosition;

	private Material mat;

	private Color prevColor;

	private bool istriggering;

	public GameObject controller;

	Collider buttonCollider1;

	Collider buttonCollider2;

	Collider controllerCollider;

	public bool enableDebugoutput;

	// Use this for initialization
	void Start()
	{
		startPosition = transform.localPosition; // Reset position for Button Animation
		mat = GetComponent<Renderer>().material;
		prevColor = mat.color;

		// for the old collision solution
		controllerCollider = controller.GetComponent<Collider>();
		buttonCollider1 = GameObject.Find(name.Remove(name.Length - 6) + "Cube1").GetComponent<Collider>();
		buttonCollider2 = GameObject.Find(name.Remove(name.Length - 6) + "Cube2").GetComponent<Collider>();

	}

	// Update is called once per frame
	void Update()
	{
		handleEMS();
		//TODO: Deactivate Cube1/Cube2 Colliders, Add RigidBody and Box Collider to all Buttons, then set all their Mass to 20, Drag to 100, and uncheck "Use Gravity"
		//TODO: For each Button, lock all rotational Axis. Axis movement is restricted in the function.
		AnimatedButtonUpdate();
		//ColliderInteraction();		// The old solution
	}

	public void checkButton()
	{
		if (PersistentManager.Instance.markIfValidStep(name))
		{
			mat.color = Color.green;
            StartCoroutine(resetColor());

            if (enableDebugoutput) { Debug.Log("Pressed Correct Button"); };
			StartCoroutine(ems_handler.LockEMS_enum(2.0f));

			int lastProcedureStep = PersistentManager.Instance.isProcedureDone(false);

            if (lastProcedureStep == 1)
			{
                // Write new Log to File
                PersistentManager.Instance.writeLogFile("Zwischenschritt");
                feedbackForSucces(true);
			}
			else
			if (lastProcedureStep == 2)
			{
                // Write new Log to File
                PersistentManager.Instance.writeLogFile("Sucessfully solved Procedure");
                feedbackForSucces(false);
			}
		}
		else
		{
			StartCoroutine(resetColor());
			mat.color = Color.red;
			if (PersistentManager.Instance.isProcedureFailed())
            {
                // Write new Log to File
                PersistentManager.Instance.writeLogFile("Failed Procedure");
                feedbackForFailure();
			}
			if (enableDebugoutput) { Debug.Log("Pressed wrong button"); };
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

	void handleEMS()
	{
		if (PersistentManager.Instance.isStepValid(name))
		{
			ems_handler.CheckEMS_rightButton(transform.position, transform.parent.rotation, false, false);
		}
		else
		{
			ems_handler.CheckEMS_wrongButton(transform.position, transform.parent.rotation);
		}
	}

	void ColliderInteraction()
	{
		if (buttonCollider1.bounds.Intersects(controllerCollider.bounds) || buttonCollider2.bounds.Intersects(controllerCollider.bounds))
		{
			if (!istriggering)
			{
				istriggering = true;
				checkButton();
			}
		}
		else
		{
			if (istriggering)
			{
				Debug.Log(istriggering);
				istriggering = false;
			}
		}
	}

	void AnimatedButtonUpdate()
	{
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
			//Debug.Log("pressed " + localPos.y);
			checkButton();
		}
		//Deactivate when released
		else if (pressComplete <= 0.01f && pressed)
		{
			pressed = false;
			released = true;
			//Debug.Log("released " + localPos.y);
		}
	}

	//false entspricht alles geschafft
	//true entspricht Zwischenschritt
	public void feedbackForSucces(bool zwischenschritt)
    {
        if (zwischenschritt)
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
            GameObject test = GameObject.Find("AlarmLight1");
            Component f = test.GetComponent(typeof(LightAlarm));
            LightAlarm l = (LightAlarm)f;

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

	public void feedbackForFailure()
	{
		Application.Quit();
		mat.color = Color.black;
		StartCoroutine(resetColor());
	}
}
