using UnityEngine;
using UnityEngine.InputSystem; // Fondamentale

public class GestoreMenu : MonoBehaviour
{
    public GameObject ilMioMenu;

    // Questa variabile magica ci permette di scegliere i tasti dall'Inspector
    public InputAction tastoMenu;

    private void OnEnable()
    {
        tastoMenu.Enable(); // Attiva l'ascolto del tasto
    }

    private void OnDisable()
    {
        tastoMenu.Disable(); // Disattiva l'ascolto
    }

    void Update()
    {
        // Se il tasto configurato è stato premuto...
        if (tastoMenu.WasPressedThisFrame())
        {
            if (ilMioMenu != null)
            {
                bool stato = ilMioMenu.activeSelf;
                ilMioMenu.SetActive(!stato);
            }
        }
    }
}
