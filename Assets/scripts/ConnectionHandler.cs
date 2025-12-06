using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ConnectionHandler : MonoBehaviour
{
    public Button connectButton;
    public TextMeshProUGUI buttonText;

    public string initialtext = "Press to initialize connection";
    public string connectedtext = "Press to water the plant";
    
    public ESP32Connector esp32Connector;
    private Action defaultAction;
    private bool isConnected = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //check if components are assigned
        if (connectButton == null || buttonText == null || esp32Connector == null)
        {
            Debug.LogError("Fehlende Komponente. Stellen Sie sicher, dass Button, Text und VsArduinoSkript im Inspector zugewiesen sind.");
            enabled = false; 
            return;
        }
        UpdateButtonState();
    }

    // Update is called once per frame
    void Update()
    {
        if (esp32Connector != null)
        {
         isConnected = true;   
        }
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (isConnected)
        {
            if (buttonText.text != connectedtext) {
            buttonText.text = connectedtext;
            connectButton.onClick.RemoveAllListeners();
            connectButton.onClick.AddListener(() => esp32Connector.Send("WATER_PLANT"));
            }
            connectButton.interactable = true;
        }
        else
        {
            if (buttonText.text != initialtext)
            {
            buttonText.text = initialtext;
            connectButton.onClick.RemoveAllListeners();
            connectButton.onClick.AddListener(() => esp32Connector.Connect());
            }
            connectButton.interactable = true;
        }
    }
}
