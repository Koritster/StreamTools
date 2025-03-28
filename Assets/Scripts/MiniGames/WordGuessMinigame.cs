using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordGuessMinigame : MinigameClass
{
    private string secretWord = ""; // Palabra secreta establecida por el streamer.
    private bool gameStarted = false; // Controla si el juego está activo.
    private HashSet<string> guessedUsers = new HashSet<string>(); // Almacena usuarios que ya intentaron adivinar.

    private void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
        twitch.OnChatMessage.AddListener(StartGameCommand);
    }

    // Inicia el juego con una palabra ingresada por el streamer usando !setword <palabra>
    public override void StartGameCommand(string user, string message)
    {
        string channelName = PlayerPrefs.GetString("ChannelName"); // Obtener el nombre del canal
        if (message.StartsWith("!setword ") && user.ToLower() == channelName.ToLower())
        {
            secretWord = message.Substring(9).ToLower(); // Establece la palabra
            gameStarted = true;
            guessedUsers.Clear(); // Reinicia los intentos de usuarios.
            twitch.SendTwitchMessage("✅ ¡La palabra ha sido establecida! Los espectadores pueden adivinar usando !guess <palabra>.");
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
                    twitch.SendTwitchMessage("🎉 " + user + " ha adivinado correctamente. ¡Felicidades!");
                    DatabaseManager.Instance.UpdateKoritos(user, 100); // Da 100 Koritos al ganador.
                    gameStarted = false; // Termina el juego.
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
}
