using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManager : MonoBehaviour {

    public static PersistentManager Instance { get; private set; }

	public SequenceElement procedure { get; set; }

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

	public string isStepValid(string name)
	{
		return procedure.checkIfCorrect(name);
	}



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
