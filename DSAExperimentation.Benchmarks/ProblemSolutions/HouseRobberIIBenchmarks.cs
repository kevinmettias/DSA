using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HouseRobberII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is HouseRobberIISolution's. The previous class was a
// compile-smoke placeholder (`=> 1` on both arms) that measured nothing; this
// measures the actual memoized recursion against LeetCode's own example.
[MemoryDiagnoser]
public class HouseRobberIIBenchmarks
{
    private static readonly int[] Nums = [1, 2, 3, 1];

    [Benchmark]
    public int MemoizedRecursion() => HouseRobberIISolution.RobByMemoizedRecursion(Nums);
}
