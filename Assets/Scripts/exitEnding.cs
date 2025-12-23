using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;

public class exitEnding : MonoBehaviour
{
    public void Exit()
    {
        SceneManager.LoadScene("menu");
    }
}
