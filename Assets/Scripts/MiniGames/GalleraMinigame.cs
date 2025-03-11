using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleraMinigame : MinigameClass
{
    // Almacena las apuestas realizadas a favor del Gallo X.
    private Dictionary<string, int> apuestasA = new Dictionary<string, int>();
    private Dictionary<string, int> apuestasB = new Dictionary<string, int>(); 

    private int vidaA = 100, vidaB = 100;
    private bool gameStarted = false;

    private void Start()
    {
        twitch = TwitchConnect.Instance;
        twitch.OnChatMessage.AddListener(OnChatMessage);
        twitch.OnChatMessage.AddListener(StartGameCommand);
    }

    //Función que inicia la pelea de gallos cuando se recibe el comando !gallera.
    public override void StartGameCommand(string user, string message)
    {
        if (message == "!gallera" && !gameStarted)
        {
            gameStarted = true;
            vidaA = 100;
            vidaB = 100;
            apuestasA.Clear();
            apuestasB.Clear();
            twitch.SendTwitchMessage("¡Comienza la pelea de gallos! Usa !apuesto galloA/galloB cantidad para participar.");
        }
    }

    //maneja los comandos del chat, como apuestas y el inicio de la pelea.
    public override void OnChatMessage(string user, string message)
    {
        if (!gameStarted) return;

        string[] parts = message.Split(' ');
        if (parts.Length == 3 && parts[0] == "!apuesto" && int.TryParse(parts[2], out int apuesta))
        {
            if (parts[1].ToLower() == "galloa")
            {
                apuestasA[user] = apuesta;
                DatabaseManager.Instance.UpdateKoritos(user, -apuesta);
                twitch.SendTwitchMessage(user + " apuesta " + apuesta + " Koritos a Gallo A!");
            }
            else if (parts[1].ToLower() == "gallob")
            {
                apuestasB[user] = apuesta;
                DatabaseManager.Instance.UpdateKoritos(user, -apuesta);
                twitch.SendTwitchMessage(user + " apuesta " + apuesta + " Koritos a Gallo B!");
            }
        }
        else if (message == "!startfight")
        {
            StartCoroutine(Fight());
        }
    }

    //Inicia la pelea por turnos hasta que un gallo pierda toda su vida.
    private System.Collections.IEnumerator Fight()
    {
        while (vidaA > 0 && vidaB > 0)
        {
            yield return new WaitForSeconds(2);
            int damageA = Attack();
            int damageB = Attack();
            vidaB -= damageA;
            vidaA -= damageB;
            twitch.SendTwitchMessage("Gallo A golpea con " + damageA + " de daño. Gallo B golpea con " + damageB + " de daño.");
        }
        DeclareWinner();
    }

    //Determina el daño de un ataque con posibilidades de fallar, ser normal o crítico.
    private int Attack()
    {
        int chance = Random.Range(1, 100);
        if (chance <= 10) return 0; // Falla
        if (chance >= 90) return Random.Range(20, 30); // Crítico
        return Random.Range(10, 20); // Normal
    }

    //Determina el ganador y otorga las ganacias.
    private void DeclareWinner()
    {
        gameStarted = false;
        string winner = vidaA > 0 ? "Gallo A" : "Gallo B";
        twitch.SendTwitchMessage(winner + " gana la pelea!");
        Dictionary<string, int> ganadores = vidaA > 0 ? apuestasA : apuestasB;
        foreach (var player in ganadores)
        {
            DatabaseManager.Instance.UpdateKoritos(player.Key, player.Value * 2);
        }
    }
}
