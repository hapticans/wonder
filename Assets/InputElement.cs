using System;
using System.Collections.Generic;
using UnityEngine;

public class InputElement : SequenceElement
{
    private String name;

    private bool counter = false;
    
    public InputElement(String name)
    {
        this.name = name;
    }

    public bool checkIfCorrect(String input)
    {
        Debug.Log("Checking Element" + this.name + " against" + input);
        // Element bereits aktiviert
        if (counter) { return false; }
        
        if (name == input)
        {
            counter = true;
            return true;
        }
        return false;
    }
    
    
    public bool counterReached()
    {
        return counter;
    }
}
