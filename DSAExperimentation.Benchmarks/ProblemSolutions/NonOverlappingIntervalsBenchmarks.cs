using DSAExperimentation.LeetCode.NonOverlappingIntervals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NonOverlappingIntervalsSolution's, the same
// methods NonOverlappingIntervalsSolutionTests proves correct. _intervals is generated
// as mostly non-overlapping, shuffled intervals (few removals needed), which
// hits brute force's true worst case - its outer "find the next kept interval"
// loop runs close to n times, each paying a full O(n) rescan - instead of the
// heavy-overlap case where most intervals get eliminated in the first few
// rounds. SortByEndThenGreedyScan can pass _intervals as it is: the sort strategy
// reads it through IntervalEndOrder.SortedByEnd, which sorts a copy, so every
// BenchmarkDotNet iteration starts from the same unsorted, shuffled workload
// without the harness having to clone it first. Each interval ends 1 or 2 past its
// start, so starti < endi as LC 435 requires.
public class NonOverlappingIntervalsBenchmarks
{
    // LC 435.
    private const int RandomSeed = 435;
    private const int IntervalSpacing = 3;

    // The largest end offset, inclusive.
    private const int EndOffsetUpperBound = 2;

    private (int Start, int End)[] _intervals = [];

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _intervals = Enumerable.Range(0, Length)
            .Select(i => (Start: i * IntervalSpacing, End: i * IntervalSpacing + random.Next(1, EndOffsetUpperBound + 1)))
            .OrderBy(_ => random.Next())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRepeatedMinEndScan() =>
        NonOverlappingIntervalsSolution.EraseOverlapIntervalsByBruteForce(_intervals);

    [Benchmark]
    public int SortByEndThenGreedyScan() =>
        NonOverlappingIntervalsSolution.EraseOverlapIntervalsBySortThenGreedy(_intervals);
}
