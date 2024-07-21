using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarSpawner : MonoBehaviour
{
    private void Start()
    {
        GameObject[] locations = GameObject.FindGameObjectsWithTag("SpawnAvatarLocation");
        for(int i = 0; i < locations.Length; i++)
        {
            spawnLocations[i] = locations[i].transform;
        }
    }

    //Editables
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private GameObject avatarPrefab;

    //No editables
    [SerializeField] private string[] usersWithAvatar;

    public void OnChatMessage(string user, string message)
    {
        if (!usersWithAvatar.Contains(user))
        {
            GameObject avatarGO = Instantiate(avatarPrefab, spawnLocations[Random.Range(0, spawnLocations.Length)]);

            Avatars avatar = avatarGO.GetComponent<Avatars>();
            avatar.ChangeName(user);
        }
    }
}
