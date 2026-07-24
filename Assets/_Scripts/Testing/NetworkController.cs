using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkController : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            GameManager.Instance.ConnectAsHost();
            Destroy(this);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            GameManager.Instance.ConnectAsClient();
            Destroy(this);
        }
    }
}
