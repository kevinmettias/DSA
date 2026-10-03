using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.HouseRobberII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HouseRobberIISolution's, the same methods
// HouseRobberIISolutionTests proves correct. The previous class was a compile-smoke
// placeholder (`=> 1` on both arms) that measured nothing; this measures the
// memoized circular split against the iterative two-pass split over a circle whose
// values are drawn from a seeded Random across LC 213's [0, 1000]. Length stops at
// LC 213's 100-house cap, which keeps even the largest circle to microseconds.
public class HouseRobberIIBenchmarks
{
    private const int RandomSeed = 213; // LC problem number
    private const int MaxValue = 1_000;

    private int[] _nums = [];

    [Params(10, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = SeededDraws.Values(Length, 0, MaxValue + 1, new Random(RandomSeed));

    [Benchmark(Baseline = true)]
    public int MemoizedRecursion() => HouseRobberIISolution.RobByMemoizedRecursion(_nums);

    [Benchmark]
    public int IterativeTwoPass() => HouseRobberIISolution.RobByIterativeTwoPass(_nums);
}
