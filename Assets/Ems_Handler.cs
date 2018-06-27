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
  private int Time = 250; 


	// Config stuff
	public bool debug_mode; // set to GUI output
	public bool ems_live;  // activate EMS
	public float ems_triggerDistance = 1.0f; // TODO: Test for a proper value

	// Initialize with large value TODO: Solve properly, and also the post-frame reset in LateUpdate
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
	// TODO: Check what happens with buttons that were already correctly pressed
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
				StartEMS(1);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		// Emergency Stop when pressing "End"
		if (Input.GetKey (KeyCode.End)) {
			ems_live = false;
			Ems_SendMessage(EmsModule+"C"+c+"I"+ems_Intensity+"T1");
			//Ems_SendMessage(EmsModule+"C"+c+"I"+ems_Intensity+"T0001");
		}

	}

	void OnGUI(){
		if(debug_mode){
			GUI.Label (new Rect(0,0,100,100), "EMS - Level = " + ems_Intensity);
		}
	}

	// Calculation of EMS Intensity and EMS-Activation in LateUpdate, since all Position reports come in during Update. Avoids excution order configuration
	void LateUpdate(){

		ems_Intensity = (int) (System.Math.Ceiling(60 + (1.0f - ems_lowestDistance / ems_triggerDistance) * 40) * 10); // TODO: Check if 600 - 1000 (60 % - 100 %) is really the right actuation range

		if(ems_Intensity > 1000 || ems_lowestDistance < ems_triggerDistance/6){
				ems_Intensity = 1000;
		}

		if(ems_Intensity < 600 || ems_lowestDistance_right < ems_lowestDistance){
				ems_Intensity = 0;
		}




		// TODO: Solve after-frame reset in a proper way
	  ems_lowestDistance = 10000.0f;
		ems_lowestDistance_right = 10000.0f;

	}

}
