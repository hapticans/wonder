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
        if (Input.GetKeyDown("1")||Input.GetKeyDown("[1]"))
        {
			GameObject button = GameObject.Find("Konsole1_Rot_Button");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("2")||Input.GetKeyDown("[2]"))
        {
			GameObject button = GameObject.Find("Konsole1_Blue_Button");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("3")||Input.GetKeyDown("[3]"))
        {
			GameObject button = GameObject.Find("Konsole1_Gruen_Button");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("4")||Input.GetKeyDown("[4]"))
        {
			GameObject button = GameObject.Find("Konsole1_Gelb_Button");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("5")||Input.GetKeyDown("[5]"))
        {
			GameObject button = GameObject.Find("Konsole2_Rot_Button");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("6")||Input.GetKeyDown("[6]"))
        {
            GameObject button = GameObject.Find("Druckschalter_6_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("7")||Input.GetKeyDown("[7]"))
        {
            GameObject button = GameObject.Find("Druckschalter_7_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("8")||Input.GetKeyDown("[8]"))
        {
            GameObject button = GameObject.Find("Druckschalter_8_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
        if (Input.GetKeyDown("9")||Input.GetKeyDown("[9]"))
        {
            GameObject button = GameObject.Find("Druckschalter_9_Knopf");
            button.GetComponent<CustomButton>().checkButton();
            return;
        }
    }
}
