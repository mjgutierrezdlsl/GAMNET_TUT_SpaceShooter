using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;

public class LobbyPlayerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameLabel;
    public TextMeshProUGUI NameLabel => _nameLabel;


    public void Initialize(string name)
    {
        _nameLabel.text = name;
    }

}
