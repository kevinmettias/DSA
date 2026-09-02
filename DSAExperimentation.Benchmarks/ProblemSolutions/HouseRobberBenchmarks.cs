using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.HouseRobber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is HouseRobberSolution's. The previous class was
// a compile-smoke placeholder (`=> 1` on both arms) that measured nothing;
// this measures the actual memoized recursion against LeetCode's own example.
[MemoryDiagnoser]
public class HouseRobberBenchmarks
{
    private static readonly int[] Nums = [2, 7, 9, 3, 1];

    [Benchmark]
    public int MemoizedRecursion() => HouseRobberSolution.RobByMemoizedRecursion(Nums);
}
