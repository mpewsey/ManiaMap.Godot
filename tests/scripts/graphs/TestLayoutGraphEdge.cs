using GdUnit4;

namespace MPewsey.ManiaMapGodot.Graphs.Tests
{
    [TestSuite]
    public class TestLayoutGraphEdge
    {
        [TestCase]
        public void TestContainsNode()
        {
            var edge = new LayoutGraphEdge(1, 2);
            Assertions.AssertThat(edge.ContainsNode(1)).IsTrue();
            Assertions.AssertThat(edge.ContainsNode(2)).IsTrue();
            Assertions.AssertThat(edge.ContainsNode(3)).IsFalse();
        }
    }
}