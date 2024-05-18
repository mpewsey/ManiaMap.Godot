using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    public class RoomNotInitializedException : Exception
    {
        public RoomNotInitializedException(string message) : base(message)
        {

        }
    }
}