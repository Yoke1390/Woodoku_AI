using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private GameObject group;

    [SerializeField]
    private Button restartButton;

    public event Action Restart;

    private void Start()
    {
        Hide();
        restartButton.onClick.AddListener(OnRestart);
    }

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
