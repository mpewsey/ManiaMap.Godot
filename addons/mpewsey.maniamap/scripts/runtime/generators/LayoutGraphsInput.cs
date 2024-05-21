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
    /// <summary>
    /// A GenerationInput that adds layout graphs and their references template groups to the pipeline.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class LayoutGraphsInput : GenerationInput
    {
        /// <summary>
        /// If false, the layout graphs added to the inputs will be an array of fully constructed graphs.
        /// Otherwise, they will be an array of graph creation delegates to be invoked only after selection.
        /// </summary>
        [Export] public bool LazyCreateGraphs { get; set; }

        /// <summary>
        /// An array of layout graphs.
        /// </summary>
        [Export] public LayoutGraphResource[] LayoutGraphs { get; set; } = Array.Empty<LayoutGraphResource>();

        /// <inheritdoc/>
        public override void AddInputs(Dictionary<string, object> inputs)
        {
            inputs.Add("LayoutGraphs", LazyCreateGraphs ? GetMMLayoutGraphDelegates() : GetMMLayoutGraphs());
            inputs.Add("TemplateGroups", GetMMTemplateGroups());
        }

        /// <inheritdoc/>
        public override string[] InputNames()
        {
            return new string[] { "LayoutGraphs", "TemplateGroups" };
        }

        /// <summary>
        /// Returns a set of all unique templates groups referenced by the graphs.
        /// </summary>
        private HashSet<TemplateGroup> GetTemplateGroups()
        {
            var result = new HashSet<TemplateGroup>();

            foreach (var graph in LayoutGraphs)
            {
                result.UnionWith(graph.GetTemplateGroups());
            }

            return result;
        }

        /// <summary>
        /// Returns the ManiaMap template groups used by the procedural generator.
        /// </summary>
        /// <exception cref="DuplicateNameException">Thrown if a template group has a duplicate name.</exception>
        private TemplateGroups GetMMTemplateGroups()
        {
            var names = new HashSet<string>();
            var result = new TemplateGroups();
            var templateCache = new Dictionary<RoomTemplateResource, RoomTemplate>();

            foreach (var group in GetTemplateGroups())
            {
                if (!names.Add(group.Name))
                    throw new DuplicateNameException($"Duplicate group name: (Name = {group.Name}, Path = {group.ResourcePath}).");

                result.Add(group.Name, group.GetMMTemplateGroupEntries(templateCache));
            }

            return result;
        }

        /// <summary>
        /// Returns an array of ManiaMap graph creation delegates.
        /// </summary>
        private LayoutGraphSelector.LayoutGraphDelegate[] GetMMLayoutGraphDelegates()
        {
            var result = new LayoutGraphSelector.LayoutGraphDelegate[LayoutGraphs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = LayoutGraphs[i].GetMMLayoutGraph;
            }

            return result;
        }

        /// <summary>
        /// Returns an array of ManiaMap graphs used by the procedural generator.
        /// </summary>
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