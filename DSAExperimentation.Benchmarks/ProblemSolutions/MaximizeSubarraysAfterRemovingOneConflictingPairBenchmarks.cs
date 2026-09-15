using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximizeSubarraysAfterRemovingOneConflictingPair;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MaximizeSubarraysAfterRemovingOneConflictingPairSolution's, the same methods
// MaximizeSubarraysAfterRemovingOneConflictingPairTests proves correct. N is kept
// small (unlike most other benchmarks in this project) because the baseline is
// O(n^2 * m^2) by design - it is the naive arm the O(n + m) sweep has to justify
// itself against, not a strategy meant to scale.
[MemoryDiagnoser]
public class MaximizeSubarraysAfterRemovingOneConflictingPairBenchmarks
{
    private const int Seed = 3480;

    private int[][] _conflictingPairs = [];

    [Params(10, 50)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _conflictingPairs = BuildConflictingPairs(N, random);
    }

    // One pair per position, both endpoints drawn from [1, n] and never equal -
    // stays within LC's own conflictingPairs.length <= 2n bound.
    private static int[][] BuildConflictingPairs(int n, Random random)
    {
        var pairs = new int[n][];

        for (var i = 0; i < n; i++)
        {
            var a = random.Next(1, n + 1);
            int b;

            do
            {
                b = random.Next(1, n + 1);
            }
            while (b == a);

            pairs[i] = [a, b];
        }

        return pairs;
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        MaximizeSubarraysAfterRemovingOneConflictingPairSolution.MaxSubarraysByBruteForce(N, _conflictingPairs);

    [Benchmark]
    public int GroupedBoundSweep() =>
        MaximizeSubarraysAfterRemovingOneConflictingPairSolution.MaxSubarraysByGroupedBoundSweep(N, _conflictingPairs);
}
