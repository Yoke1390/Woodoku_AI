namespace Script.Core.Primitive
{
    public readonly struct PlacementAction
    {
        public BoardPosition BasePosition { get; }
        public BlockShape Shape { get; }

        public BoardPosition[] FilledPositions { get; }

        public PlacementAction(BoardPosition basePosition, BlockShape shape)
        {
            BasePosition = basePosition;
            Shape = shape;

            FilledPositions = new BoardPosition[Shape.NBlocks];
            for (int i = 0; i < Shape.NBlocks; i++) FilledPositions[i] = basePosition + Shape.Blocks[i];
        }
    }
}
