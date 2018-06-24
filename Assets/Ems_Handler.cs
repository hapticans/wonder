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


  // EMS related variables, derived from Samuel Navas' code; TODO: Check if used properly
	public string EmsModule = "EMS09RH";
  private static string Server = "192.168.43.1";
  private static int Port = 5005;
  public int channel = 1;
  public int ems_Intensity;
  public int Time = 250; // TODO: Check if Time should be modified during actuation


	// Config stuff
	public bool debug_mode = false; // set to GUI output
	public bool ems_live  = true;  // activate EMS
	public float ems_triggerDistance = 3.0f; // TODO: Test for a proper value

	// Initialize with large value TODO: Solve properly, and also the post-frame reset in LateUpdate
	public float ems_lowestDistance = 10000.0f;
	public float ems_lowestDistance_right = 10000.0f;


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
		Ems_SendMessage(EmsModule+"C"+c+"I"+ems_Intensity+"T"+Time); //TODO: remove Debug.Log
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



	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnGUI(){
		if(debug_mode){
			GUI.Label (new Rect(0,0,100,100), "EMS - Level = " + ems_Intensity);
		}
	}

	// Calculation of EMS Intensity and EMS-Activation in LateUpdate, since all Position reports come in during Update. Avoids excution order configuration
	void LateUpdate(){
		float ems_currentActivationLevel = 60 + (1.0f - ems_lowestDistance / ems_triggerDistance) * 40;

		if(ems_currentActivationLevel > 100 || ems_lowestDistance < ems_triggerDistance/8){
				ems_currentActivationLevel = 100;
		}

		if(ems_currentActivationLevel < 60 || ems_lowestDistance_right < ems_lowestDistance){

				ems_currentActivationLevel = 0;
		}

			ems_Intensity = (int) System.Math.Ceiling(ems_currentActivationLevel) * 10; // TODO: Check if 600 - 1000 (60 % - 100 %) is really the right actuation range


		// EMS Activation


			StartEMS(1);

		// TODO: Solve after-frame reset in a proper way
	  ems_lowestDistance = 10000.0f;
		ems_lowestDistance_right = 10000.0f;

	}

}
