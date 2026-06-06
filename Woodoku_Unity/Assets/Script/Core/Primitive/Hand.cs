using System;
using System.Collections.Generic;

namespace Script.Core.Primitive
{
    public readonly struct Hand // : IReadOnlyHands
    {
        private readonly BlockShape?[] _slots;
        public IReadOnlyList<BlockShape?> CurrentHand => _slots;
        public int NSlots => _slots.Length;
        public bool IsEmpty => Array.TrueForAll(_slots, s => !s.HasValue);

        public Hand(IReadOnlyList<BlockShape> shapes)
        {
            var slots = new BlockShape?[shapes.Count];
            for (int i = 0; i < shapes.Count; i++) slots[i] = shapes[i];

            _slots = slots;
        }

        private Hand(BlockShape?[] slots)
        {
            _slots = slots;
        }

        public Hand Consume(int slot)
        {
            if (slot < 0 || slot >= _slots.Length)
                throw new ArgumentOutOfRangeException(nameof(slot), $"Slot {slot} does not exist.");

            if (!_slots[slot].HasValue)
                throw new ArgumentException($"Cannot consume empty slot {slot}", nameof(slot));

            var newSlots = (BlockShape?[])_slots.Clone();
            newSlots[slot] = null;

            return new Hand(newSlots);
        }
    }
}
