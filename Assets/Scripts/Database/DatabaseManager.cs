using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using UnityEditor;
using Unity.VisualScripting;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private DatabaseReference dbRef;

    private DatabaseReference koritosDispRef;

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        
        if (Instance == null)
        {
            Instance = this;
        }


        //Crear disparador al cambiar los Koritos
        koritosDispRef = FirebaseDatabase.DefaultInstance.GetReference("users");
        koritosDispRef.ChildChanged -= OnKoritosChanged;
        koritosDispRef.ChildChanged += OnKoritosChanged;
    }

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
        User newUser = new User(name, 0);
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

    public async void UpdateKoritos(string name, int value)
    {
        await ModifyKoritosTask(name, value);
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


    private void OnKoritosChanged(object sender, ChildChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            string user = e.Snapshot.Key; // Nombre del usuario que cambió
            int newKoritos = int.Parse(e.Snapshot.Child("koritos").Value.ToString());

            TwitchConnect.Instance.SendTwitchMessage($"@{user} ahora tiene {newKoritos} Koritos!");
        }
    }
}
