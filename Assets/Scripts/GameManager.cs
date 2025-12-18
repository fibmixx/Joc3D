using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int movesCount;
    public TextMeshProUGUI movesText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            movesCount = 0;
            PlayerPrefs.SetInt("MovesSaved", 0);
        }
        else
        {
            movesCount = PlayerPrefs.GetInt("MovesSaved", 0);
        }

        UpdateUI();
    }

    public void AddMove()
    {
        movesCount++;
        PlayerPrefs.SetInt("MovesSaved", movesCount);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (movesText != null)
            movesText.text = "Moves: " + movesCount;
    }
}
