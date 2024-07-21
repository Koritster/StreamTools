using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Avatars : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txt_user;

    public void ChangeName(string userName)
    {
        txt_user.text = userName;
    }
}
