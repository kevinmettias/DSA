using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestWellPerformingInterval;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LongestWellPerformingIntervalSolution's, the same
// methods LongestWellPerformingIntervalTests proves correct. Hours alternate 9
// ("tiring", +1) vs. 6 ("not tiring", -1) by an unweighted coin flip, which keeps
// the running score oscillating near zero for its whole length - the scenario that
// makes the HashMap actually earn repeated lookups instead of only ever growing, and
// forces the baseline's inner loop through its full O(n^2) worst case since no early
// exit is possible either way (both strategies must scan every candidate interval to
// find the longest, unlike TwoSumBenchmarks' find-any-pair shape).
[MemoryDiagnoser]
public class LongestWellPerformingIntervalBenchmarks
{
    private const int CoinFlipUpperBoundExclusive = 2;

    private const int TiringHour = 9;

    private const int NotTiringHour = 6;

    private int[] _hours = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _hours = Enumerable.Range(0, Length)
            .Select(_ => IsTiringHour(random) ? TiringHour : NotTiringHour)
            .ToArray();
    }

    // An unweighted coin flip, heads (1) being the tiring hour.
    private static bool IsTiringHour(Random random) => random.Next(0, CoinFlipUpperBoundExclusive) == 1;

    [Benchmark(Baseline = true)]
    public int BruteForce() => LongestWellPerformingIntervalSolution.LongestWpiByBruteForce(_hours);

    [Benchmark]
    public int HashMapPrefixScore() => LongestWellPerformingIntervalSolution.LongestWpiByPrefixScoreMap(_hours);
}
