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
    int scene;
    GameObject[] pont;
    // Start is called before the first frame update
    void Start()
    {
        active = false;
        if (SceneManager.GetActiveScene().name == "level4")
        {
            scene = 4;
        }
            
    }

    void Scene4ButtonActivate()
    {
        pont[1] = Instantiate(tilePont, new Vector3(7, -0.05f, 9), transform.rotation);
        pont[1].transform.parent = transform;

        pont[2] = Instantiate(tilePont, new Vector3(8, -0.05f, 9), transform.rotation);
        pont[2].transform.parent = transform;
        active = true;
    }
    void Scene4ButtonDeactivate()
    {
        pont[1] = Instantiate(tilePont, new Vector3(7, -0.05f, 9), transform.rotation);
        pont[1].transform.parent = transform;

        pont[2] = Instantiate(tilePont, new Vector3(8, -0.05f, 9), transform.rotation);
        pont[2].transform.parent = transform;
        active = true;
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, detectDistance))
        {
            // Intentamos obtener el script del cubo
            moveRectangle cube = hit.collider.GetComponent<moveRectangle>();

            if (cube != null)        // si lo tiene, es el cubo
            {   
                if (cube.bMoving == false && !active) // comprobamos el state y que no se este moviendo
                {
                    if (scene == 4) Scene4ButtonActivate();

                }
                else if (cube.bMoving == false && active) // comprobamos el state y que no se este moviendo
                {
                    if (scene == 4) Scene4ButtonDeactivate();

                }
            }
        }
    }
}
