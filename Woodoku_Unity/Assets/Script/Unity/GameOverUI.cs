using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private GameObject group;

    private void Start()
    {
        group.SetActive(false);
    }

    public void Show()
    {
        group.SetActive(true);
    }
}
