using System;
using System.Collections.Generic;
using Script.Core.Primitive;

namespace Script.Core.Interfaces
{
    public interface IReadOnlyHands
    {
        IReadOnlyList<BlockShape?> CurrentHand { get; }
        int NSlots { get; }
        event Action<int> HandBlockConsumed;
        event Action<int, BlockShape> HandBlockGenerated;
    }

    public interface IReadOnlyScore
    {
        int Score { get; }
        event Action<int> ScoreUpdate;
    }
}
