using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkController : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            NetworkManager.Singleton.StartHost();
            print("Starting Host...");
            Destroy(this);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            NetworkManager.Singleton.StartClient();
            print("Starting Client...");
            Destroy(this);
        }
    }
}
