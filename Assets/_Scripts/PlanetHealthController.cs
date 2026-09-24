using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlanetHealthController : NetworkBehaviour
{
    [SerializeField] private Image _healthFillImage;

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.RUNNING) return;
        UpdateHealthRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateHealthRpc()
    {
        _healthFillImage.fillAmount = (float)Planet.Instance.Health.Value / Planet.Instance.MaxHealth;
    }

}
