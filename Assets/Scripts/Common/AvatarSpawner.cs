using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarSpawner : MonoBehaviour
{
    private void Awake()
    {
        //Establece las localizaciones automáticamente. Las localizaciones deben tener el tag SpawnAvatarLocation
        GameObject[] locations = GameObject.FindGameObjectsWithTag("SpawnAvatarLocation");

        spawnLimits = new Transform[locations.Length];
        for (int i = 0; i < locations.Length; i++)
        {
            spawnLimits[i] = locations[i].transform;
        }
    }

    public string avatarCategory; //"any" for default
    public AvatarCharacter[] avatarCharacters;
    //Lista para los usuarios que no se desea que puedan usar comandos o sus mensajes repercutan en las acciones del programa
    public List<string> bannedUsers = new List<string>();
    //Lista de comandos que permiten la aparicion de un avatar
    public Dictionary<string, Animator> avatarSprites = new Dictionary<string, Animator>();
    
    [SerializeField] private List<string> commandsWhiteList = new List<string>();
    [SerializeField] private GameObject pf_Avatar;

    //Localizaciones donde un avatar puede aparecer al escribir un mensaje
    private Transform[] spawnLimits;
    //Diccionario que almacena el nombre de usuario como llave y su script de Avatar para acceder a él desde cualquier script
    [HideInInspector] public Dictionary<string, Avatar> usersWithAvatar = new Dictionary<string, Avatar>();

    public void OnChatMessage(string user, string message)
    {
        bool canSpawn = false;

        //Verifica que el usuario no esté baneado
        foreach (string list in bannedUsers)
        {
            if (user.Contains(list.ToLower()))
            {
                return;
            }
        }

        //Verifica que ciertos comandos puedan aparecer un avatar
        foreach(string list in commandsWhiteList)
        {
            if (message.ToLower().Contains(list.ToLower()))
            {
                canSpawn = true;
            }
        }

        //Verifica que el mensaje no haya sido un comando
        if (message.Contains("!") && !canSpawn)
        {
            return;
        }

        //Instancia un nuevo avatar si aún no había uno para la persona que habló. Lo guarda en el diccionario
        if (!usersWithAvatar.ContainsKey(user))
        {
            //ciclo para validar que el cambio sea adecuado para la categoría que se seleccione
            while (true)
            {
                GameObject avatarGO = Instantiate(pf_Avatar, spawnLocations[Random.Range(0, spawnLocations.Length)]);

                Avatar avatar = avatarGO.GetComponent<Avatar>();
                avatar.ChangeName(user);
                AvatarCharacter tempAvatar = avatarCharacters[Random.Range(0, avatarCharacters.Length)];
                avatar.ChangeAvatar(tempAvatar);

                usersWithAvatar.Add(user, avatar);

                if(avatarCategory == null || avatarCategory == "any" || tempAvatar.category == avatarCategory)
                {
                    return;
                }
            }
        }
        else
        {
            //Habilita nuevamente el avatar, en vez de instanciar uno nuevo
            if (!usersWithAvatar[user].gameObject.activeSelf)
            {
                usersWithAvatar[user].gameObject.SetActive(true);
            }
        }
    }
}
