using DSAExperimentation.LeetCode.CountIncreasingQuadruplets;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountIncreasingQuadrupletsSolution's, the same
// methods CountIncreasingQuadrupletsSolutionTests proves correct.
//
// Sizes are per arm. The O(n^4) baseline would dominate the run past a modest
// permutation, so it stops at 16 elements; the O(n^2 log n) Fenwick sweep runs on to
// 2,000, half LC 2552's own bound of 4,000, and the two are compared at the sizes both
// run.
public class CountIncreasingQuadrupletsBenchmarks
{
    private const int RandomSeed = 2552; // LeetCode problem number

    private Dictionary<int, int[]> _numsByLength = [];

    public static IEnumerable<int> BruteForceSizes => [8, 16];

    public static IEnumerable<int> FenwickSizes => [.. BruteForceSizes, 200, 2_000];

    // Every length any arm runs is shuffled here, outside the timed region, each from its
    // own generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _numsByLength = FenwickSizes.ToDictionary(length => length, BuildPermutation);

    private static int[] BuildPermutation(int length)
    {
        var random = new Random(RandomSeed);

        return Enumerable.Range(1, length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForce(int length) =>
        CountIncreasingQuadrupletsSolution.CountQuadrupletsByBruteForce(_numsByLength[length]);

    [Benchmark]
    [ArgumentsSource(nameof(FenwickSizes))]
    public long FenwickTreeSweep(int length) =>
        CountIncreasingQuadrupletsSolution.CountQuadrupletsByFenwickTreeSweep(_numsByLength[length]);
}
