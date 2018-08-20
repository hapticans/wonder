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
	private int ems_SideIntensity;
	private int ems_downIntensity;

	private bool emstest_running = false;
	private bool checkingDirection = false;
	private bool pulsating = false;
	private int ems_lockedByInput;

	// Config stuff
	public bool debug_mode; // set to GUI output
	public bool ems_negativeFeedbackActive;  // activate negative feedback EMS
	public bool directionCheck_active; // EMS deactivates when user moves towards the right button. Caution: Test phase. TODO: Test
	public bool ems_positiveFeedbackActive; // pulsating encouragement downwards when facing the right button, and left/right for the right direction when approaching a knob TODO: implement condition
	public bool sideDeviceConnected;
	public float ems_triggerDistance = 0.2f;



	// Initialize with large value
	private Vector3 ems_wrong_angles;
	private int currentDirection;
	private float ems_lowDist_wrong = 10000.0f;
	private float ems_lowDist_correct = 10000.0f;
	private float playerButtonDistHelper;

	// EMS Activation
	IEnumerator Start () {
		while(true){

			yield return new WaitForSeconds(0.15f); //Alternative
			// yield return new WaitForSeconds(((float)(Time)) / 1000);
			if(ems_Intensity != 0 && ems_lockedByInput == 0 && ems_negativeFeedbackActive){
				StartEMS_UpDown(channel_up, ems_Intensity, Time);
			}
			if(ems_downIntensity != 0 && ems_lockedByInput == 0 && ems_positiveFeedbackActive){
				StartEMS_UpDown(channel_down, ems_downIntensity, Time);
			}
			if(ems_SideIntensity != 0 && ems_lockedByInput == 0 && sideDeviceConnected && !ems_positiveFeedbackActive && ems_negativeFeedbackActive){
				StartEMS_LeftRight(currentDirection, ems_SideIntensity, Time);
			}
		}
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.End)) {		// Key Event for Emergency Stop when pressing "End"
			EmergencyStop();

		}
		if (Input.GetKeyDown (KeyCode.Insert) && !emstest_running && ems_negativeFeedbackActive){ // Key Event for Calibration run when pressing "Insert"
			StartCoroutine(EmsTest());
		}
	}

	void OnGUI(){
		if(debug_mode && ems_lockedByInput == 0){
			GUI.Label (new Rect(0,0,200,100), "EMS - Level = " + ems_Intensity);
		}
		if(debug_mode && sideDeviceConnected && !ems_positiveFeedbackActive){
			GUI.Label (new Rect(0,20,200,100), "Side Intensity = " + ems_SideIntensity);
		}
		if(debug_mode && sideDeviceConnected && !ems_positiveFeedbackActive && ems_SideIntensity > 1){
			switch (currentDirection){
				case 0:
					GUI.Label (new Rect(0,40,300,100), "Left");
					break;
				case 1:
					GUI.Label (new Rect(0,40,300,100), "Right");
					break;
			}
		}
		if(debug_mode && ems_lockedByInput > 0){
			GUI.Label (new Rect(0,0,200,100), "EMS - Level = 0");
			GUI.Label (new Rect(0,60,300,100), "EMS locked by right input or direction");
		}
		if(debug_mode && ems_positiveFeedbackActive && pulsating){
			GUI.Label (new Rect(0,80,200,100), "Pulsating");
		}
		if(debug_mode && ems_positiveFeedbackActive && ems_mode == 2){
			GUI.Label (new Rect(0,100,200,100), "EMS - Level Downwards = " + ems_downIntensity);
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
		calculateDownIntensity();
		calculateSideIntensity();

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
		else if(ems_Intensity > 100 || ems_lowDist_wrong < ems_triggerDistance/3){
			ems_Intensity = 100;
		}
	}

	void EmsStyle_2(){ // harsher actuation, but larger deadzone around the right button
		ems_Intensity = (int) (System.Math.Ceiling(60 + (1.2f - ems_lowDist_wrong / (ems_triggerDistance*1.0f)) * 40));
		if(ems_Intensity < 60 || ems_lowDist_correct < (ems_lowDist_wrong*1.2f)){
			ems_Intensity = 0;
		}

		else if(ems_Intensity > 100 || ems_lowDist_wrong < ems_triggerDistance/2.2f){
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
		if(ems_lowDist_wrong < ems_triggerDistance/2.0f){
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

	private void calculateSideIntensity(){
		if(sideDeviceConnected && !ems_positiveFeedbackActive){
			if(ems_wrong_angles.y < 330.0f && ems_wrong_angles.y > 290.0f){
				currentDirection = 0;
				ems_SideIntensity = ems_Intensity - 10;
			}
			else if(ems_wrong_angles.y > 30.0f && ems_wrong_angles.y < 70.0f){
				currentDirection = 1;
				ems_SideIntensity = ems_Intensity - 10;
			}
			if(ems_SideIntensity < 50){
				ems_SideIntensity = 0;
			}
		}
		else{
			ems_SideIntensity = 0;
		}
	}

	private void calculateDownIntensity(){
		if(ems_positiveFeedbackActive){
			ems_downIntensity = (int) (System.Math.Ceiling(60 + (1.2f - ems_lowDist_correct / (ems_triggerDistance*0.8f)) * 40));
			if(ems_Intensity > 0 || ems_lowDist_correct > ems_triggerDistance || ems_downIntensity < 60){
				ems_downIntensity = 0;
			}
			if(ems_downIntensity > 100){
				ems_downIntensity = 100;
			}
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
	public void CheckEMS_rightButton(Vector3 button_position, Quaternion button_parentrotation, bool isKnob, bool knob_leftIsCorrect){

		float distance_min = System.Math.Min(Vector3.Distance(button_position,player.position), Vector3.Distance(button_position,predictor.position));
		playerButtonDistHelper = Vector3.Distance(button_position, player.position);
		if(ems_lowDist_correct > distance_min){
			ems_lowDist_correct = distance_min;
		}

		if(ems_positiveFeedbackActive){
			if(isKnob && knob_leftIsCorrect && !pulsating  && sideDeviceConnected && ems_lowDist_correct < ems_triggerDistance / 2){
				//Debug.Log("Approaching Knob (Left Correct)");
				StartCoroutine(EMS_PulseLeftRight(channel_left, 60, 250));
			}
			else if(isKnob && !knob_leftIsCorrect && !pulsating  && sideDeviceConnected && ems_lowDist_correct < ems_triggerDistance / 2){
				//Debug.Log("Approaching Knob (Right Correct)");
				StartCoroutine(EMS_PulseLeftRight(channel_right, 60, 250));
			}
			else if(!isKnob && !pulsating && ems_lowDist_correct < ems_triggerDistance / 1.4f && ems_mode != 2)
			{
				Debug.Log("Approaching Correct Button");
				StartCoroutine(EMS_PulseDown(60,250));  // deactivated for now; TODO: Make toggle for Pulse or continuous positive Feedback
			}
		}
	}

	public void StartEMS_UpDown(int c, int i, int t)
	{
		if(ems_negativeFeedbackActive || ems_positiveFeedbackActive){
			Ems_SendMessage(EmsModule_UpDown+"C"+c+"I"+i+"T"+t);
		}
	}

	public void StartEMS_LeftRight(int c, int i, int t)
	{
		if(ems_negativeFeedbackActive || ems_positiveFeedbackActive){
			Ems_SendMessage(EmsModule_LeftRight+"C"+c+"I"+i+"T"+t);
		}
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
		if(ems_lowDist_wrong < ems_triggerDistance && ems_negativeFeedbackActive){
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
		StartEMS_LeftRight(c,i,t);
		yield return new WaitForSeconds((((float)(t)) / 500));
		StartEMS_LeftRight(c,i,t);
		yield return new WaitForSeconds(3.0f);
		pulsating = false;
	}

	public IEnumerator EMS_PulseDown(int i, int t){
		pulsating = true;
		StartEMS_UpDown(channel_down,i,t);
		yield return new WaitForSeconds((((float)(t)) / 500));
		StartEMS_UpDown(channel_down,i,t);
		yield return new WaitForSeconds((((float)(t)) / 500));
		StartEMS_UpDown(channel_down,i,t);
		yield return new WaitForSeconds(2.0f);
		pulsating = false;
	}
	private void EmergencyStop(){
		StartEMS_UpDown(channel_up, 1, 1);
		if(ems_positiveFeedbackActive){
			StartEMS_UpDown(channel_down, 1, 1);
		}
		if(sideDeviceConnected && (ems_positiveFeedbackActive || ems_negativeFeedbackActive)){
			StartEMS_LeftRight(channel_left, 1, 1);
			StartEMS_LeftRight(channel_right, 1, 1);
		}
		ems_negativeFeedbackActive = false;
		ems_positiveFeedbackActive = false;
	}

	// Calibration run, cycling through 60% - 100% - 60% EMS
	private IEnumerator EmsTest(){
		emstest_running = true;
		if(ems_negativeFeedbackActive){
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
	  }
		emstest_running = false;
	}
}
