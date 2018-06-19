using System;
using System.Collections.Generic;
using UnityEngine;

public class DevelopHelper
{
	public DevelopHelper()
	{
	}

    public void registerKeyBindings()
    {
        if (Input.GetKeyDown("1"))
        {
            GameObject button = GameObject.Find("Druckschalter_1_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("2"))
        {
            GameObject button = GameObject.Find("Druckschalter_2_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("3"))
        {
            GameObject button = GameObject.Find("Druckschalter_3_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("4"))
        {
            GameObject button = GameObject.Find("Druckschalter_4_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("5"))
        {
            GameObject button = GameObject.Find("Druckschalter_5_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("6"))
        {
            GameObject button = GameObject.Find("Druckschalter_6_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("7"))
        {
            GameObject button = GameObject.Find("Druckschalter_7_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("8"))
        {
            GameObject button = GameObject.Find("Druckschalter_8_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("9"))
        {
            GameObject button = GameObject.Find("Druckschalter_9_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
    }
}
