using DSAExperimentation.LeetCode.HouseRobber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HouseRobberSolution's, the same methods
// HouseRobberSolutionTests proves correct. The previous class was a compile-smoke
// placeholder (`=> 1` on both arms) that measured nothing; this measures the
// memoized recurrence against the rolling-totals pass, both over LeetCode's
// own example street.
public class HouseRobberBenchmarks
{
    private static readonly int[] Nums = [2, 7, 9, 3, 1];

    [Benchmark(Baseline = true)]
    public int MemoizedRecursion() => HouseRobberSolution.RobByMemoizedRecursion(Nums);

    [Benchmark]
    public int IterativeRollingTotals() => HouseRobberSolution.RobByIterativeRollingTotals(Nums);
}
