using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Diagnostics;


public class ExitMenu : MonoBehaviour
{
    public GameObject exitPopup;
    [SerializeField] private GameManager gameManager;

    public void OpenPopup()
    {
        exitPopup.SetActive(true);
    }

    public void ClosePopup()
    {
        exitPopup.SetActive(false);
    }

    public void ConfirmExit()
    {

        
        PlayerPrefs.SetInt("MovesSaved", 0); // reiniciar comptador

        UnityEngine.Debug.Log("boto!");
        exitPopup.SetActive(false);
        gameManager.ExitToMainMenu();
        UnityEngine.Debug.Log("tornant");
    }
}
