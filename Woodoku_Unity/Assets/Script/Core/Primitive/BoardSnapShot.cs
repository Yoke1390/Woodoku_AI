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
            return this.IsInBoard(boardPosition)
                ? _board[boardPosition.x, boardPosition.y]
                : CellState.OutOfBoard;
        }

        public BoardSnapShot(int gridSize, CellState[,] board)
        {
            GridSize = gridSize;
            BoardSize = GridSize * GridSize;
            NGrids = BoardSize / GridSize;

            _board = new CellState[BoardSize, BoardSize];
            _board = (CellState[,])board.Clone();
        }
    }
}
