using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private GameObject sprite;

    public void Show()
    {
        sprite.SetActive(true);
    }

    public void Hide()
    {
        sprite.SetActive(false);
    }
}
