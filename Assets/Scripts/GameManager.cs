using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _startingSceneTransition;
    [SerializeField] private float startDuration = 1.0f;

    public void ExitToMainMenu()
    {
        StartCoroutine(ExitCoroutine());
    }

    private IEnumerator ExitCoroutine()
    {
        // 1️⃣ 放大（当前关卡）
        _startingSceneTransition.SetActive(true);
        yield return new WaitForSeconds(startDuration);

        // 2️⃣ 切换场景
        SceneManager.LoadScene(0);
    }
}
