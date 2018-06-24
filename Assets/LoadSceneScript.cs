using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LoadSceneScript : MonoBehaviour
{

    public string SceneConfigFolder = "Scene_1";

    public GameObject persistenceManager;

	private Dictionary<String, String> sequenceDictionary;

	private String[] sequenceArray;

    // Use this for initialization
    void Start()
    {
        // Instantiate Global Singleton
        if (PersistentManager.Instance == null)
        {
            Instantiate(persistenceManager);
        };

        // Get Filepath
        string filePath = Application.dataPath + "/" + SceneConfigFolder + "/";

        LoadProcedure(filePath); 
        LoadObjects(filePath);
    }

    // Update is called once per frame
    void Update()
    {
        // Temporary Development Helpers
        //TODO: remove
        DevelopHelper dev = new DevelopHelper();
        dev.registerKeyBindings();
    }

    // Loads needed GameObjects from file
    void LoadObjects(string filepath)
    {
        string objectfile = filepath + "objects.txt"; 
        if (File.Exists(objectfile))
        {
            // Iterate through Object list
            String[] data = File.ReadAllLines(objectfile);
            for (int i = 0; i < data.Length; i++)
            {
                // Ignore comments
                if (!data[i].StartsWith("//")) {

                // Get associated Game Object
                GameObject obj = GameObject.Find(data[i]);
                if (obj != null) {
                    obj.GetComponent<Renderer>().enabled = true;
                    // Debug.LogError("Line " + i + "reads" + data[i]);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Did not load Objects. File not found: " + objectfile);
        }
    }

    void LoadEvents(String filepath)
    {
        String objectfile = filepath + "events.txt";
        if (File.Exists(objectfile))
        {
            // Iterate through Object list
            String[] data = File.ReadAllLines(objectfile);
            for (int i = 0; i < data.Length; i++)
            {
                // Ignore comments
                if (!data[i].StartsWith("//"))
                {
                    String[] line = data[i].Split(';');
                    int timeout = Int32.Parse(line[0]);
                    //TODO(specki): Parse Events 
                }
            }
        } else
        {
            Debug.LogError("Did not load Objects. File not found: " + objectfile);
        }
    }

    // Loads List of Data Objects
    void LoadProcedure(string filepath)
    {
        string objectfile = filepath + "procedure.txt";
        if (File.Exists(objectfile))
        {
            // Iterate through Object list
            String data = File.ReadAllText(objectfile);
            generateDictionaryAndArray(data);

            PersistentManager.Instance.procedure = this.parseSequenceElementRow(sequenceArray[0]);
        }
        else
        {
            Debug.LogError("Did not load Procedure. File not found: " + objectfile);
        }
    }

	void generateDictionaryAndArray(String input)
	{      
        // Save as Array
		String[] semicolonArray = input.Split(';');
        sequenceArray = new String[semicolonArray.Length];
		
		
		// Create Dictionary
        sequenceDictionary = new Dictionary<string, string>();
        for (int i = 0; i < semicolonArray.GetLength(0)-1; i++)
		{
			String[] doubledotSplit = semicolonArray[i].Split(':');
            // Copy Value to Sequence Array - discard sequence name
			sequenceArray[i] = doubledotSplit[1];
            // Add Key-Value to Dictionary with SequenceName-Sequence Definition
            sequenceDictionary.Add(doubledotSplit[0].Replace(System.Environment.NewLine, ""), doubledotSplit[1]);
		}
	}

    SequenceElement parseSequenceElementRow(String input)
	{
		String[] commaSplit = input.Split(',');      

        // Behandle ganze Zeile
		SequenceElement[] elements = new SequenceElement[commaSplit.Length-2];
        for (int j = 2; j < commaSplit.Length; j++)
        {
			elements[j-2] = parseSequenceElement(commaSplit[j]);
        }
              
		SequenceElement seq = new Sequence(
            Int32.Parse(commaSplit[0]),
            Convert.ToBoolean(commaSplit[1]),
			elements
		);

		return seq;
	}

    // Parsing of single String
	SequenceElement parseSequenceElement(String input)
	{
        // Pruefe ob Referenz oder InputElement
        if (sequenceDictionary.ContainsKey(input))
        {
			String value;
			sequenceDictionary.TryGetValue(input, out value);
			return parseSequenceElementRow(value);
        }
        else
        {
            //Debug.Log("Setting up input element:" + input);
            return new InputElement(input);
        }
}
}
