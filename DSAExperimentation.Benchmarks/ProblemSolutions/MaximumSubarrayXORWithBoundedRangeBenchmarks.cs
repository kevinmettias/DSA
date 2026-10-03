using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximumSubarrayXORWithBoundedRange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSubarrayXORWithBoundedRangeSolution's, the
// same methods MaximumSubarrayXORWithBoundedRangeSolutionTests proves correct. The
// values are a seeded walk that moves at most MaxStep per element, clamped into
// LC 3845's [0, 2^15), so a spread of MaxSpread holds across windows thousands of
// elements long: the scan pays for every element of every window, while the
// sliding window pays a fixed 32-level trie walk per element. Sizes are per arm - the scan stops
// at 5,000 elements, where it is already quadratic, and the composed arm runs to
// LC's 4 * 10^4.
public class MaximumSubarrayXORWithBoundedRangeBenchmarks
{
    private const int RandomSeed = 3845;
    private const int ValueCeilingExclusive = 1 << 15;
    private const int MaxStep = 16;
    private const int MaxSpread = 2_048;

    private Dictionary<int, int[]> _numsBySize = [];

    public static IEnumerable<int> BruteForceSizes => [500, 5_000];

    public static IEnumerable<int> SlidingWindowSizes => [.. BruteForceSizes, 40_000];

    [GlobalSetup]
    public void Setup() => _numsBySize = SlidingWindowSizes.ToDictionary(size => size, DriftingValues);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public int BruteForce(int length) =>
        MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorByBruteForce(_numsBySize[length], MaxSpread);

    [Benchmark]
    [ArgumentsSource(nameof(SlidingWindowSizes))]
    public int SlidingWindowBitTrie(int length) =>
        MaximumSubarrayXORWithBoundedRangeSolution.MaxSubarrayXorBySlidingWindowBitTrie(_numsBySize[length], MaxSpread);

    // A walk from the middle of the value range, each step drawn from
    // [-MaxStep, MaxStep] and clamped into [0, ValueCeilingExclusive).
    private static int[] DriftingValues(int length)
    {
        var random = new Random(RandomSeed);
        var steps = SeededDraws.Values(length, -MaxStep, MaxStep + 1, random);
        var values = new int[length];
        var current = ValueCeilingExclusive / 2;

        for (var i = 0; i < length; i++)
        {
            current = Math.Clamp(current + steps[i], 0, ValueCeilingExclusive - 1);
            values[i] = current;
        }

        return values;
    }
}
