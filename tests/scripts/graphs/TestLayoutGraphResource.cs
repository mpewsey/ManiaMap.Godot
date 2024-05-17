using GdUnit4;
using Godot;
using System;

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
            Assertions.AssertThrown(() => graph.AddNode(node.Id, Vector2.Zero)).IsInstanceOf<Exception>();
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
            Assertions.AssertThrown(() => graph.AddEdge(1, 2)).IsInstanceOf<Exception>();
            Assertions.AssertThrown(() => graph.AddEdge(2, 1)).IsInstanceOf<Exception>();
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
            Assertions.AssertThrown(() => graph.GetEdge(1, 2)).IsInstanceOf<Exception>();
        }

        [TestCase]
        public void TestRemoveEdge()
        {
            var graph = new LayoutGraphResource();
            Assertions.AssertThat(graph.RemoveEdge(1, 2)).IsFalse();
            Assertions.AssertThat(graph.RemoveEdge(2, 1)).IsFalse();

            graph.AddNode(1, Vector2.Zero);
            graph.AddNode(2, Vector2.Zero);

            graph.AddEdge(1, 2);
            Assertions.AssertThat(graph.Edges.Count).IsEqual(1);
            Assertions.AssertThat(graph.RemoveEdge(1, 2)).IsTrue();
            Assertions.AssertThat(graph.Edges.Count).IsEqual(0);

            graph.AddEdge(1, 2);
            Assertions.AssertThat(graph.Edges.Count).IsEqual(1);
            Assertions.AssertThat(graph.RemoveEdge(2, 1)).IsTrue();
            Assertions.AssertThat(graph.Edges.Count).IsEqual(0);
        }

        [TestCase]
        public void TestRemoveNode()
        {
            var graph = new LayoutGraphResource();
            Assertions.AssertThat(graph.RemoveNode(1)).IsFalse();

            graph.AddNode(1, Vector2.Zero);
            graph.AddNode(2, Vector2.Zero);
            graph.AddEdge(1, 2);

            Assertions.AssertThat(graph.RemoveNode(1)).IsTrue();
            Assertions.AssertThat(graph.Nodes.Count).IsEqual(1);
            Assertions.AssertThat(graph.Edges.Count).IsEqual(0);
        }

        [TestCase]
        public void TestEdgeIndexes()
        {
            var graph = new LayoutGraphResource();
            graph.AddNode(1, Vector2.Zero);
            graph.AddNode(2, Vector2.Zero);
            graph.AddNode(3, Vector2.Zero);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(2, 3);

            var result = graph.NodeEdgeIndexes(1);
            Assertions.AssertThat(result.Count).IsEqual(2);
            Assertions.AssertThat(result.Contains(new Vector2I(1, 2))).IsTrue();
            Assertions.AssertThat(result.Contains(new Vector2I(1, 3))).IsTrue();
        }

        [TestCase]
        public void TestRemoveNodeEdges()
        {
            var graph = new LayoutGraphResource();
            graph.AddNode(1, Vector2.Zero);
            graph.AddNode(2, Vector2.Zero);
            graph.AddNode(3, Vector2.Zero);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(2, 3);

            graph.RemoveNodeEdges(1);
            Assertions.AssertThat(graph.Edges.Count).IsEqual(1);
        }

        [TestCase]
        public void TestContainsEdge()
        {
            var graph = new LayoutGraphResource();
            graph.AddNode(1, Vector2.Zero);
            graph.AddNode(2, Vector2.Zero);
            graph.AddEdge(1, 2);

            Assertions.AssertThat(graph.ContainsEdge(1, 2)).IsTrue();
            Assertions.AssertThat(graph.ContainsEdge(2, 1)).IsTrue();
            Assertions.AssertThat(graph.ContainsEdge(1, 3)).IsFalse();
            Assertions.AssertThat(graph.ContainsEdge(3, 1)).IsFalse();
        }
    }
}