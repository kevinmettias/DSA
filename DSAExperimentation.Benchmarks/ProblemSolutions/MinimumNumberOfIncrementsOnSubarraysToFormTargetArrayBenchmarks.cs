using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfIncrementsOnSubarraysToFormTargetArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution's - the literal layer-by-layer
// simulation of the increment operations (O(n * max(target))) against the O(n) single pass that
// sums positive rises between consecutive elements. Random heights up to 50 give the simulation
// enough layers to separate the two.
[MemoryDiagnoser]
public class MinimumNumberOfIncrementsOnSubarraysToFormTargetArrayBenchmarks
{
    private const int RandomSeed = 1526; // LC 1526
    private const int MaxTargetHeight = 50;

    [Params(200, 2_000)]
    public int Length;

    private int[] _target = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _target = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxTargetHeight)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LayerByLayerSimulation() =>
        MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution.MinNumberOperationsByLayerSimulation(_target);

    [Benchmark]
    public int RunningDiffScan() =>
        MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution.MinNumberOperationsByRisingDiffScan(_target);
}
