using System;
using System.Collections.Generic;
using Script.Core.Interfaces;

namespace Script.Core.Primitive
{
    public readonly struct BoardSnapShot : IReadOnlyBoard
    {
        public int GridSize { get; }
        public int BoardSize { get; }
        public int NGrids { get; }

        private readonly CellState[,] _board;

        public CellState GetCell(BoardPosition boardPosition)
        {
            return this.Contains(boardPosition)
                ? _board[boardPosition.x, boardPosition.y]
                : CellState.OutOfBoard;
        }

        public CellState GetCell(int x, int y)
        {
            return GetCell(new BoardPosition(x, y));
        }

        public BoardSnapShot(int gridSize, CellState[,] board)
        {
            GridSize = gridSize;
            BoardSize = GridSize * GridSize;
            NGrids = BoardSize / GridSize;

            _board = (CellState[,])board.Clone();
        }

        public BoardSnapShot FillWith(IReadOnlyCollection<BoardPosition> filled)
        {
            if (filled == null || filled.Count == 0) return this;

            var newBoard = (CellState[,])_board.Clone();
            foreach (BoardPosition pos in filled)
            {
                if (!this.Contains(pos))
                    throw new ArgumentOutOfRangeException(nameof(filled), $"Cannot fill outside of the board: {pos}");

                newBoard[pos.x, pos.y] = CellState.Filled;
            }

            return new BoardSnapShot(GridSize, newBoard);
        }

        public BoardSnapShot ClearWith(IReadOnlyCollection<BoardPosition> cleared)
        {
            if (cleared == null || cleared.Count == 0) return this;

            var newBoard = (CellState[,])_board.Clone();
            foreach (BoardPosition pos in cleared)
            {
                if (!this.Contains(pos))
                    throw new ArgumentOutOfRangeException(nameof(cleared), $"Cannot clear outside of the board: {pos}");

                newBoard[pos.x, pos.y] = CellState.Empty;
            }

            return new BoardSnapShot(GridSize, newBoard);
        }
    }
}
