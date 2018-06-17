using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureController : MonoBehaviour {

    // Denotes the order of the buttons to be pressed
    static string[] procedure;
    // Current Index in List of button order
    int currentIndex;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    string getCorrectButton() => procedure[currentIndex];


}
