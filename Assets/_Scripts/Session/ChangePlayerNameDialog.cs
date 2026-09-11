using TMPro;
using UnityEngine;

public class ChangePlayerNameDialog : MonoBehaviour
{
    [SerializeField] private TMP_InputField _playerNameInput;

    public async void UpdatePlayerName()
    {
        if (string.IsNullOrEmpty(_playerNameInput.text))
        {
            Debug.LogWarning("Please enter a name.");
            return;
        }
        await SessionManager.Instance.UpdatePlayerName(_playerNameInput.text);
        _playerNameInput.text = "";
    }
}
