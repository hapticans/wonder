using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAlarm : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        SetAlarm();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetAlarm()
    {
        StartCoroutine("redAlarm");
    }

	public void StopAlarm()
	{
		StopCoroutine ("redalarm");
	}


    IEnumerator redAlarm()
    {
        while (true)
        {
            Light light = GetComponent<Light>();
            for(int i=1; i<=8; i++) { 
                light.intensity = 0.1f * i;
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 8; i >= 1; i--)
            {
                light.intensity = 0.1f * i;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
