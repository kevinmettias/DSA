using DSAExperimentation.LeetCode.ParallelCoursesII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ParallelCoursesIISolution's, the same methods
// ParallelCoursesIISolutionTests proves correct. The workload has zero prerequisites, so
// every not-yet-taken course is always "ready" - the same "force the real worst
// case" shape CanIWinBenchmarks already uses, here maximizing how many different
// semester orders can reach the same completed-course mask and therefore how much
// the unmemoized arm re-explores.
//
// Sizes are per arm, counted in courses. The unmemoized arm re-solves a mask once per
// semester order that reaches it, so it stops at 8; the memoized arm solves each of the
// 2^n masks once, at ~3^n submask steps in all, and runs on to LC 1494's own bound of
// 15. The two are compared at the sizes both run.
public class ParallelCoursesIIBenchmarks
{
    private const int MaxPerSemester = 2;

    private int[][] _relations = [];

    public static IEnumerable<int> BaselineSizes => [6, 8];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 12, 15];

    // No prerequisites at any size, so the one workload serves every course count.
    [GlobalSetup]
    public void Setup() => _relations = [];

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForceRecursion(int courseCount) =>
        ParallelCoursesIISolution.MinNumberOfSemestersByBruteForceRecursion(
            courseCount, _relations, MaxPerSemester);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int MemoizedRecursion(int courseCount) =>
        ParallelCoursesIISolution.MinNumberOfSemestersByMemoizedRecursion(
            courseCount, _relations, MaxPerSemester);
}
