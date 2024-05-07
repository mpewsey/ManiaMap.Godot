using MPewsey.Common.Random;
using System;

namespace MPewsey.ManiaMapGodot
{
    public static class Rand
    {
        public static Random Random { get; } = new Random();

        public static int AutoAssignId(int id)
        {
            return id <= 0 ? GetRandomId() : id;
        }

        public static int GetRandomId()
        {
            return Random.Next(1, int.MaxValue);
        }

        public static RandomSeed CreateRandomSeed(int seed)
        {
            return new RandomSeed(seed <= 0 ? Random.Next(1, int.MaxValue) : seed);
        }
    }
}