using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindBuildingWhereAliceAndBobCanMeet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindBuildingWhereAliceAndBobCanMeetSolution's, the
// same methods FindBuildingWhereAliceAndBobCanMeetTests proves correct. Neither
// strategy needs a prepared representation beyond LeetCode's own heights/queries
// shape - unlike a strategy built on a reusable graph or a sorted array, the
// offline heap sweep's own "preparation" (bucketing each query at its hi index)
// IS the algorithm, not a separable setup cost, so nothing here is hoisted past
// [GlobalSetup] the way OpenTheLockBenchmarks hoists LockGraph.Build.
[MemoryDiagnoser]
public class FindBuildingWhereAliceAndBobCanMeetBenchmarks
{
    private const int RandomSeed = 2940;
    private const int MaxHeightExclusive = 1_000_000;

    private int[] _heights = [];

    private int[][] _queries = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heights = SeededDraws.Values(Length, 1, MaxHeightExclusive, random);
        _queries = SeededDraws.Pairs(Length, 0, Length, random);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        FindBuildingWhereAliceAndBobCanMeetSolution.FindMeetingBuildingsByBruteForce(_heights, _queries);

    [Benchmark]
    public int[] OfflineHeapSweep() =>
        FindBuildingWhereAliceAndBobCanMeetSolution.FindMeetingBuildingsByOfflineHeapSweep(_heights, _queries);
}
