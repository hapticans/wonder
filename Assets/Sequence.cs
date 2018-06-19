using System;


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
    private SequenceElement elements;


    public Sequence(int counter, bool order, SequenceElement[] elements)
    {
        this.targetInteractionCount = counter;
        this.order = order;
        this.elements = elements;
    }
    
    bool checkIfCorrect(String name)
    {
        // Sequenz abgearbeitet
        if (counterReached()) { return false; }

        //
        if (order)
        {
            bool returnValue = false;
            // Check if Element is already finished
            if (elements[runner].counterReached())
            {
                // Increment runner to check next element
                runner++;
            }
            bool returnValue = elements[runner].checkIfCorrect(name);
            
            // Increment Return Value when true was returned
            if (returnValue)
            {
                returncounter++;
            }
            return returnValue;
        } else
        {
            bool returnValue = false;
            for (int i = 0; i < elements.GetLength(0); i++)
            {
                returnValue = elements[i].checkIfCorrect(name);
                
                // Already found fitting Element
                if (returnValue)
                {
                    returncounter++;
                    break;
                }
            }         
            return returnValue;
        }
    };

    bool counterReached()
    {
        return returncounter >= targetInteractionCount; 
    };
    
}