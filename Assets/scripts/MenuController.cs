using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public GameObject menuToOpen;
    public InputActionProperty activationKey;


    //  As soon as the object is enabled, we enable key listening
    void OnEnable()
    {
        activationKey.action.Enable();
    }

    // As soon as the object is disabled, we stop listening (for cleanup)
    void OnDisable()
    {
        activationKey.action.Disable();
    }
    // --------------------------------

    void Update()
    {
        // Now the key should respond
        if (activationKey.action.WasPressedThisFrame())
        {
            //Debug.Log("KEY PRESSED CORRECTLY!"); // Keep this for safety if needed
           
            if (menuToOpen != null)
            {
                bool currentState = menuToOpen.activeSelf;
                menuToOpen.SetActive(!currentState);
            }
        }
    }
}