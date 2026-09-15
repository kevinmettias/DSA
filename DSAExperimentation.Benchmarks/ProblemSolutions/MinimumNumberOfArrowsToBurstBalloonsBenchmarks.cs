using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfArrowsToBurstBalloons;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfArrowsToBurstBalloonsSolution's, the
// same methods MinimumNumberOfArrowsToBurstBalloonsTests proves correct. Each arm is
// handed the prepared (Start, End) pairs its hoisted overload takes, so unpacking
// LeetCode's int[][] shape isn't charged to the measured method. _points is
// generated as mostly non-overlapping, shuffled balloons (needing close to n
// arrows), which hits brute force's true worst case - its outer "find the next
// arrow" loop runs close to n times, each paying a full O(n) rescan - instead of the
// few-arrows case where heavy overlap lets it finish in a handful of passes and look
// artificially competitive.
[MemoryDiagnoser]
public class MinimumNumberOfArrowsToBurstBalloonsBenchmarks
{
    // LC 452.
    private const int RandomSeed = 452;
    private const int IntervalSpacing = 3;
    private const int EndOffsetUpperBound = 2;

    private (int Start, int End)[] _points = [];

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _points = Enumerable.Range(0, Length)
            .Select(i => (Start: i * IntervalSpacing, End: i * IntervalSpacing + random.Next(0, EndOffsetUpperBound)))
            .OrderBy(_ => random.Next())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRescan() =>
        MinimumNumberOfArrowsToBurstBalloonsSolution.FindMinArrowShotsByBruteForceRescan(_points);

    [Benchmark]
    public int SortEndsThenGreedyScan() =>
        MinimumNumberOfArrowsToBurstBalloonsSolution.FindMinArrowShotsBySortEndsThenGreedyScan(_points);
}
