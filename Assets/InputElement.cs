using System;

public class InputElement : SequenceElement
{
    private String name;

    private bool counter = false;
    
    public InputElement(String name)
    {
        this.name = name;
    }
    
    bool checkIfCorrect(String input)
    {
        // Element bereits aktiviert
        if (counter) { return false; }
        
        if (name == input)
        {
            counter = true;
            return true;
        }
        return false;
    };
    
    
    bool counterReached()
    {
        return counter;
    };
}
