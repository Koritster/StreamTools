using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ShowMessage : MonoBehaviour
{
    private Text txt_message;
    private RectTransform bg_text;
    private AvatarSpawner avatarSpawner;

    private void Start()
    {
        avatarSpawner = GetComponent<AvatarSpawner>();
    }

    //Evento llamado desde TwitchConnect (Editor). Muestra el mensaje de un usuario en su avatar
    public void OnChatMessage(string user, string message)
    {
        if (message.Contains("!"))
        {
            return; 
        }

        foreach(string list in avatarSpawner.bannedUsers)
        {
            if(user.Contains(list.ToLower()))
            {
                return;
            }
        }
        
        //Asignación de variables
        avatarSpawner.usersWithAvatar[user].ShowMessage(message);
        bg_text = avatarSpawner.usersWithAvatar[user].go_textBox.GetComponent<RectTransform>();
        txt_message = avatarSpawner.usersWithAvatar[user].txt_message;

        //Ajuste del tamaño del TextBox
        float preferredHeight = txt_message.preferredHeight;
        bg_text.sizeDelta = new Vector2(bg_text.sizeDelta.x, preferredHeight);
    }
}
