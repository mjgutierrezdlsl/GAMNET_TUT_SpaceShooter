using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    private TextMeshProUGUI _label;

    private void Awake()
    {
        _label = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetText(ulong id, int score)
    {
        _label.text = $"Player {id}: {score:000}";
    }
}
