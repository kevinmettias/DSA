using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountOfSmallerNumbersAfterSelf;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountOfSmallerNumbersAfterSelfSolution's, the same
// methods CountOfSmallerNumbersAfterSelfSolutionTests proves correct.
public class CountOfSmallerNumbersAfterSelfBenchmarks
{
    private const int RandomSeed = 315; private int[] _nums = [];

    // LeetCode problem number

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = CountOfSmallerNumbersAfterSelfWorkloads.BuildNums(Length, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int[] PairwiseScan() => CountOfSmallerNumbersAfterSelfSolution.CountSmallerByPairwiseScan(_nums);

    [Benchmark]
    public int[] FenwickTreeSweep() => CountOfSmallerNumbersAfterSelfSolution.CountSmallerByFenwickTreeSweep(_nums);
}
