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
    }

    #region User

    public async void CreateUser(string name)
    {
        DataSnapshot snapshot = await (UserSnapshot(name));

        //Si ya existe, no se crea
        if (snapshot.Exists)
        {
            Debug.LogWarning("Ya existe este usuario");
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
                Debug.Log($"Usuario {name} eliminado correctamente.");
            }
            else
            {
                Debug.LogError($"Error al eliminar el usuario: {task.Exception}");
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
                Debug.Log("Clave: " + dato.Key + ", Valor: " + dato.Value);
            }
        }
        else
        {
            Debug.Log("El usuario no existe");
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

    public async void UseKoritos(string name, int value) 
    {
        bool success = await SubtractKoritos(name, value);

        if (success)
        {
            Debug.Log("Operación exitosa: Koritos restados.");
        }
        else
        {
            Debug.LogWarning("Operación fallida: No hay suficientes Koritos.");
        }
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

    public bool CheckKoritos(string user, int requiredKoritos)
    {
        int currentKoritos = OnGetKoritosSync(user); // Utiliza la versión síncrona de obtener Koritos
        return currentKoritos >= requiredKoritos;
    }

    // Función síncrona para obtener los Koritos (llama a la función OnGetKoritos pero de manera bloqueante)
    public int OnGetKoritosSync(string user)
    {
        // Suponiendo que OnGetKoritos es asíncrona, deberías implementarlo de manera síncrona.
        // Aquí hay un ejemplo simple de cómo hacerlo:
        int koritos = 0;

        // Si ya tienes un sistema para obtener los koritos de manera síncrona, puedes usarlo aquí.
        Task.Run(async () =>
        {
            koritos = await OnGetKoritos(user); // Obtener de la base de datos
        }).Wait(); // Espera a que termine la tarea sincrónicamente

        return koritos;
    }

    public async Task<bool> SubtractKoritos(string name, int value)
    {
        DatabaseReference userRef = dbRef.Child("users").Child(name);
        DataSnapshot snapshot = await userRef.Child("koritos").GetValueAsync();

        // Si el usuario no tiene "koritos", devolvemos false
        if (!snapshot.Exists)
        {
            Debug.LogWarning("El usuario no existe o no tiene Koritos.");
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

            Debug.Log("Koritos restados correctamente");
            return true; // La operación fue exitosa
        }
        else
        {
            // Si no tiene suficientes Koritos, retornamos false
            Debug.LogWarning("No tiene suficientes Koritos.");
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

        Debug.Log("Koritos added correctamente");
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

        Debug.Log($"El avatar de la base de datos es {avatar}");
        //Si el avatar es default, cambiarlo a uno aleatorio
        if (avatar == default)
        {
            AvatarCharacter tempAvatar = avatarSpawner.avatarCharacters[UnityEngine.Random.Range(0, avatarSpawner.avatarCharacters.Length)];
            Debug.Log($"El avatar seleccionado aleatoriamente es {tempAvatar.name}");
            DatabaseManager.Instance.UpdateAvatar(name, tempAvatar.name);
        }
        else
        {
            Debug.Log("El jugador ya cuenta con un avatar");
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

        Debug.LogWarning("Avatar cambiado exitosamente");
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
