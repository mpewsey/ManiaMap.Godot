using System;

namespace MPewsey.ManiaMapGodot.Exceptions
{
    /// <summary>
    /// Raised if a room template has not been initialized.
    /// </summary>
    public class RoomTemplateNotInitializedException : Exception
    {
        public RoomTemplateNotInitializedException(string message) : base(message)
        {

        }
    }
}