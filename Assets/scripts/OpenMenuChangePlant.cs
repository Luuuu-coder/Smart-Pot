using UnityEngine;

public class ApriMenu : MonoBehaviour
{
    public GameObject menuDaAprire;

    public void Apri()
    {
        if (menuDaAprire != null)
        {
            menuDaAprire.SetActive(true);
            Debug.Log("MenuOpzioni attivato!");
        }
        else
        {
            Debug.Log("menuDaAprire NON assegnato!");
        }
    }
}