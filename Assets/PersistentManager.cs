using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManager : MonoBehaviour {

    public static PersistentManager Instance { get; private set; }

    public string[] procedure { get; set; }

    private int procedureRunner = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    public string getCorrectStep()
    {
        return procedure[procedureRunner];
    }

    // Return true if successfully switched to next step.
    // Return false if at End of Steps
    public bool advanceStep()
    {
        if (procedureRunner < procedure.Length-1)
        {
            procedureRunner++;
            return true;
        } else
        {
            return false;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
