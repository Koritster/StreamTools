using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Threading.Tasks;
using UnityEngine;

public class MinigameMagicNumber : MinigameClass
{
    private void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
        twitch.OnChatMessage.AddListener(StartGameCommand);
    }

    public override void OnChatMessage(string user, string message)
    {
        string commandValue = DetectCommand(message);

        Debug.Log(commandValue);

        if (commandValue == "" || commandValue == null)
            return;

        if (int.TryParse(commandValue, out int value))
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
    
    public override void StartGameCommand(string user, string message)
    {   
        if (message.Contains(commandStartGame) && user == twitch.User.ToLower())
        {
            StartGame();
        }
    }

    [Header("Variables juego")]
    [SerializeField] private int koritosToWin;
    [SerializeField] private GameObject[] texts;

    private int magicNo;

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
