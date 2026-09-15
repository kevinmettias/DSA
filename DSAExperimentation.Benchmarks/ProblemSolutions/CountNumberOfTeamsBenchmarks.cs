using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountNumberOfTeams;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountNumberOfTeamsSolution's, the same methods
// CountNumberOfTeamsTests proves correct - the textbook cubic triple-loop scan
// against the O(n log n) pair of coordinate-compressed FenwickTree sweeps. The
// rating array is the problem's own input shape, so building it is all
// [GlobalSetup] has to do.
[MemoryDiagnoser]
public class CountNumberOfTeamsBenchmarks
{
    private const int RandomSeed = 1395; // LC problem number
    private const int MaxRatingExclusive = 100_000;

    private int[] _rating = [];

    [Params(80, 200)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rating = [.. Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxRatingExclusive))];
    }

    [Benchmark(Baseline = true)]
    public int TripleLoopScan() => CountNumberOfTeamsSolution.CountTeamsByTripleLoop(_rating);

    [Benchmark]
    public int FenwickTreeSweeps() => CountNumberOfTeamsSolution.CountTeamsByFenwickSweeps(_rating);
}
