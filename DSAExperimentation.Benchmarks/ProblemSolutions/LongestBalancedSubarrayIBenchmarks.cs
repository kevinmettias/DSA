using DSAExperimentation.LeetCode.LongestBalancedSubarrayI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestBalancedSubarrayISolution's, the same
// methods LongestBalancedSubarrayISolutionTests proves correct. _nums alternates
// even/odd values so every prefix keeps both distinct-value sets growing
// together rather than one saturating out of a tiny alphabet early. The values
// start at 2, inside LC 3719's [1, 10^5].
public class LongestBalancedSubarrayIBenchmarks
{
    private int[] _nums = [];

    [Params(100, 1_500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = Enumerable.Range(0, Length).Select(i => 2 * (i + 1) + (i % 2)).ToArray();

    [Benchmark(Baseline = true)]
    public int BruteForce() => LongestBalancedSubarrayISolution.FindLongestBalancedLengthByBruteForce(_nums);

    [Benchmark]
    public int DistinctSetScan() =>
        LongestBalancedSubarrayISolution.FindLongestBalancedLengthByDistinctSetScan(_nums);
}
