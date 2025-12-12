using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class BotoDividir : MonoBehaviour
{
    public Vector3 ubicMeitat1 = Vector3.zero;
    public Vector3 ubicMeitat2 = Vector3.one;
    public GameObject meitat1;
    public GameObject meitat2;

    bool summoned = false;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        summoned = false;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 1.0f))
        {
            // Intentamos obtener el script del cubo
            moveRectangle cube = hit.collider.GetComponent<moveRectangle>();

            if (cube != null)        // si lo tiene, es el cubo
            {
                if (!summoned) { 
                    summoned=true;
                    cube.selfdestroy();
                    Instantiate(meitat1, ubicMeitat1 + new Vector3(0,0.55f,0), transform.rotation);
                    Instantiate(meitat2, ubicMeitat2 + new Vector3(0, 0.55f, 0), transform.rotation);
                }
            }
        }
        if (summoned) //poder reutilziar el bloque separador
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                summoned = false;
            }
        }
        
    }
}
