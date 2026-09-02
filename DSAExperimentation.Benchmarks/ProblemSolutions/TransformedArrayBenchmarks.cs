using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TransformedArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TransformedArraySolution's, the same methods
// TransformedArrayTests proves correct. Shifts are drawn from the full [-Length,
// Length] range so the step-walk baseline is forced through long walks rather
// than the small shifts LeetCode's own examples use.
[MemoryDiagnoser]
public class TransformedArrayBenchmarks
{
    private const int Seed = 3379;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-Length, Length + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] StepWalk() => TransformedArraySolution.TransformByStepWalk(_nums);

    [Benchmark]
    public int[] ModuloWalk() => TransformedArraySolution.TransformByModuloWalk(_nums);
}
