using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public string userName;
    public int koritos;
    public string avatar;

    public User(string userName, int koritos, string avatar)
    {
        this.userName = userName;
        this.koritos = koritos;
        this.avatar = avatar;
    }
}

