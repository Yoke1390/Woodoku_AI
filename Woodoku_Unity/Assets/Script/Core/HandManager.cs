using System;
using System.Collections.Generic;
using System.Linq;
using Script.Core.Interfaces;
using Script.Core.Primitive;

namespace Script.Core
{
    public class HandManager : IReadOnlyHands
    {
        private readonly BlockShape[] _blockShapes;

        private readonly BlockShape?[] _currentHand;
        private readonly int _randomSeed;
        private Random _random;

        public HandManager(IEnumerable<BlockShape> blockShapes, int nHandSlots, int randomSeed)
        {
            _blockShapes = blockShapes.ToArray();
            if (_blockShapes.Length == 0) throw new ArgumentException("No Block Shapes passed", nameof(_blockShapes));

            _randomSeed = randomSeed;

            if (nHandSlots > 0)
            {
                NSlots = nHandSlots;
                _currentHand = new BlockShape?[nHandSlots];
            }
            else
            {
                throw new ArgumentException(
                    "Number of Hand Blocks must be positive",
                    nameof(nHandSlots)
                );
            }
        }

        public IReadOnlyList<BlockShape?> CurrentHand => _currentHand;
        public int NSlots { get; }
        public event Action<int> HandBlockConsumed;
        public event Action<int, BlockShape> HandBlockGenerated;
        public event Action HandSettled;

        public void Reset(int? newSeed = null)
        {
            _random = new Random(newSeed ?? _randomSeed);
            ConsumeAllHand();
            GenerateAll();
        }

        private BlockShape GetRandomBlockShape()
        {
            if (_random == null) throw new InvalidOperationException("random generator must be initialized");
            return _blockShapes[_random.Next(0, _blockShapes.Length)];
        }

        private void GenerateAll()
        {
            for (var i = 0; i < NSlots; i++)
            {
                var blockShape = GetRandomBlockShape();
                _currentHand[i] = blockShape;
                HandBlockGenerated?.Invoke(i, blockShape);
            }
        }

        private void ConsumeAllHand()
        {
            for (var i = 0; i < NSlots; i++)
                if (_currentHand[i].HasValue)
                {
                    _currentHand[i] = null;
                    HandBlockConsumed?.Invoke(i);
                }
        }

        public void CommitPlacement(int slotIndex)
        {
            _currentHand[slotIndex] = null;
            HandBlockConsumed?.Invoke(slotIndex);
            if (IsHandEmpty()) GenerateAll();
            HandSettled?.Invoke();
        }

        private bool IsHandEmpty()
        {
            return Array.TrueForAll(_currentHand, hand => !hand.HasValue);
        }
    }
}