using UnityEngine;

public class WaterController : MonoBehaviour
{
    // Qui trascineremo il nostro effetto "Cascata"
    public ParticleSystem waterEffect;

    // Questa è la funzione che chiamerà il pulsante
    public void ToggleWater()
    {
        // Se l'acqua sta scendendo, la fermiamo
        if (waterEffect.isPlaying)
        {
            waterEffect.Stop();
        }
        // Altrimenti, la facciamo partire
        else
        {
            waterEffect.Play();
        }
    }
}