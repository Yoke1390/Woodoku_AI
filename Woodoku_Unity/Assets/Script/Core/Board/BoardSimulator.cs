using System.Collections.Generic;
using Script.Core.Primitive;

namespace Script.Core.Board
{
    public static class BoardSimulator
    {
        public static (BoardSnapShot newSnap, PlacementResult result)
            SimulatePlaceAndClear(BoardSnapShot snapShot, PlacementAction action)
        {
            BoardSnapShot filledSnap = snapShot.FillWith(action.FilledPositions);

            (int nClearedTimes, List<BoardPosition> cellsToClearList) = BoardRule.GetCellsToClear(filledSnap);

            BoardSnapShot clearedSnap = filledSnap.ClearWith(cellsToClearList);

            return (clearedSnap, new PlacementResult(true, action.Shape, nClearedTimes, cellsToClearList));
        }
    }
}
