using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.Common.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Generators
{
    [GlobalClass]
    public partial class GenerationPipeline : Node
    {
        [Export] public string[] ManualInputNames { get; set; } = Array.Empty<string>();

        public override string[] _GetConfigurationWarnings()
        {
            var warnings = base._GetConfigurationWarnings();

            try
            {
                Validate();
            }
            catch (Exception exception)
            {
                return warnings.Append(exception.Message).ToArray();
            }

            return warnings;
        }

        public List<GenerationStep> FindStepNodes()
        {
            return FindChildren("*", nameof(GenerationStep)).Cast<GenerationStep>().ToList();
        }

        public List<GenerationInput> FindInputNodes()
        {
            return FindChildren("*", nameof(GenerationInput)).Cast<GenerationInput>().ToList();
        }

        public Task<PipelineResults> RunAsync(Dictionary<string, object> manualInputs = null, CancellationToken cancellationToken = default)
        {
            return BuildPipeline().RunAsync(BuildInputs(manualInputs), x => GD.Print(x), cancellationToken);
        }

        public async Task<PipelineResults> RunAttemptsAsync(int seed, int attempts = 10, int timeout = 5000, Dictionary<string, object> manualInputs = null)
        {
            manualInputs ??= new Dictionary<string, object>();

            for (int i = 0; i < attempts; i++)
            {
                GD.Print($"[Generation Pipeline] Beginning attempt {i + 1} / {attempts}...");

                var inputs = new Dictionary<string, object>(manualInputs);
                inputs.Add("RandomSeed", new RandomSeed(seed + i * 1447));
                var token = new CancellationTokenSource(timeout).Token;
                var results = await RunAsync(inputs, token);

                if (results.Success)
                {
                    GD.Print("[Generation Pipeline] Attempt successful.");
                    return results;
                }
            }

            GD.Print("[Generation Pipeline] Generation failed for all attempts.");
            return new PipelineResults(manualInputs);
        }

        public Pipeline BuildPipeline()
        {
            var nodes = FindStepNodes();
            var steps = new IPipelineStep[nodes.Count];

            for (int i = 0; i < steps.Length; i++)
            {
                steps[i] = nodes[i].CreateStep();
            }

            return new Pipeline(steps);
        }

        public Dictionary<string, object> BuildInputs(Dictionary<string, object> manualInputs = null)
        {
            manualInputs ??= new Dictionary<string, object>();
            Validate(manualInputs.Keys);
            var inputs = new Dictionary<string, object>(manualInputs);
            var nodes = FindInputNodes();

            foreach (var node in nodes)
            {
                node.AddInputs(inputs);
            }

            return inputs;
        }

        public void Validate()
        {
            Validate(ManualInputNames);
        }

        public void Validate(IEnumerable<string> manualInputNames)
        {
            manualInputNames ??= Enumerable.Empty<string>();
            var names = new HashSet<string>(manualInputNames);
            ValidateInputs(names);
            ValidateSteps(names);
        }

        private void ValidateInputs(HashSet<string> names)
        {
            var nodes = FindInputNodes();

            foreach (var node in nodes)
            {
                foreach (var name in node.InputNames())
                {
                    if (!names.Add(name))
                        throw new Exception($"Duplicate input name: (Name = {name}, NodePath = {node.GetPath()})");
                }
            }
        }

        private void ValidateSteps(HashSet<string> names)
        {
            var nodes = FindStepNodes();

            foreach (var node in nodes)
            {
                foreach (var name in node.RequiredInputNames())
                {
                    if (!names.Contains(name))
                        throw new Exception($"Missing input name: (Name = {name}, NodePath = {node.GetPath()})");
                }

                foreach (var name in node.OutputNames())
                {
                    names.Add(name);
                }
            }
        }
    }
}