using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private InputField username;
    [SerializeField] private InputField channelName;
    [SerializeField] private InputField oauth;

    private void Awake()
    {
        username.text = PlayerPrefs.GetString("Username");
        channelName.text = PlayerPrefs.GetString("ChannelName");
        oauth.text = PlayerPrefs.GetString("Oauth");
    }

    public void Play(string scene)
    {
        PlayerPrefs.SetString("Username", username.text);
        PlayerPrefs.SetString("ChannelName", channelName.text);
        PlayerPrefs.SetString("Oauth", oauth.text);
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
