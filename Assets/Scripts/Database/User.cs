using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public string userName;
    public int koritos;

    public User(string userName, int koritos)
    {
        this.userName = userName;
        this.koritos = koritos;
    }
}

