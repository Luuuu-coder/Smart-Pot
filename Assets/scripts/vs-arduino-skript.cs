using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.IO.Ports;

public class Status : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM21", 9600);

    public GameObject highWaterLevel;      // Wird aktiviert, wenn Wasser erkannt wird
    public GameObject mediumWaterLevel;
    public GameObject lowWaterLevel;        // Wird aktiviert, wenn kein Wasser erkannt wird
    public Material materialActivated;
    public Material materialDeactivated;

    void Start()
    {
        try {
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
            highWaterLevel.GetComponent<Renderer>().material = materialActivated;
            //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(true);
            mediumWaterLevel.GetComponent<Renderer>().material = materialActivated;
            //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
            lowWaterLevel.GetComponent<Renderer>().material = materialActivated;


            }
            else if (dir == 1)
                {
                Debug.Log("Water medium");
                //if (highWaterLevel != null) highWaterLevel.SetActive(false);
                highWaterLevel.GetComponent<Renderer>().material = materialDeactivated;
                //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(true);
                mediumWaterLevel.GetComponent<Renderer>().material = materialActivated;
                //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
                lowWaterLevel.GetComponent<Renderer>().material = materialActivated;

                }
            else
            {
                Debug.Log("Water low");
                //if (highWaterLevel != null) highWaterLevel.SetActive(false);
                highWaterLevel.GetComponent<Renderer>().material = materialDeactivated;
                //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(false);
                mediumWaterLevel.GetComponent<Renderer>().material = materialDeactivated;
                //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
                lowWaterLevel.GetComponent<Renderer>().material = materialActivated;
            }
        }
    }
}