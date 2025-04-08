using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckKoritos : MonoBehaviour
{
    [SerializeField] private string[] commands;

    private AvatarSpawner avatarSpawner;
    private TwitchConnect twitch;

    private void Start()
    {
        avatarSpawner = GetComponent<AvatarSpawner>();
        twitch = GameObject.FindWithTag("GameController").GetComponent<TwitchConnect>();
        twitch.OnChatMessage.AddListener(OnChatMessage);
    }

    public void OnChatMessage(string user, string message)
    {
        //Detectar comando del array
        string command = null;

        foreach (string com in commands)
        {
            //Debug.Log($"{message.ToLower()} comparing with {com.ToLower()}");
            if (message.ToLower().Contains(com.ToLower()))
            {
                command = com;
                break;
            }
        }

        if (command == null)
        {
            Debug.Log("El comando fue nulo");
            return;
        }

        //Mostrar los koritos del jugador
        DatabaseManager.Instance.PrintKoritos(user);
    }
}
