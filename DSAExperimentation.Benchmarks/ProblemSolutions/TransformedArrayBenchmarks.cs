using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TransformedArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TransformedArraySolution's, the same methods
// TransformedArraySolutionTests proves correct. Shifts are drawn from the full [-Length,
// Length] range so the step-walk baseline is forced through long walks rather
// than the small shifts LeetCode's own examples use. Length stops at LC 3379's 100,
// so every shift stays inside its [-100, 100].
public class TransformedArrayBenchmarks
{
    private const int Seed = 3379;

    private int[] _nums = [];

    [Params(10, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = SeededDraws.Values(Length, -Length, Length + 1, random);
    }

    [Benchmark(Baseline = true)]
    public int[] StepWalk() => TransformedArraySolution.TransformByStepWalk(_nums);

    [Benchmark]
    public int[] ModuloWalk() => TransformedArraySolution.TransformByModuloWalk(_nums);
}
