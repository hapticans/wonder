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
		if (PersistentManager.Instance.isStepValid(name)){
			ems_handler.CheckEMS_rightButton(transform.position);	
		}
		
		else{
			ems_handler.CheckEMS_wrongButton(transform.position);
		}
		
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
            int lastProcedureStep = PersistentManager.Instance.isProcedureDone(false);
            if(lastProcedureStep == 1)
            {
                feedbackForSucces(true);
            }else
            if(lastProcedureStep == 2)
            {
                feedbackForSucces(false);
            }
        }
        else
		{
			StartCoroutine(resetColor());
            mat.color = Color.red;
            if(PersistentManager.Instance.isProcedureFailed()){
                feedbackForFailure();
            }
            Debug.Log("Pressed wrong button");
        }
    }


    IEnumerator resetColor()
    {
        yield return new WaitForSeconds(1);
		GetComponent<Renderer>().material.color = prevColor;
    }
	
    //false entspricht alles geschafft
    //true entspricht Zwischenschritt
    public void feedbackForSucces(bool zwischenschritt)
    {
        if(zwischenschritt)
        {
            mat.color = Color.white;
            StartCoroutine(resetColor());
            Debug.Log("Zwischenschritt!");
        }else{
            mat.color = Color.blue;
            StartCoroutine(resetColor());
            Debug.Log("Done!");
        }
    }

    public void feedbackForFailure(){
        mat.color = Color.black;
        StartCoroutine(resetColor());
        Debug.Log("Failed!");
    }
}
