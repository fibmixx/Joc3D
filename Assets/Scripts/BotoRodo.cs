using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;


public class BotoRodo : MonoBehaviour
{
    public GameObject pont;
    public GameObject pont2;
    bool activat = false;
    
   

    public void TogglePont()
    {
        activat = !activat;

        if (pont != null) pont.SetActive(activat);
        if (pont2 != null) pont2.SetActive(activat);
    }

    void Update()
    {
        
    }

    void OnEnable()
    {
        moveRectangle.OnRestart += HandleRestart;
    }

    void OnDisable()
    {
        moveRectangle.OnRestart -= HandleRestart;
    }

    void HandleRestart()
    {
        activat = true;
        TogglePont();
    }
}
