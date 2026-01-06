using UnityEngine;
using UnityEngine.InputSystem; 

public class GestoreMenu : MonoBehaviour
{
    public GameObject myMenu;

    // This variable lets us choose the key from the Inspector
    public InputAction manuKey;

    private void OnEnable()
    {
        manuKey.Enable(); // Enable key listening
    }

    private void OnDisable()
    {
        manuKey.Disable(); // Disable key listening
    }

    void Update()
    {
        // If the configured key was pressed...
        if (manuKey.WasPressedThisFrame())
        {
            if (myMenu != null)
            {
                bool stato = myMenu.activeSelf;
                myMenu.SetActive(!stato);
            }
        }
    }
}
