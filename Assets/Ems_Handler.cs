using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Ems_Handler : MonoBehaviour {

	public Transform player; // player collision object
	public Transform predictor; // predictor collision object

  // EMS related variable
	//-----------------------------------------------------------------------
	// IP and port for the Mobile device running the App and hosting the Hotspot
	public string Server = "192.168.43.1";
	public int Port = 5005;

	// Names of the two MoveYourBody-Toolkit Arduino devices, and their respective channels
	public string EmsModule_UpDown = "EMS08IK";
	public string EmsModule_LeftRight = "EMS09RH";
	public int channel_down = 0;
	public int channel_up = 1;
	public int channel_left = 0;
	public int channel_right = 1;

	// EMS activation mode of operation
	public int ems_mode = 2;

	// EMS activation time per trigger
	private int Time = 175;
	private int ems_Intensity;

	private bool emstest_running = false;
	private bool checkingDirection = false;
	private bool pulsating = false;
	public int ems_lockedByInput;

	// Config stuff
	public bool debug_mode = true; // set to GUI output
	public bool ems_active = true;  // activate EMS
	public bool directionCheck_active = true; // EMS deactivates when user moves towards the right button. Caution: Test phase. TODO: Test
	public bool positiveFeedbackActive = true; // pulsating encouragement downwards when facing the right button, and left/right for the right direction when approaching a knob TODO: implement condition
	public float ems_triggerDistance = 0.2f;



	// Initialize with large value
	private Vector3 ems_wrong_angles;
	private float ems_lowDist_wrong = 10000.0f;
	private float ems_lowDist_correct = 10000.0f;
	private float playerButtonDistHelper;

	// EMS Activation
	IEnumerator Start () {
		while(true){

			yield return new WaitForSeconds(0.15f); //Alternative
			// yield return new WaitForSeconds(((float)(Time)) / 1000);
			if(ems_active && ems_Intensity != 0 && ems_lockedByInput == 0){
				StartEMS_UpDown(channel_up, ems_Intensity, Time);
			}
		}
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.End)) {		// Key Event for Emergency Stop when pressing "End"
			ems_active = false;
			StartEMS_UpDown(channel_up, 1, 1);
		}
		if (Input.GetKeyDown (KeyCode.Insert) && !emstest_running){ // Key Event for Calibration run when pressing "Insert"
			StartCoroutine(EmsTest());
		}
	}

	void OnGUI(){
		if(debug_mode && ems_lockedByInput == 0){
			GUI.Label (new Rect(0,0,100,100), "EMS - Level = " + ems_Intensity);
		}
		if(ems_lockedByInput > 0 && debug_mode){
			GUI.Label (new Rect(0,0,100,100), "EMS - Level = 0");
			GUI.Label (new Rect(0,20,100,100), "EMS locked by right input or direction");
		}
	}

	// Calculation of EMS Intensity and EMS-Activation in LateUpdate, since all Position reports come in during Update. Avoids excution order configuration
	void LateUpdate(){
		switch(ems_mode){
			case 1:
				EmsStyle_1();
				break;
			case 2:
				EmsStyle_2();
				break;
			case 3:
				EmsStyle_3();
				break;
			case 4:
				EmsStyle_4();
				break;
		}
		if(!checkingDirection && directionCheck_active){
			StartCoroutine(CheckDirection(ems_lowDist_correct, player.position));
	  }
		//Debug.Log(ems_wrong_angles.y);


		// TODO: Solve after-frame reset in a proper way
	  ems_lowDist_wrong = 10000.0f;
		ems_lowDist_correct = 10000.0f;
	}

	void EmsStyle_1(){ // simple first linear approach, small deadzone
		ems_Intensity = (int) (System.Math.Ceiling(60 + (1.0f - ems_lowDist_wrong / ems_triggerDistance) * 40));

		if(ems_Intensity < 60 || ems_lowDist_correct < ems_lowDist_wrong){
			ems_Intensity = 0;
		}
		else if(ems_Intensity > 100 || ems_lowDist_wrong < ems_triggerDistance/6){
			ems_Intensity = 100;
		}
	}

	void EmsStyle_2(){ // harsher actuation, but larger deadzone around the right button
		ems_Intensity = (int) (System.Math.Ceiling(60 + (1.1f - ems_lowDist_wrong / (ems_triggerDistance*0.8f)) * 40));
		if(ems_Intensity < 60 || ems_lowDist_correct < (ems_lowDist_wrong*1.2f)){
			ems_Intensity = 0;
		}

		else if(ems_Intensity > 100 || ems_lowDist_wrong < ems_triggerDistance/2){
			ems_Intensity = 100;
		}
	}

	void EmsStyle_3(){	// binary, but even larger deadzone. lower triggerdistance
		if(ems_lowDist_correct < (ems_lowDist_wrong*1.3f) || ems_lowDist_wrong > ems_triggerDistance*0.75f){
			ems_Intensity = 0;
		}
		else{
			ems_Intensity = 100;
		}
	}

	void EmsStyle_4(){
		if(ems_lowDist_wrong < ems_triggerDistance/2f){
			ems_Intensity = 100;
		}
		else if(ems_lowDist_wrong < ems_triggerDistance/1.6f){
			ems_Intensity = 90;
		}
		else if(ems_lowDist_wrong < ems_triggerDistance/1.4f){
			ems_Intensity = 80;
		}
		else if(ems_lowDist_wrong < ems_triggerDistance/1.2f){
			ems_Intensity = 70;
		}
		else if(ems_lowDist_wrong < ems_triggerDistance){
			ems_Intensity = 60;
		}
		else ems_Intensity = 0;

		if(ems_lowDist_correct < (ems_lowDist_wrong * 1.05f)){
			ems_Intensity = 0;
		}
	}

	// to be called by wrong buttons during their update, in order to check their position for EMS relevance
	public void CheckEMS_wrongButton(Vector3 button_position, Quaternion button_parentrotation){

		float distance_min = System.Math.Min(Vector3.Distance(button_position,player.position), Vector3.Distance(button_position,predictor.position));
		if(ems_lowDist_wrong > distance_min){
			ems_lowDist_wrong = distance_min;

			Vector3 player_button_delta = player.position - button_position;
			Quaternion look_dir = Quaternion.LookRotation(player_button_delta);
			var q = Quaternion.Inverse(button_parentrotation) * look_dir ;
			ems_wrong_angles = q.eulerAngles;
		}

	}
	// to be called by right buttons during their update, in order to check their position for EMS relevance
	public void CheckEMS_rightButton(Vector3 button_position, Quaternion button_parentrotation){

		float distance_min = System.Math.Min(Vector3.Distance(button_position,player.position), Vector3.Distance(button_position,predictor.position));
		playerButtonDistHelper = Vector3.Distance(button_position, player.position);
		if(ems_lowDist_correct > distance_min){
			ems_lowDist_correct = distance_min;
		}
	}

	public void StartEMS_UpDown(int c, int i, int t)
	{
		Ems_SendMessage(EmsModule_UpDown+"C"+c+"I"+i+"T"+t);
	}

	public void StartEMS_LeftRight(int c, int i, int t)
	{
		Ems_SendMessage(EmsModule_LeftRight+"C"+c+"I"+i+"T"+t);
	}

	public void Ems_SendMessage(string message)
	{
		Debug.Log("UDP: " + message);
		var client = new UdpClient();
		var ep = new IPEndPoint(IPAddress.Parse(Server), Port);
		client.Connect(ep);
		var data = Encoding.ASCII.GetBytes(message);
		client.Send(data, data.Length);
	}


	// To be called by CustomButton or CustomKnob after a right entry has been made.
	// Also to be called by the movement tracker, if the player moves towards the right object
	public IEnumerator LockEMS_enum(float time){
		ems_lockedByInput++;
		if(ems_active && ems_lowDist_wrong < ems_triggerDistance){
			StartEMS_UpDown(channel_up, 1, 1);
		}
		yield return new WaitForSeconds(time);
		ems_lockedByInput--;
	}

	public IEnumerator CheckDirection(float lowdist_correct, Vector3 playerPos){
		checkingDirection = true;
		Vector3 oldPlayerPosition = playerPos;
		float oldDistanceTowardsRight = playerButtonDistHelper;
		yield return new WaitForSeconds(0.5f);
		float deltaDistanceTowardsRight = oldDistanceTowardsRight - playerButtonDistHelper;
		float deltaDistancePlayer = Vector3.Distance(player.position, oldPlayerPosition);
		if(deltaDistancePlayer > 0 && deltaDistanceTowardsRight > deltaDistancePlayer * 0.98f){
			StartCoroutine(LockEMS_enum(0.5f));
		}
		checkingDirection = false;
	}

	public IEnumerator EMS_PulseLeftRight(int c, int i, int t){
		pulsating = true;
		StartEMS_LeftRight(c,i,t);
		yield return new WaitForSeconds((((float)(t)) / 500));
		pulsating = false;
	}

	public IEnumerator EMS_PulseDown(int i, int t){
		pulsating = true;
		StartEMS_LeftRight(channel_down,i,t);
		yield return new WaitForSeconds((((float)(t)) / 500));
		pulsating = false;
	}

	// Calibration run, cycling through 60% - 100% - 60% EMS
	private IEnumerator EmsTest(){
		emstest_running = true;
		yield return new WaitForSeconds(1.0f);
		StartEMS_UpDown(channel_up, 60, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS_UpDown(channel_up, 70, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS_UpDown(channel_up, 80, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS_UpDown(channel_up, 90, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS_UpDown(channel_up, 100, 999);
		yield return new WaitForSeconds(0.8f);
		StartEMS_UpDown(channel_up, 90, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS_UpDown(channel_up, 80, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS_UpDown(channel_up, 70, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS_UpDown(channel_up, 60, 500);
		emstest_running = false;
	}
}
