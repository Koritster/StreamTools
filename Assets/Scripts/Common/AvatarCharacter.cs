using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Avatar", menuName = "New Avatar", order = 110)]
public class AvatarCharacter : ScriptableObject
{
    public new string name;
    public string category;
    public GameObject avatarPrefab;
}
