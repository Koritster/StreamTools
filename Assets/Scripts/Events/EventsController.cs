using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class EventsController : MonoBehaviour
{
    public string pythonScriptPath;
    
    [SerializeField] private string[] commands;

    private TwitchConnect twitch;

    private void Start()
    {
        twitch = GameObject.FindWithTag("GameController").GetComponent<TwitchConnect>();
        twitch.OnChatMessage.AddListener(OnChatMessage);
        
        string relativePath = "Assets/Scripts/Events/Events.py";
        string absolutePath = Path.GetFullPath(relativePath);
        pythonScriptPath = absolutePath;
    }

    public void OnChatMessage(string user, string message)
    {
        //Detectar comando del array
        string command = null;

        foreach (string com in commands)
        {
            //Debug.Log($"{message.ToLower()} comparing with {com.ToLower()}");
            if (message.ToLower().Contains(com.ToLower()))
            {
                command = com;
                break;
            }
        }

        if (command == null)
        {
            print("El comando fue nulo");
            return;
        }

        message = message.Substring(message.IndexOf(command) + command.Length).Trim();

        switch (message)
        {
            case "apagar":
                ShutdownComputer();
                twitch.SendTwitchMessage($"El usuario {user} ha canjeado Apagar!");
                break;
            case "rickroll":
                RickRoll();
                twitch.SendTwitchMessage($"El usuario {user} ha canjeado RickRoll!");
                break;
            default:
                twitch.SendTwitchMessage($"Eventos disponibles: apagar - 5000 koritos, rickroll - 250 koritos");
                break;
        }
    }

    public void ShutdownComputer()
    {
        RunPythonScript("shutdown");
    }

    public void SearchPO()
    {
        RunPythonScript("searchPO");
    }

    public void RickRoll()
    {
        RunPythonScript("rickroll");
    }

    private void RunPythonScript(string action, string argument = "")
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "python", // o "python3" si aplica
            Arguments = $"\"{pythonScriptPath}\" {action} {argument}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        try
        {
            using (Process process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                UnityEngine.Debug.Log($"Python output: {output}");
                if (!string.IsNullOrEmpty(error))
                    UnityEngine.Debug.LogError($"Python error: {error}");
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Error ejecutando el script de Python: " + ex.Message);
        }
    }
}
