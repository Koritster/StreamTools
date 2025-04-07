using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAvatarByName : MonoBehaviour
{
    [SerializeField] private string[] commands;

    private AvatarSpawner avatarSpawner;
    private TwitchConnect twitch;

    private void Start()
    {
        avatarSpawner = GetComponent<AvatarSpawner>();
        twitch = GameObject.FindWithTag("GameController").GetComponent<TwitchConnect>();
        //twitch.OnChatMessage.AddListener(OnChatMessage);
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

        message = message.Substring(message.IndexOf(command) + command.Length).Trim();
        //Debug.LogWarning("Sigue funcando este pdo");

        //Si el comando no tiene parametros, muestra una lista de los avatares disponibles
        if(message == "")
        {
            string msg = "Avatares disponibles: ";
            
            foreach(AvatarCharacter avatar in avatarSpawner.avatarCharacters)
            {
                //if(avatarSpawner.avatarCategory == "any" || avatar.category == avatarSpawner.avatarCategory)                    
                msg += avatar.name;
                //Detecta si no es el último valor
                if(!(avatar.name == avatarSpawner.avatarCharacters[avatarSpawner.avatarCharacters.Length - 1].name))
                {
                    msg += ", ";
                }
            }

            twitch.SendTwitchMessage(msg);
        }

        Debug.LogWarning($"Buscando el avatar {message}");

        bool avatarFound = false;

        //Cambia el modelo del avatar
        foreach (AvatarCharacter avatar in avatarSpawner.avatarCharacters)
        {
            if (message.ToLower().Contains(avatar.name.ToLower()))
            {
                DatabaseManager.Instance.UpdateAvatar(user, avatar.name);
                avatarFound = true;
                break;
            }
        }

        if (!avatarFound && message != "")
        {
            twitch.SendTwitchMessage("No se encontró un avatar con ese nombre");
        }
    }
}
