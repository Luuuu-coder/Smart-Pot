using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public GameObject menuDaAprire;
    public InputActionProperty tastoAttivazione;

    // --- NUOVA PARTE FONDAMENTALE ---
    // Appena l'oggetto si attiva, accendiamo l'ascolto del tasto
    void OnEnable()
    {
        tastoAttivazione.action.Enable();
    }

    // Appena l'oggetto si disattiva, spegniamo l'ascolto (per pulizia)
    void OnDisable()
    {
        tastoAttivazione.action.Disable();
    }
    // --------------------------------

    void Update()
    {
        // Ora il tasto dovrebbe rispondere
        if (tastoAttivazione.action.WasPressedThisFrame())
        {
            //Debug.Log("TASTO PREMUTO CORRETTAMENTE!"); // Lasciamo il debug per sicurezza
           
            if (menuDaAprire != null)
            {
                bool statoAttuale = menuDaAprire.activeSelf;
                menuDaAprire.SetActive(!statoAttuale);
            }
        }
    }
}