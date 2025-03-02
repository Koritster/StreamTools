using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.iOS.Extensions.Common;
using UnityEditor.VersionControl;
using UnityEngine;

public abstract class MinigameClass : MonoBehaviour 
{
    [HideInInspector] public TwitchConnect twitch;

    [Tooltip("Lista de comandos que usarán los usuarios para jugar. Recomendable usar !play y !p")]
    [SerializeField] protected List<string> commands;
    [Tooltip("Comando para iniciar el minijuego.")]
    [SerializeField] protected string commandStartGame;
    [Tooltip("Booleando que sirve para el control del flujo del juego, detectando si se encuentran jugando o no")]
    [SerializeField] protected bool inGame;

    public abstract void OnChatMessage(string user, string message);

    public abstract void StartGameCommand(string user, string message);

    public string DetectCommand(string message)
    {
        if (!inGame)
        {
            return null;
        }

        string command = null;

        foreach (string com in commands)
        {
            if (message.ToLower().Contains(com))
            {
                command = com;
                break;
            }
        }

        command = message.Substring(message.IndexOf(command) + command.Length).Trim();

        return command;
    }
}
