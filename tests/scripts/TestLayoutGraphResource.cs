using GdUnit4;
using Godot;

namespace MPewsey.ManiaMapGodot.Graphs.Tests
{
    [TestSuite]
    public class TestLayoutGraphResource
    {
        [TestCase]
        public void TestAddNode()
        {
            var graph = new LayoutGraphResource();
            var node = graph.AddNode(new Vector2(1, 2));
            Assertions.AssertThat(node != null).IsTrue();
            Assertions.AssertThat(node.Id).IsEqual(1);
            Assertions.AssertThat(node.Position).IsEqual(new Vector2(1, 2));
            Assertions.AssertThat(node.Name).IsEqual("Node 1");
            Assertions.AssertThat(graph.Nodes.Count).IsEqual(1);
            Assertions.AssertThat(graph.Nodes[node.Id] == node).IsTrue();
        }

        [TestCase]
        public void TestAddNodeThrowsExceptionForDuplicateId()
        {
            var graph = new LayoutGraphResource();
            var node = graph.AddNode(1, Vector2.Zero);
            Assertions.AssertThrown(() => graph.AddNode(node.Id, Vector2.Zero));
        }

        [TestCase]
        public void TestAddEdge()
        {
            var graph = new LayoutGraphResource();
            graph.AddNode(1, Vector2.Zero);
            graph.AddNode(2, Vector2.Zero);
            var edge = graph.AddEdge(1, 2);
            Assertions.AssertThat(edge != null).IsTrue();
            Assertions.AssertThat(edge.FromNode).IsEqual(1);
            Assertions.AssertThat(edge.ToNode).IsEqual(2);
            Assertions.AssertThat(edge.Name).IsEqual("Edge (1, 2)");
            Assertions.AssertThat(graph.Nodes.Count).IsEqual(2);
            Assertions.AssertThat(graph.Edges.Count).IsEqual(1);
            Assertions.AssertThat(graph.Edges[new Vector2I(1, 2)] == edge).IsTrue();
        }

        [TestCase]
        public void TestAddEdgeReturnsNullForDuplicateEdge()
        {
            var graph = new LayoutGraphResource();
            graph.AddNode(1, Vector2.Zero);
            graph.AddNode(2, Vector2.Zero);
            var edge = graph.AddEdge(1, 2);
            Assertions.AssertThat(edge != null).IsTrue();
            var edge1 = graph.AddEdge(1, 2);
            Assertions.AssertThat(edge1 == null).IsTrue();
            var edge2 = graph.AddEdge(2, 1);
            Assertions.AssertThat(edge2 == null).IsTrue();
        }

        [TestCase]
        public void TestAddEdgeThrowsExceptionForNonExistentNodes()
        {
            var graph = new LayoutGraphResource();
            graph.AddNode(1, Vector2.Zero);
            Assertions.AssertThrown(() => graph.AddEdge(1, 2));
            Assertions.AssertThrown(() => graph.AddEdge(2, 1));
        }

        [TestCase]
        public void TestGetEdge()
        {
            var graph = new LayoutGraphResource();
            graph.AddNode(1, Vector2.Zero);
            graph.AddNode(2, Vector2.Zero);
            var edge = graph.AddEdge(1, 2);
            Assertions.AssertThat(graph.GetEdge(1, 2) == edge).IsTrue();
            Assertions.AssertThat(graph.GetEdge(2, 1) == edge).IsTrue();
        }

        [TestCase]
        public void TestGetEdgeThrowsExceptionForNonExistentEdge()
        {
            var graph = new LayoutGraphResource();
            Assertions.AssertThrown(() => graph.GetEdge(1, 2));
        }
    }
}