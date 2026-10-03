using DSAExperimentation.LeetCode.CombinationSumIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CombinationSumIVSolution's, the same methods
// CombinationSumIVSolutionTests proves correct. LC 377 promises an answer that fits a
// 32-bit int, and over these nums the count first passes int.MaxValue at a target of 35
// (2,719,190,965), so Target stops at 34 (1,438,436,011) although LC 377 allows 1,000;
// past that, both arms would overflow to the same wrong number and still agree.
public class CombinationSumIVBenchmarks
{
    private static readonly int[] Nums = [1, 2, 3, 5, 10];

    [Params(10, 34)]
    public int Target { get; set; }

    [Benchmark(Baseline = true)]
    public int Tabulation() => CombinationSumIVSolution.CountCombinationsByTabulation(Nums, Target);

    [Benchmark]
    public int Memoized() => CombinationSumIVSolution.CountCombinationsByMemoizedRecursion(Nums, Target);
}
