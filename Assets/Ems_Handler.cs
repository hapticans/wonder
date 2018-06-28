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
  private static string Server = "192.168.43.1";
  private static int Port = 5005;
	private int channel = 1;
	public int ems_Intensity;
	public int ems_mode = 2;
	private int Time = 250;


	// Config stuff
	public bool debug_mode; // set to GUI output
	public bool ems_live;  // activate EMS
	public float ems_triggerDistance = 0.3f;

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

	public void StartEMS(int c)
	{
		Ems_SendMessage(EmsModule+"C"+c+"I"+ems_Intensity+"T"+Time);
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



	// EMS Activation
	IEnumerator Start () {
		while(true){
			yield return new WaitForSeconds(((float)(Time)) / 1000);
			if(ems_live && ems_Intensity != 0 ){
				StartEMS(channel);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		// Emergency Stop when pressing "End"
		if (Input.GetKey (KeyCode.End)) {
			ems_live = false;
			Ems_SendMessage(EmsModule+"C"+channel+"I"+0+"T1");
			//Ems_SendMessage(EmsModule+"C"+c+"I"+ems_Intensity+"T0001");
		}
	}

	void OnGUI(){
		if(debug_mode){
			GUI.Label (new Rect(0,0,100,100), "EMS - Level = " + ems_Intensity);
		}
	}

	void EmsStyle_1(){ // simple first linear approach, small deadzone
		ems_Intensity = (int) (System.Math.Ceiling(60 + (1.0f - ems_lowestDistance / ems_triggerDistance) * 40) * 10);

		if(ems_Intensity < 600 || ems_lowestDistance_right < ems_lowestDistance){
			ems_Intensity = 0;
		}
		else if(ems_Intensity > 1000 || ems_lowestDistance < ems_triggerDistance/6){
			ems_Intensity = 1000;
		}
	}

	void EmsStyle_2(){ // harsher actuation, but larger deadzone around the right button
		ems_Intensity = (int) (System.Math.Ceiling(60 + (1.1f - ems_lowestDistance / (ems_triggerDistance*0.8f)) * 40) * 10);
		if(ems_Intensity < 600 || ems_lowestDistance_right < (ems_lowestDistance*1.2f)){
			ems_Intensity = 0;
		}

		else if(ems_Intensity > 1000 || ems_lowestDistance < ems_triggerDistance/4){
			ems_Intensity = 1000;
		}
	}

	void EmsStyle_3(){	// binary, but even larger deadzone. lower triggerdistance
		if(ems_lowestDistance_right < (ems_lowestDistance*1.3f) || ems_lowestDistance > ems_triggerDistance*0.75f){
			ems_Intensity = 0;
		}
		else{
			ems_Intensity = 1000;
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
		}

		// TODO: Solve after-frame reset in a proper way
	  ems_lowestDistance = 10000.0f;
		ems_lowestDistance_right = 10000.0f;
	}
}
