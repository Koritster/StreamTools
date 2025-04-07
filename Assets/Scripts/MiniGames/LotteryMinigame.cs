using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Permite usar consultas LINQ para manipular colecciones de datos

public class LotteryMinigame : MinigameClass
{
    public Transform cardContainer; // Asignar en el Inspector
    private Dictionary<string, GameObject> cardPrefabs = new Dictionary<string, GameObject>(); // Para mantener referencia a los prefabs
    private List<string> allElements = new List<string> { "Sol", "Luna", "Estrella", "Sirena", "Catrin", "Diablo", "Barril", "Arbol", "Gallo", "Rana" }; // Lista de elementos disponibles
    private Dictionary<string, HashSet<string>> playerCards = new Dictionary<string, HashSet<string>>(); // Tarjetas de los jugadores
    private HashSet<string> calledElements = new HashSet<string>(); // Elementos anunciados
    private bool gameStarted = false;
    private int elementsSpawned = 0; // Para saber cuántos van

    private void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
        twitch.OnChatMessage.AddListener(StartGameCommand);

        // Cargar todos los prefabs automáticamente desde Resources/Cards
        foreach (var element in allElements)
        {
            GameObject prefab = Resources.Load<GameObject>("Lottery/" + element);
            if (prefab != null)
            {
                cardPrefabs[element] = prefab;
            }
            else
            {
                Debug.LogWarning("No se encontró prefab para: " + element);
            }
        }
    }

    // Comienza el juego y da una tarjeta única a cada jugador
    public override void StartGameCommand(string user, string message)
    {
        if (message.ToLower() == "!loteria")
        {
            gameStarted = true; // Activa el juego
            playerCards.Clear(); // Limpia tarjetas anteriores
            calledElements.Clear(); // Reinicia elementos ya llamados
            twitch.SendTwitchMessage("¡Lotería iniciada! Usa !tarjeta para recibir la tuya.");
        }

        if (message == "!tarjeta")
        {
            if (!playerCards.ContainsKey(user))
            {
                playerCards[user] = GenerateCard();
                twitch.SendTwitchMessage(user + ", tu tarjeta es: " + string.Join(", ", playerCards[user]));
                StartCoroutine(CallElementsRoutine()); // Inicia el anuncio automático de elementos cada 8 segundos
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
                    ClearCardsFromScreen();
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

            if (calledElements.Count >= allElements.Count)
            {
                twitch.SendTwitchMessage("🎉 ¡Todos los elementos han sido anunciados! El juego termina sin ganadores.");
                gameStarted = false;
                playerCards.Clear();
                calledElements.Clear();
                ClearCardsFromScreen();
                yield break;
            }

            string newElement;
            do
            {
                newElement = allElements[Random.Range(0, allElements.Count)];
            } while (calledElements.Contains(newElement));

            calledElements.Add(newElement);
            twitch.SendTwitchMessage("Nuevo elemento: " + newElement);

            // Mostrar sprite en pantalla
            GameObject prefab = Resources.Load<GameObject>("Lottery/" + newElement);
            if (prefab != null && cardContainer != null)
            {
                GameObject instance = Instantiate(prefab, cardContainer);
                int maxPerRow = 5;
                float xSpacing = 2f;
                float ySpacing = -3f;
                int column = elementsSpawned % maxPerRow;
                int row = elementsSpawned / maxPerRow;
                // Posición horizontal ordenada
                instance.transform.localPosition = new Vector3(column * xSpacing, row * ySpacing, 0);
                Animator anim = instance.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Play("Carts");
                }
                elementsSpawned++; // Aumenta contador para que el siguiente se acomode a la derecha
            }
        }
    }

    private void ClearCardsFromScreen()
    {
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
        elementsSpawned = 0; // Reiniciar el contador
    }
}

