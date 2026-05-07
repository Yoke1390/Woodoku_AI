using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class BoardUI : MonoBehaviour
{
    [SerializeField]
    private Cell cellPrefab;

    private List<Cell> cellList = new List<Cell>();
    private GridLayoutGroup gridLayout;
    private float cellSize;

    private const int GRID_SIZE = 9;

    private void Start()
    {
        gridLayout = GetComponent<GridLayoutGroup>();

        // UIのレイアウト計算（Horizontal Layout Groupなど）を強制的に完了させる
        Canvas.ForceUpdateCanvases();

        AdjustCellSize();
        InitializeBoard();

        // test
        UpdateCellState(0, 1, true);
        UpdateCellState(2, 7, true);
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < 81; i++)
        {
            Cell newCell = Instantiate(cellPrefab, gameObject.transform);
            newCell.Hide();
            cellList.Add(newCell);
        }
    }

    private void AdjustCellSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Debug.Log(rectTransform.rect.width);
        Debug.Log(rectTransform.rect.height);
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

        Debug.Log(availableWidth);
        Debug.Log(availableHeight);
        cellSize = Mathf.Min(availableWidth, availableHeight) / GRID_SIZE;
        Debug.Log(cellSize);

        gridLayout.cellSize = new Vector2(cellSize, cellSize);
    }

    public void UpdateCellState(int x, int y, bool isFilled)
    {
        int index = y * 9 + x;
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
