using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountWaysToMakeArrayWithProduct;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountWaysToMakeArrayWithProductSolution's, the same
// methods CountWaysToMakeArrayWithProductTests proves correct. Trial division
// re-pays O(sqrt(k)) on every single query; the shared smallest-prime-factor sieve
// pays O(maxK log log maxK) once and amortizes it across QueryCount queries, so the
// sieve build stays inside the measured method - it is the cost being amortized,
// not setup.
[MemoryDiagnoser]
public class CountWaysToMakeArrayWithProductBenchmarks
{
    // Inclusive upper bound for the generated product k in each query.
    private const int MaxK = 10_000;

    // Seeds query generation; not the LC problem number here.
    private const int RandomSeed = 6;

    // Exclusive upper bound for the generated array length n in each query.
    private const int MaxArrayLength = 50;

    [Params(200, 2_000)]
    public int QueryCount;

    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => new[] { random.Next(1, MaxArrayLength), random.Next(1, MaxK + 1) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] TrialDivisionPerQuery() =>
        CountWaysToMakeArrayWithProductSolution.WaysToFillArrayByTrialDivision(_queries);

    [Benchmark]
    public int[] SmallestPrimeFactorSieve() =>
        CountWaysToMakeArrayWithProductSolution.WaysToFillArrayBySmallestPrimeFactorSieve(_queries);
}
