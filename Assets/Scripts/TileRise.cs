using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileRise : MonoBehaviour
{
    public float minTime = 0.4f;  // temps mínim
    public float maxTime = 0.8f;  // temps màxim
    public float startOffset = -10f; // posició y abans de pujar
    float duration;
    Vector3 finalPos;

    void Start()
    {
        // Guardem la posició final
        finalPos = transform.position;

        // Baixem el tile
        transform.position = new Vector3(finalPos.x, finalPos.y + startOffset, finalPos.z);

        // Iniciem la pujada
        duration = Random.Range(minTime, maxTime);
        StartCoroutine(RiseCoroutine(duration));
    }

    IEnumerator RiseCoroutine(float t)
    {
        transform.position = new Vector3(finalPos.x, finalPos.y + startOffset, finalPos.z);
        Vector3 startPos = transform.position;
        float timer = 0f;

        while (timer < t)
        {
            timer += Time.deltaTime;
            float k = timer / t;

            // suavitzar el moviment (ease-out)
            k = Mathf.Sin(k * Mathf.PI * 0.5f);

            transform.position = Vector3.Lerp(startPos, finalPos, k);
            yield return null;
        }

        // Ens assegurem que arriba exactament a la posició final
        transform.position = finalPos;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            //duration = Random.Range(minTime, maxTime);
            StartCoroutine(RiseCoroutine(duration));
    }
}