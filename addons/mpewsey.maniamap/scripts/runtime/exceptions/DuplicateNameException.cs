using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    /// <summary>
    /// Raised if a duplicate name is encountered.
    /// </summary>
    public class DuplicateNameException : Exception
    {
        public DuplicateNameException(string message) : base(message)
        {

        }
    }
}