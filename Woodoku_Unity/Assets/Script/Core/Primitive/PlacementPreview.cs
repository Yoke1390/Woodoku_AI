namespace Script.Core.Primitive
{
    public struct PlacementPreview
    {
        public readonly PlacementAction LastAction;
        public readonly BoardSnapShot Board;
        public readonly PlacementResult Result;
        public readonly int ScoreDiff;

        public PlacementPreview(PlacementAction lastAction, BoardSnapShot board, PlacementResult result, int scoreDiff)
        {
            LastAction = lastAction;
            Board = board;
            Result = result;
            ScoreDiff = scoreDiff;
        }
    }
}
