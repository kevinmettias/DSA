using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DistinctPrimeFactorsOfProductOfArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistinctPrimeFactorsOfProductOfArraySolution's, the
// same methods DistinctPrimeFactorsOfProductOfArrayTests proves correct. Forming
// the actual BigInteger product first (limb count grows with every multiply, the
// same O(n^2) blowup shape AddTwoNumbersBenchmarks' BigInteger baseline hits) and
// then trial-dividing that huge number, vs. trial-dividing each (small, <= 1000)
// element on its own and deduping through this repo's own Set<int> - the product's
// prime factors are exactly the union of each element's, so the product itself
// never needs to be built.
[MemoryDiagnoser]
public class DistinctPrimeFactorsOfProductOfArrayBenchmarks
{
    private const int MaxValueInclusive = 1_000;
    private const int Seed = 2521; // LC problem number

    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(2, MaxValueInclusive + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ProductThenTrialDivide() =>
        DistinctPrimeFactorsOfProductOfArraySolution.DistinctPrimeFactorsByProductTrialDivision(_values);

    [Benchmark]
    public int PerElementFactorSet() =>
        DistinctPrimeFactorsOfProductOfArraySolution.DistinctPrimeFactorsByElementFactorSet(_values);
}
