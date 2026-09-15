using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.GCDSortOfAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are GCDSortOfAnArraySolution's, the same methods
// GCDSortOfAnArrayTests proves correct. Values are drawn once in [GlobalSetup] as products
// of a small shared prime pool so real shared-factor chains - and therefore real merge
// work - actually occur, the same generator intent LargestComponentSizeByCommonFactorBenchmarks
// uses. What is measured is the O(distinctValues^2) pairwise gcd sweep plus Array.Sort
// against the O(n*sqrt(maxValue)) per-factor union plus this repo's own MergeSort.
[MemoryDiagnoser]
public class GCDSortOfAnArrayBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1998;

    private static readonly int[] SharedPrimes = [2, 3, 5, 7, 11, 13];

    private int[] _values = [];

    [Params(50, 400)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length)
            .Select(_ => SharedPrimes[random.Next(SharedPrimes.Length)] * SharedPrimes[random.Next(SharedPrimes.Length)])
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool PairwiseGcdUnionFind() =>
        GCDSortOfAnArraySolution.CanBeSortedByPairwiseGcdUnionFind(_values);

    [Benchmark]
    public bool DisjointSetByPrimeFactor() =>
        GCDSortOfAnArraySolution.CanBeSortedByPrimeFactorDisjointSet(_values);
}
