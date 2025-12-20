using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _endingSceneTransition;
    [SerializeField] private float endDuration = 1.0f;

    private void Start()
    {
       
        _endingSceneTransition.SetActive(true);
        Invoke(nameof(DisableEnd), endDuration);
    }

    private void DisableEnd()
    {
        _endingSceneTransition.SetActive(false);
    }
}
