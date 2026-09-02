using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumOperationsToMakeArrayEqualToTarget;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumOperationsToMakeArrayEqualToTargetSolution's, the
// same methods MinimumOperationsToMakeArrayEqualToTargetTests proves correct.
[MemoryDiagnoser]
public class MinimumOperationsToMakeArrayEqualToTargetBenchmarks
{
    private const int Seed = 3229;

    [Params(100, 2_000)]
    public int Length;

    private int[] _nums = null!;
    private int[] _target = null!;

    [GlobalSetup]
    public void Setup() => (_nums, _target) = ArrayEqualToTargetWorkloads.BuildArrays(Length, Seed);

    [Benchmark(Baseline = true)]
    public long BruteForceSimulation() =>
        MinimumOperationsToMakeArrayEqualToTargetSolution.MinOperationsByBruteForceSimulation(_nums, _target);

    [Benchmark]
    public long DifferenceScan() =>
        MinimumOperationsToMakeArrayEqualToTargetSolution.MinOperationsByDifferenceScan(_nums, _target);
}
