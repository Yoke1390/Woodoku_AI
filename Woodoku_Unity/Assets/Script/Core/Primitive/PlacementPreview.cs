namespace Script.Core.Primitive
{
    public readonly struct PlacementPreview
    {
        public readonly BoardSnapShot Board;
        public readonly PlacementAction LastAction;
        public readonly PlacementResult Result;
        public readonly int ScoreDiff;

        public PlacementPreview(BoardSnapShot board, PlacementAction lastAction, PlacementResult result, int scoreDiff)
        {
            LastAction = lastAction;
            Board = board;
            Result = result;
            ScoreDiff = scoreDiff;
        }

        public static PlacementPreview Failure(BoardSnapShot board, PlacementAction action)
        {
            return new PlacementPreview(
                board,
                action,
                PlacementResult.Failure(action.Shape),
                0
            );
        }
    }
}
