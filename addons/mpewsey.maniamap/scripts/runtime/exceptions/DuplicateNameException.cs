using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    public class DuplicateNameException : Exception
    {
        public DuplicateNameException(string message) : base(message)
        {

        }
    }
}