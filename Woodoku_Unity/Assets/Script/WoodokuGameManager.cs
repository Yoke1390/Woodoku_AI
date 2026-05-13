using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodokuGameManager : MonoBehaviour
{
    [SerializeField]
    private BoardUI boardUI;

    [SerializeField]
    private HandManager handManager;

    void Start()
    {
        boardUI.Initialize();
        handManager.Initialize();
    }
}
