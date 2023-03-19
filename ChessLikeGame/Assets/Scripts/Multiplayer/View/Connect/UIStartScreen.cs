using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartScreen : MonoBehaviour
{
    [SerializeField] private GameObject loginDisplay;
    [SerializeField] private GameObject hostGameDisplay;
    [SerializeField] private GameObject joinGameDisplay;
    [SerializeField] private GameObject loginButton;
    [SerializeField] private GameObject hostGameButton;
    [SerializeField] private GameObject joinGameButton;
    [SerializeField] private GameObject startMenuDisplay;

    public void ShowDisplay()
    {
        startMenuDisplay.SetActive(true);
    }
    
    public void HideDisplay()
    {
        startMenuDisplay.SetActive(false);
    }

    public void Login()
    {
        loginDisplay.SetActive(true);
    }

    public void HostGame()
    {
        hostGameDisplay.SetActive(true);
    }

    public void JoinGame()
    {
        joinGameDisplay.SetActive(true);
    }
}