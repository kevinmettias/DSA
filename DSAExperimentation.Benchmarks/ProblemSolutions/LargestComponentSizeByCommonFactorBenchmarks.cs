using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LargestComponentSizeByCommonFactor;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LargestComponentSizeByCommonFactorSolution's, the same
// methods LargestComponentSizeByCommonFactorTests proves correct. Values are drawn once
// in [GlobalSetup] as products of a small shared prime pool, so real overlaps - and
// therefore real merge work - actually occur, the same "force genuine matches, not
// coincidental ones" intent AccountsMergeBenchmarks' own generator uses. What is
// measured is the O(n^2) pairwise gcd sweep against the O(n*sqrt(maxValue)) per-factor
// union.
[MemoryDiagnoser]
public class LargestComponentSizeByCommonFactorBenchmarks
{
    private static readonly int[] SharedPrimes = [2, 3, 5, 7, 11, 13];

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 952;

    [Params(50, 400)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length)
            .Select(_ => SharedPrimes[random.Next(SharedPrimes.Length)] * SharedPrimes[random.Next(SharedPrimes.Length)])
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseGcdScan() =>
        LargestComponentSizeByCommonFactorSolution.LargestComponentSizeByPairwiseGcd(_values);

    [Benchmark]
    public int DisjointSetByPrimeFactor() =>
        LargestComponentSizeByCommonFactorSolution.LargestComponentSizeByPrimeFactorUnion(_values);
}
