using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void ExitGame()
    {
        UnityEngine.Debug.Log("QUIT!");
        Application.Quit();
    }

    public void Credits()
    {
        SceneManager.LoadScene("menu");
    }
}
