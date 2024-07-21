using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Net.Sockets;

public class TwitchChat : MonoBehaviour
{
    public UnityEvent<string, string> OnChatMessage; 

    public string username;
    public string password;
    public string channelName;

    private TcpClient tcpClient;
    private StreamReader sr;
    private StreamWriter sw;
    private float reconnectTimer;
    private float reconnectAfter;

    private void Connect()
    {
        tcpClient = new TcpClient("irc.chat.twitch.tv", 6667); 
        sr = new StreamReader(tcpClient.GetStream()); 
        sw = new StreamWriter(tcpClient.GetStream());
        sw.WriteLine("PASS " + password); 
        sw.WriteLine("NICK " + username);
        sw.WriteLine("USER " + username + " 8 *:" + username);
        sw.WriteLine("JOIN #" + channelName);
        sw.Flush();
    }

    public void ReadChat()
    {
        if (tcpClient.Available > 0)
        {
            string message = sr.ReadLine();

            if (message.Contains("PRIVMSG"))
            {
                int splitPoint = message.IndexOf("!");
                string chatter = message.Substring(0, splitPoint - 1);

                splitPoint = message.IndexOf(":", 1);
                string chatMsg = message.Substring(splitPoint + 1);

                OnChatMessage?.Invoke(chatter, chatMsg);
            }
        }
    }

    private void Start()
    {
        reconnectAfter = 60f;
        Connect();
    }

    private void Update()
    {
        if(tcpClient.Available == 0) 
        {
            reconnectTimer = Time.deltaTime;
        }

        if(tcpClient.Available == 0 && reconnectTimer >= reconnectAfter)
        {
            Connect();
            reconnectTimer = 0.0f;
        }

        ReadChat();
    }
}
