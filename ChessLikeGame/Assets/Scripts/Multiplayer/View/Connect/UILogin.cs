using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILogin : MonoBehaviour
{
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private GameObject activateGameObject;

    public void Login()
    {
        HideDisplay();
    }

    public void ShowDisplay()
    {
        activateGameObject.SetActive(true);
    }

    public void HideDisplay()
    {
        activateGameObject.SetActive(false);
    }
}
