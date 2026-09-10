using TMPro;
using UnityEngine;

public class JoinLobbyDialog : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;

    public void JoinLobby()
    {
        if (string.IsNullOrEmpty(_inputField.text))
        {
            Debug.LogWarning("Please enter a code in the input field.");
            return;
        }

        SessionManager.Instance.JoinSessionByCode(_inputField.text);
        _inputField.text = "";
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        _inputField.text = "";
        gameObject.SetActive(false);
    }
}