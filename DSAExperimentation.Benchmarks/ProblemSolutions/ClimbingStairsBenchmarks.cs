using DSAExperimentation.LeetCode.ClimbingStairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClimbingStairsSolution's, the same methods
// ClimbingStairsTests proves correct. The two answer the same recurrence by
// different means - one through Memoizer's dictionary plus a call stack as deep
// as the step count, one through two running totals - so the ratio shows what the
// memoized shape actually costs over the flattened one.
public class ClimbingStairsBenchmarks
{
    [Params(10, 25, 40)]
    public int StepCount { get; set; }

    [Benchmark(Baseline = true)]
    public int IterativeRollingTotals() =>
        ClimbingStairsSolution.CountWaysByIterativeRollingTotals(StepCount);

    [Benchmark]
    public int MemoizedRecurrence() =>
        ClimbingStairsSolution.CountWaysByMemoizedRecurrence(StepCount);
}
