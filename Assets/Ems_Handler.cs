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

  // EMS related variables
	public string EmsModule = "EMS09RH";
  public string Server = "192.168.43.1";
  public int Port = 5005;
	public int channel = 1;
	private int ems_Intensity;
	public int ems_mode = 2;
	private int Time = 250;
	private bool emstest_running = false;


	// Config stuff
	public bool ems_lockedByInput;
	public bool debug_mode = true; // set to GUI output
	public bool ems_live = false;  // activate EMS
	public float ems_triggerDistance = 0.2f;

	// Initialize with large value
	private float ems_lowestDistance = 10000.0f;
	private float ems_lowestDistance_right = 10000.0f;


	// to be called by wrong buttons during their update, in order to check their position for EMS relevance
	public void CheckEMS_wrongButton(Vector3 button_position){

		float distance_min = System.Math.Min(Vector3.Distance(button_position,player.position), Vector3.Distance(button_position,predictor.position));

		if(ems_lowestDistance > distance_min){
			ems_lowestDistance = distance_min;
		}
	}
	// to be called by right buttons during their update, in order to check their position for EMS relevance
	public void CheckEMS_rightButton(Vector3 button_position){

		float distance_min = System.Math.Min(Vector3.Distance(button_position,player.position), Vector3.Distance(button_position,predictor.position));

		if(ems_lowestDistance_right > distance_min){
			ems_lowestDistance_right = distance_min;
		}
	}

	public void StartEMS(int startchannel, int startintensity, int starttime)
	{
		Ems_SendMessage(EmsModule+"C"+startchannel+"I"+startintensity+"T"+starttime);
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

	// Calibration run, cycling through 60% - 100% - 60% EMS
	private IEnumerator EmsTest(){
		emstest_running = true;
		yield return new WaitForSeconds(1.0f);
		StartEMS(channel, 60, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS(channel, 70, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS(channel, 80, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS(channel, 90, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS(channel, 100, 999);
		yield return new WaitForSeconds(0.8f);
		StartEMS(channel, 90, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS(channel, 80, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS(channel, 70, 500);
		yield return new WaitForSeconds(0.35f);
		StartEMS(channel, 60, 500);
		emstest_running = false;
	}

	public IEnumerator LockEMS_enum(){
		ems_lockedByInput = true;
		yield return new WaitForSeconds(1.0f);
		ems_lockedByInput = false;
	}

	public void LockEMS(){
		StartCoroutine(LockEMS_enum());
	}


	// EMS Activation
	IEnumerator Start () {
		while(true){
			yield return new WaitForSeconds(0.15f); //Alternative
			//yield return new WaitForSeconds((((float)(Time)) / 1000) - 150);
			if(ems_live && ems_Intensity != 0 && !ems_lockedByInput){
				StartEMS(channel, ems_Intensity, Time);
			}
		}
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.End)) {		// Key Event for Emergency Stop when pressing "End"
			ems_live = false;
			StartEMS(channel, 0, 1);
		}
		if (Input.GetKeyDown (KeyCode.Insert) && !emstest_running){ // Key Event for Calibration run when pressing "Insert"
			StartCoroutine(EmsTest());
		}
	}

	void OnGUI(){
		if(debug_mode){
			GUI.Label (new Rect(0,0,100,100), "EMS - Level = " + ems_Intensity);
		}
	}

	void EmsStyle_1(){ // simple first linear approach, small deadzone
		ems_Intensity = (int) (System.Math.Ceiling(60 + (1.0f - ems_lowestDistance / ems_triggerDistance) * 40));

		if(ems_Intensity < 60 || ems_lowestDistance_right < ems_lowestDistance){
			ems_Intensity = 0;
		}
		else if(ems_Intensity > 100 || ems_lowestDistance < ems_triggerDistance/6){
			ems_Intensity = 100;
		}
	}

	void EmsStyle_2(){ // harsher actuation, but larger deadzone around the right button
		ems_Intensity = (int) (System.Math.Ceiling(60 + (1.1f - ems_lowestDistance / (ems_triggerDistance*0.8f)) * 40));
		if(ems_Intensity < 60 || ems_lowestDistance_right < (ems_lowestDistance*1.2f)){
			ems_Intensity = 0;
		}

		else if(ems_Intensity > 100 || ems_lowestDistance < ems_triggerDistance/2){
			ems_Intensity = 100;
		}
	}

	void EmsStyle_3(){	// binary, but even larger deadzone. lower triggerdistance
		if(ems_lowestDistance_right < (ems_lowestDistance*1.3f) || ems_lowestDistance > ems_triggerDistance*0.75f){
			ems_Intensity = 0;
		}
		else{
			ems_Intensity = 100;
		}
	}

	void EmsStyle_4(){
		if(ems_lowestDistance < ems_triggerDistance/2f){
			ems_Intensity = 100;
		}
		else if(ems_lowestDistance < ems_triggerDistance/1.6f){
			ems_Intensity = 90;
		}
		else if(ems_lowestDistance < ems_triggerDistance/1.4f){
			ems_Intensity = 80;
		}
		else if(ems_lowestDistance < ems_triggerDistance/1.2f){
			ems_Intensity = 70;
		}
		else if(ems_lowestDistance < ems_triggerDistance){
			ems_Intensity = 60;
		}
		else ems_Intensity = 0;

		if(ems_lowestDistance_right < (ems_lowestDistance * 1.05f)){
			ems_Intensity = 0;
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

		// TODO: Solve after-frame reset in a proper way
	  ems_lowestDistance = 10000.0f;
		ems_lowestDistance_right = 10000.0f;
	}
}
