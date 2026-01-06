using UnityEngine;

public class InterazioneLibri : MonoBehaviour
{
    [Header("Menu Settings")]
    // Drag your menu here (the GameObject that contains the Canvas/Panel)
    public GameObject menuDaAprire;

    // This function is automatically called by Unity when you click the object
    private void OnMouseDown()
    {
        // Check if the menu is assigned to avoid errors
        if (menuDaAprire != null)
        {
            // Enable the menu
            menuDaAprire.SetActive(true);
            
            //  Optional: Unlock the mouse so you can click in the menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("Menu opened!");
        }
        else
        {
            Debug.LogError("You haven't assigned the Menu in the script!");
        }
    }
}