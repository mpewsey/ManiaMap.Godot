using MPewsey.Common.Random;
using System;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Contains the random number generator used for ID and random seed assignments.
    /// </summary>
    public static class Rand
    {
        public static Random Random { get; } = new Random();

        /// <summary>
        /// If the specified ID is less than or equal to zero, returns a new random positive integer ID.
        /// Otherwise, returns the ID.
        /// </summary>
        /// <param name="id">The input ID.</param>
        public static int AutoAssignId(int id)
        {
            return id <= 0 ? GetRandomId() : id;
        }

        /// <summary>
        /// Returns a random positive integer ID.
        /// </summary>
        public static int GetRandomId()
        {
            return Random.Next(1, int.MaxValue);
        }

        /// <summary>
        /// Returns a new RandomSeed object for the given seed. If the specified seed is less than or equal to zero,
        /// a random positive integer seed is used instead.
        /// </summary>
        /// <param name="seed">The random seed.</param>
        public static RandomSeed CreateRandomSeed(int seed)
        {
            return new RandomSeed(seed <= 0 ? Random.Next(1, int.MaxValue) : seed);
        }
    }
}