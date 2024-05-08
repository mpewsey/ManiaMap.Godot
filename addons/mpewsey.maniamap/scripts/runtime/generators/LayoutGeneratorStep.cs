using Godot;
using MPewsey.Common.Pipelines;
using MPewsey.ManiaMap.Generators;

namespace MPewsey.ManiaMapGodot.Generators
{
    [Tool]
    [GlobalClass]
    public partial class LayoutGeneratorStep : GenerationStep
    {
        [Export(PropertyHint.Range, "1,100,1,or_greater")] public int MaxRebases { get; set; } = 100;
        [Export(PropertyHint.Range, "0,1,0.05,or_greater")] public float RebaseDecayRate { get; set; } = 0.25f;
        [Export(PropertyHint.Range, "-1,10,1,or_greater")] public int MaxBranchLength { get; set; } = -1;

        public override IPipelineStep CreateStep()
        {
            return new LayoutGenerator(MaxRebases, RebaseDecayRate, MaxBranchLength);
        }

        public override string[] RequiredInputNames()
        {
            return new string[] { "LayoutId", "LayoutGraph", "TemplateGroups", "RandomSeed" };
        }

        public override string[] OutputNames()
        {
            return new string[] { "Layout" };
        }
    }
}