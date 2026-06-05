using System.Collections.Generic;
using Script.Core.Interfaces;
using Script.Core.Primitive;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Unity.Board
{
    [RequireComponent(typeof(GridLayoutGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class BoardUI : MonoBehaviour
    {
        [SerializeField] private Cell cellPrefab;

        [SerializeField] private Color backgroundColor1;

        [SerializeField] private Color backgroundColor2;

        [SerializeField] private Color defaultBorderColor;

        [SerializeField] private Color highlightBorderColor;

        private readonly List<Cell> _cellList = new();

        private IReadOnlyBoard _boardData;
        private GridLayoutGroup _gridLayout;

        private RectTransform _rectTransform;
        public float CellSize { get; private set; }

        public void Initialize(IReadOnlyBoard boardData, IBoardEventPublisher boardEvent)
        {
            _boardData = boardData;
            boardEvent.CellUpdate += BoardData_OnCellUpdate;

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.pivot = Vector2.zero;

            _gridLayout = GetComponent<GridLayoutGroup>();
            // XY座標の向きを合わせる (x：右が正, y：上が正)
            _gridLayout.startCorner = GridLayoutGroup.Corner.LowerLeft;
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = boardData.BoardSize;
            _gridLayout.padding = new RectOffset(0, 0, 0, 0);
            _gridLayout.spacing = Vector2.zero;
            // UIのレイアウト計算（Horizontal Layout Groupなど）を強制的に完了させる
            Canvas.ForceUpdateCanvases();

            AdjustCellSize();
            InitializeCells();
        }

        private void InitializeCells()
        {
            int totalCellNumber = _boardData.BoardSize * _boardData.BoardSize;
            for (int i = 0; i < totalCellNumber; i++)
            {
                Cell newCell = Instantiate(cellPrefab, gameObject.transform);
                SetCellColor(i, newCell);
                newCell.Hide();
                _cellList.Add(newCell);
            }
        }

        private void SetCellColor(int index, Cell cell)
        {
            int x = index % _boardData.BoardSize;
            int y = index / _boardData.BoardSize;

            Color backgroundColor = GetBackgroundColor(x, y);
            cell.SetBackgroundColor(backgroundColor);

            cell.InitializeBorder(defaultBorderColor);

            if ((x + 1) % _boardData.GridSize == 0) cell.HighlightRightBorder(highlightBorderColor);
            if ((y + 1) % _boardData.GridSize == 0) cell.HighlightTopBorder(highlightBorderColor);

            if (x + 1 == _boardData.BoardSize) cell.HideRightBorder();
            if (y + 1 == _boardData.BoardSize) cell.HideTopBorder();
        }

        private Color GetBackgroundColor(int x, int y)
        {
            int gridX = x / _boardData.GridSize;
            int gridY = y / _boardData.GridSize;

            if ((gridX + gridY) % 2 == 0) return backgroundColor1;

            return backgroundColor2;
        }

        private void AdjustCellSize()
        {
            CellSize =
                Mathf.Min(_rectTransform.rect.width, _rectTransform.rect.height) / _boardData.BoardSize;

            _gridLayout.cellSize = CellSize * Vector2.one;
        }

        public void UpdateCellState(int x, int y, bool isFilled)
        {
            int index = y * _boardData.BoardSize + x;
            if (isFilled)
                _cellList[index].Show();
            else
                _cellList[index].Hide();
        }

        internal void BoardData_OnCellUpdate(object sender, CellUpdateData data)
        {
            bool isFilled = data.State == CellState.Filled;
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
                    _rectTransform,
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

            boardPosition = default;
            return false;
        }
    }
}