using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageGuessMinigame : MinigameClass, IDropHandler
{
    public RawImage displayImage; // Muestra la imagen en la interfaz.
    public Material blackAndWhiteMaterial; // Material para aplicar el efecto de blanco y negro.
    private string correctAnswer; // Respuesta correcta (nombre del archivo).
    private bool gameStarted = false; // Indica si el juego está activo.
    private HashSet<string> guessedUsers = new HashSet<string>(); // Almacena usuarios que ya intentaron adivinar.

    private void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
    }

    // Maneja el inicio del juego cuando el streamer escribe !imagen
    public override void StartGameCommand(string user, string message)
    {
        if (message == "!imagen" && !gameStarted)
        {
            twitch.SendTwitchMessage("¡Arrastra una imagen para comenzar el juego!");
        }
    }

    // Maneja el evento de arrastrar y soltar la imagen
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            string path = eventData.pointerDrag.name; // Obtiene el nombre del archivo.
            correctAnswer = path.Split('.')[0].ToLower(); // Usa el nombre del archivo como la respuesta.
            LoadAndProcessImage(path);
            twitch.SendTwitchMessage("Imagen subida. ¡Adivinen el nombre usando !guess <nombre>!");
            gameStarted = true;
            guessedUsers.Clear(); // Reinicia los intentos de usuarios.
        }
    }

    // Carga la imagen y aplica el shader de blanco y negro
    private void LoadAndProcessImage(string imagePath)
    {
        byte[] imageData = System.IO.File.ReadAllBytes(imagePath); // Carga la imagen.
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);
        displayImage.texture = texture;
        displayImage.material = blackAndWhiteMaterial; // Aplica el material.
    }

    // Maneja los mensajes del chat para las respuestas
    public override void OnChatMessage(string user, string message)
    {
        if (!gameStarted) return;

        string[] parts = message.Split(' ');
        if (parts[0] == "!guess" && parts.Length == 2)
        {
            string guess = parts[1].ToLower();

            if (!guessedUsers.Contains(user)) // Verifica si el usuario ya intentó adivinar
            {
                guessedUsers.Add(user);

                if (guess == correctAnswer)
                {
                    twitch.SendTwitchMessage(user + " ha adivinado correctamente. ¡Felicidades!");
                    DatabaseManager.Instance.UpdateKoritos(user, 100);
                    gameStarted = false; // Termina el juego
                }
                else
                {
                    twitch.SendTwitchMessage(user + " ha fallado. ¡Intenta de nuevo!");
                }
            }
        }
    }
}
