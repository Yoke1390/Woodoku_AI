using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[RequireComponent(typeof(RectTransform))]
public class BoardUI : MonoBehaviour
{
    public static BoardUI Instance { get; private set; }

    public BoardData boardData;

    [SerializeField]
    private Cell cellPrefab;

    private List<Cell> cellList = new List<Cell>();
    public float CellSize { get; private set; }

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayout;

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
        rectTransform = GetComponent<RectTransform>();
        gridLayout = GetComponent<GridLayoutGroup>();

        // XY座標の向きを合わせる (x：右が正, y：上が正)
        gridLayout.startCorner = GridLayoutGroup.Corner.LowerLeft;

        // UIのレイアウト計算（Horizontal Layout Groupなど）を強制的に完了させる
        Canvas.ForceUpdateCanvases();

        AdjustCellSize();
        InitializeCells();
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

    // return BoardPosition for (0,0) of BoardData
    public BoardPosition? GetBlockBaseBoardPosition(
        PointerEventData eventData,
        Vector2 centerCellOffset
    )
    {
        Vector2 localOffset = centerCellOffset * CellSize;
        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPointerPosition
            )
        )
        {
            Vector2 blockBaseScreenLocalPosition = localPointerPosition - localOffset;

            int blockBaseBoardPositionX = Mathf.FloorToInt(
                blockBaseScreenLocalPosition.x / CellSize
            );
            int blockBaseBoardPositionY = Mathf.FloorToInt(
                blockBaseScreenLocalPosition.y / CellSize
            );

            if (
                BoardPosition.IsValid(
                    new Vector2Int(blockBaseBoardPositionX, blockBaseBoardPositionY)
                )
            )
            {
                return new BoardPosition(blockBaseBoardPositionX, blockBaseBoardPositionY);
            }
        }

        Debug.LogWarning("Block Base out of range");
        return null;
    }

    internal void BoradData_OnCellUpdate(object sender, BoardData.CellUpdateData data)
    {
        bool isFilled = data.Value == 1;
        UpdateCellState(data.X, data.Y, isFilled);
    }
}
