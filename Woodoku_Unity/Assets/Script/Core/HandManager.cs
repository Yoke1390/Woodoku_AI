using System;
using System.Collections.Generic;
using System.Linq;

public class HandManager : IReadOnlyHands
{
    private Random random;
    private readonly BlockShape[] _blockShapes;

    private BlockShape?[] _currentHand;
    public IReadOnlyList<BlockShape?> CurrentHand => _currentHand;

    public event Action HandSettled;
    public event Action<int, BlockShape> HandBlockGenerated;

    public HandManager(IEnumerable<BlockShape> blockShapes, int NHandSlots, int randomSeed)
    {
        _blockShapes = blockShapes.ToArray();
        if (_blockShapes.Length == 0)
        {
            throw new ArgumentException("No Block Shapes passed", nameof(_blockShapes));
        }

        random = new(randomSeed);

        if (NHandSlots > 0)
        {
            _currentHand = new BlockShape?[NHandSlots];
        }
        else
        {
            throw new ArgumentException(
                "Number of Hand Blocks must be popsitive",
                nameof(NHandSlots)
            );
        }
    }

    private BlockShape GetRandomBlockShape()
    {
        return _blockShapes[random.Next(0, _blockShapes.Length)];
    }

    public void Begin() => GenerateAll();

    private void GenerateAll()
    {
        for (int i = 0; i < _currentHand.Length; i++)
        {
            BlockShape blockShape = GetRandomBlockShape();
            _currentHand[i] = blockShape;
            HandBlockGenerated?.Invoke(i, blockShape);
        }
    }

    public void CommitPlacement(int slotIndex)
    {
        _currentHand[slotIndex] = null;
        if (IsHandEmpty())
        {
            GenerateAll();
        }
        HandSettled?.Invoke();
    }

    private bool IsHandEmpty()
    {
        return Array.TrueForAll(_currentHand, hand => !hand.HasValue);
    }
}
