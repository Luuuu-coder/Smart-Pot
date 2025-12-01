using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public GameObject[] plants;

    public void ActivatePlant(int index)
    {
        // Disattiva tutte le piante
        for (int i = 0; i < plants.Length; i++)
        {
            plants[i].SetActive(i == index);
        }
    }
}