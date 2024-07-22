using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar : MonoBehaviour
{
    [SerializeField] private Text txt_user;

    public void ChangeName(string userName)
    {
        txt_user.text = userName;
    }
}
