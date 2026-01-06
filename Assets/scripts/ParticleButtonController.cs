using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ParticleButtonController : MonoBehaviour
{
    // References of gameObjects and components
    public ParticleSystem waterParcticels;
    public GameObject buttonObject; 
    public COMCommunication comCommunication;
    public ESP32Connector esp32Connector;

    private bool isPlaying = false;

    public void OnButtonPressed()
    {
        if (isPlaying) return; // avoids restart of particles if already playing

        isPlaying = true;
        
        waterParcticels.gameObject.SetActive(true);
        comCommunication.Send("WATER_PLANT");
        esp32Connector.Send("WATER_PLANT");
        
        waterParcticels.Play();
        
        
        buttonObject.GetComponent<Button>().interactable = false;


        // Start Coroutine, wait for particles to finish
        // Thread.Sleep(2000);
        StartCoroutine(WaitForParticles());
        
    }

    private System.Collections.IEnumerator WaitForParticles()
    {
        // wait until all particles are dead
        while (waterParcticels.IsAlive(true))
        {
            yield return null;
        }
        // Particles finished
        isPlaying = false;

        // reactivate button
        // if(buttonObject != null) buttonObject.SetActive(true);
        buttonObject.GetComponent<Button>().interactable = true;

        // stop and clear particles
        waterParcticels.Clear(); // delete existing particles
        waterParcticels.Stop();  // ensure particle system is stopped
    }
}
