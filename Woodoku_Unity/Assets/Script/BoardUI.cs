using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class BoardUI : MonoBehaviour
{
    public static BoardUI Instance { get; private set; }

    [SerializeField]
    private Cell cellPrefab;

    private List<Cell> cellList = new List<Cell>();
    private GridLayoutGroup gridLayout;
    public float CellSize { get; private set; }

    private const int GRID_SIZE = 9;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void Initialize()
    {
        gridLayout = GetComponent<GridLayoutGroup>();

        // UIのレイアウト計算（Horizontal Layout Groupなど）を強制的に完了させる
        Canvas.ForceUpdateCanvases();

        AdjustCellSize();
        InitializeCells();

        // test
        UpdateCellState(0, 1, true);
        UpdateCellState(2, 7, true);
    }

    private void InitializeCells()
    {
        for (int i = 0; i < GRID_SIZE * GRID_SIZE; i++)
        {
            Cell newCell = Instantiate(cellPrefab, gameObject.transform);
            newCell.Hide();
            cellList.Add(newCell);
        }
    }

    private void AdjustCellSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float availableWidth =
            rectTransform.rect.width
            - gridLayout.padding.left
            - gridLayout.padding.right
            - (GRID_SIZE - 1) * gridLayout.spacing.x;
        float availableHeight =
            rectTransform.rect.height
            - gridLayout.padding.top
            - gridLayout.padding.bottom
            - (GRID_SIZE - 1) * gridLayout.spacing.y;

        CellSize = Mathf.Min(availableWidth, availableHeight) / GRID_SIZE;

        gridLayout.cellSize = new Vector2(CellSize, CellSize);
    }

    public void UpdateCellState(int x, int y, bool isFilled)
    {
        int index = y * GRID_SIZE + x;
        if (isFilled)
        {
            cellList[index].Show();
        }
        else
        {
            cellList[index].Hide();
        }
    }
}
