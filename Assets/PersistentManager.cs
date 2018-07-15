using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistentManager : MonoBehaviour
{

	public static PersistentManager Instance { get; private set; }

	public SequenceElement procedure { get; set; }

	private int procedureRunner = 0;

	private int procedureTarget = 0;

	//0 entspricht false
	//1 entspricht einem zwischenschritt
	//2 entspricht geschafft
	private int lastProcedureStep = 0;

	//Fehler und maximal erlaubte Fehler
	private int strikes = 0;
	private int maxStrikes = 3;

    // Log
    List<string> logmessages = new List<string>();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public bool isStepValid(string name)
	{
		return procedure.checkIfCorrect(name, false);
	}

	public bool markIfValidStep(string name)
	{
		return procedure.checkIfCorrect(name, true);
	}

	public int isProcedureDone(bool sqeunceJustDone)
	{
		int returnValue = lastProcedureStep;
		if (sqeunceJustDone)
		{
			lastProcedureStep = 1;
			procedureRunner++;
			if (procedureRunner >= procedureTarget)
			{
				lastProcedureStep = 2;
			}
        }
        else
		{
			lastProcedureStep = 0;
		}
		return returnValue;
	}

	public bool isProcedureFailed()
	{
		strikes++;
		if (strikes >= maxStrikes)
		{
			return true;
		}
		return false;
	}

	public void setProceduretarget(int initTarget)
	{
		if (procedureTarget == 0)
		{
			procedureTarget = initTarget;
		}
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void printSequence()
	{
		print(procedure);
		print(procedure.counterReached());
	}

	public int getStrikes()
	{
		return strikes;
	}

	public void addLogMessage(string message)
	{
		logmessages.Add(message);
	}

	public void writeLogFile(string reason)
	{
		String output = "";
        foreach (string element in logmessages)
        {
            output += element + ";" + System.Environment.NewLine;
        }
        output += reason;

		File.WriteAllText(Application.dataPath + "/logs/" + DateTime.Now.ToString().Replace("/","-").Replace(" ", "").Replace(":","_"), output);      
	}

}
