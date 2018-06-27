using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CustomButton : MonoBehaviour {

	public Ems_Handler ems_handler;
	
	private Material mat;

	private Color prevColor;
	
	private bool istriggering;
	
	public GameObject controller;

    Collider buttonCollider1;

    Collider buttonCollider2;

    Collider controllerCollider;

    String basis = "OutputLED_";

    // Use this for initialization
    void Start () {
		mat = GetComponent<Renderer>().material;
        prevColor = mat.color;
        
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
		//handleEMS();	// Deactivated for now
		// Debug.Log(istriggering);
		Debug.Log (istriggering);

		if (buttonCollider1.bounds.Intersects (controllerCollider.bounds) || buttonCollider2.bounds.Intersects(controllerCollider.bounds)) {
			if (!istriggering) {
				istriggering = true;
				checkButton();
            }
		} else {
			if (istriggering) {
				Debug.Log (istriggering);
				istriggering = false;
			}
        }
	}

    public void checkButton()
    {
		if (PersistentManager.Instance.markIfValidStep(name))
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
	
    //false entspricht alles geschafft
    //true entspricht Zwischenschritt
    public void feedbackForSucces(bool zwischenschritt)
    {
        if(zwischenschritt)
        {
            mat.color = Color.white;
            StartCoroutine(resetColor());
            //Debug.Log("Zwischenschritt!");
        }else{
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
