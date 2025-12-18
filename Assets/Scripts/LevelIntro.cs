using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelIntro : MonoBehaviour
{
    public Image fadePanel;
    public TextMeshProUGUI levelNameText;

    public float fadeDuration = 0.2f;
    public float textDuration = 0.5f;

    IEnumerator Start()
    {
        // 1. Poner el texto del nivel
        levelNameText.text = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // 2. Comenzar opaco
        fadePanel.color = new Color(0, 0, 0, 1);
        levelNameText.alpha = 1;

        // 3. Esperar con pantalla negra y texto visible
        yield return new WaitForSeconds(0.1f);

        // 4. Fade-out del negro
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = 1 - (t / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, a);
            yield return null;
        }

        // 5. Esperar un poco con el texto visible
        yield return new WaitForSeconds(textDuration);

        // 6. Fade-out del texto
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = 1 - (t / fadeDuration);
            levelNameText.alpha = a;
            yield return null;
        }
    }

    IEnumerator FadeEffect()
    {
        // FADE OUT a negro
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = t / fadeDuration;
            fadePanel.color = new Color(0, 0, 0, a);
            yield return new WaitForSeconds(15);
            yield return null;
        }
    }

    void OnEnable()
    {
        moveRectangle.OnFall += HandleFall;
        moveRectangle.OnRestart += HandleFall;
        MoveCube.OnFall += HandleFall;
    }

    void OnDisable()
    {
        moveRectangle.OnFall -= HandleFall;
        moveRectangle.OnRestart -= HandleFall;
        MoveCube.OnFall += HandleFall;
    }


    void HandleFall()
    {
        if (this == null || !gameObject.activeInHierarchy) //arelga un error que pasa con el cubo por algun motivo
            return;
        StartCoroutine(FadeEffect());
    }

    void HandleRestart()
    {
        FadeEffect();
    }
}
