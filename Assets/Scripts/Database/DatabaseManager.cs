using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using UnityEditor;
using Unity.VisualScripting;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using static UnityEngine.Rendering.DebugUI;
using System.Diagnostics;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private DatabaseReference dbRef;

    private DatabaseReference koritosDispRef;

    private AvatarSpawner avatarSpawner;

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        avatarSpawner = FindAnyObjectByType<AvatarSpawner>();
        
        if (Instance == null)
        {
            Instance = this;
        }


        //Crear disparador al cambiar los Koritos
        koritosDispRef = FirebaseDatabase.DefaultInstance.GetReference("users");
        //koritosDispRef.ChildChanged -= OnKoritosChanged;
        //koritosDispRef.ChildChanged += OnKoritosChanged;

        koritosDispRef.ChildChanged -= OnAvatarChanged;
        koritosDispRef.ChildChanged += OnAvatarChanged;

        string relativePath = "Assets/Scripts/Events/Events.py";
        string absolutePath = Path.GetFullPath(relativePath);
        pythonScriptPath = absolutePath;

        twitch = GameObject.FindWithTag("GameController").GetComponent<TwitchConnect>();
    }

    #region User

    public async void CreateUser(string name)
    {
        DataSnapshot snapshot = await (UserSnapshot(name));

        //Si ya existe, no se crea
        if (snapshot.Exists)
        {
            UnityEngine.Debug.LogWarning("Ya existe este usuario");
            return;
        }

        //Crear usuario
        User newUser = new User(name, 0, default);
        string json = JsonUtility.ToJson(newUser);

        await dbRef.Child("users").Child(name).SetRawJsonValueAsync(json);
    }

    public async void DeleteUser(string name)
    {
        DatabaseReference userRef = dbRef.Child("users").Child(name);

        await userRef.RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                UnityEngine.Debug.Log($"Usuario {name} eliminado correctamente.");
            }
            else
            {
                UnityEngine.Debug.LogError($"Error al eliminar el usuario: {task.Exception}");
            }
        });
    }

    private async Task OnGetUserData(string name)
    {
        DataSnapshot snapshot = await (UserSnapshot(name));

        if (snapshot.Exists)
        {
            foreach (var dato in snapshot.Children)
            {
                UnityEngine.Debug.Log("Clave: " + dato.Key + ", Valor: " + dato.Value);
            }
        }
        else
        {
            UnityEngine.Debug.Log("El usuario no existe");
        }
    }

    private async Task<DataSnapshot> UserSnapshot(string name)
    {
        DatabaseReference userRef = dbRef.Child("users").Child(name);

        DataSnapshot snapshot = await userRef.GetValueAsync();

        return snapshot;
    }

    #endregion

    #region Koritos

    public async void UpdateKoritos(string name, int value)
    {
        await ModifyKoritosTask(name, value);
    }

    public async void PrintKoritos(string name)
    {
        int koritos = await OnGetKoritos(name);

        twitch.SendTwitchMessage($"{name} tienes {koritos} koritos!");
    }

    public int GetKoritos(string name)
    {
        int koritos = 0;
        Task.Run(async () =>
        {
            koritos = await OnGetKoritos(name);
        });    
        return koritos;
    }

    public async Task<bool> SubtractKoritos(string name, int value)
    {
        DatabaseReference userRef = dbRef.Child("users").Child(name);
        DataSnapshot snapshot = await userRef.Child("koritos").GetValueAsync();

        // Si el usuario no tiene "koritos", devolvemos false
        if (!snapshot.Exists)
        {
            UnityEngine.Debug.LogWarning("El usuario no existe o no tiene Koritos.");
            return false;
        }

        // Obtenemos los Koritos actuales
        int currentKoritos = int.Parse(snapshot.Value.ToString());

        // Verificamos si el usuario tiene suficientes Koritos
        if (currentKoritos >= value)
        {
            // Restamos los Koritos
            int newKoritos = currentKoritos - value;

            // Actualizamos el valor en la base de datos
            await userRef.UpdateChildrenAsync(new System.Collections.Generic.Dictionary<string, object> {
            { "koritos", newKoritos }
        });

            UnityEngine.Debug.Log("Koritos restados correctamente");
            return true; // La operación fue exitosa
        }
        else
        {
            // Si no tiene suficientes Koritos, retornamos false
            UnityEngine.Debug.LogWarning("No tiene suficientes Koritos.");
            return false;
        }
    }

    private async Task ModifyKoritosTask(string name, int value)
    {
        DatabaseReference userRef = dbRef.Child("users").Child(name);
        DataSnapshot snapshot = await userRef.Child("koritos").GetValueAsync();

        int currentKoritos = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;

        int newKoritos = currentKoritos + value;

        await userRef.UpdateChildrenAsync(new System.Collections.Generic.Dictionary<string, object>{
            { "koritos", newKoritos }
        });

        UnityEngine.Debug.Log("Koritos added correctamente");
    }

    private async Task<int> OnGetKoritos(string name)
    {
        DataSnapshot snapshot = await (UserSnapshot(name));

        if (snapshot.Exists)
        {
            if (int.TryParse(snapshot.Child("koritos").Value.ToString(), out int koritos))
            {
                return koritos;
            }
        }

        return 0;
    }

    #endregion

    #region Avatar

    public async void SpawnAvatar(string name)
    {
        string avatar = await OnGetAvatar(name);

        UnityEngine.Debug.Log($"El avatar de la base de datos es {avatar}");
        //Si el avatar es default, cambiarlo a uno aleatorio
        if (avatar == default)
        {
            AvatarCharacter tempAvatar = avatarSpawner.avatarCharacters[UnityEngine.Random.Range(0, avatarSpawner.avatarCharacters.Length)];
            UnityEngine.Debug.Log($"El avatar seleccionado aleatoriamente es {tempAvatar.name}");
            DatabaseManager.Instance.UpdateAvatar(name, tempAvatar.name);
        }
        else
        {
            UnityEngine.Debug.Log("El jugador ya cuenta con un avatar");
            foreach (AvatarCharacter tempAvatar in avatarSpawner.avatarCharacters)
            {
                if (tempAvatar.name == avatar)
                {
                    avatarSpawner.usersWithAvatar[name].ChangeAvatar(tempAvatar);
                    break;
                }
            }
        }
    }

    public async void UpdateAvatar(string name, string avatar)
    {
        await ModifyAvatarTask(name, avatar);
    }

    private async Task ModifyAvatarTask(string name, string avatar)
    {
        DatabaseReference userRef = dbRef.Child("users").Child(name);

        await userRef.UpdateChildrenAsync(new System.Collections.Generic.Dictionary<string, object>{
            { "avatar", avatar }
        });

        UnityEngine.Debug.LogWarning("Avatar cambiado exitosamente");
    }

    //Buscar avatar en la bd, retornar default si no tiene el usuario
    private async Task<string> OnGetAvatar(string name)
    {
        DataSnapshot snapshot = await (UserSnapshot(name));

        if (snapshot.Exists)
        {
            return snapshot.Child("avatar").Value.ToString();
        }

        return default;
    }

    #endregion

    #region Events

    public string pythonScriptPath;

    private TwitchConnect twitch;

    public async void CallPythonEvent(string user, string message)
    {
        switch (message)
        {
            case "apagar":
                if (await SubtractKoritos(user, 5000))
                {
                    ShutdownComputer();
                    twitch.SendTwitchMessage($"El usuario {user} ha canjeado Apagar!");
                }
                else
                {
                    twitch.SendTwitchMessage($"No cuentas con los koritos suficientes para Apagar!");
                }
                break;
            case "rickroll":
                if (await SubtractKoritos(user, 250))
                {
                    RickRoll();
                    twitch.SendTwitchMessage($"El usuario {user} ha canjeado RickRoll!");
                }
                else
                {
                    twitch.SendTwitchMessage($"No cuentas con los koritos suficientes para RickRoll!");
                }
                break;
            case "chamba":
                if (await SubtractKoritos(user, 100))
                {
                    Chamba();
                    twitch.SendTwitchMessage($"El usuario {user} ha canjeado Chamba!");
                }
                else
                {
                    twitch.SendTwitchMessage($"No cuentas con los koritos suficientes para Chamba!");
                }
                break;
            default:
                twitch.SendTwitchMessage($"Eventos disponibles: apagar - 5000 koritos, rickroll - 250 koritos, chamba - 100 koritos");
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

    public void Chamba()
    {
        RunPythonScript("chamba");
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

    #endregion

    #region Triggers

    private void OnKoritosChanged(object sender, ChildChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            string user = e.Snapshot.Key; // Nombre del usuario que cambió
            int newKoritos = int.Parse(e.Snapshot.Child("koritos").Value.ToString());

            TwitchConnect.Instance.SendTwitchMessage($"@{user} ahora tiene {newKoritos} Koritos!");
        }
    }

    private void OnAvatarChanged(object sender, ChildChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            string user = e.Snapshot.Key; // Nombre del usuario que cambió
            string newAvatar = e.Snapshot.Child("avatar").Value.ToString();

            if (avatarSpawner.usersWithAvatar.ContainsKey(user))
            {
                //AvatarCharacter tempAvatar = avatarSpawner.avatarCharacters[UnityEngine.Random.Range(0, avatarSpawner.avatarCharacters.Length)];
                foreach (AvatarCharacter tempAvatar in avatarSpawner.avatarCharacters)
                {
                    if (tempAvatar.name == newAvatar)
                    {
                        avatarSpawner.usersWithAvatar[user].ChangeAvatar(tempAvatar);
                        break;
                    }
                }
            }
        }
    }

    #endregion
}
