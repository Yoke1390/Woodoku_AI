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

    [SerializeField]
    private Color backgroundColor1;

    [SerializeField]
    private Color backgroundColor2;

    [SerializeField]
    private Color defaultBorderColor;

    [SerializeField]
    private Color highlightBorderColor;

    private List<Cell> cellList = new List<Cell>();
    public float CellSize { get; private set; }

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayout;

    public void Initialize(BoardData boardData)
    {
        this.boardData = boardData;

        rectTransform = GetComponent<RectTransform>();
        rectTransform.pivot = Vector2.zero;

        gridLayout = GetComponent<GridLayoutGroup>();
        // XY座標の向きを合わせる (x：右が正, y：上が正)
        gridLayout.startCorner = GridLayoutGroup.Corner.LowerLeft;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = boardData.BoardSize;
        gridLayout.padding = new RectOffset(0, 0, 0, 0);
        gridLayout.spacing = Vector2.zero;
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
            SetCellColor(i, newCell);
            newCell.Hide();
            cellList.Add(newCell);
        }
    }

    private void SetCellColor(int index, Cell cell)
    {
        int x = index % boardData.BoardSize;
        int y = index / boardData.BoardSize;

        Color backgroundColor = GetBackgroundColor(x, y);
        cell.SetBackgroundColor(backgroundColor);

        cell.InitializeBorder(defaultBorderColor);

        if ((x + 1) % boardData.GridSize == 0)
        {
            cell.HighlightRightBorder(highlightBorderColor);
        }
        if ((y + 1) % boardData.GridSize == 0)
        {
            cell.HighlightTopBorder(highlightBorderColor);
        }

        if ((x + 1) == boardData.BoardSize)
        {
            cell.HideRightBorder();
        }
        if ((y + 1) == boardData.BoardSize)
        {
            cell.HideTopBorder();
        }
    }

    private Color GetBackgroundColor(int x, int y)
    {
        int gridX = x / boardData.GridSize;
        int gridY = y / boardData.GridSize;

        if ((gridX + gridY) % 2 == 0)
        {
            return backgroundColor1;
        }
        else
        {
            return backgroundColor2;
        }
    }

    private void AdjustCellSize()
    {
        CellSize =
            Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) / boardData.BoardSize;

        gridLayout.cellSize = CellSize * Vector2.one;
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

    internal void BoardData_OnCellUpdate(object sender, BoardData.CellUpdateData data)
    {
        bool isFilled = data.State == BoardData.CellState.Filled;
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

        boardPosition = default(BoardPosition);
        return false;
    }
}
