using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    public class MissingInputException : Exception
    {
        public MissingInputException(string message) : base(message)
        {

        }
    }
}