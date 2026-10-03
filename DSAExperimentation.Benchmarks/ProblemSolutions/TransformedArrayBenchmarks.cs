using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.TransformedArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TransformedArraySolution's, the same methods
// TransformedArraySolutionTests proves correct. Shifts are drawn from the full [-Length,
// Length] range so the step-walk baseline is forced through long walks rather
// than the small shifts LeetCode's own examples use.
public class TransformedArrayBenchmarks
{
    private const int Seed = 3379;

    private int[] _nums = [];

    [Params(200, 5_000)]
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
