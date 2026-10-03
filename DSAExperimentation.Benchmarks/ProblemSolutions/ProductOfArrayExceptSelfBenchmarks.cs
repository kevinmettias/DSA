using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ProductOfArrayExceptSelf;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is ProductOfArrayExceptSelfSolution's, the same
// method ProductOfArrayExceptSelfSolutionTests proves correct. The prior benchmark was
// a compile-smoke placeholder with no real workload; this backfills it with the
// prefix/suffix pass over a random array of negative and positive values in LC 238's
// [-30, 30]. BoundedProductDraws plants them so the product of every value fits in an
// int, which keeps each answer[i] inside the 32 bits LC 238 guarantees.
public class ProductOfArrayExceptSelfBenchmarks
{
    private const int Seed = 238;
    private const int MagnitudeBoundExclusive = 31;

    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _nums = BoundedProductDraws.SignedFactors(Length, MagnitudeBoundExclusive, new Random(Seed));

    [Benchmark]
    public int[] PrefixSuffixPass() =>
        ProductOfArrayExceptSelfSolution.ProductExceptSelfByPrefixSuffixPass(_nums);
}
