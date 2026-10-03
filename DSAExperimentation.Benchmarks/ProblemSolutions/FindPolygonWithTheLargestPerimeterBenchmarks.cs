using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FindPolygonWithTheLargestPerimeter;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindPolygonWithTheLargestPerimeterSolution's,
// the same methods FindPolygonWithTheLargestPerimeterSolutionTests proves correct.
// Neither strategy needs anything hoisted beyond the raw int[] LeetCode
// itself hands in, so [GlobalSetup] only sizes+seeds the workload.
//
// Sizes are per arm. The brute-force arm enumerates every subset, so it stops at 20
// sides; the sorted running sum is O(n log n) and runs on to LC 2971's own bound of
// 10^5, and the two are compared at the counts both run.
public class FindPolygonWithTheLargestPerimeterBenchmarks
{
    private const int SideSeed = 2971;

    private Dictionary<int, int[]> _sidesByCount = [];

    public static IEnumerable<int> BruteForceSizes => [16, 20];

    public static IEnumerable<int> SortedRunningSumSizes => [.. BruteForceSizes, 1_000, 100_000];

    // Every side count any arm runs is drawn here, outside the timed region; an arm looks
    // its own up.
    [GlobalSetup]
    public void Setup() =>
        _sidesByCount = SortedRunningSumSizes.ToDictionary(
            count => count,
            count => PolygonWorkloads.BuildSides(count, seed: SideSeed));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForceSubsets(int sideCount) =>
        FindPolygonWithTheLargestPerimeterSolution.LargestPerimeterByBruteForceSubsets(_sidesByCount[sideCount]);

    [Benchmark]
    [ArgumentsSource(nameof(SortedRunningSumSizes))]
    public long SortedRunningSum(int sideCount) =>
        FindPolygonWithTheLargestPerimeterSolution.LargestPerimeterBySortedRunningSum(_sidesByCount[sideCount]);
}
