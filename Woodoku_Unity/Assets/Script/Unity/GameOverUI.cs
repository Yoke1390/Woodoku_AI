using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Unity
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject group;

        [SerializeField] private Button restartButton;

        private void Start()
        {
            Hide();
            restartButton.onClick.AddListener(OnRestart);
        }

        public event Action Restart;

        private void OnRestart()
        {
            Hide();
            Restart?.Invoke();
        }

        public void Hide()
        {
            group.SetActive(false);
        }

        public void Show()
        {
            group.SetActive(true);
        }
    }
}