using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    private float score;

    void Update()
    {
        score += Time.deltaTime;
        scoreText.text = "Score: " + Mathf.RoundToInt(score).ToString();
    }

    public int GetScore()
    {
        return (int)score;
    }
}
