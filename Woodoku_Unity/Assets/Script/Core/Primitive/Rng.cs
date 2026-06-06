using System;

namespace Script.Core.Primitive
{
    public readonly struct Rng
    {
        private readonly ulong _state;

        private Rng(ulong state)
        {
            _state = state;
        }

        public static Rng FromSeed(int seed)
        {
            return new Rng(unchecked((ulong)seed + 0x9E3779B97F4A7C15));
        }

        public (Rng next, int value) Next(int maxExclusive) // 0 <= value < maxExclusive
        {
            if (maxExclusive <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), "maxExclusive must be > 0");

            // splitmix64
            ulong z = _state + 0x9E3779B97F4A7C15;
            ulong n = z;
            n = (n ^ (n >> 30)) * 0xBF58476D1CE4E5B9;
            n = (n ^ (n >> 27)) * 0x94D049BB133111EB;
            n ^= n >> 31;
            return (new Rng(z), (int)(n % (ulong)maxExclusive));
        }
    }
}
