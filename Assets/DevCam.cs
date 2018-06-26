using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevCam : MonoBehaviour
{

	/*
		WASD: 								Planar Movement
		Q:    								Move Up
		E:    								Move Down
		Mousewheel Up/Down: 	Move Collider
    R:    								Show Cursor

	*/

	public float cameraSensitivity = 90;
	public float climbSpeed = 4;
	public float normalMoveSpeed = 10;
	public float slowMoveFactor = 0.25f;
	public float fastMoveFactor = 3;

	private float rotationX = 0.0f;
	private float rotationY = 0.0f;

	// requires a Camera Pointer Object for Collision Interaction
	public GameObject CameraPointer;
	public float camerapointer_distance = 1.0f;



	void Start ()
	{
		Cursor.visible = false;
	}

	void Update ()

	{

		//Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
		if (Input.GetAxis("Mouse ScrollWheel") > 0f && camerapointer_distance < 2.0f) // forward
		{
	 		camerapointer_distance = camerapointer_distance + 0.1f;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0f && camerapointer_distance > 0.1f) // backwards
 		{
    	camerapointer_distance = camerapointer_distance - 0.1f;
 		}


		CameraPointer.transform.position = Camera.main.transform.position + Camera.main.transform.up * -0.2f + Camera.main.transform.forward * camerapointer_distance;


		rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
		rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
		rotationY = Mathf.Clamp (rotationY, -90, 90);

		transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);


	 	transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
		transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;

		if (Input.GetKey (KeyCode.Q)) {transform.position += transform.up * climbSpeed * Time.deltaTime;}
		if (Input.GetKey (KeyCode.E)) {transform.position -= transform.up * climbSpeed * Time.deltaTime;}

		if (Input.GetKeyDown (KeyCode.R))
		{
			Cursor.visible = true;
		}
	}
}
