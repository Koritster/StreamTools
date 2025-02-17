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
        twitch.OnChatMessage.AddListener(OnChatMessage);
    }

    public void OnChatMessage(string user, string message)
    {
        //Detectar comando del array
        string command = null;

        foreach(string com in commands)
        {
            Debug.Log($"{message.ToLower()} comparing with {com.ToLower()}");
            if (message.ToLower().Contains(com.ToLower()))
            {
                command = com;
                break;
            }
        }

        if (command == null)
            return;

        message = message.Substring(message.IndexOf(command) + command.Length).Trim();

        Debug.Log(message);

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

        try
        {
            //Cambia el modelo del avatar
            foreach (AvatarCharacter avatar in avatarSpawner.avatarCharacters)
            {
                if (message.ToLower().Contains(avatar.name.ToLower()))
                {
                    avatarSpawner.usersWithAvatar[user].ChangeAvatar(avatar);
                }
            }
        }
        catch (System.Exception ex)
        {
            twitch.SendTwitchMessage("Hey! Relaja la raja, primero debes tener un avatar activo");
        }
    }
}
