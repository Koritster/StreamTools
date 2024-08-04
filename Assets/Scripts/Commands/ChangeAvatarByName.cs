using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAvatarByName : MonoBehaviour
{
    private AvatarSpawner avatarSpawner;
    private TwitchConnect twitch;

    private void Start()
    {
        avatarSpawner = GetComponent<AvatarSpawner>();
        twitch = GameObject.FindWithTag("GameController").GetComponent<TwitchConnect>();
    }

    public void OnChatMessage(string user, string message)
    {
        if (message.Contains("!avatar"))
        {
            message = message.Substring(7);

            //Si el comando no tiene parametros, muestra una lista de los avatares disponibles
            if(message == "")
            {
                string msg = "Avatares disponibles: ";
                
                foreach(AvatarCharacter avatar in avatarSpawner.avatarCharacters)
                {
                    if(avatarSpawner.avatarCategory == "any" || avatar.category == avatarSpawner.avatarCategory)
                    {
                        msg += avatar.name;
                        //Detecta si no es el último valor
                        if(!(avatar.name == avatarSpawner.avatarCharacters[avatarSpawner.avatarCharacters.Length - 1].name))
                        {
                            msg += ", ";
                        }
                    }
                }

                twitch.SendTwitchMessage(msg);
            }

            try
            {
                //Cambia el modelo del avatar
                foreach (AvatarCharacter avatar in avatarSpawner.avatarCharacters)
                {
                    if (message.Contains(avatar.name.ToLower()))
                    {
                        if (avatarSpawner.avatarCategory == "any" || avatar.category == avatarSpawner.avatarCategory)
                        {
                            avatarSpawner.usersWithAvatar[user].ChangeAvatar(avatar);
                            break;
                        }
                        else
                        {
                            twitch.SendTwitchMessage("No puedes usar ese avatar");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                twitch.SendTwitchMessage("Hey! Relaja la raja, primero debes tener un avatar activo");
            }
        }
    }
}
