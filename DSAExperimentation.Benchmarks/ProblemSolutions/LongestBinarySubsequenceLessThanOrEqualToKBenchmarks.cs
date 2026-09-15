using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestBinarySubsequenceLessThanOrEqualToK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestBinarySubsequenceLessThanOrEqualToKSolution's,
// the same methods LongestBinarySubsequenceLessThanOrEqualToKTests proves agree -
// exhaustive subset enumeration against the O(n) right-to-left greedy scan.
//
// Length is kept small (<= 20) so the 2^n baseline finishes in reasonable time, and
// the bits are randomized rather than all-1s or all-0s so it cannot short-circuit on
// a degenerate case. The workload is the LeetCode input shape itself, so building it
// in [GlobalSetup] already keeps string construction off the measured methods.
[MemoryDiagnoser]
public class LongestBinarySubsequenceLessThanOrEqualToKBenchmarks
{
    private const int RandomSeed = 2311; // LC problem number
    private const int BinaryDigits = 2; // '0' or '1', the only characters the input holds
    private const int K = 100;

    private string _bits = "";

    [Params(16, 20)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _bits = new string(
            Enumerable.Range(0, Length).Select(_ => IsZeroBit(random) ? '0' : '1').ToArray());
    }

    // The character is the draw itself: a zero out of BinaryDigits is the '0' bit.
    private static bool IsZeroBit(Random random) => random.Next(BinaryDigits) == 0;

    [Benchmark(Baseline = true)]
    public int BruteForceSubsetEnumeration() =>
        LongestBinarySubsequenceLessThanOrEqualToKSolution.LongestSubsequenceBySubsetEnumeration(_bits, K);

    [Benchmark]
    public int GreedyRightToLeftScan() =>
        LongestBinarySubsequenceLessThanOrEqualToKSolution.LongestSubsequenceByGreedyScan(_bits, K);
}
