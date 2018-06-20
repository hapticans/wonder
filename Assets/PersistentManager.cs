using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManager : MonoBehaviour {

    public static PersistentManager Instance { get; private set; }

	public SequenceElement procedure { get; set; }

    private int procedureRunner = 0;

     private int procedureTarget = 0;

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

	public bool isStepValid(string name)
	{
		return procedure.checkIfCorrect(name);
	}

    public bool isProcedureDone(bool sqeunceJustDone)
	{
        if(sqeunceJustDone){procedureRunner++;}
        if(procedureRunner >= procedureTarget){
            return true;
        }
		return false;
	}

    public void setProceduretarget(int initTarget)
	{
        if(procedureTarget == 0){
            Debug.Log("<");
            procedureTarget = initTarget;
        }
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void printSequence()
    {
        print(procedure);
        print(procedure.counterReached());
    }
}
