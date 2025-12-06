using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
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

    private NetworkStream stream;
    private TcpClient client;

    int dryValue = 500;
    int wetValue = 200;

    // Datei: Assets/Scripts/WaterRequirement.cs
    public enum WaterRequirement
    {
        High,   // 0
        Medium, // 1
        Low     // 2
    }


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
                byte[] buffer = new byte[256];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string msg = Encoding.ASCII.GetString(buffer, 0, bytes);

                // Jede Zeile separat auswerten
                string[] lines = msg.Split('\n');
                foreach (string line in lines)
                {
                    if (line.StartsWith("LightValue:"))
                    {
                        int value = int.Parse(line.Split(':')[1]);
                        UpdateWaterStatus(value);
                    }
                    else if (line.StartsWith("SoilValue:"))
                    {
                        int value = int.Parse(line.Split(':')[1]);
                        // UpdateLightStatus(value);
                    }
                }
            }
            catch (TimeoutException)
            {
            }
    }

    void UpdateWaterStatus(int dir)
    {
            
            int intervals = (dryValue - wetValue) / 3;


            if (dir > dryValue && dir < (wetValue + intervals))            
            {
            //Debug.Log("Water high");
            //if (highWaterLevel != null) highWaterLevel.SetActive(true);
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(true);
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;


            }
            else if (dir >= (wetValue + intervals) && dir < (dryValue - intervals))
            {
                //Debug.Log("Water medium");
                //if (highWaterLevel != null) highWaterLevel.SetActive(false);
                highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
                //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(true);
                mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
                //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
                lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;

            }
            else if(dir < dryValue && dir > (dryValue - intervals))
            {
                //Debug.Log("Water low");
                //if (highWaterLevel != null) highWaterLevel.SetActive(false);
                highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
                //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(false);
                mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
                //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
                lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            }
            else
            {
                Debug.Log("Message recives:" + dir);
            }
        }
    }

    private Dictionary<WaterRequirement, int> wetValues = new Dictionary<WaterRequirement, int>()
    {
        { WaterRequirement.High, 200 },
        { WaterRequirement.Medium, 350 },
        { WaterRequirement.Low, 500 }
    };

    public void SetWaterRequirement(WaterRequirement requirement)
    {
        if (wetValues.TryGetValue(requirement, out int value))
        {
            wetValue = value;
        }
        else
        {
            Debug.LogWarning("Ung�ltige Anforderung!");
        }
    }
    // Wrapper-Methoden f�r Buttons
    public void SetHighWater() => SetWaterRequirement(WaterRequirement.High);
    public void SetMediumWater() => SetWaterRequirement(WaterRequirement.Medium);
    public void SetLowWater() => SetWaterRequirement(WaterRequirement.Low);


    public void SendToArduino(string message)
    {
        if (sp != null && sp.IsOpen)
        {
            Debug.Log("Befehl gesendet");
            sp.WriteLine(message);
        }
    }
}