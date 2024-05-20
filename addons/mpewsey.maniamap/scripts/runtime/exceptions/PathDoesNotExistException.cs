using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    /// <summary>
    /// Raised if a path does not exist.
    /// </summary>
    public class PathDoesNotExistException : Exception
    {
        public PathDoesNotExistException(string message) : base(message)
        {

        }
    }
}