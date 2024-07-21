using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Net.Sockets;
using System.IO;

//ESTE SI SIRVE
public class TwitchConnect : MonoBehaviour
{
    public UnityEvent<string, string> OnChatMessage;
    TcpClient Twitch;
    StreamReader Reader;
    StreamWriter Writer;

    const string URL = "irc.chat.twitch.tv";
    const int PORT = 6667;

    public string User;
    public string OAuth; //El OAuth es de la cuenta del User, la cual puede y es recomendable que sea diferente al canal
    public string Channel; //Es el canal de twitch al que se va a conectar

    float PingCounter = 0;
    private void ConnectToTwitch()
    {
        Twitch = new TcpClient(URL, PORT);
        Reader = new StreamReader(Twitch.GetStream());
        Writer = new StreamWriter(Twitch.GetStream());

        Writer.WriteLine("PASS " + OAuth);
        Writer.WriteLine("NICK " + User.ToLower());
        Writer.WriteLine("JOIN #" + Channel.ToLower());
        Writer.Flush();
    }

    private void Awake()
    {
        ConnectToTwitch();
    }
    void Update()
    {
        PingCounter += Time.deltaTime;
        if(PingCounter > 60)
        {
            Writer.WriteLine("PING " + URL);
            Writer.Flush();
            PingCounter = 0;
        }
        if (!Twitch.Connected)
        {
            ConnectToTwitch();
        }
        if (Twitch.Available > 0)
        {
            string message = Reader.ReadLine();

            if (message.Contains("PRIVMSG"))
            {
                int splitPoint = message.IndexOf("!");
                string chatter = message.Substring(1, splitPoint - 1);

                splitPoint = message.IndexOf(":", 1);
                string msg = message.Substring(splitPoint + 1);

                OnChatMessage?.Invoke(chatter, msg);
            }
            print(message);
        }
    }
}
