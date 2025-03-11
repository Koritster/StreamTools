using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackjackMinigame : MinigameClass
{
    private Dictionary<string, int> playerBets = new Dictionary<string, int>();//Esta variable almacena las apuestas de cada jugador.
    private Dictionary<string, List<int>> playerHands = new Dictionary<string, List<int>>();//Esta variable guarda las cartas de cada jugador.
    private List<int> dealerHand = new List<int>();//Esta variable representa la mano del crupier.
    private bool gameStarted = false;

    void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
        twitch.OnChatMessage.AddListener(StartGameCommand);
    }

    //Función que inicia el juego cuando se recibe el comando
    public override void StartGameCommand(string user, string message)
    {
        if (message == "!blackjack" && !gameStarted)
        {
            gameStarted = true;
            playerBets.Clear();
            playerHands.Clear();
            dealerHand.Clear();
            twitch.SendTwitchMessage("¡Se ha iniciado una partida de Blackjack! Usa !join para participar.");
        }
    }

    //Función que maneja los comandos de los jugadores
    public override void OnChatMessage(string user, string message)
    {
        if (!gameStarted) return;

        if (message.StartsWith("!join"))
        {
            if (!playerHands.ContainsKey(user))
            {
                playerHands[user] = new List<int> { DrawCard(), DrawCard() };
                twitch.SendTwitchMessage(user + " se ha unido al Blackjack.");
            }
        }
        else if (message.StartsWith("!bet"))
        {
            string[] parts = message.Split(' ');
            if (parts.Length > 1 && int.TryParse(parts[1], out int bet))
            {
                playerBets[user] = bet;
                DatabaseManager.Instance.UpdateKoritos(user, -bet);
                twitch.SendTwitchMessage(user + " ha apostado " + bet + " Koritos.");
            }
        }
        else if (message == "!hit")
        {
            if (playerHands.ContainsKey(user))
            {
                playerHands[user].Add(DrawCard());
                twitch.SendTwitchMessage(user + " ha tomado una carta. Su mano: " + string.Join(", ", playerHands[user]));
            }
        }
        else if (message == "!stand")
        {
            twitch.SendTwitchMessage(user + " se ha plantado.");
        }
        else if (message == "!endgame")
        {
            DetermineWinners();
            gameStarted = false;
        }
    }

    //Función para obtener una carta aleatoria del mazo.
    private int DrawCard()
    {
        return Random.Range(1, 11);
    }

    //Evalúar los resultados y determina los ganadores.
    private void DetermineWinners()
    {
        foreach (var player in playerHands)
        {
            int playerTotal = CalculateHand(player.Value);
            int dealerTotal = CalculateHand(dealerHand);

            if (playerTotal > 21 || (dealerTotal <= 21 && dealerTotal > playerTotal))
            {
                twitch.SendTwitchMessage(player.Key + " ha perdido su apuesta.");
            }
            else if (playerTotal == dealerTotal)
            {
                DatabaseManager.Instance.UpdateKoritos(player.Key, playerBets[player.Key]);
                twitch.SendTwitchMessage(player.Key + " ha empatado y recupera su apuesta.");
            }
            else
            {
                DatabaseManager.Instance.UpdateKoritos(player.Key, playerBets[player.Key] * 2);
                twitch.SendTwitchMessage(player.Key + " ha ganado! Se lleva " + (playerBets[player.Key] * 2) + " Koritos.");
            }
        }
    }

    //Suma los valores de las cartas en una mano.
    private int CalculateHand(List<int> hand)
    {
        int total = 0;
        foreach (int card in hand)
        {
            total += card;
        }
        return total;
    }
}
