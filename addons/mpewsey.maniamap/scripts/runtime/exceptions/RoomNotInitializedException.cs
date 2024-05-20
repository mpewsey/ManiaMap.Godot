using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    /// <summary>
    /// Raised if a room is not initialized when its on ready method is called.
    /// </summary>
    public class RoomNotInitializedException : Exception
    {
        public RoomNotInitializedException(string message) : base(message)
        {

        }
    }
}