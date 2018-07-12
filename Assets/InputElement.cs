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

	public bool checkIfCorrect(String input, bool markAsDone)
    {
		// Element bereits aktiviert
		if (counter) { return false; }

		if (name == input)
		{
			if (markAsDone) {
				counter = true;
			}
			return true;
		}
		return false;
    }
    
    public bool counterReached()
    {
        return counter;
    }

	public int GetTargetInteractionCount() {
		return 1;
	}
}
