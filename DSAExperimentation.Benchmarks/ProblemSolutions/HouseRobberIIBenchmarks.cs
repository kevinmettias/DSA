using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HouseRobberII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are HouseRobberIISolution's, the same methods
// HouseRobberIITests proves correct. The previous class was a compile-smoke
// placeholder (`=> 1` on both arms) that measured nothing; this measures the
// memoized circular split against the iterative two-pass split, both over
// LeetCode's own example circle.
[MemoryDiagnoser]
public class HouseRobberIIBenchmarks
{
    private static readonly int[] Nums = [1, 2, 3, 1];

    [Benchmark(Baseline = true)]
    public int MemoizedRecursion() => HouseRobberIISolution.RobByMemoizedRecursion(Nums);

    [Benchmark]
    public int IterativeTwoPass() => HouseRobberIISolution.RobByIterativeTwoPass(Nums);
}
