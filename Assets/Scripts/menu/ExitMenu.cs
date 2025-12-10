using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Diagnostics;


public class ExitMenu : MonoBehaviour
{
    public GameObject exitPopup;

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
        SceneManager.LoadScene(0);

    }
}
