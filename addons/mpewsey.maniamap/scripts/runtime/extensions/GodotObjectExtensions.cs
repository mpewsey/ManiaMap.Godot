using Godot;
using System;

namespace MPewsey.ManiaMapGodot.Extensions
{
    public static class GodotObjectExtensions
    {
        public static void QuietDisconnect(this GodotObject obj, StringName signalName, Action action)
        {
            var callable = Callable.From(action);

            if (obj.IsConnected(signalName, callable))
                obj.Disconnect(signalName, callable);
        }

        public static void QuietDisconnect<T0>(this GodotObject obj, StringName signalName, Action<T0> action)
        {
            var callable = Callable.From(action);

            if (obj.IsConnected(signalName, callable))
                obj.Disconnect(signalName, callable);
        }
    }
}