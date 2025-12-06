using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class ESP32Connector : MonoBehaviour
{
    public GameObject highWaterLevel;      // activated when Water detected
    public GameObject mediumWaterLevel;
    public GameObject lowWaterLevel;        // activated when no water detected
    public GameObject lightIndicator;
    public Color materialActivated;
    public Color materialDeactivated;
    public Color materialdark;
    public Color materialbright;
    public Color materialverybright;

    private TcpClient client;
    private NetworkStream stream;
    public bool IsConnected {get; private set;} = false;

    public string ip = "10.91.17.12"; // <- Replace with ESP IP
    public int port = 3333;
    public GameObject connectButton;
    int dryValue =2960;
    int wetValue =1300;
    int brightValue = 1500;
    int darkValue = 200;

    //connect this funcuin to a button to initialize connection
    public void Connect()
    {
        if (IsConnected) return;
        try
        {
            client = new TcpClient();
            client.Connect(ip, port);
            stream = client.GetStream();
            stream.ReadTimeout = 1000;
            IsConnected = true;
            Debug.Log("Connected to ESP32");
        }
        catch (System.Exception e)
        {
            IsConnected = false;
            Debug.Log("Connection failed: " + e.Message);
        }
    }

    // Function to send a Message to ESP32 hist call in Unity handler:
    //FindObjectOfType<ESP32Connector>().Send("LED_ON");
    // or call
    //Send("VALUE:123");

    public void Send(string text)
    {
        if (stream == null) return;
        byte[] data = Encoding.ASCII.GetBytes(text + "\n");
        stream.Write(data, 0, data.Length);
    }

    // reciving message from ESP32
    void Update()
    {
        if (stream != null && stream.DataAvailable)
        {
            try{
                connectButton.SetActive(false);
                byte[] buffer = new byte[256];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string msg = Encoding.ASCII.GetString(buffer, 0, bytes);

                Debug.Log("ESP32: " + msg);
                // put programm as in other scripts to handle the received message
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
                            UpdateLightStatus(value);
                        }
                    }
            }
            catch (System.Exception){
                Debug.Log("Error while reading message");// Timeout or read error
            }
        } 
        else {
            connectButton.SetActive(true);
        }
    }

    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
        IsConnected = false;
        connectButton.SetActive(true);
    }
    void UpdateLightStatus(int dir)
    {
        // Implement light status update logic here
        int intervals = (brightValue - darkValue) / 3;

        if (dir <= darkValue)
    {
        Debug.Log("Level 0 very dark");
        lightIndicator.GetComponent<SpriteRenderer>().color = materialdark;
        materialdark.a = 0.5f;
    }
    else if (dir <= darkValue + intervals)
    {
        Debug.Log("Level 1 dark");
    }
    else if (dir <= darkValue + intervals * 2)
    {
        Debug.Log("Level 2 medium light");
    }
    else if (dir <= darkValue + intervals * 3)
    {
        Debug.Log("Level 3 bright");
    }
    else if (dir <= brightValue)
    {
        Debug.Log("Level 4 very bright");
    }
    else
    {
        Debug.Log("Wert über brightValue – Fehler?");
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
            Debug.Log("Unknown water status:" + dir);
        }    
    }
    //change max wet value according to plant requirement
    public void SetWaterRequirement(WaterRequirement requirement){
        if (wetValues.TryGetValue(requirement, out int value))
        {
            wetValue = value;
        }
        else
        {
            Debug.LogWarning("Failed to update water need!");
        }
    }
    public void SetHighWater() => SetWaterRequirement(WaterRequirement.High);
    public void SetMediumWater() => SetWaterRequirement(WaterRequirement.Medium);
    public void SetLowWater() => SetWaterRequirement(WaterRequirement.Low);
    public enum WaterRequirement
    {
        High,   // 0
        Medium, // 1
        Low     // 2
    }
    private Dictionary<WaterRequirement, int> wetValues = new Dictionary<WaterRequirement, int>()
    {
        { WaterRequirement.High, 1300 },
        { WaterRequirement.Medium, 1700 },
        { WaterRequirement.Low, 2100 }
    };
}
