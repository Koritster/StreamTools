using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Permite usar consultas LINQ para manipular colecciones de datos

public class LotteryMinigame : MinigameClass
{
    private List<string> allElements = new List<string> { "Sol", "Luna", "Estrella", "Sirena", "Catrin", "Diablo", "Barril", "Arbol", "Gallo", "Rana" }; // Lista de elementos disponibles
    private Dictionary<string, HashSet<string>> playerCards = new Dictionary<string, HashSet<string>>(); // Tarjetas de los jugadores
    private HashSet<string> calledElements = new HashSet<string>(); // Elementos anunciados
    private bool gameStarted = false;

    private void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
        StartCoroutine(CallElementsRoutine()); // Inicia el anuncio automático de elementos cada 8 segundos
    }

    // Comienza el juego y da una tarjeta única a cada jugador
    public override void StartGameCommand(string user, string message)
    {
        if (message == "!tarjeta")
        {
            if (!playerCards.ContainsKey(user))
            {
                playerCards[user] = GenerateCard();
                twitch.SendTwitchMessage(user + ", tu tarjeta es: " + string.Join(", ", playerCards[user]));
            }
            else
            {
                twitch.SendTwitchMessage(user + ", ya tienes una tarjeta.");
            }
        }
    }

    // Genera una tarjeta aleatoria con 4 elementos únicos
    private HashSet<string> GenerateCard()
    {
        HashSet<string> card = new HashSet<string>();
        while (card.Count < 4)
        {
            string element = allElements[Random.Range(0, allElements.Count)];
            card.Add(element);
        }
        return card;
    }

    // Maneja las respuestas del chat para marcar elementos
    public override void OnChatMessage(string user, string message)
    {
        string[] parts = message.Split(' ');
        if (parts.Length == 2 && parts[0].ToLower() == "!marcar")
        {
            string element = parts[1];
            if (playerCards.ContainsKey(user) && playerCards[user].Contains(element) && calledElements.Contains(element))
            {
                playerCards[user].Remove(element); // Marca el elemento quitándolo de la tarjeta
                twitch.SendTwitchMessage(user + " ha marcado " + element);

                if (playerCards[user].Count == 0) // Si ya no quedan elementos, gana el jugador
                {
                    twitch.SendTwitchMessage(user + " ha ganado la lotería!");
                    gameStarted = false;
                    playerCards.Clear(); // Reinicia el juego
                    calledElements.Clear();
                }
            }
        }
    }

    // Rutina que anuncia un nuevo elemento cada 8 segundos
    private IEnumerator CallElementsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(8);
            if (!gameStarted) continue;

            string newElement;
            do
            {
                newElement = allElements[Random.Range(0, allElements.Count)];
            } while (calledElements.Contains(newElement)); // Asegura que no se repita

            calledElements.Add(newElement);
            twitch.SendTwitchMessage("Nuevo elemento: " + newElement);
        }
    }
}

