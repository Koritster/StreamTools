using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordGuessMinigame : MinigameClass
{
    private string secretWord = ""; // Palabra secreta establecida por el streamer.
    private bool gameStarted = false; // Controla si el juego está activo.
    private HashSet<string> guessedUsers = new HashSet<string>(); // Almacena usuarios que ya intentaron adivinar.

    public GameObject panelSetWord;
    public GameObject panelProgreso;
    public InputField inputPalabra;
    public Button botonIniciar;

    public Text textoProgresoPalabra;
    private char[] displayWord;
    private int pistasMostradas;

    private void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
        twitch.OnChatMessage.AddListener(StartGameCommand);

        if (botonIniciar != null)
            botonIniciar.onClick.AddListener(IniciarJuegoDesdeUI);
    }

    // Inicia el juego con una palabra ingresada por el streamer usando !setword <palabra>
    public override void StartGameCommand(string user, string message)
    {
        string channelName = PlayerPrefs.GetString("ChannelName"); // Nombre del streamer

        if (message.ToLower() == "!setword" && user.ToLower() == channelName.ToLower())
        {
            if (panelSetWord != null)
            {
                panelSetWord.SetActive(true); // Muestra el input y botón
            }
        }
    }

    // Maneja las respuestas del chat
    public override void OnChatMessage(string user, string message)
    {
        if (!gameStarted || string.IsNullOrEmpty(secretWord)) return;

        string[] parts = message.Split(' ');
        if (parts.Length == 2 && parts[0].ToLower() == "!guess")
        {
            string guess = parts[1].ToLower();

            if (!guessedUsers.Contains(user)) // Verifica si el usuario ya intentó adivinar
            {
                guessedUsers.Add(user); // Registra el intento

                if (guess == secretWord)
                {
                    displayWord = secretWord.ToCharArray(); // Mostrar toda la palabra
                    ActualizarTextoProgreso();

                    twitch.SendTwitchMessage("🎉 " + user + " ha adivinado correctamente. ¡Felicidades!");
                    DatabaseManager.Instance.UpdateKoritos(user, 100);

                    twitch.SendTwitchMessage($"🎉 {user} ha adivinado correctamente la palabra secreta y ganó 100 koritos.");
                    gameStarted = false;
                    textoProgresoPalabra.text = "";
                    panelProgreso.SetActive(false); //Ocultar el panel al ganar
                }
                else
                {
                    twitch.SendTwitchMessage(user + " ha fallado. ❌ ¡Sigue intentando!");
                }
            }
            else
            {
                twitch.SendTwitchMessage(user + ", ¡ya intentaste adivinar! ⏳ Espera la próxima ronda.");
            }
        }
    }
    public void IniciarJuegoDesdeUI()
    {
        if (!gameStarted && inputPalabra != null && !string.IsNullOrEmpty(inputPalabra.text))
        {
            secretWord = inputPalabra.text.ToLower();

            int longitud = secretWord.Length;

            if (longitud <= 4)
                pistasMostradas = 0;
            else if (longitud <= 7)
                pistasMostradas = 2;
            else if (longitud <= 10)
                pistasMostradas = 3;
            else
                pistasMostradas = 4;

            displayWord = new char[secretWord.Length];
            for (int i = 0; i < secretWord.Length; i++)
            {
                displayWord[i] = '_';
            }

            // Revelar automáticamente algunas letras como pista
            List<int> indices = new List<int>();
            for (int i = 0; i < secretWord.Length; i++) indices.Add(i);
            for (int i = 0; i < Mathf.Min(pistasMostradas, secretWord.Length); i++)
            {
                int index = indices[Random.Range(0, indices.Count)];
                indices.Remove(index);
                displayWord[index] = secretWord[index];
            }
            ActualizarTextoProgreso();


            gameStarted = true;
            guessedUsers.Clear();

            twitch.SendTwitchMessage($"✅ ¡La palabra ha sido establecida! Tiene {secretWord.Length} letras. Usa !guess <palabra> para intentar adivinar.");

            inputPalabra.text = ""; // Limpia el input
            panelSetWord.SetActive(false);
        }
        if (panelProgreso != null)
        {
            panelProgreso.SetActive(true); // Muestra el panel de progreso
        }
    }
    private void ActualizarTextoProgreso()
    {
    textoProgresoPalabra.text = string.Join(" ", displayWord);
    }
}
