using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarSpawner : MonoBehaviour
{
    private void Start()
    {
        GameObject[] locations = GameObject.FindGameObjectsWithTag("SpawnAvatarLocation");

        spawnLocations = new Transform[locations.Length];
        for(int i = 0; i < locations.Length; i++)
        {
            spawnLocations[i] = locations[i].transform;
        }
    }

    [SerializeField] private GameObject pf_Avatar;

    private Transform[] spawnLocations;
    [SerializeField] private List<string> usersWithAvatar = new List<string>();

    public void OnChatMessage(string user, string message)
    {
        if (!usersWithAvatar.Contains(user))
        {
            GameObject avatarGO = Instantiate(pf_Avatar, spawnLocations[Random.Range(0, spawnLocations.Length)]);

            Avatar avatar = avatarGO.GetComponent<Avatar>();
            avatar.ChangeName(user);

            usersWithAvatar.Add(user);
        }
    }
}
