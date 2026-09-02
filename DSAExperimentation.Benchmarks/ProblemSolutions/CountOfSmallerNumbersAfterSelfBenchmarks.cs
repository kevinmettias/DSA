using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CountOfSmallerNumbersAfterSelf;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountOfSmallerNumbersAfterSelfSolution's, the same
// methods CountOfSmallerNumbersAfterSelfTests proves correct.
[MemoryDiagnoser]
public class CountOfSmallerNumbersAfterSelfBenchmarks
{
    private const int RandomSeed = 315; // LeetCode problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup() => _nums = CountOfSmallerNumbersAfterSelfWorkloads.BuildNums(Length, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public int[] PairwiseScan() => CountOfSmallerNumbersAfterSelfSolution.CountSmallerByPairwiseScan(_nums);

    [Benchmark]
    public int[] FenwickTreeSweep() => CountOfSmallerNumbersAfterSelfSolution.CountSmallerByFenwickTreeSweep(_nums);
}
