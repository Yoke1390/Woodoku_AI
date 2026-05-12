using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPiece : MonoBehaviour
{
    [SerializeField]
    private GameObject sprite;

    private void Awake()
    {
        Show();
    }

    public void Show()
    {
        sprite.SetActive(true);
    }

    public void Hide()
    {
        sprite.SetActive(false);
    }
}
