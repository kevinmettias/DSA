using DSAExperimentation.LeetCode.LongestBinarySubsequenceLessThanOrEqualToK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestBinarySubsequenceLessThanOrEqualToKSolution's,
// the same methods LongestBinarySubsequenceLessThanOrEqualToKSolutionTests proves agree -
// exhaustive subset enumeration against the O(n) right-to-left greedy scan.
//
// The bits are randomized rather than all-1s or all-0s so the baseline cannot
// short-circuit on a degenerate case. The workload is the LeetCode input shape itself,
// so building it in [GlobalSetup] already keeps string construction off the measured
// methods.
//
// Sizes are per arm. The 2^n baseline stops at 20 characters; the greedy scan runs on
// to LC 2311's own bound of 1,000, and the two are compared at the lengths both run.
public class LongestBinarySubsequenceLessThanOrEqualToKBenchmarks
{
    private const int RandomSeed = 2311; // LC problem number
    private const int BinaryDigits = 2; // '0' or '1', the only characters the input holds
    private const int K = 100;

    private Dictionary<int, string> _bitsByLength = [];

    public static IEnumerable<int> SubsetEnumerationSizes => [16, 20];

    public static IEnumerable<int> GreedyScanSizes => [.. SubsetEnumerationSizes, 100, 1_000];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _bitsByLength = GreedyScanSizes.ToDictionary(length => length, BuildBits);

    private static string BuildBits(int length)
    {
        var random = new Random(RandomSeed);

        return new string(Enumerable.Range(0, length).Select(_ => IsZeroBit(random) ? '0' : '1').ToArray());
    }

    // The character is the draw itself: a zero out of BinaryDigits is the '0' bit.
    private static bool IsZeroBit(Random random) => random.Next(BinaryDigits) == 0;

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(SubsetEnumerationSizes))]
    public int BruteForceSubsetEnumeration(int length) =>
        LongestBinarySubsequenceLessThanOrEqualToKSolution.LongestSubsequenceBySubsetEnumeration(_bitsByLength[length], K);

    [Benchmark]
    [ArgumentsSource(nameof(GreedyScanSizes))]
    public int GreedyRightToLeftScan(int length) =>
        LongestBinarySubsequenceLessThanOrEqualToKSolution.LongestSubsequenceByGreedyScan(_bitsByLength[length], K);
}
