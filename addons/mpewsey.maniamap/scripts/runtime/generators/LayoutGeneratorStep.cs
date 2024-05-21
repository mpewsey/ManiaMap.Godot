using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMapGodot.Generators
{
    /// <summary>
    /// A GenerationStep that produces a procedurally generated `Layout`.
    /// </summary>
    [Tool]
    [GlobalClass]
    public partial class LayoutGeneratorStep : GenerationStep
    {
        /// <summary>
        /// The maximum number of times a layout may be used as a base.
        /// </summary>
        [Export(PropertyHint.Range, "1,100,1,or_greater")] public int MaxRebases { get; set; } = 100;

        /// <summary>
        /// The decay rate applied to the maximum number of rebases.
        /// </summary>
        [Export(PropertyHint.Range, "0,1,0.05,or_greater")] public float RebaseDecayRate { get; set; } = 0.25f;

        /// <summary>
        /// The maximum length for graph branches. If less than or equal to zero, branches will not be split.
        /// </summary>
        [Export(PropertyHint.Range, "-1,10,1,or_greater")] public int MaxBranchLength { get; set; } = -1;

        /// <inheritdoc/>
        public override IPipelineStep CreateStep()
        {
            return new LayoutGenerator(MaxRebases, RebaseDecayRate, MaxBranchLength);
        }

        /// <inheritdoc/>
        public override string[] RequiredInputNames()
        {
            return new string[] { "LayoutId", "LayoutGraph", "TemplateGroups", "RandomSeed" };
        }

        /// <inheritdoc/>
        public override string[] OutputNames()
        {
            return new string[] { "Layout" };
        }
    }
}