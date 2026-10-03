using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.HouseRobber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HouseRobberSolution's, the same methods
// HouseRobberSolutionTests proves correct. The previous class was a compile-smoke
// placeholder (`=> 1` on both arms) that measured nothing; this measures the
// memoized recurrence against the rolling-totals pass over a street whose values
// are drawn from a seeded Random across LC 198's [0, 400]. Length stops at LC
// 198's 100-house cap, which keeps even the largest street to microseconds.
public class HouseRobberBenchmarks
{
    private const int RandomSeed = 198; // LC problem number
    private const int MaxValue = 400;

    private int[] _nums = [];

    [Params(10, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = SeededDraws.Values(Length, 0, MaxValue + 1, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public int MemoizedRecursion() => HouseRobberSolution.RobByMemoizedRecursion(_nums);

    [Benchmark]
    public int IterativeRollingTotals() => HouseRobberSolution.RobByIterativeRollingTotals(_nums);
}
