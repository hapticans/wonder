using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnCollisionEnter() {
			Debug.Log("Colliding with button");
	}
	
	void OnTriggerEnter() {
		GetComponent<Renderer>().enabled = false;
		if (name == GameStatic.getCorrectButton()) {
			Debug.Log("incorrect button");
		} else {
			Debug.Log("triggering");
		}
	}
	
}
