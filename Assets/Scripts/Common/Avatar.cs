using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Avatar : MonoBehaviour
{
    private void Start()
    {
        timerAvatar = 0f;
    }

    #region UI

    //UI variables
    public string user;
    public GameObject go_textBox;
    public Text txt_message;

    [SerializeField] private Text txt_user;

    //Cambiar el nombre mostrado del usuario. Esta función se llama desde el AvatarSpawner.cs
    public void ChangeName(string userName)
    {
        txt_user.text = userName;
        user = userName;
    }

    //Muestra el mensaje que los usuarios escriban. Esta función se llama desde el ShowMessage.cs
    public void ShowMessage(string message)
    {
        go_textBox.SetActive(true);
        txt_message.text = message;

        //Reinicia los timers
        timerMessages = 0f;
        timerAvatar = 0f;
        active = true;
    }

    //Oculta el mensaje después de un periodo de tiempo. Se llama en la función Update, sección Timer Messages
    public void HideMessage()
    {
        go_textBox.SetActive(false);
    }

    #endregion

    #region Timer Messages - General Timer

    //Timer Variables
    [SerializeField] private float timeToQuitMessage;

    private float timerMessages = 0f;
    private bool active;

    private void Update()
    {
        //Sección Timer Messages. Funcionamiento del temporizador para desaparecer el mensaje
        if (active)
        {
            timerMessages += Time.deltaTime;

            if (timerMessages >= timeToQuitMessage)
            {
                HideMessage();
                active = false;
            }
        }

        //Sección Timer Avatar. El avatar desaparecerá después de un tiempo en caso de que los usuarios no manden mensajes
        timerAvatar += Time.deltaTime;
        if(timerAvatar >= timeToDissapear)
        {
            DissapearAvatar();
        }
    }

    #endregion

    #region Timer Avatar

    //Timer Variables
    [SerializeField] private float timeToDissapear;

    private float timerAvatar = 0f;

    private void DissapearAvatar()
    {
        gameObject.SetActive(false);
    }

    #endregion

}
