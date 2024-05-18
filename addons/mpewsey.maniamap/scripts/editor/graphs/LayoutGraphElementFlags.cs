#if TOOLS
namespace MPewsey.ManiaMapGodot.Graphs.Editor
{
    public enum LayoutGraphElementFlags
    {
        Node = 1 << 0,
        Edge = 1 << 1,
        All = Node | Edge,
    }
}
#endif