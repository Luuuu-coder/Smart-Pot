using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public GameObject[] plants;

    public void ActivatePlant(int index)
    {
        // deactivate all plants
        for (int i = 0; i < plants.Length; i++)
        {
            plants[i].SetActive(i == index);
        }
    }
}