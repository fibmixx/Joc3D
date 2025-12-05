using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine;


public class BotoCreu : MonoBehaviour
{
    public GameObject pont;
    public GameObject pont2;
    bool activat = false;

    public void TogglePont()
    {
        activat = !activat;          

        if (pont != null && pont2 != null){
            pont.SetActive(activat);
            pont2.SetActive(activat);
        }
    }
}
