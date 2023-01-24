using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuPlayerData : MonoBehaviour
{
    public static MenuPlayerData instance;

    private string _playerName;
   [SerializeField] private TMP_InputField _inputField;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SetPlayerName(string name)
    {
        _playerName = _inputField.text;
    }

    public string GetPlayerName()
    {
        return _playerName;
    }
}
