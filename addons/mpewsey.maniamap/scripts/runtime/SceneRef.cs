using Godot;

namespace MPewsey.ManiaMapGodot
{
    public readonly struct SceneRef
    {
        public string UidPath { get; }
        public string ResPath { get; }

        public SceneRef(string uidPath, string resPath)
        {
            UidPath = uidPath;
            ResPath = resPath;
        }

        public string GetLoadPath()
        {
            return ResourceLoader.Exists(UidPath) ? UidPath : ResPath;
        }

        public T Load<T>() where T : class
        {
            return ResourceLoader.Load<T>(GetLoadPath());
        }
    }
}