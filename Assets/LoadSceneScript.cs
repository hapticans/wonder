using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LoadSceneScript : MonoBehaviour
{

    public string SceneConfigFolder = "Scene_1";

    public GameObject persistenceManager;

    // Use this for initialization
    void Start()
    {
        // Instantiate Global Singleton
        if (PersistentManager.Instance == null)
        {
            Instantiate(persistenceManager);
        }

        // Get Filepath
        string filePath = Application.dataPath + "/" + SceneConfigFolder + "/";

        LoadProcedure(filePath); 
        LoadObjects(filePath);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            GameObject button = GameObject.Find("Druckschalter_1_Knopf");
            button.GetComponent<CustomButton>().checkButton();
        }
        if (Input.GetKeyDown("2"))
        {
            GameObject button = GameObject.Find("Druckschalter_2_Knopf");
            button.GetComponent<CustomButton>().checkButton();
        }
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
            String[] data = File.ReadAllLines(objectfile);
            PersistentManager.Instance.procedure = data;
        }
        else
        {
            Debug.LogError("Did not load Procedure. File not found: " + objectfile);
        }
    }

}
