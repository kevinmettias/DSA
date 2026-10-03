using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ShortestSubarrayToBeRemovedToMakeArraySorted;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ShortestSubarrayToBeRemovedToMakeArraySortedSolution's,
// the same methods ShortestSubarrayToBeRemovedToMakeArraySortedSolutionTests proves correct.
// The workload is a deterministic random array - almost never sorted anywhere, so
// the cubic arm pays its full price and the stitching arm's prefix and suffix are
// both short; [GlobalSetup] owns its construction.
public class ShortestSubarrayToBeRemovedToMakeArraySortedBenchmarks
{
    // LC problem number, reused as the deterministic element seed.
    private const int RandomSeed = 1574;
    private const int MaxElementValue = 1_000;

    private int[] _arr = [];

    [Params(80, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = SeededDraws.Values(Length, 1, MaxElementValue, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() =>
        ShortestSubarrayToBeRemovedToMakeArraySortedSolution.FindLengthOfShortestSubarrayByBruteForce(_arr);

    [Benchmark]
    public int TwoPointerBinarySearch() =>
        ShortestSubarrayToBeRemovedToMakeArraySortedSolution.FindLengthOfShortestSubarrayByBinarySearchStitch(_arr);
}
