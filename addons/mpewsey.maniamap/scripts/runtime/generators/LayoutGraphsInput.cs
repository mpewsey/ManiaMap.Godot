using Godot;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Generators;
using MPewsey.ManiaMap.Graphs;
using MPewsey.ManiaMapGodot.Exceptions;
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
            inputs.Add("LayoutGraphs", LazyCreateGraphs ? GetMMLayoutGraphDelegates() : GetMMLayoutGraphs());
            inputs.Add("TemplateGroups", GetMMTemplateGroups());
        }

        public override string[] InputNames()
        {
            return new string[] { "LayoutGraphs", "TemplateGroups" };
        }

        private HashSet<TemplateGroup> GetTemplateGroups()
        {
            var result = new HashSet<TemplateGroup>();

            foreach (var graph in LayoutGraphs)
            {
                result.UnionWith(graph.GetTemplateGroups());
            }

            return result;
        }

        private TemplateGroups GetMMTemplateGroups()
        {
            var names = new HashSet<string>();
            var result = new TemplateGroups();
            var templateCache = new Dictionary<RoomTemplateResource, RoomTemplate>();

            foreach (var group in GetTemplateGroups())
            {
                if (!names.Add(group.Name))
                    throw new DuplicateNameException($"Duplicate group name: {group.Name}.");

                result.Add(group.Name, group.GetMMTemplateGroupEntries(templateCache));
            }

            return result;
        }

        private LayoutGraphSelector.LayoutGraphDelegate[] GetMMLayoutGraphDelegates()
        {
            var result = new LayoutGraphSelector.LayoutGraphDelegate[LayoutGraphs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = LayoutGraphs[i].GetMMLayoutGraph;
            }

            return result;
        }

        private LayoutGraph[] GetMMLayoutGraphs()
        {
            var result = new LayoutGraph[LayoutGraphs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = LayoutGraphs[i].GetMMLayoutGraph();
            }

            return result;
        }
    }
}