namespace Script.Core.Primitive
{
    public readonly struct PlacementAction
    {
        public BoardPosition BasePosition { get; }
        public BlockShape Shape { get; }

        public PlacementAction(BoardPosition basePosition, BlockShape shape)
        {
            BasePosition = basePosition;
            Shape = shape;
        }
    }
}