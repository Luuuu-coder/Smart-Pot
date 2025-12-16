using UnityEngine;
using System.IO.Ports;
using System.Text;
using System.Collections.Generic;

public class COMCommunication : MonoBehaviour
{
    public GameObject highWaterLevel;
    public GameObject mediumWaterLevel;
    public GameObject lowWaterLevel;
    public GameObject lightIndicator;
    public GameObject connectButton;

    public Color materialActivated;
    public Color materialDeactivated;
    public Color materialdark;
    public Color materialdim;
    public Color materialbright;
    public Color materialverybright;

    private SerialPort serial;
    public bool IsConnected { get; private set; } = false;

    [Header("Serial Settings")]
    public string comPort = "COM19";     // << ANPASSEN
    public int baudRate = 115200;

    int dryValue = 3000;
    int wetValue = 400;
    int brightValue = 1500;
    int darkValue = 200;

    [System.Serializable]
    private class SensorData
    {
        public int soilValue;
        public int lightValue;
    }

    [System.Serializable]
    private class CommandMessage
    {
        public string command;
    }

    // ---------- CONNECT ----------
    public void Connect()
    {
        if (IsConnected) return;

        try
        {
            serial = new SerialPort(comPort, baudRate);
            serial.ReadTimeout = 100;
            serial.NewLine = "\n";
            serial.Open();

            IsConnected = true;
            Debug.Log("Connected to ESP32 via COM");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial connection failed: " + e.Message);
            IsConnected = false;
        }
    }

    // ---------- SEND ----------
    public void Send(string text)
    {
        if (!IsConnected || serial == null || !serial.IsOpen) return;

        try
        {
            CommandMessage cmd = new CommandMessage { command = text };
            string json = JsonUtility.ToJson(cmd);

            serial.WriteLine(json); // \n automatisch
            Debug.Log("Sent: " + json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial write error: " + e.Message);
        }
    }

    // ---------- RECEIVE ----------
    void Update()
    {
        if (!IsConnected || serial == null || !serial.IsOpen) return;

        try
        {
            if (serial.BytesToRead > 0)
            {
                string line = serial.ReadLine().Trim();
                if (string.IsNullOrEmpty(line)) return;

                Debug.Log("Raw received: " + line);

                SensorData data = JsonUtility.FromJson<SensorData>(line);
                if (data != null)
                {
                    UpdateWaterStatus(data.soilValue);
                    UpdateLightStatus(data.lightValue);
                }
            }
        }
        catch (System.TimeoutException)
        {
            // normal, ignorieren
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial read error: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (serial != null && serial.IsOpen)
            serial.Close();

        IsConnected = false;
    }

    // ---------- UI LOGIC (unverändert) ----------
    void UpdateLightStatus(int dir)
    {
        int intervals = (brightValue - darkValue) / 3;

        if (dir <= darkValue)
            lightIndicator.GetComponent<SpriteRenderer>().color = materialdark;
        else if (dir <= darkValue + intervals)
            lightIndicator.GetComponent<SpriteRenderer>().color = materialdark;
        else if (dir <= darkValue + intervals * 2)
            lightIndicator.GetComponent<SpriteRenderer>().color = materialdim;
        else if (dir <= darkValue + intervals * 3)
            lightIndicator.GetComponent<SpriteRenderer>().color = materialbright;
        else
            lightIndicator.GetComponent<SpriteRenderer>().color = materialverybright;
    }

    void UpdateWaterStatus(int dir)
    {
        int intervals = (dryValue - wetValue) / 3;

        if (dir < wetValue)
        {
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
        }
        else if (dir < wetValue + intervals)
        {
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
        }
        else if (dir < wetValue + intervals * 2)
        {
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialActivated;
        }
        else
        {
            highWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            mediumWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
            lowWaterLevel.GetComponent<SpriteRenderer>().color = materialDeactivated;
        }
    }

    // ---------- WATER REQUIREMENT ----------
    public void SetHighWater() => SetWaterRequirement(WaterRequirement.High);
    public void SetMediumWater() => SetWaterRequirement(WaterRequirement.Medium);
    public void SetLowWater() => SetWaterRequirement(WaterRequirement.Low);

    void SetWaterRequirement(WaterRequirement requirement)
    {
        wetValue = wetValues[requirement];
        Debug.Log("Water need set to " + requirement);
    }

    public enum WaterRequirement { High, Medium, Low }

    private Dictionary<WaterRequirement, int> wetValues = new Dictionary<WaterRequirement, int>()
    {
        { WaterRequirement.High, 700 },
        { WaterRequirement.Medium, 1300 },
        { WaterRequirement.Low, 2000 }
    };
}
