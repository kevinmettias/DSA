using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ParallelCoursesII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ParallelCoursesIISolution's, the same methods
// ParallelCoursesIITests proves correct. The workload has zero prerequisites, so
// every not-yet-taken course is always "ready" - the same "force the real worst
// case" shape CanIWinBenchmarks already uses, here maximizing how many different
// semester orders can reach the same completed-course mask and therefore how much
// the unmemoized arm re-explores.
[MemoryDiagnoser]
public class ParallelCoursesIIBenchmarks
{
    private const int K = 2;

    [Params(6, 8)]
    public int CourseCount;

    private int[][] _relations = null!;

    [GlobalSetup]
    public void Setup() => _relations = [];

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        ParallelCoursesIISolution.MinNumberOfSemestersByBruteForceRecursion(CourseCount, _relations, K);

    [Benchmark]
    public int MemoizedRecursion() =>
        ParallelCoursesIISolution.MinNumberOfSemestersByMemoizedRecursion(CourseCount, _relations, K);
}
