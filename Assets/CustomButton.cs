using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CustomButton : MonoBehaviour {
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

    // Use this for initialization
    void Start () {
		startPosition = transform.localPosition; // Reset position for Button Animation
		mat = GetComponent<Renderer>().material;
    prevColor = mat.color;

		// for the old collision solution
    controllerCollider = controller.GetComponent<Collider>();
    buttonCollider1 = GameObject.Find(name.Remove(name.Length - 6) + "Cube1").GetComponent<Collider>();
    buttonCollider2 = GameObject.Find(name.Remove(name.Length - 6) + "Cube2").GetComponent<Collider>();

    }

    /*
	void OnDrawGizmos(){
		List<Vector3> copyOfVerts = new List<Vector3>();
		GetComponent<MeshFilter>().mesh.GetVertices(copyOfVerts);
		Vector3[] array = copyOfVerts.ToArray();
			for (var i = 0; i < array.Length; i++) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(array[i], 0.001f);
		}
	}
    */


	// Update is called once per frame
	void Update () {
			handleEMS();
		 //TODO: Deactivate Cube1/Cube2 Colliders, Add RigidBody and Box Collider to all Buttons, then set all their Mass to 20, Drag to 100, and uncheck "Use Gravity"
		 //TODO: For each Button, lock all rotational Axis. Axis movement is restricted in the function.
		AnimatedButtonUpdate();
		//ColliderInteraction();		// The old solution
		// Debug.Log(istriggering);


	}

    public void checkButton()
    {
		if (PersistentManager.Instance.markIfValidStep(name))
        {
            mat.color = Color.green;

			Debug.Log("Pressed Correct Button");
			StartCoroutine(ems_handler.LockEMS_enum(2.0f));

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
				ems_handler.CheckEMS_rightButton(transform.position, transform.parent.rotation);
			}
			else{
				ems_handler.CheckEMS_wrongButton(transform.position, transform.parent.rotation);
			}
    }

		void ColliderInteraction(){
			if (buttonCollider1.bounds.Intersects (controllerCollider.bounds) || buttonCollider2.bounds.Intersects(controllerCollider.bounds)) {
				if (!istriggering) {
					istriggering = true;
					checkButton();
	    	}
			}
			else
			{
				if (istriggering)
				{
					Debug.Log (istriggering);
					istriggering = false;
				}
	  	}
		}

		void AnimatedButtonUpdate(){
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
					checkButton();
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
