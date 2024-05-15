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
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.GenerationPipelineIcon)]
    public partial class GenerationPipeline : Node
    {
#if TOOLS
        [Export] public bool AddDefaultNodes { get => false; set => CreateDefaultNodes(value); }
#endif

        [Export] public string[] ManualInputNames { get; set; } = Array.Empty<string>();

#if TOOLS
        public override string[] _GetConfigurationWarnings()
        {
            try
            {
                Validate();
            }
            catch (Exception exception)
            {
                return new string[] { exception.Message };
            }

            return Array.Empty<string>();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (Engine.IsEditorHint())
                UpdateConfigurationWarnings();
        }
#endif

        public void CreateDefaultNodes(bool run = true)
        {
            if (!run)
                return;

            var owner = Engine.IsEditorHint() ? EditorInterface.Singleton.GetEditedSceneRoot() : null;
            AddDefaultInputNodes(owner);
            AddDefaultStepNodes(owner);
        }

        private void AddDefaultInputNodes(Node owner)
        {
            var inputs = new Node() { Name = "Inputs" };

            var layoutIdInput = new GenerationIntInput()
            {
                Name = "LayoutIdInput",
                InputName = "LayoutId",
                Value = Rand.GetRandomId(),
            };

            var randomSeedInput = new RandomSeedInput() { Name = nameof(RandomSeedInput) };
            var layoutGraphsInput = new LayoutGraphsInput() { Name = nameof(LayoutGraphsInput) };
            var collectableGroupsInput = new CollectableGroupsInput() { Name = nameof(CollectableGroupsInput) };

            inputs.AddChild(layoutIdInput);
            inputs.AddChild(randomSeedInput);
            inputs.AddChild(layoutGraphsInput);
            inputs.AddChild(collectableGroupsInput);
            AddChild(inputs);

            inputs.Owner = owner;
            layoutIdInput.Owner = owner;
            randomSeedInput.Owner = owner;
            layoutGraphsInput.Owner = owner;
            collectableGroupsInput.Owner = owner;
        }

        private void AddDefaultStepNodes(Node owner)
        {
            var steps = new Node() { Name = "Steps" };
            var layoutGraphSelectorStep = new LayoutGraphSelectorStep() { Name = nameof(LayoutGraphSelectorStep) };
            var layoutGraphRandomizerStep = new LayoutGraphRandomizerStep() { Name = nameof(LayoutGraphRandomizerStep) };
            var layoutGeneratorStep = new LayoutGeneratorStep() { Name = nameof(LayoutGeneratorStep) };
            var collectableGeneratorStep = new CollectableGeneratorStep() { Name = nameof(CollectableGeneratorStep) };

            steps.AddChild(layoutGraphSelectorStep);
            steps.AddChild(layoutGraphRandomizerStep);
            steps.AddChild(layoutGeneratorStep);
            steps.AddChild(collectableGeneratorStep);
            AddChild(steps);

            steps.Owner = owner;
            layoutGraphSelectorStep.Owner = owner;
            layoutGraphRandomizerStep.Owner = owner;
            layoutGeneratorStep.Owner = owner;
            collectableGeneratorStep.Owner = owner;
        }

        public List<GenerationStep> FindStepNodes()
        {
            var nodes = FindChildren("*", nameof(GenerationStep));
            var result = new List<GenerationStep>(nodes.Count);

            foreach (var node in nodes)
            {
                result.Add((GenerationStep)node);
            }

            return result;
        }

        public List<GenerationInput> FindInputNodes()
        {
            var nodes = FindChildren("*", nameof(GenerationInput));
            var result = new List<GenerationInput>(nodes.Count);

            foreach (var node in nodes)
            {
                result.Add((GenerationInput)node);
            }

            return result;
        }

        public PipelineResults Run(Dictionary<string, object> manualInputs = null, Action<string> logger = null, CancellationToken cancellationToken = default)
        {
            return BuildPipeline().Run(BuildInputs(manualInputs), logger, cancellationToken);
        }

        public Task<PipelineResults> RunAsync(Dictionary<string, object> manualInputs = null, Action<string> logger = null, CancellationToken cancellationToken = default)
        {
            return BuildPipeline().RunAsync(BuildInputs(manualInputs), logger, cancellationToken);
        }

        public async Task<PipelineResults> RunAttemptsAsync(int seed, int attempts = 10, int timeout = 5000, Dictionary<string, object> manualInputs = null, Action<string> logger = null)
        {
            manualInputs ??= new Dictionary<string, object>();

            for (int i = 0; i < attempts; i++)
            {
                logger?.Invoke($"[Generation Pipeline] Beginning attempt {i + 1} / {attempts}...");
                var inputs = new Dictionary<string, object>(manualInputs);
                inputs.Add("RandomSeed", new RandomSeed(seed + i * 1447));
                var token = new CancellationTokenSource(timeout).Token;
                var results = await RunAsync(inputs, logger, token);

                if (results.Success)
                {
                    logger?.Invoke("[Generation Pipeline] Attempt successful.");
                    return results;
                }
            }

            logger?.Invoke("[Generation Pipeline] Generation failed for all attempts.");
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
                        throw new Exception($"Duplicate input name: (Name = {name}, NodePath = {GetPathTo(node)})");
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
                        throw new Exception($"Missing input name: (Name = {name}, NodePath = {GetPathTo(node)})");
                }

                foreach (var name in node.OutputNames())
                {
                    names.Add(name);
                }
            }
        }
    }
}