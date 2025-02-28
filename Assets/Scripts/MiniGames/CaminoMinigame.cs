using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaminoMinigame : MinigameClass
{
    private List<string> jugadores = new List<string>();
    private List<string> sobrevivientes = new List<string>();

    private bool gameStarted = false;
    private int rondaActual = 0;
    private string caminoIncorrecto;
    private string[] consecuencias = { "cayeron por un barranco", "fueron atacados por lobos", "se perdieron en la niebla" };

    private void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
        twitch.OnChatMessage.AddListener(StartGameCommand);
    }

    public override void StartGameCommand(string user, string message)
    {
        if (message == "!camino" && !gameStarted)
        {
            gameStarted = true;
            rondaActual = 1;
            jugadores.Clear();
            sobrevivientes.Clear();
            twitch.SendTwitchMessage("¡Elige tu camino! Usa !izquierda o !derecha para avanzar.");
            caminoIncorrecto = Random.Range(0, 2) == 0 ? "izquierda" : "derecha";
        }
    }

    public override void OnChatMessage(string user, string message)
    {
        if (!gameStarted) return;

        if (message == "!izquierda" || message == "!derecha")
        {
            if (!jugadores.Contains(user))
            {
                jugadores.Add(user);
                if (message.Substring(1) != caminoIncorrecto)
                {
                    sobrevivientes.Add(user);
                }
            }
        }
    }

    private void NextRound()
    {
        string consecuencia = consecuencias[Random.Range(0, consecuencias.Length)];
        twitch.SendTwitchMessage("Los que eligieron " + caminoIncorrecto + " " + consecuencia + " y fueron eliminados.");

        jugadores = new List<string>(sobrevivientes);
        sobrevivientes.Clear();
        rondaActual++;

        if (rondaActual > 3 || jugadores.Count == 0)
        {
            EndGame();
        }
        else
        {
            caminoIncorrecto = Random.Range(0, 2) == 0 ? "izquierda" : "derecha";
            twitch.SendTwitchMessage("Ronda " + rondaActual + " ¡Elige de nuevo! !izquierda o !derecha");
        }
    }

    private void EndGame()
    {
        gameStarted = false;
        if (jugadores.Count > 0)
        {
            twitch.SendTwitchMessage("¡Felicidades a los sobrevivientes: " + string.Join(", ", jugadores) + "!");
        }
        else
        {
            twitch.SendTwitchMessage("¡Nadie sobrevivió! Inténtalo de nuevo.");
        }
    }
}
