using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BotoRodo : MonoBehaviour
{
    float detectDistance = 2.0f;
    public GameObject tilePont;
    bool active;
     bool isPressed = false;  // Si el cub està actualment a sobre del botó
    bool wasPressed = false; // Si el cub estava a sobre en el frame anterior
    int scene;
    GameObject[] pont;
    // Start is called before the first frame update

    void Start()
    {
        active = false;
        if (SceneManager.GetActiveScene().name == "level4")
        {
            scene = 4;
            pont = new GameObject[2];
        }
            
    }

    void Scene4ButtonActivate()
    {
        pont[0] = Instantiate(tilePont, new Vector3(7, -0.05f, 9), transform.rotation);
        pont[0].transform.parent = transform;

        pont[1] = Instantiate(tilePont, new Vector3(8, -0.05f, 9), transform.rotation);
        pont[1].transform.parent = transform;

        active = true;
    }
    void Scene4ButtonDeactivate()
    {
        if (pont[0] != null) Destroy(pont[0]);
        if (pont[1] != null) Destroy(pont[1]);

        active = false;
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        isPressed = Physics.Raycast(transform.position, Vector3.up, out hit, detectDistance) && hit.collider.GetComponent<moveRectangle>() != null;
        //Debug.Log(wasPressed);
        if (isPressed && !wasPressed)
        {
            // Intentamos obtener el script del cubo
            moveRectangle cube = hit.collider.GetComponent<moveRectangle>();
            if (scene == 4) 
            {
                if (!active) Scene4ButtonActivate();
                else Scene4ButtonDeactivate();
            }
        }
        wasPressed = isPressed;
    }
}
