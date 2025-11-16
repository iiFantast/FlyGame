using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score Settings")]
    public int scoreToWin = 5; // Очков для победы
    private int player1Score = 0;
    private int player2Score = 0;

    [Header("UI References")]
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public GameObject victoryPanel; // Панель с объявлением победителя
    public TextMeshProUGUI victoryText;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI();
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (victoryPanel != null && victoryPanel.activeSelf)
        {
            // Чтобы работало при Time.timeScale = 0
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                RestartGame();
            }
        }
    }

    // Добавить очко игроку
    public void AddScore(int playerNumber)
    {
        if (gameEnded) return;

        if (playerNumber == 1)
        {
            player1Score++;
            Debug.Log("Player 1 scored! Total: " + player1Score);
        }
        else if (playerNumber == 2)
        {
            player2Score++;
            Debug.Log("Player 2 scored! Total: " + player2Score);
        }

        UpdateScoreUI();
        CheckWinCondition();
    }

    // Обновить отображение счёта
    private void UpdateScoreUI()
    {
        if (player1ScoreText != null)
        {
            player1ScoreText.text = "P1: " + player1Score;
        }

        if (player2ScoreText != null)
        {
            player2ScoreText.text = "P2: " + player2Score;
        }
    }

    // Проверить условие победы
    private void CheckWinCondition()
    {
        if (player1Score >= scoreToWin)
        {
            EndGame("Player 1 Wins!");
        }
        else if (player2Score >= scoreToWin)
        {
            EndGame("Player 2 Wins!");
        }
    }

    // Завершить игру и показать победителя
    private void EndGame(string winnerMessage)
    {
        gameEnded = true;
        Debug.Log(winnerMessage);

        if (victoryPanel != null && victoryText != null)
        {
            victoryPanel.SetActive(true);
            victoryText.text = winnerMessage;
        }

        // Можно заморозить время или отключить управление
        Time.timeScale = 0f; // Останавливает игру
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Возобновляем время
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver(string playerName)
    {
        // Определяем, кто получает очко
        if (playerName == "Player1")
        {
            AddScore(2); // Player2 получает очко
        }
        else if (playerName == "Player2")
        {
            AddScore(1); // Player1 получает очко
        }
    }
}
