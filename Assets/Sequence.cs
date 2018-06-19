using System;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : SequenceElement
{
    // Anzahl der in dieser Sequenz aktivierenden Elemente
    private int targetInteractionCount;

    // Runner durch element Array 
    private int runner = 0;
    
    // Anzahl der bereits zurückgegebenen Elemente
    private int returncounter = 0;
    
    // Bool ob Reihenfolge eingehalten werden muss
    private bool order = false;

    // Elemente dieser Sequenz
    private SequenceElement[] elements;


    public Sequence(int counter, bool order, SequenceElement[] elements)
    {
        this.targetInteractionCount = counter;
        this.order = order;
        this.elements = elements;
    }

    public bool checkIfCorrect(String name)
    {
        //Debug.Log(elements.Length);
        //Debug.Log("Returncounter" + returncounter);
        // Sequenz abgearbeitet
        if (counterReached()) { return false; }

        bool returnValue = false;
        if (order)
        {
            // Check if Element is already finished
            if (elements[runner].counterReached())
            {
                // Increment runner to check next element
                runner++;
            }
            returnValue = elements[runner].checkIfCorrect(name);
        } else
        {
            for (int i = 0; i < elements.GetLength(0); i++)
            {
                returnValue = elements[i].checkIfCorrect(name);
                
                // Already found fitting Element
                if (returnValue)
                {
                    break;
                }
            }         
        }
        // Increment Return Counter when true was returned AND subsequence is fully satisfied
        if (returnValue && elements[runner].counterReached())
        {
            returncounter++;
        }
        return returnValue;
    }

    public bool counterReached()
    {
        return returncounter >= targetInteractionCount; 
    }
    
}