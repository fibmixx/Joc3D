using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntrarEntrada : MonoBehaviour
{
    float detectDistance = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        

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
                if (cube.state == 0 && cube.bMoving == false) // comprobamos el state y que no se este moviendo
                {
                    cube.win = true;
                }
            }
        }
    }
}
