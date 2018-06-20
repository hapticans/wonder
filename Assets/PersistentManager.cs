using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentManager : MonoBehaviour {

    public static PersistentManager Instance { get; private set; }

	public SequenceElement procedure { get; set; }

    private int procedureRunner = 0;

     private int procedureTarget = 0;

    //0 entspricht false
    //1 entspricht einem zwischenschritt
    //2 entspricht geschafft
     private int lastProcedureStep = 0;

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

    public int isProcedureDone(bool sqeunceJustDone)
	{
        int returnValue = lastProcedureStep;
        if(sqeunceJustDone)
        {
            lastProcedureStep = 1;
            procedureRunner++;
            if(procedureRunner >= procedureTarget){
                lastProcedureStep = 2;
            }
        }else{
            lastProcedureStep = 0;
        }
		return returnValue;
	}

    public void setProceduretarget(int initTarget)
	{
        if(procedureTarget == 0){
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
