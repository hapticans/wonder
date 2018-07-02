using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CustomKnob : MonoBehaviour {
	// EMS - related
	public Ems_Handler ems_handler;
	// Button Animation related
	private bool pressed = false;
	private bool released = false;
	private float startRotation;

	private Material mat;
	private Color prevColor;

    // Use this for initialization
    void Start () {
		mat = GetComponent<Renderer>().material;
    prevColor = mat.color;
		startRotation = transform.eulerAngles.y;
    }

	void Update () {
		handleEMS();
		AnimatedKnobUpdate();
	}

    public void checkKnob(String direction)		// direction must be L for left or R for right in the case of the standard knob
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

    void handleEMS(){  // TODO: Include feedback for starting to flip into the wrong direction
    	if (PersistentManager.Instance.isStepValid(name + "_L") || PersistentManager.Instance.isStepValid(name + "_R")){
				ems_handler.CheckEMS_rightButton(transform.position);
			}
			else{
				ems_handler.CheckEMS_wrongButton(transform.position);
			}
    }

		void AnimatedKnobUpdate(){
			released = false;
			float difference;
			difference = transform.eulerAngles.y - startRotation;
			//Debug.Log(transform.eulerAngles.y);

			if(transform.eulerAngles.y > 30 && transform.eulerAngles.y < 50 && !pressed && !released){
				//checkKnob("R");
				pressed = true;
				checkKnob("R");
				Debug.Log("right " + name + "_R");
			}
			else if(transform.eulerAngles.y < 230 && transform.eulerAngles.y > 210 && !pressed && !released){
				pressed = true;
				Debug.Log("left " + name + "_L");
				checkKnob("L");
			}
			else if(transform.eulerAngles.y > 310 && transform.eulerAngles.y < 335 && pressed){
				pressed = false;
				released = true;
				Debug.Log("released");
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

        StartCoroutine(resetColor());
        Debug.Log("Failed!");
    }
}
