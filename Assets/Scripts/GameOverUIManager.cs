using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class GameOverUIManager : MonoBehaviour
{
    public static GameOverUIManager Instance { get; private set; }

    public GameObject gameOverPanel;
    public TMP_InputField nicknameInput;
    public TMP_Text leaderboardText;
    public TMP_Text warningText;
    public GameObject leaderboardPanel;
    public GameObject playAgainButton;

    private List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
    private string leaderboardFilePath;

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
        leaderboardFilePath = Path.Combine(Application.persistentDataPath, "leaderboard.dat");

        gameOverPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        playAgainButton.SetActive(false);

        LoadLeaderboard();

        // Attach the character limit handler to the input field's onValueChanged event
        nicknameInput.onValueChanged.AddListener(RestrictInputLength);
    }

    public void GameOver()
    {
        Time.timeScale = 0f; // Pause the game
        gameOverPanel.SetActive(true);
        leaderboardPanel.SetActive(true);
        playAgainButton.SetActive(true);
        DisplayLeaderboard();
    }

    public void SaveScore()
    {
        if (nicknameInput.text.Length == 3)
        {
            warningText.text = "";
            string nickname = nicknameInput.text.ToUpper();
            int score = FindObjectOfType<ScoreManager>().GetScore();
            leaderboard.Add(new LeaderboardEntry(nickname, score));

            // Sort leaderboard by score (descending)
            leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

            SaveLeaderboard();
            DisplayLeaderboard();
        }
        else
        {
            warningText.text = "Please input 3 letters and try again!";
        }
    }

    void DisplayLeaderboard()
    {
        leaderboardText.text = ""; 
        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboardText.text += $"{i + 1}. {leaderboard[i].nickname} - {leaderboard[i].score}\n";
        }
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f; // Resume the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void SaveLeaderboard()
    {
        LeaderboardData data = new LeaderboardData { entries = leaderboard };
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(leaderboardFilePath, FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }
    }

    private void LoadLeaderboard()
    {
        if (File.Exists(leaderboardFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(leaderboardFilePath, FileMode.Open))
            {
                LeaderboardData data = (LeaderboardData)formatter.Deserialize(stream);
                leaderboard = data.entries;
            }
        }
        else
        {
            leaderboard = new List<LeaderboardEntry>();
        }
    }
    
    private void RestrictInputLength(string input)
    {
        if (input.Length > 3)
        {
            nicknameInput.text = input.Substring(0, 3);
        }
    }
}
