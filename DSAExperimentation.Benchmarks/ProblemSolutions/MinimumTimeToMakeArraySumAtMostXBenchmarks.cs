using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumTimeToMakeArraySumAtMostX;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumTimeToMakeArraySumAtMostXSolution's, the
// same methods MinimumTimeToMakeArraySumAtMostXTests proves correct. Both run the
// same O(n^2) knapsack recurrence over the pairs sorted ascending by nums2, so
// what the comparison isolates is the DP's own footprint - a dense
// long[n+1, n+1] table behind Array.Sort against a single long[n+1] rolling row
// behind this repo's MergeSort over an ArrayIndexedSequence<(int, int)>. x is set
// below any reachable sum so both arms are forced through every candidate
// operation count instead of returning on the first one checked.
[MemoryDiagnoser]
public class MinimumTimeToMakeArraySumAtMostXBenchmarks
{
    private const int RandomSeed = 2809; // LC problem number
    private const int ValueUpperBoundExclusive = 1_000;
    private const int UnreachableTarget = 0;

    private int[] _nums1 = [];

    private int[] _nums2 = [];
    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBoundExclusive)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DenseTable() =>
        MinimumTimeToMakeArraySumAtMostXSolution.MinimumTimeByDenseTable(_nums1, _nums2, UnreachableTarget);

    [Benchmark]
    public int RollingKnapsack() =>
        MinimumTimeToMakeArraySumAtMostXSolution.MinimumTimeByRollingKnapsack(_nums1, _nums2, UnreachableTarget);
}
