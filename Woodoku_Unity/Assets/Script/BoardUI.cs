using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(RectTransform))]
public class BoardUI : MonoBehaviour
{
    private BoardData boardData;

    [SerializeField]
    private Cell cellPrefab;

    private List<Cell> cellList = new List<Cell>();
    public float CellSize { get; private set; }

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayout;

    public void Initialize(BoardData boardData)
    {
        this.boardData = boardData;
        rectTransform = GetComponent<RectTransform>();
        gridLayout = GetComponent<GridLayoutGroup>();

        // XY座標の向きを合わせる (x：右が正, y：上が正)
        gridLayout.startCorner = GridLayoutGroup.Corner.LowerLeft;

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = boardData.BoardSize;

        // UIのレイアウト計算（Horizontal Layout Groupなど）を強制的に完了させる
        Canvas.ForceUpdateCanvases();

        AdjustCellSize();
        InitializeCells();
    }

    private void InitializeCells()
    {
        int totalCellNumber = boardData.BoardSize * boardData.BoardSize;
        for (int i = 0; i < totalCellNumber; i++)
        {
            Cell newCell = Instantiate(cellPrefab, gameObject.transform);
            newCell.Hide();
            cellList.Add(newCell);
        }
    }

    private void AdjustCellSize()
    {
        int boardSize = boardData.BoardSize;
        float availableWidth =
            rectTransform.rect.width
            - gridLayout.padding.left
            - gridLayout.padding.right
            - (boardSize - 1) * gridLayout.spacing.x;
        float availableHeight =
            rectTransform.rect.height
            - gridLayout.padding.top
            - gridLayout.padding.bottom
            - (boardSize - 1) * gridLayout.spacing.y;

        CellSize = Mathf.Min(availableWidth, availableHeight) / boardSize;

        gridLayout.cellSize = new Vector2(CellSize, CellSize);
    }

    public void UpdateCellState(int x, int y, bool isFilled)
    {
        int index = y * boardData.BoardSize + x;
        if (isFilled)
        {
            cellList[index].Show();
        }
        else
        {
            cellList[index].Hide();
        }
    }

    internal void BoradData_OnCellUpdate(object sender, BoardData.CellUpdateData data)
    {
        bool isFilled = data.Value == 1;
        UpdateCellState(data.X, data.Y, isFilled);
    }

    public bool TryScreenPointToBoardPosition(
        Vector2 screenPoint,
        Camera cam,
        Vector2 centerCellOffset,
        out BoardPosition boardPosition
    )
    {
        Vector2 localOffset = centerCellOffset * CellSize;
        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPoint,
                cam,
                out Vector2 localPointerPosition
            )
        )
        {
            Vector2 screenLocalPosition = localPointerPosition - localOffset;

            int boardPositionX = Mathf.FloorToInt(screenLocalPosition.x / CellSize);
            int boardPositionY = Mathf.FloorToInt(screenLocalPosition.y / CellSize);

            boardPosition = new BoardPosition(boardPositionX, boardPositionY);
            return true;
        }

        boardPosition = new BoardPosition(-1, -1);
        return false;
    }
}
