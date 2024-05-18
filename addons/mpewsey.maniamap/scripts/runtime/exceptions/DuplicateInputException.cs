using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    public class DuplicateInputException : Exception
    {
        public DuplicateInputException(string message) : base(message)
        {

        }
    }
}