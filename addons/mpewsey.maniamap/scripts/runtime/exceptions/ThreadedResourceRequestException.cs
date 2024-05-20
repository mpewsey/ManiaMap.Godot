using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    /// <summary>
    /// Raised if an error is encountered while loading a resource asynchronously.
    /// </summary>
    public class ThreadedResourceRequestException : Exception
    {
        public ThreadedResourceRequestException(string message) : base(message)
        {

        }
    }
}