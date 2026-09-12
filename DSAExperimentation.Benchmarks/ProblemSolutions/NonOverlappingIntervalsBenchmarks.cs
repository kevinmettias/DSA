using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NonOverlappingIntervals;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NonOverlappingIntervalsSolution's, the same
// methods NonOverlappingIntervalsTests proves correct. _intervals is generated
// as mostly non-overlapping, shuffled intervals (few removals needed), which
// hits brute force's true worst case - its outer "find the next kept interval"
// loop runs close to n times, each paying a full O(n) rescan - instead of the
// heavy-overlap case where most intervals get eliminated in the first few
// rounds. SortByEndThenGreedyScan clones _intervals per invocation because the
// sort strategy mutates its input in place and each BenchmarkDotNet iteration
// must start from the same unsorted, shuffled workload.
[MemoryDiagnoser]
public class NonOverlappingIntervalsBenchmarks
{
    // LC 435.
    private const int RandomSeed = 435;
    private const int IntervalSpacing = 3;
    private const int EndOffsetUpperBound = 2;

    [Params(200, 3_000)]
    public int Length;

    private (int Start, int End)[] _intervals = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _intervals = Enumerable.Range(0, Length)
            .Select(i => (Start: i * IntervalSpacing, End: i * IntervalSpacing + random.Next(0, EndOffsetUpperBound)))
            .OrderBy(_ => random.Next())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRepeatedMinEndScan() =>
        NonOverlappingIntervalsSolution.EraseOverlapIntervalsByBruteForce(_intervals);

    [Benchmark]
    public int SortByEndThenGreedyScan() =>
        NonOverlappingIntervalsSolution.EraseOverlapIntervalsBySortThenGreedy(((int Start, int End)[])_intervals.Clone());
}
