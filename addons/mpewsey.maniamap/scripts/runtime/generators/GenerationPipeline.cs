using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.Common.Random;
using MPewsey.ManiaMapGodot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// A pipeline that runs a series of steps sequentially.
    /// </summary>
    [Tool]
    [GlobalClass]
    [Icon(ManiaMapResources.Icons.GenerationPipelineIcon)]
    public partial class GenerationPipeline : Node
    {
        /// <summary>
        /// An array of input strings that the user will supply manually to any of the run pipeline methods.
        /// These names are used for pipeline input validation.
        /// </summary>
        [Export] public string[] ManualInputNames { get; set; } = Array.Empty<string>();

#if TOOLS

#if GODOT4_4_0_OR_GREATER
        [ExportToolButton("Add Default Nodes")] public Callable AddDefaultNodesButton => Callable.From(OnSubmitAddDefaultNodesButton);
        private void OnSubmitAddDefaultNodesButton() => CreateDefaultNodes();
#else
        /// <summary>
        /// [Editor] When set to true, adds a set of default input and step nodes as children of the node.
        /// </summary>
        [Export] public bool AddDefaultNodes { get => false; set => CreateDefaultNodes(value); }
#endif

#endif

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

        /// <summary>
        /// Adds the default inputs and steps nodes as children of the pipeline.
        /// </summary>
        /// <param name="run">Executes the method only if this value is true.</param>
        public void CreateDefaultNodes(bool run = true)
        {
            if (!run)
                return;

            var owner = Engine.IsEditorHint() ? EditorInterface.Singleton.GetEditedSceneRoot() : null;
            AddDefaultInputNodes(owner);
            AddDefaultStepNodes(owner);
        }

        /// <summary>
        /// Adds the default input nodes to the pipeline.
        /// </summary>
        /// <param name="owner">The owner of the nodes.</param>
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

        /// <summary>
        /// Adds the default step nodes to the pipeline.
        /// </summary>
        /// <param name="owner">The owner of the nodes.</param>
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

        /// <summary>
        /// Returns a list of generation steps in the pipeline.
        /// This method returns all generation steps that are children of the pipeline.
        /// </summary>
        public List<GenerationStep> FindStepNodes()
        {
            var nodes = FindChildren("*", nameof(GenerationStep), true, false);
            var result = new List<GenerationStep>(nodes.Count);

            foreach (var node in nodes)
            {
                result.Add((GenerationStep)node);
            }

            return result;
        }

        /// <summary>
        /// Returns a list of generation inputs in the pipeline.
        /// This method returns all generation inputs that are children of the pipeline.
        /// </summary>
        public List<GenerationInput> FindInputNodes()
        {
            var nodes = FindChildren("*", nameof(GenerationInput), true, false);
            var result = new List<GenerationInput>(nodes.Count);

            foreach (var node in nodes)
            {
                result.Add((GenerationInput)node);
            }

            return result;
        }

        /// <summary>
        /// Runs the pipeline synchronously and returns the results.
        /// </summary>
        /// <param name="manualInputs">A dictionary of manual inputs. If null, no manual inputs will be used.</param>
        /// <param name="logger">The delegate invoked when pipeline log messages and issues. If null, no logs will be issues.</param>
        /// <param name="cancellationToken">The pipeline cancellation token.</param>
        public PipelineResults Run(Dictionary<string, object> manualInputs = null, Action<string> logger = null, CancellationToken cancellationToken = default)
        {
            return BuildPipeline().Run(BuildInputs(manualInputs), logger, cancellationToken);
        }

        /// <summary>
        /// Runs the pipeline synchronously and returns the results.
        /// </summary>
        /// <param name="manualInputs">A dictionary of manual inputs. If null, no manual inputs will be used.</param>
        /// <param name="logger">The delegate invoked when pipeline log messages and issues. If null, no logs will be issues.</param>
        /// <param name="cancellationToken">The pipeline cancellation token.</param>
        public Task<PipelineResults> RunAsync(Dictionary<string, object> manualInputs = null, Action<string> logger = null, CancellationToken cancellationToken = default)
        {
            return BuildPipeline().RunAsync(BuildInputs(manualInputs), logger, cancellationToken);
        }

        /// <summary>
        /// Runs the pipeline asynchronously until successful or until the maximum number of attempts have been exceeded.
        /// </summary>
        /// <param name="seed">The initial random seed.</param>
        /// <param name="attempts">The maximum number of pipeline run attempts.</param>
        /// <param name="timeout">The timeout used for each pipeline run.</param>
        /// <param name="manualInputs">A dictionary of manual inputs. If null, no manual inputs will be used.</param>
        /// <param name="logger">The delegate invoked when pipeline log messages and issues. If null, no logs will be issues.</param>
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

        /// <summary>
        /// Returns the pipeline used for generation.
        /// </summary>
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

        /// <summary>
        /// Returns a new dictionary of inputs.
        /// </summary>
        /// <param name="manualInputs">A dictionary of manual inputs. If null, no manual inputs will be used.</param>
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

        /// <summary>
        /// Validates the pipeline using the manual input names and throws exceptions if invalid.
        /// </summary>
        public void Validate()
        {
            Validate(ManualInputNames);
        }

        /// <summary>
        /// Validates the pipeline and throws exceptions if invalid.
        /// </summary>
        /// <param name="manualInputNames">A collection of manual input names.</param>
        public void Validate(IEnumerable<string> manualInputNames)
        {
            manualInputNames ??= Enumerable.Empty<string>();
            var names = new HashSet<string>(manualInputNames);
            ValidateInputs(names);
            ValidateSteps(names);
        }

        /// <summary>
        /// Valides the pipeline input names.
        /// </summary>
        /// <param name="names">A set of current input names.</param>
        /// <exception cref="DuplicateInputException">Thrown if a duplicate input name is encountered.</exception>
        private void ValidateInputs(HashSet<string> names)
        {
            var nodes = FindInputNodes();

            foreach (var node in nodes)
            {
                foreach (var name in node.InputNames())
                {
                    if (!names.Add(name))
                        throw new DuplicateInputException($"Duplicate input name: (Name = {name}, NodePath = {GetPathTo(node)})");
                }
            }
        }

        /// <summary>
        /// Validates the pipeline step inputs and outputs names.
        /// </summary>
        /// <param name="names">A set of current input names.</param>
        /// <exception cref="MissingInputException">Thrown if an input name is missing for a given step.</exception>
        private void ValidateSteps(HashSet<string> names)
        {
            var nodes = FindStepNodes();

            foreach (var node in nodes)
            {
                foreach (var name in node.RequiredInputNames())
                {
                    if (!names.Contains(name))
                        throw new MissingInputException($"Missing input name: (Name = {name}, NodePath = {GetPathTo(node)})");
                }

                foreach (var name in node.OutputNames())
                {
                    names.Add(name);
                }
            }
        }

        /// <summary>
        /// Finds the random seed input for the pipeline and sets its value.
        /// </summary>
        /// <param name="seed">The random seed value.</param>
        /// <exception cref="MissingInputException">Thrown if a random seed input does not exist as a descendent of the pipeline.</exception>
        /// <exception cref="DuplicateInputException">Thrown if multiple random seed inputs are descendents of the pipeline.</exception>
        public void SetRandomSeed(int seed)
        {
            var children = FindChildren("*", nameof(RandomSeedInput), true, false);

            if (children.Count == 0)
                throw new MissingInputException("Random seed input not found.");
            if (children.Count > 1)
                throw new DuplicateInputException($"Generation pipeline has multiple random seed inputs: {children.Count}.");

            foreach (var child in children)
            {
                var input = (RandomSeedInput)child;
                input.RandomSeed = seed;
            }
        }
    }
}