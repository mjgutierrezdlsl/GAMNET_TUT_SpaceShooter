using TMPro;
using UnityEngine;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _greetingLabel;

    public void UpdateLabel(string playerName)
    {
        _greetingLabel.text = $"Hello, {playerName}!";
    }
}
