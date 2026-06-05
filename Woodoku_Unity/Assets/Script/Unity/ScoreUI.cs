using TMPro;
using UnityEngine;

namespace Script.Unity
{
    public class ScoreUI : MonoBehaviour
    {
        private const string ScoreTextPrefix = "Score: ";

        [SerializeField] private TextMeshProUGUI scoreText;

        public void UpdateScore(int score)
        {
            scoreText.SetText(ScoreTextPrefix + score);
        }
    }
}