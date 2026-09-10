using TMPro;
using UnityEngine;

public class LobbyPlayerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;

    public void Initialize(string name)
    {
        _nameLabel.text = name;
    }
}
