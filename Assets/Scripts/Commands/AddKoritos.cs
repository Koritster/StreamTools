using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AddKoritos : MonoBehaviour
{
    private TwitchConnect twitch;

    private void Start()
    {
        twitch = GameObject.FindWithTag("GameController").GetComponent<TwitchConnect>();
        twitch.OnChatMessage.AddListener(OnChatMessage);
    }

    public void OnChatMessage(string user, string message)
    {
        string command = "!addkoritos";

        if (message.ToLower().Contains(command))
        {
            message = message.Substring(message.IndexOf(command) + command.Length).Trim();

            Debug.Log(message);

            if (message == "")
                return;

            if (int.TryParse(message, out int value))
            {
                DatabaseManager.Instance.UpdateKoritos(user, value);
            }
            else {
                Debug.Log("No se pudo convertir");
            }
        }
    }
}
