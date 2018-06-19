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
        checkButton();
	}

    public void checkButton()
    {
        //print(name);
        //PersistentManager.Instance.printSequence();
        if (PersistentManager.Instance.isStepValid(name))
        {
            Material mat = GetComponent<Renderer>().material;
            mat.color = Color.green;
            //Debug.Log("Pressed Correct Button");
        }
        else
        {
            //Debug.Log("Pressed wrong button");
        }
    }
	

}
