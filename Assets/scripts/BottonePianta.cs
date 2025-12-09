using UnityEngine;

public class BottonePianta : MonoBehaviour
{
    [Header("COSA DEVO APRIRE?")]
    public GameObject schedaDellaPianta; // Qui trascinerai la scheda specifica (es. Scheda Rosa)

    [Header("COSA DEVO CHIUDERE?")]
    public GameObject menuLibreria;      // Qui trascinerai il menu con tutti i bottoni

    // Questa è la funzione che collegheremo al click
    public void MostraScheda()
    {
        // 1. Accendi la scheda della pianta
        if (schedaDellaPianta != null)
        {
            schedaDellaPianta.SetActive(true);
        }

        // 2. Spegni il menu della libreria (così non copre la scheda)
        if (menuLibreria != null)
        {
            menuLibreria.SetActive(false);
        }
    }
}