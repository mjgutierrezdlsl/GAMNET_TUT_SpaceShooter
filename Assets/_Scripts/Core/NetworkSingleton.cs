using Unity.Netcode;
using UnityEngine;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
        }
    }
}