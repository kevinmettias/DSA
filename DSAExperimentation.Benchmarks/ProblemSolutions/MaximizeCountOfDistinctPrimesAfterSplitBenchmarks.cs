using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximizeCountOfDistinctPrimesAfterSplit;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeCountOfDistinctPrimesAfterSplitSolution's,
// the same methods MaximizeCountOfDistinctPrimesAfterSplitTests proves correct.
// Each [Benchmark] call re-applies the same query list to whatever state
// _nums was left in by the previous call - every index a query ever touches
// gets overwritten to that query's own value regardless of starting point, so
// repeated invocations within one job (and the two separate jobs BenchmarkDotNet
// runs per [GlobalSetup]) still measure the identical, deterministic workload.
[MemoryDiagnoser]
public class MaximizeCountOfDistinctPrimesAfterSplitBenchmarks
{
    private const int Seed = 3569; // LC problem number
    private const int MaxValueExclusive = 100_001;
    private const int QueryCount = 15;

    private int[] _nums = [];

    private int[][] _queries = [];
    [Params(50, 200)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = SeededDraws.Values(Length, 1, MaxValueExclusive, random);

        // A query is not two draws from one range - a split index bounded by the array
        // length, then the replacement value bounded by the value range - so it keeps
        // its own draw pair rather than going through SeededDraws.Pairs.
        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => new[] { random.Next(0, Length), random.Next(1, MaxValueExclusive) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        MaximizeCountOfDistinctPrimesAfterSplitSolution.MaxDistinctPrimeCountsByBruteForce(_nums, _queries);

    [Benchmark]
    public int[] PrefixSuffixScan() =>
        MaximizeCountOfDistinctPrimesAfterSplitSolution.MaxDistinctPrimeCountsByPrefixSuffixScan(_nums, _queries);
}
