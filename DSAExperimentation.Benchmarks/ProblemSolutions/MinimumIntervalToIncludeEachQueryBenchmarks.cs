using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumIntervalToIncludeEachQuery;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumIntervalToIncludeEachQuerySolution's, the same
// methods MinimumIntervalToIncludeEachQueryTests proves correct, and both take
// LeetCode's own intervals and queries arrays, so the workload is generated once in
// [GlobalSetup] rather than inside either measured call.
//
// Endpoints and queries are drawn from the same coordinate space, and intervals are
// kept shorter than that space, so a sizeable fraction of the queries land inside a
// sizeable fraction of the intervals - the scan arm therefore pays its full O(n*q)
// rather than short-circuiting on a mostly-uncovered axis.
[MemoryDiagnoser]
public class MinimumIntervalToIncludeEachQueryBenchmarks
{
    // LC problem number, used as the deterministic random seed.
    private const int RandomSeed = 1851;

    // Both interval endpoints and query values are drawn from the same [0, Count * this) space.
    private const int CoordinateSpaceMultiplier = 2;

    // Interval lengths are drawn from [0, Count / this) so intervals stay shorter than the full space.
    private const int MaxIntervalLengthDivisor = 2;

    [Params(200, 3_000)]
    public int Count;

    private int[][] _intervals = null!;
    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _intervals = Enumerable.Range(0, Count)
            .Select(_ =>
            {
                var left = random.Next(0, Count * CoordinateSpaceMultiplier);
                var right = left + random.Next(0, Count / MaxIntervalLengthDivisor);
                return new[] { left, right };
            })
            .ToArray();
        _queries = Enumerable.Range(0, Count).Select(_ => random.Next(0, Count * CoordinateSpaceMultiplier)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] PerQueryScan() =>
        MinimumIntervalToIncludeEachQuerySolution.MinIntervalsByPerQueryScan(_intervals, _queries);

    [Benchmark]
    public int[] HeapSweep() =>
        MinimumIntervalToIncludeEachQuerySolution.MinIntervalsByHeapSweep(_intervals, _queries);
}
