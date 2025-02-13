using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Threading.Tasks;
using UnityEngine;

public class MinigameMagicNumber : MonoBehaviour
{
    [SerializeField] private string[] commands;

    private TwitchConnect twitch;

    private void Start()
    {
        twitch = GameObject.FindWithTag("GameController").GetComponent<TwitchConnect>();
        twitch.OnChatMessage.AddListener(OnChatMessage);
        twitch.OnChatMessage.AddListener(StartGameCommand);
    }

    public void OnChatMessage(string user, string message)
    {
        if (!inGame)
        {
            return;
        }

        string command = null;

        foreach (string com in commands)
        {
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

        if (message == "")
            return;

        if (int.TryParse(message, out int value))
        {
            var guessedNo = GuessNumber(value);
            if (guessedNo.correct)
            {
                try 
                {
                    DatabaseManager.Instance.UpdateKoritos(user, koritosToWin);

                    twitch.SendTwitchMessage($"@{user} {guessedNo.msg}");
                }
                catch (System.Exception ex)
                {
                    twitch.SendTwitchMessage(ex.Message);
                }

                EndGame();
            }
            else
            {
                twitch.SendTwitchMessage($"@{user} {guessedNo.msg}");
            }
        }
        else
        {
            twitch.SendTwitchMessage($"@{user} El numero no es valido");
        }
    }

    //Comando para iniciar el juego
    [SerializeField] private string commandStartGame;

    public void StartGameCommand(string user, string message)
    {
        //Debug.Log($"{message} from {user}");
        //Debug.Log($"{commandStartGame} correct {twitch.User}");
        if (message.Contains(commandStartGame) && user == twitch.User.ToLower())
        {
            StartGame();
        }
    }

    [Header("Variables juego")]
    [SerializeField] private int koritosToWin;
    [SerializeField] private GameObject[] texts;

    private int magicNo;
    private bool inGame;

    private void StartGame()
    {
        magicNo = Random.Range(1, 20 + 1);

        twitch.SendTwitchMessage($"El juego del numero mágico ha empezado! Usa el comando {commands[0]} para jugar e intentar adivinar el número entre el 0 y el 20! :D");

        foreach(GameObject txt in texts)
        {
            txt.SetActive(true);
        }

        inGame = true;
    }

    private void EndGame()
    {
        foreach (GameObject txt in texts)
        {
            txt.SetActive(false);
        }

        inGame = false;
    }

    private (bool correct, string msg) GuessNumber(int no)
    {
        if(no == magicNo)
        {
            return (true, "Adivinaste el número mágico!");
        }
        //Numero mágico es menor al numero
        else if (no > magicNo)
        {
            return (false, "El número mágico es menor a " + no);
        }
        //Numero mágico es mayor al numero
        else
        {
            return (false, "El número mágico es mayor a " + no);
        }
    }
}
