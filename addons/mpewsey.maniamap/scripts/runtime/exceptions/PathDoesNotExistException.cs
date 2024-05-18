using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    public class PathDoesNotExistException : Exception
    {
        public PathDoesNotExistException(string message) : base(message)
        {

        }
    }
}