using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuletaMinigame : MinigameClass
{
    private Dictionary<string, (string, int)> apuestas = new Dictionary<string, (string, int)>();
    private bool gameStarted = false;

    private void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
        twitch.OnChatMessage.AddListener(StartGameCommand);
    }

    public override void StartGameCommand(string user, string message)
    {
        if (message == "!ruleta" && !gameStarted)
        {
            gameStarted = true;
            apuestas.Clear();
            twitch.SendTwitchMessage("¡La ruleta ha comenzado! Usa !apuesto <número/color/par-impar> <cantidad> para participar.");
        }
    }

    public override void OnChatMessage(string user, string message)
    {
        if (!gameStarted) return;

        string[] parts = message.Split(' ');
        if (parts.Length == 3 && parts[0] == "!apuesto" && int.TryParse(parts[2], out int apuesta))
        {
            apuestas[user] = (parts[1].ToLower(), apuesta);
            DatabaseManager.Instance.UpdateKoritos(user, -apuesta);
            twitch.SendTwitchMessage(user + " ha apostado " + apuesta + " Koritos a " + parts[1] + "!");
        }
        else if (message == "!girar")
        {
            GirarRuleta();
        }
    }

    private void GirarRuleta()
    {
        gameStarted = false;
        int numeroGanador = Random.Range(0, 37);
        string colorGanador = (numeroGanador % 2 == 0) ? "rojo" : "negro";
        string paridad = (numeroGanador % 2 == 0) ? "par" : "impar";
        twitch.SendTwitchMessage("La ruleta ha girado... y salió el " + numeroGanador + " (" + colorGanador + ", " + paridad + ")!");

        foreach (var apuesta in apuestas)
        {
            string jugador = apuesta.Key;
            (string tipoApuesta, int monto) = apuesta.Value;

            if (tipoApuesta == numeroGanador.ToString())
            {
                DatabaseManager.Instance.UpdateKoritos(jugador, monto * 35);
                twitch.SendTwitchMessage(jugador + " acertó el número y ganó " + (monto * 35) + " Koritos!");
            }
            else if (tipoApuesta == colorGanador || tipoApuesta == paridad)
            {
                DatabaseManager.Instance.UpdateKoritos(jugador, monto * 2);
                twitch.SendTwitchMessage(jugador + " acertó y ganó " + (monto * 2) + " Koritos!");
            }
            else
            {
                twitch.SendTwitchMessage(jugador + " perdió su apuesta de " + monto + " Koritos.");
            }
        }
    }
}
