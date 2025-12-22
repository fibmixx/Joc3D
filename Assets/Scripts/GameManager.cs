using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private float startDuration = 1.0f;

    
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip meowClip;

    public void ExitToMainMenu()
    {
        StartCoroutine(ExitCoroutine());
    }

    private IEnumerator ExitCoroutine()
    {
        if (sfxSource != null && meowClip != null)
        {
            sfxSource.PlayOneShot(meowClip);
        }

        _startingSceneTransition.SetActive(true);
        yield return new WaitForSeconds(startDuration);


        SceneManager.LoadScene(0);
        
    }
}
