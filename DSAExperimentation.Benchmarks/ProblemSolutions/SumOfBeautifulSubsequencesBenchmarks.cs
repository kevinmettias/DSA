using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SumOfBeautifulSubsequences;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfBeautifulSubsequencesSolution's, the same
// methods SumOfBeautifulSubsequencesTests proves correct. Neither strategy needs
// anything prepared beyond the array itself, so [GlobalSetup] only charges
// workload construction.
[MemoryDiagnoser]
public class SumOfBeautifulSubsequencesBenchmarks
{
    private const int Seed = 3671;

    private int[] _nums = [];

    [Params(12, 16)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup() => _nums = SumOfBeautifulSubsequencesWorkloads.BuildNums(Size, Seed);

    [Benchmark(Baseline = true)]
    public int BruteForceBitmask() => SumOfBeautifulSubsequencesSolution.SumBeautyByBruteForce(_nums);

    [Benchmark]
    public int DivisorSieve() => SumOfBeautifulSubsequencesSolution.SumBeautyByDivisorSieve(_nums);
}
