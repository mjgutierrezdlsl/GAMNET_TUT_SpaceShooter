using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    [SerializeField] private bool _isPersistent = false;
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this as T;
            if(_isPersistent)DontDestroyOnLoad(gameObject);
        }
    }
}