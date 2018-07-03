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

	private Material mat;
	private Color prevColor;
	// IMPORTANT: Made for a Knob with Rigidbody (Gravity disabled), Box Collider,
	// Hinge Joint with all movement restricted, except for the y- axis, which is limited to -90 to 90 degrees


  void Start () {
		mat = GetComponent<Renderer>().material;
    prevColor = mat.color;
  }

	void Update () {
		handleEMS();
		AnimatedKnobUpdate();
	}

  public void checkKnob(String direction){		// direction must be L for left or R for right in the case of the standard knob
		if (PersistentManager.Instance.markIfValidStep(name + "_" + direction))
    {
      mat.color = Color.green;

			Debug.Log("Pressed Correct Button");
			StartCoroutine(ems_handler.LockEMS_enum());

			int lastProcedureStep = PersistentManager.Instance.isProcedureDone(false);
      if(lastProcedureStep == 1)
      {
        feedbackForSucces(true);
      }
			else if(lastProcedureStep == 2)
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
		if(transform.localEulerAngles.y > 50 && transform.localEulerAngles.y <= 90 && !pressed && !released){
			pressed = true;
			checkKnob("R");
			//Debug.Log("right " + name + "_R");
		}
		else if(transform.localEulerAngles.y >= 270 && transform.localEulerAngles.y < 310 && !pressed && !released){
			pressed = true;
			checkKnob("L");
			//Debug.Log("left " + name + "_L");

		}
		else if(((transform.localEulerAngles.y > 350 && transform.localEulerAngles.y <= 360) || (transform.localEulerAngles.y >= 0 && transform.localEulerAngles.y < 10)) && pressed){
			pressed = false;
			released = true;
			//Debug.Log("released");
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
