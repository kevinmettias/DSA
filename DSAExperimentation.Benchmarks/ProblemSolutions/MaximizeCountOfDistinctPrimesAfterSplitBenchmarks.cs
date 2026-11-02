using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximizeCountOfDistinctPrimesAfterSplit;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeCountOfDistinctPrimesAfterSplitSolution's,
// the same methods MaximizeCountOfDistinctPrimesAfterSplitSolutionTests proves correct.
// Both apply each query to the array in place, as LC 3569 does, so each call hands its
// arm a fresh copy of the seeded array. Re-applied to the array a previous call left
// behind, an early query would split an array whose later-queried indices already hold
// their final values, and every call after the first would time a different workload
// from the one the tests check. The copy is timed on purpose, and both arms pay it.
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
        MaximizeCountOfDistinctPrimesAfterSplitSolution.MaxDistinctPrimeCountsByBruteForce((int[])_nums.Clone(), _queries);

    [Benchmark]
    public int[] PrefixSuffixScan() =>
        MaximizeCountOfDistinctPrimesAfterSplitSolution.MaxDistinctPrimeCountsByPrefixSuffixScan((int[])_nums.Clone(), _queries);
}
