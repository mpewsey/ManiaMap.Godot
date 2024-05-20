using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    /// <summary>
    /// Raised if an input is missing.
    /// </summary>
    public class MissingInputException : Exception
    {
        public MissingInputException(string message) : base(message)
        {

        }
    }
}