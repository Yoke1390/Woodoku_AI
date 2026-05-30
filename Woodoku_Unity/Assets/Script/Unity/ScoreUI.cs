using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private const string ScoreTextPrefix = "Score: ";

    public void UpdateScore(int score)
    {
        scoreText.SetText(ScoreTextPrefix + score);
    }
}
