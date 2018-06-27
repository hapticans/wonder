using System;
public interface SequenceElement
{
	bool checkIfCorrect(String name, bool markAsDone);

    bool counterReached();
}
