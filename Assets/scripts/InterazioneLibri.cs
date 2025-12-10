using UnityEngine;

public class InterazioneLibri : MonoBehaviour
{
    [Header("Impostazioni Menu")]
    // Qui trascinerai il tuo menu (il GameObject che contiene Canvas/Pannello)
    public GameObject menuDaAprire;

    // Questa funzione viene chiamata automaticamente da Unity quando clicchi l'oggetto
    private void OnMouseDown()
    {
        // Controlla se abbiamo assegnato il menu per evitare errori
        if (menuDaAprire != null)
        {
            // Attiva il menu
            menuDaAprire.SetActive(true);
            
            // Opzionale: Se vuoi sbloccare il mouse per cliccare nel menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("Menu aperto!");
        }
        else
        {
            Debug.LogError("Non hai assegnato il Menu nello script!");
        }
    }
}