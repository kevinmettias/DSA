using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountAllPossibleRoutes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountAllPossibleRoutesSolution's, the same strategies
// CountAllPossibleRoutesTests proves correct. Fuel stays small enough that the naive
// side's branching (up to Locations.Length - 1 per step) still finishes in reasonable
// time while remaining clearly exponential next to the memoized
// O(Locations.Length^2 * Fuel) side.
[MemoryDiagnoser]
public class CountAllPossibleRoutesBenchmarks
{
    private const int Start = 0;
    private const int Finish = 3;
    private static readonly int[] Locations = [0, 1, 2, 3];

    [Params(8, 12)]
    public int Fuel { get; set; }

    [Benchmark(Baseline = true)]
    public int NaiveRecursion() =>
        CountAllPossibleRoutesSolution.CountRoutesByNaiveRecursion(Locations, Start, Finish, Fuel);

    [Benchmark]
    public int MemoizedTopDown() =>
        CountAllPossibleRoutesSolution.CountRoutesByMemoizedRecurrence(Locations, Start, Finish, Fuel);
}
