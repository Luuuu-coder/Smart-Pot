using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.IO.Ports;

public class Status : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM19", 9600);

    public GameObject highWaterLevel;      // activated when Water detected
    public GameObject mediumWaterLevel;
    public GameObject lowWaterLevel;        // activated when no water detected
    public Color materialActivated;
    public Color materialDeactivated;

    void Start()
    {
        try {
            Debug.Log(Application.platform);
            sp.Open();
            sp.ReadTimeout = 1;
            Debug.Log("Serial Port open.");
        }
        catch
        {
            Debug.Log("Error while opening Port");
        }
    }

    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                string input = sp.ReadLine();
                int value;
                if (int.TryParse(input.Trim(), out value))
                {
                    UpdateWaterStatus(value);
                }
            }
            catch (TimeoutException)
            {
            }
    }
    void UpdateWaterStatus(int dir)
    {
        if (dir == 0)
        {
            Debug.Log("Water high");
            //if (highWaterLevel != null) highWaterLevel.SetActive(true);
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(true);
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;


            }
            else if (dir == 1)
                {
                Debug.Log("Water medium");
                //if (highWaterLevel != null) highWaterLevel.SetActive(false);
                highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
                //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(true);
                mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
                //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
                lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;

                }
            else
            {
                Debug.Log("Water low");
                //if (highWaterLevel != null) highWaterLevel.SetActive(false);
                highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
                //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(false);
                mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
                //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
                lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            }
        }
    }
}