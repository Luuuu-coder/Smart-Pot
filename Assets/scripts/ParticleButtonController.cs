using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ParticleButtonController : MonoBehaviour
{
    public ParticleSystem waterParcticels;
    public GameObject buttonObject; // optional, um Button visuell zu deaktivieren/aktivieren^
    public COMCommunication comCommunication;
    public ESP32Connector esp32Connector;

    private bool isPlaying = false;

    public void OnButtonPressed()
    {
        if (isPlaying) return; // verhindert erneutes Starten während Partikel laufen

        isPlaying = true;
        
        waterParcticels.gameObject.SetActive(true);
        comCommunication.Send("WATER_PLANT");
        esp32Connector.Send("WATER_PLANT");
        
        waterParcticels.Play();
        
        // optional: Button deaktivieren, wenn du willst
        // if(buttonObject != null) buttonObject.SetActive(false);
        
        buttonObject.GetComponent<Button>().interactable = false;


        // Starte Coroutine, die wartet bis Partikel fertig sind
        // Thread.Sleep(2000);
        StartCoroutine(WaitForParticles());
        
    }

    private System.Collections.IEnumerator WaitForParticles()
    {
        // warte, bis Partikel fertig sind
        while (waterParcticels.IsAlive(true))
        {
            yield return null;
        }
        // Partikel sind durchgelaufen
        isPlaying = false;

        // Button wieder aktivieren
        // if(buttonObject != null) buttonObject.SetActive(true);
        buttonObject.GetComponent<Button>().interactable = true;

        // Partikel stoppen und resetten
        waterParcticels.Clear(); // löscht alte Partikel
        waterParcticels.Stop();  // sichert, dass es gestoppt ist
    }
}
