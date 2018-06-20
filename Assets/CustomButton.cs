using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButton : MonoBehaviour {

	private Material mat;

	private Color prevColor;

	// Use this for initialization
	void Start () {
		mat = GetComponent<Renderer>().material;
        prevColor = mat.color;
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
        if (PersistentManager.Instance.isStepValid(name))
        {
            mat.color = Color.green;
            prevColor = Color.green;
            //Debug.Log("Pressed Correct Button");
            if(PersistentManager.Instance.isProcedureDone(false)){
                feedbackForSucces();
            }
        }
        else
		{
			StartCoroutine(resetColor());
            mat.color = Color.red;
            Debug.Log("Pressed wrong button");
        }
    }


    IEnumerator resetColor()
    {
        yield return new WaitForSeconds(1);
		GetComponent<Renderer>().material.color = prevColor;
    }
	
    public void feedbackForSucces(){
        mat.color = Color.blue;
                StartCoroutine(resetColor());
                Debug.Log("Done!");
    }
}