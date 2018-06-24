using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButton : MonoBehaviour {

	private Material mat;

	private Color prevColor;
	
	private bool istriggering;
	
	public GameObject controller;
	
	Renderer buttonCollider;
	Collider controllerCollider;

	// Use this for initialization
	void Start () {
		mat = GetComponent<Renderer>().material;
        prevColor = mat.color;
		
		controllerCollider = controller.GetComponent<Collider>();
		buttonCollider = GetComponent<Renderer>();
	}

	void OnDrawGizmos(){
		List<Vector3> copyOfVerts = new List<Vector3>();
		GetComponent<MeshFilter>().mesh.GetVertices(copyOfVerts);
		Vector3[] array = copyOfVerts.ToArray();
			for (var i = 0; i < array.Length; i++) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(array[i], 0.001f);
		}
	}

	
	// Update is called once per frame
	void Update () {
		Debug.Log(buttonCollider.bounds.size);
		if (buttonCollider.bounds.Intersects (controllerCollider.bounds)) {
			if (!istriggering) {
				istriggering = true;
				checkButton();
			}
		} else {
			istriggering = false;
			waitOneSecond();
		}
	}

    public void checkButton()
    {
        if (PersistentManager.Instance.isStepValid(name))
        {
            mat.color = Color.green;
            prevColor = Color.green;
            
			Debug.Log("Pressed Correct Button");
            
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

	IEnumerator waitOneSecond()
	{
		yield return new WaitForSeconds(1);
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
			prevColor = Color.white;
            StartCoroutine(resetColor());
            Debug.Log("Zwischenschritt!");
        }else{
            mat.color = Color.blue;
			prevColor = Color.blue;
            StartCoroutine(resetColor());
            Debug.Log("Done!");
        }
    }

    public void feedbackForFailure(){
		Application.Quit();
        mat.color = Color.black;
        StartCoroutine(resetColor());
        Debug.Log("Failed!");
    }
}