using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class LoadSceneScript : MonoBehaviour
{

    public string SceneConfigFolder = "Scene_1";
    

    // Use this for initialization
    void Start()
    {
        GameObject hand = GameObject.Find("Druckschalter (1)");
        Destroy(hand);
        LoadScene();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadScene()
    {
        string filePath = Application.dataPath + "/" + SceneConfigFolder + "/";

        LoadObjects(filePath);
    }

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
                }
            }
        } else
        {
            Debug.LogError("Did not load Objects. File not found: " + objectfile);
        }
    }

    IEnumerator Fade(float time)
    {
        yield return new WaitForSeconds(time);
    }

}
