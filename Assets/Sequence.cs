﻿using System;
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

	public bool checkIfCorrect(String name, bool markAsDone)
	{
		// Sequenz abgearbeitet
		if (counterReached()) { return false; }
		bool returnValue = false;
		if (order && !counterReached())
		{
			// Check if Element is already finished
			if (elements[runner].counterReached())
			{
				// Increment runner to check next element
				runner++;
			}

			returnValue = elements[runner].checkIfCorrect(name, markAsDone);
		}
		else
			if (!order)
		{
			for (int i = 0; i < elements.GetLength(0); i++)
			{
				returnValue = elements[i].checkIfCorrect(name, markAsDone);

				// Already found fitting Element
				if (returnValue)
				{
					runner = i;
					break;
				}
			}
		}

		// Increment Return Counter when true was returned AND subsequence is fully satisfied
		if (returnValue && elements[runner].counterReached())
		{
				returncounter++;
        }

        // Sequenz abgearbeitet
        if (counterReached())
		{
			PersistentManager.Instance.isProcedureDone(true);
		}
		return returnValue;
	}

	public bool counterReached()
	{
		//Fängt Fehler beim erstellen von Szenen ab, sonst out of bounds
		if (runner >= this.elements.GetLength(0))
		{
			return true;
		}

		return returncounter >= targetInteractionCount;
	}

	public int GetTargetInteractionCount()
	{
		return targetInteractionCount;
	}
}