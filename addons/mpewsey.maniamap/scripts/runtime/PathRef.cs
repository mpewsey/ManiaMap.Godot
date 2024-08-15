using Godot;

namespace MPewsey.ManiaMapGodot
{
    public readonly struct PathRef
    {
        public string UidPath { get; }
        public string ResPath { get; }

        public PathRef(string uidPath, string resPath)
        {
            UidPath = uidPath;
            ResPath = resPath;
        }

        public string GetLoadPath()
        {
            return ResourceLoader.Exists(ResPath) ? ResPath : UidPath;
        }

        public T Load<T>() where T : class
        {
            return ResourceLoader.Load<T>(GetLoadPath());
        }
    }
}