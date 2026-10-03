using DSAExperimentation.LeetCode.CountAllPossibleRoutes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountAllPossibleRoutesSolution's, the same strategies
// CountAllPossibleRoutesSolutionTests proves correct. The fuel is the whole workload, so
// there is nothing to build in [GlobalSetup].
//
// Sizes are per arm. The naive side branches up to Locations.Length - 1 ways per step,
// so it stops at 12 units of fuel; the memoized O(Locations.Length^2 * Fuel) side runs on
// to LC 1575's own bound of 200, and the two are compared at the fuel both run.
public class CountAllPossibleRoutesBenchmarks
{
    private const int Start = 0;
    private const int Finish = 3;
    private static readonly int[] Locations = [0, 1, 2, 3];

    public static IEnumerable<int> NaiveSizes => [8, 12];

    public static IEnumerable<int> MemoizedSizes => [.. NaiveSizes, 50, 200];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(NaiveSizes))]
    public int NaiveRecursion(int fuel) =>
        CountAllPossibleRoutesSolution.CountRoutesByNaiveRecursion(Locations, Start, Finish, fuel);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedTopDown(int fuel) =>
        CountAllPossibleRoutesSolution.CountRoutesByMemoizedRecurrence(Locations, Start, Finish, fuel);
}
