using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSelectionManager : MonoBehaviour
{
    public MoveCube cube1;
    public MoveCube cube2;

    MoveCube current;

    void Start()
    {
        // Decide cuál empieza seleccionado
        current = cube1.selected ? cube1 : cube2;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Cambiar selección
            current.selected = false;

            if (current == cube1)
                current = cube2;
            else
                current = cube1;

            current.selected = true;
        }
    }
}
