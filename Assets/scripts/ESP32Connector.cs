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
    public GameObject connectButton;
    public Color materialActivated;
    public Color materialDeactivated;
    public Color materialdark;
    public Color materialdim;
    public Color materialbright;
    public Color materialverybright;
    private TcpClient client;
    private NetworkStream stream;
    public bool IsConnected {get; private set;} = false;

    public string ip = "10.91.17.12"; // <- Replace with ESP IP
    public int port = 3333;
    // public string brokerIp = "192.168.4.1";
    // public int brokerPort = 1883;
    
    int dryValue =3005;
    int wetValue =2000;
    int brightValue = 1500;
    int darkValue = 200;

    // JSON-Serialisierung/Deserialisierung
    [System.Serializable]
    private class SensorData
    {
        public int Moisture;
        public int Light;
    }
    
    [System.Serializable]
    private class CommandMessage
    {
        public string command;
    }

    //connect this function to a button to initialize connection
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

    public void Send(string text)
    {
        if (!IsConnected || stream == null) return;
        
        try
        {
            CommandMessage cmd = new CommandMessage { command = text};
            string json = JsonUtility.ToJson(cmd);
            Debug.Log("Prepared command: " + json);
            // JSON als String mit Zeilenumbruch senden
            byte[] data = Encoding.ASCII.GetBytes(json + "\n"); 
            stream.Write(data, 0, data.Length);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Write error: " + e.Message);
        }
    }

    // reciving message from ESP32
    void Update()
    {
        if (!IsConnected) {
            // connectButton.SetActive(true);
            return;
        }

        // --- RECIVE and PARS DATA ---
        if (stream != null && stream.DataAvailable)
        {
            // connectButton.SetActive(false); 
            try
            {
                byte[] buffer = new byte[1024]; // Puffergröße erhöht
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string msg = Encoding.ASCII.GetString(buffer, 0, bytes);

                // split lines
                string[] lines = msg.Split('\n'); 
                foreach (string line in lines)
                {   
                    string trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine)) continue;
                    
                    Debug.Log("Raw received line: " + line);
                    if(line.StartsWith("ACK:")) {
                        continue; // ignore ACK Message
                    }
                    // try to JSON parse
                    SensorData data = JsonUtility.FromJson<SensorData>(trimmedLine);
                    Debug.Log("Received values: " + data.Moisture + ", " + data.Light);
                    // check if parsing was sucsessfull (expected 'Moisture' und 'Light')
                    if (data != null && data.Moisture != 0 || data.Light != 0) 
                    {
                        // use parsed data
                        UpdateWaterStatus(data.Moisture);
                        UpdateLightStatus(data.Light);
                    }
                    else
                    {
                        // ignoring all data in the wrong format
                        Debug.LogWarning("Ignored non-JSON data: " + trimmedLine);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Read error: " + e.Message);
            }
        } 
        else {
            // Optional: if mo values are recived for a long time
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
            materialdark.a = 0.4f;
        }
        else if (dir <= darkValue + intervals)
        {
            Debug.Log("Level 1 dark");
            lightIndicator.GetComponent<SpriteRenderer>().color = materialdark;
            materialdark.a = 0.8f;
        }
        else if (dir <= darkValue + intervals * 2)
        {
            Debug.Log("Level 2 medium light");
            lightIndicator.GetComponent<SpriteRenderer>().color = materialdim;
            materialdark.a = 0.8f;
        }
        else if (dir <= darkValue + intervals * 3)
        {
            Debug.Log("Level 3 bright");
            lightIndicator.GetComponent<SpriteRenderer>().color = materialbright;
            materialdark.a = 0.8f;
        }
        else
        {
            Debug.Log("Level 4 very bright");
            lightIndicator.GetComponent<SpriteRenderer>().color = materialverybright;
            materialdark.a = 0.8f;
        }
    }

    void UpdateWaterStatus(int dir)
    {
        int intervals = (dryValue - wetValue) / 3;   
        if (dir < wetValue)            
        {
            Debug.Log("Water level very high");
            //if (highWaterLevel != null) highWaterLevel.SetActive(true);
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(true);
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
        }
        else if (dir < wetValue + intervals)
        {
            Debug.Log("Water level high");
            //if (highWaterLevel != null) highWaterLevel.SetActive(false);
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(true);
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;

        }
        else if(dir < wetValue + intervals * 2)
        {
            Debug.Log("Water level medium");
            //if (highWaterLevel != null) highWaterLevel.SetActive(false);
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(false);
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            //if (lowWaterLevel != null) lowWaterLevel.SetActive(true);
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
        }
        else
        {
            Debug.Log("Water level low");
            //if (highWaterLevel != null) highWaterLevel.SetActive(false);
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            //if (mediumWaterLevel != null) mediumWaterLevel.SetActive(false);
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            //if (lowWaterLevel != null) lowWaterLevel.SetActive(false);
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
        }    
    }
    //change max wet value according to plant requirement
    public void SetWaterRequirement(WaterRequirement requirement){
        if (wetValues.TryGetValue(requirement, out int value))
        {
            wetValue = value;
            Debug.Log("Water need updated to: " + requirement.ToString() + " (" + value + ")");
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
