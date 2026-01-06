using UnityEngine;

public class PlantButton : MonoBehaviour
{
    [Header("WHAT DO I OPEN?")]
    public GameObject plantCard; // Drag the specific plant card here (e.g. RoseCard)

    [Header("WHAT DO I CLOSE?")]
    public GameObject libraryMenu; // Drag the menu with all the buttons here

    // This is the function we will connect to the button click
    public void ShowCard()
    {
        // 1. Enable the plant card
        if (plantCard != null)
        {
            plantCard.SetActive(true);
        }

        // 2. Disable the library menu (so it doesn't cover the card)
        if (libraryMenu != null)
        {
            libraryMenu.SetActive(false);
        }
    }
}
