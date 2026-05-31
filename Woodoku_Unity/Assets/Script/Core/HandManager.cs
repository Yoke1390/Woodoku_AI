using System;
using System.Collections.Generic;
using System.Linq;

public class HandManager : IReadOnlyHands
{
    private readonly int _randomSeed;
    private Random random;
    private readonly BlockShape[] _blockShapes;

    private BlockShape?[] _currentHand;

    public IReadOnlyList<BlockShape?> CurrentHand => _currentHand;
    public int NSlots { get; }
    public event Action HandSettled;
    public event Action<int> HandBlockConsumed;
    public event Action<int, BlockShape> HandBlockGenerated;

    public HandManager(IEnumerable<BlockShape> blockShapes, int NHandSlots, int randomSeed)
    {
        _blockShapes = blockShapes.ToArray();
        if (_blockShapes.Length == 0)
        {
            throw new ArgumentException("No Block Shapes passed", nameof(_blockShapes));
        }

        _randomSeed = randomSeed;

        if (NHandSlots > 0)
        {
            NSlots = NHandSlots;
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

    public void Reset(int? newSeed = null)
    {
        random = new(newSeed ?? _randomSeed);
        ConsumeAllHand();
        GenerateAll();
    }

    private BlockShape GetRandomBlockShape()
    {
        if (random == null)
        {
            throw new InvalidOperationException("random generator must be initialized");
        }
        return _blockShapes[random.Next(0, _blockShapes.Length)];
    }

    private void GenerateAll()
    {
        for (int i = 0; i < NSlots; i++)
        {
            BlockShape blockShape = GetRandomBlockShape();
            _currentHand[i] = blockShape;
            HandBlockGenerated?.Invoke(i, blockShape);
        }
    }

    private void ConsumeAllHand()
    {
        for (int i = 0; i < NSlots; i++)
        {
            if (_currentHand[i].HasValue)
            {
                _currentHand[i] = null;
                HandBlockConsumed?.Invoke(i);
            }
        }
    }

    public void CommitPlacement(int slotIndex)
    {
        _currentHand[slotIndex] = null;
        HandBlockConsumed?.Invoke(slotIndex);
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
