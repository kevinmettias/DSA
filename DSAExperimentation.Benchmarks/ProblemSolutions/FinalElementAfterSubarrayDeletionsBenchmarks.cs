using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FinalElementAfterSubarrayDeletions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are FinalElementAfterSubarrayDeletionsSolution's,
// the same methods FinalElementAfterSubarrayDeletionsTests proves correct.
// Length stays tiny: the two game-tree arms recurse over every surviving
// subset of the array, so this is the axis whose state count they pay for -
// EndpointComparison ignores it entirely, which is the point of measuring it
// alongside them.
[MemoryDiagnoser]
public class FinalElementAfterSubarrayDeletionsBenchmarks
{
    private const int Seed = 3828; // LC problem number
    private const int MaxValueExclusive = 100_000;

    [Params(8, 12)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DictionaryMinimax() => FinalElementAfterSubarrayDeletionsSolution.FinalElementByDictionaryMinimax(_nums);

    [Benchmark]
    public int MemoizedMinimax() => FinalElementAfterSubarrayDeletionsSolution.FinalElementByMemoizedMinimax(_nums);

    [Benchmark]
    public int EndpointComparison() => FinalElementAfterSubarrayDeletionsSolution.FinalElementByEndpointComparison(_nums);
}
