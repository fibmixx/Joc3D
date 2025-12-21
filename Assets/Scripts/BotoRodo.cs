using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine;


public class BotoRodo : MonoBehaviour
{
    public GameObject pont;
    public GameObject pont2;
    bool activat = false;
    bool isPressed = false;
    bool wasPressed = false;

    public void TogglePont()
    {
        activat = !activat;

        if (pont != null && pont2 != null)
        {
            pont.SetActive(activat);
            pont2.SetActive(activat);
        }
    }

    void Update()
    {
        RaycastHit hit;
        isPressed = Physics.Raycast(transform.position, Vector3.up, out hit, 2f);
        if (isPressed && !wasPressed)
        {   
            TogglePont();
        }
        wasPressed = isPressed;
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
