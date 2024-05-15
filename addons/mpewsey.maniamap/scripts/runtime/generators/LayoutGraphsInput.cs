using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Graphs;
using MPewsey.ManiaMapGodot.Graphs;
using System;
using System.Collections.Generic;

namespace MPewsey.ManiaMapGodot.Generators
{
    [Tool]
    [GlobalClass]
    public partial class LayoutGraphsInput : GenerationInput
    {
        [Export] public bool LazyCreateGraphs { get; set; }
        [Export] public LayoutGraphResource[] LayoutGraphs { get; set; } = Array.Empty<LayoutGraphResource>();

        public override void AddInputs(Dictionary<string, object> inputs)
        {
            inputs.Add("LayoutGraphs", LazyCreateGraphs ? GetGraphDelegates() : GetGraphs());
            inputs.Add("TemplateGroups", GetTemplateGroups());
        }

        public override string[] InputNames()
        {
            return new string[] { "LayoutGraphs", "TemplateGroups" };
        }

        public HashSet<TemplateGroup> GetTemplateGroupSet()
        {
            var result = new HashSet<TemplateGroup>();

            foreach (var graph in LayoutGraphs)
            {
                result.UnionWith(graph.GetTemplateGroups());
            }

            return result;
        }

        public TemplateGroups GetTemplateGroups()
        {
            var names = new HashSet<string>();
            var result = new TemplateGroups();

            foreach (var group in GetTemplateGroupSet())
            {
                if (!names.Add(group.Name))
                    throw new Exception($"Duplicate group name: {group.Name}.");

                result.Add(group.Name, group.GetEntries());
            }

            return result;
        }

        public Func<LayoutGraph>[] GetGraphDelegates()
        {
            var result = new Func<LayoutGraph>[LayoutGraphs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = LayoutGraphs[i].GetGraph;
            }

            return result;
        }

        public LayoutGraph[] GetGraphs()
        {
            var result = new LayoutGraph[LayoutGraphs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = LayoutGraphs[i].GetGraph();
            }

            return result;
        }
    }
}