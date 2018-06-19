using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButton : MonoBehaviour {

	private Material mat;

	private Material originalMaterial;

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
		string correct = PersistentManager.Instance.getCorrectStep();
		originalMaterial = GetComponent<Renderer>().material;

		mat = originalMaterial;
        if (name == correct)
        {
            mat.color = Color.green;
            Debug.Log("Pressed Correct Button");
            PersistentManager.Instance.advanceStep();
        }
        else
		{
			StartCoroutine(resetColor);
            mat.color = Color.red;
            Debug.Log("Pressed wrong button");
        }
    }


    IEnumerator resetColor()
    {
        yield return new WaitForSeconds(1);
		GetComponent<Renderer>().material = originalMaterial;
    }
	
}
