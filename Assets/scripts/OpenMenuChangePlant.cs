using UnityEngine;

public class ApriMenu : MonoBehaviour
{
    public GameObject menuToOpen;

    public void Apri()
    {
        if (menuToOpen != null)
        {
            menuToOpen.SetActive(true);
            Debug.Log("Menu avtivated!");
        }
        else
        {
            Debug.Log("<menu not assigned>!");
        }
    }
}